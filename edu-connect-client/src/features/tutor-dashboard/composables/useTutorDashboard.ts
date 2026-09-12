import { ref, watch, onMounted } from 'vue'
import { extractApiErrorMessage } from '@/utils/apiError'
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
  const actionError = ref<string | null>(null)
  const actionSuccess = ref<string | null>(null)

  const selectedSession = ref<TutorSession | null>(null)
  const isCompleteModalOpen = ref(false)
  const isCancelModalOpen = ref(false)

  watch(isCancelModalOpen, isOpen => {
    if (!isOpen) {
      actionError.value = null
    }
  })

  async function fetchDashboardData() {
    isLoading.value = true
    try {
      const [fetchedStats, fetchedSessions] = await Promise.all([
        tutorDashboardService.getStats(),
        tutorDashboardService.getPendingSessions()
      ])
      stats.value = fetchedStats
      sessions.value = fetchedSessions
    } catch (error: unknown) {
      actionError.value = extractApiErrorMessage(error, 'Error al cargar los datos del panel.')
    } finally {
      isLoading.value = false
    }
  }

  function openCompleteModal(session: TutorSession) {
    selectedSession.value = session
    actionError.value = null
    isCompleteModalOpen.value = true
  }

  function openCancelModal(session: TutorSession) {
    selectedSession.value = session
    actionError.value = null
    isCancelModalOpen.value = true
  }

  function dismissActionSuccess() {
    actionSuccess.value = null
  }

  function dismissActionError() {
    actionError.value = null
  }

  async function handleCompleteSession(payload: Omit<CompleteSessionPayload, 'sesionId'>) {
    if (!selectedSession.value) return
    isProcessingAction.value = true
    actionError.value = null
    try {
      await tutorDashboardService.completeSession({
        sesionId: selectedSession.value.id,
        ...payload
      })
      sessions.value = sessions.value.filter(s => s.id !== selectedSession.value?.id)
      stats.value.sesionesPendientes = Math.max(0, stats.value.sesionesPendientes - 1)
      stats.value.sesionesAtendidasMes += 1
      tutorDashboardService.getStats().then(s => { stats.value = s }).catch(() => {})
      actionSuccess.value = 'La sesión ha sido completada exitosamente.'
      isCompleteModalOpen.value = false
      selectedSession.value = null
    } catch (error: unknown) {
      actionError.value = extractApiErrorMessage(error, 'Error al completar la sesión.')
    } finally {
      isProcessingAction.value = false
    }
  }

  async function handleCancelSession(payload: Omit<CancelSessionPayload, 'sesionId'>) {
    if (!selectedSession.value) return
    isProcessingAction.value = true
    actionError.value = null
    try {
      const response = await tutorDashboardService.cancelSession({
        sesionId: selectedSession.value.id,
        motivo: payload.motivo,
        mensajeDisculpa: payload.mensajeDisculpa
      })
      sessions.value = sessions.value.filter(s => s.id !== selectedSession.value?.id)
      stats.value.sesionesPendientes = Math.max(0, stats.value.sesionesPendientes - 1)
      stats.value.sesionesCanceladas += 1
      tutorDashboardService.getStats().then(s => { stats.value = s }).catch(() => {})
      actionSuccess.value =
        response.mensaje ||
        'La sesión ha sido cancelada exitosamente y el estudiante ha sido notificado por correo electrónico.'
      isCancelModalOpen.value = false
      selectedSession.value = null
    } catch (error: unknown) {
      actionError.value = extractApiErrorMessage(
        error,
        'Ocurrió un error inesperado al cancelar la sesión.'
      )
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
    actionError,
    actionSuccess,
    selectedSession,
    isCompleteModalOpen,
    isCancelModalOpen,
    fetchDashboardData,
    openCompleteModal,
    openCancelModal,
    handleCompleteSession,
    handleCancelSession,
    dismissActionSuccess,
    dismissActionError
  }
}
