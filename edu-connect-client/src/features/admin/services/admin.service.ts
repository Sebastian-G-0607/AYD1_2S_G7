import api from '@/services/api'
import type {
  StudentApprovalItem,
  TutorApprovalItem,
  ApprovalActionPayload,
  ApprovalActionResponse,
  ActiveStudentItem,
  ActiveTutorItem,
  DarBajaPayload,
  DarBajaResponse
} from '../types'

function getAdminBasePath(): string {
  const isApiPrefix = api.defaults.baseURL?.replace(/\/+$/, '').endsWith('/api')
  return isApiPrefix ? '/administrador' : '/api/administrador'
}

export const adminService = {
  // ==========================================
  // HU-05: GESTIÓN DE ESTUDIANTES PENDIENTES
  // ==========================================
  async getPendingStudents(): Promise<StudentApprovalItem[]> {
    const basePath = getAdminBasePath()
    try {
      const { data } = await api.get<StudentApprovalItem[]>(`${basePath}/estudiantes/pendientes`)
      return data
    } catch {
      const { data } = await api.get<StudentApprovalItem[]>('/administrador/estudiantes/pendientes')
      return data
    }
  },

  async approveStudent(estudianteId: number): Promise<ApprovalActionResponse> {
    const basePath = getAdminBasePath()
    const payload: ApprovalActionPayload = { estado: 'APROBADO' }
    try {
      const { data } = await api.put<ApprovalActionResponse>(
        `${basePath}/estudiantes/${estudianteId}/estado`,
        payload
      )
      return data
    } catch {
      const { data } = await api.put<ApprovalActionResponse>(
        `/administrador/estudiantes/${estudianteId}/estado`,
        payload
      )
      return data
    }
  },

  async rejectStudent(estudianteId: number, motivo?: string): Promise<ApprovalActionResponse> {
    const basePath = getAdminBasePath()
    const payload: ApprovalActionPayload = {
      estado: 'RECHAZADO',
      motivo: motivo || 'Solicitud rechazada por el administrador'
    }
    try {
      const { data } = await api.put<ApprovalActionResponse>(
        `${basePath}/estudiantes/${estudianteId}/estado`,
        payload
      )
      return data
    } catch {
      const { data } = await api.put<ApprovalActionResponse>(
        `/administrador/estudiantes/${estudianteId}/estado`,
        payload
      )
      return data
    }
  },

  // ==========================================
  // HU-06: GESTIÓN DE TUTORES PENDIENTES
  // ==========================================
  async getPendingTutors(): Promise<TutorApprovalItem[]> {
    const basePath = getAdminBasePath()
    try {
      const { data } = await api.get<TutorApprovalItem[]>(`${basePath}/tutores/pendientes`)
      return data
    } catch {
      const { data } = await api.get<TutorApprovalItem[]>('/administrador/tutores/pendientes')
      return data
    }
  },

  async approveTutor(tutorId: number): Promise<ApprovalActionResponse> {
    const basePath = getAdminBasePath()
    const payload: ApprovalActionPayload = { estado: 'APROBADO' }
    try {
      const { data } = await api.put<ApprovalActionResponse>(
        `${basePath}/tutores/${tutorId}/estado`,
        payload
      )
      return data
    } catch {
      const { data } = await api.put<ApprovalActionResponse>(
        `/administrador/tutores/${tutorId}/estado`,
        payload
      )
      return data
    }
  },

  async rejectTutor(tutorId: number, motivo?: string): Promise<ApprovalActionResponse> {
    const basePath = getAdminBasePath()
    const payload: ApprovalActionPayload = {
      estado: 'RECHAZADO',
      motivo: motivo || 'Solicitud de tutor rechazada por el administrador'
    }
    try {
      const { data } = await api.put<ApprovalActionResponse>(
        `${basePath}/tutores/${tutorId}/estado`,
        payload
      )
      return data
    } catch {
      const { data } = await api.put<ApprovalActionResponse>(
        `/administrador/tutores/${tutorId}/estado`,
        payload
      )
      return data
    }
  },

  // ==========================================
  // HU-07: GESTIÓN DE USUARIOS ACTIVOS
  // ==========================================
  async getActiveStudents(): Promise<ActiveStudentItem[]> {
    const basePath = getAdminBasePath()
    try {
      const { data } = await api.get<ActiveStudentItem[]>(`${basePath}/estudiantes/activos`)
      return data
    } catch {
      const { data } = await api.get<ActiveStudentItem[]>('/administrador/estudiantes/activos')
      return data
    }
  },

  async deactivateStudent(studentId: number, motivo?: string): Promise<DarBajaResponse> {
    const basePath = getAdminBasePath()
    const payload: DarBajaPayload = { motivo: motivo || 'Cuenta dada de baja por el administrador' }
    try {
      const { data } = await api.put<DarBajaResponse>(
        `${basePath}/estudiantes/${studentId}/dar-baja`,
        payload
      )
      return data
    } catch {
      const { data } = await api.put<DarBajaResponse>(
        `/administrador/estudiantes/${studentId}/dar-baja`,
        payload
      )
      return data
    }
  },

  async getActiveTutors(): Promise<ActiveTutorItem[]> {
    const basePath = getAdminBasePath()
    try {
      const { data } = await api.get<ActiveTutorItem[]>(`${basePath}/tutores/activos`)
      return data
    } catch {
      const { data } = await api.get<ActiveTutorItem[]>('/administrador/tutores/activos')
      return data
    }
  },

  async deactivateTutor(tutorId: number, motivo?: string): Promise<DarBajaResponse> {
    const basePath = getAdminBasePath()
    const payload: DarBajaPayload = { motivo: motivo || 'Cuenta dada de baja por el administrador' }
    try {
      const { data } = await api.put<DarBajaResponse>(
        `${basePath}/tutores/${tutorId}/dar-baja`,
        payload
      )
      return data
    } catch {
      const { data } = await api.put<DarBajaResponse>(
        `/administrador/tutores/${tutorId}/dar-baja`,
        payload
      )
      return data
    }
  }
}


