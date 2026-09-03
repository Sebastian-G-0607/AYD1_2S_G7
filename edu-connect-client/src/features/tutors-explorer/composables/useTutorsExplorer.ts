import { ref, reactive } from 'vue'
import { tutorsExplorerService } from '../services/tutorsExplorer.service'
import type { TutorExplorerItem, TutorFilterCriteria } from '../types'

export function useTutorsExplorer() {
  const tutors = ref<TutorExplorerItem[]>([])
  const isLoading = ref(false)
  const error = ref<string | null>(null)
  const viewMode = ref<'grid' | 'list'>('grid')

  const filters = reactive<TutorFilterCriteria>({
    materia: '',
    universidad: '',
    experienciaMinima: 0,
    edadMaxima: 65,
    genero: 'any'
  })

  function resetFilters() {
    filters.materia = ''
    filters.universidad = ''
    filters.experienciaMinima = 0
    filters.edadMaxima = 65
    filters.genero = 'any'
    fetchTutors()
  }

  async function fetchTutors() {
    isLoading.value = true
    error.value = null
    try {
      tutors.value = await tutorsExplorerService.getTutors(filters)
    } catch {
      error.value = 'No se pudo cargar la lista de tutores. Intenta de nuevo.'
      tutors.value = []
    } finally {
      isLoading.value = false
    }
  }

  return {
    tutors,
    isLoading,
    error,
    viewMode,
    filters,
    totalCount: tutors,
    resetFilters,
    fetchTutors
  }
}