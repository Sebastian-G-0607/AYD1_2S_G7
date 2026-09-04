<script setup lang="ts">
import axios from 'axios'
import { computed, onMounted, reactive, ref } from 'vue'
import { RouterLink, useRoute } from 'vue-router'
import {
  BaseAlert,
  BaseButton,
  BaseCard,
  BaseInput,
  BaseSelect,
  type SelectOption
} from '@/components/ui'
import { useMaterias } from '@/composables/useMaterias'
import { tutorsExplorerService } from '@/features/tutors-explorer/services/tutorsExplorer.service'
import { sessionBookingService } from '@/features/session-booking/services/sessionBooking.service'
import type { TutorExplorerItem } from '@/features/tutors-explorer/types'
import type { ApiProblemDetails } from '@/features/session-booking/types'

const route = useRoute()
const { materias, isLoading: isLoadingMaterias, error: materiasError } = useMaterias()

const tutor = ref<TutorExplorerItem | null>(null)
const isLoadingTutor = ref(false)
const tutorLoadError = ref<string | null>(null)
const successMessage = ref<string | null>(null)
const submitError = ref<string | null>(null)
const isSubmitting = ref(false)

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

const materiaOptions = computed<SelectOption[]>(() =>
  materias.value.map(materia => ({
    value: materia.id,
    label: materia.nombre
  }))
)

const today = computed(() => {
  const now = new Date()
  const year = now.getFullYear()
  const month = String(now.getMonth() + 1).padStart(2, '0')
  const day = String(now.getDate()).padStart(2, '0')
  return `${year}-${month}-${day}`
})

const tutorInitial = computed(() => tutor.value?.nombre?.charAt(0).toUpperCase() || 'T')

async function loadTutor() {
  if (!Number.isInteger(tutorId.value) || tutorId.value <= 0) {
    tutorLoadError.value = 'El tutor seleccionado no es válido.'
    return
  }

  isLoadingTutor.value = true
  tutorLoadError.value = null

  try {
    const tutors = await tutorsExplorerService.getTutors()
    tutor.value = tutors.find(item => item.id === tutorId.value) ?? null

    if (!tutor.value) {
      tutorLoadError.value = 'No fue posible cargar la información del tutor seleccionado.'
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
    errors.fechaSesion = 'Selecciona la fecha de la sesión.'
    isValid = false
  } else if (form.fechaSesion < today.value) {
    errors.fechaSesion = 'La fecha de la sesión no puede estar en el pasado.'
    isValid = false
  }

  if (!form.horaInicio) {
    errors.horaInicio = 'Selecciona la hora de inicio.'
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

    form.materiaId = ''
    form.fechaSesion = ''
    form.horaInicio = ''
    form.motivo = ''
  } catch (error: unknown) {
    if (axios.isAxiosError<ApiProblemDetails>(error)) {
      if (!error.response) {
        submitError.value = 'No fue posible conectar con el servidor. Verifica que el backend esté encendido.'
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
              <h2 class="text-lg font-bold font-headline text-on-surface">
                Datos de la tutoría
              </h2>
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

          <BaseSelect
            v-model="form.materiaId"
            id="materia"
            name="materia"
            label="Materia"
            placeholder="Selecciona una materia"
            icon="menu_book"
            :options="materiaOptions"
            :disabled="isLoadingMaterias"
            :error="errors.materiaId"
            required
          />

          <div class="grid grid-cols-1 sm:grid-cols-2 gap-5">
            <BaseInput
              v-model="form.fechaSesion"
              id="fechaSesion"
              name="fechaSesion"
              label="Fecha"
              type="date"
              :min="today"
              :error="errors.fechaSesion"
              required
            />

            <BaseInput
              v-model="form.horaInicio"
              id="horaInicio"
              name="horaInicio"
              label="Hora de inicio"
              type="time"
              :error="errors.horaInicio"
              required
            />
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
              rows="5"
              maxlength="500"
              placeholder="Describe brevemente el tema o la duda que deseas trabajar con el tutor."
              :class="[
                'w-full resize-y min-h-[132px] bg-surface-container-low px-4 py-3 rounded-lg text-base text-on-surface placeholder:text-outline-variant outline-none focus:ring-2 focus:ring-primary focus:bg-surface-container-lowest border border-outline-variant focus:border-transparent transition-all',
                errors.motivo ? 'border-error bg-error-container/20 focus:ring-error' : ''
              ]"
              required
            />
            <div class="flex items-center justify-between gap-3">
              <p v-if="errors.motivo" class="text-xs text-error font-medium flex items-center gap-1">
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
              La disponibilidad definitiva será validada al momento de programar la sesión.
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
              :disabled="isSubmitting || !tutor"
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
            <span class="material-symbols-outlined animate-spin text-primary">progress_activity</span>
            <span class="text-sm text-on-surface-variant">Cargando tutor...</span>
          </div>

          <div v-else class="flex flex-col gap-5">
            <div class="flex items-center gap-4">
              <img
                v-if="tutor?.fotografiaUrl"
                :src="tutor.fotografiaUrl"
                :alt="tutor.nombre"
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
                  {{ tutor?.nombre || `Tutor #${tutorId}` }}
                </h3>
                <p class="text-sm text-on-surface-variant truncate">
                  {{ tutor?.titulo || tutor?.especialidad || 'Tutor académico' }}
                </p>
              </div>
            </div>

            <div v-if="tutor" class="space-y-3 pt-4 border-t border-outline-variant/30">
              <div class="flex items-center gap-2 text-sm text-on-surface-variant">
                <span class="material-symbols-outlined text-[19px] text-primary">school</span>
                <span>{{ tutor.universidad }}</span>
              </div>
              <div class="flex items-center gap-2 text-sm text-on-surface-variant">
                <span class="material-symbols-outlined text-[19px] text-primary">workspace_premium</span>
                <span>{{ tutor.aniosExperiencia }} años de experiencia</span>
              </div>
              <div class="flex items-center gap-2 text-sm text-on-surface-variant">
                <span class="material-symbols-outlined text-[19px] text-[#eab308]">star</span>
                <span>{{ tutor.rating.toFixed(1) }} · {{ tutor.totalResenas }} reseñas</span>
              </div>
            </div>
          </div>
        </BaseCard>

        <div class="rounded-xl border border-outline-variant/20 bg-surface-container-low p-5">
          <div class="flex items-start gap-3">
            <span class="material-symbols-outlined text-primary text-[22px]">schedule</span>
            <div>
              <h3 class="text-sm font-bold text-on-surface">Antes de programar</h3>
              <p class="text-sm text-on-surface-variant mt-1 leading-relaxed">
                El sistema verificará que el tutor atienda el día y hora elegidos y que no exista
                otro compromiso en ese horario.
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
