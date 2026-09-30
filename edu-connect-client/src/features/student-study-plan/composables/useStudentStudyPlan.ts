import { onMounted, ref } from 'vue'
import { studentStudyPlanService } from '../services/studentStudyPlan.service'
import type { StudyPlan } from '../types'

export function useStudentStudyPlan() {
  const studyPlan = ref<StudyPlan | null>(null)
  const isLoading = ref(false)
  const errorMessage = ref('')

  async function fetchStudyPlan() {
    isLoading.value = true
    errorMessage.value = ''

    try {
      studyPlan.value = await studentStudyPlanService.getStudyPlan()
    } catch {
      studyPlan.value = null
      errorMessage.value = 'No fue posible cargar tu plan de estudio.'
    } finally {
      isLoading.value = false
    }
  }

  onMounted(fetchStudyPlan)

  return {
    studyPlan,
    isLoading,
    errorMessage,
    fetchStudyPlan
  }
}