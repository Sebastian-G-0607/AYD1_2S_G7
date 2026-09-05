import api from '@/services/api'
import type {
  TutorScheduleRequest,
  TutorScheduleResponse
} from '../types'

export const tutorScheduleService = {
  async updateSchedule(
    payload: TutorScheduleRequest
  ): Promise<TutorScheduleResponse> {
    const { data } = await api.put<TutorScheduleResponse>(
      '/tutores/horarios',
      payload
    )

    return data
  }
}