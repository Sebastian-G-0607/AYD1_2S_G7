<script setup lang="ts">
import { ref, computed } from 'vue'

interface Props {
  modelValue?: string | number
  id?: string
  name?: string
  label?: string
  type?: string
  placeholder?: string
  icon?: string
  trailingIcon?: string
  required?: boolean
  disabled?: boolean
  error?: string
  hint?: string
  autocomplete?: string
  showPasswordToggle?: boolean
  min?: string | number
  max?: string | number
  step?: string | number
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: '',
  id: undefined,
  name: undefined,
  label: undefined,
  type: 'text',
  placeholder: '',
  icon: undefined,
  trailingIcon: undefined,
  required: false,
  disabled: false,
  error: undefined,
  hint: undefined,
  autocomplete: undefined,
  showPasswordToggle: false,
  min: undefined,
  max: undefined,
  step: undefined
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
  (e: 'focus', event: FocusEvent): void
  (e: 'blur', event: FocusEvent): void
}>()

const isPasswordVisible = ref(false)

const computedType = computed(() => {
  if (props.showPasswordToggle) {
    return isPasswordVisible.value ? 'text' : 'password'
  }
  return props.type
})

const togglePasswordVisibility = () => {
  isPasswordVisible.value = !isPasswordVisible.value
}

const onInput = (event: Event) => {
  const target = event.target as HTMLInputElement
  emit('update:modelValue', target.value)
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

      <input
        :id="id"
        :name="name"
        :type="computedType"
        :value="modelValue"
        :placeholder="placeholder"
        :required="required"
        :disabled="disabled"
        :autocomplete="autocomplete"
        :min="min"
        :max="max"
        :step="step"
        :class="[
          'w-full bg-surface-container-low py-3 rounded-lg text-base text-on-surface placeholder:text-outline-variant outline-none focus:ring-2 focus:ring-primary focus:bg-surface-container-lowest border border-outline-variant focus:border-transparent transition-all disabled:opacity-60 disabled:cursor-not-allowed',
          icon ? 'pl-10' : 'pl-4',
          showPasswordToggle || trailingIcon ? 'pr-10' : 'pr-4',
          error ? 'border-error bg-error-container/20 focus:ring-error' : ''
        ]"
        @input="onInput"
        @focus="emit('focus', $event)"
        @blur="emit('blur', $event)"
      />

      <button
        v-if="showPasswordToggle"
        type="button"
        class="absolute inset-y-0 right-0 pr-3 flex items-center text-on-surface-variant hover:text-primary transition-colors focus:outline-none"
        @click="togglePasswordVisibility"
      >
        <span class="material-symbols-outlined text-[20px]">
          {{ isPasswordVisible ? 'visibility_off' : 'visibility' }}
        </span>
      </button>

      <div
        v-else-if="trailingIcon"
        class="absolute inset-y-0 right-0 pr-3 flex items-center pointer-events-none"
      >
        <span class="material-symbols-outlined text-on-surface-variant text-[20px]">
          {{ trailingIcon }}
        </span>
      </div>
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
