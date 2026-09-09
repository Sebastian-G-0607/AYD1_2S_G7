import api from '@/services/api'
import type { StudentHistorySession } from '../types'

export const studentHistoryService = {
  async getHistory(): Promise<StudentHistorySession[]> {
    const { data } = await api.get<StudentHistorySession[]>('/estudiantes/historial')
    return data
  }
}