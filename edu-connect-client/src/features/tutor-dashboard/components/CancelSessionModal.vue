<script setup lang="ts">
import { reactive, ref, watch } from 'vue'
import { BaseAlert, BaseButton, BaseModal } from '@/components/ui'
import type { TutorSession } from '../types'

interface Props {
  modelValue: boolean
  session: TutorSession | null
  loading?: boolean
  errorMessage?: string | null
}

interface Emits {
  (e: 'update:modelValue', value: boolean): void
  (
    e: 'submit',
    payload: {
      motivo: string
      mensajeDisculpa?: string
    }
  ): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const form = reactive({
  motivo: '',
  mensajeDisculpa: ''
})

const clientValidationError = ref<string | null>(null)

watch(
  () => props.modelValue,
  isOpen => {
    if (isOpen) {
      form.motivo = ''
      form.mensajeDisculpa = ''
      clientValidationError.value = null
    }
  }
)

function handleClose() {
  clientValidationError.value = null
  emit('update:modelValue', false)
}

function handleSubmit() {
  const motivoTrimmed = form.motivo.trim()
  if (!motivoTrimmed) {
    clientValidationError.value = 'Debe ingresar el motivo de cancelación de la sesión.'
    return
  }

  clientValidationError.value = null
  emit('submit', {
    motivo: motivoTrimmed,
    mensajeDisculpa: form.mensajeDisculpa.trim() ? form.mensajeDisculpa.trim() : undefined
  })
}
</script>

<template>
  <BaseModal
    :model-value="modelValue"
    title="Cancelar Sesión"
    max-width="lg"
    @update:model-value="handleClose"
  >
    <div class="flex flex-col gap-5">
      <div class="flex items-center gap-3">
        <div
          class="w-12 h-12 rounded-full bg-error-container text-on-error-container flex items-center justify-center shrink-0"
        >
          <span class="material-symbols-outlined text-[26px]">cancel</span>
        </div>
        <div>
          <h4 class="font-semibold text-on-surface text-base">
            Cancelar sesión con {{ session?.estudianteNombre }}
          </h4>
          <p class="text-xs text-on-surface-variant">
            {{ session?.materia }} • {{ session?.fecha }} ({{ session?.hora }})
          </p>
        </div>
      </div>

      <p class="text-sm text-on-surface-variant font-body">
        Esta acción cancelará la sesión, liberará el horario en tu agenda y notificará
        inmediatamente al estudiante por correo electrónico.
      </p>

      <BaseAlert
        v-if="clientValidationError || errorMessage"
        type="error"
        :message="clientValidationError || errorMessage || undefined"
        dismissible
        @dismiss="clientValidationError = null"
      />

      <div class="flex flex-col gap-4">
        <div class="flex flex-col gap-1.5">
          <label for="cancel-motivo" class="text-xs font-semibold text-on-surface">
            Motivo de cancelación <span class="text-error">*</span>
          </label>
          <textarea
            id="cancel-motivo"
            v-model="form.motivo"
            rows="3"
            required
            :disabled="loading"
            class="w-full bg-surface-container-low text-on-surface px-4 py-3 rounded-lg border border-outline-variant/40 focus:outline-none focus:ring-2 focus:ring-error/40 focus:border-error text-sm resize-none disabled:opacity-60"
            @input="clientValidationError = null"
          />
        </div>

        <div class="flex flex-col gap-1.5">
          <label for="cancel-disculpa" class="text-xs font-semibold text-on-surface">
            Mensaje de disculpa personalizado
            <span class="text-xs text-on-surface-variant font-normal">(Opcional)</span>
          </label>
          <textarea
            id="cancel-disculpa"
            v-model="form.mensajeDisculpa"
            rows="3"
            :disabled="loading"
            class="w-full bg-surface-container-low text-on-surface px-4 py-3 rounded-lg border border-outline-variant/40 focus:outline-none focus:ring-2 focus:ring-primary/40 focus:border-primary text-sm resize-none disabled:opacity-60"
          />
        </div>
      </div>
    </div>

    <template #footer>
      <BaseButton variant="outline" size="md" :disabled="loading" @click="handleClose">
        Mantener Sesión
      </BaseButton>
      <BaseButton
        variant="danger"
        size="md"
        :loading="loading"
        :disabled="!form.motivo.trim() || loading"
        @click="handleSubmit"
      >
        <template #iconLeft>
          <span class="material-symbols-outlined text-[18px]">event_busy</span>
        </template>
        Confirmar Cancelación
      </BaseButton>
    </template>
  </BaseModal>
</template>
