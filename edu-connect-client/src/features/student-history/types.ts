export interface StudentHistorySession {
  sesionId: number
  fechaSesion: string
  tutor: string
  materia: string
  direccionTutoria: string
  motivo: string
  resumen: string | null
  estado: string
  yaReportada?: boolean
}

export interface ReportCategory {
  id: number
  nombre: string
  descripcion: string | null
}

export interface ReportTutorPayload {
  categoriaId: number
  motivo: string
}

export interface ReportTutorResponse {
  id: number
  sesionId: number
  categoria: string
  estado: string
  mensaje: string
}
