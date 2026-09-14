<script setup lang="ts">
import { AvailabilityModal, useAvailability } from '@/features/tutor-availability'
import axios from 'axios'
import { computed, onMounted, reactive, ref, watch } from 'vue'
import { RouterLink, useRoute } from 'vue-router'
import { BaseAlert, BaseButton, BaseCalendar, BaseCard, BaseTimePicker } from '@/components/ui'
import { useMaterias } from '@/composables/useMaterias'
import { tutorsExplorerService } from '@/features/tutors-explorer/services/tutorsExplorer.service'
import { sessionBookingService } from '@/features/session-booking/services/sessionBooking.service'
import type { TutorExplorerItem } from '@/features/tutors-explorer/types'
import type { ApiProblemDetails } from '@/features/session-booking/types'
import type { SesionOcupada } from '@/features/tutor-availability'

const route = useRoute()
const { materias, isLoading: isLoadingMaterias, error: materiasError } = useMaterias()
const {
  availability,
  isLoading: isLoadingAvailability,
  error: availabilityError,
  fetchAvailability
} = useAvailability()

const tutor = ref<TutorExplorerItem | null>(null)
const isLoadingTutor = ref(false)
const tutorLoadError = ref<string | null>(null)
const successMessage = ref<string | null>(null)
const submitError = ref<string | null>(null)
const isSubmitting = ref(false)
const showAvailability = ref(false)

const tutorDiasAtencion = ref<number[]>([])
const tutorHoraInicio = ref<string | null>(null)
const tutorHoraFin = ref<string | null>(null)

const form = reactive({
  materiaId: '',
  fechaSesion: '',
  horaInicio: '',
  motivo: ''
})

const errors = reactive({
  materiaId: '',
  fechaSesion: '',
  horaInicio: '',
  motivo: ''
})

const tutorId = computed(() => Number(route.params.tutorId))

const materiaOptions = computed(() => {
  if (!tutor.value) {
    return []
  }

  const materiasDelTutor = new Set(tutor.value.materias.map(nombre => nombre.trim().toLowerCase()))

  return materias.value
    .filter(materia => materiasDelTutor.has(materia.nombre.trim().toLowerCase()))
    .map(materia => ({
      value: materia.id,
      label: materia.nombre
    }))
})

const today = computed(() => {
  const now = new Date()
  const year = now.getFullYear()
  const month = String(now.getMonth() + 1).padStart(2, '0')
  const day = String(now.getDate()).padStart(2, '0')
  return `${year}-${month}-${day}`
})

const tutorInitial = computed(() => tutor.value?.nombreCompleto?.charAt(0).toUpperCase() || 'T')

const diasSemanaNombres: Record<number, string> = {
  1: 'Lunes',
  2: 'Martes',
  3: 'Miércoles',
  4: 'Jueves',
  5: 'Viernes',
  6: 'Sábado',
  7: 'Domingo'
}

const tutorDiasAtencionTexto = computed(() => {
  if (!tutorDiasAtencion.value || tutorDiasAtencion.value.length === 0) {
    return 'Sin días configurados'
  }
  return tutorDiasAtencion.value.map(dia => diasSemanaNombres[dia] || `Día ${dia}`).join(', ')
})

function formatTime(time?: string | null): string {
  if (!time) return ''
  return time.slice(0, 5)
}

function timeToMinutes(timeStr: string): number {
  const [hStr, mStr] = timeStr.split(':')
  return Number(hStr) * 60 + Number(mStr)
}

function addOneHour(timeStr: string): string {
  if (!timeStr || !timeStr.includes(':')) return ''
  const [hStr, mStr] = timeStr.split(':')
  const h = Number(hStr)
  const m = Number(mStr)
  if (isNaN(h) || isNaN(m)) return ''
  const newH = (h + 1) % 24
  return `${String(newH).padStart(2, '0')}:${String(m).padStart(2, '0')}`
}

const minHoraInicio = computed(() => {
  return tutorHoraInicio.value ? tutorHoraInicio.value.slice(0, 5) : '00:00'
})

