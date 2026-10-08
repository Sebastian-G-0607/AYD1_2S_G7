import api from '@/services/api'
import type {
  ReportCategory,
  ReportTutorPayload,
  ReportTutorResponse,
  StudentHistorySession
} from '../types'

export const studentHistoryService = {
  async getHistory(): Promise<StudentHistorySession[]> {
    const { data } = await api.get<StudentHistorySession[]>('/estudiantes/historial')
    return data
  },

  async getReportCategories(): Promise<ReportCategory[]> {
    const { data } = await api.get<ReportCategory[]>('/estudiantes/reportes-tutor/categorias')
    return data
  },

  async reportTutor(sesionId: number, payload: ReportTutorPayload): Promise<ReportTutorResponse> {
    const { data } = await api.post<ReportTutorResponse>(
      `/estudiantes/sesiones/${sesionId}/reportar-tutor`,
      payload
    )
    return data
  }
}
