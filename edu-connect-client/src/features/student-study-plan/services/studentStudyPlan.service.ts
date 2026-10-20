import api from '@/services/api'
import type { StudyPlan } from '../types'

export const studentStudyPlanService = {
  async getStudyPlan(): Promise<StudyPlan | null> {
    const { data, status } = await api.get<StudyPlan>('/estudiantes/plan-estudio', {
      validateStatus: httpStatus => httpStatus === 200 || httpStatus === 204
    })

    if (status === 204) {
      return null
    }

    return data
  }
}
