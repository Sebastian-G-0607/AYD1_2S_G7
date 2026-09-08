export interface ProgramarSesionRequest {
  tutorId: number
  materiaId: number
  fechaSesion: string
  horaInicio: string
  motivo: string
}

export interface ProgramarSesionResponse {
  id: number
  estudianteId: number
  tutorId: number
  materiaId: number
  materia: string
  fechaSesion: string
  horaInicio: string
  horaFin: string | null
  motivo: string
  estado: string
}

export interface ApiProblemDetails {
  type?: string
  title?: string
  status?: number
  detail?: string
  instance?: string
}
