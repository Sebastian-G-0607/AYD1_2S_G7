export interface StudentApprovalItem {
  id: number
  nombre: string
  apellido: string
  carnet: string
  genero: string
  fechaNacimiento: string
  correo: string
  fotografiaUrl?: string
  direccion?: string
  telefono?: string
  fechaRegistro?: string
  estado?: 'PENDIENTE' | 'APROBADO' | 'RECHAZADO'
}

export interface TutorApprovalItem {
  id: number
  nombre: string
  apellido: string
  carnetId: string
  numeroIdentificacion: string
  genero: string
  fechaNacimiento: string
  correo: string
  fotografiaUrl?: string
  especialidad?: string
  materias: string[]
  direccionTutoria?: string
  anioInicio?: number
  universidad?: string
  direccion?: string
  telefono?: string
  fechaRegistro?: string
  estado?: 'PENDIENTE' | 'APROBADO' | 'RECHAZADO'
}

export interface ApprovalActionPayload {
  estado: 'APROBADO' | 'RECHAZADO'
  motivo?: string
}

export interface ApprovalActionResponse {
  id: number
  correo: string
  estado: string
  mensaje: string
}

export type ApprovalTabType = 'estudiantes' | 'tutores'

export interface ApprovalStats {
  pendingStudents: number
  pendingTutors: number
  totalPending: number
}

// ==========================================
// HU-07: GESTIÓN DE USUARIOS ACTIVOS
// ==========================================
export interface ActiveStudentItem {
  id: number
  nombre: string
  apellido: string
  carnet: string
  genero: string
  fechaNacimiento: string
  correo: string
  fotografiaUrl?: string
  direccion?: string
  telefono?: string
  fechaRegistro?: string
  estado?: string
}

export interface ActiveTutorItem {
  id: number
  nombre: string
  apellido: string
  carnetId: string
  numeroIdentificacion: string
  genero: string
  fechaNacimiento: string
  correo: string
  fotografiaUrl?: string
  especialidad?: string
  materias: string[]
  direccionTutoria?: string
  anioInicio?: number
  universidad?: string
  direccion?: string
  telefono?: string
  fechaRegistro?: string
  estado?: string
}

export interface DarBajaPayload {
  motivo?: string
}

export interface DarBajaResponse {
  id: number
  correo: string
  estado: string
  fechaBaja: string
  motivo?: string
  mensaje: string
}

export type ActiveUsersTab = 'estudiantes' | 'tutores'
export type ActiveUsersTopTab = 'activos' | 'baja'

