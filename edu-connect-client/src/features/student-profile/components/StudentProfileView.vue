<script setup lang="ts">
import {
  computed,
  onBeforeUnmount,
  reactive,
  ref,
  watch
} from 'vue'

import {
  BaseAlert,
  BaseButton,
  BaseInput,
  BaseModal,
  BaseSelect,
  type SelectOption
} from '@/components/ui'

import { PasswordRequirements } from '@/features/auth'
import { useStudentProfile } from '../composables/useStudentProfile'

const {
  profile,
  isLoading,
  isSaving,
  isChangingPassword,
  errorMessage,
  successMessage,
  passwordErrorMessage,
  updateProfile,
  changePassword,
  clearMessages,
  clearPasswordError
} = useStudentProfile()

const genderOptions: SelectOption[] = [
  {
    value: 'femenino',
    label: 'Femenino'
  },
  {
    value: 'masculino',
    label: 'Masculino'
  }
]

const form = reactive({
  nombre: '',
  apellido: '',
  carnet: '',
  genero: '',
  direccion: '',
  telefono: '',
  fechaNacimiento: ''
})

const fotografia = ref<File | null>(null)
const fotografiaPreview = ref<string | null>(null)
const fileInput = ref<HTMLInputElement | null>(null)

const isPasswordModalOpen = ref(false)

const passwordForm = reactive({
  passwordActual: '',
  nuevaPassword: '',
  confirmarNuevaPassword: ''
})

const carnetError = ref('')
const telefonoError = ref('')
const fechaNacimientoError = ref('')

const maxBirthDate = computed(() => {
  const today = new Date()

  today.setFullYear(today.getFullYear() - 16)

  return today.toISOString().split('T')[0]
})

const passwordMismatch = computed(() => {
  if (
    !passwordForm.nuevaPassword ||
    !passwordForm.confirmarNuevaPassword
  ) {
    return false
  }

  return (
    passwordForm.nuevaPassword !==
    passwordForm.confirmarNuevaPassword
  )
})

const passwordIsValid = computed(() => {
  const password = passwordForm.nuevaPassword

  return (
    password.length >= 8 &&
    /[A-Z]/.test(password) &&
    /[a-z]/.test(password) &&
    /[0-9]/.test(password)
  )
})

watch(
  profile,
  value => {
    if (!value) return

    form.nombre = value.nombre
    form.apellido = value.apellido
    form.carnet = value.carnet
    form.genero = value.genero?.toLowerCase() || ''
    form.direccion = value.direccion
    form.telefono = value.telefono
    form.fechaNacimiento = value.fechaNacimiento

    if (
      fotografiaPreview.value &&
      fotografiaPreview.value.startsWith('blob:')
    ) {
      URL.revokeObjectURL(fotografiaPreview.value)
    }

    fotografia.value = null
    fotografiaPreview.value = value.fotografiaUrl || null
  },
  {
    immediate: true
  }
)

function triggerFileInput() {
  fileInput.value?.click()
}

function handleFileChange(event: Event) {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]

  if (!file) return

  if (!file.type.startsWith('image/')) {
    return
  }

  if (
    fotografiaPreview.value &&
    fotografiaPreview.value.startsWith('blob:')
  ) {
    URL.revokeObjectURL(fotografiaPreview.value)
  }

  fotografia.value = file
  fotografiaPreview.value = URL.createObjectURL(file)
}

function handleCarnet(value: string) {
  form.carnet = value.replace(/\D/g, '').slice(0, 10)
  carnetError.value = ''
}

function handleTelefono(value: string) {
  form.telefono = value.replace(/\D/g, '').slice(0, 8)
  telefonoError.value = ''
}

function validarPerfil(): boolean {
  carnetError.value = ''
  telefonoError.value = ''
  fechaNacimientoError.value = ''

  if (!/^\d{6,10}$/.test(form.carnet)) {
    carnetError.value =
      'El carnet debe contener entre 6 y 10 dígitos.'
  }

  if (!/^\d{8}$/.test(form.telefono)) {
    telefonoError.value =
      'El teléfono debe contener exactamente 8 dígitos.'
  }

  if (
    !form.fechaNacimiento ||
    form.fechaNacimiento > maxBirthDate.value
  ) {
    fechaNacimientoError.value =
      'El estudiante debe tener al menos 16 años cumplidos.'
  }

  return !(
    carnetError.value ||
    telefonoError.value ||
    fechaNacimientoError.value
  )
}

async function handleSubmit() {
  clearMessages()

  if (!validarPerfil()) return

  const success = await updateProfile({
    nombre: form.nombre.trim(),
    apellido: form.apellido.trim(),
    carnet: form.carnet,
    genero: form.genero,
    direccion: form.direccion.trim(),
    telefono: form.telefono,
    fechaNacimiento: form.fechaNacimiento,
    fotografia: fotografia.value
  })

  if (success) {
    fotografia.value = null

    window.scrollTo({
      top: 0,
      behavior: 'smooth'
    })
  }
}

