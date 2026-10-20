import { computed, onMounted, ref } from 'vue'
import { adminService } from '../services/admin.service'

import type {
  StudentReportItem,
  TutorReportItem,
  UserReportsTab
} from '../types'

type ReportAction = 'dar-baja' | 'rechazar'
type ReportTarget = 'tutor' | 'estudiante'

export function useAdminUserReports() {
  const tutorReports = ref<TutorReportItem[]>([])
  const studentReports = ref<StudentReportItem[]>([])

  const activeTab = ref<UserReportsTab>('tutores')
  const searchQuery = ref('')

  const isLoading = ref(false)
  const isProcessingAction = ref(false)

  const feedbackMessage = ref<{
    type: 'success' | 'error'
    text: string
  } | null>(null)

  const selectedReport = ref<TutorReportItem | StudentReportItem | null>(null)
  const selectedTarget = ref<ReportTarget | null>(null)
  const selectedAction = ref<ReportAction | null>(null)

  const isConfirmModalOpen = ref(false)

  const filteredTutorReports = computed(() => {
    const query = searchQuery.value.trim().toLowerCase()

    if (!query) {
      return tutorReports.value
    }

    return tutorReports.value.filter(report => {
      return (
        report.categoria.toLowerCase().includes(query) ||
        report.motivo.toLowerCase().includes(query) ||
        report.tutorNombreCompleto.toLowerCase().includes(query) ||
        report.tutorCorreo.toLowerCase().includes(query) ||
        report.estudianteDenuncianteNombreCompleto
          .toLowerCase()
          .includes(query) ||
        report.estudianteDenuncianteCorreo
          .toLowerCase()
          .includes(query) ||
        report.estado.toLowerCase().includes(query)
      )
    })
  })

  const filteredStudentReports = computed(() => {
    const query = searchQuery.value.trim().toLowerCase()

    if (!query) {
      return studentReports.value
    }

    return studentReports.value.filter(report => {
      return (
        report.categoria.toLowerCase().includes(query) ||
        report.motivo.toLowerCase().includes(query) ||
        report.estudianteNombreCompleto.toLowerCase().includes(query) ||
        report.estudianteCorreo.toLowerCase().includes(query) ||
        report.tutorDenuncianteNombreCompleto
          .toLowerCase()
          .includes(query) ||
        report.tutorDenuncianteCorreo
          .toLowerCase()
          .includes(query) ||
        report.estado.toLowerCase().includes(query)
      )
    })
  })

  const pendingTutorReports = computed(() => {
    return tutorReports.value.filter(
      report => !isReportResolved(report.estado)
    ).length
  })

  const pendingStudentReports = computed(() => {
    return studentReports.value.filter(
      report => !isReportResolved(report.estado)
    ).length
  })

  async function loadReports() {
    isLoading.value = true
    feedbackMessage.value = null

    try {
      const [tutors, students] = await Promise.all([
        adminService.getTutorReports(),
        adminService.getStudentReports()
      ])

      tutorReports.value = tutors
      studentReports.value = students
    } catch (error) {
      console.error('Error al cargar reportes:', error)

      feedbackMessage.value = {
        type: 'error',
        text: 'No fue posible cargar los reportes.'
      }
    } finally {
      isLoading.value = false
    }
  }

  function openActionModal(
    report: TutorReportItem | StudentReportItem,
    target: ReportTarget,
    action: ReportAction
  ) {
    selectedReport.value = report
    selectedTarget.value = target
    selectedAction.value = action
    isConfirmModalOpen.value = true
  }

  function closeActionModal() {
    if (isProcessingAction.value) {
      return
    }

    isConfirmModalOpen.value = false
    selectedReport.value = null
    selectedTarget.value = null
    selectedAction.value = null
  }

  async function confirmAction() {
    if (
      !selectedReport.value ||
      !selectedTarget.value ||
      !selectedAction.value
    ) {
      return
    }

    isProcessingAction.value = true
    feedbackMessage.value = null

    try {
      const reportId = selectedReport.value.id

      if (
        selectedTarget.value === 'tutor' &&
        selectedAction.value === 'dar-baja'
      ) {
        const response =
          await adminService.deactivateTutorFromReport(reportId)

        updateTutorReportState(
          reportId,
          response.estadoReporte
        )

        feedbackMessage.value = {
          type: 'success',
          text: response.mensaje
        }
      }

      if (
        selectedTarget.value === 'tutor' &&
        selectedAction.value === 'rechazar'
      ) {
        const response =
          await adminService.rejectTutorReport(reportId)

        updateTutorReportState(
          reportId,
          response.estadoReporte
        )

        feedbackMessage.value = {
          type: 'success',
          text: response.mensaje
        }
      }

      if (
        selectedTarget.value === 'estudiante' &&
        selectedAction.value === 'dar-baja'
      ) {
        const response =
          await adminService.deactivateStudentFromReport(reportId)

        updateStudentReportState(
          reportId,
          response.estadoReporte
        )

        feedbackMessage.value = {
          type: 'success',
          text: response.mensaje
        }
      }

      if (
        selectedTarget.value === 'estudiante' &&
        selectedAction.value === 'rechazar'
      ) {
        const response =
          await adminService.rejectStudentReport(reportId)

        updateStudentReportState(
          reportId,
          response.estadoReporte
        )

        feedbackMessage.value = {
          type: 'success',
          text: response.mensaje
        }
      }

      closeActionModalAfterSuccess()
    } catch (error) {
      console.error('Error al resolver reporte:', error)

      feedbackMessage.value = {
        type: 'error',
        text: 'No fue posible procesar la denuncia.'
      }
    } finally {
      isProcessingAction.value = false
    }
  }

  function updateTutorReportState(
    reportId: number,
    estado: string
  ) {
    const report = tutorReports.value.find(
      item => item.id === reportId
    )

    if (report) {
      report.estado = estado
    }
  }

  function updateStudentReportState(
    reportId: number,
    estado: string
  ) {
    const report = studentReports.value.find(
      item => item.id === reportId
    )

    if (report) {
      report.estado = estado
    }
  }

  function closeActionModalAfterSuccess() {
    isConfirmModalOpen.value = false
    selectedReport.value = null
    selectedTarget.value = null
    selectedAction.value = null
  }

  function isReportResolved(estado: string): boolean {
    const value = estado.toUpperCase()

    return (
      value === 'RESUELTO' ||
      value === 'DESESTIMADO'
    )
  }

  function formatDate(value?: string): string {
    if (!value) {
      return 'Sin fecha'
    }

    const date = new Date(value)

    if (Number.isNaN(date.getTime())) {
      return value
    }

    return new Intl.DateTimeFormat('es-GT', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    }).format(date)
  }

  function formatDateTime(value?: string): string {
    if (!value) {
      return 'Sin fecha'
    }

    const date = new Date(value)

    if (Number.isNaN(date.getTime())) {
      return value
    }

    return new Intl.DateTimeFormat('es-GT', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    }).format(date)
  }

  function dismissFeedback() {
    feedbackMessage.value = null
  }

  onMounted(() => {
    void loadReports()
  })

  return {
    tutorReports,
    studentReports,
    filteredTutorReports,
    filteredStudentReports,
    pendingTutorReports,
    pendingStudentReports,
    activeTab,
    searchQuery,
    isLoading,
    isProcessingAction,
    feedbackMessage,
    selectedReport,
    selectedTarget,
    selectedAction,
    isConfirmModalOpen,
    loadReports,
    openActionModal,
    closeActionModal,
    confirmAction,
    isReportResolved,
    formatDate,
    formatDateTime,
    dismissFeedback
  }
}