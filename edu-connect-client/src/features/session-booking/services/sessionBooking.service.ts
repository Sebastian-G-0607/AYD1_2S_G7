import api from '@/services/api'
import type {
  ProgramarSesionRequest,
  ProgramarSesionResponse
} from '@/features/session-booking/types'

function getSesionesEndpoint(): string {
  const baseUrl = api.defaults.baseURL?.replace(/\/+$/, '') ?? ''
  return baseUrl.endsWith('/api') ? '/sesiones' : '/api/sesiones'
}

export const sessionBookingService = {
  async programarSesion(payload: ProgramarSesionRequest): Promise<ProgramarSesionResponse> {
    const { data } = await api.post<ProgramarSesionResponse>(getSesionesEndpoint(), payload)
    return data
  }
}