function openPasswordModal() {
  clearPasswordError()

  passwordForm.passwordActual = ''
  passwordForm.nuevaPassword = ''
  passwordForm.confirmarNuevaPassword = ''

  isPasswordModalOpen.value = true
}

function closePasswordModal() {
  if (isChangingPassword.value) return

  isPasswordModalOpen.value = false

  clearPasswordError()

  passwordForm.passwordActual = ''
  passwordForm.nuevaPassword = ''
  passwordForm.confirmarNuevaPassword = ''
}

async function handlePasswordChange() {
  clearPasswordError()

  if (!passwordForm.passwordActual.trim()) {
    return
  }

  if (!passwordIsValid.value || passwordMismatch.value) {
    return
  }

  const success = await changePassword({
    passwordActual: passwordForm.passwordActual,
    nuevaPassword: passwordForm.nuevaPassword,
    confirmarNuevaPassword:
      passwordForm.confirmarNuevaPassword
  })

  if (success) {
    closePasswordModal()

    window.scrollTo({
      top: 0,
      behavior: 'smooth'
    })
  }
}

onBeforeUnmount(() => {
  if (
    fotografiaPreview.value &&
    fotografiaPreview.value.startsWith('blob:')
  ) {
    URL.revokeObjectURL(fotografiaPreview.value)
  }
})
</script>

