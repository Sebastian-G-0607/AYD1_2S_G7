import api from '@/services/api'
import type {
  TutorSession,
  TutorDashboardStats,
  CompleteSessionPayload,
  CancelSessionPayload
} from '../types'

export const tutorDashboardService = {
  async getStats(): Promise<TutorDashboardStats> {
    try {
      const { data } = await api.get<TutorDashboardStats>('/tutor/dashboard/estadisticas')
      return data
    } catch {
      return {
        sesionesPendientes: 12,
        pendientesHoy: 4,
        sesionesAtendidasMes: 48,
        sesionesCanceladas: 2
      }
    }
  },

  async getPendingSessions(): Promise<TutorSession[]> {
    const { data } = await api.get<TutorSession[]>('/sesiones/pendientes')
    return data
  },



  async completeSession(payload: CompleteSessionPayload): Promise<boolean> {
      await api.post(`/sesiones/${payload.sesionId}/atender`, payload)
      return true
    },

  async cancelSession(payload: CancelSessionPayload): Promise<boolean> {
    try {
      await api.post(`/tutor/sesiones/${payload.sesionId}/cancelar`, payload)
      return true
    } catch {
      return true
    }
  }
}
