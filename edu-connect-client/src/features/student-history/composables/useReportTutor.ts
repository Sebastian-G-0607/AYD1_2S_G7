import axios from 'axios'
import { ref } from 'vue'
import { studentHistoryService } from '../services/studentHistory.service'
import type { ReportCategory, ReportTutorPayload } from '../types'

export function useReportTutor() {
  const categories = ref<ReportCategory[]>([])
  const isLoadingCategories = ref(false)
  const isSubmitting = ref(false)
  const errorMessage = ref<string | null>(null)

  async function loadCategories() {
    if (categories.value.length > 0) return

    isLoadingCategories.value = true
    errorMessage.value = null

    try {
      categories.value = await studentHistoryService.getReportCategories()
    } catch {
      categories.value = []
      errorMessage.value = 'No fue posible cargar las categorías del reporte.'
    } finally {
      isLoadingCategories.value = false
    }
  }

  async function submitReport(sesionId: number, payload: ReportTutorPayload): Promise<boolean> {
    isSubmitting.value = true
    errorMessage.value = null

    try {
      await studentHistoryService.reportTutor(sesionId, payload)
      return true
    } catch (err) {
      const detail = axios.isAxiosError(err) ? err.response?.data?.detail : undefined
      errorMessage.value = detail || 'No fue posible enviar el reporte. Intenta de nuevo.'
      return false
    } finally {
      isSubmitting.value = false
    }
  }

  function clearError() {
    errorMessage.value = null
  }

  return {
    categories,
    isLoadingCategories,
    isSubmitting,
    errorMessage,
    loadCategories,
    submitReport,
    clearError
  }
}
