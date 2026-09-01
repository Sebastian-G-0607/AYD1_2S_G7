<script setup lang="ts">
import { reactive, watch } from 'vue'
import { BaseButton, BaseModal } from '@/components/ui'
import type { TutorSession } from '../types'

interface Props {
  modelValue: boolean
  session: TutorSession | null
  loading?: boolean
}

interface Emits {
  (e: 'update:modelValue', value: boolean): void
  (
    e: 'submit',
    payload: {
      resumen: string
      recomendaciones: string
      enviarCopiaCorreo: boolean
    }
  ): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const form = reactive({
  resumen: '',
  recomendaciones: '',
  enviarCopiaCorreo: true
})

watch(
  () => props.modelValue,
  isOpen => {
    if (isOpen) {
      form.resumen = ''
      form.recomendaciones = ''
      form.enviarCopiaCorreo = true
    }
  }
)

function handleSubmit() {
  if (!form.resumen.trim()) return
  emit('submit', { ...form })
}
</script>

<template>
  <BaseModal
    :model-value="modelValue"
    title="Marcar como Atendida"
    max-width="lg"
    @update:model-value="emit('update:modelValue', $event)"
  >
    <div class="flex flex-col gap-5">
      <div class="flex items-center gap-3">
        <div
          class="w-12 h-12 rounded-full bg-[#16a34a]/10 text-[#16a34a] flex items-center justify-center"
        >
          <span class="material-symbols-outlined text-[26px]">task_alt</span>
        </div>
        <div>
          <h4 class="font-semibold text-on-surface text-base">
            Sesión con {{ session?.estudianteNombre }}
          </h4>
          <p class="text-xs text-on-surface-variant">
            {{ session?.materia }} • {{ session?.fecha }} ({{ session?.hora }})
          </p>
        </div>
      </div>

      <p class="text-sm text-on-surface-variant font-body">
        Proporciona un breve resumen de la sesión y recomendaciones para el estudiante.
      </p>

      <div class="flex flex-col gap-4">
        <div class="flex flex-col gap-1.5">
          <label for="complete-resumen" class="text-xs font-semibold text-on-surface"
            >Resumen de la sesión *</label
          >
          <textarea
            id="complete-resumen"
            v-model="form.resumen"
            rows="3"
            placeholder="Temas cubiertos, dudas resueltas..."
            required
            class="w-full bg-surface-container-low text-on-surface px-4 py-3 rounded-lg border border-outline-variant/40 focus:outline-none focus:ring-2 focus:ring-secondary/40 focus:border-secondary text-sm resize-none"
          />
        </div>

        <div class="flex flex-col gap-1.5">
          <label for="complete-recom" class="text-xs font-semibold text-on-surface"
            >Recomendaciones para el estudiante</label
          >
          <textarea
            id="complete-recom"
            v-model="form.recomendaciones"
            rows="3"
            placeholder="Ejercicios sugeridos, lecturas adicionales..."
            class="w-full bg-surface-container-low text-on-surface px-4 py-3 rounded-lg border border-outline-variant/40 focus:outline-none focus:ring-2 focus:ring-secondary/40 focus:border-secondary text-sm resize-none"
          />
        </div>

        <div class="flex items-center gap-2 pt-1">
          <input
            id="sendEmailCopy"
            v-model="form.enviarCopiaCorreo"
            type="checkbox"
            class="w-4 h-4 rounded border-outline-variant text-primary focus:ring-primary/40 bg-surface-container cursor-pointer"
          />
          <label
            for="sendEmailCopy"
            class="text-xs text-on-surface-variant font-medium cursor-pointer"
          >
            Enviar copia del resumen al estudiante
          </label>
        </div>
      </div>
    </div>

    <template #footer>
      <BaseButton
        variant="outline"
        size="md"
        :disabled="loading"
        @click="emit('update:modelValue', false)"
      >
        Cancelar
      </BaseButton>
      <BaseButton
        variant="primary"
        size="md"
        :loading="loading"
        :disabled="!form.resumen.trim()"
        @click="handleSubmit"
      >
        <template #iconLeft>
          <span class="material-symbols-outlined text-[18px]">save</span>
        </template>
        Guardar y Finalizar
      </BaseButton>
    </template>
  </BaseModal>
</template>
