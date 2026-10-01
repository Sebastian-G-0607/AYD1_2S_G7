import { onMounted, ref } from 'vue'
import { studentStudyPlanService } from '../services/studentStudyPlan.service'
import { studentStudyPlanPdfService } from '../services/studentStudyPlanPdf.service'
import { downloadBlob } from '@/utils/downloadBlob'
import type { StudyPlan } from '../types'

export function useStudentStudyPlan() {
  const studyPlan = ref<StudyPlan | null>(null)
  const isLoading = ref(false)
  const isDownloading = ref(false)
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

  async function downloadStudyPlanPdf() {
    if (!studyPlan.value) return

    isDownloading.value = true
    errorMessage.value = ''

    try {
      const blob = await studentStudyPlanPdfService.downloadPdf(studyPlan.value)
      downloadBlob(blob, `plan-estudio-${studyPlan.value.fechaUltimaSesion}.pdf`)
    } catch {
      errorMessage.value = 'No fue posible generar la constancia en PDF.'
    } finally {
      isDownloading.value = false
    }
  }

  onMounted(fetchStudyPlan)

  return {
    studyPlan,
    isLoading,
    isDownloading,
    errorMessage,
    fetchStudyPlan,
    downloadStudyPlanPdf
  }
}