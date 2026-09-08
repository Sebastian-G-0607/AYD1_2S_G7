import type { AxiosInstance, InternalAxiosRequestConfig, AxiosResponse, AxiosError } from 'axios'
import { extractApiErrorMessage } from '@/utils/apiError'

export function setupInterceptors(client: AxiosInstance): AxiosInstance {
  client.interceptors.request.use(
    (config: InternalAxiosRequestConfig) => {
      const token = localStorage.getItem('edu_auth_token')
      if (token && config.headers) {
        config.headers.Authorization = `Bearer ${token}`
      }
      return config
    },
    (error: unknown) => Promise.reject(error)
  )

  client.interceptors.response.use(
    (response: AxiosResponse) => response,
    (error: AxiosError) => {
      const formattedMessage = extractApiErrorMessage(error)
      if (formattedMessage) {
        error.message = formattedMessage
      }

      if (error.response?.status === 401) {
        const url = error.config?.url || ''
        const isAuthRequest =
          url.includes('/login') || url.includes('/admin-login') || url.includes('/admin-2fa')
        const isAuthPage =
          window.location.pathname === '/login' || window.location.pathname === '/admin/2fa'

        if (!isAuthRequest && !isAuthPage) {
          localStorage.removeItem('edu_auth_token')
          localStorage.removeItem('edu_auth_user')
          window.location.href = '/login'
        }
      }
      return Promise.reject(error)
    }
  )

  return client
}
