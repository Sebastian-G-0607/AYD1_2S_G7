import { onMounted, ref } from 'vue'
import { tutorHistoryService } from '../services/tutorHistory.service'
import type { TutorHistorySession } from '../types'

export function useTutorHistory() {
  const sessions = ref<TutorHistorySession[]>([])

  const fecha = ref('')
  const estudiante = ref('')

  const isLoading = ref(false)
  const errorMessage = ref('')

  async function fetchHistory() {
    isLoading.value = true
    errorMessage.value = ''

    try {
      sessions.value = await tutorHistoryService.getHistory({
        fecha: fecha.value || undefined,
        estudiante: estudiante.value || undefined
      })
    } catch {
      sessions.value = []
      errorMessage.value = 'No fue posible cargar el historial de sesiones.'
    } finally {
      isLoading.value = false
    }
  }

  async function clearFilters() {
    fecha.value = ''
    estudiante.value = ''
    await fetchHistory()
  }

  onMounted(() => {
    fetchHistory()
  })

  return {
    sessions,
    fecha,
    estudiante,
    isLoading,
    errorMessage,
    fetchHistory,
    clearFilters
  }
}
