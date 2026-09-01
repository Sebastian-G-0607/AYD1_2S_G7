<script setup lang="ts">
import { reactive, onMounted, ref } from 'vue'
import { RouterLink, useRoute } from 'vue-router'
import { BaseButton, BaseInput, BaseAlert } from '@/components/ui'
import { useAuth } from '../composables/useAuth'
import type { LoginCredentials } from '../types'

const route = useRoute()
const showSuccessNotification = ref(false)

const credentials = reactive<LoginCredentials>({
  correo: '',
  password: ''
})

const { isLoading, errorMessage, clearError, login } = useAuth()

onMounted(() => {
  if (route.query.registered === 'success') {
    showSuccessNotification.value = true
  }
})

const handleSubmit = async () => {
  if (!credentials.correo || !credentials.password) return
  await login({ ...credentials })
}
</script>

<template>
  <div class="w-full flex flex-col gap-10 relative z-10">
    <div class="flex flex-col gap-2">
      <h1 class="text-3xl font-bold text-primary font-headline tracking-tight">Iniciar Sesión</h1>
      <p class="text-base text-on-surface-variant font-body">Bienvenido de nuevo a EduConnect</p>
    </div>

    <form class="flex flex-col gap-6" @submit.prevent="handleSubmit">
      <BaseAlert
        v-if="showSuccessNotification"
        type="success"
        title="Registro exitoso"
        message="Tu cuenta ha sido creada y se encuentra en estado de aprobación. Podrás iniciar sesión una vez habilitada."
        @dismiss="showSuccessNotification = false"
      />

      <BaseAlert
        v-if="errorMessage"
        type="error"
        title="Acceso denegado"
        :message="errorMessage"
        @dismiss="clearError"
      />

      <BaseInput
        id="correo"
        v-model="credentials.correo"
        name="correo"
        type="email"
        label="Correo Electrónico"
        placeholder="tu@correo.edu"
        icon="mail"
        autocomplete="email"
        required
      />

      <BaseInput
        id="password"
        v-model="credentials.password"
        name="password"
        type="password"
        label="Contraseña"
        placeholder="••••••••"
        icon="lock"
        autocomplete="current-password"
        show-password-toggle
        required
      >
        <template #labelAction>
          <RouterLink
            to="/forgot-password"
            class="text-xs font-medium text-secondary hover:text-primary transition-colors"
          >
            ¿Olvidaste tu contraseña?
          </RouterLink>
        </template>
      </BaseInput>

      <BaseButton type="submit" variant="primary" size="md" block :loading="isLoading" class="mt-2">
        Ingresar a la Plataforma
      </BaseButton>
    </form>

    <div class="pt-6 flex flex-col items-start gap-4 border-t border-surface-container">
      <p class="text-sm text-on-surface-variant font-body">¿No tienes cuenta?</p>
      <div class="flex flex-col sm:flex-row gap-3 w-full">
        <RouterLink
          to="/register/student"
          class="flex-1 py-2.5 px-4 rounded-lg bg-surface-container-low hover:bg-surface-container-high text-primary font-semibold text-sm transition-colors text-center"
        >
          Registro Estudiante
        </RouterLink>
        <RouterLink
          to="/register/tutor"
          class="flex-1 py-2.5 px-4 rounded-lg bg-surface-container-low hover:bg-surface-container-high text-secondary font-semibold text-sm transition-colors text-center"
        >
          Registro Tutor
        </RouterLink>
      </div>
    </div>
  </div>
</template>
