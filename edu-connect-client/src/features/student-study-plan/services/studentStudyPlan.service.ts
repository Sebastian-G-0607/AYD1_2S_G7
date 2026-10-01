import api from '@/services/api'
import type { StudyPlan } from '../types'

/**
 * ⚠️ TEMPORAL: el endpoint real depende de HU-29 (Atender estudiante),
 * que aún no persiste el plan de estudio en el backend.
 * Mientras tanto se sirve un mock para poder construir la UI.
 *
 * Cuando el endpoint exista, reemplazar el cuerpo de `getStudyPlan`
 * por la llamada comentada más abajo y borrar `MOCK_STUDY_PLAN`.
 */
const MOCK_STUDY_PLAN: StudyPlan = {
  sesionId: 101,
  fechaUltimaSesion: '2026-09-24',
  tutorNombre: 'Ana Lucía Pérez',
  tutorIdentificacion: 'TUT-00457',
  tutorEspecialidad: 'Matemática, Cálculo Diferencial',
  dificultadesIdentificadas:
    'Confusión al aplicar la regla de la cadena en derivadas compuestas y dificultad para identificar cuándo usar derivación implícita.',
  recursos: [
    {
      recursoId: 1,
      nombre: 'Derivadas parciales - Capítulo 4',
      tipo: 'PDF',
      descripcionUso: 'Leer capítulo 4 completo, enfocarse en los ejemplos resueltos 4.3 y 4.4.'
    },
    {
      recursoId: 2,
      nombre: 'Khan Academy - Regla de la cadena',
      tipo: 'VIDEO',
      descripcionUso:
        'Ver el video completo hasta el minuto 12 y resolver los ejercicios de práctica.'
    },
    {
      recursoId: 3,
      nombre: 'Guía de ejercicios de derivación implícita',
      tipo: 'ENLACE',
      descripcionUso: 'Resolver los ejercicios del 1 al 8 antes de la próxima sesión.'
    }
  ]
}

export const studentStudyPlanService = {
  async getStudyPlan(): Promise<StudyPlan | null> {
    // TODO(HU-29): reemplazar mock por la llamada real cuando el backend exista.
    // const { data } = await api.get<StudyPlan | null>('/estudiantes/plan-estudio')
    // return data

    void api // evita warning de import sin uso mientras el mock está activo
    return new Promise(resolve => {
      setTimeout(() => resolve(MOCK_STUDY_PLAN), 300)
    })
  }
}
