export interface ApiResponse<T> {
  data: T
  message?: string
  success?: boolean
}

export interface ApiErrorResponse {
  message: string
  statusCode?: number
  errors?: Record<string, string[]>
}

export interface ProblemDetails {
  type?: string
  title?: string
  status?: number
  detail?: string
  instance?: string
  errors?: Record<string, string[] | string>
}