<template>
  <div class="w-full max-w-5xl mx-auto">
    <!-- Mensajes -->
    <BaseAlert
      v-if="errorMessage"
      type="error"
      title="No se pudo actualizar el perfil"
      :message="errorMessage"
      class="mb-6"
      @dismiss="clearMessages"
    />

    <BaseAlert
      v-if="successMessage"
      type="success"
      title="Cambios guardados"
      :message="successMessage"
      class="mb-6"
      @dismiss="clearMessages"
    />

    <!-- Cargando -->
    <div
      v-if="isLoading"
      class="min-h-[500px] flex flex-col items-center justify-center gap-3"
    >
      <span
        class="material-symbols-outlined text-primary text-[42px] animate-spin"
      >
        progress_activity
      </span>

      <p class="text-on-surface-variant">
        Cargando información del perfil...
      </p>
    </div>

    <!-- Perfil -->
    <div
      v-else-if="profile"
      class="w-full bg-surface-container-lowest rounded-2xl shadow-sm border border-surface-container overflow-hidden"
    >
      <!-- Banner -->
      <div class="h-40 bg-primary relative">
        <div
          class="absolute inset-0 bg-gradient-to-bl from-primary-fixed-dim/20 via-transparent to-transparent"
        />

        <div class="absolute bottom-6 left-6 sm:left-8 text-on-primary">
          <h1 class="text-3xl font-bold font-headline">
            Perfil de Estudiante
          </h1>

          <p class="text-base opacity-80 mt-1">
            Configuración y detalles de la cuenta
          </p>
        </div>
      </div>

      <!-- Contenido -->
      <div class="px-6 sm:px-8 pb-8 pt-20 relative">
        <!-- Avatar -->
        <div
          class="absolute -top-16 right-6 sm:right-12 flex flex-col items-center gap-2"
        >
          <button
            type="button"
            class="relative group rounded-full"
            aria-label="Cambiar fotografía"
            @click="triggerFileInput"
          >
            <div
              class="w-32 h-32 rounded-full bg-surface-container-highest flex items-center justify-center overflow-hidden shadow-md border-4 border-surface-container-lowest"
            >
              <img
                v-if="fotografiaPreview"
                :src="fotografiaPreview"
                alt="Fotografía del estudiante"
                class="w-full h-full object-cover"
              />

              <span
                v-else
                class="material-symbols-outlined text-[52px] text-on-surface-variant"
              >
                person
              </span>
            </div>

            <div
              class="absolute inset-0 rounded-full bg-primary/60 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity"
            >
              <span
                class="material-symbols-outlined text-on-primary text-[28px]"
              >
                photo_camera
              </span>
            </div>
          </button>

          <button
            type="button"
            class="text-xs font-semibold text-secondary hover:text-secondary-container transition-colors"
            @click="triggerFileInput"
          >
            Cambiar foto
          </button>

          <input
            ref="fileInput"
            type="file"
            accept="image/*"
            class="hidden"
            @change="handleFileChange"
          />
        </div>

        <form
          class="flex flex-col gap-6"
          @submit.prevent="handleSubmit"
        >
          <!-- Nombre y apellido -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <BaseInput
              id="nombre"
              v-model="form.nombre"
              name="nombre"
              label="Nombres"
              autocomplete="given-name"
              required
            />

            <BaseInput
              id="apellido"
              v-model="form.apellido"
              name="apellido"
              label="Apellidos"
              autocomplete="family-name"
              required
            />
          </div>

          <!-- Carnet y género -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <BaseInput
              id="carnet"
              :model-value="form.carnet"
              name="carnet"
              label="Carnet universitario"
              only-numbers
              :maxlength="10"
              :error="carnetError || undefined"
              required
              @update:model-value="handleCarnet"
            />

            <BaseSelect
              id="genero"
              v-model="form.genero"
              name="genero"
              label="Género"
              placeholder="Selecciona tu género"
              :options="genderOptions"
              required
            />
          </div>

          <!-- Teléfono y nacimiento -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <BaseInput
              id="telefono"
              :model-value="form.telefono"
              name="telefono"
              label="Teléfono"
              type="tel"
              only-numbers
              :maxlength="8"
              :error="telefonoError || undefined"
              autocomplete="tel"
              required
              @update:model-value="handleTelefono"
            />

            <BaseInput
              id="fechaNacimiento"
              v-model="form.fechaNacimiento"
              name="fechaNacimiento"
              label="Fecha de nacimiento"
              type="date"
              trailing-icon="calendar_today"
              :max="maxBirthDate"
              :error="fechaNacimientoError || undefined"
              required
            />
          </div>

          <!-- Dirección -->
          <BaseInput
            id="direccion"
            v-model="form.direccion"
            name="direccion"
            label="Dirección"
            icon="location_on"
            autocomplete="street-address"
            required
          />

          <!-- Correo bloqueado -->
          <div class="flex flex-col gap-2">
            <label
              for="correo"
              class="text-sm font-semibold text-on-surface flex items-center gap-2"
            >
              Correo electrónico

              <span
                class="material-symbols-outlined text-[16px] text-on-surface-variant"
                title="El correo electrónico no puede modificarse"
              >
                lock
              </span>
            </label>

            <input
              id="correo"
              :value="profile.correo"
              type="email"
              disabled
              class="w-full bg-surface-container-low text-on-surface-variant px-4 py-3 rounded-lg border border-outline-variant/40 opacity-70 cursor-not-allowed"
            />

            <p class="text-xs text-on-surface-variant">
              El correo se utiliza para el acceso a tu cuenta y no
              puede modificarse desde el perfil.
            </p>
          </div>

          <!-- Acciones -->
          <div
            class="flex flex-col sm:flex-row items-stretch sm:items-center justify-between gap-4 pt-7 mt-2 border-t border-surface-container"
          >
            <BaseButton
              type="button"
              variant="surface"
              icon="key"
              @click="openPasswordModal"
            >
              Cambiar Contraseña
            </BaseButton>

            <BaseButton
              type="submit"
              variant="primary"
              icon="save"
              :loading="isSaving"
              :disabled="isSaving"
            >
              Guardar Cambios
            </BaseButton>
          </div>
        </form>
      </div>
    </div>

    <!-- Modal de contraseña -->
    <BaseModal
      v-model="isPasswordModalOpen"
      title="Cambiar Contraseña"
      max-width="lg"
      @close="closePasswordModal"
    >
      <div class="space-y-6">
        <p class="text-sm text-on-surface-variant">
          Por seguridad, debes ingresar tu contraseña actual antes
          de establecer una nueva.
        </p>

        <BaseAlert
          v-if="passwordErrorMessage"
          type="error"
          title="No se pudo cambiar la contraseña"
          :message="passwordErrorMessage"
          @dismiss="clearPasswordError"
        />

        <BaseInput
          id="passwordActual"
          v-model="passwordForm.passwordActual"
          name="passwordActual"
          type="password"
          label="Contraseña actual"
          autocomplete="current-password"
          show-password-toggle
          required
        />

        <BaseInput
          id="nuevaPassword"
          v-model="passwordForm.nuevaPassword"
          name="nuevaPassword"
          type="password"
          label="Nueva contraseña"
          autocomplete="new-password"
          show-password-toggle
          required
        />

        <PasswordRequirements
          :password="passwordForm.nuevaPassword"
        />

        <BaseInput
          id="confirmarNuevaPassword"
          v-model="passwordForm.confirmarNuevaPassword"
          name="confirmarNuevaPassword"
          type="password"
          label="Confirmar nueva contraseña"
          autocomplete="new-password"
          show-password-toggle
          :error="
            passwordMismatch
              ? 'Las contraseñas no coinciden.'
              : undefined
          "
          required
        />
      </div>

      <template #footer>
        <BaseButton
          variant="outline"
          :disabled="isChangingPassword"
          @click="closePasswordModal"
        >
          Cancelar
        </BaseButton>

        <BaseButton
          variant="primary"
          icon="key"
          :loading="isChangingPassword"
          :disabled="
            isChangingPassword ||
            !passwordForm.passwordActual ||
            !passwordIsValid ||
            passwordMismatch ||
            !passwordForm.confirmarNuevaPassword
          "
          @click="handlePasswordChange"
        >
          Actualizar Contraseña
        </BaseButton>
      </template>
    </BaseModal>
  </div>
</template>