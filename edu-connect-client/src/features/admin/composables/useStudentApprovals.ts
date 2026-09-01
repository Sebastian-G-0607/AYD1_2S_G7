import { ref, computed, onMounted } from 'vue'
import { adminService } from '../services/admin.service'
import type { StudentApprovalItem } from '../types'

export function useStudentApprovals() {
  const students = ref<StudentApprovalItem[]>([])
  const isLoading = ref(false)
  const searchQuery = ref('')
  const activeTab = ref<'estudiantes' | 'tutores'>('estudiantes')
  const selectedStudent = ref<StudentApprovalItem | null>(null)
  const isApproveModalOpen = ref(false)
  const isRejectModalOpen = ref(false)
  const isProcessingAction = ref(false)

  const filteredStudents = computed(() => {
    const query = searchQuery.value.toLowerCase().trim()
    if (!query) return students.value

    return students.value.filter(student => {
      const fullName = `${student.nombre} ${student.apellido}`.toLowerCase()
      const carnet = student.carnet.toLowerCase()
      const email = student.correo.toLowerCase()
      return fullName.includes(query) || carnet.includes(query) || email.includes(query)
    })
  })

  const pendingCount = computed(() => students.value.length)

  async function fetchStudents() {
    isLoading.value = true
    try {
      students.value = await adminService.getPendingStudents()
    } finally {
      isLoading.value = false
    }
  }

  function openApproveModal(student: StudentApprovalItem) {
    selectedStudent.value = student
    isApproveModalOpen.value = true
  }

  function openRejectModal(student: StudentApprovalItem) {
    selectedStudent.value = student
    isRejectModalOpen.value = true
  }

  async function confirmApprove() {
    if (!selectedStudent.value) return
    isProcessingAction.value = true
    try {
      await adminService.approveStudent(selectedStudent.value.id)
      students.value = students.value.filter(s => s.id !== selectedStudent.value?.id)
      isApproveModalOpen.value = false
      selectedStudent.value = null
    } finally {
      isProcessingAction.value = false
    }
  }

  async function confirmReject() {
    if (!selectedStudent.value) return
    isProcessingAction.value = true
    try {
      await adminService.rejectStudent(selectedStudent.value.id)
      students.value = students.value.filter(s => s.id !== selectedStudent.value?.id)
      isRejectModalOpen.value = false
      selectedStudent.value = null
    } finally {
      isProcessingAction.value = false
    }
  }

  onMounted(() => {
    fetchStudents()
  })

  return {
    students,
    filteredStudents,
    isLoading,
    searchQuery,
    activeTab,
    pendingCount,
    selectedStudent,
    isApproveModalOpen,
    isRejectModalOpen,
    isProcessingAction,
    fetchStudents,
    openApproveModal,
    openRejectModal,
    confirmApprove,
    confirmReject
  }
}
