import { ref, reactive, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { adminService } from '../services/admin.service'
import { useMaterias } from '@/composables/useMaterias'
import { extractApiErrorMessage } from '@/utils/apiError'
import type { ActiveTutorItem, UpdateTutorAdminPayload } from '../types'

export function useAdminEditTutor() {
  const route = useRoute()
  const router = useRouter()
  const tutorId = Number(route.params.id)

  const isLoading = ref(true)
  const isSaving = ref(false)
  const errorMessage = ref<string | null>(null)
  const successMessage = ref<string | null>(null)
  const tutorOriginal = ref<ActiveTutorItem | null>(null)

  const { materias: availableMaterias, isLoading: isLoadingMaterias } = useMaterias()

  const formData = reactive({
    nombre: '',
    apellido: '',
    carnetId: '',
    numeroIdentificacion: '',
    genero: 'masculino',
    fechaNacimiento: '',
    correo: '',
    telefono: '',
    direccion: '',
    direccionTutoria: '',
    anioInicio: new Date().getFullYear(),
    universidad: '',
    fotografiaUrl: '',
    selectedMaterias: [] as string[]
  })

  const fieldErrors = reactive<Record<string, string | null>>({
    nombre: null,
    apellido: null,
    carnetId: null,
    numeroIdentificacion: null,
    fechaNacimiento: null,
    direccion: null,
    direccionTutoria: null,
    universidad: null,
    anioInicio: null,
    materias: null,
    telefono: null
  })

  const currentYear = new Date().getFullYear()

  const canSave = computed(() => {
    return !isSaving.value
  })

  function clearFieldError(field: string) {
    if (fieldErrors[field]) {
      fieldErrors[field] = null
    }
    clearMessages()
  }

  function handleCarnetInput(e: Event) {
    const target = e.target as HTMLInputElement
    formData.carnetId = target.value.replace(/\D/g, '').slice(0, 10)
    clearFieldError('carnetId')
  }

  function handleDpiInput(e: Event) {
    const target = e.target as HTMLInputElement
    formData.numeroIdentificacion = target.value.replace(/\D/g, '').slice(0, 13)
    clearFieldError('numeroIdentificacion')
  }

  function handlePhoneInput(e: Event) {
    const target = e.target as HTMLInputElement
    formData.telefono = target.value.replace(/\D/g, '').slice(0, 8)
    clearFieldError('telefono')
  }

  function toggleMateria(materiaNombre: string) {
    const index = formData.selectedMaterias.indexOf(materiaNombre)
    if (index > -1) {
      formData.selectedMaterias.splice(index, 1)
    } else {
      formData.selectedMaterias.push(materiaNombre)
    }
    clearFieldError('materias')
  }

  function isMateriaSelected(materiaNombre: string): boolean {
    return formData.selectedMaterias.includes(materiaNombre)
  }

  function clearMessages() {
    errorMessage.value = null
    successMessage.value = null
  }

  function validateForm(): boolean {
    Object.keys(fieldErrors).forEach(key => {
      fieldErrors[key] = null
    })

    let isValid = true

    if (!formData.nombre.trim()) {
      fieldErrors.nombre = 'El nombre del tutor es obligatorio.'
      isValid = false
    }

    if (!formData.apellido.trim()) {
      fieldErrors.apellido = 'El apellido del tutor es obligatorio.'
      isValid = false
    }

    if (!formData.carnetId.trim() || formData.carnetId.length < 6 || formData.carnetId.length > 10) {
      fieldErrors.carnetId = 'El carnet o ID debe ser numérico y contener entre 6 y 10 dígitos.'
      isValid = false
    }

    if (!formData.numeroIdentificacion.trim() || formData.numeroIdentificacion.length !== 13) {
      fieldErrors.numeroIdentificacion = 'El documento de identificación (DPI) debe contener exactamente 13 dígitos numéricos.'
      isValid = false
    }

    if (!formData.fechaNacimiento) {
      fieldErrors.fechaNacimiento = 'La fecha de nacimiento es obligatoria.'
      isValid = false
    }

    if (!formData.direccion.trim()) {
      fieldErrors.direccion = 'La dirección de residencia es obligatoria.'
      isValid = false
    }

    if (!formData.direccionTutoria.trim()) {
      fieldErrors.direccionTutoria = 'La dirección de tutoría o modalidad es obligatoria.'
      isValid = false
    }

    if (!formData.universidad.trim()) {
      fieldErrors.universidad = 'La universidad de graduación es obligatoria.'
      isValid = false
    }

    if (!formData.anioInicio || formData.anioInicio < 1980 || formData.anioInicio > currentYear) {
      fieldErrors.anioInicio = `El año de inicio debe ser entre 1980 y ${currentYear}.`
      isValid = false
    }

    if (formData.selectedMaterias.length === 0) {
      fieldErrors.materias = 'Debe asignar al menos una materia al tutor.'
      isValid = false
    }

    if (formData.telefono && formData.telefono.length !== 8) {
      fieldErrors.telefono = 'El teléfono debe contener 8 dígitos numéricos.'
      isValid = false
    }

    return isValid
  }

  async function loadTutorData() {
    if (!tutorId || isNaN(tutorId)) {
      errorMessage.value = 'El identificador del tutor es inválido.'
      isLoading.value = false
      return
    }

    isLoading.value = true
    clearMessages()
    try {
      const tutor = await adminService.getTutorById(tutorId)
      tutorOriginal.value = tutor

      formData.nombre = tutor.nombre || ''
      formData.apellido = tutor.apellido || ''
      formData.carnetId = tutor.carnetId || ''
      formData.numeroIdentificacion = tutor.numeroIdentificacion || ''
      formData.genero = (tutor.genero || 'masculino').toLowerCase()
      formData.fechaNacimiento = tutor.fechaNacimiento || ''
      formData.correo = tutor.correo || ''
      formData.telefono = tutor.telefono || ''
      formData.direccion = tutor.direccion || ''
      formData.direccionTutoria = tutor.direccionTutoria || ''
      formData.anioInicio = tutor.anioInicio || currentYear
      formData.universidad = tutor.universidad || ''
      formData.fotografiaUrl = tutor.fotografiaUrl || ''
      formData.selectedMaterias = tutor.materias ? [...tutor.materias] : []
    } catch (err) {
      errorMessage.value = extractApiErrorMessage(err, 'No fue posible cargar la información del tutor.')
    } finally {
      isLoading.value = false
    }
  }

  async function handleSave() {
    clearMessages()

    if (!validateForm()) {
      errorMessage.value = 'Por favor revise los campos resaltados en rojo antes de guardar.'
      return
    }

    isSaving.value = true
    try {
      const payload: UpdateTutorAdminPayload = {
        nombre: formData.nombre.trim(),
        apellido: formData.apellido.trim(),
        carnetId: formData.carnetId.trim(),
        numeroIdentificacion: formData.numeroIdentificacion.trim(),
        genero: formData.genero,
        fechaNacimiento: formData.fechaNacimiento,
        direccion: formData.direccion.trim(),
        telefono: formData.telefono.trim(),
        direccionTutoria: formData.direccionTutoria.trim(),
        anioInicio: Number(formData.anioInicio),
        universidad: formData.universidad.trim(),
        materias: formData.selectedMaterias,
        fotografiaUrl: formData.fotografiaUrl.trim() || undefined
      }

      const response = await adminService.updateTutor(tutorId, payload)
      successMessage.value = response.mensaje || 'Información del tutor actualizada correctamente.'

      // Actualizar datos locales
      if (tutorOriginal.value) {
        tutorOriginal.value.nombre = response.nombre
        tutorOriginal.value.apellido = response.apellido
        tutorOriginal.value.carnetId = response.carnetId
        tutorOriginal.value.numeroIdentificacion = response.numeroIdentificacion
        tutorOriginal.value.especialidad = response.especialidad
        tutorOriginal.value.materias = response.materias
      }
    } catch (err) {
      const msg = extractApiErrorMessage(err, 'Ocurrió un error al actualizar los datos del tutor.')
      errorMessage.value = msg

      const lowerMsg = msg.toLowerCase()
      if (lowerMsg.includes('carnet')) {
        fieldErrors.carnetId = msg
      }
      if (lowerMsg.includes('identificaci') || lowerMsg.includes('dpi')) {
        fieldErrors.numeroIdentificacion = msg
      }
      if (lowerMsg.includes('materia')) {
        fieldErrors.materias = msg
      }
    } finally {
      isSaving.value = false
    }
  }

  function goBack() {
    router.push('/admin/usuarios')
  }

  onMounted(() => {
    loadTutorData()
  })

  return {
    tutorId,
    formData,
    tutorOriginal,
    availableMaterias,
    isLoadingMaterias,
    isLoading,
    isSaving,
    errorMessage,
    successMessage,
    fieldErrors,
    canSave,
    currentYear,
    handleCarnetInput,
    handleDpiInput,
    handlePhoneInput,
    toggleMateria,
    isMateriaSelected,
    clearMessages,
    clearFieldError,
    validateForm,
    handleSave,
    goBack
  }
}
