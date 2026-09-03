import api from '@/services/api'
import type { TutorAvailability } from '../types'

export const availabilityService = {
  async getByDate(tutorId: number, fecha: string): Promise<TutorAvailability> {
    const { data } = await api.get<TutorAvailability>(`/tutores/${tutorId}/disponibilidad`, {
      params: { fecha }
    })
    return data
  }
}