const maxHoraInicio = computed(() => {
  if (!tutorHoraFin.value) return '23:00'
  const totalMin = timeToMinutes(tutorHoraFin.value.slice(0, 5)) - 60
  if (totalMin <= 0) return '00:00'
  const h = Math.floor(totalMin / 60)
  const m = totalMin % 60
  return `${String(h).padStart(2, '0')}:${String(m).padStart(2, '0')}`
})

const selectedSessionEndTime = computed(() => {
  if (!form.horaInicio) return ''
  return addOneHour(form.horaInicio)
})

function checkConflictWithOccupied(start: string): SesionOcupada | null {
  if (!start || !availability.value?.sesionesOcupadas) return null
  const startMin = timeToMinutes(start)
  const endMin = startMin + 60

  for (const s of availability.value.sesionesOcupadas) {
    const sStart = timeToMinutes(s.horaInicio.slice(0, 5))
    const sEnd = timeToMinutes(s.horaFin.slice(0, 5))
    if (startMin < sEnd && endMin > sStart) {
      return s
    }
  }
  return null
}

const sessionConflict = computed<SesionOcupada | null>(() => {
  if (!form.horaInicio) return null
  return checkConflictWithOccupied(form.horaInicio)
})

const isHoraOutOfRange = computed(() => {
  if (!form.horaInicio || !tutorHoraInicio.value || !tutorHoraFin.value) return false
  const chosenMin = timeToMinutes(form.horaInicio)
  const minMin = timeToMinutes(minHoraInicio.value)
  const maxMin = timeToMinutes(maxHoraInicio.value)
  return chosenMin < minMin || chosenMin > maxMin
})

const horaInicioEffectiveError = computed(() => {
  if (errors.horaInicio) return errors.horaInicio
  if (sessionConflict.value) {
    const confStart = formatTime(sessionConflict.value.horaInicio)
    const confEnd = formatTime(sessionConflict.value.horaFin)
    return `El horario seleccionado (${form.horaInicio} - ${selectedSessionEndTime.value}) se traslapa con una sesión ya agendada (${confStart} - ${confEnd}).`
  }
  if (isHoraOutOfRange.value) {
    return `La hora de inicio debe estar entre ${minHoraInicio.value} y ${maxHoraInicio.value} para finalizar antes de las ${formatTime(tutorHoraFin.value)}.`
  }
  return ''
})

function isConflictSession(s: SesionOcupada): boolean {
  if (!sessionConflict.value) return false
  return (
    sessionConflict.value.horaInicio === s.horaInicio &&
    sessionConflict.value.horaFin === s.horaFin
  )
}

