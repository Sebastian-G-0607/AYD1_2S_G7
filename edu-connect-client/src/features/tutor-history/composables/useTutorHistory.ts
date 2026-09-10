import { computed, onMounted, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { tutorHistoryService } from '../services/tutorHistory.service'
import type { TutorHistorySession } from '../types'

export function useTutorHistory() {
  const route = useRoute()
  const router = useRouter()

  const sessions = ref<TutorHistorySession[]>([])

  const initialFecha = typeof route?.query?.fecha === 'string' ? route.query.fecha : ''
  const initialEstudiante =
    typeof route?.query?.estudiante === 'string' ? route.query.estudiante : ''

  const fecha = ref(initialFecha)
  const estudiante = ref(initialEstudiante)

  const isInitialLoading = ref(true)
  const isFiltering = ref(false)
  const errorMessage = ref('')

  const hasActiveFilters = computed(() => Boolean(fecha.value.trim() || estudiante.value.trim()))

  const activeFiltersCount = computed(() => {
    let count = 0
    if (fecha.value.trim()) count++
    if (estudiante.value.trim()) count++
    return count
  })

  async function fetchHistory(isLive = false) {
    if (isLive) {
      isFiltering.value = true
    } else {
      isInitialLoading.value = true
    }
    errorMessage.value = ''

    const cleanFecha = fecha.value.trim()
    const cleanEstudiante = estudiante.value.trim()

    if (router) {
      const query: Record<string, string> = {}
      if (cleanFecha) {
        query.fecha = cleanFecha
      }
      if (cleanEstudiante) {
        query.estudiante = cleanEstudiante
      }
      router.replace({ query }).catch(() => {})
    }

    try {
      sessions.value = await tutorHistoryService.getHistory({
        fecha: cleanFecha || undefined,
        estudiante: cleanEstudiante || undefined
      })
    } catch {
      sessions.value = []
      errorMessage.value = 'No fue posible cargar el historial de sesiones.'
    } finally {
      isInitialLoading.value = false
      isFiltering.value = false
    }
  }

  function clearFecha() {
    fecha.value = ''
  }

  function clearEstudiante() {
    estudiante.value = ''
  }

  let isResetting = false

  async function clearFilters() {
    clearTimeout(debounceTimer)
    isResetting = true
    fecha.value = ''
    estudiante.value = ''
    isResetting = false
    if (router) {
      router.replace({ query: {} }).catch(() => {})
    }
    await fetchHistory(true)
  }

  let debounceTimer: ReturnType<typeof setTimeout> | undefined

  watch([fecha, estudiante], ([newFecha, newEstudiante], [oldFecha, oldEstudiante]) => {
    if (isResetting) return
    clearTimeout(debounceTimer)

    const fechaChanged = newFecha !== oldFecha
    const estudianteChanged = newEstudiante !== oldEstudiante

    if (fechaChanged && !estudianteChanged) {
      fetchHistory(true)
    } else {
      debounceTimer = setTimeout(() => {
        fetchHistory(true)
      }, 350)
    }
  })

  onMounted(() => {
    fetchHistory(false)
  })

  return {
    sessions,
    fecha,
    estudiante,
    isInitialLoading,
    isFiltering,
    isLoading: computed(() => isInitialLoading.value || isFiltering.value),
    hasActiveFilters,
    activeFiltersCount,
    errorMessage,
    fetchHistory,
    clearFecha,
    clearEstudiante,
    clearFilters
  }
}
