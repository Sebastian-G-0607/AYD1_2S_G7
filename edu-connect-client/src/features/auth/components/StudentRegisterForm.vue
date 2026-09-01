<script setup lang="ts">
import { reactive, computed } from 'vue'
import { RouterLink } from 'vue-router'
import {
  BaseButton,
  BaseInput,
  BaseSelect,
  BaseAlert,
  BaseAvatarUpload,
  type SelectOption
} from '@/components/ui'
import PasswordRequirements from './PasswordRequirements.vue'
import { useAuth } from '../composables/useAuth'
import type { StudentRegisterData } from '../types'

const formData = reactive<StudentRegisterData>({
  nombre: '',
  apellido: '',
  carnet: '',
  genero: '',
  direccion: '',
  telefono: '',
  fechaNacimiento: '',
  correo: '',
  password: '',
  confirmPassword: '',
  fotografia: null
})

const genderOptions: SelectOption[] = [
  { value: 'masculino', label: 'Masculino' },
  { value: 'femenino', label: 'Femenino' }
]

const { isLoading, errorMessage, clearError, registerStudent } = useAuth()

const passwordMismatch = computed(() => {
  if (!formData.confirmPassword || !formData.password) return false
  return formData.password !== formData.confirmPassword
})

const handleSubmit = async () => {
  if (passwordMismatch.value) return
  await registerStudent({ ...formData })
}
</script>

<template>
  <div class="w-full flex flex-col gap-6 relative z-10 py-4">
    <div>
      <RouterLink
        to="/login"
        class="inline-flex items-center gap-2 text-sm font-semibold text-on-surface-variant hover:text-primary transition-colors group"
      >
        <span
          class="material-symbols-outlined text-[18px] group-hover:-translate-x-1 transition-transform"
        >
          arrow_back
        </span>
        <span>Volver al Login</span>
      </RouterLink>
    </div>

    <div
      class="flex flex-col sm:flex-row sm:items-end justify-between gap-6 border-b border-outline-variant/30 pb-6"
    >
      <div>
        <h1 class="text-3xl font-bold text-on-surface font-headline mb-2">
          Registro de Estudiante
        </h1>
        <p class="text-base text-on-surface-variant font-body">
          Completa los campos detallados a continuación para registrar tu cuenta de estudiante.
        </p>
      </div>

      <BaseAvatarUpload v-model="formData.fotografia" alt="Foto de perfil del estudiante" />
    </div>

    <form class="space-y-10" @submit.prevent="handleSubmit">
      <BaseAlert
        v-if="errorMessage"
        type="error"
        title="Error en el registro"
        :message="errorMessage"
        @dismiss="clearError"
      />

      <div class="space-y-6">
        <div class="flex items-center gap-3 border-b border-outline-variant/30 pb-3">
          <span class="material-symbols-outlined text-primary text-[24px]">person</span>
          <h2 class="text-xl font-bold text-on-surface font-headline">Información Personal</h2>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <BaseInput
            id="nombre"
            v-model="formData.nombre"
            name="nombre"
            label="Nombre"
            placeholder="Ej. Carlos"
            autocomplete="given-name"
            required
          />

          <BaseInput
            id="apellido"
            v-model="formData.apellido"
            name="apellido"
            label="Apellido"
            placeholder="Ej. Gómez"
            autocomplete="family-name"
            required
          />

          <BaseInput
            id="carnet"
            v-model="formData.carnet"
            name="carnet"
            label="Carnet Universitario"
            placeholder="Ej. 202010123"
            required
          />

          <BaseSelect
            id="genero"
            v-model="formData.genero"
            name="genero"
            label="Género"
            placeholder="Selecciona tu género"
            :options="genderOptions"
            required
          />

          <BaseInput
            id="telefono"
            v-model="formData.telefono"
            name="telefono"
            type="tel"
            label="Teléfono"
            placeholder="Ej. +502 5555 1234"
            autocomplete="tel"
            required
          />

          <BaseInput
            id="fechaNacimiento"
            v-model="formData.fechaNacimiento"
            name="fechaNacimiento"
            type="date"
            label="Fecha de nacimiento"
            trailing-icon="calendar_today"
            required
          />

          <div class="md:col-span-2">
            <BaseInput
              id="direccion"
              v-model="formData.direccion"
              name="direccion"
              label="Dirección"
              placeholder="Ej. Zona 1, Ciudad de Guatemala"
              autocomplete="street-address"
              required
            />
          </div>
        </div>
      </div>

      <div class="space-y-6">
        <div class="flex items-center gap-3 border-b border-outline-variant/30 pb-3">
          <span class="material-symbols-outlined text-primary text-[24px]">lock</span>
          <h2 class="text-xl font-bold text-on-surface font-headline">Credenciales</h2>
        </div>

        <div class="grid grid-cols-1 gap-6">
          <BaseInput
            id="correo"
            v-model="formData.correo"
            name="correo"
            type="email"
            label="Correo electrónico"
            placeholder="estudiante@universidad.edu"
            icon="mail"
            autocomplete="email"
            required
          />

          <div class="space-y-3">
            <BaseInput
              id="password"
              v-model="formData.password"
              name="password"
              type="password"
              label="Contraseña"
              placeholder="••••••••"
              icon="key"
              autocomplete="new-password"
              show-password-toggle
              required
            />

            <PasswordRequirements :password="formData.password" />
          </div>

          <BaseInput
            id="confirmPassword"
            v-model="formData.confirmPassword"
            name="confirmPassword"
            type="password"
            label="Confirmar Contraseña"
            placeholder="••••••••"
            icon="key"
            autocomplete="new-password"
            show-password-toggle
            :error="passwordMismatch ? 'Las contraseñas no coinciden.' : undefined"
            required
          />
        </div>
      </div>

      <div
        class="pt-8 border-t border-outline-variant/30 flex flex-col-reverse sm:flex-row items-center justify-between gap-6"
      >
        <RouterLink
          to="/login"
          class="text-sm font-semibold text-on-surface-variant hover:text-primary transition-colors inline-flex items-center gap-2 group"
        >
          <span class="material-symbols-outlined group-hover:-translate-x-1 transition-transform">
            arrow_back
          </span>
          Volver al Login
        </RouterLink>

        <BaseButton
          type="submit"
          variant="primary"
          size="md"
          :loading="isLoading"
          class="w-full sm:w-auto px-8 py-3"
        >
          <span class="inline-flex items-center gap-2">
            <span>Registrarse</span>
            <span class="material-symbols-outlined text-lg">how_to_reg</span>
          </span>
        </BaseButton>
      </div>
    </form>
  </div>
</template>
