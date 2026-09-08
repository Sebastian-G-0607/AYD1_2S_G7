<script setup lang="ts">
import { reactive, computed, ref } from 'vue'
import { RouterLink } from 'vue-router'
import {
  BaseButton,
  BaseInput,
  BaseSelect,
  BaseAlert,
  BaseAvatarUpload,
  BaseMultiSelect,
  type SelectOption
} from '@/components/ui'
import PasswordRequirements from './PasswordRequirements.vue'
import { useAuth } from '../composables/useAuth'
import { useMaterias } from '@/composables/useMaterias'
import type { TutorRegisterData } from '../types'

const formData = reactive<TutorRegisterData>({
  nombre: '',
  apellido: '',
  carnetId: '',
  numeroIdentificacion: '',
  genero: '',
  direccion: '',
  telefono: '',
  fechaNacimiento: '',
  fotografia: null,
  direccionTutoria: '',
  anioInicio: new Date().getFullYear(),
  universidad: '',
  correo: '',
  password: '',
  confirmPassword: '',
  materiasIds: [],
  horaInicio: '',
  horaFin: '',
  diasAtencion: []
})

const {
  materias,
  isLoading: isLoadingMaterias,
  error: errorMaterias,
  fetchMaterias
} = useMaterias()

const materiaOptions = computed<SelectOption[]>(() => {
  return materias.value.map(m => ({
    value: m.id,
    label: m.nombre
  }))
})

const selectedDays = ref<number[]>([1, 2, 3, 4, 5])

const daysOfWeek = [
  { id: 1, label: 'Lun' },
  { id: 2, label: 'Mar' },
  { id: 3, label: 'Mié' },
  { id: 4, label: 'Jue' },
  { id: 5, label: 'Vie' },
  { id: 6, label: 'Sáb' },
  { id: 7, label: 'Dom' }
]

const toggleDay = (dayId: number) => {
  const index = selectedDays.value.indexOf(dayId)
  if (index === -1) {
    selectedDays.value.push(dayId)
  } else {
    selectedDays.value.splice(index, 1)
  }
}

const isDaySelected = (dayId: number) => selectedDays.value.includes(dayId)

const genderOptions: SelectOption[] = [
  { value: 'masculino', label: 'Masculino' },
  { value: 'femenino', label: 'Femenino' }
]

const { isLoading, errorMessage, clearError, registerTutor, isAtLeast18YearsOld } = useAuth()

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
const idTouchedError = ref<string | null>(null)
const phoneTouchedError = ref<string | null>(null)
const birthDateTouchedError = ref<string | null>(null)

const carnetError = computed(() => {
  if (carnetTouchedError.value) return carnetTouchedError.value
  if (!errorMessage.value) return undefined
  const lower = errorMessage.value.toLowerCase()
  if (lower.includes('carnet')) {
    return errorMessage.value
  }
  return undefined
})

const idError = computed(() => {
  if (idTouchedError.value) return idTouchedError.value
  if (!errorMessage.value) return undefined
  const lower = errorMessage.value.toLowerCase()
  if (
    lower.includes('identificación') ||
    lower.includes('identificacion') ||
    lower.includes('dpi')
  ) {
    return errorMessage.value
  }
  return undefined
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
  if (lower.includes('18 años') || lower.includes('nacimiento')) {
    return errorMessage.value
  }
  return undefined
})

const maxBirthDate = computed(() => {
  const date = new Date()
  date.setFullYear(date.getFullYear() - 18)
  return date.toISOString().split('T')[0]
})

const handleCarnetInput = (val: string) => {
  formData.carnetId = val.replace(/\D/g, '').slice(0, 10)
  carnetTouchedError.value = null
  if (errorMessage.value?.toLowerCase().includes('carnet')) clearError()
}

const onCarnetBlur = () => {
  if (formData.carnetId && formData.carnetId.length < 6) {
    carnetTouchedError.value = 'El carnet debe tener al menos 6 dígitos numéricos.'
  } else {
    carnetTouchedError.value = null
  }
}

const handleIdInput = (val: string) => {
  formData.numeroIdentificacion = val.replace(/\D/g, '').slice(0, 13)
  idTouchedError.value = null
  if (
    errorMessage.value?.toLowerCase().includes('identificación') ||
    errorMessage.value?.toLowerCase().includes('identificacion') ||
    errorMessage.value?.toLowerCase().includes('dpi')
  ) {
    clearError()
  }
}

