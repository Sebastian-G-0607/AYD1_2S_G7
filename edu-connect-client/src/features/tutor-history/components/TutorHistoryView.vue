<script setup lang="ts">
import { BaseAlert, BaseBadge, BaseButton, BaseInput } from '@/components/ui'
import { useTutorHistory } from '../composables/useTutorHistory'

const {
  sessions,
  fecha,
  estudiante,
  isInitialLoading,
  isFiltering,
  hasActiveFilters,
  activeFiltersCount,
  errorMessage,
  fetchHistory,
  clearFecha,
  clearEstudiante,
  clearFilters
} = useTutorHistory()

function formatDate(date: string) {
  if (!date) return '-'

  const [year, month, day] = date.split('-')

  if (!year || !month || !day) {
    return date
  }

  return `${day}/${month}/${year}`
}

function formatTime(time: string) {
  if (!time) return '-'
  return time.slice(0, 5)
}

function formatStatus(status: string) {
  const normalized = status?.toUpperCase().trim()

  switch (normalized) {
    case 'PENDIENTE':
      return 'Pendiente'
    case 'ATENDIDA':
      return 'Atendida'
    case 'CANCELADA_TUTOR':
      return 'Cancelada por el tutor'
    case 'CANCELADA_ESTUDIANTE':
      return 'Cancelada por el estudiante'
    case 'CANCELADA':
      return 'Cancelada'
    default:
      return status || '-'
  }
}

function getStatusVariant(
  status: string
): 'primary' | 'secondary' | 'neutral' | 'success' | 'error' {
  const normalized = status?.toUpperCase().trim()

  switch (normalized) {
    case 'ATENDIDA':
      return 'success'
    case 'PENDIENTE':
      return 'secondary'
    case 'CANCELADA_TUTOR':
    case 'CANCELADA_ESTUDIANTE':
    case 'CANCELADA':
      return 'error'
    default:
      return 'neutral'
  }
}
</script>

