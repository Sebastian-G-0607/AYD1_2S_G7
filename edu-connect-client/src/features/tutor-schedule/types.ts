export interface TutorScheduleRequest {
  horaInicio: string
  horaFin: string
  diasAtencion: number[]
}

export interface TutorScheduleResponse {
  tutorId: number
  horaInicio: string
  horaFin: string
  diasAtencion: number[]
}

export interface TutorScheduleGetResponse {
  tutorId: number
  horaInicio: string | null
  horaFin: string | null
  diasAtencion: number[]
}

export interface ScheduleDay {
  id: number
  label: string
  shortLabel: string
}

export interface ApiProblemDetails {
  type?: string
  title?: string
  status?: number
  detail?: string
  traceId?: string
}