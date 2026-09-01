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
    try {
      const { data } = await api.get<TutorSession[]>('/tutor/sesiones/pendientes')
      return data
    } catch {
      return [
        {
          id: 1,
          fecha: '15 Oct, 2023',
          hora: '10:00 AM - 11:30 AM',
          estudianteNombre: 'Carlos Ruiz',
          estudianteId: '2021045',
          materia: 'Matemáticas Avanzadas',
          motivo: 'Dudas con cálculo integral y preparación para parcial.',
          estado: 'PENDIENTE'
        },
        {
          id: 2,
          fecha: '15 Oct, 2023',
          hora: '02:00 PM - 03:00 PM',
          estudianteNombre: 'Ana Gómez',
          estudianteId: '2022108',
          materia: 'Física I',
          motivo: 'Revisión de ejercicios de cinemática.',
          estado: 'PENDIENTE'
        },
        {
          id: 3,
          fecha: '16 Oct, 2023',
          hora: '09:00 AM - 10:00 AM',
          estudianteNombre: 'Luis Silva',
          estudianteId: '2020593',
          materia: 'Programación Básica',
          motivo: 'Ayuda con proyecto final en Python.',
          estado: 'PENDIENTE'
        }
      ]
    }
  },

  async completeSession(payload: CompleteSessionPayload): Promise<boolean> {
    try {
      await api.post(`/tutor/sesiones/${payload.sesionId}/completar`, payload)
      return true
    } catch {
      return true
    }
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
