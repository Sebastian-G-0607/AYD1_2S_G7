import api from '@/services/api'
import type {
  TutorScheduleRequest,
  TutorScheduleResponse,
  TutorScheduleGetResponse
} from '../types'

export const tutorScheduleService = {
  async getSchedule(): Promise<TutorScheduleGetResponse> {
    const { data } = await api.get<TutorScheduleGetResponse>('/tutores/horarios')

    return data
  },

  async updateSchedule(payload: TutorScheduleRequest): Promise<TutorScheduleResponse> {
    const { data } = await api.put<TutorScheduleResponse>('/tutores/horarios', payload)

    return data
  }
}
