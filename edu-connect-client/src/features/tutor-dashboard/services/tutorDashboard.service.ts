import api from '@/services/api'
import type {
  TutorSession,
  TutorDashboardStats,
  CompleteSessionPayload,
  CancelSessionPayload,
  CancelarSesionResponseDto
} from '../types'

export const tutorDashboardService = {
  async getStats(): Promise<TutorDashboardStats> {
    const { data } = await api.get<TutorDashboardStats>('/tutores/dashboard/estadisticas')
    return data
  },

  async getPendingSessions(): Promise<TutorSession[]> {
    const { data } = await api.get<TutorSession[]>('/sesiones/pendientes')
    return data
  },

  async completeSession(payload: CompleteSessionPayload): Promise<boolean> {
    await api.post(`/sesiones/${payload.sesionId}/atender`, payload)
    return true
  },

  async cancelSession(payload: CancelSessionPayload): Promise<CancelarSesionResponseDto> {
    const body: { motivo: string; mensajeDisculpa?: string } = {
      motivo: payload.motivo.trim()
    }
    if (payload.mensajeDisculpa && payload.mensajeDisculpa.trim().length > 0) {
      body.mensajeDisculpa = payload.mensajeDisculpa.trim()
    }
    const { data } = await api.post<CancelarSesionResponseDto>(
      `/sesiones/${payload.sesionId}/cancelar`,
      body
    )
    return data
  }
}
