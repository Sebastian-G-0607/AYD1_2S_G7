<script setup lang="ts">
import type { TutorSession } from '../types'

interface Props {
  sessions: TutorSession[]
}

interface Emits {
  (e: 'complete', session: TutorSession): void
  (e: 'cancel', session: TutorSession): void
}

defineProps<Props>()
const emit = defineEmits<Emits>()
</script>

<template>
  <div
    class="rounded-2xl overflow-hidden bg-surface-container-lowest border border-outline-variant/20 shadow-sm"
  >
    <div class="overflow-x-auto">
      <table class="w-full text-left border-collapse text-sm">
        <thead>
          <tr
            class="bg-surface-container-low text-on-surface-variant font-semibold border-b border-surface-container-high"
          >
            <th class="py-4 px-6">Fecha y Hora</th>
            <th class="py-4 px-6">Estudiante</th>
            <th class="py-4 px-6">Materia</th>
            <th class="py-4 px-6">Motivo</th>
            <th class="py-4 px-6 text-right">Acciones</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-surface-container text-on-surface">
          <tr
            v-for="session in sessions"
            :key="session.id"
            class="hover:bg-surface-container-low/60 transition-colors"
          >
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="flex flex-col">
                <span class="font-semibold text-on-surface">{{ session.fecha }}</span>
                <span class="text-xs text-on-surface-variant">{{ session.hora }}</span>
              </div>
            </td>

            <td class="px-6 py-4">
              <div class="flex items-center gap-3">
                <div
                  class="w-10 h-10 rounded-full bg-secondary-container text-on-secondary-container flex items-center justify-center font-bold text-xs shadow-xs overflow-hidden flex-shrink-0"
                >
                  <img
                    v-if="session.estudianteAvatarUrl"
                    :src="session.estudianteAvatarUrl"
                    :alt="session.estudianteNombre"
                    class="w-full h-full object-cover"
                  />
                  <span v-else>{{ session.estudianteNombre.charAt(0) }}</span>
                </div>
                <div class="flex flex-col">
                  <span class="font-semibold text-on-surface">{{ session.estudianteNombre }}</span>
                  <span class="text-xs text-on-surface-variant"
                    >ID: {{ session.estudianteId }}</span
                  >
                </div>
              </div>
            </td>

            <td class="px-6 py-4">
              <span
                class="inline-flex items-center px-3 py-1 rounded-full bg-primary text-on-primary text-xs font-semibold"
              >
                {{ session.materia }}
              </span>
            </td>

            <td class="px-6 py-4 max-w-xs truncate text-on-surface-variant" :title="session.motivo">
              {{ session.motivo }}
            </td>

            <td class="px-6 py-4 text-right">
              <div class="flex items-center justify-end gap-2">
                <button
                  type="button"
                  class="px-3.5 py-1.5 rounded-full bg-[#16a34a]/10 text-[#16a34a] hover:bg-[#16a34a] hover:text-white text-xs font-semibold transition-all flex items-center gap-1.5 shadow-xs"
                  @click="emit('complete', session)"
                >
                  <span class="material-symbols-outlined text-[18px]">check_circle</span>
                  <span>Atendido</span>
                </button>

                <button
                  type="button"
                  class="p-1.5 rounded-full text-on-surface-variant hover:text-error hover:bg-error-container/40 transition-colors"
                  title="Cancelar sesión"
                  @click="emit('cancel', session)"
                >
                  <span class="material-symbols-outlined text-[18px]">cancel</span>
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
