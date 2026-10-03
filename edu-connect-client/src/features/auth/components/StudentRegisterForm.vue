<script setup lang="ts">
import { reactive, computed, ref } from 'vue'
import { RouterLink } from 'vue-router'
import {
  BaseButton,
  BaseInput,
  BaseSelect,
  BaseAlert,
  BaseAvatarUpload,
  BaseFileUpload,
  type SelectOption
} from '@/components/ui'
import PasswordRequirements from './PasswordRequirements.vue'
import { useAuth } from '../composables/useAuth'
import type { StudentRegisterData } from '../types'
import {
  MAX_IMAGE_SIZE_MB,
  MAX_PDF_SIZE_MB,
  validateImageFile,
  validatePdfFile
} from '@/utils/fileValidation'

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
  fotografia: null,
  documentoCarnet: null
})

const genderOptions: SelectOption[] = [
  { value: 'masculino', label: 'Masculino' },
  { value: 'femenino', label: 'Femenino' }
]

const { isLoading, errorMessage, clearError, registerStudent, isAtLeast16YearsOld } = useAuth()

const passwordMismatch = computed(() => {
  if (!formData.confirmPassword || !formData.password) return false
  return formData.password !== formData.confirmPassword
})

const emailError = computed(() => {
  if (!errorMessage.value) return undefined
  const lower = errorMessage.value.toLowerCase()
  if (lower.includes('correo') || lower.includes('email')) {
    return errorMessage.value
  }
  return undefined
})

const carnetTouchedError = ref<string | null>(null)
const phoneTouchedError = ref<string | null>(null)
const birthDateTouchedError = ref<string | null>(null)

const isFileErrorMessage = (message: string): boolean => {
  const lower = message.toLowerCase()
  return lower.includes('pdf') || lower.includes('fotograf')
}

const carnetError = computed(() => {
  if (carnetTouchedError.value) return carnetTouchedError.value
  if (!errorMessage.value) return undefined
  const lower = errorMessage.value.toLowerCase()
  if (lower.includes('carnet') && !isFileErrorMessage(errorMessage.value)) {
    return errorMessage.value
  }
  return undefined
})

const photoTouchedError = ref<string | null>(null)
const carnetPdfTouchedError = ref<string | null>(null)
const avatarUploadRef = ref<InstanceType<typeof BaseAvatarUpload> | null>(null)

const photoError = computed(() => {
  if (photoTouchedError.value) return photoTouchedError.value
  if (!errorMessage.value) return undefined
  return errorMessage.value.toLowerCase().includes('fotograf') ? errorMessage.value : undefined
})

const carnetPdfError = computed(() => {
  if (carnetPdfTouchedError.value) return carnetPdfTouchedError.value
  if (!errorMessage.value) return undefined
  return errorMessage.value.toLowerCase().includes('pdf') ? errorMessage.value : undefined
})

const phoneError = computed(() => {
  if (phoneTouchedError.value) return phoneTouchedError.value
  if (!errorMessage.value) return undefined
  const lower = errorMessage.value.toLowerCase()
  if (lower.includes('teléfono') || lower.includes('telefono')) {
    return errorMessage.value
  }
  return undefined
})

const birthDateError = computed(() => {
  if (birthDateTouchedError.value) return birthDateTouchedError.value
  if (!errorMessage.value) return undefined
  const lower = errorMessage.value.toLowerCase()
  if (lower.includes('16 años') || lower.includes('nacimiento')) {
    return errorMessage.value
  }
  return undefined
})

const maxBirthDate = computed(() => {
  const date = new Date()
  date.setFullYear(date.getFullYear() - 16)
  return date.toISOString().split('T')[0]
})

const handleCarnetInput = (val: string) => {
  formData.carnet = val.replace(/\D/g, '').slice(0, 10)
  carnetTouchedError.value = null
  if (errorMessage.value?.toLowerCase().includes('carnet')) clearError()
}

const onCarnetBlur = () => {
  if (formData.carnet && formData.carnet.length < 6) {
    carnetTouchedError.value = 'El carnet debe tener al menos 6 dígitos numéricos.'
  } else {
    carnetTouchedError.value = null
  }
}

const handlePhoneInput = (val: string) => {
  formData.telefono = val.replace(/\D/g, '').slice(0, 8)
  phoneTouchedError.value = null
  if (
    errorMessage.value?.toLowerCase().includes('teléfono') ||
    errorMessage.value?.toLowerCase().includes('telefono')
  ) {
    clearError()
  }
}

const onPhoneBlur = () => {
  if (formData.telefono && formData.telefono.length < 8) {
    phoneTouchedError.value = 'El teléfono debe tener exactamente 8 dígitos numéricos.'
  } else {
    phoneTouchedError.value = null
  }
}

const onBirthDateBlur = () => {
  if (formData.fechaNacimiento && !isAtLeast16YearsOld(formData.fechaNacimiento)) {
    birthDateTouchedError.value = 'El estudiante debe tener al menos 16 años cumplidos.'
  } else {
    birthDateTouchedError.value = null
  }
}

