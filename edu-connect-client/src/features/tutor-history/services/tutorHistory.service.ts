import api from '@/services/api'
import type { TutorHistoryFilters, TutorHistorySession } from '../types'

export const tutorHistoryService = {
  async getHistory(filters: TutorHistoryFilters = {}): Promise<TutorHistorySession[]> {
    const params: Record<string, string> = {}

    if (filters.fecha?.trim()) {
      params.fecha = filters.fecha.trim()
    }

    if (filters.estudiante?.trim()) {
      params.estudiante = filters.estudiante.trim()
    }

    const { data } = await api.get<TutorHistorySession[]>('/tutores/historial', { params })

    return data
  }
}
