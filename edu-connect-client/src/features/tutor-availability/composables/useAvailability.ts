import { ref } from 'vue'
import { availabilityService } from '../services/availability.service'
import type { TutorAvailability } from '../types'

export function useAvailability() {
  const availability = ref<TutorAvailability | null>(null)
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  async function fetchAvailability(tutorId: number, fecha: string) {
    isLoading.value = true
    error.value = null
    try {
      availability.value = await availabilityService.getByDate(tutorId, fecha)
    } catch {
      error.value = 'No se pudo consultar la disponibilidad del tutor.'
      availability.value = null
    } finally {
      isLoading.value = false
    }
  }

  return {
    availability,
    isLoading,
    error,
    fetchAvailability
  }
}