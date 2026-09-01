<script setup lang="ts">
import { BaseButton, BaseModal } from '@/components/ui'
import { useStudentApprovals } from '../composables/useStudentApprovals'

const {
  filteredStudents,
  isLoading,
  searchQuery,
  activeTab,
  pendingCount,
  selectedStudent,
  isApproveModalOpen,
  isRejectModalOpen,
  isProcessingAction,
  openApproveModal,
  openRejectModal,
  confirmApprove,
  confirmReject
} = useStudentApprovals()

function getInitials(nombre: string, apellido: string): string {
  return `${nombre.charAt(0)}${apellido.charAt(0)}`.toUpperCase()
}
</script>

<template>
  <div class="flex flex-col w-full">
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-8">
      <div>
        <h1 class="text-3xl font-bold font-headline text-on-surface tracking-tight mb-2">
          Aprobaciones Pendientes
        </h1>
        <p class="text-base text-on-surface-variant max-w-2xl font-body">
          Gestión de solicitudes de nuevos usuarios en la plataforma. Revise cuidadosamente los
          datos antes de aceptar o rechazar.
        </p>
      </div>

      <div class="flex items-center gap-3">
        <BaseButton variant="secondary" size="md">
          <template #iconLeft>
            <span class="material-symbols-outlined text-[20px]">filter_list</span>
          </template>
          Filtrar
        </BaseButton>

        <BaseButton variant="secondary" size="md">
          <template #iconLeft>
            <span class="material-symbols-outlined text-[20px]">download</span>
          </template>
          Exportar Lista
        </BaseButton>
      </div>
    </div>

    <div
      class="bg-surface-container-lowest rounded-xl shadow-sm border border-outline-variant/20 p-4 mb-8"
    >
      <div class="flex flex-col md:flex-row items-center gap-4">
        <div class="flex w-full md:w-auto bg-surface-container-low rounded-lg p-1">
          <button
            type="button"
            :class="[
              'flex-1 md:flex-none flex items-center justify-center gap-2 py-2 px-4 rounded-md text-sm font-semibold transition-all',
              activeTab === 'estudiantes'
                ? 'bg-surface-container-lowest text-on-surface shadow-sm'
                : 'text-on-surface-variant hover:text-on-surface'
            ]"
            @click="activeTab = 'estudiantes'"
          >
            <span class="material-symbols-outlined text-[18px]">school</span>
            <span>Estudiantes Pendientes</span>
            <span
              class="bg-primary text-on-primary text-xs px-2 py-0.5 rounded-full ml-1 font-bold"
            >
              {{ pendingCount }}
            </span>
          </button>

          <button
            type="button"
            :class="[
              'flex-1 md:flex-none flex items-center justify-center gap-2 py-2 px-4 rounded-md text-sm font-semibold transition-all',
              activeTab === 'tutores'
                ? 'bg-surface-container-lowest text-on-surface shadow-sm'
                : 'text-on-surface-variant hover:text-on-surface'
            ]"
            @click="activeTab = 'tutores'"
          >
            <span class="material-symbols-outlined text-[18px]">co_present</span>
            <span>Tutores Pendientes</span>
            <span
              class="bg-surface-container-highest text-on-surface text-xs px-2 py-0.5 rounded-full ml-1 font-bold"
            >
              0
            </span>
          </button>
        </div>

        <div class="w-full md:flex-1 relative">
          <span
            class="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-on-surface-variant text-[20px]"
          >
            search
          </span>
          <input
            v-model="searchQuery"
            type="text"
            placeholder="Buscar por nombre, carnet o correo..."
            class="w-full h-10 bg-surface-container-lowest text-on-surface text-sm pl-10 pr-4 rounded-lg border border-outline-variant focus:outline-none focus:ring-2 focus:ring-secondary/40 focus:border-secondary transition-all"
          />
        </div>
      </div>
    </div>

    <div
      v-if="filteredStudents.length > 0"
      class="bg-surface-container-lowest shadow-sm rounded-xl border border-outline-variant/20 overflow-hidden mb-8"
    >
      <div class="overflow-x-auto">
        <table class="w-full text-left text-sm text-on-surface">
          <thead
            class="bg-surface-container/60 text-on-surface-variant uppercase text-xs tracking-wider border-b border-surface-container-high"
          >
            <tr>
              <th class="py-3.5 px-6 font-semibold">Fotografía</th>
              <th class="py-3.5 px-6 font-semibold">Nombre Completo</th>
              <th class="py-3.5 px-6 font-semibold">Carnet</th>
              <th class="py-3.5 px-6 font-semibold">Género</th>
              <th class="py-3.5 px-6 font-semibold">Fecha de Nacimiento</th>
              <th class="py-3.5 px-6 font-semibold">Correo Electrónico</th>
              <th class="py-3.5 px-6 font-semibold text-center">Acciones</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-surface-container">
            <tr
              v-for="student in filteredStudents"
              :key="student.id"
              class="hover:bg-surface-container-low/60 transition-colors"
            >
              <td class="py-4 px-6">
                <div
                  class="w-10 h-10 rounded-full overflow-hidden bg-primary-container text-on-primary-container flex items-center justify-center font-bold text-xs shadow-sm"
                >
                  <img
                    v-if="student.fotografiaUrl"
                    :src="student.fotografiaUrl"
                    :alt="student.nombre"
                    class="w-full h-full object-cover"
                  />
                  <span v-else>{{ getInitials(student.nombre, student.apellido) }}</span>
                </div>
              </td>
              <td class="py-4 px-6">
                <p class="font-semibold text-on-surface">
                  {{ student.nombre }} {{ student.apellido }}
                </p>
              </td>
              <td class="py-4 px-6">
                <span
                  class="font-mono text-xs bg-surface-container-highest text-on-surface px-2.5 py-1 rounded-md font-semibold"
                >
                  {{ student.carnet }}
                </span>
              </td>
              <td class="py-4 px-6 text-on-surface-variant capitalize">
                {{ student.genero }}
              </td>
              <td class="py-4 px-6 text-on-surface-variant">
                {{ student.fechaNacimiento }}
              </td>
              <td class="py-4 px-6 text-on-surface-variant truncate max-w-[200px]">
                {{ student.correo }}
              </td>
              <td class="py-4 px-6">
                <div class="flex items-center justify-center gap-2">
                  <button
                    type="button"
                    class="inline-flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-[#c3e6cb] text-[#155724] hover:bg-[#218838] hover:text-white transition-all text-xs font-semibold shadow-xs"
                    @click="openApproveModal(student)"
                  >
                    <span class="material-symbols-outlined text-[16px]">check_circle</span>
                    <span>Aceptar</span>
                  </button>

                  <button
                    type="button"
                    class="inline-flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-error-container text-on-error-container hover:bg-error hover:text-white transition-all text-xs font-semibold shadow-xs"
                    @click="openRejectModal(student)"
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

      <div
        class="p-4 bg-surface-container-lowest border-t border-surface-container flex items-center justify-between"
      >
        <span class="text-xs text-on-surface-variant">
          Mostrando {{ filteredStudents.length }} de {{ pendingCount }} registros
        </span>

        <div class="flex items-center gap-1">
          <button
            type="button"
            disabled
            class="w-8 h-8 flex items-center justify-center rounded-md text-on-surface-variant hover:bg-surface-container-low disabled:opacity-40"
          >
            <span class="material-symbols-outlined text-[18px]">chevron_left</span>
          </button>
          <button
            type="button"
            class="w-8 h-8 flex items-center justify-center rounded-md bg-primary text-on-primary text-xs font-bold"
          >
            1
          </button>
          <button
            type="button"
            disabled
            class="w-8 h-8 flex items-center justify-center rounded-md text-on-surface-variant hover:bg-surface-container-low disabled:opacity-40"
          >
            <span class="material-symbols-outlined text-[18px]">chevron_right</span>
          </button>
        </div>
      </div>
    </div>

    <div
      v-else-if="!isLoading"
      class="bg-surface-container-lowest shadow-sm rounded-xl border border-outline-variant/20 p-12 text-center flex flex-col items-center justify-center min-h-[320px]"
    >
      <div
        class="w-16 h-16 bg-surface-container-highest rounded-full flex items-center justify-center mb-4 text-on-surface-variant"
      >
        <span class="material-symbols-outlined text-[32px]">inbox</span>
      </div>
      <h3 class="text-xl font-bold font-headline text-on-surface mb-2">Todo al día</h3>
      <p class="text-sm text-on-surface-variant max-w-md font-body">
        No hay solicitudes de estudiantes pendientes de aprobación en este momento. Vuelve más
        tarde.
      </p>
    </div>

    <BaseModal v-model="isApproveModalOpen" title="Confirmar Aprobación" max-width="md">
      <div class="flex flex-col gap-4">
        <div
          class="w-12 h-12 rounded-full bg-[#c3e6cb] flex items-center justify-center text-[#155724]"
        >
          <span class="material-symbols-outlined text-[26px]">verified</span>
        </div>

        <p class="text-sm text-on-surface-variant font-body">
          ¿Estás seguro de que deseas aceptar a
          <strong class="text-on-surface font-semibold">
            {{ selectedStudent?.nombre }} {{ selectedStudent?.apellido }} </strong
          >? Se enviará un correo notificando la decisión con sus credenciales de acceso.
        </p>

        <div class="bg-surface-container-low rounded-lg p-3 flex gap-3 items-center">
          <span class="material-symbols-outlined text-on-surface-variant text-[20px]">mail</span>
          <span class="text-xs text-on-surface-variant truncate">
            Notificación a: {{ selectedStudent?.correo }}
          </span>
        </div>
      </div>

      <template #footer>
        <BaseButton
          variant="outline"
          size="md"
          :disabled="isProcessingAction"
          @click="isApproveModalOpen = false"
        >
          Cancelar
        </BaseButton>
        <BaseButton
          variant="primary"
          size="md"
          :loading="isProcessingAction"
          class="!bg-[#28a745] hover:!bg-[#218838]"
          @click="confirmApprove"
        >
          Aceptar Estudiante
        </BaseButton>
      </template>
    </BaseModal>

    <BaseModal v-model="isRejectModalOpen" title="Confirmar Rechazo" max-width="md">
      <div class="flex flex-col gap-4">
        <div
          class="w-12 h-12 rounded-full bg-error-container text-on-error-container flex items-center justify-center"
        >
          <span class="material-symbols-outlined text-[26px]">cancel</span>
        </div>

        <p class="text-sm text-on-surface-variant font-body">
          ¿Estás seguro de que deseas rechazar la solicitud de
          <strong class="text-on-surface font-semibold">
            {{ selectedStudent?.nombre }} {{ selectedStudent?.apellido }} </strong
          >? Esta acción no se puede deshacer.
        </p>
      </div>

      <template #footer>
        <BaseButton
          variant="outline"
          size="md"
          :disabled="isProcessingAction"
          @click="isRejectModalOpen = false"
        >
          Cancelar
        </BaseButton>
        <BaseButton variant="danger" size="md" :loading="isProcessingAction" @click="confirmReject">
          Rechazar Solicitud
        </BaseButton>
      </template>
    </BaseModal>
  </div>
</template>
