<script setup lang="ts">
import { ref, watch } from 'vue'
import { BaseModal, BaseInput } from '@/components/ui'
import { useAvailability } from '../composables/useAvailability'

interface Props {
  modelValue: boolean
  tutorId: number | null
}

const props = defineProps<Props>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
}>()

const { availability, isLoading, error, fetchAvailability } = useAvailability()

function getLocalDateString(): string {
  const now = new Date()
  const year = now.getFullYear()
  const month = String(now.getMonth() + 1).padStart(2, '0')
  const day = String(now.getDate()).padStart(2, '0')
  return `${year}-${month}-${day}`
}

const fecha = ref(getLocalDateString())

watch(
  () => [props.modelValue, props.tutorId] as const,
  ([isOpen, tutorId]) => {
    if (isOpen && tutorId) {
      fetchAvailability(tutorId, fecha.value)
    }
  }
)

watch(fecha, newFecha => {
  if (props.modelValue && props.tutorId) {
    fetchAvailability(props.tutorId, newFecha)
  }
})

const diasNombre: Record<number, string> = {
  1: 'Lunes',
  2: 'Martes',
  3: 'Miércoles',
  4: 'Jueves',
  5: 'Viernes',
  6: 'Sábado',
  7: 'Domingo'
}

function formatTime(time?: string | null): string {
  if (!time) return ''
  return time.slice(0, 5)
}
</script>

<template>
  <BaseModal
    :model-value="modelValue"
    title="Horarios y Disponibilidad"
    max-width="lg"
    @update:model-value="emit('update:modelValue', $event)"
  >
    <div class="flex flex-col gap-5">
      <BaseInput v-model="fecha" type="date" label="Selecciona una fecha" />

      <div v-if="isLoading" class="text-sm text-on-surface-variant text-center py-8">
        Consultando disponibilidad...
      </div>

      <div v-else-if="error" class="text-sm text-error text-center py-8">
        {{ error }}
      </div>

      <template v-else-if="availability">
        <div class="text-sm text-on-surface-variant">
          <span class="font-semibold text-on-surface">{{ availability.nombreCompleto }}</span>
          atiende:
          <span v-for="dia in availability.diasAtencion" :key="dia" class="inline-block mr-1">
            {{ diasNombre[dia] }}<span v-if="dia !== availability.diasAtencion.at(-1)">,</span>
          </span>
          <template v-if="availability.horaInicioAtencion && availability.horaFinAtencion">
            de {{ formatTime(availability.horaInicioAtencion) }} a
            {{ formatTime(availability.horaFinAtencion) }}
          </template>
        </div>

        <div
          v-if="!availability.atiendeEseDia"
          class="text-sm text-on-surface-variant text-center py-8"
        >
          El tutor no atiende en la fecha seleccionada.
        </div>

        <template v-else>
          <div
            v-if="availability.sesionesOcupadas && availability.sesionesOcupadas.length > 0"
            class="rounded-lg border border-error/30 bg-error-container/10 p-3 text-xs flex flex-col gap-1.5"
          >
            <span class="font-bold text-error flex items-center gap-1">
              <span class="material-symbols-outlined text-[16px]">event_busy</span>
              Sesiones agendadas (ocupadas):
            </span>
            <div class="flex flex-wrap gap-1.5">
              <span
                v-for="s in availability.sesionesOcupadas"
                :key="`${s.horaInicio}-${s.horaFin}`"
                class="px-2 py-0.5 rounded bg-surface text-error border border-error/20 font-semibold"
              >
                {{ formatTime(s.horaInicio) }} - {{ formatTime(s.horaFin) }}
              </span>
            </div>
          </div>

          <div class="grid grid-cols-2 sm:grid-cols-3 gap-2">
            <div
              v-for="bloque in availability.bloques"
              :key="`${bloque.horaInicio}-${bloque.horaFin}`"
              :class="[
                'rounded-lg py-2 px-2 text-center text-xs font-semibold border whitespace-nowrap',
                bloque.disponible
                  ? 'bg-secondary-fixed/40 text-on-secondary-fixed border-secondary/30'
                  : 'bg-surface-container-high text-on-surface-variant border-outline-variant/30 line-through'
              ]"
            >
              {{ formatTime(bloque.horaInicio) }} - {{ formatTime(bloque.horaFin) }}
            </div>
          </div>

          <div class="flex items-center gap-4 text-xs text-on-surface-variant">
            <span class="flex items-center gap-1.5">
              <span class="w-3 h-3 rounded-full bg-secondary-fixed/40 border border-secondary/30" />
              Disponible
            </span>
            <span class="flex items-center gap-1.5">
              <span
                class="w-3 h-3 rounded-full bg-surface-container-high border border-outline-variant/30"
              />
              Ocupado
            </span>
          </div>
        </template>
      </template>
    </div>
  </BaseModal>
</template>
