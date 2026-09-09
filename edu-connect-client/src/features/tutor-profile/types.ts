export interface TutorProfile {
  usuarioId: number
  nombre: string
  apellido: string
  carnetId: string
  numeroIdentificacion: string
  genero: string
  direccion: string
  telefono: string
  fechaNacimiento: string
  fotografiaUrl: string
  direccionTutoria: string
  anioInicio: number
  universidad: string
  correo: string
}

export interface TutorProfileForm {
  nombre: string
  apellido: string
  carnetId: string
  numeroIdentificacion: string
  genero: string
  direccion: string
  telefono: string
  fechaNacimiento: string
  fotografia: File | string | null
  direccionTutoria: string
  anioInicio: number
  universidad: string
  correo: string
}

export interface ChangePasswordRequest {
  passwordActual: string
  nuevaPassword: string
  confirmarNuevaPassword: string
}

export interface ApiProblemDetails {
  type?: string
  title?: string
  status?: number
  detail?: string
  traceId?: string
}