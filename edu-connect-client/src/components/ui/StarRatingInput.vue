<script setup lang="ts">
interface Props {
  modelValue: number
  disabled?: boolean
}

withDefaults(defineProps<Props>(), {
  disabled: false
})

const emit = defineEmits<{
  'update:modelValue': [value: number]
}>()

function selectStar(value: number) {
  emit('update:modelValue', value)
}
</script>

<template>
  <div class="flex items-center gap-1">
    <button
      v-for="star in 5"
      :key="star"
      type="button"
      :disabled="disabled"
      class="disabled:cursor-not-allowed"
      @click="selectStar(star)"
    >
      <span
        class="material-symbols-outlined text-[28px] transition-colors"
        :class="star <= modelValue ? 'text-amber-400' : 'text-outline-variant'"
        :style="star <= modelValue ? { fontVariationSettings: '\'FILL\' 1' } : {}"
      >
        star
      </span>
    </button>
  </div>
</template>