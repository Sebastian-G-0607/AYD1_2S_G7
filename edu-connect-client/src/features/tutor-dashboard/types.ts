export interface TutorSession {
  id: number
  fecha: string
  hora: string
  estudianteNombre: string
  estudianteId: string
  estudianteAvatarUrl?: string
  materia: string
  motivo: string
  estado: 'PENDIENTE' | 'ATENDIDA' | 'CANCELADA'
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

export interface CancelSessionPayload {
  sesionId: number
  motivo?: string
}
