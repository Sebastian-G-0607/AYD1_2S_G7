<script setup lang="ts">
import { useAdminActiveUsers } from '../composables/useAdminActiveUsers'

const {
  filteredStudents,
  filteredTutors,
  filteredInactiveStudents,
  filteredInactiveTutors,
  isLoading,
  searchQuery,
  activeTopTab,
  activeSubTab,
  selectedUser,
  isBajaModalOpen,
  bajaMotivo,
  isProcessingAction,
  feedbackMessage,
  totalActiveCount,
  totalInactiveCount,
  openBajaModal,
  closeBajaModal,
  confirmBaja,
  dismissFeedback,
  getInitials,
  formatFecha
} = useAdminActiveUsers()
</script>

<template>
  <div class="flex flex-col w-full font-body-md text-on-surface relative">
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
        aria-label="Cerrar notificación"
        @click="dismissFeedback"
      >
        <span class="material-symbols-outlined text-[18px]">close</span>
      </button>
    </div>

    <!-- ENCABEZADO DE PÁGINA (STITCH TEMPLATE) -->
    <div class="flex flex-col sm:flex-row sm:items-end sm:justify-between gap-4 mb-margin-desktop">
      <div>
        <h1
          class="mb-2"
          style="
            font-family: 'Plus Jakarta Sans', sans-serif;
            font-size: 32px;
            font-weight: 700;
            letter-spacing: -0.02em;
            line-height: 1.2;
            color: rgb(30, 41, 59);
          "
        >
          Gestión de Usuarios
        </h1>
        <p class="font-body-md text-body-md text-on-surface-variant">
          Administra el acceso y roles de todos los miembros de la plataforma.
        </p>
      </div>

      <div class="relative w-full sm:w-80">
        <span
          class="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-on-surface-variant z-10 pointer-events-none text-[20px]"
        >
          search
        </span>
        <input
          v-model="searchQuery"
          class="w-full bg-surface-container-low text-on-surface font-body-sm text-body-sm rounded-lg pl-10 pr-4 py-2.5 focus:outline-none focus:ring-2 focus:ring-primary focus:bg-surface-container-lowest transition-all shadow-sm"
          placeholder="Buscar por nombre o correo..."
          type="text"
        />
        <button
          v-if="searchQuery"
          type="button"
          class="absolute right-3 top-1/2 -translate-y-1/2 text-on-surface-variant hover:text-on-surface"
          @click="searchQuery = ''"
        >
          <span class="material-symbols-outlined text-[18px]">close</span>
        </button>
      </div>
    </div>

    <!-- CONTENEDOR PRINCIPAL (STITCH TEMPLATE) -->
    <div
      class="bg-surface-container-lowest rounded-xl shadow-[0_4px_12px_rgba(30,41,59,0.05)] overflow-hidden flex flex-col min-h-[550px]"
    >
      <!-- Pestañas Superiores -->
      <div
        class="flex px-6 pt-4 bg-surface-container-lowest border-b border-surface-container-high relative z-10 gap-2"
      >
        <button
          type="button"
          class="font-label-md text-label-md pb-4 px-4 border-b-2 relative transition-colors cursor-pointer"
          :class="
            activeTopTab === 'activos'
              ? 'text-primary border-primary font-semibold'
              : 'text-on-surface-variant border-transparent hover:text-on-surface'
          "
          @click="activeTopTab = 'activos'"
        >
          Usuarios Activos
          <span class="absolute right-0 top-0 -mt-1 -mr-2 flex h-3 w-3">
            <span
              class="animate-ping absolute inline-flex h-full w-full rounded-full bg-secondary-fixed opacity-75"
            ></span>
            <span class="relative inline-flex rounded-full h-3 w-3 bg-secondary"></span>
          </span>
        </button>
        <button
          type="button"
          class="font-label-md text-label-md pb-4 px-4 border-b-2 relative transition-colors cursor-pointer"
          :class="
            activeTopTab === 'baja'
              ? 'text-primary border-primary font-semibold'
              : 'text-on-surface-variant border-transparent hover:text-on-surface'
          "
          @click="activeTopTab = 'baja'"
        >
          Usuarios Dados de Baja
          <span class="absolute right-0 top-0 -mt-1 -mr-2 flex h-3 w-3">
            <span
              class="animate-ping absolute inline-flex h-full w-full rounded-full bg-error/30 opacity-75"
            ></span>
            <span class="relative inline-flex rounded-full h-3 w-3 bg-error"></span>
          </span>
        </button>
      </div>

      <!-- Sub Pestañas (Estudiantes vs Tutores) -->
      <div
        v-if="activeTopTab === 'activos'"
        class="flex items-center px-6 py-4 bg-surface-bright border-b border-surface-container-high gap-4 flex-wrap"
      >
        <button
          type="button"
          class="font-label-sm text-label-sm px-4 py-2 rounded-full shadow-sm transition-colors cursor-pointer"
          :class="
            activeSubTab === 'estudiantes'
              ? 'bg-primary text-on-primary'
              : 'bg-surface-container-low text-on-surface-variant hover:bg-surface-container hover:text-on-surface'
          "
          @click="activeSubTab = 'estudiantes'"
        >
          Estudiantes
        </button>
        <button
          type="button"
          class="font-label-sm text-label-sm px-4 py-2 rounded-full transition-colors cursor-pointer"
          :class="
            activeSubTab === 'tutores'
              ? 'bg-primary text-on-primary shadow-sm'
              : 'bg-surface-container-low text-on-surface-variant hover:bg-surface-container hover:text-on-surface'
          "
          @click="activeSubTab = 'tutores'"
        >
          Tutores
        </button>

        <div class="ml-auto flex items-center gap-2">
          <span
            class="font-label-sm text-label-sm text-on-surface-variant uppercase tracking-wider"
          >
            Total Activos: {{ totalActiveCount }}
          </span>
        </div>
      </div>

      <div
        v-else
        class="flex items-center px-6 py-4 bg-surface-bright border-b border-surface-container-high gap-4 flex-wrap"
      >
        <button
          type="button"
          class="font-label-sm text-label-sm px-4 py-2 rounded-full shadow-sm transition-colors cursor-pointer"
          :class="
            activeSubTab === 'estudiantes'
              ? 'bg-primary text-on-primary'
              : 'bg-surface-container-low text-on-surface-variant hover:bg-surface-container hover:text-on-surface'
          "
          @click="activeSubTab = 'estudiantes'"
        >
          Estudiantes
        </button>
        <button
          type="button"
          class="font-label-sm text-label-sm px-4 py-2 rounded-full transition-colors cursor-pointer"
          :class="
            activeSubTab === 'tutores'
              ? 'bg-primary text-on-primary shadow-sm'
              : 'bg-surface-container-low text-on-surface-variant hover:bg-surface-container hover:text-on-surface'
          "
          @click="activeSubTab = 'tutores'"
        >
          Tutores
        </button>

        <div class="ml-auto flex items-center gap-2">
          <span
            class="font-label-sm text-label-sm text-on-surface-variant uppercase tracking-wider"
          >
            Total Dados de Baja: {{ totalInactiveCount }}
          </span>
        </div>
      </div>

      <!-- Tabla de Datos -->
      <div class="flex-1 bg-surface-container-lowest overflow-auto relative min-h-[350px]">
        <!-- Indicador de Carga -->
        <div
          v-if="isLoading"
          class="absolute inset-0 bg-surface-container-lowest/70 backdrop-blur-xs flex flex-col items-center justify-center z-20"
        >
          <span class="material-symbols-outlined text-primary text-[36px] animate-spin"
            >progress_activity</span
          >
          <p class="font-label-md text-label-md text-on-surface-variant mt-2">
            Cargando usuarios activos...
          </p>
        </div>

        <table v-if="activeTopTab === 'activos'" class="w-full text-left border-collapse">
          <thead>
            <tr class="bg-surface-bright sticky top-0 z-10 border-b border-surface-container-high">
              <th
                class="py-3 px-6 font-label-sm text-label-sm text-on-surface-variant uppercase tracking-wider whitespace-nowrap w-16"
              >
                Perfil
              </th>
              <th
                class="py-3 px-6 font-label-sm text-label-sm text-on-surface-variant uppercase tracking-wider whitespace-nowrap min-w-[200px]"
              >
                Nombre &amp; Correo
              </th>
              <th
                class="py-3 px-6 font-label-sm text-label-sm text-on-surface-variant uppercase tracking-wider whitespace-nowrap"
              >
                {{ activeSubTab === 'estudiantes' ? 'Carnet / Rol' : 'Especialidad / Materias' }}
              </th>
              <th
                class="py-3 px-6 font-label-sm text-label-sm text-on-surface-variant uppercase tracking-wider whitespace-nowrap"
              >
                Fecha Ingreso
              </th>
              <th
                class="py-3 px-6 font-label-sm text-label-sm text-on-surface-variant uppercase tracking-wider whitespace-nowrap text-right"
              >
                Acciones
              </th>
            </tr>
          </thead>

          <!-- VISTA DE ESTUDIANTES -->
          <tbody v-if="activeSubTab === 'estudiantes'" class="text-body-sm font-body-sm">
            <tr
              v-for="student in filteredStudents"
              :key="student.id"
              class="border-b border-surface-container hover:bg-surface-container-low/50 transition-colors group"
            >
              <!-- Avatar -->
              <td class="py-4 px-6">
                <div
                  class="h-10 w-10 rounded-full overflow-hidden bg-primary-fixed shadow-sm flex items-center justify-center"
                >
                  <img
                    v-if="student.fotografiaUrl"
                    class="w-full h-full object-cover"
                    :src="student.fotografiaUrl"
                    :alt="`${student.nombre} ${student.apellido}`"
                    @error="student.fotografiaUrl = undefined"
                  />
                  <span
                    v-else
                    class="font-label-md text-on-primary-fixed-variant text-xs font-bold"
                  >
                    {{ getInitials(student.nombre, student.apellido) }}
                  </span>
                </div>
              </td>

              <!-- Nombre y Correo -->
              <td class="py-4 px-6">
                <div class="flex flex-col">
                  <span class="font-label-md text-label-md text-on-surface">
                    {{ student.nombre }} {{ student.apellido }}
                  </span>
                  <span class="text-on-surface-variant text-xs mt-0.5">
                    {{ student.correo }}
                  </span>
                </div>
              </td>

              <!-- Rol / Carnet -->
              <td class="py-4 px-6">
                <span
                  class="inline-flex items-center px-2.5 py-1 rounded-md text-xs font-medium bg-primary-fixed text-on-primary-fixed-variant font-mono"
                >
                  {{ student.carnet || 'Estudiante Activo' }}
                </span>
              </td>

              <!-- Fecha Ingreso -->
              <td class="py-4 px-6 text-on-surface-variant">
                {{ formatFecha(student.fechaRegistro) }}
              </td>

              <!-- Acciones -->
              <td class="py-4 px-6 text-right">
                <button
                  type="button"
                  class="font-label-sm text-label-sm text-error hover:text-on-error-container hover:bg-error-container/50 px-3 py-1.5 rounded-md transition-all inline-flex items-center gap-1 border border-error-container cursor-pointer"
                  @click="openBajaModal(student, 'estudiante')"
                >
                  <span class="material-symbols-outlined text-[16px]">person_remove</span>
                  Dar de Baja
                </button>
              </td>
            </tr>

            <!-- Estado Vacío Estudiantes -->
            <tr v-if="!isLoading && filteredStudents.length === 0">
              <td colspan="5" class="py-12 text-center text-on-surface-variant">
                <div class="flex flex-col items-center justify-center gap-2">
                  <span class="material-symbols-outlined text-[36px] text-outline-variant"
                    >group_off</span
                  >
                  <p class="font-label-md text-label-md">No se encontraron estudiantes activos.</p>
                  <p class="text-xs text-on-surface-variant">
                    {{
                      searchQuery
                        ? 'Intenta con otro término de búsqueda.'
                        : 'Actualmente no hay estudiantes aprobados en el sistema.'
                    }}
                  </p>
                </div>
              </td>
            </tr>
          </tbody>

          <!-- VISTA DE TUTORES -->
          <tbody v-else class="text-body-sm font-body-sm">
            <tr
              v-for="tutor in filteredTutors"
              :key="tutor.id"
              class="border-b border-surface-container hover:bg-surface-container-low/50 transition-colors group"
            >
              <!-- Avatar -->
              <td class="py-4 px-6">
                <div
                  class="h-10 w-10 rounded-full overflow-hidden bg-secondary-fixed shadow-sm flex items-center justify-center text-on-secondary-fixed-variant font-label-md"
                >
                  <img
                    v-if="tutor.fotografiaUrl"
                    class="w-full h-full object-cover"
                    :src="tutor.fotografiaUrl"
                    :alt="`${tutor.nombre} ${tutor.apellido}`"
                    @error="tutor.fotografiaUrl = undefined"
                  />
                  <span
                    v-else
                    class="font-label-md text-on-secondary-fixed-variant text-xs font-bold"
                  >
                    {{ getInitials(tutor.nombre, tutor.apellido) }}
                  </span>
                </div>
              </td>

              <!-- Nombre y Correo -->
              <td class="py-4 px-6">
                <div class="flex flex-col">
                  <span class="font-label-md text-label-md text-on-surface">
                    {{ tutor.nombre }} {{ tutor.apellido }}
                  </span>
                  <span class="text-on-surface-variant text-xs mt-0.5">
                    {{ tutor.correo }}
                  </span>
                </div>
              </td>

              <!-- Especialidad / Materias -->
              <td class="py-4 px-6">
                <span
                  class="inline-flex items-center px-2.5 py-1 rounded-md text-xs font-medium bg-secondary-fixed text-on-secondary-fixed-variant max-w-[260px] truncate"
                  :title="tutor.especialidad"
                >
                  {{
                    tutor.especialidad ||
                    (tutor.materias?.length ? tutor.materias.join(', ') : 'Tutor Académico')
                  }}
                </span>
              </td>

              <!-- Fecha Ingreso -->
              <td class="py-4 px-6 text-on-surface-variant">
                {{ formatFecha(tutor.fechaRegistro) }}
              </td>

              <!-- Acciones -->
              <td class="py-4 px-6 text-right">
                <button
                  type="button"
                  class="font-label-sm text-label-sm text-error hover:text-on-error-container hover:bg-error-container/50 px-3 py-1.5 rounded-md transition-all inline-flex items-center gap-1 border border-error-container cursor-pointer"
                  @click="openBajaModal(tutor, 'tutor')"
                >
                  <span class="material-symbols-outlined text-[16px]">person_remove</span>
                  Dar de Baja
                </button>
              </td>
            </tr>

            <!-- Estado Vacío Tutores -->
            <tr v-if="!isLoading && filteredTutors.length === 0">
              <td colspan="5" class="py-12 text-center text-on-surface-variant">
                <div class="flex flex-col items-center justify-center gap-2">
                  <span class="material-symbols-outlined text-[36px] text-outline-variant"
                    >school</span
                  >
                  <p class="font-label-md text-label-md">No se encontraron tutores activos.</p>
                  <p class="text-xs text-on-surface-variant">
                    {{
                      searchQuery
                        ? 'Intenta con otro término de búsqueda.'
                        : 'Actualmente no hay tutores aprobados en el sistema.'
                    }}
                  </p>
                </div>
              </td>
            </tr>
          </tbody>
        </table>

        <table v-else class="w-full text-left border-collapse">
          <thead>
            <tr class="bg-surface-bright sticky top-0 z-10 border-b border-surface-container-high">
              <th
                class="py-3 px-6 font-label-sm text-label-sm text-on-surface-variant uppercase tracking-wider whitespace-nowrap w-16"
              >
                Perfil
              </th>
              <th
                class="py-3 px-6 font-label-sm text-label-sm text-on-surface-variant uppercase tracking-wider whitespace-nowrap min-w-[220px]"
              >
                Nombre &amp; Correo
              </th>
              <th
                class="py-3 px-6 font-label-sm text-label-sm text-on-surface-variant uppercase tracking-wider whitespace-nowrap"
              >
                Fecha Registro
              </th>
              <th
                class="py-3 px-6 font-label-sm text-label-sm text-on-surface-variant uppercase tracking-wider whitespace-nowrap"
              >
                Fecha Baja
              </th>
              <th
                class="py-3 px-6 font-label-sm text-label-sm text-on-surface-variant uppercase tracking-wider whitespace-nowrap min-w-[220px]"
              >
                Motivo de Baja
              </th>
            </tr>
          </thead>

          <tbody v-if="activeSubTab === 'estudiantes'" class="text-body-sm font-body-sm">
            <tr
              v-for="user in filteredInactiveStudents"
              :key="user.id"
              class="border-b border-surface-container hover:bg-surface-container-low/50 transition-colors group"
            >
              <td class="py-4 px-6">
                <div
                  class="h-10 w-10 rounded-full overflow-hidden bg-error-container shadow-sm flex items-center justify-center text-on-error-container font-label-md"
                >
                  <span class="font-label-md text-xs font-bold">{{
                    user.nombreCompleto
                      ? user.nombreCompleto
                          .split(' ')
                          .map(part => part[0])
                          .slice(0, 2)
                          .join('')
                          .toUpperCase()
                      : 'U'
                  }}</span>
                </div>
              </td>
              <td class="py-4 px-6">
                <div class="flex flex-col">
                  <span class="font-label-md text-label-md text-on-surface">
                    {{ user.nombreCompleto || 'Usuario sin nombre' }}
                  </span>
                  <span class="text-on-surface-variant text-xs mt-0.5">
                    {{ user.correo }}
                  </span>
                </div>
              </td>
              <td class="py-4 px-6 text-on-surface-variant">
                {{ formatFecha(user.fechaRegistro) }}
              </td>
              <td class="py-4 px-6 text-on-surface-variant">
                {{ formatFecha(user.fechaBaja) }}
              </td>
              <td class="py-4 px-6 text-on-surface-variant min-w-[220px]">
                {{ user.motivoBaja || 'Sin motivo registrado' }}
              </td>
            </tr>

            <tr v-if="!isLoading && filteredInactiveStudents.length === 0">
              <td colspan="5" class="py-12 text-center text-on-surface-variant">
                <div class="flex flex-col items-center justify-center gap-2">
                  <span class="material-symbols-outlined text-[36px] text-outline-variant"
                    >person_off</span
                  >
                  <p class="font-label-md text-label-md">
                    No se encontraron estudiantes dados de baja.
                  </p>
                </div>
              </td>
            </tr>
          </tbody>

          <tbody v-else class="text-body-sm font-body-sm">
            <tr
              v-for="user in filteredInactiveTutors"
              :key="user.id"
              class="border-b border-surface-container hover:bg-surface-container-low/50 transition-colors group"
            >
              <td class="py-4 px-6">
                <div
                  class="h-10 w-10 rounded-full overflow-hidden bg-error-container shadow-sm flex items-center justify-center text-on-error-container font-label-md"
                >
                  <span class="font-label-md text-xs font-bold">{{
                    user.nombreCompleto
                      ? user.nombreCompleto
                          .split(' ')
                          .map(part => part[0])
                          .slice(0, 2)
                          .join('')
                          .toUpperCase()
                      : 'U'
                  }}</span>
                </div>
              </td>
              <td class="py-4 px-6">
                <div class="flex flex-col">
                  <span class="font-label-md text-label-md text-on-surface">
                    {{ user.nombreCompleto || 'Usuario sin nombre' }}
                  </span>
                  <span class="text-on-surface-variant text-xs mt-0.5">
                    {{ user.correo }}
                  </span>
                </div>
              </td>
              <td class="py-4 px-6 text-on-surface-variant">
                {{ formatFecha(user.fechaRegistro) }}
              </td>
              <td class="py-4 px-6 text-on-surface-variant">
                {{ formatFecha(user.fechaBaja) }}
              </td>
              <td class="py-4 px-6 text-on-surface-variant min-w-[220px]">
                {{ user.motivoBaja || 'Sin motivo registrado' }}
              </td>
            </tr>

            <tr v-if="!isLoading && filteredInactiveTutors.length === 0">
              <td colspan="5" class="py-12 text-center text-on-surface-variant">
                <div class="flex flex-col items-center justify-center gap-2">
                  <span class="material-symbols-outlined text-[36px] text-outline-variant"
                    >school_off</span
                  >
                  <p class="font-label-md text-label-md">
                    No se encontraron tutores dados de baja.
                  </p>
                </div>
              </td>
            </tr>
          </tbody>
        </table>

        <!-- Elemento geométrico decorativo (Stitch Template) -->
        <div class="absolute bottom-4 right-4 pointer-events-none opacity-5">
          <svg
            fill="none"
            height="120"
            viewBox="0 0 120 120"
            width="120"
            xmlns="http://www.w3.org/2000/svg"
          >
            <circle
              cx="60"
              cy="60"
              r="50"
              stroke="currentColor"
              stroke-dasharray="4 8"
              stroke-width="2"
            ></circle>
            <path d="M60 20V100M20 60H100" stroke="currentColor" stroke-width="2"></path>
          </svg>
        </div>
      </div>
    </div>

    <!-- MODAL DAR DE BAJA (STITCH TEMPLATE) -->
    <div
      v-if="isBajaModalOpen"
      class="fixed inset-0 z-[100] flex items-center justify-center bg-surface/80 backdrop-blur-sm p-4"
    >
      <div
        class="bg-surface-container-lowest w-full max-w-md rounded-2xl shadow-[0_8px_32px_rgba(30,41,59,0.15)] flex flex-col border border-surface-container-high overflow-hidden transform transition-all"
        @click.stop
      >
        <!-- Modal Header -->
        <div class="bg-error-container/30 px-6 py-4 flex items-center gap-3">
          <div
            class="w-10 h-10 rounded-full bg-error text-on-error flex items-center justify-center shadow-sm"
          >
            <span class="material-symbols-outlined">warning</span>
          </div>
          <div>
            <h3 class="font-headline-md text-[20px] leading-tight text-on-error-container">
              Confirmar Baja
            </h3>
            <p class="font-label-sm text-label-sm text-on-surface-variant mt-0.5">
              {{ selectedUser?.tipo === 'estudiante' ? 'Estudiante' : 'Tutor' }}:
              {{ selectedUser?.nombre }}
            </p>
          </div>
        </div>

        <!-- Modal Body -->
        <div class="p-6 flex flex-col gap-4 bg-surface-container-lowest">
          <p class="font-body-sm text-body-sm text-on-surface">
            Esta acción revocará el acceso de este usuario a la plataforma de manera inmediata. Se
            enviará un correo electrónico notificándole la baja de su cuenta.
          </p>

          <div class="flex flex-col gap-1.5 mt-2">
            <label class="font-label-md text-label-md text-on-surface" for="motivo-baja">
              Motivo de la baja <span class="text-error">*</span>
            </label>
            <textarea
              id="motivo-baja"
              v-model="bajaMotivo"
              class="w-full bg-surface-container text-on-surface font-body-sm text-body-sm p-3 rounded-lg border border-transparent focus:border-primary focus:bg-surface-container-lowest focus:ring-1 focus:ring-primary focus:outline-none transition-all resize-none"
              placeholder="Detalle la razón por la cual se revoca el acceso..."
              rows="3"
            ></textarea>
          </div>
        </div>

        <!-- Modal Footer -->
        <div
          class="p-4 bg-surface-bright flex justify-end gap-3 border-t border-surface-container-high"
        >
          <button
            type="button"
            class="font-label-md text-label-md text-on-surface-variant hover:text-on-surface px-4 py-2 rounded-lg border border-outline-variant hover:bg-surface-container-low transition-colors cursor-pointer"
            :disabled="isProcessingAction"
            @click="closeBajaModal"
          >
            Cancelar
          </button>
          <button
            type="button"
            class="font-label-md text-label-md bg-error text-on-error hover:bg-on-error-container px-6 py-2 rounded-lg shadow-sm transition-colors flex items-center gap-2 cursor-pointer disabled:opacity-50"
            :disabled="isProcessingAction"
            @click="confirmBaja"
          >
            <span
              v-if="isProcessingAction"
              class="material-symbols-outlined animate-spin text-[18px]"
              >progress_activity</span
            >
            <span v-else class="material-symbols-outlined text-[18px]">check</span>
            Confirmar Baja
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
