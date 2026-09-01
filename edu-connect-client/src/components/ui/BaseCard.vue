<script setup lang="ts">
interface Props {
  hoverable?: boolean
  padding?: 'none' | 'sm' | 'md' | 'lg'
}

withDefaults(defineProps<Props>(), {
  hoverable: false,
  padding: 'md'
})

const paddingClasses = {
  none: '',
  sm: 'p-4',
  md: 'p-6',
  lg: 'p-8'
}
</script>

<template>
  <div
    :class="[
      'bg-surface-container-lowest rounded-xl border border-outline-variant/20 shadow-sm transition-all duration-300',
      paddingClasses[padding],
      hoverable ? 'hover:shadow-lg hover:-translate-y-0.5 cursor-pointer' : ''
    ]"
  >
    <div v-if="$slots.header" class="mb-4 pb-3 border-b border-surface-container">
      <slot name="header" />
    </div>
    <slot />
    <div v-if="$slots.footer" class="mt-4 pt-3 border-t border-surface-container">
      <slot name="footer" />
    </div>
  </div>
</template>
