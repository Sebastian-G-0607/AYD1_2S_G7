<script setup lang="ts">
import { ref, watch, onBeforeUnmount } from 'vue'

interface Props {
  modelValue?: File | string | null
  alt?: string
  disabled?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: null,
  alt: 'Avatar',
  disabled: false
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: File | string | null): void
  (e: 'change', file: File): void
}>()

const fileInputRef = ref<HTMLInputElement | null>(null)
const previewUrl = ref<string | null>(null)

const defaultAvatar =
  'data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="%238590a6"><path d="M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z"/></svg>'

const updatePreview = (val: File | string | null) => {
  if (previewUrl.value && previewUrl.value.startsWith('blob:')) {
    URL.revokeObjectURL(previewUrl.value)
    previewUrl.value = null
  }
  if (!val) {
    previewUrl.value = null
  } else if (typeof val === 'string') {
    previewUrl.value = val
  } else if (val instanceof File) {
    previewUrl.value = URL.createObjectURL(val)
  }
}

watch(
  () => props.modelValue,
  newVal => {
    updatePreview(newVal)
  },
  { immediate: true }
)

onBeforeUnmount(() => {
  if (previewUrl.value && previewUrl.value.startsWith('blob:')) {
    URL.revokeObjectURL(previewUrl.value)
  }
})

const triggerFileSelect = () => {
  if (props.disabled) return
  fileInputRef.value?.click()
}

const onFileChange = (event: Event) => {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]
  if (file) {
    emit('update:modelValue', file)
    emit('change', file)
  }
}
</script>

<template>
  <div
    class="relative w-24 h-24 mx-auto sm:mx-0 group cursor-pointer flex-shrink-0"
    @click="triggerFileSelect"
  >
    <div
      class="w-24 h-24 rounded-full overflow-hidden bg-surface-container-highest shadow-sm relative flex items-center justify-center border border-outline-variant/30"
    >
      <img :src="previewUrl || defaultAvatar" :alt="alt" class="w-full h-full object-cover" />
      <div
        class="absolute inset-0 bg-primary/60 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity duration-300"
      >
        <span class="material-symbols-outlined text-on-primary">photo_camera</span>
      </div>
    </div>
    <div
      class="absolute -bottom-1 -right-1 bg-primary text-on-primary rounded-full w-8 h-8 flex items-center justify-center shadow-md transition-transform group-hover:scale-105"
    >
      <span class="material-symbols-outlined text-[16px]">add</span>
    </div>
    <input
      ref="fileInputRef"
      type="file"
      accept="image/*"
      class="hidden"
      :disabled="disabled"
      @change="onFileChange"
    />
  </div>
</template>
