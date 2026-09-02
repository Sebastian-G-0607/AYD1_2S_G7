import { ref, computed, onMounted } from 'vue'
import { adminService } from '../services/admin.service'
import type {
  StudentApprovalItem,
  TutorApprovalItem,
  ApprovalTabType
} from '../types'

export type SelectedApprovalItem =
  | { type: 'estudiante'; data: StudentApprovalItem }
  | { type: 'tutor'; data: TutorApprovalItem }

export function useAdminApprovals() {
  const students = ref<StudentApprovalItem[]>([])
  const tutors = ref<TutorApprovalItem[]>([])
  const isLoading = ref(false)
  const searchQuery = ref('')
  const activeTab = ref<ApprovalTabType>('estudiantes')

  // Modals & Action states
  const selectedItem = ref<SelectedApprovalItem | null>(null)
  const isApproveModalOpen = ref(false)
  const isRejectModalOpen = ref(false)
  const rejectReason = ref('')
  const isProcessingAction = ref(false)
  const feedbackMessage = ref<{ type: 'success' | 'error'; text: string } | null>(null)

  // ==========================================
  // FILTRADO REACTIVO
  // ==========================================
  const filteredStudents = computed(() => {
    const query = searchQuery.value.toLowerCase().trim()
    if (!query) return students.value

    return students.value.filter(student => {
      const fullName = `${student.nombre} ${student.apellido}`.toLowerCase()
      const carnet = (student.carnet || '').toLowerCase()
      const email = (student.correo || '').toLowerCase()
      return fullName.includes(query) || carnet.includes(query) || email.includes(query)
    })
  })

  const filteredTutors = computed(() => {
    const query = searchQuery.value.toLowerCase().trim()
    if (!query) return tutors.value

    return tutors.value.filter(tutor => {
      const fullName = `${tutor.nombre} ${tutor.apellido}`.toLowerCase()
      const carnet = (tutor.carnetId || '').toLowerCase()
      const idNum = (tutor.numeroIdentificacion || '').toLowerCase()
      const email = (tutor.correo || '').toLowerCase()
      const specialty = (tutor.especialidad || '').toLowerCase()
      const uni = (tutor.universidad || '').toLowerCase()
      const materiasMatch = (tutor.materias || []).some(m => m.toLowerCase().includes(query))

      return (
        fullName.includes(query) ||
        carnet.includes(query) ||
        idNum.includes(query) ||
        email.includes(query) ||
        specialty.includes(query) ||
        uni.includes(query) ||
        materiasMatch
      )
    })
  })

  const pendingStudentsCount = computed(() => students.value.length)
  const pendingTutorsCount = computed(() => tutors.value.length)
  const totalPendingCount = computed(
    () => pendingStudentsCount.value + pendingTutorsCount.value
  )

  // ==========================================
  // CARGA DE DATOS
  // ==========================================
  async function fetchStudents() {
    try {
      students.value = await adminService.getPendingStudents()
    } catch (error) {
      console.error('Error al cargar estudiantes pendientes:', error)
    }
  }

  async function fetchTutors() {
    try {
      tutors.value = await adminService.getPendingTutors()
    } catch (error) {
      console.error('Error al cargar tutores pendientes:', error)
    }
  }

  async function fetchAll() {
    isLoading.value = true
    try {
      await Promise.all([fetchStudents(), fetchTutors()])
    } finally {
      isLoading.value = false
    }
  }

  // ==========================================
  // GESTIÓN DE MODALES
  // ==========================================
  function openApproveModal(
    item: StudentApprovalItem | TutorApprovalItem,
    type: 'estudiante' | 'tutor' = activeTab.value === 'estudiantes' ? 'estudiante' : 'tutor'
  ) {
    if (type === 'estudiante') {
      selectedItem.value = { type: 'estudiante', data: item as StudentApprovalItem }
    } else {
      selectedItem.value = { type: 'tutor', data: item as TutorApprovalItem }
    }
    isApproveModalOpen.value = true
  }

  function openRejectModal(
    item: StudentApprovalItem | TutorApprovalItem,
    type: 'estudiante' | 'tutor' = activeTab.value === 'estudiantes' ? 'estudiante' : 'tutor'
  ) {
    if (type === 'estudiante') {
      selectedItem.value = { type: 'estudiante', data: item as StudentApprovalItem }
    } else {
      selectedItem.value = { type: 'tutor', data: item as TutorApprovalItem }
    }
    rejectReason.value = ''
    isRejectModalOpen.value = true
  }

  function closeModals() {
    isApproveModalOpen.value = false
    isRejectModalOpen.value = false
    selectedItem.value = null
    rejectReason.value = ''
  }

  // ==========================================
  // CONFIRMACIÓN DE ACCIONES
  // ==========================================
  async function confirmApprove() {
    if (!selectedItem.value) return
    isProcessingAction.value = true
    feedbackMessage.value = null

    try {
      if (selectedItem.value.type === 'estudiante') {
        const studentId = selectedItem.value.data.id
        await adminService.approveStudent(studentId)
        students.value = students.value.filter(s => s.id !== studentId)
        feedbackMessage.value = {
          type: 'success',
          text: `El estudiante ${selectedItem.value.data.nombre} ${selectedItem.value.data.apellido} ha sido aprobado exitosamente.`
        }
      } else {
        const tutorId = selectedItem.value.data.id
        await adminService.approveTutor(tutorId)
        tutors.value = tutors.value.filter(t => t.id !== tutorId)
        feedbackMessage.value = {
          type: 'success',
          text: `El tutor ${selectedItem.value.data.nombre} ${selectedItem.value.data.apellido} ha sido aprobado exitosamente.`
        }
      }
      closeModals()
    } catch (err: any) {
      console.error('Error al aprobar solicitud:', err)
      feedbackMessage.value = {
        type: 'error',
        text: err?.response?.data?.detail || 'Ocurrió un error al procesar la aprobación.'
      }
    } finally {
      isProcessingAction.value = false
    }
  }

  async function confirmReject() {
    if (!selectedItem.value) return
    isProcessingAction.value = true
    feedbackMessage.value = null

    try {
      const reason = rejectReason.value.trim() || undefined
      if (selectedItem.value.type === 'estudiante') {
        const studentId = selectedItem.value.data.id
        await adminService.rejectStudent(studentId, reason)
        students.value = students.value.filter(s => s.id !== studentId)
        feedbackMessage.value = {
          type: 'success',
          text: `La solicitud del estudiante ${selectedItem.value.data.nombre} ${selectedItem.value.data.apellido} ha sido rechazada.`
        }
      } else {
        const tutorId = selectedItem.value.data.id
        await adminService.rejectTutor(tutorId, reason)
        tutors.value = tutors.value.filter(t => t.id !== tutorId)
        feedbackMessage.value = {
          type: 'success',
          text: `La solicitud del tutor ${selectedItem.value.data.nombre} ${selectedItem.value.data.apellido} ha sido rechazada.`
        }
      }
      closeModals()
    } catch (err: any) {
      console.error('Error al rechazar solicitud:', err)
      feedbackMessage.value = {
        type: 'error',
        text: err?.response?.data?.detail || 'Ocurrió un error al procesar el rechazo.'
      }
    } finally {
      isProcessingAction.value = false
    }
  }

  function dismissFeedback() {
    feedbackMessage.value = null
  }

  function getInitials(nombre: string, apellido: string): string {
    const f = nombre ? nombre.charAt(0) : ''
    const l = apellido ? apellido.charAt(0) : ''
    return `${f}${l}`.toUpperCase() || 'U'
  }

  onMounted(() => {
    fetchAll()
  })

  return {
    // State
    students,
    tutors,
    filteredStudents,
    filteredTutors,
    isLoading,
    searchQuery,
    activeTab,
    pendingStudentsCount,
    pendingTutorsCount,
    totalPendingCount,
    selectedItem,
    isApproveModalOpen,
    isRejectModalOpen,
    rejectReason,
    isProcessingAction,
    feedbackMessage,

    // Methods
    fetchStudents,
    fetchTutors,
    fetchAll,
    openApproveModal,
    openRejectModal,
    closeModals,
    confirmApprove,
    confirmReject,
    dismissFeedback,
    getInitials
  }
}

