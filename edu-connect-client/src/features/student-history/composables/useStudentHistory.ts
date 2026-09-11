import { computed, onMounted, ref } from 'vue'
import { studentHistoryService } from '../services/studentHistory.service'
import type { StudentHistorySession } from '../types'

export function useStudentHistory() {
  const sessions = ref<StudentHistorySession[]>([])
  const search = ref('')
  const statusFilter = ref('TODAS')
  const isLoading = ref(false)
  const errorMessage = ref('')

  const filteredSessions = computed(() => {
    const query = search.value.trim().toLowerCase()

    return sessions.value.filter(session => {
      const matchesSearch =
        !query ||
        session.tutor.toLowerCase().includes(query) ||
        session.materia.toLowerCase().includes(query) ||
        session.motivo.toLowerCase().includes(query)

      const matchesStatus =
        statusFilter.value === 'TODAS' ||
        session.estado.toUpperCase() === statusFilter.value

      return matchesSearch && matchesStatus
    })
  })

  const totalSessions = computed(() => sessions.value.length)

  const attendedSessions = computed(
    () => sessions.value.filter(s => s.estado.toUpperCase() === 'ATENDIDA').length
  )

  const cancelledSessions = computed(
    () =>
      sessions.value.filter(s =>
        ['CANCELADA_TUTOR', 'CANCELADA_ESTUDIANTE', 'CANCELADA'].includes(
          s.estado.toUpperCase()
        )
      ).length
  )

  async function fetchHistory() {
    isLoading.value = true
    errorMessage.value = ''

    try {
      sessions.value = await studentHistoryService.getHistory()
    } catch {
      sessions.value = []
      errorMessage.value = 'No fue posible cargar el historial de sesiones.'
    } finally {
      isLoading.value = false
    }
  }

  onMounted(fetchHistory)

  return {
    sessions,
    filteredSessions,
    search,
    statusFilter,
    totalSessions,
    attendedSessions,
    cancelledSessions,
    isLoading,
    errorMessage,
    fetchHistory
  }
}