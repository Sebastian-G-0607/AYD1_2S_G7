import axios from 'axios'

export function extractApiErrorMessage(
  error: unknown,
  fallback = 'Ocurrió un error inesperado al procesar la solicitud.'
): string {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data

    if (data) {
      if (typeof data === 'string' && data.trim().length > 0) {
        return data.trim()
      }

      if (typeof data === 'object') {
        const record = data as Record<string, unknown>

        if (record.errors && typeof record.errors === 'object') {
          const messages: string[] = []
          for (const val of Object.values(record.errors)) {
            if (Array.isArray(val)) {
              for (const item of val) {
                if (typeof item === 'string' && item.trim().length > 0) {
                  messages.push(item.trim())
                }
              }
            } else if (typeof val === 'string' && val.trim().length > 0) {
              messages.push(val.trim())
            }
          }
          if (messages.length > 0) {
            return messages.join('\n')
          }
        }

        if (typeof record.detail === 'string' && record.detail.trim().length > 0) {
          return record.detail.trim()
        }

        if (typeof record.message === 'string' && record.message.trim().length > 0) {
          return record.message.trim()
        }

        if (typeof record.title === 'string' && record.title.trim().length > 0) {
          return record.title.trim()
        }
      }
    }

    if (error.response?.status === 401) {
      const url = error.config?.url || ''
      if (url.includes('/admin-2fa')) {
        return 'El archivo de llave es inválido o no autorizado.'
      }
      return 'Credenciales inválidas o sesión no autorizada.'
    }

    if (error.response?.status === 403) {
      return 'Acceso denegado. No tienes permisos para realizar esta acción.'
    }

    if (error.response?.status === 404) {
      return 'El recurso solicitado no fue encontrado.'
    }

    if (error.response?.status === 409) {
      return 'Conflicto con el estado actual del recurso o registro ya existente.'
    }

    if (error.response?.status && error.response.status >= 500) {
      return 'Error interno del servidor. Inténtalo nuevamente más tarde.'
    }

    if (error.code === 'ERR_NETWORK') {
      return 'No se pudo conectar con el servidor. Revisa tu conexión a internet.'
    }

    return fallback
  }

  if (error instanceof Error && error.message) {
    return error.message
  }

  return fallback
}
