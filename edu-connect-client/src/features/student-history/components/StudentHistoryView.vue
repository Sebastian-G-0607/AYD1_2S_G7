<script setup lang="ts">
import { ref } from 'vue'
import { BaseAlert, BaseBadge, BaseModal } from '@/components/ui'
import { useStudentHistory } from '../composables/useStudentHistory'
import type { StudentHistorySession } from '../types'

const {
  filteredSessions,
  search,
  statusFilter,
  totalSessions,
  attendedSessions,
  cancelledSessions,
  isLoading,
  errorMessage
} = useStudentHistory()

const selectedSession = ref<StudentHistorySession | null>(null)
const isSummaryOpen = ref(false)

function formatDate(date: string) {
  if (!date) return '-'

  const [year, month, day] = date.split('-')

  if (!year || !month || !day) return date

  return `${day}/${month}/${year}`
}

function formatStatus(status: string) {
  switch (status?.toUpperCase().trim()) {
    case 'ATENDIDA':
      return 'Atendida'
    case 'CANCELADA_ESTUDIANTE':
      return 'Cancelada por ti'
    case 'CANCELADA_TUTOR':
      return 'Cancelada por tutor'
    case 'CANCELADA':
      return 'Cancelada'
    default:
      return status || '-'
  }
}

function getStatusVariant(
  status: string
): 'primary' | 'secondary' | 'neutral' | 'success' | 'error' {
  return status?.toUpperCase() === 'ATENDIDA' ? 'success' : 'error'
}

function openSummary(session: StudentHistorySession) {
  if (session.estado.toUpperCase() !== 'ATENDIDA') return

  selectedSession.value = session
  isSummaryOpen.value = true
}
</script>

