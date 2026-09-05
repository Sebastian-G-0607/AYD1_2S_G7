export interface TutorExplorerItem {
  tutorId: number
  nombreCompleto: string
  materias: string[]
  direccionTutoria: string
  fotografiaUrl: string
  universidad: string
  genero: string
  aniosExperiencia: number
  edad: number
}

export interface TutorFilterCriteria {
  materia?: string
  universidad?: string
  experienciaMinima?: number
  edadMinima?: number
  edadMaxima?: number
  genero?: 'any' | 'female' | 'male'
}
