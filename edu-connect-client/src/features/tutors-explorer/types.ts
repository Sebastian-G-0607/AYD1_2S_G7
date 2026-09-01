export interface TutorExplorerItem {
  id: number
  nombre: string
  titulo: string
  especialidad: string
  rating: number
  totalResenas: number
  ubicacion: string
  universidad: string
  aniosExperiencia: number
  tags: string[]
  fotografiaUrl?: string
  isOnline?: boolean
  genero?: 'masculino' | 'femenino'
  edad?: number
}

export interface TutorFilterCriteria {
  materia?: string
  universidad?: string
  expMinima: number
  rangoEdad: number
  genero: 'any' | 'female' | 'male'
}
