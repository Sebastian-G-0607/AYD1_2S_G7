<script setup lang="ts">
import { computed } from 'vue'
import { BaseButton, BaseModal } from '@/components/ui'
import { useAdminUserReports } from '../composables/useAdminUserReports'
import type { StudentReportItem, TutorReportItem } from '../types'

const {
  filteredTutorReports,
  filteredStudentReports,
  pendingTutorReports,
  pendingStudentReports,
  activeTab,
  searchQuery,
  isLoading,
  isProcessingAction,
  feedbackMessage,
  selectedReport,
  selectedTarget,
  selectedAction,
  isConfirmModalOpen,
  loadReports,
  openActionModal,
  closeActionModal,
  confirmAction,
  isReportResolved,
  formatDate,
  formatDateTime,
  dismissFeedback
} = useAdminUserReports()

const selectedUserName = computed(() => {
  if (!selectedReport.value || !selectedTarget.value) {
    return ''
  }

  if (selectedTarget.value === 'tutor') {
    return (selectedReport.value as TutorReportItem).tutorNombreCompleto
  }

  return (selectedReport.value as StudentReportItem).estudianteNombreCompleto
})

const modalTitle = computed(() => {
  if (selectedAction.value === 'dar-baja') {
    return selectedTarget.value === 'tutor'
      ? 'Dar de baja al tutor'
      : 'Dar de baja al estudiante'
  }

  return 'Rechazar denuncia'
})

const modalDescription = computed(() => {
  if (selectedAction.value === 'dar-baja') {
    return `¿Estás seguro de que deseas dar de baja a ${selectedUserName.value}? La cuenta quedará inactiva y la denuncia será marcada como resuelta.`
  }

  return `¿Estás seguro de que deseas rechazar esta denuncia? La cuenta de ${selectedUserName.value} no será modificada.`
})

const modalButtonLabel = computed(() => {
  if (selectedAction.value === 'dar-baja') {
    return 'Confirmar baja'
  }

  return 'Rechazar denuncia'
})

function getStatusClasses(estado: string): string {
  const value = estado.toUpperCase()

  if (value === 'RESUELTO') {
    return 'bg-green-100 text-green-800 border-green-200'
  }

  if (value === 'DESESTIMADO') {
    return 'bg-surface-container-high text-on-surface-variant border-outline-variant/30'
  }

  return 'bg-amber-50 text-amber-800 border-amber-200'
}
</script>

