export interface TutorSession {
  id: number
  fecha: string
  hora: string
  estudianteNombre: string
  estudianteId: string
  estudianteAvatarUrl?: string
  materia: string
  motivo: string
  estado: 'PENDIENTE' | 'ATENDIDA' | 'CANCELADA' | 'CANCELADA_TUTOR'
}

export interface TutorDashboardStats {
  sesionesPendientes: number
  pendientesHoy: number
  sesionesAtendidasMes: number
  sesionesCanceladas: number
}

export interface CompleteSessionPayload {
  sesionId: number
  resumen: string
  recomendaciones?: string
  enviarCopiaCorreo?: boolean
}

export interface CancelSessionRequest {
  motivo: string
  mensajeDisculpa?: string
}

export interface CancelSessionPayload {
  sesionId: number
  motivo: string
  mensajeDisculpa?: string
}

export interface CancelarSesionResponseDto {
  id: number
  estado: string
  motivoCancelacion: string
  fechaSesion: string
  horaInicio: string
  materia: string
  estudianteNombre: string
  mensaje: string
}
