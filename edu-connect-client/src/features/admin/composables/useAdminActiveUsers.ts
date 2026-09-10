import { ref, computed, onMounted } from 'vue'
import { extractApiErrorMessage } from '@/utils/apiError'
import { adminService } from '../services/admin.service'
import type {
  ActiveStudentItem,
  ActiveTutorItem,
  ActiveUsersTab,
  ActiveUsersTopTab,
  InactiveUserItem
} from '../types'

export interface SelectedBajaUser {
  id: number
  nombre: string
  correo: string
  tipo: 'estudiante' | 'tutor'
}

export function useAdminActiveUsers() {
  const students = ref<ActiveStudentItem[]>([])
  const tutors = ref<ActiveTutorItem[]>([])
  const inactiveUsers = ref<InactiveUserItem[]>([])
  const isLoading = ref(false)
  const searchQuery = ref('')
  const activeTopTab = ref<ActiveUsersTopTab>('activos')
  const activeSubTab = ref<ActiveUsersTab>('estudiantes')

  // Modal & Action state
  const selectedUser = ref<SelectedBajaUser | null>(null)
  const isBajaModalOpen = ref(false)
  const bajaMotivo = ref('')
  const isProcessingAction = ref(false)
  const feedbackMessage = ref<{ type: 'success' | 'error'; text: string } | null>(null)

  // ==========================================
  // FILTRADO REACTIVO
  // ==========================================
  const filteredStudents = computed(() => {
    const query = searchQuery.value.toLowerCase().trim()
    if (!query) return students.value

    return students.value.filter(s => {
      const fullName = `${s.nombre} ${s.apellido}`.toLowerCase()
      const carnet = (s.carnet || '').toLowerCase()
      const email = (s.correo || '').toLowerCase()
      return fullName.includes(query) || carnet.includes(query) || email.includes(query)
    })
  })

  const filteredTutors = computed(() => {
    const query = searchQuery.value.toLowerCase().trim()
    if (!query) return tutors.value

    return tutors.value.filter(t => {
      const fullName = `${t.nombre} ${t.apellido}`.toLowerCase()
      const carnet = (t.carnetId || '').toLowerCase()
      const idNum = (t.numeroIdentificacion || '').toLowerCase()
      const email = (t.correo || '').toLowerCase()
      const specialty = (t.especialidad || '').toLowerCase()
      const uni = (t.universidad || '').toLowerCase()
      const materiasMatch = (t.materias || []).some(m => m.toLowerCase().includes(query))

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

  const filteredInactiveUsers = computed(() => {
    const query = searchQuery.value.toLowerCase().trim()
    if (!query) return inactiveUsers.value

    return inactiveUsers.value.filter(user => {
      const fullName = (user.nombreCompleto || '').toLowerCase()
      const email = (user.correo || '').toLowerCase()
      const role = (user.rol || '').toLowerCase()
      const motivo = (user.motivoBaja || '').toLowerCase()
      return (
        fullName.includes(query) ||
        email.includes(query) ||
        role.includes(query) ||
        motivo.includes(query)
      )
    })
  })

  const filteredInactiveStudents = computed(() =>
    filteredInactiveUsers.value.filter(user => user.tipoUsuario === 'Estudiante')
  )

  const filteredInactiveTutors = computed(() =>
    filteredInactiveUsers.value.filter(user => user.tipoUsuario === 'Tutor')
  )

  const activeStudentsCount = computed(() => students.value.length)
  const activeTutorsCount = computed(() => tutors.value.length)
  const totalActiveCount = computed(() => activeStudentsCount.value + activeTutorsCount.value)

  const inactiveStudentsCount = computed(
    () => inactiveUsers.value.filter(user => user.tipoUsuario === 'Estudiante').length
  )
  const inactiveTutorsCount = computed(
    () => inactiveUsers.value.filter(user => user.tipoUsuario === 'Tutor').length
  )
  const totalInactiveCount = computed(() => inactiveUsers.value.length)

  // ==========================================
  // CARGA DE DATOS
  // ==========================================
  async function fetchStudents() {
    try {
      students.value = await adminService.getActiveStudents()
    } catch (error) {
      console.error('Error al cargar estudiantes activos:', error)
    }
  }

  async function fetchTutors() {
    try {
      tutors.value = await adminService.getActiveTutors()
    } catch (error) {
      console.error('Error al cargar tutores activos:', error)
    }
  }

  async function fetchInactiveUsers() {
    try {
      inactiveUsers.value = await adminService.getInactiveUsers()
    } catch (error) {
      console.error('Error al cargar usuarios dados de baja:', error)
    }
  }

  async function fetchAll() {
    isLoading.value = true
    try {
      await Promise.all([fetchStudents(), fetchTutors(), fetchInactiveUsers()])
    } finally {
      isLoading.value = false
    }
  }

  // ==========================================
  // GESTIÓN DE MODAL DE BAJA
  // ==========================================
  function openBajaModal(user: ActiveStudentItem | ActiveTutorItem, tipo: 'estudiante' | 'tutor') {
    selectedUser.value = {
      id: user.id,
      nombre: `${user.nombre} ${user.apellido}`.trim(),
      correo: user.correo,
      tipo
    }
    bajaMotivo.value = ''
    isBajaModalOpen.value = true
  }

  function closeBajaModal() {
    isBajaModalOpen.value = false
    selectedUser.value = null
    bajaMotivo.value = ''
  }

  // ==========================================
  // CONFIRMACIÓN DE BAJA
  // ==========================================
  async function confirmBaja() {
    if (!selectedUser.value) return
    isProcessingAction.value = true
    feedbackMessage.value = null

    try {
      const motivo = bajaMotivo.value.trim() || undefined
      const target = selectedUser.value

      if (target.tipo === 'estudiante') {
        await adminService.deactivateStudent(target.id, motivo)
        students.value = students.value.filter(s => s.id !== target.id)
        feedbackMessage.value = {
          type: 'success',
          text: `El estudiante ${target.nombre} ha sido dado de baja exitosamente. Se envió la notificación por correo.`
        }
      } else {
        await adminService.deactivateTutor(target.id, motivo)
        tutors.value = tutors.value.filter(t => t.id !== target.id)
        feedbackMessage.value = {
          type: 'success',
          text: `El tutor ${target.nombre} ha sido dado de baja exitosamente. Se envió la notificación por correo.`
        }
      }

      await fetchInactiveUsers()
      closeBajaModal()
    } catch (err: unknown) {
      console.error('Error al dar de baja al usuario:', err)
      feedbackMessage.value = {
        type: 'error',
        text: extractApiErrorMessage(err, 'Ocurrió un error al procesar la baja del usuario.')
      }
    } finally {
      isProcessingAction.value = false
    }
  }

  function dismissFeedback() {
    feedbackMessage.value = null
  }

  function getInitials(nombre: string, apellido?: string): string {
    const f = nombre ? nombre.charAt(0) : ''
    const l = apellido ? apellido.charAt(0) : ''
    return `${f}${l}`.toUpperCase() || 'U'
  }

  function formatFecha(dateStr?: string): string {
    if (!dateStr) return '-'
    try {
      const d = new Date(dateStr)
      if (isNaN(d.getTime())) return dateStr
      return d.toLocaleDateString('es-ES', {
        day: '2-digit',
        month: 'short',
        year: 'numeric'
      })
    } catch {
      return dateStr
    }
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
    activeTopTab,
    activeSubTab,
    selectedUser,
    isBajaModalOpen,
    bajaMotivo,
    isProcessingAction,
    feedbackMessage,
    inactiveUsers,
    filteredInactiveUsers,
    filteredInactiveStudents,
    filteredInactiveTutors,
    activeStudentsCount,
    activeTutorsCount,
    totalActiveCount,
    inactiveStudentsCount,
    inactiveTutorsCount,
    totalInactiveCount,

    // Methods
    fetchStudents,
    fetchTutors,
    fetchInactiveUsers,
    fetchAll,
    openBajaModal,
    closeBajaModal,
    confirmBaja,
    dismissFeedback,
    getInitials,
    formatFecha
  }
}
