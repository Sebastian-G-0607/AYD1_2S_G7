export interface StudentHistorySession {
  sesionId: number
  fechaSesion: string
  tutor: string
  materia: string
  direccionTutoria: string
  motivo: string
  resumen: string | null
  estado: string
}