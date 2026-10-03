<script setup lang="ts">
import { computed, ref } from 'vue'
import { formatFileSize } from '@/utils/fileValidation'

interface Props {
  modelValue?: File | null
  id: string
  name?: string
  label?: string
  accept?: string
  required?: boolean
  disabled?: boolean
  hint?: string
  /** Error externo (por ejemplo, un error devuelto por el servidor o un campo faltante al enviar). */
  error?: string
  icon?: string
  /** Retorna un mensaje si el archivo no es válido, o null si es válido. */
  validate?: (file: File) => string | null
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: null,
  name: undefined,
  label: undefined,
  accept: 'application/pdf',
  required: false,
  disabled: false,
  hint: undefined,
  error: undefined,
  icon: 'picture_as_pdf',
  validate: undefined
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: File | null): void
  (e: 'reject', message: string): void
}>()

const inputRef = ref<HTMLInputElement | null>(null)
const localError = ref<string | null>(null)
const isDragging = ref(false)

const displayError = computed(() => localError.value || props.error)
const errorId = computed(() => `${props.id}-error`)
const hintId = computed(() => `${props.id}-hint`)

const openFileDialog = () => {
  if (props.disabled) return
  inputRef.value?.click()
}

const handleFile = (file: File | undefined) => {
  if (!file) return

  const validationMessage = props.validate?.(file) ?? null
  if (validationMessage) {
    // Se conserva el archivo válido anterior (si existía) y se informa por qué no se aceptó el nuevo.
    localError.value = `No se aceptó "${file.name}". ${validationMessage}`
    emit('reject', validationMessage)
    return
  }

  localError.value = null
  emit('update:modelValue', file)
}

const onFileChange = (event: Event) => {
  const target = event.target as HTMLInputElement
  handleFile(target.files?.[0])
  // Permite volver a elegir exactamente el mismo archivo después de quitarlo o cambiarlo.
  target.value = ''
}

const onDrop = (event: DragEvent) => {
  isDragging.value = false
  if (props.disabled) return
  handleFile(event.dataTransfer?.files?.[0])
}

const clearFile = () => {
  localError.value = null
  emit('update:modelValue', null)
}

defineExpose({ openFileDialog })
</script>

<template>
  <div class="flex flex-col gap-2 w-full">
    <span v-if="label" :id="`${id}-label`" class="text-sm font-semibold text-on-surface">
      {{ label }}
      <span v-if="required" class="text-error">*</span>
    </span>

    <input
      :id="id"
      ref="inputRef"
      type="file"
      class="hidden"
      :name="name"
      :accept="accept"
      :disabled="disabled"
      :aria-invalid="displayError ? 'true' : undefined"
      :aria-describedby="displayError ? errorId : hint ? hintId : undefined"
      @change="onFileChange"
    />

    <div
      v-if="modelValue"
      class="flex items-center gap-3 rounded-lg border border-outline-variant bg-surface-container-lowest p-3"
      :class="displayError ? 'border-error' : ''"
    >
      <span
        class="material-symbols-outlined flex-shrink-0 rounded-lg bg-primary-fixed p-2 text-[24px] text-primary"
      >
        {{ icon }}
      </span>

      <div class="min-w-0 flex-1">
        <p class="truncate text-sm font-semibold text-on-surface" :title="modelValue.name">
          {{ modelValue.name }}
        </p>
        <p class="text-xs text-on-surface-variant">{{ formatFileSize(modelValue.size) }}</p>
      </div>

      <div class="flex flex-shrink-0 items-center gap-1">
        <button
          type="button"
          class="rounded-lg px-3 py-1.5 text-sm font-semibold text-secondary transition-colors hover:bg-surface-container-low focus:outline-none focus:ring-2 focus:ring-primary disabled:cursor-not-allowed disabled:opacity-60"
          :disabled="disabled"
          :aria-label="`Cambiar ${label ?? 'archivo'}`"
          @click="openFileDialog"
        >
          Cambiar
        </button>
        <button
          type="button"
          class="rounded-lg px-3 py-1.5 text-sm font-semibold text-error transition-colors hover:bg-error-container/40 focus:outline-none focus:ring-2 focus:ring-error disabled:cursor-not-allowed disabled:opacity-60"
          :disabled="disabled"
          :aria-label="`Quitar ${label ?? 'archivo'}`"
          @click="clearFile"
        >
          Quitar
        </button>
      </div>
    </div>

    <button
      v-else
      type="button"
      class="flex w-full flex-col items-center justify-center gap-2 rounded-lg border-2 border-dashed px-4 py-6 text-center transition-colors focus:outline-none focus:ring-2 focus:ring-primary disabled:cursor-not-allowed disabled:opacity-60"
      :class="[
        displayError
          ? 'border-error bg-error-container/20'
          : isDragging
            ? 'border-primary bg-surface-container-lowest'
            : 'border-outline-variant bg-surface-container-low hover:border-primary hover:bg-surface-container-lowest'
      ]"
      :disabled="disabled"
      :aria-labelledby="label ? `${id}-label` : undefined"
      @click="openFileDialog"
      @dragover.prevent="isDragging = true"
      @dragleave.prevent="isDragging = false"
      @drop.prevent="onDrop"
    >
      <span class="material-symbols-outlined text-[32px] text-on-surface-variant">
        upload_file
      </span>
      <span class="text-sm font-semibold text-on-surface">
        Haz clic para seleccionar o arrastra el archivo aquí
      </span>
    </button>

    <p
      v-if="displayError"
      :id="errorId"
      :data-testid="errorId"
      aria-live="polite"
      class="mt-0.5 flex items-start gap-1 text-xs font-medium text-error"
    >
      <span class="material-symbols-outlined text-[16px]">info</span>
      <span>{{ displayError }}</span>
    </p>
    <p v-else-if="hint" :id="hintId" class="mt-0.5 text-xs text-on-surface-variant">
      {{ hint }}
    </p>
  </div>
</template>
