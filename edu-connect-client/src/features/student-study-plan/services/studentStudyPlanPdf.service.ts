import api from '@/services/api'
import type { StudyPlan } from '../types'

export const studentStudyPlanPdfService = {
  async downloadPdf(plan: StudyPlan): Promise<Blob> {
    const { data } = await api.post(
      '/estudiantes/plan-estudio/pdf',
      {
        fechaUltimaSesion: plan.fechaUltimaSesion,
        tutorNombre: plan.tutorNombre,
        tutorIdentificacion: plan.tutorIdentificacion,
        tutorEspecialidad: plan.tutorEspecialidad,
        dificultadesIdentificadas: plan.dificultadesIdentificadas,
        recursos: plan.recursos.map(recurso => ({
          nombre: recurso.nombre,
          tipo: recurso.tipo,
          descripcionUso: recurso.descripcionUso
        }))
      },
      { responseType: 'blob' }
    )

    return data
  }
}
