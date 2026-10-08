<script setup lang="ts">
import { BaseAlert } from '@/components/ui'
import { useAdminTutorRatings } from '../composables/useAdminTutorRatings'

const { items, sortedItems, totalRated, isLoading, error, searchQuery, sortDirection, toggleSort } =
  useAdminTutorRatings()

function formatAverage(value: number | null) {
  return value === null ? 'Sin calificar' : value.toFixed(2)
}

function starState(value: number | null, position: number): 'full' | 'half' | 'empty' {
  if (value === null) return 'empty'
  if (value >= position) return 'full'
  if (value >= position - 0.5) return 'half'
  return 'empty'
}

function starIcon(state: 'full' | 'half' | 'empty') {
  return state === 'half' ? 'star_half' : 'star'
}
</script>

<template>
  <div class="flex flex-col w-full gap-8">
    <div>
      <h1 class="text-3xl font-bold font-headline text-primary-container tracking-tight">
        Calificación de Tutores
      </h1>
      <p class="text-base text-on-surface-variant mt-2">
        Promedio de estrellas (0 a 5) que los estudiantes asignaron a cada tutor activo.
      </p>
    </div>

    <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
      <div
        class="bg-surface-container-lowest rounded-xl p-6 shadow-sm border border-outline-variant/20"
      >
        <p class="text-xs font-semibold text-on-surface-variant uppercase tracking-wider">
          Tutores activos
        </p>
        <p class="text-4xl font-bold font-headline text-primary mt-2">{{ items.length }}</p>
      </div>

      <div
        class="bg-surface-container-lowest rounded-xl p-6 shadow-sm border border-outline-variant/20"
      >
        <p class="text-xs font-semibold text-on-surface-variant uppercase tracking-wider">
          Con calificaciones
        </p>
        <p class="text-4xl font-bold font-headline text-secondary mt-2">{{ totalRated }}</p>
      </div>
    </div>

    <BaseAlert v-if="error" type="error" :message="error" />

    <div
      class="bg-surface-container-lowest rounded-xl shadow-sm border border-outline-variant/20 overflow-hidden"
    >
      <div class="p-6 border-b border-surface-container-low">
        <div class="relative w-full max-w-md">
          <span
            class="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-on-surface-variant"
          >
            search
          </span>
          <input
            v-model="searchQuery"
            type="text"
            aria-label="Buscar tutor"
            placeholder="Buscar por nombre o especialidad..."
            class="w-full bg-background rounded-lg pl-10 pr-4 py-2.5 text-sm text-on-surface placeholder:text-on-surface-variant outline-none focus:ring-2 focus:ring-primary/20 transition-all"
          />
        </div>
      </div>

      <div v-if="isLoading" class="flex flex-col items-center justify-center py-16 gap-3">
        <span class="material-symbols-outlined text-4xl text-primary animate-spin">
          progress_activity
        </span>
        <p class="text-sm text-on-surface-variant">Cargando calificaciones...</p>
      </div>

      <div v-else-if="sortedItems.length" class="overflow-x-auto">
        <table class="w-full text-left border-collapse">
          <thead>
            <tr class="bg-surface-container-low/50">
              <th class="py-4 px-6 text-sm font-semibold text-on-surface-variant">Tutor</th>
              <th class="py-4 px-6 text-sm font-semibold text-on-surface-variant">Especialidad</th>
              <th class="py-4 px-6 text-sm font-semibold text-on-surface-variant">
                <button
                  type="button"
                  class="inline-flex items-center gap-1 hover:text-primary transition-colors"
                  :aria-label="`Ordenar por promedio ${sortDirection === 'desc' ? 'ascendente' : 'descendente'}`"
                  @click="toggleSort"
                >
                  Promedio
                  <span class="material-symbols-outlined text-[18px]">
                    {{ sortDirection === 'desc' ? 'arrow_downward' : 'arrow_upward' }}
                  </span>
                </button>
              </th>
              <th class="py-4 px-6 text-sm font-semibold text-on-surface-variant text-right">
                Calificaciones
              </th>
            </tr>
          </thead>

          <tbody class="divide-y divide-surface-container-low">
            <tr
              v-for="item in sortedItems"
              :key="item.tutorId"
              class="hover:bg-primary/5 transition-colors"
            >
              <td class="py-5 px-6 text-sm font-medium text-on-surface">
                {{ item.nombreCompleto }}
              </td>

              <td class="py-5 px-6 text-sm text-on-surface-variant max-w-[260px]">
                <span class="block truncate" :title="item.especialidad">
                  {{ item.especialidad }}
                </span>
              </td>

              <td class="py-5 px-6">
                <div class="flex items-center gap-3">
                  <div class="flex" aria-hidden="true">
                    <span
                      v-for="position in 5"
                      :key="position"
                      :class="[
                        'material-symbols-outlined text-[20px]',
                        starState(item.promedioCalificacion, position) === 'empty'
                          ? 'text-outline-variant'
                          : 'text-amber-500'
                      ]"
                    >
                      {{ starIcon(starState(item.promedioCalificacion, position)) }}
                    </span>
                  </div>
                  <span
                    :class="[
                      'text-sm font-semibold',
                      item.promedioCalificacion === null
                        ? 'text-on-surface-variant'
                        : 'text-on-surface'
                    ]"
                  >
                    {{ formatAverage(item.promedioCalificacion) }}
                  </span>
                </div>
              </td>

              <td class="py-5 px-6 text-sm text-on-surface-variant text-right">
                {{ item.totalCalificaciones }}
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <div v-else class="flex flex-col items-center justify-center py-16 px-4 text-center">
        <div
          class="w-16 h-16 rounded-full bg-surface-container-high flex items-center justify-center text-on-surface-variant mb-4"
        >
          <span class="material-symbols-outlined text-[32px]">star</span>
        </div>
        <h3 class="text-xl font-bold font-headline text-on-surface mb-2">
          No se encontraron tutores
        </h3>
        <p class="text-sm text-on-surface-variant max-w-md">
          No hay tutores activos que coincidan con la búsqueda.
        </p>
      </div>
    </div>
  </div>
</template>
