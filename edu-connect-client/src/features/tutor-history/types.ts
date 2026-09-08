export interface TutorHistorySession {
  sesionId: number
  fechaSesion: string
  horaInicio: string
  estudiante: string
  estado: string
}

export interface TutorHistoryFilters {
  fecha?: string
  estudiante?: string
}