import { ref, reactive, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { adminService } from '../services/admin.service'
import { extractApiErrorMessage } from '@/utils/apiError'
import type { ActiveStudentItem, UpdateStudentAdminPayload } from '../types'

export function useAdminEditStudent() {
  const route = useRoute()
  const router = useRouter()
  const studentId = Number(route.params.id)

  const isLoading = ref(true)
  const isSaving = ref(false)
  const errorMessage = ref<string | null>(null)
  const successMessage = ref<string | null>(null)
  const studentOriginal = ref<ActiveStudentItem | null>(null)

  const formData = reactive({
    nombre: '',
    apellido: '',
    carnet: '',
    genero: 'masculino',
    fechaNacimiento: '',
    correo: '',
    telefono: '',
    direccion: '',
    fotografiaUrl: '',
    documentoCarnetUrl: ''
  })

  const fieldErrors = reactive<Record<string, string | null>>({
    nombre: null,
    apellido: null,
    carnet: null,
    genero: null,
    fechaNacimiento: null,
    direccion: null,
    telefono: null
  })

  const maxBirthDate = computed(() => {
    return new Date().toISOString().split('T')[0]
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
    formData.carnet = target.value.replace(/\D/g, '').slice(0, 10)
    clearFieldError('carnet')
  }

  function handlePhoneInput(e: Event) {
    const target = e.target as HTMLInputElement
    formData.telefono = target.value.replace(/\D/g, '').slice(0, 8)
    clearFieldError('telefono')
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
      fieldErrors.nombre = 'El nombre del estudiante es obligatorio.'
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
      fieldErrors.apellido = 'El apellido del estudiante es obligatorio.'
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
      !formData.carnet.trim() ||
      formData.carnet.length < 6 ||
      formData.carnet.length > 10
    ) {
      fieldErrors.carnet = 'El carnet o ID debe ser numérico y contener entre 6 y 10 dígitos.'
      isValid = false
    } else if (/^0+$/.test(formData.carnet.trim())) {
      fieldErrors.carnet = 'El carnet no puede estar compuesto únicamente por ceros.'
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

  async function loadStudentData() {
    if (!studentId || isNaN(studentId)) {
      errorMessage.value = 'El identificador del estudiante es inválido.'
      isLoading.value = false
      return
    }

    isLoading.value = true
    clearMessages()
    try {
      const student = await adminService.getStudentById(studentId)
      studentOriginal.value = student

      formData.nombre = student.nombre || ''
      formData.apellido = student.apellido || ''
      formData.carnet = student.carnet || ''
      formData.genero = (student.genero || 'masculino').toLowerCase()
      formData.fechaNacimiento = student.fechaNacimiento || ''
      formData.correo = student.correo || ''
      formData.telefono = student.telefono || ''
      formData.direccion = student.direccion || ''
      formData.fotografiaUrl = student.fotografiaUrl || ''
      formData.documentoCarnetUrl = student.documentoCarnetUrl || ''
    } catch (err) {
      errorMessage.value = extractApiErrorMessage(
        err,
        'No fue posible cargar la información del estudiante.'
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
      const payload: UpdateStudentAdminPayload = {
        nombre: formData.nombre.trim(),
        apellido: formData.apellido.trim(),
        carnet: formData.carnet.trim(),
        genero: formData.genero,
        fechaNacimiento: formData.fechaNacimiento,
        direccion: formData.direccion.trim(),
        telefono: formData.telefono.trim() || undefined,
        fotografiaUrl: formData.fotografiaUrl.trim() || undefined,
        documentoCarnetUrl: formData.documentoCarnetUrl.trim() || undefined
      }

      const response = await adminService.updateStudent(studentId, payload)
      successMessage.value = response.mensaje || 'Información del estudiante actualizada correctamente.'

      if (studentOriginal.value) {
        studentOriginal.value.nombre = response.nombre
        studentOriginal.value.apellido = response.apellido
        studentOriginal.value.carnet = response.carnet
        studentOriginal.value.genero = response.genero
        studentOriginal.value.fechaNacimiento = response.fechaNacimiento
        studentOriginal.value.direccion = response.direccion
        studentOriginal.value.telefono = response.telefono
        studentOriginal.value.fotografiaUrl = response.fotografiaUrl
        studentOriginal.value.documentoCarnetUrl = response.documentoCarnetUrl
      }
    } catch (err) {
      const msg = extractApiErrorMessage(err, 'Ocurrió un error al actualizar los datos del estudiante.')
      errorMessage.value = msg

      const lowerMsg = msg.toLowerCase()
      if (lowerMsg.includes('carnet')) {
        fieldErrors.carnet = msg
      }
      if (lowerMsg.includes('teléfono') || lowerMsg.includes('telefono')) {
        fieldErrors.telefono = msg
      }
      if (lowerMsg.includes('dirección') || lowerMsg.includes('direccion')) {
        fieldErrors.direccion = msg
      }
    } finally {
      isSaving.value = false
    }
  }

  function goBack() {
    router.push('/admin/usuarios')
  }

  onMounted(() => {
    loadStudentData()
  })

  return {
    studentId,
    formData,
    studentOriginal,
    isLoading,
    isSaving,
    errorMessage,
    successMessage,
    fieldErrors,
    canSave,
    maxBirthDate,
    minBirthDate,
    handleNombreInput,
    handleApellidoInput,
    handleCarnetInput,
    handlePhoneInput,
    clearMessages,
    clearFieldError,
    validateForm,
    handleSave,
    goBack
  }
}
