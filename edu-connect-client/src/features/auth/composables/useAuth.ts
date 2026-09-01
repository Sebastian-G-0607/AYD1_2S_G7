import { ref } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'
import { authService } from '../services/auth.service'
import { useAuthStore } from '../store'
import type { LoginCredentials, StudentRegisterData, TutorRegisterData } from '../types'

export function useAuth() {
  const router = useRouter()
  const authStore = useAuthStore()

  const isLoading = ref(false)
  const errorMessage = ref<string | null>(null)
  const successMessage = ref<string | null>(null)

  function clearError() {
    errorMessage.value = null
  }

  function clearSuccess() {
    successMessage.value = null
  }

  function validatePassword(password: string): boolean {
    const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/
    return passwordRegex.test(password)
  }

  function extractErrorMessage(error: unknown, fallback: string): string {
    if (axios.isAxiosError(error)) {
      const data = error.response?.data
      if (data && typeof data === 'object') {
        if ('detail' in data && typeof data.detail === 'string' && data.detail.trim().length > 0) {
          return data.detail
        }
        if (
          'message' in data &&
          typeof data.message === 'string' &&
          data.message.trim().length > 0
        ) {
          return data.message
        }
        if ('errors' in data && data.errors && typeof data.errors === 'object') {
          const errorList = Object.values(data.errors).flat().filter(Boolean).join('. ')
          if (errorList.length > 0) {
            return errorList
          }
        }
        if ('title' in data && typeof data.title === 'string' && data.title.trim().length > 0) {
          return data.title
        }
      }
      if (error.response?.status === 401) {
        return 'El correo o la contraseña son incorrectos.'
      }
      if (error.response?.status === 403) {
        return 'El usuario no puede iniciar sesión porque su estado actual no está habilitado.'
      }
      if (error.response?.status === 409) {
        return 'El correo electrónico, carnet o número de identificación ya se encuentra registrado.'
      }
      if (error.code === 'ERR_NETWORK') {
        return 'No se pudo conectar con el servidor. Revisa tu conexión a internet.'
      }
      return fallback
    }
    return 'Ocurrió un error inesperado al procesar la solicitud.'
  }

  async function login(credentials: LoginCredentials): Promise<boolean> {
    isLoading.value = true
    errorMessage.value = null

    try {
      const response = await authService.login(credentials)
      authStore.setAuthFromTokenResponse(response)
      await router.push('/')
      return true
    } catch (error: unknown) {
      errorMessage.value = extractErrorMessage(
        error,
        'Ocurrió un error inesperado al iniciar sesión. Inténtalo nuevamente.'
      )
      return false
    } finally {
      isLoading.value = false
    }
  }

  async function registerStudent(studentData: StudentRegisterData): Promise<boolean> {
    if (studentData.password !== studentData.confirmPassword) {
      errorMessage.value = 'Las contraseñas no coinciden.'
      return false
    }

    if (!validatePassword(studentData.password)) {
      errorMessage.value = 'La contraseña no cumple con los requisitos mínimos de seguridad.'
      return false
    }

    isLoading.value = true
    errorMessage.value = null

    try {
      await authService.registerStudent(studentData)
      await router.push({
        path: '/login',
        query: { registered: 'success' }
      })
      return true
    } catch (error: unknown) {
      errorMessage.value = extractErrorMessage(
        error,
        'Ocurrió un error al registrar el estudiante. Inténtalo nuevamente.'
      )
      return false
    } finally {
      isLoading.value = false
    }
  }

  async function registerTutor(tutorData: TutorRegisterData): Promise<boolean> {
    if (!tutorData.fotografia) {
      errorMessage.value = 'La fotografía de perfil es obligatoria para el registro de tutor.'
      return false
    }

    if (!tutorData.materiasIds || tutorData.materiasIds.length === 0) {
      errorMessage.value = 'Debes seleccionar al menos una materia de especialidad.'
      return false
    }

    if (tutorData.horaInicio && tutorData.horaFin && tutorData.horaFin <= tutorData.horaInicio) {
      errorMessage.value = 'La hora de fin debe ser posterior a la hora de inicio.'
      return false
    }

    if (tutorData.password !== tutorData.confirmPassword) {
      errorMessage.value = 'Las contraseñas no coinciden.'
      return false
    }

    if (!validatePassword(tutorData.password)) {
      errorMessage.value = 'La contraseña no cumple con los requisitos mínimos de seguridad.'
      return false
    }

    isLoading.value = true
    errorMessage.value = null

    try {
      await authService.registerTutor(tutorData)
      await router.push({
        path: '/login',
        query: { registered: 'success' }
      })
      return true
    } catch (error: unknown) {
      errorMessage.value = extractErrorMessage(
        error,
        'Ocurrió un error al registrar el tutor. Inténtalo nuevamente.'
      )
      return false
    } finally {
      isLoading.value = false
    }
  }

  return {
    isLoading,
    errorMessage,
    successMessage,
    clearError,
    clearSuccess,
    login,
    registerStudent,
    registerTutor
  }
}
