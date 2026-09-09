<script setup lang="ts">
import {
  BaseAlert,
  BaseAvatarUpload,
  BaseButton,
  BaseInput,
  BaseSelect,
  type SelectOption
} from '@/components/ui'
import PasswordRequirements from '@/features/auth/components/PasswordRequirements.vue'
import { useTutorProfile } from '../composables/useTutorProfile'

const {
  formData,
  passwordData,
  isLoading,
  isSaving,
  isChangingPassword,
  errorMessage,
  successMessage,
  passwordErrorMessage,
  passwordSuccessMessage,
  passwordMismatch,
  canSaveProfile,
  canChangePassword,
  clearProfileMessages,
  clearPasswordMessages,
  saveProfile,
  changePassword
} = useTutorProfile()

const genderOptions: SelectOption[] = [
  { value: 'masculino', label: 'Masculino' },
  { value: 'femenino', label: 'Femenino' }
]

const currentYear = new Date().getFullYear()

function handleCarnetInput(value: string) {
  formData.carnetId = value.replace(/\D/g, '').slice(0, 10)
  clearProfileMessages()
}

function handleIdentificationInput(value: string) {
  formData.numeroIdentificacion = value
    .replace(/\D/g, '')
    .slice(0, 13)

  clearProfileMessages()
}

function handlePhoneInput(value: string) {
  formData.telefono = value.replace(/\D/g, '').slice(0, 8)
  clearProfileMessages()
}
</script>