<template>
  <div class="flex flex-col w-full">
    <div class="mb-8">
      <h1 class="text-3xl font-bold font-headline text-on-surface tracking-tight mb-2">
        Historial de Sesiones
      </h1>

      <p class="text-base text-on-surface-variant font-body">
        Consulta las tutorías atendidas y canceladas de tu historial.
      </p>
    </div>

    <div
      class="bg-surface-container-lowest rounded-2xl border border-outline-variant/20 shadow-sm p-6 mb-6 transition-all"
    >
      <div class="flex items-center justify-between gap-3 mb-5">
        <div class="flex items-center gap-2">
          <span class="material-symbols-outlined text-on-surface-variant text-[22px]">
            filter_list
          </span>

          <h2 class="text-lg font-bold font-headline text-on-surface">Filtros</h2>

          <span
            v-if="hasActiveFilters"
            class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-semibold bg-primary/10 text-primary border border-primary/20"
          >
            {{ activeFiltersCount }}
            {{ activeFiltersCount === 1 ? 'filtro activo' : 'filtros activos' }}
          </span>

          <span
            v-if="isFiltering"
            class="material-symbols-outlined text-primary text-[18px] animate-spin ml-1"
          >
            progress_activity
          </span>
        </div>

        <button
          v-if="hasActiveFilters"
          type="button"
          class="inline-flex items-center gap-1.5 px-3 py-1.5 rounded-xl text-xs font-semibold bg-surface-container-high/60 hover:bg-error/10 text-on-surface-variant hover:text-error transition-all duration-200 group active:scale-95 cursor-pointer border border-outline-variant/30"
          title="Restablecer todos los filtros"
          @click="clearFilters"
        >
          <span
            class="material-symbols-outlined text-[16px] group-hover:rotate-180 transition-transform duration-500"
          >
            restart_alt
          </span>
          <span>Limpiar filtros</span>
        </button>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <BaseInput id="fecha" v-model="fecha" type="date" label="Fecha" icon="calendar_today">
          <template v-if="fecha" #labelAction>
            <button
              type="button"
              class="text-xs text-on-surface-variant hover:text-error transition-colors flex items-center gap-0.5 cursor-pointer"
              @click="clearFecha"
            >
              <span class="material-symbols-outlined text-[14px]">close</span>
              <span>Borrar fecha</span>
            </button>
          </template>
        </BaseInput>

        <BaseInput
          id="estudiante"
          v-model="estudiante"
          type="text"
          label="Estudiante o Correo"
          placeholder="Buscar por nombre, apellido o correo..."
          icon="search"
          @enter="fetchHistory(true)"
        >
          <template v-if="estudiante" #labelAction>
            <button
              type="button"
              class="text-xs text-on-surface-variant hover:text-error transition-colors flex items-center gap-0.5 cursor-pointer"
              @click="clearEstudiante"
            >
              <span class="material-symbols-outlined text-[14px]">close</span>
              <span>Borrar texto</span>
            </button>
          </template>
        </BaseInput>
      </div>

      <div
        v-if="hasActiveFilters"
        class="flex flex-wrap items-center gap-2 pt-4 mt-4 border-t border-outline-variant/15"
      >
        <span class="text-xs font-medium text-on-surface-variant">Filtros aplicados:</span>

        <span
          v-if="fecha"
          class="inline-flex items-center gap-1.5 pl-2.5 pr-1.5 py-1 rounded-full text-xs font-medium bg-surface-container-low text-on-surface border border-outline-variant/30"
        >
          <span class="material-symbols-outlined text-[14px] text-primary">calendar_today</span>
          <span>Fecha: {{ formatDate(fecha) }}</span>
          <button
            type="button"
            class="w-4 h-4 rounded-full flex items-center justify-center hover:bg-surface-container-high text-on-surface-variant hover:text-error transition-colors cursor-pointer"
            title="Quitar filtro de fecha"
            @click="clearFecha"
          >
            <span class="material-symbols-outlined text-[13px]">close</span>
          </button>
        </span>

        <span
          v-if="estudiante"
          class="inline-flex items-center gap-1.5 pl-2.5 pr-1.5 py-1 rounded-full text-xs font-medium bg-surface-container-low text-on-surface border border-outline-variant/30"
        >
          <span class="material-symbols-outlined text-[14px] text-primary">person</span>
          <span>"{{ estudiante }}"</span>
          <button
            type="button"
            class="w-4 h-4 rounded-full flex items-center justify-center hover:bg-surface-container-high text-on-surface-variant hover:text-error transition-colors cursor-pointer"
            title="Quitar filtro de estudiante"
            @click="clearEstudiante"
          >
            <span class="material-symbols-outlined text-[13px]">close</span>
          </button>
        </span>
      </div>
    </div>

    <BaseAlert v-if="errorMessage" type="error" :message="errorMessage" class="mb-6" />

    <div v-if="isInitialLoading" class="flex items-center justify-center py-16">
      <div class="flex flex-col items-center gap-3">
        <span class="material-symbols-outlined text-4xl text-primary animate-spin">
          progress_activity
        </span>

        <p class="text-on-surface-variant">Cargando historial...</p>
      </div>
    </div>

    <div
      v-else-if="sessions.length > 0"
      class="bg-surface-container-lowest rounded-2xl border border-outline-variant/20 shadow-sm overflow-hidden"
    >
      <div v-if="isFiltering" class="h-0.5 w-full bg-surface-container overflow-hidden relative">
        <div class="h-full bg-primary animate-pulse w-full"></div>
      </div>

      <div class="overflow-x-auto" :class="{ 'opacity-60 transition-opacity': isFiltering }">
        <table class="w-full text-left">
          <thead class="bg-surface-container-low">
            <tr>
              <th class="px-6 py-4 text-sm font-semibold text-on-surface">Fecha</th>

              <th class="px-6 py-4 text-sm font-semibold text-on-surface">Hora</th>

              <th class="px-6 py-4 text-sm font-semibold text-on-surface">Estudiante</th>

              <th class="px-6 py-4 text-sm font-semibold text-on-surface">Correo</th>

              <th class="px-6 py-4 text-sm font-semibold text-on-surface">Estado</th>
            </tr>
          </thead>

          <tbody>
            <tr
              v-for="session in sessions"
              :key="session.sesionId"
              class="border-t border-outline-variant/20 hover:bg-surface-container-low/50 transition-colors"
            >
              <td class="px-6 py-5 text-sm text-on-surface whitespace-nowrap">
                {{ formatDate(session.fechaSesion) }}
              </td>

              <td class="px-6 py-5 text-sm text-on-surface whitespace-nowrap">
                {{ formatTime(session.horaInicio) }}
              </td>

              <td class="px-6 py-5">
                <div class="flex items-center gap-3">
                  <div
                    class="w-9 h-9 rounded-full bg-primary/10 flex items-center justify-center overflow-hidden flex-shrink-0"
                  >
                    <img
                      v-if="session.estudianteAvatarUrl"
                      :src="session.estudianteAvatarUrl"
                      :alt="session.estudiante"
                      class="w-full h-full object-cover"
                    />
                    <span v-else class="material-symbols-outlined text-primary text-[20px]">
                      person
                    </span>
                  </div>

                  <span class="text-sm font-medium text-on-surface whitespace-nowrap">
                    {{ session.estudiante }}
                  </span>
                </div>
              </td>

              <td class="px-6 py-5 text-sm text-on-surface-variant">
                {{ session.estudianteEmail || '-' }}
              </td>

              <td class="px-6 py-5 whitespace-nowrap">
                <BaseBadge :variant="getStatusVariant(session.estado)" size="sm">
                  {{ formatStatus(session.estado) }}
                </BaseBadge>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <div
      v-else
      class="flex flex-col items-center justify-center py-16 px-4 text-center bg-surface-container-lowest rounded-2xl border border-outline-variant/20 shadow-sm"
    >
      <div
        class="w-16 h-16 rounded-full bg-surface-container-high flex items-center justify-center text-on-surface-variant mb-4"
      >
        <span class="material-symbols-outlined text-[32px]"> history </span>
      </div>

      <h3 class="text-xl font-bold font-headline text-on-surface mb-2">
        No se encontraron sesiones
      </h3>

      <p class="text-sm text-on-surface-variant max-w-md font-body">
        {{
          hasActiveFilters
            ? 'No existen sesiones en el historial que coincidan con los filtros seleccionados.'
            : 'Aún no tienes sesiones registradas en tu historial.'
        }}
      </p>

      <BaseButton
        v-if="hasActiveFilters"
        variant="secondary"
        size="sm"
        class="mt-5"
        @click="clearFilters"
      >
        <template #iconLeft>
          <span class="material-symbols-outlined text-[18px]">restart_alt</span>
        </template>
        Limpiar filtros
      </BaseButton>
    </div>
  </div>
</template>