const onIdBlur = () => {
  if (formData.numeroIdentificacion && formData.numeroIdentificacion.length < 13) {
    idTouchedError.value = 'El DPI debe tener exactamente 13 dígitos numéricos.'
  } else {
    idTouchedError.value = null
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
  if (formData.fechaNacimiento && !isAtLeast18YearsOld(formData.fechaNacimiento)) {
    birthDateTouchedError.value = 'El tutor debe ser mayor de 18 años.'
  } else {
    birthDateTouchedError.value = null
  }
}

const handleSubmit = async () => {
  if (passwordMismatch.value) return

  if (formData.materiasIds.length === 0) {
    window.scrollTo({ top: 400, behavior: 'smooth' })
  }

  const payload: TutorRegisterData = {
    ...formData,
    materiasIds: formData.materiasIds.map(Number),
    diasAtencion: [...selectedDays.value]
  }

  const success = await registerTutor(payload)
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
        <h1 class="text-3xl font-bold text-on-surface font-headline mb-2">Registro de Tutor</h1>
        <p class="text-base text-on-surface-variant font-body">
          Únete a nuestra red de tutores y comparte tus conocimientos con los estudiantes.
        </p>
      </div>

      <div class="flex flex-col items-center sm:items-end">
        <BaseAvatarUpload v-model="formData.fotografia" alt="Foto de perfil del tutor" />
        <span class="text-xs text-error font-medium mt-1">* Fotografía obligatoria</span>
      </div>
    </div>

    <form class="space-y-12" @submit.prevent="handleSubmit">
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
            placeholder="Ej. Ana"
            autocomplete="given-name"
            required
          />

          <BaseInput
            id="apellido"
            v-model="formData.apellido"
            name="apellido"
            label="Apellido"
            placeholder="Ej. Martínez"
            autocomplete="family-name"
            required
          />

          <BaseInput
            id="carnetId"
            :model-value="formData.carnetId"
            name="carnetId"
            label="Carnet Universitario / Tutor"
            placeholder="Ej. 201800456"
            only-numbers
            :maxlength="10"
            :error="carnetError"
            required
            @update:model-value="handleCarnetInput"
            @blur="onCarnetBlur"
          />

          <BaseInput
            id="numeroIdentificacion"
            :model-value="formData.numeroIdentificacion"
            name="numeroIdentificacion"
            label="DPI / Documento de Identificación"
            placeholder="Ej. 3001123450101"
            only-numbers
            :maxlength="13"
            :error="idError"
            required
            @update:model-value="handleIdInput"
            @blur="onIdBlur"
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
            placeholder="Ej. 55559876"
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
            @blur="onBirthDateBlur"
            @update:model-value="birthDateError && (birthDateTouchedError = null)"
          />

          <BaseInput
            id="direccion"
            v-model="formData.direccion"
            name="direccion"
            label="Dirección de Residencia"
            placeholder="Ej. Zona 10, Ciudad de Guatemala"
            autocomplete="street-address"
            required
          />
        </div>
      </div>

      <div class="space-y-6">
        <div class="flex items-center gap-3 border-b border-outline-variant/30 pb-3">
          <span class="material-symbols-outlined text-secondary text-[24px]">school</span>
          <h2 class="text-xl font-bold text-on-surface font-headline">Información Profesional</h2>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <BaseInput
            id="universidad"
            v-model="formData.universidad"
            name="universidad"
            label="Universidad"
            placeholder="Ej. Universidad de San Carlos de Guatemala"
            required
          />

          <BaseInput
            id="anioInicio"
            v-model="formData.anioInicio"
            name="anioInicio"
            type="number"
            label="Año de Inicio"
            min="1980"
            max="2030"
            step="1"
            trailing-icon="calendar_today"
            required
          />

          <div class="md:col-span-2">
            <BaseInput
              id="direccionTutoria"
              v-model="formData.direccionTutoria"
              name="direccionTutoria"
              label="Dirección / Salón de Tutoría"
              placeholder="Ej. Edificio T-3, Salón 201"
              required
            />
          </div>

          <div class="md:col-span-2 flex flex-col gap-3">
            <BaseAlert
              v-if="errorMaterias"
              type="warning"
              title="Aviso del catálogo de materias"
              :message="errorMaterias"
              dismissible
            >
              <template #default>
                <BaseButton
                  type="button"
                  variant="outline"
                  size="sm"
                  class="mt-2"
                  @click="fetchMaterias"
                >
                  Reintentar carga
                </BaseButton>
              </template>
            </BaseAlert>

            <BaseMultiSelect
              id="materiasIds"
              v-model="formData.materiasIds"
              name="materiasIds"
              label="Materias de Especialidad"
              placeholder="Buscar o seleccionar materias..."
              :options="materiaOptions"
              :loading="isLoadingMaterias"
              hint="Haz clic para ver las materias disponibles o escribe para buscar."
              required
            />
          </div>

          <BaseInput
            id="horaInicio"
            v-model="formData.horaInicio"
            name="horaInicio"
            type="time"
            label="Hora de Inicio de Atención"
            trailing-icon="schedule"
          />

          <BaseInput
            id="horaFin"
            v-model="formData.horaFin"
            name="horaFin"
            type="time"
            label="Hora de Fin de Atención"
            trailing-icon="schedule"
          />

          <div class="md:col-span-2 flex flex-col gap-2">
            <label class="text-sm font-semibold text-on-surface">
              Días de Atención Disponibles
            </label>
            <div class="flex flex-wrap gap-2">
              <button
                v-for="day in daysOfWeek"
                :key="day.id"
                type="button"
                :class="[
                  'px-4 py-2 rounded-lg text-sm font-semibold transition-all border',
                  isDaySelected(day.id)
                    ? 'bg-primary text-on-primary border-primary shadow-sm'
                    : 'bg-surface-container-low text-on-surface-variant border-outline-variant/40 hover:bg-surface-container-high'
                ]"
                @click="toggleDay(day.id)"
              >
                {{ day.label }}
              </button>
            </div>
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
            placeholder="tutor@universidad.edu"
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
            <span class="material-symbols-outlined text-lg">person_add</span>
          </span>
        </BaseButton>
      </div>
    </form>
  </div>
</template>
