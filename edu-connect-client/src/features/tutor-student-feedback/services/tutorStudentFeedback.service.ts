import api from '@/services/api'
import type { CalificarEstudianteRequest, CategoriaReporteEstudiante, ReportarEstudianteRequest } from '../types'

export const tutorStudentFeedbackService = {
  async calificarEstudiante(sesionId: number, payload: CalificarEstudianteRequest): Promise<void> {
    await api.post(`/tutores/sesiones/${sesionId}/calificar-estudiante`, payload)
  },

  async getCategoriasReporte(): Promise<CategoriaReporteEstudiante[]> {
    const { data } = await api.get<CategoriaReporteEstudiante[]>('/tutores/reportes-estudiante/categorias')
    return data
  },

  async reportarEstudiante(sesionId: number, payload: ReportarEstudianteRequest): Promise<void> {
    await api.post(`/tutores/sesiones/${sesionId}/reportar-estudiante`, payload)
  }
}
