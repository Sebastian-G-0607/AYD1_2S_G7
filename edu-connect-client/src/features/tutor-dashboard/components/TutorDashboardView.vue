<script setup lang="ts">
import { BaseButton } from '@/components/ui'
import { useTutorDashboard } from '../composables/useTutorDashboard'
import TutorStatsCards from './TutorStatsCards.vue'
import TutorSessionsTable from './TutorSessionsTable.vue'
import CompleteSessionModal from './CompleteSessionModal.vue'
import CancelSessionModal from './CancelSessionModal.vue'

const {
  sessions,
  stats,
  isLoading,
  isProcessingAction,
  selectedSession,
  isCompleteModalOpen,
  isCancelModalOpen,
  openCompleteModal,
  openCancelModal,
  handleCompleteSession,
  handleCancelSession
} = useTutorDashboard()
</script>

<template>
  <div class="flex flex-col w-full">
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-8">
      <div>
        <h1 class="text-3xl font-bold font-headline text-on-surface tracking-tight mb-2">
          Mis Sesiones Pendientes
        </h1>
        <p class="text-base text-on-surface-variant font-body">
          Gestiona y actualiza el estado de tus próximas tutorías.
        </p>
      </div>

      <div class="flex items-center gap-3">
        <BaseButton variant="secondary" size="md">
          <template #iconLeft>
            <span class="material-symbols-outlined text-[20px]">filter_list</span>
          </template>
          Filtrar
        </BaseButton>

        <BaseButton variant="primary" size="md">
          <template #iconLeft>
            <span class="material-symbols-outlined text-[20px]">add</span>
          </template>
          Nueva Sesión
        </BaseButton>
      </div>
    </div>

    <TutorStatsCards :stats="stats" />

    <div v-if="sessions.length > 0">
      <TutorSessionsTable
        :sessions="sessions"
        @complete="openCompleteModal"
        @cancel="openCancelModal"
      />
    </div>

    <div
      v-else-if="!isLoading"
      class="flex flex-col items-center justify-center py-16 px-4 text-center bg-surface-container-lowest rounded-2xl border border-outline-variant/20 shadow-sm"
    >
      <div
        class="w-16 h-16 rounded-full bg-surface-container-high flex items-center justify-center text-on-surface-variant mb-4"
      >
        <span class="material-symbols-outlined text-[32px]">event_busy</span>
      </div>
      <h3 class="text-xl font-bold font-headline text-on-surface mb-2">
        No tienes sesiones pendientes
      </h3>
      <p class="text-sm text-on-surface-variant max-w-md font-body mb-6">
        Tu agenda está libre por ahora. Puedes tomar un descanso o revisar el historial de sesiones
        pasadas.
      </p>
      <BaseButton variant="secondary" size="md"> Ver Historial </BaseButton>
    </div>

    <CompleteSessionModal
      v-model="isCompleteModalOpen"
      :session="selectedSession"
      :loading="isProcessingAction"
      @submit="handleCompleteSession"
    />

    <CancelSessionModal
      v-model="isCancelModalOpen"
      :session="selectedSession"
      :loading="isProcessingAction"
      @submit="handleCancelSession"
    />
  </div>
</template>
