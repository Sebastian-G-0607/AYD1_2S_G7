<script setup lang="ts">
import { BaseAlert, BaseButton, BaseModal } from '@/components/ui'
import { useStudentSessions } from '../composables/useStudentSessions'
import StudentSessionCard from './StudentSessionCard.vue'

const {
  filteredSessions,
  isLoading,
  errorMessage,
  successMessage,
  isFilterOpen,
  filterStatus,
  sessionToCancel,
  isCancelModalOpen,
  isCanceling,
  loadSessions,
  toggleFilter,
  setFilter,
  openCancelModal,
  closeCancelModal,
  confirmCancel,
  dismissError,
  dismissSuccess,
  createNewSession
} = useStudentSessions()
</script>

<template>
  <div class="py-2 flex flex-col gap-8 max-w-7xl mx-auto w-full">
    <div class="flex flex-col md:flex-row md:items-end justify-between gap-6 pb-2">
      <div class="flex flex-col gap-2">
        <div class="flex items-center gap-3">
          <div
            class="w-12 h-12 rounded-xl bg-primary-container flex items-center justify-center text-on-primary-container shadow-sm transform -rotate-3 hover:rotate-0 transition-transform duration-300"
          >
            <span
              class="material-symbols-outlined text-[24px]"
              style="font-variation-settings: 'FILL' 1"
            >
              calendar_month
            </span>
          </div>

          <div>
            <h1 class="font-headline text-2xl sm:text-3xl font-bold text-on-surface tracking-tight">
              Mis Sesiones
            </h1>
            <p class="font-body text-sm sm:text-base text-on-surface-variant max-w-2xl mt-1">
              Gestiona tus próximas tutorías. Organiza tu tiempo y prepárate para el éxito
              académico.
            </p>
          </div>
        </div>
      </div>

      <div class="flex flex-wrap items-center gap-3">
        <button
          type="button"
          :class="[
            'px-5 py-2.5 rounded-lg font-semibold text-sm transition-colors flex items-center gap-2',
            isFilterOpen || filterStatus !== 'TODAS'
              ? 'bg-secondary text-on-secondary shadow-sm'
              : 'bg-surface-container-high text-on-surface hover:bg-surface-container-highest'
          ]"
          @click="toggleFilter"
        >
          <span class="material-symbols-outlined text-[18px]"> filter_list </span>
          Filtrar
        </button>

        <button
          type="button"
          class="px-5 py-2.5 rounded-lg bg-primary text-on-primary font-semibold text-sm hover:bg-primary/90 transition-colors shadow-md flex items-center gap-2"
          @click="createNewSession"
        >
          <span class="material-symbols-outlined text-[18px]"> add </span>
          Nueva Sesión
        </button>
      </div>
    </div>

    <BaseAlert
      v-if="errorMessage"
      type="error"
      title="Error"
      :message="errorMessage"
      @dismiss="dismissError"
    >
      <div class="mt-3">
        <button
          type="button"
          class="inline-flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-xs font-semibold bg-on-error-container/10 hover:bg-on-error-container/20 text-on-error-container transition-colors"
          @click="loadSessions"
        >
          <span class="material-symbols-outlined text-[16px]"> refresh </span>
          Reintentar
        </button>
      </div>
    </BaseAlert>

    <BaseAlert
      v-if="successMessage"
      type="success"
      title="Operación exitosa"
      :message="successMessage"
      @dismiss="dismissSuccess"
    />

    <div
      v-if="isFilterOpen"
      class="flex flex-wrap items-center gap-2 p-3 bg-surface-container-lowest rounded-xl border border-outline-variant/30 shadow-sm"
    >
      <span class="text-xs font-semibold text-on-surface-variant mr-2"> Filtrar por estado: </span>

      <button
        type="button"
        :class="[
          'px-3 py-1.5 rounded-lg text-xs font-semibold transition-all',
          filterStatus === 'TODAS'
            ? 'bg-primary text-on-primary shadow-sm'
            : 'bg-surface-container-high text-on-surface-variant hover:text-on-surface'
        ]"
        @click="setFilter('TODAS')"
      >
        Todas
      </button>

      <button
        type="button"
        :class="[
          'px-3 py-1.5 rounded-lg text-xs font-semibold transition-all',
          filterStatus === 'PROXIMAS'
            ? 'bg-secondary-container text-on-secondary-container shadow-sm'
            : 'bg-surface-container-high text-on-surface-variant hover:text-on-surface'
        ]"
        @click="setFilter('PROXIMAS')"
      >
        Próximas
      </button>

      <button
        type="button"
        :class="[
          'px-3 py-1.5 rounded-lg text-xs font-semibold transition-all',
          filterStatus === 'PENDIENTES'
            ? 'bg-primary-container text-on-primary-container shadow-sm'
            : 'bg-surface-container-high text-on-surface-variant hover:text-on-surface'
        ]"
        @click="setFilter('PENDIENTES')"
      >
        Pendientes
      </button>

      <button
        type="button"
        :class="[
          'px-3 py-1.5 rounded-lg text-xs font-semibold transition-all',
          filterStatus === 'CONFIRMADAS'
            ? 'bg-primary-container text-on-primary-container shadow-sm'
            : 'bg-surface-container-high text-on-surface-variant hover:text-on-surface'
        ]"
        @click="setFilter('CONFIRMADAS')"
      >
        Confirmadas
      </button>
    </div>

    <div
      v-if="isLoading"
      class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 relative z-10"
    >
      <div
        v-for="n in 3"
        :key="n"
        class="bg-surface-container-lowest rounded-2xl p-6 shadow-sm border border-outline-variant/20 flex flex-col h-72 animate-pulse"
      >
        <div class="flex justify-between items-start mb-6">
          <div class="flex flex-col gap-2 w-1/2">
            <div class="h-4 bg-surface-container-high rounded w-3/4" />
            <div class="h-3 bg-surface-container rounded w-1/2" />
          </div>
          <div class="h-6 bg-surface-container-high rounded-full w-20" />
        </div>

        <div class="flex items-center gap-4 mb-6">
          <div class="w-14 h-14 rounded-full bg-surface-container-high shrink-0" />
          <div class="flex flex-col gap-2 flex-1">
            <div class="h-4 bg-surface-container-high rounded w-3/4" />
            <div class="h-3 bg-surface-container rounded w-1/2" />
          </div>
        </div>

        <div class="h-16 bg-surface-container-low rounded-xl mb-4" />

        <div class="mt-auto pt-4 border-t border-surface-variant/50">
          <div class="h-9 bg-surface-container-high rounded-lg w-full" />
        </div>
      </div>
    </div>

    <div
      v-else-if="filteredSessions.length > 0"
      class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 relative z-10"
    >
      <StudentSessionCard
        v-for="session in filteredSessions"
        :key="session.id"
        :session="session"
        @cancel="openCancelModal"
      />
    </div>

    <div
      v-else
      class="flex flex-col items-center justify-center py-20 px-4 text-center bg-surface-container-lowest rounded-2xl shadow-sm border border-outline-variant/20"
    >
      <div
        class="w-20 h-20 bg-surface-variant rounded-full flex items-center justify-center mb-6 text-on-surface-variant"
      >
        <span class="material-symbols-outlined text-4xl"> event_busy </span>
      </div>

      <h3 class="font-headline text-xl font-bold text-on-surface mb-2">
        No tienes sesiones agendadas próximas
      </h3>

      <p class="font-body text-sm text-on-surface-variant max-w-md mb-8">
        Parece que tu calendario está libre. Encuentra al tutor ideal y empieza a potenciar tu
        aprendizaje hoy mismo.
      </p>

      <button
        type="button"
        class="px-6 py-3 rounded-xl bg-primary text-on-primary font-semibold text-sm hover:bg-primary/90 transition-colors shadow-md flex items-center gap-2"
        @click="createNewSession"
      >
        <span class="material-symbols-outlined text-[20px]"> search </span>
        Explorar Tutores
      </button>
    </div>

    <BaseModal
      :model-value="isCancelModalOpen"
      title="Cancelar Sesión"
      max-width="md"
      @update:model-value="closeCancelModal"
      @close="closeCancelModal"
    >
      <div class="flex items-start gap-4">
        <div
          class="w-12 h-12 rounded-2xl bg-error-container/60 text-error flex items-center justify-center shrink-0"
        >
          <span class="material-symbols-outlined text-[28px]"> warning </span>
        </div>

        <div class="flex flex-col gap-1.5">
          <h4 class="text-base font-semibold text-on-surface">¿Deseas cancelar esta tutoría?</h4>
          <p v-if="sessionToCancel" class="text-sm text-on-surface-variant leading-relaxed">
            Se cancelará tu sesión de <strong>{{ sessionToCancel.subject }}</strong> con
            <strong>{{ sessionToCancel.tutorName }}</strong> agendada para
            <strong>{{ sessionToCancel.timeLabel }}</strong
            >.
          </p>
        </div>
      </div>

      <template #footer>
        <BaseButton variant="outline" size="md" :disabled="isCanceling" @click="closeCancelModal">
          Volver
        </BaseButton>

        <BaseButton variant="danger" size="md" :loading="isCanceling" @click="confirmCancel">
          Confirmar Cancelación
        </BaseButton>
      </template>
    </BaseModal>
  </div>
</template>
