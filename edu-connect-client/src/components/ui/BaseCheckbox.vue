<script setup lang="ts">
interface Props {
  modelValue?: boolean
  id?: string
  name?: string
  label?: string
  disabled?: boolean
  required?: boolean
  error?: string
}

withDefaults(defineProps<Props>(), {
  modelValue: false,
  id: undefined,
  name: undefined,
  label: undefined,
  disabled: false,
  required: false,
  error: undefined
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
}>()

const onChange = (event: Event) => {
  const target = event.target as HTMLInputElement
  emit('update:modelValue', target.checked)
}
</script>

<template>
  <div class="flex flex-col gap-1">
    <label
      :for="id"
      :class="[
        'inline-flex items-center gap-2.5 select-none cursor-pointer group',
        disabled ? 'cursor-not-allowed opacity-60' : ''
      ]"
    >
      <div class="relative flex items-center justify-center">
        <input
          :id="id"
          :name="name"
          type="checkbox"
          :checked="modelValue"
          :disabled="disabled"
          :required="required"
          class="peer sr-only"
          @change="onChange"
        />
        <div
          class="w-4 h-4 rounded border border-outline-variant bg-surface-container-low transition-all duration-150 peer-checked:bg-secondary peer-checked:border-secondary peer-focus-visible:ring-2 peer-focus-visible:ring-secondary/40 peer-focus-visible:ring-offset-1 group-hover:border-outline peer-disabled:cursor-not-allowed"
        />
        <span
          class="material-symbols-outlined absolute text-[14px] text-white opacity-0 transition-opacity peer-checked:opacity-100 pointer-events-none font-bold"
        >
          check
        </span>
      </div>

      <span
        v-if="label || $slots.default"
        class="text-sm font-body text-on-surface-variant group-hover:text-on-surface transition-colors"
      >
        <slot>{{ label }}</slot>
      </span>
    </label>

    <p v-if="error" class="text-xs text-error font-medium pl-6">
      {{ error }}
    </p>
  </div>
</template>
