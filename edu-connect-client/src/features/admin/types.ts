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

export interface InactiveUserItem {
  id: number
  correo: string
  rol: string
  estado: string
  tipoUsuario: 'Estudiante' | 'Tutor' | 'Administrador'
  nombreCompleto?: string | null
  identificador?: string | null
  fechaRegistro?: string | null
  fechaBaja?: string | null
  motivoBaja?: string | null
}

export type ActiveUsersTab = 'estudiantes' | 'tutores'
export type ActiveUsersTopTab = 'activos' | 'baja'

// ==========================================
// HU-08: REPORTES Y ESTADÍSTICAS
// ==========================================
export interface TutorAtencionesReporteItem {
  tutorId: number
  nombre: string
  apellido: string
  nombreCompleto: string
  carnet: string
  correo: string
  fotografiaUrl?: string
  totalSesionesAtendidas: number
  totalEstudiantesAtendidos: number
}

export interface MateriaDemandaReporteItem {
  materiaId: number
  nombreMateria: string
  totalSesiones: number
  sesionesAtendidas: number
  sesionesPendientes: number
  sesionesCanceladas: number
  porcentajeDemanda: number
}

export interface ReportesResumen {
  totalSesiones: number
  totalSesionesAtendidas: number
  totalSesionesPendientes: number
  totalSesionesCanceladas: number
  tasaEfectividad: number
  totalTutoresConAtenciones: number
  totalMateriasConDemanda: number
  tutorTopNombre?: string | null
  tutorTopAtenciones: number
  materiaTopNombre?: string | null
  materiaTopSesiones: number
}

export type ReportsTabType = 'todos' | 'tutores' | 'materias'
