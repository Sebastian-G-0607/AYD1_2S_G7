export type RecursoTipo = 'TEXTO' | 'VIDEO' | 'PDF' | 'ENLACE'

export interface RecursoRecomendado {
  recursoId: number
  nombre: string
  tipo: RecursoTipo
  descripcionUso: string
}

/**
 * Plan de estudio más reciente del estudiante autenticado.
 * Se origina cuando el tutor marca una sesión como "Atendida" (HU-29)
 * e ingresa dificultades identificadas + recursos recomendados.
 */
export interface StudyPlan {
  sesionId: number
  fechaUltimaSesion: string // ISO date (yyyy-MM-dd)
  tutorNombre: string
  tutorIdentificacion: string
  tutorEspecialidad: string
  dificultadesIdentificadas: string
  recursos: RecursoRecomendado[]
}