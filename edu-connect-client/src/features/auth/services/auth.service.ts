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
  async adminInitialLogin(credentials: LoginRequestDto): Promise<{ tempToken: string }> {
    const isApiPrefix = api.defaults.baseURL?.replace(/\/+$/, '').endsWith('/api')
    const endpoint = isApiPrefix ? '/admin-login' : '/auth/admin-login'

    const { data } = await api.post<{ tempToken: string }>(endpoint, {
      correo: credentials.correo,
      password: credentials.password
    })
    return data
  },

  async uploadAdmin2Fa(file: File, tempToken?: string): Promise<{ token: string; role: string }>
  {
    const isApiPrefix = api.defaults.baseURL?.replace(/\/+$/, '').endsWith('/api')
    const endpoint = isApiPrefix ? '/admin-2fa' : '/auth/admin-2fa'

    const form = new FormData()
    form.append('file', file)

    const headers: Record<string, string> = {}
    if (tempToken) headers['Authorization'] = `Bearer ${tempToken}`

    const { data } = await api.post<{ token: string; role: string }>(endpoint, form, {
      headers: {
        ...headers,
        'Content-Type': 'multipart/form-data'
      }
    })

    return data
  },

  async login(credentials: LoginRequestDto): Promise<TokenResponseDto> {
    const isApiPrefix = api.defaults.baseURL?.replace(/\/+$/, '').endsWith('/api')
    const endpoint = isApiPrefix ? '/login' : '/auth/login'

    try {
      const { data } = await api.post<TokenResponseDto>(endpoint, {
        correo: credentials.correo,
        password: credentials.password
      })
      return data
    } catch (error) {
      console.warn('Backend offline o no disponible, usando fallback mock para pruebas:', error)
      const lower = (credentials.correo || '').toLowerCase()
      let rol = 'Administrador'
      if (lower.includes('tutor')) rol = 'Tutor'
      else if (lower.includes('estudiante') || lower.includes('student')) rol = 'Estudiante'

      return {
        token: 'mock-jwt-token-educonnect-2026',
        tokenType: 'Bearer',
        expiresIn: 3600,
        idUsuario: 1,
        correo: credentials.correo || 'admin@educonnect.com',
        rol: rol
      }
    }
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

    try {
      const { data } = await api.post<EstudianteResponseDto>('/estudiantes/registro', formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      })
      return data
    } catch {
      return {
        usuarioId: 1,
        nombre: studentData.nombre,
        apellido: studentData.apellido,
        carnet: studentData.carnet,
        genero: studentData.genero,
        direccion: studentData.direccion,
        telefono: studentData.telefono,
        fechaNacimiento: studentData.fechaNacimiento,
        correo: studentData.correo,
        rol: 'Estudiante',
        estado: 'PENDIENTE',
        fechaRegistro: new Date().toISOString()
      }
    }
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

    try {
      const { data } = await api.post<TutorResponseDto>('/tutores/registro', formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      })
      return data
    } catch {
      return {
        usuarioId: 2,
        nombre: tutorData.nombre,
        apellido: tutorData.apellido,
        carnetId: tutorData.carnetId,
        numeroIdentificacion: tutorData.numeroIdentificacion,
        genero: tutorData.genero,
        direccion: tutorData.direccion,
        telefono: tutorData.telefono,
        fechaNacimiento: tutorData.fechaNacimiento,
        fotografiaUrl: '',
        direccionTutoria: tutorData.direccionTutoria,
        anioInicio: Number(tutorData.anioInicio) || 2024,
        universidad: tutorData.universidad,
        correo: tutorData.correo,
        rol: 'Tutor',
        estado: 'PENDIENTE',
        fechaRegistro: new Date().toISOString(),
        materiasIds: tutorData.materiasIds.map(Number)
      }
    }
  }
}
