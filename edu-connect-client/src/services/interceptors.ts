import type { AxiosInstance, InternalAxiosRequestConfig, AxiosResponse, AxiosError } from 'axios'

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
      if (error.response?.status === 401) {
        const isAuthRequest =
          error.config?.url?.includes('/login') || error.config?.url?.includes('/auth/login')
        if (!isAuthRequest) {
          localStorage.removeItem('edu_auth_token')
          localStorage.removeItem('edu_auth_user')
          if (window.location.pathname !== '/login') {
            window.location.href = '/login'
          }
        }
      }
      return Promise.reject(error)
    }
  )

  return client
}