<template>
  <div class="flex flex-col w-full gap-8">
    <!-- Encabezado -->
    <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
      <div>
        <h1 class="text-3xl font-bold font-headline text-primary-container tracking-tight">
          Historial de Sesiones
        </h1>

        <p class="text-base text-on-surface-variant mt-2">
          Revisa el registro completo de tus tutorías pasadas y su estado.
        </p>
      </div>

      <select
        v-model="statusFilter"
        class="bg-surface-container-low border border-outline-variant/40 rounded-lg px-4 py-2.5 text-sm text-on-surface outline-none focus:ring-2 focus:ring-primary/20"
      >
        <option value="TODAS">Todos los estados</option>
        <option value="ATENDIDA">Atendidas</option>
        <option value="CANCELADA_ESTUDIANTE">Canceladas por ti</option>
        <option value="CANCELADA_TUTOR">Canceladas por tutor</option>
      </select>
    </div>

    <!-- Tarjetas resumen -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <div
        class="bg-surface-container-lowest rounded-xl p-6 shadow-sm border border-outline-variant/20 flex items-center justify-between relative overflow-hidden"
      >
        <div>
          <p class="text-xs font-semibold text-on-surface-variant uppercase tracking-wider">
            Total sesiones
          </p>

          <p class="text-4xl font-bold font-headline text-primary mt-2">
            {{ totalSessions }}
          </p>
        </div>

        <div
          class="w-12 h-12 rounded-full bg-primary-fixed flex items-center justify-center text-on-primary-fixed"
        >
          <span class="material-symbols-outlined">library_books</span>
        </div>
      </div>

      <div
        class="bg-surface-container-lowest rounded-xl p-6 shadow-sm border border-outline-variant/20 flex items-center justify-between"
      >
        <div>
          <p class="text-xs font-semibold text-on-surface-variant uppercase tracking-wider">
            Atendidas
          </p>

          <p class="text-4xl font-bold font-headline text-secondary mt-2">
            {{ attendedSessions }}
          </p>
        </div>

        <div
          class="w-12 h-12 rounded-full bg-secondary-fixed flex items-center justify-center text-on-secondary-fixed"
        >
          <span class="material-symbols-outlined">check_circle</span>
        </div>
      </div>

      <div
        class="bg-surface-container-lowest rounded-xl p-6 shadow-sm border border-outline-variant/20 flex items-center justify-between"
      >
        <div>
          <p class="text-xs font-semibold text-on-surface-variant uppercase tracking-wider">
            Canceladas
          </p>

          <p class="text-4xl font-bold font-headline text-error mt-2">
            {{ cancelledSessions }}
          </p>
        </div>

        <div
          class="w-12 h-12 rounded-full bg-error-container flex items-center justify-center text-on-error-container"
        >
          <span class="material-symbols-outlined">event_busy</span>
        </div>
      </div>
    </div>

    <BaseAlert
      v-if="errorMessage"
      type="error"
      :message="errorMessage"
    />

    <!-- Tabla -->
    <div
      class="bg-surface-container-lowest rounded-xl shadow-sm border border-outline-variant/20 overflow-hidden"
    >
      <!-- Buscador -->
      <div class="p-6 border-b border-surface-container-low">
        <div class="relative w-full max-w-md">
          <span
            class="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-on-surface-variant"
          >
            search
          </span>

          <input
            v-model="search"
            type="text"
            placeholder="Buscar por tutor, materia o motivo..."
            class="w-full bg-background rounded-lg pl-10 pr-4 py-2.5 text-sm text-on-surface placeholder:text-on-surface-variant outline-none focus:ring-2 focus:ring-primary/20 transition-all"
          />
        </div>
      </div>

      <!-- Loading -->
      <div
        v-if="isLoading"
        class="flex flex-col items-center justify-center py-16 gap-3"
      >
        <span
          class="material-symbols-outlined text-4xl text-primary animate-spin"
        >
          progress_activity
        </span>

        <p class="text-sm text-on-surface-variant">
          Cargando historial...
        </p>
      </div>

      <!-- Tabla con datos -->
      <div
        v-else-if="filteredSessions.length"
        class="overflow-x-auto"
      >
        <table class="w-full text-left border-collapse">
          <thead>
            <tr class="bg-surface-container-low/50">
              <th class="py-4 px-6 text-sm font-semibold text-on-surface-variant">
                Fecha
              </th>

              <th class="py-4 px-6 text-sm font-semibold text-on-surface-variant">
                Tutor
              </th>

              <th class="py-4 px-6 text-sm font-semibold text-on-surface-variant">
                Materia
              </th>

              <th class="py-4 px-6 text-sm font-semibold text-on-surface-variant">
                Motivo
              </th>

              <th
                class="py-4 px-6 text-sm font-semibold text-on-surface-variant hidden lg:table-cell"
              >
                Dirección
              </th>

              <th class="py-4 px-6 text-sm font-semibold text-on-surface-variant">
                Estado
              </th>

              <th
                class="py-4 px-6 text-sm font-semibold text-on-surface-variant text-right"
              >
                Resumen
              </th>
            </tr>
          </thead>

          <tbody class="divide-y divide-surface-container-low">
            <tr
              v-for="session in filteredSessions"
              :key="session.sesionId"
              class="hover:bg-primary/5 transition-colors group"
            >
              <td class="py-5 px-6 whitespace-nowrap">
                <span class="text-sm font-medium text-on-surface">
                  {{ formatDate(session.fechaSesion) }}
                </span>
              </td>

              <td class="py-5 px-6">
                <div class="flex items-center gap-3">
                  <div
                    class="w-9 h-9 rounded-full bg-primary/10 flex items-center justify-center shrink-0"
                  >
                    <span class="material-symbols-outlined text-primary text-[19px]">
                      person
                    </span>
                  </div>

                  <span class="text-sm font-medium text-on-surface">
                    {{ session.tutor }}
                  </span>
                </div>
              </td>

              <td class="py-5 px-6">
                <span
                  class="inline-flex items-center px-2.5 py-1 rounded-md bg-secondary-fixed/40 text-on-secondary-fixed text-xs font-medium"
                >
                  {{ session.materia }}
                </span>
              </td>

              <td class="py-5 px-6">
                <span
                  class="text-sm text-on-surface block max-w-[180px] truncate"
                  :title="session.motivo"
                >
                  {{ session.motivo }}
                </span>
              </td>

              <td class="py-5 px-6 hidden lg:table-cell">
                <div class="flex items-center gap-1.5 text-on-surface-variant">
                  <span class="material-symbols-outlined text-[17px]">
                    location_on
                  </span>

                  <span
                    class="text-sm max-w-[180px] truncate"
                    :title="session.direccionTutoria"
                  >
                    {{ session.direccionTutoria }}
                  </span>
                </div>
              </td>

              <td class="py-5 px-6">
                <BaseBadge
                  :variant="getStatusVariant(session.estado)"
                  size="sm"
                >
                  {{ formatStatus(session.estado) }}
                </BaseBadge>
              </td>

              <td class="py-5 px-6 text-right">
                <button
                  v-if="session.estado.toUpperCase() === 'ATENDIDA'"
                  type="button"
                  title="Ver resumen"
                  class="p-2 rounded-lg text-primary hover:bg-primary/10 transition-colors"
                  @click="openSummary(session)"
                >
                  <span class="material-symbols-outlined">
                    visibility
                  </span>
                </button>

                <span
                  v-else
                  class="text-xs text-on-surface-variant"
                >
                  No aplica
                </span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Vacío -->
      <div
        v-else
        class="flex flex-col items-center justify-center py-16 px-4 text-center"
      >
        <div
          class="w-16 h-16 rounded-full bg-surface-container-high flex items-center justify-center text-on-surface-variant mb-4"
        >
          <span class="material-symbols-outlined text-[32px]">
            history
          </span>
        </div>

        <h3 class="text-xl font-bold font-headline text-on-surface mb-2">
          No se encontraron sesiones
        </h3>

        <p class="text-sm text-on-surface-variant max-w-md">
          No existen sesiones que coincidan con los criterios seleccionados.
        </p>
      </div>
    </div>

    <!-- Modal resumen -->
    <BaseModal
      v-model="isSummaryOpen"
      title="Resumen de la sesión"
      max-width="lg"
    >
      <div
        v-if="selectedSession"
        class="space-y-5"
      >
        <div>
          <p class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant">
            Materia
          </p>

          <p class="mt-1 font-semibold text-on-surface">
            {{ selectedSession.materia }}
          </p>
        </div>

        <div>
          <p class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant">
            Tutor
          </p>

          <p class="mt-1 text-on-surface">
            {{ selectedSession.tutor }}
          </p>
        </div>

        <div>
          <p class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant">
            Motivo
          </p>

          <p class="mt-1 text-on-surface">
            {{ selectedSession.motivo }}
          </p>
        </div>

        <div>
          <p class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant">
            Resumen
          </p>

          <p
            class="mt-2 whitespace-pre-line rounded-xl bg-surface-container-low p-4 text-sm text-on-surface leading-6"
          >
            {{ selectedSession.resumen || 'No se registró un resumen para esta sesión.' }}
          </p>
        </div>
      </div>
    </BaseModal>
  </div>
</template>