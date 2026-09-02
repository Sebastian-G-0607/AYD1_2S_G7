<script setup lang="ts">
import { useAdminApprovals } from '../composables/useAdminApprovals'
import AdminApprovalsStudentList from './AdminApprovalsStudentList.vue'
import AdminApprovalsTutorList from './AdminApprovalsTutorList.vue'

const {
  filteredStudents,
  filteredTutors,
  isLoading,
  searchQuery,
  activeTab,
  pendingStudentsCount,
  pendingTutorsCount,
  selectedItem,
  isApproveModalOpen,
  isRejectModalOpen,
  rejectReason,
  isProcessingAction,
  feedbackMessage,
  fetchAll,
  openApproveModal,
  openRejectModal,
  closeModals,
  confirmApprove,
  confirmReject,
  dismissFeedback
} = useAdminApprovals()
</script>

<template>
  <div class="flex flex-col w-full relative">
    <!-- MENSAJE DE FEEDBACK / ALERTA -->
    <div
      v-if="feedbackMessage"
      :class="[
        'mb-6 p-4 rounded-xl flex items-center justify-between shadow-sm transition-all',
        feedbackMessage.type === 'success'
          ? 'bg-[#c3e6cb] text-[#155724] border border-[#a3d7b0]'
          : 'bg-error-container text-on-error-container border border-error/20'
      ]"
    >
      <div class="flex items-center gap-3">
        <span class="material-symbols-outlined text-[22px]">
          {{ feedbackMessage.type === 'success' ? 'check_circle' : 'error' }}
        </span>
        <span class="font-label-md text-label-md">{{ feedbackMessage.text }}</span>
      </div>
      <button
        type="button"
        class="text-current opacity-70 hover:opacity-100 p-1 cursor-pointer"
        @click="dismissFeedback"
      >
        <span class="material-symbols-outlined text-[18px]">close</span>
      </button>
    </div>

    <!-- ENCABEZADO DE PÁGINA (STITCH TEMPLATE) -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-margin-desktop">
      <div>
        <h1 class="font-headline-lg text-headline-lg text-on-surface mb-2">
          Aprobaciones Pendientes
        </h1>
        <p class="font-body-lg text-body-lg text-on-surface-variant max-w-2xl">
          Gestión de solicitudes de nuevos usuarios en la plataforma. Revise cuidadosamente los datos antes de aceptar o rechazar.
        </p>
      </div>

      <div class="flex items-center gap-2 flex-wrap">
        <button
          type="button"
          class="bg-surface-container-highest text-on-surface hover:bg-surface-dim transition-colors h-10 px-4 rounded-lg flex items-center justify-center gap-2 font-label-md text-label-md cursor-pointer"
          :disabled="isLoading"
          @click="fetchAll"
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

        <button
          type="button"
          class="bg-surface-container-highest text-on-surface hover:bg-surface-dim transition-colors h-10 px-4 rounded-lg flex items-center justify-center gap-2 font-label-md text-label-md cursor-pointer"
        >
          <span class="material-symbols-outlined text-[20px]">filter_list</span>
          Filtrar
        </button>

        <button
          type="button"
          class="bg-surface-container-highest text-on-surface hover:bg-surface-dim transition-colors h-10 px-4 rounded-lg flex items-center justify-center gap-2 font-label-md text-label-md cursor-pointer"
        >
          <span class="material-symbols-outlined text-[20px]">download</span>
          Exportar Lista
        </button>
      </div>
    </div>

    <!-- CONTENEDOR DE PESTAÑAS Y BÚSQUEDA (STITCH TEMPLATE) -->
    <div class="bg-surface-container-lowest rounded-xl shadow-sm p-4 mb-margin-desktop relative overflow-hidden">
      <div class="absolute inset-0 bg-gradient-to-r from-primary/5 via-transparent to-transparent opacity-50 pointer-events-none" />

      <div class="flex flex-col md:flex-row items-center gap-4 md:gap-6 relative z-10">
        <!-- Selector de Pestañas (HU-05 vs HU-06) -->
        <div class="flex-1 w-full bg-surface-container-low rounded-lg p-1 flex">
          <!-- Pestaña Estudiantes -->
          <button
            type="button"
            :class="[
              'flex-1 rounded-md py-2 px-4 font-label-md text-label-md flex items-center justify-center gap-2 transition-all cursor-pointer',
              activeTab === 'estudiantes'
                ? 'text-on-surface bg-surface-container-lowest shadow-sm'
                : 'text-on-surface-variant hover:text-on-surface'
            ]"
            @click="activeTab = 'estudiantes'"
          >
            <span class="material-symbols-outlined text-[18px]">school</span>
            <span>Estudiantes Pendientes</span>
            <span
              :class="[
                'font-label-sm text-label-sm px-2 py-0.5 rounded-full ml-1 font-semibold',
                activeTab === 'estudiantes'
                  ? 'bg-primary text-on-primary'
                  : 'bg-surface-container-highest text-on-surface'
              ]"
            >
              {{ pendingStudentsCount }}
            </span>
          </button>

          <!-- Pestaña Tutores -->
          <button
            type="button"
            :class="[
              'flex-1 rounded-md py-2 px-4 font-label-md text-label-md flex items-center justify-center gap-2 transition-all cursor-pointer',
              activeTab === 'tutores'
                ? 'text-on-surface bg-surface-container-lowest shadow-sm'
                : 'text-on-surface-variant hover:text-on-surface'
            ]"
            @click="activeTab = 'tutores'"
          >
            <span class="material-symbols-outlined text-[18px]">co_present</span>
            <span>Tutores Pendientes</span>
            <span
              :class="[
                'font-label-sm text-label-sm px-2 py-0.5 rounded-full ml-1 font-semibold',
                activeTab === 'tutores'
                  ? 'bg-primary text-on-primary'
                  : 'bg-surface-container-highest text-on-surface'
              ]"
            >
              {{ pendingTutorsCount }}
            </span>
          </button>
        </div>

        <!-- Buscador Reactivo -->
        <div class="flex-1 w-full relative">
          <span class="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-on-surface-variant text-[20px]">
            search
          </span>
          <input
            v-model="searchQuery"
            type="text"
            :placeholder="
              activeTab === 'estudiantes'
                ? 'Buscar por nombre, carnet o correo...'
                : 'Buscar por nombre, carnet, ID o correo...'
            "
            class="w-full h-10 bg-surface-container-lowest text-on-surface font-body-md text-body-md pl-10 pr-4 rounded-lg focus:outline-none ring-1 ring-outline-variant focus:ring-2 focus:ring-primary transition-all"
          />
        </div>
      </div>
    </div>

    <!-- LISTA DE ESTUDIANTES (HU-05) -->
    <div v-show="activeTab === 'estudiantes'">
      <AdminApprovalsStudentList
        :students="filteredStudents"
        :is-loading="isLoading"
        @approve="openApproveModal($event, 'estudiante')"
        @reject="openRejectModal($event, 'estudiante')"
      />
    </div>

    <!-- LISTA DE TUTORES (HU-06) -->
    <div v-show="activeTab === 'tutores'">
      <AdminApprovalsTutorList
        :tutors="filteredTutors"
        :is-loading="isLoading"
        @approve="openApproveModal($event, 'tutor')"
        @reject="openRejectModal($event, 'tutor')"
      />
    </div>

    <!-- ========================================================================= -->
    <!-- MODAL DE CONFIRMACIÓN: ACEPTAR (ESTILO STITCH)                           -->
    <!-- ========================================================================= -->
    <div
      v-if="isApproveModalOpen && selectedItem"
      class="fixed inset-0 z-50 flex items-center justify-center bg-on-background/30 backdrop-blur-sm p-4"
    >
      <div class="bg-surface-container-lowest rounded-2xl shadow-xl w-full max-w-md overflow-hidden animate-scale-in">
        <div class="p-6">
          <div class="w-12 h-12 rounded-full bg-[#c3e6cb] flex items-center justify-center mb-4 text-[#155724]">
            <span class="material-symbols-outlined text-[24px]">verified</span>
          </div>

          <h2 class="font-headline-md text-headline-md text-on-surface mb-2">
            Confirmar Aprobación
          </h2>

          <p class="font-body-md text-body-md text-on-surface-variant mb-6">
            ¿Estás seguro de que deseas aceptar a
            <strong class="text-on-surface font-semibold">
              {{ selectedItem.data.nombre }} {{ selectedItem.data.apellido }}
            </strong>
            ({{ selectedItem.type === 'tutor' ? 'Tutor' : 'Estudiante' }})? Se activará su cuenta en el sistema y se le enviará un correo notificando la decisión.
          </p>

          <div class="bg-surface-container-low rounded-lg p-3 mb-6 flex gap-3 items-center">
            <span class="material-symbols-outlined text-on-surface-variant">mail</span>
            <span class="font-label-sm text-label-sm text-on-surface-variant truncate">
              Se enviará notificación a <strong class="text-on-surface">{{ selectedItem.data.correo }}</strong>
            </span>
          </div>

          <div class="flex gap-3 justify-end">
            <button
              type="button"
              :disabled="isProcessingAction"
              class="px-5 py-2.5 rounded-lg text-on-surface font-label-md text-label-md hover:bg-surface-container-highest transition-colors cursor-pointer disabled:opacity-50"
              @click="closeModals"
            >
              Cancelar
            </button>

            <button
              type="button"
              :disabled="isProcessingAction"
              class="px-5 py-2.5 rounded-lg bg-[#28a745] text-white font-label-md text-label-md hover:bg-[#218838] transition-colors shadow-sm flex items-center gap-2 cursor-pointer disabled:opacity-50"
              @click="confirmApprove"
            >
              <span v-if="isProcessingAction" class="w-4 h-4 border-2 border-white/40 border-t-white rounded-full animate-spin" />
              <span>Aceptar {{ selectedItem.type === 'tutor' ? 'Tutor' : 'Estudiante' }}</span>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- ========================================================================= -->
    <!-- MODAL DE CONFIRMACIÓN: RECHAZAR (ESTILO STITCH)                          -->
    <!-- ========================================================================= -->
    <div
      v-if="isRejectModalOpen && selectedItem"
      class="fixed inset-0 z-50 flex items-center justify-center bg-on-background/30 backdrop-blur-sm p-4"
    >
      <div class="bg-surface-container-lowest rounded-2xl shadow-xl w-full max-w-md overflow-hidden animate-scale-in">
        <div class="p-6">
          <div class="w-12 h-12 rounded-full bg-error-container text-on-error-container flex items-center justify-center mb-4">
            <span class="material-symbols-outlined text-[24px]">cancel</span>
          </div>

          <h2 class="font-headline-md text-headline-md text-on-surface mb-2">
            Confirmar Rechazo
          </h2>

          <p class="font-body-md text-body-md text-on-surface-variant mb-4">
            ¿Estás seguro de que deseas rechazar la solicitud de
            <strong class="text-on-surface font-semibold">
              {{ selectedItem.data.nombre }} {{ selectedItem.data.apellido }}
            </strong>
            ({{ selectedItem.type === 'tutor' ? 'Tutor' : 'Estudiante' }})? Esta acción no se puede deshacer.
          </p>

          <div class="flex flex-col gap-1.5 mb-6">
            <label for="reject-motivo" class="font-label-sm text-label-sm text-on-surface font-semibold">
              Motivo del rechazo (opcional):
            </label>
            <textarea
              id="reject-motivo"
              v-model="rejectReason"
              rows="3"
              placeholder="Indica la razón por la cual se rechaza la solicitud..."
              class="w-full bg-surface-container-low text-on-surface font-body-sm text-body-sm rounded-lg border border-outline-variant p-3 focus:outline-none focus:ring-2 focus:ring-error/40 focus:border-error resize-none"
            />
          </div>

          <div class="flex gap-3 justify-end">
            <button
              type="button"
              :disabled="isProcessingAction"
              class="px-5 py-2.5 rounded-lg text-on-surface font-label-md text-label-md hover:bg-surface-container-highest transition-colors cursor-pointer disabled:opacity-50"
              @click="closeModals"
            >
              Cancelar
            </button>

            <button
              type="button"
              :disabled="isProcessingAction"
              class="px-5 py-2.5 rounded-lg bg-error text-on-error font-label-md text-label-md hover:bg-[#93000a] transition-colors shadow-sm flex items-center gap-2 cursor-pointer disabled:opacity-50"
              @click="confirmReject"
            >
              <span v-if="isProcessingAction" class="w-4 h-4 border-2 border-white/40 border-t-white rounded-full animate-spin" />
              <span>Confirmar Rechazo</span>
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
