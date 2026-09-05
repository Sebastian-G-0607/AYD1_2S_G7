<script setup lang="ts">
import { computed } from 'vue'
import { BaseButton } from '@/components/ui'
import { useTutorSchedule } from '../composables/useTutorSchedule'

const {
  days,
  selectedDays,
  horaInicio,
  horaFin,
  isSaving,
  errorMessage,
  successMessage,
  canSubmit,
  toggleDay,
  isDaySelected,
  clearMessages,
  saveSchedule
} = useTutorSchedule()

const selectedDayNames = computed(() =>
  days
    .filter(day => selectedDays.value.includes(day.id))
    .map(day => day.label)
)

function handleTimeChange() {
  clearMessages()
}
</script>

<template>
  <div class="flex flex-col w-full max-w-5xl mx-auto">
    <!-- Encabezado -->
    <div class="mb-8">
      <h1 class="text-3xl font-bold font-headline text-on-surface tracking-tight mb-2">
        Configuración de Horarios
      </h1>

      <p class="text-base text-on-surface-variant font-body">
        Define los días y el horario en los que estarás disponible para brindar tutorías.
      </p>
    </div>

    <!-- Mensaje de éxito -->
    <div
      v-if="successMessage"
      class="flex items-start gap-3 p-4 mb-6 rounded-xl border border-green-200 bg-green-50 text-green-800"
    >
      <span class="material-symbols-outlined text-[22px]">
        check_circle
      </span>

      <div>
        <p class="font-semibold">
          Horario actualizado
        </p>

        <p class="text-sm mt-1">
          {{ successMessage }}
        </p>
      </div>
    </div>

    <!-- Mensaje de error -->
    <div
      v-if="errorMessage"
      class="flex items-start gap-3 p-4 mb-6 rounded-xl border border-red-200 bg-red-50 text-red-800"
    >
      <span class="material-symbols-outlined text-[22px]">
        error
      </span>

      <div>
        <p class="font-semibold">
          No fue posible guardar el horario
        </p>

        <p class="text-sm mt-1">
          {{ errorMessage }}
        </p>
      </div>
    </div>

    <form
      class="grid grid-cols-1 lg:grid-cols-3 gap-6"
      @submit.prevent="saveSchedule"
    >
      <!-- Configuración principal -->
      <section
        class="lg:col-span-2 bg-surface-container-lowest rounded-2xl border border-outline-variant/20 shadow-sm p-6 sm:p-8"
      >
        <div class="flex items-center gap-3 mb-7">
          <div
            class="w-11 h-11 rounded-xl bg-primary/10 text-primary flex items-center justify-center"
          >
            <span class="material-symbols-outlined">
              calendar_month
            </span>
          </div>

          <div>
            <h2 class="text-xl font-bold text-on-surface font-headline">
              Disponibilidad semanal
            </h2>

            <p class="text-sm text-on-surface-variant mt-1">
              Selecciona los días en los que podrás atender estudiantes.
            </p>
          </div>
        </div>

        <!-- Días -->
        <div class="mb-8">
          <label class="block text-sm font-semibold text-on-surface mb-3">
            Días de atención
          </label>

          <div class="grid grid-cols-4 sm:grid-cols-7 gap-2">
            <button
              v-for="day in days"
              :key="day.id"
              type="button"
              :aria-pressed="isDaySelected(day.id)"
              :class="[
                'min-h-12 rounded-xl border text-sm font-semibold transition-all duration-200',
                isDaySelected(day.id)
                  ? 'bg-primary text-on-primary border-primary shadow-sm'
                  : 'bg-surface text-on-surface-variant border-outline-variant/50 hover:border-primary hover:text-primary hover:bg-primary/5'
              ]"
              @click="toggleDay(day.id)"
            >
              {{ day.shortLabel }}
            </button>
          </div>

          <p class="text-xs text-on-surface-variant mt-3">
            Puedes seleccionar uno o varios días.
          </p>
        </div>

        <!-- Horario -->
        <div>
          <div class="flex items-center gap-2 mb-3">
            <span class="material-symbols-outlined text-primary text-[20px]">
              schedule
            </span>

            <label class="text-sm font-semibold text-on-surface">
              Horario de atención
            </label>
          </div>

          <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <!-- Hora inicio -->
            <div>
              <label
                for="horaInicio"
                class="block text-sm text-on-surface-variant mb-2"
              >
                Hora de inicio
              </label>

              <input
                id="horaInicio"
                v-model="horaInicio"
                type="time"
                class="w-full h-12 px-4 rounded-xl bg-surface border border-outline-variant/50 text-on-surface outline-none transition-colors focus:border-primary focus:ring-2 focus:ring-primary/10"
                @change="handleTimeChange"
              />
            </div>

            <!-- Hora fin -->
            <div>
              <label
                for="horaFin"
                class="block text-sm text-on-surface-variant mb-2"
              >
                Hora de finalización
              </label>

              <input
                id="horaFin"
                v-model="horaFin"
                type="time"
                class="w-full h-12 px-4 rounded-xl bg-surface border border-outline-variant/50 text-on-surface outline-none transition-colors focus:border-primary focus:ring-2 focus:ring-primary/10"
                @change="handleTimeChange"
              />
            </div>
          </div>

          <div
            class="flex items-start gap-2 mt-4 p-3 rounded-xl bg-surface-container-low text-on-surface-variant"
          >
            <span class="material-symbols-outlined text-[19px] mt-0.5">
              info
            </span>

            <p class="text-sm leading-relaxed">
              El mismo rango de horario se aplicará a todos los días que selecciones.
            </p>
          </div>
        </div>

        <!-- Acciones -->
        <div
          class="flex flex-col-reverse sm:flex-row sm:justify-end gap-3 mt-8 pt-6 border-t border-outline-variant/20"
        >
          <BaseButton
            type="submit"
            variant="primary"
            size="md"
            :loading="isSaving"
            :disabled="!canSubmit"
          >
            <template #loading>
              Guardando...
            </template>

            Guardar cambios
          </BaseButton>
        </div>
      </section>

      <!-- Resumen -->
      <aside
        class="bg-surface-container-lowest rounded-2xl border border-outline-variant/20 shadow-sm p-6 h-fit"
      >
        <div class="flex items-center gap-3 mb-6">
          <div
            class="w-10 h-10 rounded-xl bg-secondary/10 text-secondary flex items-center justify-center"
          >
            <span class="material-symbols-outlined">
              event_available
            </span>
          </div>

          <div>
            <h2 class="text-lg font-bold text-on-surface font-headline">
              Resumen
            </h2>

            <p class="text-xs text-on-surface-variant mt-0.5">
              Tu disponibilidad actual
            </p>
          </div>
        </div>

        <div class="space-y-5">
          <!-- Días seleccionados -->
          <div>
            <p
              class="text-xs font-semibold uppercase tracking-wide text-on-surface-variant mb-2"
            >
              Días seleccionados
            </p>

            <div
              v-if="selectedDayNames.length > 0"
              class="flex flex-wrap gap-2"
            >
              <span
                v-for="dayName in selectedDayNames"
                :key="dayName"
                class="px-3 py-1.5 rounded-full bg-primary/10 text-primary text-xs font-semibold"
              >
                {{ dayName }}
              </span>
            </div>

            <p
              v-else
              class="text-sm text-on-surface-variant"
            >
              Aún no has seleccionado días.
            </p>
          </div>

          <div class="h-px bg-outline-variant/20" />

          <!-- Horario -->
          <div>
            <p
              class="text-xs font-semibold uppercase tracking-wide text-on-surface-variant mb-2"
            >
              Horario
            </p>

            <div
              v-if="horaInicio && horaFin"
              class="flex items-center gap-2 text-on-surface font-semibold"
            >
              <span class="material-symbols-outlined text-primary text-[20px]">
                schedule
              </span>

              <span>
                {{ horaInicio }} - {{ horaFin }}
              </span>
            </div>

            <p
              v-else
              class="text-sm text-on-surface-variant"
            >
              Define una hora de inicio y finalización.
            </p>
          </div>

          <div class="h-px bg-outline-variant/20" />

          <!-- Estado -->
          <div
            class="flex items-start gap-3 p-3 rounded-xl bg-surface-container-low"
          >
            <span class="material-symbols-outlined text-primary text-[20px]">
              verified
            </span>

            <p class="text-xs leading-relaxed text-on-surface-variant">
              Los cambios se guardarán en tu perfil de tutor y serán utilizados
              para gestionar tu disponibilidad.
            </p>
          </div>
        </div>
      </aside>
    </form>
  </div>
</template>