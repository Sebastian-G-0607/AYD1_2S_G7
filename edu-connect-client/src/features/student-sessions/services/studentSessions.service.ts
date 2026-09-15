import api from '@/services/api'
import type {
  CancelarSesionEstudianteResponseDto,
  EstudianteSesionActivaDto,
  StudentSession
} from '../types'
import { formatDateLabel, formatTimeLabel, parseLocation, extractInitials } from '../utils'

function getSesionesActivasEndpoint(): string {
  const baseUrl = api.defaults.baseURL?.replace(/\/+$/, '') ?? ''
  return baseUrl.endsWith('/api')
    ? '/estudiantes/sesiones-activas'
    : '/api/estudiantes/sesiones-activas'
}

function getCancelarSesionEndpoint(sessionId: string | number): string {
  const baseUrl = api.defaults.baseURL?.replace(/\/+$/, '') ?? ''
  return baseUrl.endsWith('/api')
    ? `/sesiones/${sessionId}/cancelar`
    : `/api/sesiones/${sessionId}/cancelar`
}

export const studentSessionsService = {
  async getActiveSessions(): Promise<StudentSession[]> {
    const { data } = await api.get<EstudianteSesionActivaDto[]>(getSesionesActivasEndpoint())

    return data.map((dto, index): StudentSession => {
      const fecha = dto.fechaSesion || dto.fecha || ''
      const hora = dto.hora || dto.horaInicio || ''
      const avatar = dto.fotografiaTutorUrl || dto.fotografiaUrl || undefined
      const location = parseLocation(dto.direccionTutoria)
      const timeLabel = formatTimeLabel(fecha, hora)
      const isUpcoming =
        index === 0 || dto.estado?.toUpperCase() === 'PROXIMA' || timeLabel.includes('Hoy')

      const estadoNormalized = dto.estado ? dto.estado.toUpperCase() : 'PENDIENTE'

      let statusLabel = 'Pendiente'
      if (isUpcoming && estadoNormalized !== 'CANCELADA') {
        statusLabel = 'Próxima'
      } else if (estadoNormalized === 'CONFIRMADA') {
        statusLabel = 'Confirmada'
      } else if (estadoNormalized === 'CANCELADA') {
        statusLabel = 'Cancelada'
      }

      return {
        id: dto.id,
        timeLabel,
        dateLabel: formatDateLabel(fecha),
        status: estadoNormalized,
        statusLabel,
        isUpcoming: isUpcoming && estadoNormalized !== 'CANCELADA',
        tutorName: dto.nombreTutor || 'Tutor',
        subject: dto.materia || 'Tutoría',
        avatarUrl: avatar || undefined,
        initials: extractInitials(dto.nombreTutor),
        locationType: location.locationType,
        locationTitle: location.locationTitle,
        locationSubtitle: location.locationSubtitle,
        meetingUrl: location.meetingUrl,
        reason: dto.motivo || 'Sesión de tutoría académica.'
      }
    })
  },

  async cancelSession(sessionId: string | number): Promise<CancelarSesionEstudianteResponseDto> {
    const { data } = await api.put<CancelarSesionEstudianteResponseDto>(
      getCancelarSesionEndpoint(sessionId),
      null,
      {
        headers: {
          Accept: 'application/json'
        }
      }
    )
    return data
  }
}