const onPhotoSelected = (value: File | string | null) => {
  if (value instanceof File) {
    const validationMessage = validateImageFile(value)
    if (validationMessage) {
      // Se conserva la fotografía anterior (si había una) y se informa el motivo del rechazo.
      photoTouchedError.value = `No se aceptó "${value.name}". ${validationMessage}`
      return
    }
  }
  formData.fotografia = value
  photoTouchedError.value = null
  if (errorMessage.value && isFileErrorMessage(errorMessage.value)) clearError()
}

const removePhoto = () => {
  formData.fotografia = null
  photoTouchedError.value = null
}

const onCarnetPdfSelected = (file: File | null) => {
  formData.documentoCarnet = file
  carnetPdfTouchedError.value = null
  if (errorMessage.value && isFileErrorMessage(errorMessage.value)) clearError()
}

const validateRequiredFiles = (): boolean => {
  photoTouchedError.value = formData.fotografia ? null : 'La fotografía reciente es obligatoria.'
  carnetPdfTouchedError.value = formData.documentoCarnet
    ? null
    : 'El archivo PDF con tu carnet escaneado es obligatorio.'
  return !photoTouchedError.value && !carnetPdfTouchedError.value
}

const handleSubmit = async () => {
  if (passwordMismatch.value) return
  if (!validateRequiredFiles()) {
    window.scrollTo({ top: 0, behavior: 'smooth' })
    return
  }
  const success = await registerStudent({ ...formData })
  if (!success) {
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }
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

        <div class="mt-4 space-y-1">
          <p class="text-sm font-semibold text-on-surface">
            Fotografía reciente <span class="text-error">*</span>
          </p>
          <p class="text-xs text-on-surface-variant">
            Formato JPG, PNG o WEBP, máximo {{ MAX_IMAGE_SIZE_MB }} MB. Haz clic en la imagen para
            elegirla o cambiarla.
          </p>
          <div class="flex items-center gap-4 pt-1">
            <button
              type="button"
              class="text-sm font-semibold text-secondary hover:underline focus:outline-none focus:ring-2 focus:ring-primary rounded"
              @click="avatarUploadRef?.openFileDialog()"
            >
              {{ formData.fotografia ? 'Cambiar fotografía' : 'Elegir fotografía' }}
            </button>
            <button
              v-if="formData.fotografia"
              type="button"
              class="text-sm font-semibold text-error hover:underline focus:outline-none focus:ring-2 focus:ring-error rounded"
              @click="removePhoto"
            >
              Quitar fotografía
            </button>
          </div>
          <p
            v-if="photoError"
            data-testid="foto-error"
            aria-live="polite"
            class="flex items-start gap-1 text-xs text-error font-medium pt-1"
          >
            <span class="material-symbols-outlined text-[16px]">info</span>
            <span>{{ photoError }}</span>
          </p>
        </div>
      </div>

      <BaseAvatarUpload
        ref="avatarUploadRef"
        :model-value="formData.fotografia"
        alt="Foto de perfil del estudiante"
        @update:model-value="onPhotoSelected"
      />
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
            :model-value="formData.carnet"
            name="carnet"
            label="Carnet Universitario"
            placeholder="Ej. 202010123"
            only-numbers
            :maxlength="10"
            :error="carnetError"
            required
            @update:model-value="handleCarnetInput"
            @blur="onCarnetBlur"
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
            :model-value="formData.telefono"
            name="telefono"
            type="tel"
            label="Teléfono"
            placeholder="Ej. 55551234"
            only-numbers
            :maxlength="8"
            autocomplete="tel"
            :error="phoneError"
            required
            @update:model-value="handlePhoneInput"
            @blur="onPhoneBlur"
          />

          <BaseInput
            id="fechaNacimiento"
            v-model="formData.fechaNacimiento"
            name="fechaNacimiento"
            type="date"
            label="Fecha de nacimiento"
            trailing-icon="calendar_today"
            :max="maxBirthDate"
            :error="birthDateError"
            required
            @update:model-value="birthDateError && clearError()"
            @blur="onBirthDateBlur"
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
          <span class="material-symbols-outlined text-primary text-[24px]">description</span>
          <h2 class="text-xl font-bold text-on-surface font-headline">Documentos</h2>
        </div>

        <BaseFileUpload
          id="carnetPdf"
          name="documentoCarnet"
          label="Carnet escaneado (PDF)"
          accept="application/pdf"
          required
          :model-value="formData.documentoCarnet"
          :error="carnetPdfError"
          :hint="`Solo archivos PDF, máximo ${MAX_PDF_SIZE_MB} MB. El administrador lo usará para validar tu identidad.`"
          :validate="validatePdfFile"
          @update:model-value="onCarnetPdfSelected"
        />
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
            :error="emailError"
            required
            @update:model-value="emailError && clearError()"
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
