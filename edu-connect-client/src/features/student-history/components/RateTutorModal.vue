<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { BaseAlert, BaseButton, BaseModal } from '@/components/ui'
import type { RateTutorPayload, StudentHistorySession } from '../types'

interface Props {
  modelValue: boolean
  session: StudentHistorySession | null
  loading?: boolean
  errorMessage?: string | null
}

interface Emits {
  (e: 'update:modelValue', value: boolean): void
  (e: 'submit', payload: RateTutorPayload): void
}

const props = withDefaults(defineProps<Props>(), {
  loading: false,
  errorMessage: null
})
const emit = defineEmits<Emits>()

const selectedStars = ref<number | null>(null)
const comment = ref('')
const canSubmit = computed(() => selectedStars.value !== null)

watch(
  () => props.modelValue,
  isOpen => {
    if (isOpen) {
      selectedStars.value = null
      comment.value = ''
    }
  }
)

function handleSubmit() {
  if (selectedStars.value === null) return

  emit('submit', {
    estrellas: selectedStars.value,
    comentario: comment.value.trim()
  })
}

function handleClose() {
  emit('update:modelValue', false)
}
</script>

<template>
  <BaseModal
    :model-value="modelValue"
    title="Calificar atención de tutoría"
    max-width="lg"
    @update:model-value="handleClose"
  >
    <div class="flex flex-col gap-5">
      <div class="flex items-center gap-3">
        <div
          class="w-12 h-12 rounded-full bg-secondary-container text-on-secondary-container flex items-center justify-center shrink-0"
        >
          <span class="material-symbols-outlined text-[26px]">star_rate</span>
        </div>
        <div>
          <h4 class="font-semibold text-on-surface text-base">
            Atención de {{ session?.tutor }}
          </h4>
          <p class="text-xs text-on-surface-variant">
            {{ session?.materia }} · {{ session?.fechaSesion }}
          </p>
        </div>
      </div>

      <BaseAlert v-if="errorMessage" type="error" :message="errorMessage" />

      <fieldset class="flex flex-col gap-2" :disabled="loading">
        <legend class="text-sm font-semibold text-on-surface">
          Calificación <span class="text-error">*</span>
        </legend>
        <div role="radiogroup" aria-label="Calificación de 0 a 5 estrellas" class="flex gap-2">
          <button
            v-for="stars in [0, 1, 2, 3, 4, 5]"
            :key="stars"
            type="button"
            role="radio"
            :aria-checked="selectedStars === stars"
            :aria-label="`${stars} ${stars === 1 ? 'estrella' : 'estrellas'}`"
            :title="`${stars} ${stars === 1 ? 'estrella' : 'estrellas'}`"
            :class="[
              'h-11 min-w-11 px-2 rounded-md border transition-colors focus-visible:outline focus-visible:outline-2 focus-visible:outline-primary',
              selectedStars === stars
                ? 'border-primary bg-primary/10 text-primary'
                : 'border-outline-variant/50 text-on-surface-variant hover:bg-surface-container-low'
            ]"
            @click="selectedStars = stars"
          >
            <span v-if="stars === 0" class="text-sm font-semibold">0</span>
            <span v-else class="material-symbols-outlined text-[21px]">star</span>
          </button>
        </div>
        <p class="text-xs text-on-surface-variant">Selecciona de 0 a 5 estrellas.</p>
      </fieldset>

      <div class="flex flex-col gap-1.5">
        <label for="rating-comment" class="text-sm font-semibold text-on-surface">
          Comentario <span class="text-on-surface-variant font-normal">(opcional)</span>
        </label>
        <textarea
          id="rating-comment"
          v-model="comment"
          rows="4"
          maxlength="1000"
          :disabled="loading"
          placeholder="Cuéntanos brevemente el motivo de tu calificación"
          class="w-full bg-surface-container-low text-on-surface px-4 py-3 rounded-lg border border-outline-variant/40 focus:outline-none focus:ring-2 focus:ring-primary/30 focus:border-primary text-sm resize-y disabled:opacity-60"
        />
        <span class="text-xs text-on-surface-variant self-end">{{ comment.length }}/1000</span>
      </div>
    </div>

    <template #footer>
      <BaseButton variant="outline" size="md" :disabled="loading" @click="handleClose">
        Cancelar
      </BaseButton>
      <BaseButton
        variant="primary"
        size="md"
        :loading="loading"
        :disabled="!canSubmit"
        @click="handleSubmit"
      >
        Guardar calificación
      </BaseButton>
    </template>
  </BaseModal>
</template>