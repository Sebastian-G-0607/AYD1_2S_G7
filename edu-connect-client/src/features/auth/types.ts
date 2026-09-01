export interface LoginRequestDto {
  correo: string
  password: string
}

export type LoginCredentials = LoginRequestDto

export interface TokenResponseDto {
  token: string
  tokenType: string
  expiresIn: number
  idUsuario: number
  correo: string
  rol: string
}

export type AuthResponse = TokenResponseDto

export interface AuthUser {
  id: number
  correo: string
  rol: string
  nombre?: string
  apellido?: string
  fotografiaUrl?: string
}

export interface StudentRegisterData {
  nombre: string
  apellido: string
  carnet: string
  genero: string
  direccion: string
  telefono: string
  fechaNacimiento: string
  correo: string
  password: string
  confirmPassword?: string
  fotografia?: File | string | null
}

export interface EstudianteResponseDto {
  usuarioId: number
  nombre: string
  apellido: string
  carnet: string
  genero: string
  direccion: string
  telefono: string
  fechaNacimiento: string
  fotografiaUrl?: string
  correo: string
  rol: string
  estado: string
  fechaRegistro: string
}

export interface TutorRegisterData {
  nombre: string
  apellido: string
  carnetId: string
  numeroIdentificacion: string
  genero: string
  direccion: string
  telefono: string
  fechaNacimiento: string
  fotografia?: File | string | null
  direccionTutoria: string
  anioInicio: number | string
  universidad: string
  correo: string
  password: string
  confirmPassword?: string
  materiasIds: (number | string)[]
  horaInicio?: string
  horaFin?: string
  diasAtencion?: (number | string)[]
}

export interface TutorResponseDto {
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
  horaInicio?: string
  horaFin?: string
  correo: string
  rol: string
  estado: string
  fechaRegistro: string
  diasAtencion?: number[]
  materiasIds: number[]
}
