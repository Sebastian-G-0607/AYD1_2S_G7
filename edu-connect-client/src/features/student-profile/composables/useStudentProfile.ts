import { onMounted, ref } from 'vue'
import { useAuthStore } from '@/features/auth'
import {
  getProfileApiError,
  studentProfileService
} from '../services/studentProfile.service'
import type {
  StudentProfile,
  UpdateStudentProfilePayload,
  ChangeStudentPasswordPayload
} from '../types'

export function useStudentProfile() {
  const authStore = useAuthStore()

  const profile = ref<StudentProfile | null>(null)

  const isLoading = ref(false)
  const isSaving = ref(false)
  const isChangingPassword = ref(false)

  const errorMessage = ref('')
  const successMessage = ref('')
  const passwordErrorMessage = ref('')

  function clearMessages() {
    errorMessage.value = ''
    successMessage.value = ''
  }

  function clearPasswordError() {
    passwordErrorMessage.value = ''
  }

  async function fetchProfile() {
    isLoading.value = true
    errorMessage.value = ''

    try {
      profile.value = await studentProfileService.getProfile()
    } catch (error) {
      errorMessage.value = getProfileApiError(
        error,
        'No fue posible cargar la información del perfil.'
      )
    } finally {
      isLoading.value = false
    }
  }

  async function updateProfile(
    payload: UpdateStudentProfilePayload
  ): Promise<boolean> {
    isSaving.value = true
    clearMessages()

    try {
      const updatedProfile =
        await studentProfileService.updateProfile(payload)

      profile.value = updatedProfile

      // Actualizamos también los datos mostrados
      // en el encabezado del Dashboard.
      if (authStore.token && authStore.user) {
        authStore.setAuth(authStore.token, {
          ...authStore.user,
          nombre: updatedProfile.nombre,
          apellido: updatedProfile.apellido,
          fotografiaUrl:
            updatedProfile.fotografiaUrl || undefined
        })
      }

      successMessage.value =
        'Perfil actualizado correctamente.'

      return true
    } catch (error) {
      errorMessage.value = getProfileApiError(
        error,
        'No fue posible actualizar el perfil.'
      )

      return false
    } finally {
      isSaving.value = false
    }
  }

  async function changePassword(
    payload: ChangeStudentPasswordPayload
  ): Promise<boolean> {
    isChangingPassword.value = true
    passwordErrorMessage.value = ''

    try {
      await studentProfileService.changePassword(payload)

      successMessage.value =
        'Contraseña actualizada correctamente.'

      return true
    } catch (error) {
      passwordErrorMessage.value = getProfileApiError(
        error,
        'No fue posible actualizar la contraseña.'
      )

      return false
    } finally {
      isChangingPassword.value = false
    }
  }

  onMounted(fetchProfile)

  return {
    profile,
    isLoading,
    isSaving,
    isChangingPassword,
    errorMessage,
    successMessage,
    passwordErrorMessage,
    fetchProfile,
    updateProfile,
    changePassword,
    clearMessages,
    clearPasswordError
  }
}