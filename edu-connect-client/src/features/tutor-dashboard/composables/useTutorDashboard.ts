import { ref, onMounted } from 'vue'
import { tutorDashboardService } from '../services/tutorDashboard.service'
import type {
  TutorSession,
  TutorDashboardStats,
  CompleteSessionPayload,
  CancelSessionPayload
} from '../types'

export function useTutorDashboard() {
  const sessions = ref<TutorSession[]>([])
  const stats = ref<TutorDashboardStats>({
    sesionesPendientes: 0,
    pendientesHoy: 0,
    sesionesAtendidasMes: 0,
    sesionesCanceladas: 0
  })
  const isLoading = ref(false)
  const isProcessingAction = ref(false)

  const selectedSession = ref<TutorSession | null>(null)
  const isCompleteModalOpen = ref(false)
  const isCancelModalOpen = ref(false)

  async function fetchDashboardData() {
    isLoading.value = true
    try {
      const [fetchedStats, fetchedSessions] = await Promise.all([
        tutorDashboardService.getStats(),
        tutorDashboardService.getPendingSessions()
      ])
      stats.value = fetchedStats
      sessions.value = fetchedSessions
    } finally {
      isLoading.value = false
    }
  }

  function openCompleteModal(session: TutorSession) {
    selectedSession.value = session
    isCompleteModalOpen.value = true
  }

  function openCancelModal(session: TutorSession) {
    selectedSession.value = session
    isCancelModalOpen.value = true
  }

  async function handleCompleteSession(payload: Omit<CompleteSessionPayload, 'sesionId'>) {
    if (!selectedSession.value) return
    isProcessingAction.value = true
    try {
      await tutorDashboardService.completeSession({
        sesionId: selectedSession.value.id,
        ...payload
      })
      sessions.value = sessions.value.filter(s => s.id !== selectedSession.value?.id)
      stats.value.sesionesPendientes = Math.max(0, stats.value.sesionesPendientes - 1)
      stats.value.sesionesAtendidasMes += 1
      isCompleteModalOpen.value = false
      selectedSession.value = null
    } finally {
      isProcessingAction.value = false
    }
  }

  async function handleCancelSession(payload: Omit<CancelSessionPayload, 'sesionId'>) {
    if (!selectedSession.value) return
    isProcessingAction.value = true
    try {
      await tutorDashboardService.cancelSession({
        sesionId: selectedSession.value.id,
        ...payload
      })
      sessions.value = sessions.value.filter(s => s.id !== selectedSession.value?.id)
      stats.value.sesionesPendientes = Math.max(0, stats.value.sesionesPendientes - 1)
      stats.value.sesionesCanceladas += 1
      isCancelModalOpen.value = false
      selectedSession.value = null
    } finally {
      isProcessingAction.value = false
    }
  }

  onMounted(() => {
    fetchDashboardData()
  })

  return {
    sessions,
    stats,
    isLoading,
    isProcessingAction,
    selectedSession,
    isCompleteModalOpen,
    isCancelModalOpen,
    fetchDashboardData,
    openCompleteModal,
    openCancelModal,
    handleCompleteSession,
    handleCancelSession
  }
}
