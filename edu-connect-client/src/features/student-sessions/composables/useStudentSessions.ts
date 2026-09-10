import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { extractApiErrorMessage } from '@/utils/apiError'
import { studentSessionsService } from '../services/studentSessions.service'
import type { StudentSession, SessionFilter } from '../types'

export function useStudentSessions() {
  const router = useRouter()

  const sessions = ref<StudentSession[]>([])
  const isLoading = ref(false)
  const errorMessage = ref<string | null>(null)
  const successMessage = ref<string | null>(null)
  const isFilterOpen = ref(false)
  const filterStatus = ref<SessionFilter>('TODAS')

  const sessionToCancel = ref<StudentSession | null>(null)
  const isCancelModalOpen = ref(false)
  const isCanceling = ref(false)

  const filteredSessions = computed(() => {
    if (filterStatus.value === 'TODAS') {
      return sessions.value
    }
    if (filterStatus.value === 'PROXIMAS') {
      return sessions.value.filter(session => session.isUpcoming)
    }
    if (filterStatus.value === 'PENDIENTES') {
      return sessions.value.filter(session => session.status === 'PENDIENTE')
    }
    if (filterStatus.value === 'CONFIRMADAS') {
      return sessions.value.filter(session => session.status === 'CONFIRMADA')
    }
    return sessions.value
  })

  async function loadSessions() {
    isLoading.value = true
    errorMessage.value = null
    try {
      sessions.value = await studentSessionsService.getActiveSessions()
    } catch (err) {
      errorMessage.value = extractApiErrorMessage(
        err,
        'No se pudieron cargar tus sesiones activas. Intenta de nuevo.'
      )
    } finally {
      isLoading.value = false
    }
  }

  function toggleFilter() {
    isFilterOpen.value = !isFilterOpen.value
  }

  function setFilter(status: SessionFilter) {
    filterStatus.value = status
  }

  function openCancelModal(session: StudentSession) {
    sessionToCancel.value = session
    isCancelModalOpen.value = true
  }

  function closeCancelModal() {
    if (isCanceling.value) return
    isCancelModalOpen.value = false
    sessionToCancel.value = null
  }

  async function confirmCancel() {
    if (!sessionToCancel.value) return

    isCanceling.value = true
    errorMessage.value = null
    try {
      const response = await studentSessionsService.cancelSession(sessionToCancel.value.id)
      sessions.value = sessions.value.filter(item => item.id !== sessionToCancel.value?.id)
      successMessage.value =
        response.mensaje || 'La sesión ha sido cancelada exitosamente por el estudiante.'
      isCancelModalOpen.value = false
      sessionToCancel.value = null
    } catch (err) {
      errorMessage.value = extractApiErrorMessage(
        err,
        'No se pudo cancelar la sesión. Intenta nuevamente.'
      )
    } finally {
      isCanceling.value = false
    }
  }

  function dismissError() {
    errorMessage.value = null
  }

  function dismissSuccess() {
    successMessage.value = null
  }

  function createNewSession() {
    void router.push('/estudiante/explorar-tutores')
  }

  onMounted(() => {
    void loadSessions()
  })

  return {
    sessions,
    filteredSessions,
    isLoading,
    errorMessage,
    successMessage,
    isFilterOpen,
    filterStatus,
    sessionToCancel,
    isCancelModalOpen,
    isCanceling,
    loadSessions,
    toggleFilter,
    setFilter,
    openCancelModal,
    closeCancelModal,
    confirmCancel,
    dismissError,
    dismissSuccess,
    createNewSession
  }
}
