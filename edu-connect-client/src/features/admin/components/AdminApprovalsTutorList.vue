<script setup lang="ts">
import { ref } from 'vue'
import type { TutorApprovalItem } from '../types'
import { getEnv } from '@/config/env'

interface Props {
  tutors: TutorApprovalItem[]
  isLoading?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  isLoading: false
})

const emit = defineEmits<{
  (e: 'approve', tutor: TutorApprovalItem): void
  (e: 'reject', tutor: TutorApprovalItem): void
}>()

const failedImages = ref<Record<number, boolean>>({})

function handleImageError(id: number) {
  failedImages.value[id] = true
}

function isImageValid(id: number): boolean {
  return !failedImages.value[id]
}

function resolveImageUrl(url?: string): string {
  if (!url) return ''
  if (url.startsWith('http://') || url.startsWith('https://') || url.startsWith('data:')) {
    return url
  }
  const base = getEnv('VITE_API_URL', 'http://localhost:5000')
  return `${base.replace(/\/+$/, '')}${url}`
}

function getInitials(nombre: string, apellido: string): string {
  const f = nombre ? nombre.charAt(0) : ''
  const l = apellido ? apellido.charAt(0) : ''
  return `${f}${l}`.toUpperCase() || 'TU'
}

const avatarBgClasses = [
  'bg-primary-container text-on-primary-container',
  'bg-secondary-container text-on-secondary-container',
  'bg-tertiary-container text-on-tertiary-container'
]

function getAvatarBgClass(index: number): string {
  return avatarBgClasses[index % avatarBgClasses.length]
}
</script>

