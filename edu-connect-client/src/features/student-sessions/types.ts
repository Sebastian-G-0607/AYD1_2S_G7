export interface EstudianteSesionActivaDto {
  id: number
  fechaSesion?: string
  fecha?: string
  hora?: string
  horaInicio?: string
  nombreTutor: string
  materia: string
  direccionTutoria?: string
  motivo?: string
  estado: string
  fotografiaTutorUrl?: string | null
  fotografiaUrl?: string | null
}

export interface CancelarSesionEstudianteResponseDto {
  id: number
  estado: string
  fechaSesion: string
  horaInicio: string
  materia: string
  tutorNombre: string
  mensaje: string
}

export type SessionFilter = 'TODAS' | 'PROXIMAS' | 'PENDIENTES' | 'CONFIRMADAS'

export interface StudentSession {
  id: number | string
  timeLabel: string
  dateLabel: string
  status: string
  statusLabel: string
  isUpcoming: boolean
  tutorName: string
  subject: string
  avatarUrl?: string
  initials?: string
  locationType: 'presencial' | 'virtual'
  locationTitle: string
  locationSubtitle: string
  meetingUrl?: string
  reason: string
}
