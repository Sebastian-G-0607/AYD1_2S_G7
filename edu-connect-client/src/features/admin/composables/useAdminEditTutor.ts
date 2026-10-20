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
    genero: null,
    fechaNacimiento: null,
    direccion: null,
    direccionTutoria: null,
    universidad: null,
    anioInicio: null,
    materias: null,
    telefono: null
  })

  const currentYear = new Date().getFullYear()

  const maxBirthDate = computed(() => {
    const date = new Date()
    date.setFullYear(date.getFullYear() - 18)
    return date.toISOString().split('T')[0]
  })

  const minBirthDate = '1920-01-01'

  const canSave = computed(() => {
    return !isSaving.value
  })

  function clearFieldError(field: string) {
    if (fieldErrors[field]) {
      fieldErrors[field] = null
    }
    clearMessages()
  }

  function handleNombreInput(e: Event) {
    const target = e.target as HTMLInputElement
    const sanitized = target.value.replace(/[^a-zA-ZáéíóúüñÁÉÍÓÚÜÑ\s'-]/g, '').slice(0, 60)
    formData.nombre = sanitized
    target.value = sanitized
    clearFieldError('nombre')
  }

  function handleApellidoInput(e: Event) {
    const target = e.target as HTMLInputElement
    const sanitized = target.value.replace(/[^a-zA-ZáéíóúüñÁÉÍÓÚÜÑ\s'-]/g, '').slice(0, 60)
    formData.apellido = sanitized
    target.value = sanitized
    clearFieldError('apellido')
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
    const nameRegex = /^[a-zA-ZáéíóúüñÁÉÍÓÚÜÑ\s'-]+$/

    if (!formData.nombre.trim()) {
      fieldErrors.nombre = 'El nombre del tutor es obligatorio.'
      isValid = false
    } else if (formData.nombre.trim().length < 2) {
      fieldErrors.nombre = 'El nombre debe tener al menos 2 caracteres.'
      isValid = false
    } else if (formData.nombre.trim().length > 60) {
      fieldErrors.nombre = 'El nombre no puede exceder los 60 caracteres.'
      isValid = false
    } else if (!nameRegex.test(formData.nombre.trim())) {
      fieldErrors.nombre = 'El nombre solo puede contener letras y espacios.'
      isValid = false
    }

    if (!formData.apellido.trim()) {
      fieldErrors.apellido = 'El apellido del tutor es obligatorio.'
      isValid = false
    } else if (formData.apellido.trim().length < 2) {
      fieldErrors.apellido = 'El apellido debe tener al menos 2 caracteres.'
      isValid = false
    } else if (formData.apellido.trim().length > 60) {
      fieldErrors.apellido = 'El apellido no puede exceder los 60 caracteres.'
      isValid = false
    } else if (!nameRegex.test(formData.apellido.trim())) {
      fieldErrors.apellido = 'El apellido solo puede contener letras y espacios.'
      isValid = false
    }

    if (
      !formData.carnetId.trim() ||
      formData.carnetId.length < 6 ||
      formData.carnetId.length > 10
    ) {
      fieldErrors.carnetId = 'El carnet o ID debe ser numérico y contener entre 6 y 10 dígitos.'
      isValid = false
    } else if (/^0+$/.test(formData.carnetId.trim())) {
      fieldErrors.carnetId = 'El carnet no puede estar compuesto únicamente por ceros.'
      isValid = false
    }

    if (!formData.numeroIdentificacion.trim() || formData.numeroIdentificacion.length !== 13) {
      fieldErrors.numeroIdentificacion =
        'El documento de identificación (DPI) debe contener exactamente 13 dígitos numéricos.'
      isValid = false
    } else if (/^0+$/.test(formData.numeroIdentificacion.trim())) {
      fieldErrors.numeroIdentificacion =
        'El documento de identificación no puede estar compuesto únicamente por ceros.'
      isValid = false
    }

    if (!formData.genero || !['masculino', 'femenino'].includes(formData.genero.toLowerCase())) {
      fieldErrors.genero = 'Debe seleccionar un género válido.'
      isValid = false
    }

    if (!formData.fechaNacimiento) {
      fieldErrors.fechaNacimiento = 'La fecha de nacimiento es obligatoria.'
      isValid = false
    } else {
      const parts = formData.fechaNacimiento.split('-')
      if (parts.length !== 3) {
        fieldErrors.fechaNacimiento = 'El formato de fecha no es válido.'
        isValid = false
      } else {
        const year = Number(parts[0])
        const month = Number(parts[1])
        const day = Number(parts[2])

        if (
          isNaN(year) ||
          isNaN(month) ||
          isNaN(day) ||
          month < 1 ||
          month > 12 ||
          day < 1 ||
          day > 31
        ) {
          fieldErrors.fechaNacimiento =
            'La fecha de nacimiento contiene valores numéricos de día o mes inválidos.'
          isValid = false
        } else {
          const testDate = new Date(year, month - 1, day)
          if (
            testDate.getFullYear() !== year ||
            testDate.getMonth() !== month - 1 ||
            testDate.getDate() !== day
          ) {
            fieldErrors.fechaNacimiento = 'La fecha no corresponde a un día de calendario válido.'
            isValid = false
          } else {
            const today = new Date()
            if (testDate > today) {
              fieldErrors.fechaNacimiento = 'La fecha de nacimiento no puede ser una fecha futura.'
              isValid = false
            } else if (year < 1920) {
              fieldErrors.fechaNacimiento = 'El año de nacimiento no puede ser anterior a 1920.'
              isValid = false
            } else {
              let age = today.getFullYear() - year
              const monthDiff = today.getMonth() - (month - 1)
              if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < day)) {
                age--
              }
              if (age < 18) {
                fieldErrors.fechaNacimiento = 'El tutor debe ser mayor de 18 años.'
                isValid = false
              }
            }
          }
        }
      }
    }

    if (!formData.direccion.trim()) {
      fieldErrors.direccion = 'La dirección de residencia es obligatoria.'
      isValid = false
    } else if (formData.direccion.trim().length < 5) {
      fieldErrors.direccion = 'La dirección de residencia debe contener al menos 5 caracteres.'
      isValid = false
    } else if (formData.direccion.trim().length > 200) {
      fieldErrors.direccion = 'La dirección de residencia no puede exceder los 200 caracteres.'
      isValid = false
    }

    if (!formData.direccionTutoria.trim()) {
      fieldErrors.direccionTutoria = 'La dirección de tutoría o modalidad es obligatoria.'
      isValid = false
    } else if (formData.direccionTutoria.trim().length < 5) {
      fieldErrors.direccionTutoria =
        'La dirección o modalidad de tutoría debe contener al menos 5 caracteres.'
      isValid = false
    } else if (formData.direccionTutoria.trim().length > 200) {
      fieldErrors.direccionTutoria = 'La dirección de tutoría no puede exceder los 200 caracteres.'
      isValid = false
    }

    if (!formData.universidad.trim()) {
      fieldErrors.universidad = 'La universidad de graduación es obligatoria.'
      isValid = false
    } else if (formData.universidad.trim().length < 3) {
      fieldErrors.universidad = 'El nombre de la universidad debe contener al menos 3 caracteres.'
      isValid = false
    } else if (formData.universidad.trim().length > 100) {
      fieldErrors.universidad = 'El nombre de la universidad no puede exceder los 100 caracteres.'
      isValid = false
    } else if (!/[a-zA-ZáéíóúüñÁÉÍÓÚÜÑ]/.test(formData.universidad.trim())) {
      fieldErrors.universidad = 'El nombre de la universidad debe contener texto válido.'
      isValid = false
    }

    if (!formData.anioInicio || isNaN(Number(formData.anioInicio))) {
      fieldErrors.anioInicio = 'El año de inicio de tutorías es obligatorio.'
      isValid = false
    } else {
      const anioNum = Number(formData.anioInicio)
      if (anioNum < 1980 || anioNum > currentYear) {
        fieldErrors.anioInicio = `El año de inicio debe ser entre 1980 y ${currentYear}.`
        isValid = false
      } else if (formData.fechaNacimiento) {
        const birthYear = Number(formData.fechaNacimiento.split('-')[0])
        if (!isNaN(birthYear) && anioNum < birthYear + 18) {
          fieldErrors.anioInicio = `El año de inicio (${anioNum}) no puede ser previo a la mayoría de edad del tutor (${birthYear + 18}).`
          isValid = false
        }
      }
    }

    if (formData.selectedMaterias.length === 0) {
      fieldErrors.materias = 'Debe asignar al menos una materia al tutor.'
      isValid = false
    }

    if (formData.telefono.trim()) {
      if (formData.telefono.trim().length !== 8) {
        fieldErrors.telefono = 'El teléfono debe contener exactamente 8 dígitos numéricos.'
        isValid = false
      } else if (!/^[2-8]\d{7}$/.test(formData.telefono.trim())) {
        fieldErrors.telefono = 'El teléfono debe iniciar con un dígito válido (2 al 8).'
        isValid = false
      }
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
      errorMessage.value = extractApiErrorMessage(
        err,
        'No fue posible cargar la información del tutor.'
      )
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
    maxBirthDate,
    minBirthDate,
    handleNombreInput,
    handleApellidoInput,
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
