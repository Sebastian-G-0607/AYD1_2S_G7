import api from '@/services/api'
import type {
  EstudianteResponseDto,
  LoginRequestDto,
  StudentRegisterData,
  TokenResponseDto,
  TutorRegisterData,
  TutorResponseDto
} from '../types'

export const authService = {
  async login(credentials: LoginRequestDto): Promise<TokenResponseDto> {
    const isApiPrefix = api.defaults.baseURL?.replace(/\/+$/, '').endsWith('/api')
    const endpoint = isApiPrefix ? '/login' : '/auth/login'

    const { data } = await api.post<TokenResponseDto>(endpoint, {
      correo: credentials.correo,
      password: credentials.password
    })
    return data
  },

  async registerStudent(studentData: StudentRegisterData): Promise<EstudianteResponseDto> {
    const formData = new FormData()
    formData.append('nombre', studentData.nombre)
    formData.append('apellido', studentData.apellido)
    formData.append('carnet', studentData.carnet)
    formData.append('genero', studentData.genero)
    formData.append('direccion', studentData.direccion)
    formData.append('telefono', studentData.telefono)
    formData.append('fechaNacimiento', studentData.fechaNacimiento)
    formData.append('correo', studentData.correo)
    formData.append('password', studentData.password)
    formData.append('confirmPassword', studentData.confirmPassword || studentData.password)

    if (studentData.fotografia instanceof File) {
      formData.append('fotografia', studentData.fotografia)
    }

    const { data } = await api.post<EstudianteResponseDto>('/estudiantes/registro', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })
    return data
  },

  async registerTutor(tutorData: TutorRegisterData): Promise<TutorResponseDto> {
    const formData = new FormData()
    formData.append('nombre', tutorData.nombre)
    formData.append('apellido', tutorData.apellido)
    formData.append('carnetId', tutorData.carnetId)
    formData.append('numeroIdentificacion', tutorData.numeroIdentificacion)
    formData.append('genero', tutorData.genero)
    formData.append('direccion', tutorData.direccion)
    formData.append('telefono', tutorData.telefono)
    formData.append('fechaNacimiento', tutorData.fechaNacimiento)
    formData.append('direccionTutoria', tutorData.direccionTutoria)
    formData.append('anioInicio', String(tutorData.anioInicio))
    formData.append('universidad', tutorData.universidad)
    formData.append('correo', tutorData.correo)
    formData.append('password', tutorData.password)
    formData.append('confirmPassword', tutorData.confirmPassword || tutorData.password)

    if (tutorData.horaInicio) {
      formData.append('horaInicio', tutorData.horaInicio)
    }
    if (tutorData.horaFin) {
      formData.append('horaFin', tutorData.horaFin)
    }

    if (tutorData.fotografia instanceof File) {
      formData.append('fotografia', tutorData.fotografia)
    }

    tutorData.materiasIds.forEach(id => {
      formData.append('materiasIds', String(id))
    })

    if (tutorData.diasAtencion && tutorData.diasAtencion.length > 0) {
      tutorData.diasAtencion.forEach(dia => {
        formData.append('diasAtencion', String(dia))
      })
    }

    const { data } = await api.post<TutorResponseDto>('/tutores/registro', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })
    return data
  }
}
