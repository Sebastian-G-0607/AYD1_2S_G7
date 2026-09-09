<script setup lang="ts">
import { BaseAlert, BaseBadge, BaseButton } from '@/components/ui'
import { useTutorHistory } from '../composables/useTutorHistory'

const { sessions, fecha, estudiante, isLoading, errorMessage, fetchHistory, clearFilters } =
  useTutorHistory()

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
      class="bg-surface-container-lowest rounded-2xl border border-outline-variant/20 shadow-sm p-6 mb-6"
    >
      <div class="flex items-center gap-2 mb-5">
        <span class="material-symbols-outlined text-on-surface-variant"> filter_list </span>

        <h2 class="text-lg font-bold font-headline text-on-surface">Filtros</h2>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div class="flex flex-col gap-2">
          <label for="fecha" class="text-sm font-medium text-on-surface font-body"> Fecha </label>

          <input
            id="fecha"
            v-model="fecha"
            type="date"
            class="w-full rounded-xl border border-outline-variant bg-surface px-4 py-3 text-on-surface outline-none focus:ring-2 focus:ring-primary"
          />
        </div>

        <div class="flex flex-col gap-2">
          <label for="estudiante" class="text-sm font-medium text-on-surface font-body">
            Estudiante
          </label>

          <input
            id="estudiante"
            v-model="estudiante"
            type="text"
            placeholder="Buscar por nombre o apellido"
            class="w-full rounded-xl border border-outline-variant bg-surface px-4 py-3 text-on-surface outline-none focus:ring-2 focus:ring-primary"
            @keyup.enter="fetchHistory"
          />
        </div>
      </div>

      <div class="flex flex-wrap gap-3 mt-5">
        <BaseButton variant="primary" size="md" :disabled="isLoading" @click="fetchHistory">
          <template #iconLeft>
            <span class="material-symbols-outlined text-[20px]"> search </span>
          </template>

          Buscar
        </BaseButton>

        <BaseButton variant="secondary" size="md" :disabled="isLoading" @click="clearFilters">
          Limpiar filtros
        </BaseButton>
      </div>
    </div>

    <BaseAlert v-if="errorMessage" type="error" :message="errorMessage" class="mb-6" />

    <div v-if="isLoading" class="flex items-center justify-center py-16">
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
      <div class="overflow-x-auto">
        <table class="w-full text-left">
          <thead class="bg-surface-container-low">
            <tr>
              <th class="px-6 py-4 text-sm font-semibold text-on-surface">Fecha</th>

              <th class="px-6 py-4 text-sm font-semibold text-on-surface">Hora</th>

              <th class="px-6 py-4 text-sm font-semibold text-on-surface">Estudiante</th>

              <th class="px-6 py-4 text-sm font-semibold text-on-surface">Estado</th>
            </tr>
          </thead>

          <tbody>
            <tr
              v-for="session in sessions"
              :key="session.sesionId"
              class="border-t border-outline-variant/20 hover:bg-surface-container-low/50 transition-colors"
            >
              <td class="px-6 py-5 text-sm text-on-surface">
                {{ formatDate(session.fechaSesion) }}
              </td>

              <td class="px-6 py-5 text-sm text-on-surface">
                {{ formatTime(session.horaInicio) }}
              </td>

              <td class="px-6 py-5">
                <div class="flex items-center gap-3">
                  <div class="w-9 h-9 rounded-full bg-primary/10 flex items-center justify-center">
                    <span class="material-symbols-outlined text-primary text-[20px]"> person </span>
                  </div>

                  <span class="text-sm font-medium text-on-surface">
                    {{ session.estudiante }}
                  </span>
                </div>
              </td>

              <td class="px-6 py-5">
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
        No existen sesiones en el historial que coincidan con los filtros seleccionados.
      </p>
    </div>
  </div>
</template>
