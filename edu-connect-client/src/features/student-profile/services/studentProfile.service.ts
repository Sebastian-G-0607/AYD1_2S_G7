import axios from 'axios'
import api from '@/services/api'
import type {
  StudentProfile,
  UpdateStudentProfilePayload,
  ChangeStudentPasswordPayload,
  ChangePasswordResponse
} from '../types'

interface ProblemDetails {
  title?: string
  detail?: string
}

export function getProfileApiError(
  error: unknown,
  fallback: string
): string {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data as ProblemDetails | undefined

    return data?.detail || data?.title || fallback
  }

  return fallback
}

export const studentProfileService = {
  async getProfile(): Promise<StudentProfile> {
    const { data } = await api.get<StudentProfile>(
      '/estudiantes/perfil'
    )

    return data
  },

  async updateProfile(
    payload: UpdateStudentProfilePayload
  ): Promise<StudentProfile> {
    const formData = new FormData()

    formData.append('Nombre', payload.nombre)
    formData.append('Apellido', payload.apellido)
    formData.append('Carnet', payload.carnet)
    formData.append('Genero', payload.genero)
    formData.append('Direccion', payload.direccion)
    formData.append('Telefono', payload.telefono)
    formData.append('FechaNacimiento', payload.fechaNacimiento)

    if (payload.fotografia instanceof File) {
      formData.append('Fotografia', payload.fotografia)
    }

    const { data } = await api.put<StudentProfile>(
      '/estudiantes/perfil',
      formData,
      {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      }
    )

    return data
  },

  async changePassword(
    payload: ChangeStudentPasswordPayload
  ): Promise<ChangePasswordResponse> {
    const { data } = await api.put<ChangePasswordResponse>(
      '/estudiantes/perfil/password',
      payload
    )

    return data
  }
}