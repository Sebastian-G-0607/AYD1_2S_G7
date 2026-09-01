<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  type?: 'button' | 'submit' | 'reset'
  variant?: 'primary' | 'secondary' | 'surface' | 'outline' | 'ghost'
  size?: 'sm' | 'md' | 'lg'
  loading?: boolean
  disabled?: boolean
  block?: boolean
  icon?: string
}

const props = withDefaults(defineProps<Props>(), {
  type: 'button',
  variant: 'primary',
  size: 'md',
  loading: false,
  disabled: false,
  block: false,
  icon: undefined
})

defineEmits<{
  (e: 'click', event: MouseEvent): void
}>()

const variantClasses = computed(() => {
  switch (props.variant) {
    case 'primary':
      return 'bg-primary text-on-primary shadow-md hover:shadow-lg hover:-translate-y-0.5 active:translate-y-0 relative overflow-hidden group'
    case 'secondary':
      return 'bg-secondary text-on-secondary shadow-md hover:shadow-lg hover:-translate-y-0.5 active:translate-y-0 relative overflow-hidden group'
    case 'surface':
      return 'bg-surface-container-low hover:bg-surface-container-high text-primary'
    case 'outline':
      return 'border border-outline hover:bg-surface-container-low text-primary'
    case 'ghost':
      return 'text-on-surface-variant hover:text-primary hover:bg-surface-container-low'
    default:
      return 'bg-primary text-on-primary'
  }
})

const sizeClasses = computed(() => {
  switch (props.size) {
    case 'sm':
      return 'py-2 px-3 text-xs font-semibold rounded-md'
    case 'lg':
      return 'py-4 px-6 text-base font-semibold rounded-xl'
    case 'md':
    default:
      return 'py-3.5 px-4 text-sm font-semibold rounded-lg'
  }
})
</script>

<template>
  <button
    :type="type"
    :disabled="disabled || loading"
    :class="[
      'inline-flex items-center justify-center font-sans transition-all duration-200 focus:outline-none disabled:opacity-60 disabled:cursor-not-allowed disabled:transform-none disabled:shadow-none',
      variantClasses,
      sizeClasses,
      block ? 'w-full' : ''
    ]"
    @click="$emit('click', $event)"
  >
    <span
      v-if="variant === 'primary' || variant === 'secondary'"
      class="absolute inset-0 w-full h-full bg-white/10 opacity-0 group-hover:opacity-100 transition-opacity pointer-events-none"
    />

    <span v-if="loading" class="inline-flex items-center justify-center gap-2">
      <span class="material-symbols-outlined animate-spin text-[20px]">progress_activity</span>
      <span v-if="$slots.loading">
        <slot name="loading" />
      </span>
      <span v-else>
        <slot />
      </span>
    </span>

    <span v-else class="inline-flex items-center justify-center gap-2">
      <span v-if="icon" class="material-symbols-outlined text-[20px]">
        {{ icon }}
      </span>
      <slot />
    </span>
  </button>
</template>
