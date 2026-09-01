import { ref, onMounted } from 'vue'
import { materiasService } from '@/services/materias.service'
import type { Materia } from '@/types'

export function useMaterias(autoFetch = true) {
  const materias = ref<Materia[]>([])
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  async function fetchMaterias(search?: string): Promise<Materia[]> {
    isLoading.value = true
    error.value = null
    try {
      const data = await materiasService.getAll(search)
      materias.value = data
      return data
    } catch {
      error.value = 'No se pudieron cargar las materias disponibles.'
      return []
    } finally {
      isLoading.value = false
    }
  }

  if (autoFetch) {
    onMounted(() => {
      fetchMaterias()
    })
  }

  return {
    materias,
    isLoading,
    error,
    fetchMaterias
  }
}
