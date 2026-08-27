import type { AxiosInstance, InternalAxiosRequestConfig, AxiosResponse } from 'axios'

export function setupInterceptors(client: AxiosInstance): AxiosInstance {
  client.interceptors.request.use(
    (config: InternalAxiosRequestConfig) => {
      // Token interceptor hook
      return config
    },
    (error: unknown) => Promise.reject(error)
  )

  client.interceptors.response.use(
    (response: AxiosResponse) => response,
    (error: unknown) => Promise.reject(error)
  )

  return client
}
