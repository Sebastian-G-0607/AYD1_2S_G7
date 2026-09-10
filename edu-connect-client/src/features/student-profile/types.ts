export interface StudentProfile {
  usuarioId: number
  nombre: string
  apellido: string
  carnet: string
  genero: string
  direccion: string
  telefono: string
  fechaNacimiento: string
  fotografiaUrl: string | null
  correo: string
}

export interface UpdateStudentProfilePayload {
  nombre: string
  apellido: string
  carnet: string
  genero: string
  direccion: string
  telefono: string
  fechaNacimiento: string
  fotografia?: File | null
}

export interface ChangeStudentPasswordPayload {
  passwordActual: string
  nuevaPassword: string
  confirmarNuevaPassword: string
}

export interface ChangePasswordResponse {
  mensaje: string
}