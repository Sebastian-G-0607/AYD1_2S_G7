import api from './api'
import type { Materia } from '@/types'

export const materiasService = {
  async getAll(search?: string): Promise<Materia[]> {
    const isApiPrefix = api.defaults.baseURL?.replace(/\/+$/, '').endsWith('/api')
    const endpoint = isApiPrefix ? '/materias' : '/api/materias'
    const { data } = await api.get<Materia[]>(endpoint, {
      params: search ? { search } : undefined
    })
    return data
  }
}
