import api from '@/services/api'
import type { TutorExplorerItem, TutorFilterCriteria } from '../types'

function buildParams(filters?: Partial<TutorFilterCriteria>) {
  if (!filters) return undefined

  return {
    materia: filters.materia || undefined,
    universidad: filters.universidad || undefined,
    experienciaMinima: filters.experienciaMinima || undefined,
    edadMinima: filters.edadMinima || undefined,
    edadMaxima: filters.edadMaxima || undefined,
    genero:
      filters.genero === 'female' ? 'Femenino' : filters.genero === 'male' ? 'Masculino' : undefined
  }
}

export const tutorsExplorerService = {
  async getTutors(filters?: Partial<TutorFilterCriteria>): Promise<TutorExplorerItem[]> {
    const { data } = await api.get<TutorExplorerItem[]>('/tutores/explorar', {
      params: buildParams(filters)
    })
    return data
  }
}