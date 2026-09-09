import api from '@/services/api'
import type {
  ChangePasswordRequest,
  TutorProfile,
  TutorProfileForm
} from '../types'

export const tutorProfileService = {
  async getProfile(): Promise<TutorProfile> {
    const { data } = await api.get<TutorProfile>(
      '/tutores/perfil'
    )

    return data
  },

  async updateProfile(
    payload: TutorProfileForm
  ): Promise<TutorProfile> {
    const formData = new FormData()

    formData.append('nombre', payload.nombre)
    formData.append('apellido', payload.apellido)
    formData.append('carnetId', payload.carnetId)
    formData.append(
      'numeroIdentificacion',
      payload.numeroIdentificacion
    )
    formData.append('genero', payload.genero)
    formData.append('direccion', payload.direccion)
    formData.append('telefono', payload.telefono)
    formData.append(
      'fechaNacimiento',
      payload.fechaNacimiento
    )
    formData.append(
      'direccionTutoria',
      payload.direccionTutoria
    )
    formData.append(
      'anioInicio',
      String(payload.anioInicio)
    )
    formData.append(
      'universidad',
      payload.universidad
    )

    if (payload.fotografia instanceof File) {
      formData.append(
        'fotografia',
        payload.fotografia
      )
    }

    const { data } = await api.put<TutorProfile>(
      '/tutores/perfil',
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
    payload: ChangePasswordRequest
  ): Promise<void> {
    await api.put(
      '/tutores/perfil/password',
      payload
    )
  }
}