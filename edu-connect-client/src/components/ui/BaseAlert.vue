<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  type?: 'error' | 'warning' | 'info' | 'success'
  title?: string
  message?: string
  dismissible?: boolean
  icon?: string
}

const props = withDefaults(defineProps<Props>(), {
  type: 'error',
  title: undefined,
  message: undefined,
  dismissible: true,
  icon: undefined
})

const emit = defineEmits<{
  (e: 'dismiss'): void
}>()

const computedIcon = computed(() => {
  if (props.icon) {
    return props.icon
  }
  switch (props.type) {
    case 'error':
      return 'error'
    case 'warning':
      return 'warning'
    case 'success':
      return 'check_circle'
    case 'info':
    default:
      return 'info'
  }
})

const typeClasses = computed(() => {
  switch (props.type) {
    case 'error':
      return 'bg-error-container text-on-error-container'
    case 'warning':
      return 'bg-secondary-fixed text-on-secondary-fixed'
    case 'success':
      return 'bg-surface-container-high text-primary'
    case 'info':
    default:
      return 'bg-surface-container-high text-on-surface'
  }
})
</script>

<template>
  <div
    role="alert"
    :class="[
      'px-4 py-3 rounded-lg flex items-start gap-3 text-sm shadow-sm transition-all',
      typeClasses
    ]"
  >
    <span class="material-symbols-outlined shrink-0 text-[20px]">
      {{ computedIcon }}
    </span>

    <div class="flex-1">
      <p v-if="title" class="font-semibold text-sm">
        {{ title }}
      </p>
      <p v-if="message" class="mt-0.5 opacity-90 text-sm">
        {{ message }}
      </p>
      <slot />
    </div>

    <button
      v-if="dismissible"
      type="button"
      class="shrink-0 opacity-70 hover:opacity-100 transition-opacity focus:outline-none"
      @click="emit('dismiss')"
    >
      <span class="material-symbols-outlined text-[16px]">close</span>
    </button>
  </div>
</template>
