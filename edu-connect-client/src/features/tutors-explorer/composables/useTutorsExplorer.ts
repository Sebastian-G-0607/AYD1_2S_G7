import { ref, reactive, computed, onMounted } from 'vue'
import { tutorsExplorerService } from '../services/tutorsExplorer.service'
import type { TutorExplorerItem, TutorFilterCriteria } from '../types'

export function useTutorsExplorer() {
  const tutors = ref<TutorExplorerItem[]>([])
  const isLoading = ref(false)
  const viewMode = ref<'grid' | 'list'>('grid')

  const filters = reactive<TutorFilterCriteria>({
    materia: '',
    universidad: '',
    expMinima: 0,
    rangoEdad: 65,
    genero: 'any'
  })

  const filteredTutors = computed(() => {
    return tutors.value.filter(tutor => {
      if (
        filters.materia &&
        !tutor.especialidad.toLowerCase().includes(filters.materia.toLowerCase())
      ) {
        const matchesTag = tutor.tags.some(tag =>
          tag.toLowerCase().includes(filters.materia.toLowerCase())
        )
        if (!matchesTag) return false
      }

      if (
        filters.universidad &&
        !tutor.universidad.toLowerCase().includes(filters.universidad.toLowerCase())
      ) {
        return false
      }

      if (tutor.aniosExperiencia < filters.expMinima) {
        return false
      }

      if (tutor.edad && tutor.edad > filters.rangoEdad) {
        return false
      }

      if (filters.genero !== 'any') {
        const expectedGenero = filters.genero === 'female' ? 'femenino' : 'masculino'
        if (tutor.genero !== expectedGenero) {
          return false
        }
      }

      return true
    })
  })

  const totalCount = computed(() => filteredTutors.value.length)

  function resetFilters() {
    filters.materia = ''
    filters.universidad = ''
    filters.expMinima = 0
    filters.rangoEdad = 65
    filters.genero = 'any'
  }

  async function fetchTutors() {
    isLoading.value = true
    try {
      tutors.value = await tutorsExplorerService.getTutors()
    } finally {
      isLoading.value = false
    }
  }

  onMounted(() => {
    fetchTutors()
  })

  return {
    tutors,
    filteredTutors,
    isLoading,
    viewMode,
    filters,
    totalCount,
    resetFilters,
    fetchTutors
  }
}
