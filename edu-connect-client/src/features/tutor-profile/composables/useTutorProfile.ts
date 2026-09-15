import { computed, onMounted, reactive, ref } from 'vue'
import { isAxiosError } from 'axios'
import { tutorProfileService } from '../services/tutorProfile.service'
import type { ApiProblemDetails, ChangePasswordRequest, TutorProfileForm } from '../types'

export function useTutorProfile() {
  const formData = reactive<TutorProfileForm>({
    nombre: '',
    apellido: '',
    carnetId: '',
    numeroIdentificacion: '',
    genero: '',
    direccion: '',
    telefono: '',
    fechaNacimiento: '',
    fotografia: null,
    direccionTutoria: '',
    anioInicio: new Date().getFullYear(),
    universidad: '',
    correo: ''
  })

  const passwordData = reactive<ChangePasswordRequest>({
    passwordActual: '',
    nuevaPassword: '',
    confirmarNuevaPassword: ''
  })

  const isLoading = ref(false)
  const isSaving = ref(false)
  const isChangingPassword = ref(false)

  const errorMessage = ref('')
  const successMessage = ref('')

  const passwordErrorMessage = ref('')
  const passwordSuccessMessage = ref('')

  const currentYear = new Date().getFullYear()

  const passwordMismatch = computed(() => {
    if (!passwordData.nuevaPassword || !passwordData.confirmarNuevaPassword) {
      return false
    }

    return passwordData.nuevaPassword !== passwordData.confirmarNuevaPassword
  })

  const isNewPasswordValid = computed(() => {
    return /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/.test(passwordData.nuevaPassword)
  })

  const canSaveProfile = computed(() => {
    return (
      formData.nombre.trim() !== '' &&
      formData.apellido.trim() !== '' &&
      /^\d{6,10}$/.test(formData.carnetId) &&
      /^\d{13}$/.test(formData.numeroIdentificacion) &&
      formData.genero !== '' &&
      formData.direccion.trim() !== '' &&
      /^\d{8}$/.test(formData.telefono) &&
      formData.fechaNacimiento !== '' &&
      formData.direccionTutoria.trim() !== '' &&
      formData.anioInicio >= 1980 &&
      formData.anioInicio <= currentYear &&
      formData.universidad.trim() !== '' &&
      !isSaving.value
    )
  })

  const canChangePassword = computed(() => {
    return (
      passwordData.passwordActual.trim() !== '' &&
      passwordData.nuevaPassword.trim() !== '' &&
      passwordData.confirmarNuevaPassword.trim() !== '' &&
      !passwordMismatch.value &&
      isNewPasswordValid.value &&
      !isChangingPassword.value
    )
  })

  function getErrorMessage(error: unknown, fallback: string): string {
    if (isAxiosError<ApiProblemDetails>(error)) {
      return error.response?.data?.detail ?? error.response?.data?.title ?? fallback
    }

    return fallback
  }

  function clearProfileMessages() {
    errorMessage.value = ''
    successMessage.value = ''
  }

  function clearPasswordMessages() {
    passwordErrorMessage.value = ''
    passwordSuccessMessage.value = ''
  }

  async function loadProfile() {
    isLoading.value = true
    clearProfileMessages()

    try {
      const profile = await tutorProfileService.getProfile()

      formData.nombre = profile.nombre
      formData.apellido = profile.apellido
      formData.carnetId = profile.carnetId
      formData.numeroIdentificacion = profile.numeroIdentificacion
      formData.genero = profile.genero.toLowerCase()
      formData.direccion = profile.direccion
      formData.telefono = profile.telefono
      formData.fechaNacimiento = profile.fechaNacimiento
      formData.fotografia = profile.fotografiaUrl || null
      formData.direccionTutoria = profile.direccionTutoria
      formData.anioInicio = profile.anioInicio
      formData.universidad = profile.universidad
      formData.correo = profile.correo
    } catch (error: unknown) {
      errorMessage.value = getErrorMessage(
        error,
        'No fue posible cargar la información de tu perfil.'
      )
    } finally {
      isLoading.value = false
    }
  }

  async function saveProfile(): Promise<boolean> {
    clearProfileMessages()

    if (!canSaveProfile.value) {
      errorMessage.value = 'Revisa que todos los campos del perfil sean válidos.'
      return false
    }

    isSaving.value = true

    try {
      const updated = await tutorProfileService.updateProfile(formData)

      formData.nombre = updated.nombre
      formData.apellido = updated.apellido
      formData.carnetId = updated.carnetId
      formData.numeroIdentificacion = updated.numeroIdentificacion
      formData.genero = updated.genero.toLowerCase()
      formData.direccion = updated.direccion
      formData.telefono = updated.telefono
      formData.fechaNacimiento = updated.fechaNacimiento

      // IMPORTANTE:
      // sustituimos el File local por la URL de S3
      // devuelta por el backend.
      formData.fotografia = updated.fotografiaUrl || null

      formData.direccionTutoria = updated.direccionTutoria
      formData.anioInicio = updated.anioInicio
      formData.universidad = updated.universidad
      formData.correo = updated.correo

      successMessage.value = 'Tu perfil fue actualizado correctamente.'

      return true
    } catch (error: unknown) {
      errorMessage.value = getErrorMessage(error, 'No fue posible actualizar tu perfil.')

      return false
    } finally {
      isSaving.value = false
    }
  }

  async function changePassword(): Promise<boolean> {
    clearPasswordMessages()

    if (passwordMismatch.value) {
      passwordErrorMessage.value = 'La nueva contraseña y su confirmación no coinciden.'
      return false
    }

    if (!isNewPasswordValid.value) {
      passwordErrorMessage.value =
        'La nueva contraseña debe tener mínimo 8 caracteres, una mayúscula, una minúscula y un número.'
      return false
    }

    if (!canChangePassword.value) {
      return false
    }

    isChangingPassword.value = true

    try {
      await tutorProfileService.changePassword({
        ...passwordData
      })

      passwordData.passwordActual = ''
      passwordData.nuevaPassword = ''
      passwordData.confirmarNuevaPassword = ''

      passwordSuccessMessage.value = 'Contraseña actualizada correctamente.'

      return true
    } catch (error: unknown) {
      passwordErrorMessage.value = getErrorMessage(
        error,
        'No fue posible actualizar la contraseña.'
      )

      return false
    } finally {
      isChangingPassword.value = false
    }
  }

  onMounted(() => {
    loadProfile()
  })

  return {
    formData,
    passwordData,
    isLoading,
    isSaving,
    isChangingPassword,
    errorMessage,
    successMessage,
    passwordErrorMessage,
    passwordSuccessMessage,
    passwordMismatch,
    isNewPasswordValid,
    canSaveProfile,
    canChangePassword,
    clearProfileMessages,
    clearPasswordMessages,
    loadProfile,
    saveProfile,
    changePassword
  }
}