<template>
  <div class="flex flex-col w-full">
    <!-- FEEDBACK -->
    <div
      v-if="feedbackMessage"
      :class="[
        'mb-6 p-4 rounded-xl border flex items-center justify-between shadow-sm',
        feedbackMessage.type === 'success'
          ? 'bg-green-50 text-green-800 border-green-200'
          : 'bg-error-container text-on-error-container border-error/20'
      ]"
    >
      <div class="flex items-center gap-3">
        <span class="material-symbols-outlined text-[22px]">
          {{ feedbackMessage.type === 'success' ? 'check_circle' : 'error' }}
        </span>

        <span class="text-sm font-medium">
          {{ feedbackMessage.text }}
        </span>
      </div>

      <button
        type="button"
        class="p-1 opacity-70 hover:opacity-100"
        @click="dismissFeedback"
      >
        <span class="material-symbols-outlined text-[18px]">
          close
        </span>
      </button>
    </div>

    <!-- ENCABEZADO -->
    <div
      class="flex flex-col lg:flex-row lg:items-end lg:justify-between gap-5 mb-7"
    >
      <div>
        <h1
          class="text-3xl font-bold text-on-surface tracking-tight mb-2"
        >
          Gestión de Denuncias
        </h1>

        <p class="text-on-surface-variant max-w-2xl">
          Revisa los reportes realizados entre estudiantes y tutores y toma
          las acciones correspondientes.
        </p>
      </div>

      <button
        type="button"
        :disabled="isLoading"
        class="h-10 px-4 rounded-xl bg-surface-container-highest text-on-surface flex items-center justify-center gap-2 text-sm font-semibold hover:bg-surface-container-high transition-colors disabled:opacity-50"
        @click="loadReports"
      >
        <span
          :class="[
            'material-symbols-outlined text-[20px]',
            isLoading ? 'animate-spin' : ''
          ]"
        >
          refresh
        </span>

        Actualizar
      </button>
    </div>

    <!-- PESTAÑAS Y BÚSQUEDA -->
    <div
      class="bg-surface-container-lowest rounded-2xl border border-outline-variant/20 shadow-sm p-4 mb-6"
    >
      <div
        class="flex flex-col lg:flex-row lg:items-center gap-4"
      >
        <div
          class="flex-1 bg-surface-container-low rounded-xl p-1 flex"
        >
          <button
            type="button"
            :class="[
              'flex-1 rounded-lg px-4 py-2.5 flex items-center justify-center gap-2 text-sm font-semibold transition-all',
              activeTab === 'tutores'
                ? 'bg-surface-container-lowest text-primary shadow-sm'
                : 'text-on-surface-variant hover:text-on-surface'
            ]"
            @click="activeTab = 'tutores'"
          >
            <span class="material-symbols-outlined text-[19px]">
              school
            </span>

            Reportes hacia tutores

            <span
              class="px-2 py-0.5 rounded-full text-xs bg-error-container text-on-error-container"
            >
              {{ pendingTutorReports }}
            </span>
          </button>

          <button
            type="button"
            :class="[
              'flex-1 rounded-lg px-4 py-2.5 flex items-center justify-center gap-2 text-sm font-semibold transition-all',
              activeTab === 'estudiantes'
                ? 'bg-surface-container-lowest text-primary shadow-sm'
                : 'text-on-surface-variant hover:text-on-surface'
            ]"
            @click="activeTab = 'estudiantes'"
          >
            <span class="material-symbols-outlined text-[19px]">
              person
            </span>

            Reportes hacia estudiantes

            <span
              class="px-2 py-0.5 rounded-full text-xs bg-error-container text-on-error-container"
            >
              {{ pendingStudentReports }}
            </span>
          </button>
        </div>

        <div class="relative w-full lg:w-80">
          <span
            class="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-on-surface-variant text-[20px]"
          >
            search
          </span>

          <input
            v-model="searchQuery"
            type="text"
            placeholder="Buscar reporte..."
            class="w-full h-11 pl-10 pr-4 rounded-xl bg-surface-container-lowest border border-outline-variant/40 text-sm text-on-surface outline-none focus:border-primary focus:ring-2 focus:ring-primary/10"
          />
        </div>
      </div>
    </div>

    <!-- CARGANDO -->
    <div
      v-if="isLoading"
      class="min-h-[320px] flex flex-col items-center justify-center text-on-surface-variant"
    >
      <span
        class="material-symbols-outlined text-primary text-[36px] animate-spin mb-3"
      >
        progress_activity
      </span>

      <p class="text-sm font-medium">
        Cargando denuncias...
      </p>
    </div>

    <!-- REPORTES CONTRA TUTORES -->
    <div
      v-else-if="activeTab === 'tutores'"
      class="space-y-4"
    >
      <article
        v-for="report in filteredTutorReports"
        :key="report.id"
        class="bg-surface-container-lowest rounded-2xl border border-outline-variant/20 shadow-sm p-5 sm:p-6"
      >
        <div
          class="flex flex-col xl:flex-row xl:items-start xl:justify-between gap-5"
        >
          <div class="flex-1 min-w-0">
            <div
              class="flex flex-wrap items-center gap-2 mb-4"
            >
              <span
                class="inline-flex items-center gap-1.5 px-3 py-1 rounded-full bg-error-container/50 text-on-error-container text-xs font-semibold"
              >
                <span class="material-symbols-outlined text-[16px]">
                  report
                </span>

                {{ report.categoria }}
              </span>

              <span
                :class="[
                  'px-3 py-1 rounded-full border text-xs font-semibold',
                  getStatusClasses(report.estado)
                ]"
              >
                {{ report.estado }}
              </span>

              <span class="text-xs text-on-surface-variant">
                Reporte #{{ report.id }}
              </span>
            </div>

            <div
              class="grid grid-cols-1 md:grid-cols-2 gap-5 mb-5"
            >
              <div
                class="p-4 rounded-xl bg-surface-container-low"
              >
                <p
                  class="text-xs uppercase tracking-wide font-semibold text-on-surface-variant mb-2"
                >
                  Tutor denunciado
                </p>

                <p class="font-semibold text-on-surface">
                  {{ report.tutorNombreCompleto }}
                </p>

                <p class="text-sm text-on-surface-variant mt-1">
                  {{ report.tutorCorreo }}
                </p>
              </div>

              <div
                class="p-4 rounded-xl bg-surface-container-low"
              >
                <p
                  class="text-xs uppercase tracking-wide font-semibold text-on-surface-variant mb-2"
                >
                  Estudiante denunciante
                </p>

                <p class="font-semibold text-on-surface">
                  {{ report.estudianteDenuncianteNombreCompleto }}
                </p>

                <p class="text-sm text-on-surface-variant mt-1">
                  {{ report.estudianteDenuncianteCorreo }}
                </p>
              </div>
            </div>

            <div class="mb-5">
              <p
                class="text-xs uppercase tracking-wide font-semibold text-on-surface-variant mb-2"
              >
                Motivo
              </p>

              <p
                class="text-sm leading-relaxed text-on-surface"
              >
                {{ report.motivo }}
              </p>
            </div>

            <div
              class="flex flex-wrap gap-x-6 gap-y-2 text-xs text-on-surface-variant"
            >
              <span class="inline-flex items-center gap-1.5">
                <span class="material-symbols-outlined text-[17px]">
                  event
                </span>

                Sesión: {{ formatDate(report.fechaSesion) }}
              </span>

              <span class="inline-flex items-center gap-1.5">
                <span class="material-symbols-outlined text-[17px]">
                  schedule
                </span>

                Reportado: {{ formatDateTime(report.fechaReporte) }}
              </span>

              <span class="inline-flex items-center gap-1.5">
                Sesión #{{ report.sesionId }}
              </span>
            </div>
          </div>

          <div
            v-if="!isReportResolved(report.estado)"
            class="flex flex-row xl:flex-col gap-2 shrink-0"
          >
            <button
              type="button"
              class="px-4 py-2.5 rounded-xl bg-error text-on-error text-sm font-semibold flex items-center justify-center gap-2 hover:opacity-90 transition-opacity"
              @click="
                openActionModal(
                  report,
                  'tutor',
                  'dar-baja'
                )
              "
            >
              <span class="material-symbols-outlined text-[18px]">
                person_remove
              </span>

              Dar de baja
            </button>

            <button
              type="button"
              class="px-4 py-2.5 rounded-xl border border-outline-variant/50 text-on-surface text-sm font-semibold flex items-center justify-center gap-2 hover:bg-surface-container-low transition-colors"
              @click="
                openActionModal(
                  report,
                  'tutor',
                  'rechazar'
                )
              "
            >
              <span class="material-symbols-outlined text-[18px]">
                block
              </span>

              Rechazar denuncia
            </button>
          </div>

          <div
            v-else
            class="px-4 py-2.5 rounded-xl bg-surface-container-low text-on-surface-variant text-sm font-semibold shrink-0"
          >
            Reporte finalizado
          </div>
        </div>
      </article>

      <div
        v-if="filteredTutorReports.length === 0"
        class="min-h-[300px] bg-surface-container-lowest rounded-2xl border border-outline-variant/20 flex flex-col items-center justify-center text-center p-8"
      >
        <span
          class="material-symbols-outlined text-[42px] text-on-surface-variant mb-3"
        >
          inbox
        </span>

        <h3 class="font-semibold text-on-surface mb-1">
          No hay reportes hacia tutores
        </h3>

        <p class="text-sm text-on-surface-variant">
          No se encontraron denuncias para mostrar.
        </p>
      </div>
    </div>

    <!-- REPORTES CONTRA ESTUDIANTES -->
    <div
      v-else
      class="space-y-4"
    >
      <article
        v-for="report in filteredStudentReports"
        :key="report.id"
        class="bg-surface-container-lowest rounded-2xl border border-outline-variant/20 shadow-sm p-5 sm:p-6"
      >
        <div
          class="flex flex-col xl:flex-row xl:items-start xl:justify-between gap-5"
        >
          <div class="flex-1 min-w-0">
            <div
              class="flex flex-wrap items-center gap-2 mb-4"
            >
              <span
                class="inline-flex items-center gap-1.5 px-3 py-1 rounded-full bg-error-container/50 text-on-error-container text-xs font-semibold"
              >
                <span class="material-symbols-outlined text-[16px]">
                  report
                </span>

                {{ report.categoria }}
              </span>

              <span
                :class="[
                  'px-3 py-1 rounded-full border text-xs font-semibold',
                  getStatusClasses(report.estado)
                ]"
              >
                {{ report.estado }}
              </span>

              <span class="text-xs text-on-surface-variant">
                Reporte #{{ report.id }}
              </span>
            </div>

            <div
              class="grid grid-cols-1 md:grid-cols-2 gap-5 mb-5"
            >
              <div
                class="p-4 rounded-xl bg-surface-container-low"
              >
                <p
                  class="text-xs uppercase tracking-wide font-semibold text-on-surface-variant mb-2"
                >
                  Estudiante denunciado
                </p>

                <p class="font-semibold text-on-surface">
                  {{ report.estudianteNombreCompleto }}
                </p>

                <p class="text-sm text-on-surface-variant mt-1">
                  {{ report.estudianteCorreo }}
                </p>
              </div>

              <div
                class="p-4 rounded-xl bg-surface-container-low"
              >
                <p
                  class="text-xs uppercase tracking-wide font-semibold text-on-surface-variant mb-2"
                >
                  Tutor denunciante
                </p>

                <p class="font-semibold text-on-surface">
                  {{ report.tutorDenuncianteNombreCompleto }}
                </p>

                <p class="text-sm text-on-surface-variant mt-1">
                  {{ report.tutorDenuncianteCorreo }}
                </p>
              </div>
            </div>

            <div class="mb-5">
              <p
                class="text-xs uppercase tracking-wide font-semibold text-on-surface-variant mb-2"
              >
                Motivo
              </p>

              <p
                class="text-sm leading-relaxed text-on-surface"
              >
                {{ report.motivo }}
              </p>
            </div>

            <div
              class="flex flex-wrap gap-x-6 gap-y-2 text-xs text-on-surface-variant"
            >
              <span class="inline-flex items-center gap-1.5">
                <span class="material-symbols-outlined text-[17px]">
                  event
                </span>

                Sesión: {{ formatDate(report.fechaSesion) }}
              </span>

              <span class="inline-flex items-center gap-1.5">
                <span class="material-symbols-outlined text-[17px]">
                  schedule
                </span>

                Reportado: {{ formatDateTime(report.fechaReporte) }}
              </span>

              <span class="inline-flex items-center gap-1.5">
                Sesión #{{ report.sesionId }}
              </span>
            </div>
          </div>

          <div
            v-if="!isReportResolved(report.estado)"
            class="flex flex-row xl:flex-col gap-2 shrink-0"
          >
            <button
              type="button"
              class="px-4 py-2.5 rounded-xl bg-error text-on-error text-sm font-semibold flex items-center justify-center gap-2 hover:opacity-90 transition-opacity"
              @click="
                openActionModal(
                  report,
                  'estudiante',
                  'dar-baja'
                )
              "
            >
              <span class="material-symbols-outlined text-[18px]">
                person_remove
              </span>

              Dar de baja
            </button>

            <button
              type="button"
              class="px-4 py-2.5 rounded-xl border border-outline-variant/50 text-on-surface text-sm font-semibold flex items-center justify-center gap-2 hover:bg-surface-container-low transition-colors"
              @click="
                openActionModal(
                  report,
                  'estudiante',
                  'rechazar'
                )
              "
            >
              <span class="material-symbols-outlined text-[18px]">
                block
              </span>

              Rechazar denuncia
            </button>
          </div>

          <div
            v-else
            class="px-4 py-2.5 rounded-xl bg-surface-container-low text-on-surface-variant text-sm font-semibold shrink-0"
          >
            Reporte finalizado
          </div>
        </div>
      </article>

      <div
        v-if="filteredStudentReports.length === 0"
        class="min-h-[300px] bg-surface-container-lowest rounded-2xl border border-outline-variant/20 flex flex-col items-center justify-center text-center p-8"
      >
        <span
          class="material-symbols-outlined text-[42px] text-on-surface-variant mb-3"
        >
          inbox
        </span>

        <h3 class="font-semibold text-on-surface mb-1">
          No hay reportes hacia estudiantes
        </h3>

        <p class="text-sm text-on-surface-variant">
          No se encontraron denuncias para mostrar.
        </p>
      </div>
    </div>

    <!-- MODAL CONFIRMACIÓN -->
    <BaseModal
      :model-value="isConfirmModalOpen"
      :title="modalTitle"
      max-width="md"
      @update:model-value="closeActionModal"
      @close="closeActionModal"
    >
      <div class="flex gap-4">
        <div
          :class="[
            'w-12 h-12 rounded-xl flex items-center justify-center shrink-0',
            selectedAction === 'dar-baja'
              ? 'bg-error-container text-error'
              : 'bg-surface-container-high text-on-surface-variant'
          ]"
        >
          <span class="material-symbols-outlined text-[26px]">
            {{
              selectedAction === 'dar-baja'
                ? 'person_remove'
                : 'block'
            }}
          </span>
        </div>

        <div>
          <p class="text-sm leading-relaxed text-on-surface-variant">
            {{ modalDescription }}
          </p>
        </div>
      </div>

      <template #footer>
        <BaseButton
          variant="outline"
          size="md"
          :disabled="isProcessingAction"
          @click="closeActionModal"
        >
          Cancelar
        </BaseButton>

        <BaseButton
          :variant="
            selectedAction === 'dar-baja'
              ? 'danger'
              : 'primary'
          "
          size="md"
          :loading="isProcessingAction"
          @click="confirmAction"
        >
          {{ modalButtonLabel }}
        </BaseButton>
      </template>
    </BaseModal>
  </div>
</template>