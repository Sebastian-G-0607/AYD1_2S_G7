<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  password?: string
}

const props = withDefaults(defineProps<Props>(), {
  password: ''
})

const lengthValid = computed(() => props.password.length >= 8)
const hasUppercase = computed(() => /[A-Z]/.test(props.password))
const hasLowercase = computed(() => /[a-z]/.test(props.password))
const hasNumber = computed(() => /[0-9]/.test(props.password))
</script>

<template>
  <div class="p-4 rounded-lg bg-surface-container-low shadow-sm w-full">
    <p class="text-sm font-semibold text-on-surface mb-2 font-headline">
      Requisitos de la contraseña:
    </p>
    <ul class="space-y-2 text-xs font-body">
      <li
        :class="[
          'flex items-center gap-2 transition-colors',
          lengthValid ? 'text-primary font-medium' : 'text-on-surface-variant'
        ]"
      >
        <span class="material-symbols-outlined text-[16px]">
          {{ lengthValid ? 'check_circle' : 'radio_button_unchecked' }}
        </span>
        Al menos 8 caracteres
      </li>
      <li
        :class="[
          'flex items-center gap-2 transition-colors',
          hasUppercase ? 'text-primary font-medium' : 'text-on-surface-variant'
        ]"
      >
        <span class="material-symbols-outlined text-[16px]">
          {{ hasUppercase ? 'check_circle' : 'radio_button_unchecked' }}
        </span>
        Al menos 1 letra mayúscula
      </li>
      <li
        :class="[
          'flex items-center gap-2 transition-colors',
          hasLowercase ? 'text-primary font-medium' : 'text-on-surface-variant'
        ]"
      >
        <span class="material-symbols-outlined text-[16px]">
          {{ hasLowercase ? 'check_circle' : 'radio_button_unchecked' }}
        </span>
        Al menos 1 letra minúscula
      </li>
      <li
        :class="[
          'flex items-center gap-2 transition-colors',
          hasNumber ? 'text-primary font-medium' : 'text-on-surface-variant'
        ]"
      >
        <span class="material-symbols-outlined text-[16px]">
          {{ hasNumber ? 'check_circle' : 'radio_button_unchecked' }}
        </span>
        Al menos 1 número
      </li>
    </ul>
  </div>
</template>
