

export interface CategoriaReporteEstudiante {
  id: number
  nombre: string
}

export interface CalificarEstudianteRequest {
  estrellas: number
  comentario?: string
}

export interface ReportarEstudianteRequest {
  categoriaId: number
  explicacion: string
}