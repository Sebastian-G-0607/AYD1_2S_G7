import { computed, onMounted, ref } from 'vue'
import { isAxiosError } from 'axios'
import { tutorScheduleService } from '../services/tutorSchedule.service'
import type {
  ApiProblemDetails,
  ScheduleDay,
  TutorScheduleRequest,
  TutorScheduleResponse,
  TutorScheduleGetResponse
} from '../types'

export function useTutorSchedule() {
  const days: ScheduleDay[] = [
    { id: 1, label: 'Lunes', shortLabel: 'Lun' },
    { id: 2, label: 'Martes', shortLabel: 'Mar' },
    { id: 3, label: 'Miércoles', shortLabel: 'Mié' },
    { id: 4, label: 'Jueves', shortLabel: 'Jue' },
    { id: 5, label: 'Viernes', shortLabel: 'Vie' },
    { id: 6, label: 'Sábado', shortLabel: 'Sáb' },
    { id: 7, label: 'Domingo', shortLabel: 'Dom' }
  ]

  const selectedDays = ref<number[]>([])
  const horaInicio = ref('')
  const horaFin = ref('')

  const isSaving = ref(false)
  const errorMessage = ref('')
  const successMessage = ref('')
  const savedSchedule = ref<TutorScheduleResponse | null>(null)
  const isLoading = ref(false)

  const canSubmit = computed(() => {
    return (
      selectedDays.value.length > 0 &&
      horaInicio.value !== '' &&
      horaFin.value !== '' &&
      horaFin.value > horaInicio.value &&
      !isSaving.value
    )
  })

  function toggleDay(dayId: number) {
    clearMessages()

    if (selectedDays.value.includes(dayId)) {
      selectedDays.value = selectedDays.value.filter(id => id !== dayId)
      return
    }

    selectedDays.value = [...selectedDays.value, dayId].sort((a, b) => a - b)
  }

  function isDaySelected(dayId: number): boolean {
    return selectedDays.value.includes(dayId)
  }

  function clearMessages() {
    errorMessage.value = ''
    successMessage.value = ''
  }

  function validateSchedule(): boolean {
    clearMessages()

    if (selectedDays.value.length === 0) {
      errorMessage.value = 'Selecciona al menos un día de atención.'
      return false
    }

    if (!horaInicio.value || !horaFin.value) {
      errorMessage.value = 'Debes indicar la hora de inicio y la hora de finalización.'
      return false
    }

    if (horaFin.value <= horaInicio.value) {
      errorMessage.value = 'La hora de finalización debe ser posterior a la hora de inicio.'
      return false
    }

    return true
  }

  function normalizeTime(time: string): string {
    return time.length === 5 ? `${time}:00` : time
  }

    function formatTimeForInput(time: string): string {
    return time.length >= 5 ? time.slice(0, 5) : time
  }

  async function loadSchedule() {
    isLoading.value = true
    clearMessages()

    try {
      const current: TutorScheduleGetResponse = await tutorScheduleService.getSchedule()

      if (current.horaInicio && current.horaFin) {
        horaInicio.value = formatTimeForInput(current.horaInicio)
        horaFin.value = formatTimeForInput(current.horaFin)
      }

      selectedDays.value = [...current.diasAtencion]
    } catch (error: unknown) {
      if (isAxiosError<ApiProblemDetails>(error)) {
        errorMessage.value =
          error.response?.data?.detail ??
          error.response?.data?.title ??
          'No fue posible cargar tu horario actual.'
      } else {
        errorMessage.value = 'Ocurrió un error inesperado al cargar tu horario.'
      }
    } finally {
      isLoading.value = false
    }
  }

  async function saveSchedule(): Promise<boolean> {
    if (!validateSchedule()) {
      return false
    }

    isSaving.value = true

    const payload: TutorScheduleRequest = {
      horaInicio: normalizeTime(horaInicio.value),
      horaFin: normalizeTime(horaFin.value),
      diasAtencion: [...selectedDays.value]
    }

    try {
      const response = await tutorScheduleService.updateSchedule(payload)

      savedSchedule.value = response
      successMessage.value = 'Horario de atención actualizado correctamente.'

      return true
    } catch (error: unknown) {
      if (isAxiosError<ApiProblemDetails>(error)) {
        errorMessage.value =
          error.response?.data?.detail ??
          error.response?.data?.title ??
          'No fue posible actualizar el horario de atención.'
      } else {
        errorMessage.value = 'Ocurrió un error inesperado al actualizar el horario.'
      }

      return false
    } finally {
      isSaving.value = false
    }
  }

  onMounted(() => {
    loadSchedule()
  })

  return {
    days,
    selectedDays,
    horaInicio,
    horaFin,
    isSaving,
    isLoading,
    errorMessage,
    successMessage,
    savedSchedule,
    canSubmit,
    toggleDay,
    isDaySelected,
    clearMessages,
    saveSchedule,
    loadSchedule
  }
}