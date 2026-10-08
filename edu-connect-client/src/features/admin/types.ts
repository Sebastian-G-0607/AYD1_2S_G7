export interface StudentApprovalItem {
  id: number
  nombre: string
  apellido: string
  carnet: string
  genero: string
  fechaNacimiento: string
  correo: string
  fotografiaUrl?: string
  documentoCarnetUrl?: string | null
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
  documentoCvUrl?: string | null
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
  documentoCarnetUrl?: string | null
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

export interface EstudianteCalificacionReporteItem {
  estudianteId: number
  nombre: string
  apellido: string
  nombreCompleto: string
  carnet: string
  correo: string
  fotografiaUrl?: string | null
  totalSesionesAtendidas: number
  totalEvaluaciones: number
  promedioCalificacion: number
}

export type ReportsTabType = 'todos' | 'tutores' | 'materias' | 'estudiantes'

// ==========================================
// HU-35: VER Y ACTUALIZAR TUTOR (ADMIN)
// ==========================================
export interface UpdateTutorAdminPayload {
  nombre: string
  apellido: string
  carnetId: string
  numeroIdentificacion: string
  genero: string
  fechaNacimiento: string
  direccion: string
  telefono?: string
  direccionTutoria: string
  anioInicio: number
  universidad: string
  materias?: string[]
  materiasIds?: number[]
  fotografiaUrl?: string
}

export interface UpdateTutorAdminResponse {
  id: number
  nombre: string
  apellido: string
  carnetId: string
  numeroIdentificacion: string
  genero: string
  fechaNacimiento: string
  correo: string
  fotografiaUrl: string
  especialidad: string
  materias: string[]
  direccionTutoria: string
  anioInicio: number
  universidad: string
  direccion?: string
  telefono?: string
  mensaje: string
}

// ==========================================
// HU-34: VER Y ACTUALIZAR ESTUDIANTE (ADMIN)
// ==========================================
export interface UpdateStudentAdminPayload {
  nombre: string
  apellido: string
  carnet: string
  genero: string
  fechaNacimiento: string
  direccion: string
  telefono?: string
  fotografiaUrl?: string
  documentoCarnetUrl?: string
}

export interface UpdateStudentAdminResponse {
  id: number
  nombre: string
  apellido: string
  carnet: string
  genero: string
  fechaNacimiento: string
  correo: string
  fotografiaUrl?: string
  direccion: string
  telefono?: string
  mensaje: string
  documentoCarnetUrl?: string
}

// ==========================================
// HU-36 / HU-37: GESTIÓN DE DENUNCIAS
// ==========================================
export interface TutorReportItem {
  id: number
  sesionId: number
  categoria: string
  motivo: string
  tutorId: number
  tutorNombreCompleto: string
  tutorCorreo: string
  estudianteDenuncianteId: number
  estudianteDenuncianteNombreCompleto: string
  estudianteDenuncianteCorreo: string
  fechaSesion: string
  fechaReporte: string
  estado: string
}

export interface StudentReportItem {
  id: number
  sesionId: number
  categoria: string
  motivo: string
  estudianteId: number
  estudianteNombreCompleto: string
  estudianteCorreo: string
  tutorDenuncianteId: number
  tutorDenuncianteNombreCompleto: string
  tutorDenuncianteCorreo: string
  fechaSesion: string
  fechaReporte: string
  estado: string
}

export interface ResolveReportResponse {
  reporteId: number
  estadoReporte: string
  usuarioId: number
  estadoUsuario: string
  mensaje: string
}

export type UserReportsTab = 'tutores' | 'estudiantes'
