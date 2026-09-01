<script setup lang="ts">
import { ref, watch } from 'vue'
import { BaseButton, BaseModal } from '@/components/ui'
import type { TutorSession } from '../types'

interface Props {
  modelValue: boolean
  session: TutorSession | null
  loading?: boolean
}

interface Emits {
  (e: 'update:modelValue', value: boolean): void
  (e: 'submit', payload: { motivo: string }): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const motivo = ref('')

watch(
  () => props.modelValue,
  isOpen => {
    if (isOpen) {
      motivo.value = ''
    }
  }
)

function handleSubmit() {
  emit('submit', { motivo: motivo.value })
}
</script>

<template>
  <BaseModal
    :model-value="modelValue"
    title="¿Cancelar Sesión?"
    max-width="md"
    @update:model-value="emit('update:modelValue', $event)"
  >
    <div class="flex flex-col gap-4">
      <div
        class="w-12 h-12 rounded-full bg-error-container text-on-error-container flex items-center justify-center"
      >
        <span class="material-symbols-outlined text-[26px]">warning</span>
      </div>

      <p class="text-sm text-on-surface-variant font-body">
        Esta acción notificará a
        <strong class="text-on-surface font-semibold">{{ session?.estudianteNombre }}</strong>
        y no se puede deshacer. ¿Estás seguro de que deseas cancelar esta tutoría?
      </p>

      <div class="flex flex-col gap-1.5 mt-2">
        <label for="cancel-motivo" class="text-xs font-semibold text-on-surface-variant"
          >Motivo de cancelación (Opcional)</label
        >
        <input
          id="cancel-motivo"
          v-model="motivo"
          type="text"
          placeholder="Ej. Motivos personales, cruce de horarios..."
          class="w-full bg-surface-container-low text-on-surface px-4 py-2.5 rounded-lg border border-outline-variant/40 focus:outline-none focus:ring-2 focus:ring-secondary/40 focus:border-secondary text-sm"
        />
      </div>
    </div>

    <template #footer>
      <BaseButton
        variant="outline"
        size="md"
        :disabled="loading"
        @click="emit('update:modelValue', false)"
      >
        Mantener Sesión
      </BaseButton>
      <BaseButton variant="danger" size="md" :loading="loading" @click="handleSubmit">
        Sí, Cancelar
      </BaseButton>
    </template>
  </BaseModal>
</template>
