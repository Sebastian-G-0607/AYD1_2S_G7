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

  function validateCarnet(carnet: string): boolean {
    return /^\d{6,10}$/.test(carnet)
  }

  function validateDpi(dpi: string): boolean {
    return /^\d{13}$/.test(dpi)
  }

  function validateTelefono(telefono: string): boolean {
    return /^\d{8}$/.test(telefono)
  }

  function isAtLeast16YearsOld(birthDateStr: string): boolean {
    if (!birthDateStr) return false
    const birthDate = new Date(birthDateStr)
    if (isNaN(birthDate.getTime())) return false
    const today = new Date()
    let age = today.getFullYear() - birthDate.getFullYear()
    const monthDiff = today.getMonth() - birthDate.getMonth()
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
      age--
    }
    return age >= 16
  }

  function isAtLeast18YearsOld(birthDateStr: string): boolean {
    if (!birthDateStr) return false
    const birthDate = new Date(birthDateStr)
    if (isNaN(birthDate.getTime())) return false
    const today = new Date()
    let age = today.getFullYear() - birthDate.getFullYear()
    const monthDiff = today.getMonth() - birthDate.getMonth()
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
      age--
    }
    return age >= 18
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

  function getRedirectPathByRole(role: string): string {
    const normalized = role.toLowerCase().trim()
    if (normalized === 'administrador' || normalized === 'admin') {
      return '/admin/aprobaciones'
    }
    if (normalized === 'tutor') {
      return '/tutor/dashboard'
    }
    if (normalized === 'estudiante' || normalized === 'student') {
      return '/estudiante/explorar-tutores'
    }
    return '/'
  }

  async function login(credentials: LoginCredentials): Promise<boolean> {
    isLoading.value = true
    errorMessage.value = null

    try {
      if ((credentials.correo || '').toLowerCase().includes('admin')) {
        const res = await authService.adminInitialLogin(credentials)
        if (res?.tempToken) {
          sessionStorage.setItem('edu_temp_token', res.tempToken)
          await router.push('/admin/2fa')
          return true
        }
      }

      const response = await authService.login(credentials)
      authStore.setAuthFromTokenResponse(response)
      const targetPath = getRedirectPathByRole(response.rol)
      await router.push(targetPath)
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

  async function logout(): Promise<void> {
    authStore.clearAuth()
    await router.push('/login')
  }

  async function registerStudent(studentData: StudentRegisterData): Promise<boolean> {
    if (!validateCarnet(studentData.carnet)) {
      errorMessage.value = 'El carnet debe ser numérico y tener entre 6 y 10 dígitos.'
      return false
    }

    if (!validateTelefono(studentData.telefono)) {
      errorMessage.value = 'El teléfono debe ser numérico y tener exactamente 8 dígitos.'
      return false
    }

    if (!isAtLeast16YearsOld(studentData.fechaNacimiento)) {
      errorMessage.value = 'El estudiante debe tener al menos 16 años cumplidos.'
      return false
    }

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
    if (!validateCarnet(tutorData.carnetId)) {
      errorMessage.value = 'El carnet debe ser numérico y tener entre 6 y 10 dígitos.'
      return false
    }

    if (!validateDpi(tutorData.numeroIdentificacion)) {
      errorMessage.value =
        'El DPI / Documento de identificación debe ser numérico y tener exactamente 13 dígitos.'
      return false
    }

    if (!validateTelefono(tutorData.telefono)) {
      errorMessage.value = 'El teléfono debe ser numérico y tener exactamente 8 dígitos.'
      return false
    }

    if (!isAtLeast18YearsOld(tutorData.fechaNacimiento)) {
      errorMessage.value = 'El tutor debe ser mayor de 18 años.'
      return false
    }

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
    getRedirectPathByRole,
    login,
    logout,
    registerStudent,
    registerTutor,
    validateCarnet,
    validateDpi,
    validateTelefono,
    isAtLeast16YearsOld,
    isAtLeast18YearsOld
  }
}
