import axios from 'axios'
import { ref } from 'vue'
import { studentHistoryService } from '../services/studentHistory.service'
import type { RateTutorPayload } from '../types'

export function useRateTutor() {
  const isSubmitting = ref(false)
  const errorMessage = ref<string | null>(null)

  async function submitRating(sesionId: number, payload: RateTutorPayload): Promise<boolean> {
    isSubmitting.value = true
    errorMessage.value = null

    try {
      await studentHistoryService.rateTutor(sesionId, payload)
      return true
    } catch (error: unknown) {
      const detail = axios.isAxiosError(error) ? error.response?.data?.detail : undefined
      errorMessage.value = detail || 'No fue posible guardar la calificación. Intenta de nuevo.'
      return false
    } finally {
      isSubmitting.value = false
    }
  }

  function clearError() {
    errorMessage.value = null
  }

  return { isSubmitting, errorMessage, submitRating, clearError }
}