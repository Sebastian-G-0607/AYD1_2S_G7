<script setup lang="ts">
import { ref, watch } from 'vue'
import { BaseModal, BaseInput, BaseBadge } from '@/components/ui'
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

const fecha = ref(new Date().toISOString().slice(0, 10))

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
          <span
            v-for="dia in availability.diasAtencion"
            :key="dia"
            class="inline-block mr-1"
          >
            {{ diasNombre[dia] }}<span v-if="dia !== availability.diasAtencion.at(-1)">,</span>
          </span>
          <template v-if="availability.horaInicioAtencion && availability.horaFinAtencion">
            de {{ availability.horaInicioAtencion }} a {{ availability.horaFinAtencion }}
          </template>
        </div>

        <div v-if="!availability.atiendeEseDia" class="text-sm text-on-surface-variant text-center py-8">
          El tutor no atiende en la fecha seleccionada.
        </div>

        <div v-else class="grid grid-cols-3 sm:grid-cols-4 gap-2">
          <div
            v-for="bloque in availability.bloques"
            :key="bloque.horaInicio"
            :class="[
              'rounded-lg py-2 px-1 text-center text-xs font-semibold border',
              bloque.disponible
                ? 'bg-secondary-fixed/40 text-on-secondary-fixed border-secondary/30'
                : 'bg-surface-container-high text-on-surface-variant border-outline-variant/30 line-through'
            ]"
          >
            {{ bloque.horaInicio }}
          </div>
        </div>

        <div class="flex items-center gap-4 text-xs text-on-surface-variant">
          <span class="flex items-center gap-1.5">
            <span class="w-3 h-3 rounded-full bg-secondary-fixed/40 border border-secondary/30" />
            Disponible
          </span>
          <span class="flex items-center gap-1.5">
            <span class="w-3 h-3 rounded-full bg-surface-container-high border border-outline-variant/30" />
            Ocupado
          </span>
        </div>
      </template>
    </div>
  </BaseModal>
</template>