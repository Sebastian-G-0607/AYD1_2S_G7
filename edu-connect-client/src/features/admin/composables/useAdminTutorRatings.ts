import { computed, onMounted, ref } from 'vue'
import { adminService } from '../services/admin.service'
import type { CalificacionTutorReporteItem, RatingSortDirection } from '../types'

export function useAdminTutorRatings() {
  const items = ref<CalificacionTutorReporteItem[]>([])
  const isLoading = ref(false)
  const error = ref<string | null>(null)
  const searchQuery = ref('')
  const sortDirection = ref<RatingSortDirection>('desc')

  const sortedItems = computed(() => {
    const query = searchQuery.value.trim().toLowerCase()
    const factor = sortDirection.value === 'desc' ? -1 : 1

    return items.value
      .filter(
        item =>
          !query ||
          item.nombreCompleto.toLowerCase().includes(query) ||
          item.especialidad.toLowerCase().includes(query)
      )
      .sort((a, b) => {
        if (a.promedioCalificacion === null && b.promedioCalificacion === null) {
          return a.nombreCompleto.localeCompare(b.nombreCompleto)
        }
        if (a.promedioCalificacion === null) return 1
        if (b.promedioCalificacion === null) return -1

        return (
          factor * (a.promedioCalificacion - b.promedioCalificacion) ||
          a.nombreCompleto.localeCompare(b.nombreCompleto)
        )
      })
  })

  const totalRated = computed(
    () => items.value.filter(item => item.promedioCalificacion !== null).length
  )

  function toggleSort() {
    sortDirection.value = sortDirection.value === 'desc' ? 'asc' : 'desc'
  }

  async function loadRatings() {
    isLoading.value = true
    error.value = null

    try {
      items.value = await adminService.getCalificacionTutores()
    } catch {
      items.value = []
      error.value = 'No fue posible cargar el reporte de calificación de tutores.'
    } finally {
      isLoading.value = false
    }
  }

  onMounted(loadRatings)

  return {
    items,
    sortedItems,
    totalRated,
    isLoading,
    error,
    searchQuery,
    sortDirection,
    toggleSort,
    loadRatings
  }
}
