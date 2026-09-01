<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  variant?: 'primary' | 'secondary' | 'neutral' | 'success' | 'error'
  size?: 'sm' | 'md'
  removable?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  variant: 'primary',
  size: 'md',
  removable: false
})

const emit = defineEmits<{
  (e: 'remove'): void
}>()

const variantClasses = computed(() => {
  switch (props.variant) {
    case 'secondary':
      return 'bg-secondary-fixed/50 text-secondary border-secondary/20'
    case 'neutral':
      return 'bg-surface-container-high text-on-surface border-outline-variant/30'
    case 'success':
      return 'bg-emerald-500/10 text-emerald-700 dark:text-emerald-300 border-emerald-500/20'
    case 'error':
      return 'bg-error-container text-on-error-container border-error/20'
    case 'primary':
    default:
      return 'bg-primary/10 text-primary border-primary/20'
  }
})

const sizeClasses = computed(() => {
  switch (props.size) {
    case 'sm':
      return 'text-xs py-0.5 px-2 gap-1'
    case 'md':
    default:
      return 'text-sm py-1 px-3 gap-1.5'
  }
})
</script>

<template>
  <span
    :class="[
      'inline-flex items-center rounded-full font-medium border select-none transition-all',
      variantClasses,
      sizeClasses
    ]"
  >
    <slot />
    <button
      v-if="removable"
      type="button"
      class="inline-flex items-center justify-center rounded-full hover:bg-black/10 dark:hover:bg-white/10 p-0.5 transition-colors focus:outline-none"
      @click.stop="emit('remove')"
    >
      <span class="material-symbols-outlined text-[14px]">close</span>
    </button>
  </span>
</template>
