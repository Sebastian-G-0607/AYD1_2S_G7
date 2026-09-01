<script setup lang="ts">
import { computed } from 'vue'

export interface SelectOption {
  value: string | number
  label: string
  disabled?: boolean
}

interface Props {
  modelValue?: string | number | (string | number)[]
  options: SelectOption[]
  id?: string
  name?: string
  label?: string
  placeholder?: string
  required?: boolean
  disabled?: boolean
  multiple?: boolean
  size?: number
  error?: string
  hint?: string
  icon?: string
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: '',
  options: () => [],
  id: undefined,
  name: undefined,
  label: undefined,
  placeholder: 'Selecciona una opción',
  required: false,
  disabled: false,
  multiple: false,
  size: undefined,
  error: undefined,
  hint: undefined,
  icon: undefined
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: string | number | (string | number)[]): void
}>()

const isMultiple = computed(() => props.multiple)

const onChange = (event: Event) => {
  const target = event.target as HTMLSelectElement
  if (isMultiple.value) {
    const selectedValues = Array.from(target.selectedOptions).map(option => option.value)
    emit('update:modelValue', selectedValues)
  } else {
    emit('update:modelValue', target.value)
  }
}

const isSelected = (value: string | number) => {
  if (Array.isArray(props.modelValue)) {
    return props.modelValue.includes(value)
  }
  return props.modelValue === value
}
</script>

<template>
  <div class="flex flex-col gap-2 relative group w-full">
    <div v-if="label || $slots.labelAction" class="flex justify-between items-center">
      <label
        v-if="label"
        :for="id"
        class="text-sm font-semibold text-on-surface transition-colors group-focus-within:text-primary"
      >
        {{ label }}
        <span v-if="required" class="text-error">*</span>
      </label>
      <slot name="labelAction" />
    </div>

    <div class="relative">
      <div v-if="icon" class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
        <span
          class="material-symbols-outlined text-on-surface-variant text-[20px] group-focus-within:text-primary transition-colors"
        >
          {{ icon }}
        </span>
      </div>

      <select
        :id="id"
        :name="name"
        :required="required"
        :disabled="disabled"
        :multiple="multiple"
        :size="size"
        :value="multiple ? undefined : modelValue"
        :class="[
          'w-full rounded-lg bg-surface-container-low border border-outline-variant focus:outline-none focus:ring-2 focus:ring-primary focus:bg-surface-container-lowest focus:border-transparent text-base text-on-surface font-body transition-all disabled:opacity-60 disabled:cursor-not-allowed',
          multiple ? 'py-2 px-4 min-h-[120px]' : 'h-12 px-4 appearance-none',
          icon ? 'pl-10' : 'pl-4',
          !multiple ? 'pr-10' : 'pr-4',
          error ? 'border-error bg-error-container/20 focus:ring-error' : ''
        ]"
        @change="onChange"
      >
        <option v-if="!multiple && placeholder" value="" disabled :selected="modelValue === ''">
          {{ placeholder }}
        </option>
        <option
          v-for="option in options"
          :key="option.value"
          :value="option.value"
          :disabled="option.disabled"
          :selected="isSelected(option.value)"
        >
          {{ option.label }}
        </option>
      </select>

      <span
        v-if="!multiple"
        class="material-symbols-outlined absolute right-4 top-3 text-on-surface-variant pointer-events-none text-[20px]"
      >
        expand_more
      </span>
    </div>

    <p v-if="error" class="text-xs text-error font-medium flex items-center gap-1 mt-0.5">
      <span class="material-symbols-outlined text-[16px]">info</span>
      {{ error }}
    </p>
    <p v-else-if="hint" class="text-xs text-on-surface-variant mt-0.5">
      {{ hint }}
    </p>
  </div>
</template>
