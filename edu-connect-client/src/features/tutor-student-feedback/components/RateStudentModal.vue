<script setup lang="ts">
import { ref } from 'vue'
import { BaseAlert, BaseButton, BaseModal, StarRatingInput } from '@/components/ui'
import { tutorStudentFeedbackService } from '../services/tutorStudentFeedback.service'

interface Props {
  modelValue: boolean
  sesionId: number
  estudianteNombre: string
}

const props = defineProps<Props>()

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
  rated: []
}>()

const estrellas = ref(0)
const comentario = ref('')
const isSubmitting = ref(false)
const errorMessage = ref('')

function close() {
  emit('update:modelValue', false)
  estrellas.value = 0
  comentario.value = ''
  errorMessage.value = ''
}

async function handleSubmit() {
  if (estrellas.value < 0 || estrellas.value > 5) return

  isSubmitting.value = true
  errorMessage.value = ''

  try {
    await tutorStudentFeedbackService.calificarEstudiante(props.sesionId, {
      estrellas: estrellas.value,
      comentario: comentario.value.trim() || undefined
    })

    emit('rated')
    close()
  } catch (error) {
    errorMessage.value = error instanceof Error ? error.message : 'No fue posible registrar la calificación.'
  } finally {
    isSubmitting.value = false
  }
}
</script>

<template>
  <BaseModal :model-value="modelValue" title="Calificar estudiante" @update:model-value="close">
    <div class="flex flex-col gap-5">
      <p class="text-sm text-on-surface-variant">
        Califica la atención brindada a <span class="font-medium text-on-surface">{{ estudianteNombre }}</span>.
      </p>

      <BaseAlert v-if="errorMessage" type="error" :message="errorMessage" :dismissible="false" />

      <div>
        <label class="text-sm font-semibold text-on-surface">
          Calificación <span class="text-error">*</span>
        </label>
        <div class="mt-2">
          <StarRatingInput v-model="estrellas" :disabled="isSubmitting" />
        </div>
      </div>

      <div class="flex flex-col gap-2">
        <label class="text-sm font-semibold text-on-surface" for="comentario-calificacion">
          Comentario (opcional)
        </label>
        <textarea
          id="comentario-calificacion"
          v-model="comentario"
          rows="3"
          :disabled="isSubmitting"
          placeholder="Describe brevemente el motivo de tu calificación"
          class="w-full rounded-lg border border-outline-variant px-3 py-2 text-sm text-on-surface placeholder:text-on-surface-variant/60 focus:outline-none focus:ring-2 focus:ring-primary disabled:opacity-60"
        />
      </div>
    </div>

    <template #footer>
      <BaseButton variant="outline" :disabled="isSubmitting" @click="close">Cancelar</BaseButton>
      <BaseButton :loading="isSubmitting" :disabled="estrellas === 0" @click="handleSubmit">
        Guardar calificación
      </BaseButton>
    </template>
  </BaseModal>
</template>