<template>
  <div class="flex flex-col w-full max-w-5xl mx-auto">
    <div class="mb-8">
      <h1
        class="text-3xl font-bold font-headline text-on-surface tracking-tight mb-2"
      >
        Mi Perfil
      </h1>

      <p class="text-base text-on-surface-variant font-body">
        Consulta y actualiza tu información personal y profesional.
      </p>
    </div>

    <div
      v-if="isLoading"
      class="flex items-center justify-center gap-3 py-16 text-on-surface-variant"
    >
      <span
        class="material-symbols-outlined animate-spin text-[22px]"
      >
        progress_activity
      </span>

      <p class="text-sm font-medium">
        Cargando información de tu perfil...
      </p>
    </div>

    <template v-else>
      <BaseAlert
        v-if="errorMessage"
        type="error"
        title="No fue posible actualizar el perfil"
        :message="errorMessage"
        class="mb-6"
        @dismiss="clearProfileMessages"
      />

      <BaseAlert
        v-if="successMessage"
        type="success"
        title="Perfil actualizado"
        :message="successMessage"
        class="mb-6"
        @dismiss="clearProfileMessages"
      />

      <form
        class="bg-surface-container-lowest rounded-2xl border border-outline-variant/20 shadow-sm p-6 sm:p-8 mb-6"
        @submit.prevent="saveProfile"
      >
        <div
          class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-6 mb-8 pb-6 border-b border-outline-variant/30"
        >
          <div>
            <h2
              class="text-xl font-bold text-on-surface font-headline"
            >
              Información del Tutor
            </h2>

            <p
              class="text-sm text-on-surface-variant mt-1"
            >
              Mantén actualizados tus datos personales.
            </p>
          </div>

          <div class="flex flex-col items-center gap-2">
            <BaseAvatarUpload
              v-model="formData.fotografia"
              alt="Fotografía del tutor"
            />

            <span
              class="text-xs text-on-surface-variant"
            >
              Cambiar fotografía
            </span>
          </div>
        </div>

        <div class="space-y-8">
          <section>
            <div class="flex items-center gap-3 mb-5">
              <span
                class="material-symbols-outlined text-primary"
              >
                person
              </span>

              <h3
                class="text-lg font-bold text-on-surface"
              >
                Información personal
              </h3>
            </div>

            <div
              class="grid grid-cols-1 md:grid-cols-2 gap-6"
            >
              <BaseInput
                v-model="formData.nombre"
                label="Nombre"
                required
                @update:model-value="clearProfileMessages"
              />

              <BaseInput
                v-model="formData.apellido"
                label="Apellido"
                required
                @update:model-value="clearProfileMessages"
              />

              <BaseInput
                :model-value="formData.carnetId"
                label="Carnet Universitario / Tutor"
                only-numbers
                :maxlength="10"
                required
                @update:model-value="handleCarnetInput"
              />

              <BaseInput
                :model-value="formData.numeroIdentificacion"
                label="DPI / Documento de Identificación"
                only-numbers
                :maxlength="13"
                required
                @update:model-value="handleIdentificationInput"
              />

              <BaseSelect
                v-model="formData.genero"
                label="Género"
                :options="genderOptions"
                required
              />

              <BaseInput
                :model-value="formData.telefono"
                type="tel"
                label="Teléfono"
                only-numbers
                :maxlength="8"
                required
                @update:model-value="handlePhoneInput"
              />

              <BaseInput
                v-model="formData.fechaNacimiento"
                type="date"
                label="Fecha de nacimiento"
                required
              />

              <BaseInput
                v-model="formData.direccion"
                label="Dirección de residencia"
                required
              />
            </div>
          </section>

          <section>
            <div class="flex items-center gap-3 mb-5">
              <span
                class="material-symbols-outlined text-secondary"
              >
                school
              </span>

              <h3
                class="text-lg font-bold text-on-surface"
              >
                Información profesional
              </h3>
            </div>

            <div
              class="grid grid-cols-1 md:grid-cols-2 gap-6"
            >
              <BaseInput
                v-model="formData.universidad"
                label="Universidad"
                required
              />

              <BaseInput
                v-model="formData.anioInicio"
                type="number"
                label="Año de inicio"
                min="1980"
                :max="currentYear"
                required
              />

              <div class="md:col-span-2">
                <BaseInput
                  v-model="formData.direccionTutoria"
                  label="Dirección / Salón de Tutoría"
                  required
                />
              </div>
            </div>
          </section>

          <section>
            <div class="flex items-center gap-3 mb-5">
              <span
                class="material-symbols-outlined text-primary"
              >
                mail
              </span>

              <h3
                class="text-lg font-bold text-on-surface"
              >
                Cuenta
              </h3>
            </div>

            <BaseInput
              v-model="formData.correo"
              type="email"
              label="Correo electrónico"
              disabled
            />

            <p
              class="text-xs text-on-surface-variant mt-2"
            >
              El correo electrónico no puede modificarse.
            </p>
          </section>
        </div>

        <div
          class="flex justify-end mt-8 pt-6 border-t border-outline-variant/20"
        >
          <BaseButton
            type="submit"
            variant="primary"
            size="md"
            :loading="isSaving"
            :disabled="!canSaveProfile"
          >
            <template #loading>
              Guardando...
            </template>

            Guardar cambios
          </BaseButton>
        </div>
      </form>

      <!-- Cambio de contraseña -->
      <form
        class="bg-surface-container-lowest rounded-2xl border border-outline-variant/20 shadow-sm p-6 sm:p-8"
        @submit.prevent="changePassword"
      >
        <div class="flex items-center gap-3 mb-6">
          <div
            class="w-11 h-11 rounded-xl bg-primary/10 text-primary flex items-center justify-center"
          >
            <span class="material-symbols-outlined">
              lock
            </span>
          </div>

          <div>
            <h2
              class="text-xl font-bold text-on-surface font-headline"
            >
              Cambiar contraseña
            </h2>

            <p
              class="text-sm text-on-surface-variant mt-1"
            >
              Confirma primero tu contraseña actual.
            </p>
          </div>
        </div>

        <BaseAlert
          v-if="passwordErrorMessage"
          type="error"
          title="No fue posible cambiar la contraseña"
          :message="passwordErrorMessage"
          class="mb-6"
          @dismiss="clearPasswordMessages"
        />

        <BaseAlert
          v-if="passwordSuccessMessage"
          type="success"
          title="Contraseña actualizada"
          :message="passwordSuccessMessage"
          class="mb-6"
          @dismiss="clearPasswordMessages"
        />

        <div class="space-y-6">
          <BaseInput
            v-model="passwordData.passwordActual"
            type="password"
            label="Contraseña actual"
            show-password-toggle
            autocomplete="current-password"
            @update:model-value="clearPasswordMessages"
          />

          <div class="space-y-3">
            <BaseInput
              v-model="passwordData.nuevaPassword"
              type="password"
              label="Nueva contraseña"
              show-password-toggle
              autocomplete="new-password"
              @update:model-value="clearPasswordMessages"
            />

            <PasswordRequirements
              :password="passwordData.nuevaPassword"
            />
          </div>

          <BaseInput
            v-model="passwordData.confirmarNuevaPassword"
            type="password"
            label="Confirmar nueva contraseña"
            show-password-toggle
            autocomplete="new-password"
            :error="
              passwordMismatch
                ? 'Las contraseñas no coinciden.'
                : undefined
            "
            @update:model-value="clearPasswordMessages"
          />
        </div>

        <div
          class="flex justify-end mt-8 pt-6 border-t border-outline-variant/20"
        >
          <BaseButton
            type="submit"
            variant="primary"
            size="md"
            :loading="isChangingPassword"
            :disabled="!canChangePassword"
          >
            <template #loading>
              Actualizando...
            </template>

            Cambiar contraseña
          </BaseButton>
        </div>
      </form>
    </template>
  </div>
</template>