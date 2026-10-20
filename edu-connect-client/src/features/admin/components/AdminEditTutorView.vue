<script setup lang="ts">
import { useAdminEditTutor } from '../composables/useAdminEditTutor'

const {
  formData,
  tutorOriginal,
  availableMaterias,
  isLoading,
  isSaving,
  errorMessage,
  successMessage,
  fieldErrors,
  currentYear,
  maxBirthDate,
  minBirthDate,
  handleNombreInput,
  handleApellidoInput,
  handleCarnetInput,
  handleDpiInput,
  handlePhoneInput,
  toggleMateria,
  isMateriaSelected,
  clearMessages,
  clearFieldError,
  handleSave,
  goBack
} = useAdminEditTutor()
</script>

<template>
  <div class="flex flex-col w-full h-full relative font-body-md text-on-background">
    <div
      v-if="isLoading"
      class="flex flex-col items-center justify-center gap-4 py-24 text-on-surface-variant"
    >
      <span class="material-symbols-outlined animate-spin text-[36px] text-primary">
        progress_activity
      </span>
      <p class="font-label-md text-label-md">Cargando datos del tutor...</p>
    </div>

    <div
      v-else-if="errorMessage && !tutorOriginal"
      class="max-w-3xl mx-auto w-full my-12 p-6 rounded-2xl bg-error-container text-on-error-container flex flex-col items-center text-center gap-4 border border-error/20"
    >
      <span class="material-symbols-outlined text-[44px]">error</span>
      <h2 class="font-headline-md text-headline-md">No se pudo cargar el tutor</h2>
      <p class="font-body-md text-body-md">{{ errorMessage }}</p>
      <button
        type="button"
        class="mt-2 px-6 py-2.5 bg-primary text-on-primary font-label-md text-label-md rounded-lg shadow-sm hover:bg-primary/90 transition-all cursor-pointer"
        @click="goBack"
      >
        Volver a Gestión de Usuarios
      </button>
    </div>

    <div v-else class="w-full max-w-4xl mx-auto my-4 sm:my-8 relative">
      <div
        class="w-full bg-surface-container-lowest rounded-2xl shadow-sm border border-surface-container relative overflow-hidden flex flex-col"
      >
        <div class="h-44 bg-primary relative px-6 sm:px-8 pt-6 pb-6 flex flex-col justify-between">
          <div
            class="absolute inset-0 bg-[radial-gradient(ellipse_at_top_right,_var(--tw-gradient-stops))] from-primary-fixed-dim/20 via-transparent to-transparent pointer-events-none"
          ></div>

          <div>
            <button
              type="button"
              class="inline-flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-white/10 hover:bg-white/20 text-white text-xs font-semibold backdrop-blur-sm transition-all cursor-pointer"
              @click="goBack"
            >
              <span class="material-symbols-outlined text-[16px]">arrow_back</span>
              Volver a Gestión de Usuarios
            </button>
          </div>

          <div class="text-on-primary z-10">
            <div class="flex items-center gap-2">
              <span class="material-symbols-outlined text-[20px] text-primary-fixed"
                >admin_panel_settings</span
              >
              <span class="text-xs uppercase tracking-wider text-primary-fixed font-semibold"
                >Módulo Administrador</span
              >
            </div>
            <h1 class="font-headline-lg text-headline-lg text-white font-bold leading-tight mt-1">
              Perfil de Tutor
            </h1>
            <p class="font-body-sm text-body-sm opacity-80 mt-0.5">
              Edición y actualización de información personal y académica del tutor
            </p>
          </div>
        </div>

        <div class="px-6 sm:px-8 pb-8 pt-16 relative flex flex-col gap-8">
          <div class="absolute -top-16 right-6 sm:right-12 flex flex-col items-center gap-2">
            <div class="relative group">
              <div
                class="w-28 h-28 sm:w-32 sm:h-32 rounded-full bg-surface-container flex items-center justify-center overflow-hidden shadow-md border-4 border-surface-container-lowest"
              >
                <img
                  v-if="formData.fotografiaUrl"
                  class="w-full h-full object-cover"
                  :src="formData.fotografiaUrl"
                  :alt="`${formData.nombre} ${formData.apellido}`"
                  @error="formData.fotografiaUrl = ''"
                />
                <span
                  v-else
                  class="font-headline-md text-headline-md text-on-surface-variant font-bold"
                >
                  {{ (formData.nombre[0] || 'T') + (formData.apellido[0] || 'U') }}
                </span>
              </div>
            </div>
            <span
              class="inline-flex items-center gap-1 px-2.5 py-0.5 rounded-full text-xs font-semibold bg-[#c3e6cb] text-[#155724] border border-[#a3d7b0]"
            >
              <span class="w-1.5 h-1.5 rounded-full bg-[#155724]"></span>
              Tutor Activo
            </span>
          </div>

          <form class="flex flex-col gap-6" @submit.prevent="handleSave">
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div class="flex flex-col gap-2">
                <label class="font-label-md text-label-md text-on-surface" for="nombres">
                  Nombres <span class="text-error">*</span>
                </label>
                <input
                  id="nombres"
                  :value="formData.nombre"
                  type="text"
                  maxlength="60"
                  placeholder="Ingrese los nombres"
                  :class="[
                    'w-full bg-surface-container-low text-on-surface font-body-md text-body-md px-4 py-3 rounded-lg focus:outline-none transition-all',
                    fieldErrors.nombre
                      ? 'border-2 border-error ring-1 ring-error/30 bg-error-container/10'
                      : 'border-none focus:ring-2 focus:ring-primary/50'
                  ]"
                  @input="handleNombreInput"
                />
                <span
                  v-if="fieldErrors.nombre"
                  class="text-xs text-error font-medium flex items-center gap-1 mt-0.5"
                >
                  <span class="material-symbols-outlined text-[14px]">error</span>
                  {{ fieldErrors.nombre }}
                </span>
              </div>

              <div class="flex flex-col gap-2">
                <label class="font-label-md text-label-md text-on-surface" for="apellidos">
                  Apellidos <span class="text-error">*</span>
                </label>
                <input
                  id="apellidos"
                  :value="formData.apellido"
                  type="text"
                  maxlength="60"
                  placeholder="Ingrese los apellidos"
                  :class="[
                    'w-full bg-surface-container-low text-on-surface font-body-md text-body-md px-4 py-3 rounded-lg focus:outline-none transition-all',
                    fieldErrors.apellido
                      ? 'border-2 border-error ring-1 ring-error/30 bg-error-container/10'
                      : 'border-none focus:ring-2 focus:ring-primary/50'
                  ]"
                  @input="handleApellidoInput"
                />
                <span
                  v-if="fieldErrors.apellido"
                  class="text-xs text-error font-medium flex items-center gap-1 mt-0.5"
                >
                  <span class="material-symbols-outlined text-[14px]">error</span>
                  {{ fieldErrors.apellido }}
                </span>
              </div>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div class="flex flex-col gap-2">
                <label class="font-label-md text-label-md text-on-surface" for="carnet">
                  Carnet / Identificador de Tutor <span class="text-error">*</span>
                </label>
                <input
                  id="carnet"
                  :value="formData.carnetId"
                  type="text"
                  maxlength="10"
                  placeholder="6 a 10 dígitos numéricos"
                  :class="[
                    'w-full bg-surface-container-low text-on-surface font-body-md text-body-md px-4 py-3 rounded-lg focus:outline-none transition-all font-mono',
                    fieldErrors.carnetId
                      ? 'border-2 border-error ring-1 ring-error/30 bg-error-container/10'
                      : 'border-none focus:ring-2 focus:ring-primary/50'
                  ]"
                  @input="handleCarnetInput"
                />
                <span
                  v-if="fieldErrors.carnetId"
                  class="text-xs text-error font-medium flex items-center gap-1 mt-0.5"
                >
                  <span class="material-symbols-outlined text-[14px]">error</span>
                  {{ fieldErrors.carnetId }}
                </span>
              </div>

              <div class="flex flex-col gap-2">
                <label
                  class="font-label-md text-label-md text-on-surface"
                  for="numeroIdentificacion"
                >
                  DPI / Documento de Identificación <span class="text-error">*</span>
                </label>
                <input
                  id="numeroIdentificacion"
                  :value="formData.numeroIdentificacion"
                  type="text"
                  maxlength="13"
                  placeholder="13 dígitos numéricos"
                  :class="[
                    'w-full bg-surface-container-low text-on-surface font-body-md text-body-md px-4 py-3 rounded-lg focus:outline-none transition-all font-mono',
                    fieldErrors.numeroIdentificacion
                      ? 'border-2 border-error ring-1 ring-error/30 bg-error-container/10'
                      : 'border-none focus:ring-2 focus:ring-primary/50'
                  ]"
                  @input="handleDpiInput"
                />
                <span
                  v-if="fieldErrors.numeroIdentificacion"
                  class="text-xs text-error font-medium flex items-center gap-1 mt-0.5"
                >
                  <span class="material-symbols-outlined text-[14px]">error</span>
                  {{ fieldErrors.numeroIdentificacion }}
                </span>
              </div>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div class="flex flex-col gap-2">
                <label class="font-label-md text-label-md text-on-surface" for="genero">
                  Género <span class="text-error">*</span>
                </label>
                <div class="relative">
                  <select
                    id="genero"
                    v-model="formData.genero"
                    :class="[
                      'w-full bg-surface-container-low text-on-surface font-body-md text-body-md px-4 py-3 rounded-lg focus:outline-none transition-all appearance-none cursor-pointer',
                      fieldErrors.genero
                        ? 'border-2 border-error ring-1 ring-error/30 bg-error-container/10'
                        : 'border-none focus:ring-2 focus:ring-primary/50'
                    ]"
                    @change="clearFieldError('genero')"
                  >
                    <option value="masculino">Masculino</option>
                    <option value="femenino">Femenino</option>
                  </select>
                  <span
                    class="material-symbols-outlined absolute right-4 top-1/2 -translate-y-1/2 pointer-events-none text-on-surface-variant"
                  >
                    expand_more
                  </span>
                </div>
                <span
                  v-if="fieldErrors.genero"
                  class="text-xs text-error font-medium flex items-center gap-1 mt-0.5"
                >
                  <span class="material-symbols-outlined text-[14px]">error</span>
                  {{ fieldErrors.genero }}
                </span>
              </div>

              <div class="flex flex-col gap-2">
                <label class="font-label-md text-label-md text-on-surface" for="fechaNacimiento">
                  Fecha de Nacimiento <span class="text-error">*</span>
                </label>
                <input
                  id="fechaNacimiento"
                  v-model="formData.fechaNacimiento"
                  type="date"
                  :min="minBirthDate"
                  :max="maxBirthDate"
                  :class="[
                    'w-full bg-surface-container-low text-on-surface font-body-md text-body-md px-4 py-3 rounded-lg focus:outline-none transition-all cursor-pointer',
                    fieldErrors.fechaNacimiento
                      ? 'border-2 border-error ring-1 ring-error/30 bg-error-container/10'
                      : 'border-none focus:ring-2 focus:ring-primary/50'
                  ]"
                  @change="clearFieldError('fechaNacimiento')"
                />
                <span
                  v-if="fieldErrors.fechaNacimiento"
                  class="text-xs text-error font-medium flex items-center gap-1 mt-0.5"
                >
                  <span class="material-symbols-outlined text-[14px]">error</span>
                  {{ fieldErrors.fechaNacimiento }}
                </span>
              </div>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div class="flex flex-col gap-2">
                <label class="font-label-md text-label-md text-on-surface" for="telefono">
                  Teléfono de Contacto
                </label>
                <input
                  id="telefono"
                  :value="formData.telefono"
                  type="text"
                  maxlength="8"
                  placeholder="8 dígitos numéricos"
                  :class="[
                    'w-full bg-surface-container-low text-on-surface font-body-md text-body-md px-4 py-3 rounded-lg focus:outline-none transition-all font-mono',
                    fieldErrors.telefono
                      ? 'border-2 border-error ring-1 ring-error/30 bg-error-container/10'
                      : 'border-none focus:ring-2 focus:ring-primary/50'
                  ]"
                  @input="handlePhoneInput"
                />
                <span
                  v-if="fieldErrors.telefono"
                  class="text-xs text-error font-medium flex items-center gap-1 mt-0.5"
                >
                  <span class="material-symbols-outlined text-[14px]">error</span>
                  {{ fieldErrors.telefono }}
                </span>
              </div>

              <div class="flex flex-col gap-2">
                <label class="font-label-md text-label-md text-on-surface" for="direccion">
                  Dirección de Residencia <span class="text-error">*</span>
                </label>
                <input
                  id="direccion"
                  v-model="formData.direccion"
                  type="text"
                  maxlength="200"
                  placeholder="Dirección domiciliar"
                  :class="[
                    'w-full bg-surface-container-low text-on-surface font-body-md text-body-md px-4 py-3 rounded-lg focus:outline-none transition-all',
                    fieldErrors.direccion
                      ? 'border-2 border-error ring-1 ring-error/30 bg-error-container/10'
                      : 'border-none focus:ring-2 focus:ring-primary/50'
                  ]"
                  @input="clearFieldError('direccion')"
                />
                <span
                  v-if="fieldErrors.direccion"
                  class="text-xs text-error font-medium flex items-center gap-1 mt-0.5"
                >
                  <span class="material-symbols-outlined text-[14px]">error</span>
                  {{ fieldErrors.direccion }}
                </span>
              </div>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div class="flex flex-col gap-2">
                <label class="font-label-md text-label-md text-on-surface" for="universidad">
                  Universidad de Graduación <span class="text-error">*</span>
                </label>
                <input
                  id="universidad"
                  v-model="formData.universidad"
                  type="text"
                  maxlength="100"
                  placeholder="Ej. USAC, URL, UVG"
                  :class="[
                    'w-full bg-surface-container-low text-on-surface font-body-md text-body-md px-4 py-3 rounded-lg focus:outline-none transition-all',
                    fieldErrors.universidad
                      ? 'border-2 border-error ring-1 ring-error/30 bg-error-container/10'
                      : 'border-none focus:ring-2 focus:ring-primary/50'
                  ]"
                  @input="clearFieldError('universidad')"
                />
                <span
                  v-if="fieldErrors.universidad"
                  class="text-xs text-error font-medium flex items-center gap-1 mt-0.5"
                >
                  <span class="material-symbols-outlined text-[14px]">error</span>
                  {{ fieldErrors.universidad }}
                </span>
              </div>

              <div class="flex flex-col gap-2">
                <label class="font-label-md text-label-md text-on-surface" for="anioInicio">
                  Año de Inicio de Tutorías <span class="text-error">*</span>
                </label>
                <input
                  id="anioInicio"
                  v-model.number="formData.anioInicio"
                  type="number"
                  min="1980"
                  :max="currentYear"
                  placeholder="Ej. 2020"
                  :class="[
                    'w-full bg-surface-container-low text-on-surface font-body-md text-body-md px-4 py-3 rounded-lg focus:outline-none transition-all',
                    fieldErrors.anioInicio
                      ? 'border-2 border-error ring-1 ring-error/30 bg-error-container/10'
                      : 'border-none focus:ring-2 focus:ring-primary/50'
                  ]"
                  @input="clearFieldError('anioInicio')"
                />
                <span
                  v-if="fieldErrors.anioInicio"
                  class="text-xs text-error font-medium flex items-center gap-1 mt-0.5"
                >
                  <span class="material-symbols-outlined text-[14px]">error</span>
                  {{ fieldErrors.anioInicio }}
                </span>
              </div>
            </div>

            <div class="flex flex-col gap-2">
              <label class="font-label-md text-label-md text-on-surface" for="direccionTutoria">
                Dirección de Tutoría / Modalidad <span class="text-error">*</span>
              </label>
              <input
                id="direccionTutoria"
                v-model="formData.direccionTutoria"
                type="text"
                maxlength="200"
                placeholder="Ej. Edificio T3 Salón 201 o Sesiones en Google Meet"
                :class="[
                  'w-full bg-surface-container-low text-on-surface font-body-md text-body-md px-4 py-3 rounded-lg focus:outline-none transition-all',
                  fieldErrors.direccionTutoria
                    ? 'border-2 border-error ring-1 ring-error/30 bg-error-container/10'
                    : 'border-none focus:ring-2 focus:ring-primary/50'
                ]"
                @input="clearFieldError('direccionTutoria')"
              />
              <span
                v-if="fieldErrors.direccionTutoria"
                class="text-xs text-error font-medium flex items-center gap-1 mt-0.5"
              >
                <span class="material-symbols-outlined text-[14px]">error</span>
                {{ fieldErrors.direccionTutoria }}
              </span>
            </div>

            <div class="flex flex-col gap-2">
              <label
                class="font-label-md text-label-md text-on-surface flex items-center gap-2"
                for="correo"
              >
                Correo Electrónico Institucional
                <span
                  class="material-symbols-outlined text-[16px] text-on-surface-variant cursor-help"
                  title="Por políticas de seguridad, el correo electrónico institucional permanece bloqueado contra edición."
                >
                  lock
                </span>
                <span class="text-xs text-on-surface-variant font-normal">(Solo lectura)</span>
              </label>
              <input
                id="correo"
                :value="formData.correo"
                type="email"
                disabled
                readonly
                class="w-full bg-surface-container-low text-on-surface-variant font-body-md text-body-md px-4 py-3 rounded-lg cursor-not-allowed border-none opacity-70"
              />
              <p class="font-body-sm text-body-sm text-on-surface-variant mt-0.5">
                Este correo se utiliza para autenticación y notificaciones oficiales del tutor.
                Permanece protegido contra modificaciones.
              </p>
            </div>

            <div class="flex flex-col gap-3 pt-2">
              <div class="flex items-center justify-between">
                <label class="font-label-md text-label-md text-on-surface flex items-center gap-2">
                  <span class="material-symbols-outlined text-primary text-[18px]">menu_book</span>
                  Materias Asignadas / Especialidad <span class="text-error">*</span>
                </label>
                <span class="text-xs text-on-surface-variant">
                  {{ formData.selectedMaterias.length }} seleccionada(s)
                </span>
              </div>

              <div
                :class="[
                  'p-4 bg-surface-container-low rounded-xl border flex flex-wrap gap-2 max-h-48 overflow-y-auto transition-all',
                  fieldErrors.materias
                    ? 'border-2 border-error bg-error-container/10'
                    : 'border-surface-container'
                ]"
              >
                <button
                  v-for="materia in availableMaterias"
                  :key="materia.id"
                  type="button"
                  :class="[
                    'px-3 py-1.5 rounded-lg text-xs font-medium transition-all flex items-center gap-1.5 cursor-pointer',
                    isMateriaSelected(materia.nombre)
                      ? 'bg-secondary text-white shadow-sm'
                      : 'bg-surface-container-lowest text-on-surface-variant hover:bg-surface-container-high border border-outline-variant/30'
                  ]"
                  @click="toggleMateria(materia.nombre)"
                >
                  <span class="material-symbols-outlined text-[14px]">
                    {{ isMateriaSelected(materia.nombre) ? 'check' : 'add' }}
                  </span>
                  {{ materia.nombre }}
                </button>

                <p
                  v-if="availableMaterias.length === 0"
                  class="text-xs text-on-surface-variant py-2"
                >
                  Cargando catálogo de materias...
                </p>
              </div>

              <span
                v-if="fieldErrors.materias"
                class="text-xs text-error font-medium flex items-center gap-1 mt-0.5"
              >
                <span class="material-symbols-outlined text-[14px]">error</span>
                {{ fieldErrors.materias }}
              </span>
            </div>

            <div
              class="flex flex-col sm:flex-row items-center justify-between pt-6 mt-4 border-t border-surface-container gap-4"
            >
              <button
                type="button"
                class="w-full sm:w-auto px-6 py-2.5 bg-surface-container-low text-on-surface-variant font-label-md text-label-md rounded-lg hover:bg-surface-container transition-colors flex items-center justify-center gap-2 font-semibold cursor-pointer"
                @click="goBack"
              >
                <span class="material-symbols-outlined text-[18px]">arrow_back</span>
                Volver
              </button>

              <div class="flex items-center gap-4 w-full sm:w-auto">
                <button
                  type="submit"
                  :disabled="isSaving"
                  :class="[
                    'w-full sm:w-auto px-8 py-2.5 rounded-lg shadow-sm font-label-md text-label-md font-semibold transition-all flex items-center justify-center gap-2 cursor-pointer',
                    !isSaving
                      ? 'bg-primary text-on-primary hover:bg-primary/90 active:scale-[0.98]'
                      : 'bg-outline-variant/40 text-on-surface-variant/60 cursor-not-allowed'
                  ]"
                >
                  <span v-if="isSaving" class="material-symbols-outlined animate-spin text-[18px]">
                    progress_activity
                  </span>
                  <span v-else class="material-symbols-outlined text-[18px]">save</span>
                  {{ isSaving ? 'Guardando...' : 'Guardar Cambios' }}
                </button>
              </div>
            </div>

            <div
              v-if="errorMessage"
              class="p-4 rounded-xl bg-error-container text-on-error-container border border-error/20 flex items-center justify-between shadow-sm animate-[fadeIn_0.3s_ease-out]"
            >
              <div class="flex items-center gap-3">
                <span class="material-symbols-outlined text-[22px]">error</span>
                <span class="font-label-md text-label-md">{{ errorMessage }}</span>
              </div>
              <button
                type="button"
                class="opacity-70 hover:opacity-100 p-1 cursor-pointer"
                aria-label="Cerrar notificación"
                @click="clearMessages"
              >
                <span class="material-symbols-outlined text-[18px]">close</span>
              </button>
            </div>

            <div
              v-if="successMessage"
              class="p-4 rounded-xl bg-[#c3e6cb] text-[#155724] border border-[#a3d7b0] flex items-center justify-between shadow-sm animate-[fadeIn_0.3s_ease-out]"
            >
              <div class="flex items-center gap-3">
                <span class="material-symbols-outlined text-[22px]">check_circle</span>
                <span class="font-label-md text-label-md">{{ successMessage }}</span>
              </div>
              <button
                type="button"
                class="opacity-70 hover:opacity-100 p-1 cursor-pointer"
                aria-label="Cerrar notificación"
                @click="clearMessages"
              >
                <span class="material-symbols-outlined text-[18px]">close</span>
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(4px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