const formattedSelectedDate = computed(() => {
  if (!form.fechaSesion) return ''
  const [year, month, day] = form.fechaSesion.split('-').map(Number)
  const date = new Date(year, month - 1, day)
  return date.toLocaleDateString('es-ES', {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
})

watch(
  () => form.fechaSesion,
  async newFecha => {
    form.horaInicio = ''
    errors.fechaSesion = ''
    errors.horaInicio = ''
    if (!newFecha || !tutorId.value) return
    await fetchAvailability(tutorId.value, newFecha)
    if (availability.value) {
      if (availability.value.diasAtencion && availability.value.diasAtencion.length > 0) {
        tutorDiasAtencion.value = availability.value.diasAtencion
      }
      if (availability.value.horaInicioAtencion) {
        tutorHoraInicio.value = availability.value.horaInicioAtencion
      }
      if (availability.value.horaFinAtencion) {
        tutorHoraFin.value = availability.value.horaFinAtencion
      }
    }
  }
)


async function loadTutor() {
  if (!Number.isInteger(tutorId.value) || tutorId.value <= 0) {
    tutorLoadError.value = 'El tutor seleccionado no es válido.'
    return
  }

  isLoadingTutor.value = true
  tutorLoadError.value = null

  try {
    const tutors = await tutorsExplorerService.getTutors()
    tutor.value = tutors.find(item => item.tutorId === tutorId.value) ?? null

    if (!tutor.value) {
      tutorLoadError.value = 'No fue posible cargar la información del tutor seleccionado.'
      return
    }

    try {
      await fetchAvailability(tutorId.value, today.value)
      if (availability.value) {
        tutorDiasAtencion.value = availability.value.diasAtencion ?? []
        tutorHoraInicio.value = availability.value.horaInicioAtencion
        tutorHoraFin.value = availability.value.horaFinAtencion
      }
    } catch {
      tutorDiasAtencion.value = []
    }
  } catch {
    tutorLoadError.value = 'No fue posible cargar la información del tutor seleccionado.'
  } finally {
    isLoadingTutor.value = false
  }
}

function clearErrors() {
  errors.materiaId = ''
  errors.fechaSesion = ''
  errors.horaInicio = ''
  errors.motivo = ''
  successMessage.value = null
  submitError.value = null
}

function validateForm(): boolean {
  clearErrors()
  let isValid = true

  if (!form.materiaId) {
    errors.materiaId = 'Selecciona la materia de la tutoría.'
    isValid = false
  }

  if (!form.fechaSesion) {
    errors.fechaSesion = 'Selecciona la fecha de la sesión en el calendario.'
    isValid = false
  } else if (form.fechaSesion < today.value) {
    errors.fechaSesion = 'La fecha de la sesión no puede estar en el pasado.'
    isValid = false
  } else if (tutorDiasAtencion.value.length > 0) {
    const [y, m, d] = form.fechaSesion.split('-').map(Number)
    const dayDate = new Date(y, m - 1, d)
    const isoDay = dayDate.getDay() === 0 ? 7 : dayDate.getDay()
    if (!tutorDiasAtencion.value.includes(isoDay)) {
      errors.fechaSesion = 'El tutor no atiende en el día seleccionado.'
      isValid = false
    }
  }

  if (!form.horaInicio) {
    errors.horaInicio = 'Selecciona la hora de inicio de la sesión.'
    isValid = false
  } else if (isHoraOutOfRange.value) {
    errors.horaInicio = `La hora de inicio debe estar entre ${minHoraInicio.value} y ${maxHoraInicio.value} para finalizar dentro del horario de atención del tutor.`
    isValid = false
  } else if (sessionConflict.value) {
    const confStart = formatTime(sessionConflict.value.horaInicio)
    const confEnd = formatTime(sessionConflict.value.horaFin)
    errors.horaInicio = `El horario seleccionado se traslapa con una sesión ya agendada (${confStart} - ${confEnd}).`
    isValid = false
  }

  if (!form.motivo.trim()) {
    errors.motivo = 'Escribe el motivo de la sesión.'
    isValid = false
  }

  return isValid
}

async function handleSubmit() {
  if (!validateForm()) return

  if (!Number.isInteger(tutorId.value) || tutorId.value <= 0) {
    submitError.value = 'El tutor seleccionado no es válido.'
    return
  }

  isSubmitting.value = true
  submitError.value = null
  successMessage.value = null

  try {
    await sessionBookingService.programarSesion({
      tutorId: tutorId.value,
      materiaId: Number(form.materiaId),
      fechaSesion: form.fechaSesion,
      horaInicio: form.horaInicio,
      motivo: form.motivo.trim()
    })

    successMessage.value = 'La sesión fue programada correctamente.'

    const bookedFecha = form.fechaSesion
    form.materiaId = ''
    form.horaInicio = ''
    form.motivo = ''

    if (bookedFecha && tutorId.value) {
      await fetchAvailability(tutorId.value, bookedFecha)
    }
  } catch (error: unknown) {
    if (axios.isAxiosError<ApiProblemDetails>(error)) {
      if (!error.response) {
        submitError.value =
          'No fue posible conectar con el servidor. Verifica que el backend esté encendido.'
      } else {
        submitError.value =
          error.response.data?.detail ||
          error.response.data?.title ||
          'No fue posible programar la sesión.'
      }
    } else {
      submitError.value = 'Ocurrió un error inesperado al programar la sesión.'
    }
  } finally {
    isSubmitting.value = false
  }
}

onMounted(() => {
  loadTutor()
})
</script>

<template>
  <div class="w-full max-w-6xl mx-auto flex flex-col gap-6">
    <div class="flex flex-col gap-4">
      <RouterLink
        to="/estudiante/explorar-tutores"
        class="inline-flex items-center gap-2 text-sm font-semibold text-on-surface-variant hover:text-primary transition-colors self-start"
      >
        <span class="material-symbols-outlined text-[19px]">arrow_back</span>
        Volver a tutores
      </RouterLink>

      <div>
        <h1 class="text-3xl font-bold font-headline text-on-surface tracking-tight">
          Programar Sesión
        </h1>
        <p class="mt-2 text-base text-on-surface-variant max-w-3xl">
          Selecciona la materia, fecha y horario en que deseas recibir la tutoría.
        </p>
      </div>
    </div>

    <BaseAlert
      v-if="tutorLoadError"
      type="warning"
      title="Información del tutor"
      :message="tutorLoadError"
      :dismissible="false"
    />

    <div class="grid grid-cols-1 lg:grid-cols-[minmax(0,1fr)_340px] gap-6 items-start">
      <BaseCard padding="lg">
        <template #header>
          <div class="flex items-center gap-3">
            <div
              class="w-10 h-10 rounded-xl bg-primary-container text-on-primary-container flex items-center justify-center"
            >
              <span class="material-symbols-outlined text-[22px]">event</span>
            </div>
            <div>
              <h2 class="text-lg font-bold font-headline text-on-surface">Datos de la tutoría</h2>
              <p class="text-sm text-on-surface-variant mt-0.5">
                Completa la información para solicitar tu sesión.
              </p>
            </div>
          </div>
        </template>

        <form class="flex flex-col gap-6" @submit.prevent="handleSubmit">
          <BaseAlert
            v-if="materiasError"
            type="error"
            title="No se pudieron cargar las materias"
            :message="materiasError"
            :dismissible="false"
          />

          <BaseAlert
            v-if="submitError"
            type="error"
            title="No se pudo programar la sesión"
            :message="submitError"
            @dismiss="submitError = null"
          />

          <BaseAlert
            v-if="successMessage"
            type="success"
            title="Sesión programada"
            :message="successMessage"
            @dismiss="successMessage = null"
          />

          <div class="flex flex-col gap-2 relative group w-full">
            <label
              for="materiaId"
              class="text-sm font-semibold text-on-surface transition-colors group-focus-within:text-primary"
            >
              Materia <span class="text-error">*</span>
            </label>
            <div class="relative">
              <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                <span
                  class="material-symbols-outlined text-on-surface-variant text-[20px] group-focus-within:text-primary transition-colors"
                >
                  menu_book
                </span>
              </div>
              <select
                id="materiaId"
                v-model="form.materiaId"
                name="materiaId"
                required
                :disabled="isLoadingMaterias || isLoadingTutor || !tutor"
                :class="[
                  'w-full rounded-lg bg-surface-container-low border border-outline-variant focus:outline-none focus:ring-2 focus:ring-primary focus:bg-surface-container-lowest focus:border-transparent text-base text-on-surface font-body transition-all disabled:opacity-60 disabled:cursor-not-allowed h-12 px-4 appearance-none pl-10 pr-10',
                  errors.materiaId ? 'border-error bg-error-container/20 focus:ring-error' : ''
                ]"
              >
                <option value="" disabled :selected="form.materiaId === ''">
                  Selecciona una materia
                </option>
                <option v-for="opcion in materiaOptions" :key="opcion.value" :value="opcion.value">
                  {{ opcion.label }}
                </option>
              </select>
              <span
                class="material-symbols-outlined absolute right-4 top-3 text-on-surface-variant pointer-events-none text-[20px]"
              >
                expand_more
              </span>
            </div>
            <p
              v-if="errors.materiaId"
              class="text-xs text-error font-medium flex items-center gap-1 mt-0.5"
            >
              <span class="material-symbols-outlined text-[16px]">info</span>
              {{ errors.materiaId }}
            </p>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-6 items-start">
            <div class="flex flex-col gap-2">
              <BaseCalendar
                v-model="form.fechaSesion"
                label="Fecha de la sesión"
                :min-date="today"
                :allowed-days-of-week="tutorDiasAtencion"
                :error="errors.fechaSesion"
                required
              />
              <div
                v-if="form.fechaSesion"
                class="text-xs text-primary font-semibold capitalize flex items-center gap-1.5 px-1 mt-1"
              >
                <span class="material-symbols-outlined text-[16px]">calendar_month</span>
                {{ formattedSelectedDate }}
              </div>
            </div>

            <div class="flex flex-col gap-3">
              <div class="flex justify-between items-center">
                <label class="text-sm font-semibold text-on-surface">
                  Horario de la sesión <span class="text-error">*</span>
                </label>
                <span
                  v-if="tutorHoraInicio && tutorHoraFin"
                  class="text-xs text-on-surface-variant font-medium"
                >
                  Atención: {{ formatTime(tutorHoraInicio) }} - {{ formatTime(tutorHoraFin) }}
                </span>
              </div>

              <div
                v-if="!form.fechaSesion"
                class="rounded-xl border border-dashed border-outline-variant/60 p-6 text-center text-on-surface-variant flex flex-col items-center justify-center gap-2 min-h-[220px] bg-surface-container-lowest"
              >
                <span class="material-symbols-outlined text-3xl text-outline">event_available</span>
                <p class="text-sm font-medium">
                  Selecciona una fecha disponible en el calendario para consultar los horarios del
                  tutor.
                </p>
              </div>

              <div
                v-else-if="isLoadingAvailability"
                class="rounded-xl border border-outline-variant/30 bg-surface-container-lowest p-6 text-center text-on-surface-variant flex flex-col items-center justify-center gap-2 min-h-[220px]"
              >
                <span class="material-symbols-outlined animate-spin text-primary text-2xl"
                  >progress_activity</span
                >
                <p class="text-sm">Consultando horarios del tutor...</p>
              </div>

              <div
                v-else-if="availabilityError"
                class="rounded-xl border border-error/30 bg-error-container/20 p-6 text-center text-error flex flex-col items-center justify-center gap-2 min-h-[220px]"
              >
                <span class="material-symbols-outlined text-2xl">error</span>
                <p class="text-sm font-medium">{{ availabilityError }}</p>
              </div>

              <div
                v-else-if="!availability?.atiendeEseDia"
                class="rounded-xl border border-outline-variant/30 bg-surface-container-low p-6 text-center text-on-surface-variant flex flex-col items-center justify-center gap-2 min-h-[220px]"
              >
                <span class="material-symbols-outlined text-2xl text-outline">event_busy</span>
                <p class="text-sm font-medium">El tutor no atiende en la fecha seleccionada.</p>
              </div>

              <div v-else class="flex flex-col gap-4">
                <div
                  v-if="availability?.sesionesOcupadas && availability.sesionesOcupadas.length > 0"
                  class="rounded-xl border border-error/25 bg-error-container/10 p-3.5 flex flex-col gap-2"
                >
                  <div class="flex items-center gap-1.5 text-xs font-bold text-error">
                    <span class="material-symbols-outlined text-[16px]">event_busy</span>
                    <span>Sesiones ocupadas del tutor este día:</span>
                  </div>
                  <div class="flex flex-wrap gap-2">
                    <span
                      v-for="s in availability.sesionesOcupadas"
                      :key="`${s.horaInicio}-${s.horaFin}`"
                      :class="[
                        'inline-flex items-center gap-1.5 px-2.5 py-1 rounded-lg text-xs font-semibold border shadow-xs transition-all',
                        isConflictSession(s)
                          ? 'bg-error text-on-error border-error ring-2 ring-error ring-offset-1 font-bold'
                          : 'bg-surface text-on-surface border-error/30'
                      ]"
                    >
                      <span
                        :class="[
                          'w-2 h-2 rounded-full',
                          isConflictSession(s) ? 'bg-white' : 'bg-error'
                        ]"
                      />
                      <span :class="isConflictSession(s) ? '' : 'line-through text-outline'">
                        {{ formatTime(s.horaInicio) }} - {{ formatTime(s.horaFin) }}
                      </span>
                      <span
                        :class="[
                          'text-[11px] font-bold uppercase',
                          isConflictSession(s) ? 'text-white' : 'text-error'
                        ]"
                      >
                        {{ isConflictSession(s) ? '(Conflicto)' : '(Agendado)' }}
                      </span>
                    </span>
                  </div>
                </div>

                <div
                  v-else
                  class="rounded-xl border border-emerald-500/20 bg-emerald-500/10 px-3.5 py-2.5 flex items-center gap-2 text-xs text-emerald-800 dark:text-emerald-300"
                >
                  <span class="material-symbols-outlined text-[18px]">check_circle</span>
                  <span
                    >Sin sesiones agendadas para este día. El tutor tiene todo su horario
                    disponible.</span
                  >
                </div>

                <div class="flex flex-col gap-2">
                  <BaseTimePicker
                    id="horaInicio"
                    v-model="form.horaInicio"
                    name="horaInicio"
                    label="Hora de inicio de la sesión"
                    :min-time="minHoraInicio"
                    :max-time="maxHoraInicio"
                    :occupied-slots="availability?.sesionesOcupadas ?? []"
                    :error="horaInicioEffectiveError"
                    required
                  />

                  <div
                    v-if="form.horaInicio && !horaInicioEffectiveError"
                    class="rounded-lg px-3 py-2 text-xs flex items-center gap-2 transition-all bg-primary-container/30 text-on-primary-container border border-primary/20 font-medium"
                  >
                    <span class="material-symbols-outlined text-[17px]">
                      schedule
                    </span>
                    <span>
                      Sesión solicitada:
                      <strong>{{ form.horaInicio }} - {{ selectedSessionEndTime }}</strong> (1 hora
                      de duración).
                    </span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="flex flex-col gap-2 group">
            <label
              for="motivo"
              class="text-sm font-semibold text-on-surface transition-colors group-focus-within:text-primary"
            >
              Motivo de la sesión <span class="text-error">*</span>
            </label>
            <textarea
              id="motivo"
              v-model="form.motivo"
              name="motivo"
              rows="4"
              maxlength="500"
              placeholder="Describe brevemente el tema o la duda que deseas trabajar con el tutor."
              :class="[
                'w-full resize-y min-h-[110px] bg-surface-container-low px-4 py-3 rounded-lg text-base text-on-surface placeholder:text-outline-variant outline-none focus:ring-2 focus:ring-primary focus:bg-surface-container-lowest border border-outline-variant focus:border-transparent transition-all',
                errors.motivo ? 'border-error bg-error-container/20 focus:ring-error' : ''
              ]"
              required
            />
            <div class="flex items-center justify-between gap-3">
              <p
                v-if="errors.motivo"
                class="text-xs text-error font-medium flex items-center gap-1"
              >
                <span class="material-symbols-outlined text-[16px]">info</span>
                {{ errors.motivo }}
              </p>
              <span v-else class="text-xs text-on-surface-variant">
                Explica qué necesitas reforzar para que el tutor pueda prepararse.
              </span>
              <span class="text-xs text-on-surface-variant shrink-0">
                {{ form.motivo.length }}/500
              </span>
            </div>
          </div>

          <div
            class="rounded-xl bg-surface-container-low px-4 py-3 flex items-start gap-3 text-sm text-on-surface-variant"
          >
            <span class="material-symbols-outlined text-[20px] text-primary mt-0.5">info</span>
            <p>
              Las sesiones tienen una duración de 1 hora. Selecciona la hora de inicio deseada dentro del horario de atención del tutor.
            </p>
          </div>

          <div class="flex flex-col-reverse sm:flex-row sm:justify-end gap-3 pt-1">
            <RouterLink
              to="/estudiante/explorar-tutores"
              class="inline-flex items-center justify-center py-3.5 px-5 text-sm font-semibold rounded-lg border border-outline hover:bg-surface-container-low text-primary transition-all"
            >
              Cancelar
            </RouterLink>

            <BaseButton
              type="submit"
              variant="primary"
              size="md"
              :loading="isSubmitting"
              :disabled="isSubmitting || !tutor || Boolean(sessionConflict) || isHoraOutOfRange"
            >
              <template #loading>Programando...</template>
              <span class="material-symbols-outlined text-[19px]">event_available</span>
              Programar sesión
            </BaseButton>
          </div>
        </form>
      </BaseCard>

      <div class="flex flex-col gap-4 lg:sticky lg:top-28">
        <BaseCard padding="md">
          <div v-if="isLoadingTutor" class="flex items-center gap-3 py-4">
            <span class="material-symbols-outlined animate-spin text-primary"
              >progress_activity</span
            >
            <span class="text-sm text-on-surface-variant">Cargando tutor...</span>
          </div>

          <div v-else class="flex flex-col gap-5">
            <div class="flex items-center gap-4">
              <img
                v-if="tutor?.fotografiaUrl"
                :src="tutor.fotografiaUrl"
                :alt="tutor.nombreCompleto"
                class="w-16 h-16 rounded-full object-cover shadow-sm"
              />
              <div
                v-else
                class="w-16 h-16 rounded-full bg-primary-container text-on-primary-container flex items-center justify-center text-xl font-bold shrink-0"
              >
                {{ tutorInitial }}
              </div>

              <div class="min-w-0">
                <p class="text-xs font-semibold uppercase tracking-wider text-secondary">
                  Tutor seleccionado
                </p>
                <h3 class="font-headline text-lg font-bold text-on-surface truncate mt-0.5">
                  {{ tutor?.nombreCompleto || `Tutor #${tutorId}` }}
                </h3>
                <p class="text-sm text-on-surface-variant truncate">
                  {{ tutor?.materias?.join(', ') || 'Tutor académico' }}
                </p>
              </div>
            </div>

            <div v-if="tutor" class="space-y-3 pt-4 border-t border-outline-variant/30">
              <div class="flex items-center gap-2 text-sm text-on-surface-variant">
                <span class="material-symbols-outlined text-[19px] text-primary">school</span>
                <span>{{ tutor.universidad }}</span>
              </div>
              <div class="flex items-center gap-2 text-sm text-on-surface-variant">
                <span class="material-symbols-outlined text-[19px] text-primary"
                  >workspace_premium</span
                >
                <span>{{ tutor.aniosExperiencia }} años de experiencia</span>
              </div>
              <div
                class="flex items-start gap-2 text-sm text-on-surface-variant pt-2 border-t border-outline-variant/20"
              >
                <span class="material-symbols-outlined text-[19px] text-primary mt-0.5"
                  >calendar_today</span
                >
                <div>
                  <p class="font-semibold text-on-surface text-xs">Días de atención</p>
                  <p class="text-xs text-on-surface-variant mt-0.5">{{ tutorDiasAtencionTexto }}</p>
                </div>
              </div>
              <div
                v-if="tutorHoraInicio && tutorHoraFin"
                class="flex items-start gap-2 text-sm text-on-surface-variant"
              >
                <span class="material-symbols-outlined text-[19px] text-primary mt-0.5"
                  >schedule</span
                >
                <div>
                  <p class="font-semibold text-on-surface text-xs">Horario de atención</p>
                  <p class="text-xs text-on-surface-variant mt-0.5">
                    {{ formatTime(tutorHoraInicio) }} - {{ formatTime(tutorHoraFin) }}
                  </p>
                </div>
              </div>
            </div>
          </div>
        </BaseCard>

        <div class="rounded-xl border border-outline-variant/20 bg-surface-container-low p-5">
          <div class="flex items-start gap-3">
            <span class="material-symbols-outlined text-primary text-[22px]">schedule</span>
            <div class="flex-1">
              <h3 class="text-sm font-bold text-on-surface">Horarios y Disponibilidad</h3>
              <p class="text-sm text-on-surface-variant mt-1 leading-relaxed">
                Revisa los días y bloques disponibles para planificar tu sesión.
              </p>
              <BaseButton variant="outline" size="sm" class="mt-3" @click="showAvailability = true">
                Ver detalle en modal
              </BaseButton>
            </div>
          </div>
        </div>
      </div>
    </div>

    <AvailabilityModal v-model="showAvailability" :tutor-id="tutorId" />
  </div>
</template>