<template>
  <div class="w-full flex flex-col">
    <!-- ESTADO DE CARGA -->
    <div
      v-if="isLoading"
      class="bg-surface-container-lowest shadow-sm rounded-xl p-12 mb-margin-desktop text-center flex flex-col items-center justify-center min-h-[300px]"
    >
      <div class="w-10 h-10 border-4 border-primary/20 border-t-primary rounded-full animate-spin mb-4" />
      <p class="font-body-md text-body-md text-on-surface-variant">Cargando solicitudes de tutores...</p>
    </div>

    <!-- TABLA DE TUTORES PENDIENTES (HU-06) -->
    <div
      v-else-if="props.tutors.length > 0"
      class="bg-surface-container-lowest shadow-sm rounded-xl overflow-hidden mb-margin-desktop"
    >
      <div class="overflow-x-auto">
        <table class="w-full text-left font-body-md text-body-md text-on-surface table-auto">
          <thead class="bg-surface-container text-on-surface-variant font-label-md text-label-md">
            <tr class="text-on-surface-variant font-label-md text-label-md border-b border-surface-container-high">
              <th class="py-3 px-2 w-10 text-center font-semibold">Fotografía</th>
              <th class="py-3 px-3 font-semibold">Nombre Completo</th>
              <th class="py-3 px-2.5 w-32 font-semibold">Carnet</th>
              <th class="py-3 px-2.5 w-32 font-semibold">ID Tutor</th>
              <th class="py-3 px-2 w-24 font-semibold">Género</th>
              <th class="py-3 px-3 font-semibold">Especialidad</th>
              <th class="py-3 px-2.5 w-36 font-semibold">Correo</th>
              <th class="py-3 px-3 w-44 font-semibold text-center">Acciones</th>
            </tr>
          </thead>
          <tbody class="divide-y-0 text-on-surface">
            <tr
              v-for="(tutor, index) in props.tutors"
              :key="tutor.id"
              class="hover:bg-surface-container-low transition-colors group border-b border-surface-container/40 last:border-0"
            >
              <!-- Fotografía (ancho 10 / compacto) -->
              <td class="py-3 px-2 w-10 text-center">
                <div class="w-8 h-8 rounded-full overflow-hidden bg-surface-container-highest flex-shrink-0 mx-auto shadow-xs border border-primary/10">
                  <img
                    v-if="tutor.fotografiaUrl && isImageValid(tutor.id)"
                    :src="resolveImageUrl(tutor.fotografiaUrl)"
                    :alt="`${tutor.nombre} ${tutor.apellido}`"
                    class="w-full h-full object-cover"
                    @error="handleImageError(tutor.id)"
                  />
                  <div
                    v-else
                    :class="['w-full h-full flex items-center justify-center font-label-sm text-[11px] font-bold', getAvatarBgClass(index)]"
                  >
                    {{ getInitials(tutor.nombre, tutor.apellido) }}
                  </div>
                </div>
              </td>

              <!-- Nombre Completo (solo el nombre) -->
              <td class="py-3 px-3">
                <p class="font-label-md text-label-md text-on-surface whitespace-nowrap">
                  {{ tutor.nombre }} {{ tutor.apellido }}
                </p>
              </td>

              <!-- Carnet (columna individual) -->
              <td class="py-3 px-2.5 w-32">
                <span class="font-mono text-xs bg-surface-container-highest text-on-surface px-2 py-0.5 rounded font-medium inline-block">
                  {{ tutor.carnetId }}
                </span>
              </td>

              <!-- ID Tutor único -->
              <td class="py-3 px-2.5 w-32">
                <span class="font-mono text-xs bg-secondary-container text-on-secondary-container px-2 py-0.5 rounded font-semibold inline-block">
                  {{ tutor.numeroIdentificacion }}
                </span>
              </td>

              <!-- Género (columna individual) -->
              <td class="py-3 px-2 w-24 text-sm capitalize text-on-surface-variant whitespace-nowrap">
                {{ tutor.genero }}
              </td>

              <!-- Especialidad (esquinas menos redondas: rounded-md) -->
              <td class="py-3 px-3 max-w-[210px]">
                <div class="flex flex-wrap gap-1">
                  <span
                    v-for="materia in (tutor.materias || []).slice(0, 2)"
                    :key="materia"
                    class="inline-block bg-primary/10 text-primary text-[11px] px-2 py-0.5 rounded-md font-medium"
                  >
                    {{ materia }}
                  </span>
                  <span
                    v-if="(tutor.materias || []).length > 2"
                    class="inline-block bg-surface-container-highest text-on-surface-variant text-[10px] px-1.5 py-0.5 rounded-md font-medium"
                  >
                    +{{ tutor.materias.length - 2 }}
                  </span>
                </div>
                <p v-if="tutor.especialidad && (!tutor.materias || tutor.materias.length === 0)" class="text-xs text-on-surface-variant line-clamp-1 mt-0.5">
                  {{ tutor.especialidad }}
                </p>
              </td>

              <!-- Correo (angosto con tooltip) -->
              <td class="py-3 px-2.5 w-36 text-sm">
                <a
                  :href="`mailto:${tutor.correo}`"
                  :title="tutor.correo"
                  class="text-on-surface-variant hover:text-primary transition-colors truncate block max-w-[140px]"
                >
                  {{ tutor.correo }}
                </a>
              </td>

              <!-- Acciones: Aceptar / Rechazar -->
              <td class="py-3 px-3 w-44 text-center">
                <div class="flex gap-1.5 justify-center">
                  <button
                    type="button"
                    title="Aceptar solicitud de tutor"
                    class="inline-flex items-center gap-1 px-2.5 py-1.5 rounded-lg bg-[#c3e6cb] text-[#155724] hover:bg-[#218838] hover:text-white transition-all font-label-sm text-label-sm cursor-pointer shadow-xs whitespace-nowrap"
                    @click="emit('approve', tutor)"
                  >
                    <span class="material-symbols-outlined text-[16px]">check_circle</span>
                    <span>Aceptar</span>
                  </button>

                  <button
                    type="button"
                    title="Rechazar solicitud de tutor"
                    class="inline-flex items-center gap-1 px-2.5 py-1.5 rounded-lg bg-error-container text-on-error-container hover:bg-error hover:text-white transition-all font-label-sm text-label-sm cursor-pointer shadow-xs whitespace-nowrap"
                    @click="emit('reject', tutor)"
                  >
                    <span class="material-symbols-outlined text-[16px]">cancel</span>
                    <span>Rechazar</span>
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Barra Inferior de Registros / Paginación -->
      <div class="p-3.5 bg-surface-container-lowest border-t border-surface-container-highest flex items-center justify-between">
        <span class="font-label-sm text-label-sm text-on-surface-variant">
          Mostrando {{ props.tutors.length }} {{ props.tutors.length === 1 ? 'registro' : 'registros' }}
        </span>

        <div class="flex gap-1">
          <button
            type="button"
            disabled
            class="w-7 h-7 flex items-center justify-center rounded-md text-on-surface-variant hover:bg-surface-container-low disabled:opacity-50"
          >
            <span class="material-symbols-outlined text-[18px]">chevron_left</span>
          </button>
          <button
            type="button"
            class="w-7 h-7 flex items-center justify-center rounded-md bg-primary text-on-primary font-label-sm text-xs"
          >
            1
          </button>
          <button
            type="button"
            disabled
            class="w-7 h-7 flex items-center justify-center rounded-md text-on-surface-variant hover:bg-surface-container-low disabled:opacity-50"
          >
            <span class="material-symbols-outlined text-[18px]">chevron_right</span>
          </button>
        </div>
      </div>
    </div>

    <!-- ESTADO VACÍO (TODO AL DÍA) -->
    <div
      v-else
      class="bg-surface-container-lowest shadow-sm rounded-xl p-8 mb-margin-desktop text-center flex flex-col items-center justify-center min-h-[300px]"
    >
      <div class="w-16 h-16 bg-surface-container-highest rounded-full flex items-center justify-center mb-4">
        <span class="material-symbols-outlined text-[32px] text-on-surface-variant">inbox</span>
      </div>
      <h3 class="font-headline-md text-headline-md text-on-surface mb-2">Todo al día</h3>
      <p class="font-body-md text-body-md text-on-surface-variant max-w-md">
        No hay solicitudes de tutores pendientes de aprobación en este momento. Vuelve más tarde.
      </p>
    </div>
  </div>
</template>
