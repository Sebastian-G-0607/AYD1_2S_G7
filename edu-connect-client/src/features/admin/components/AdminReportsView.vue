<script setup lang="ts">
import { ref } from 'vue'
import { useAdminReports } from '../composables/useAdminReports'

const {
  tutoresReport,
  materiasReport,
  cancelacionesReport,
  estudiantesReport,
  resumen,
  isLoading,
  error,
  selectedPeriod,
  selectedTab,
  searchQuery,
  maxAtencionesTutor,
  totalEstudiantesAtendidos,
  donutSegments,
  filteredTutores,
  filteredMaterias,
  filteredCancelaciones,
  filteredEstudiantes,
  loadReports,
  exportReport
} = useAdminReports()

const isPeriodDropdownOpen = ref(false)

const periodOptions = [
  { label: 'Últimos 7 días', value: '7d' },
  { label: 'Últimos 30 días', value: '30d' },
  { label: 'Últimos 90 días', value: '90d' },
  { label: 'Todo el historial', value: 'all' }
]

function selectPeriod(period: '7d' | '30d' | '90d' | 'all') {
  selectedPeriod.value = period
  isPeriodDropdownOpen.value = false
  loadReports()
}

// Colores alternados para las barras del gráfico de tutores (Stitch Design Tokens)
const barColors = [
  'bg-primary',
  'bg-primary-fixed-dim',
  'bg-tertiary',
  'bg-primary-fixed-dim',
  'bg-secondary',
  'bg-secondary-container'
]

function getBarColor(idx: number): string {
  return barColors[idx % barColors.length]
}

function getInitials(name: string): string {
  if (!name) return 'TU'
  const parts = name.trim().split(' ')
  if (parts.length >= 2) {
    return (parts[0].charAt(0) + parts[1].charAt(0)).toUpperCase()
  }
  return parts[0].substring(0, 2).toUpperCase()
}

function formatTutorName(name: string): { first: string; last: string } {
  if (!name) return { first: 'Tutor', last: '' }
  const clean = name.trim()
  const parts = clean.split(' ')
  if (parts.length === 1) return { first: parts[0], last: '' }
  if (parts[0].includes('.') && parts.length >= 3) {
    return { first: `${parts[0]} ${parts[1]}`, last: parts.slice(2).join(' ') }
  }
  return { first: parts[0], last: parts.slice(1).join(' ') }
}

const selectedMateriaSubTab = ref<'demanda' | 'cancelaciones' | 'efectividad'>('demanda')

const tabs = [
  { key: 'general' as const, label: 'General', icon: 'dashboard' },
  { key: 'tutores' as const, label: 'Tutores', icon: 'school' },
  { key: 'estudiantes' as const, label: 'Estudiantes', icon: 'groups' },
  { key: 'materias' as const, label: 'Materias y Efectividad', icon: 'auto_stories' }
]
</script>

<template>
  <div class="flex flex-col w-full gap-gutter relative">
    <!-- MENSAJE DE ERROR -->
    <div
      v-if="error"
      class="bg-error-container text-on-error-container p-4 rounded-xl flex items-center justify-between shadow-sm"
    >
      <div class="flex items-center gap-3">
        <span class="material-symbols-outlined text-[24px]">error</span>
        <span class="font-body-md text-body-md">{{ error }}</span>
      </div>
      <button
        type="button"
        class="px-4 py-1.5 bg-error text-on-error rounded-lg font-label-md text-label-md hover:opacity-90 transition-opacity"
        @click="loadReports"
      >
        Reintentar
      </button>
    </div>

    <!-- ENCABEZADO DE PÁGINA (STITCH TEMPLATE) -->
    <div class="flex flex-col lg:flex-row justify-between items-start lg:items-end gap-6 mb-4">
      <div>
        <h1
          class="font-headline-lg text-headline-lg text-on-surface mb-2"
          style="
            font-size: 32px;
            font-weight: 700;
            letter-spacing: -0.02em;
            line-height: 1.2;
            color: rgb(30, 41, 59);
          "
        >
          Visión General Académica
        </h1>
        <p class="font-body-md text-body-md text-on-surface-variant">
          Métricas clave y rendimiento del sistema de tutorías EduConnect.
        </p>
      </div>

      <div
        class="flex flex-wrap items-center gap-4 bg-surface-container-lowest p-2 rounded-xl shadow-sm"
      >
        <!-- Selector de Período -->
        <div class="relative">
          <button
            type="button"
            class="flex items-center gap-2 px-3 py-2 bg-surface-container-low rounded-lg hover:bg-surface-container transition-colors cursor-pointer group"
            @click="isPeriodDropdownOpen = !isPeriodDropdownOpen"
          >
            <span
              class="material-symbols-outlined text-on-surface-variant group-hover:text-primary transition-colors text-[20px]"
            >
              calendar_month
            </span>
            <span class="font-label-md text-label-md text-on-surface">
              {{ periodOptions.find(p => p.value === selectedPeriod)?.label || 'Últimos 30 días' }}
            </span>
            <span
              class="material-symbols-outlined text-on-surface-variant text-[18px] transition-transform"
              :class="{ 'rotate-180': isPeriodDropdownOpen }"
            >
              arrow_drop_down
            </span>
          </button>

          <!-- Dropdown opciones -->
          <div
            v-if="isPeriodDropdownOpen"
            class="absolute right-0 top-full mt-2 w-48 bg-surface-container-lowest rounded-xl shadow-lg border border-surface-container-high py-2 z-50"
          >
            <button
              v-for="opt in periodOptions"
              :key="opt.value"
              type="button"
              class="w-full text-left px-4 py-2 text-body-sm hover:bg-surface-container-low transition-colors flex items-center justify-between"
              :class="
                selectedPeriod === opt.value
                  ? 'text-primary font-semibold bg-surface-container-low'
                  : 'text-on-surface'
              "
              @click="selectPeriod(opt.value as any)"
            >
              {{ opt.label }}
              <span
                v-if="selectedPeriod === opt.value"
                class="material-symbols-outlined text-[18px] text-primary"
                >check</span
              >
            </button>
          </div>
        </div>

        <!-- Botón Refrescar -->
        <button
          type="button"
          aria-label="Actualizar datos"
          title="Actualizar datos"
          class="p-2 rounded-lg bg-surface-container-low text-on-surface-variant hover:text-primary hover:bg-surface-container transition-colors"
          :disabled="isLoading"
          @click="loadReports"
        >
          <span
            class="material-symbols-outlined text-[20px]"
            :class="{ 'animate-spin': isLoading }"
          >
            refresh
          </span>
        </button>

        <div class="h-6 w-px bg-surface-container-high hidden md:block"></div>

        <!-- Botón Exportar -->
        <button
          type="button"
          class="flex items-center gap-2 px-4 py-2 bg-primary text-on-primary rounded-lg hover:bg-tertiary transition-all shadow-sm hover:shadow-md cursor-pointer"
          @click="exportReport"
        >
          <span class="material-symbols-outlined text-[18px]">download</span>
          <span class="font-label-md text-label-md">Exportar Reporte</span>
        </button>
      </div>
    </div>

    <!-- KPI BENTO GRID (STITCH TEMPLATE) -->
    <div class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-4 gap-4 xl:gap-6">
      <!-- Card 1: Total Estudiantes Únicos Atendidos -->
      <div
        class="bg-surface-container-lowest rounded-2xl p-6 shadow-[0_8px_24px_rgba(15,23,42,0.08)] hover:shadow-[0_12px_32px_rgba(15,23,42,0.12)] border border-surface-container-high/40 hover:-translate-y-1 transition-all duration-300 relative overflow-hidden group"
      >
        <div
          class="absolute -right-6 -top-6 w-24 h-24 bg-primary-container opacity-20 rounded-full group-hover:scale-150 transition-transform duration-500"
        ></div>
        <div class="flex justify-between items-start mb-4 relative z-10">
          <div class="w-12 h-12 bg-primary-container rounded-xl flex items-center justify-center">
            <span
              class="material-symbols-outlined text-on-primary-container text-[24px]"
              style="font-variation-settings: 'FILL' 1"
            >
              groups
            </span>
          </div>
          <div
            class="px-2 py-1 bg-surface-container-low rounded text-on-surface font-label-sm text-label-sm flex items-center gap-1"
          >
            <span class="material-symbols-outlined text-[14px] text-on-surface-variant"
              >trending_up</span
            >
            Activo
          </div>
        </div>
        <div class="relative z-10">
          <p
            class="font-label-md text-label-md text-on-surface-variant uppercase tracking-wider mb-1"
          >
            Estudiantes Atendidos
          </p>
          <p class="font-display-lg text-display-lg text-on-surface">
            {{ totalEstudiantesAtendidos.toLocaleString() }}
          </p>
        </div>
      </div>

      <!-- Card 2: Tutores Activos con Atenciones -->
      <div
        class="bg-surface-container-lowest rounded-2xl p-6 shadow-[0_8px_24px_rgba(15,23,42,0.08)] hover:shadow-[0_12px_32px_rgba(15,23,42,0.12)] border border-surface-container-high/40 hover:-translate-y-1 transition-all duration-300 relative overflow-hidden group"
      >
        <div
          class="absolute -right-6 -top-6 w-24 h-24 bg-secondary-container opacity-20 rounded-full group-hover:scale-150 transition-transform duration-500"
        ></div>
        <div class="flex justify-between items-start mb-4 relative z-10">
          <div class="w-12 h-12 bg-secondary-container rounded-xl flex items-center justify-center">
            <span
              class="material-symbols-outlined text-on-secondary-container text-[24px]"
              style="font-variation-settings: 'FILL' 1"
            >
              badge
            </span>
          </div>
          <div
            class="px-2 py-1 bg-surface-container-low rounded text-on-surface font-label-sm text-label-sm flex items-center gap-1"
          >
            <span class="material-symbols-outlined text-[14px] text-on-surface-variant"
              >trending_up</span
            >
            Top
          </div>
        </div>
        <div class="relative z-10">
          <p
            class="font-label-md text-label-md text-on-surface-variant uppercase tracking-wider mb-1"
          >
            Tutores Destacados
          </p>
          <p class="font-display-lg text-display-lg text-on-surface">
            {{ tutoresReport.length }}
          </p>
        </div>
      </div>

      <!-- Card 3: Sesiones Atendidas -->
      <div
        class="bg-surface-container-lowest rounded-2xl p-6 shadow-[0_8px_24px_rgba(15,23,42,0.08)] hover:shadow-[0_12px_32px_rgba(15,23,42,0.12)] border border-surface-container-high/40 hover:-translate-y-1 transition-all duration-300 relative overflow-hidden group"
      >
        <div
          class="absolute -right-6 -top-6 w-24 h-24 bg-tertiary-container opacity-20 rounded-full group-hover:scale-150 transition-transform duration-500"
        ></div>
        <div class="flex justify-between items-start mb-4 relative z-10">
          <div class="w-12 h-12 bg-tertiary-container rounded-xl flex items-center justify-center">
            <span
              class="material-symbols-outlined text-on-tertiary-container text-[24px]"
              style="font-variation-settings: 'FILL' 1"
            >
              school
            </span>
          </div>
          <div
            class="px-2 py-1 bg-surface-container-low rounded text-on-surface font-label-sm text-label-sm flex items-center gap-1"
          >
            <span class="material-symbols-outlined text-[14px] text-on-surface-variant"
              >trending_up</span
            >
            {{ resumen.totalSesionesAtendidas }} / {{ resumen.totalSesiones }}
          </div>
        </div>
        <div class="relative z-10">
          <p
            class="font-label-md text-label-md text-on-surface-variant uppercase tracking-wider mb-1"
          >
            Sesiones Atendidas
          </p>
          <p class="font-display-lg text-display-lg text-on-surface">
            {{ resumen.totalSesionesAtendidas.toLocaleString() }}
          </p>
        </div>
      </div>

      <!-- Card 4: Tasa de Efectividad / Satisfacción Global -->
      <div
        class="bg-primary rounded-2xl p-6 shadow-[0_8px_24px_rgba(9,20,38,0.22)] hover:shadow-[0_12px_32px_rgba(9,20,38,0.3)] hover:-translate-y-1 transition-all duration-300 relative overflow-hidden group"
      >
        <div
          class="absolute -right-6 -bottom-6 w-32 h-32 bg-on-primary opacity-10 rounded-full group-hover:scale-150 transition-transform duration-500"
        ></div>
        <div class="flex justify-between items-start mb-4 relative z-10">
          <div
            class="w-12 h-12 bg-primary-fixed/20 rounded-xl flex items-center justify-center backdrop-blur-sm"
          >
            <span
              class="material-symbols-outlined text-on-primary text-[24px]"
              style="font-variation-settings: 'FILL' 1"
            >
              star
            </span>
          </div>
        </div>
        <div class="relative z-10 mt-auto pt-8">
          <p class="font-label-md text-label-md text-on-primary/80 uppercase tracking-wider mb-1">
            Efectividad Global
          </p>
          <div class="flex items-end gap-2">
            <p class="font-display-lg text-display-lg text-on-primary">
              {{ resumen.tasaEfectividad }}%
            </p>
            <p class="font-body-md text-body-md text-on-primary/70 mb-2">atendidas</p>
          </div>
        </div>
      </div>
    </div>

    <!-- NAVEGACIÓN POR PESTAÑAS Y BÚSQUEDA -->
    <div class="flex flex-col sm:flex-row items-stretch sm:items-center justify-between gap-4 mt-2">
      <div class="flex items-center gap-1.5 p-1.5 bg-surface-container-low rounded-2xl overflow-x-auto">
        <button
          v-for="tab in tabs"
          :key="tab.key"
          type="button"
          class="flex items-center gap-2 px-4 py-2 rounded-xl font-label-md text-label-md transition-all whitespace-nowrap cursor-pointer"
          :class="
            selectedTab === tab.key
              ? 'bg-surface-container-lowest text-primary shadow-xs font-semibold'
              : 'text-on-surface-variant hover:text-on-surface hover:bg-surface-container/60'
          "
          @click="selectedTab = tab.key"
        >
          <span class="material-symbols-outlined text-[18px]">{{ tab.icon }}</span>
          <span>{{ tab.label }}</span>
        </button>
      </div>

      <div v-if="selectedTab !== 'general'" class="relative w-full sm:w-72">
        <span
          class="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-on-surface-variant text-[18px] pointer-events-none"
        >
          search
        </span>
        <input
          v-model="searchQuery"
          type="text"
          :placeholder="
            selectedTab === 'tutores'
              ? 'Buscar tutor...'
              : selectedTab === 'estudiantes'
                ? 'Buscar estudiante...'
                : 'Buscar materia...'
          "
          class="w-full bg-surface-container-lowest text-on-surface font-body-sm text-body-sm rounded-xl pl-9 pr-8 py-2.5 border border-surface-container-high/60 focus:outline-none focus:ring-2 focus:ring-primary focus:border-transparent transition-all shadow-xs"
        />
        <button
          v-if="searchQuery"
          type="button"
          class="absolute right-2.5 top-1/2 -translate-y-1/2 text-on-surface-variant hover:text-on-surface cursor-pointer"
          @click="searchQuery = ''"
        >
          <span class="material-symbols-outlined text-[16px]">close</span>
        </button>
      </div>
    </div>

    <!-- PESTAÑA 1: GENERAL -->
    <div v-if="selectedTab === 'general'" class="space-y-6">
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6 lg:gap-8">
        <!-- BAR CHART: Tutores con más estudiantes -->
        <div
          class="lg:col-span-2 bg-surface-container-lowest rounded-2xl p-6 shadow-[0_8px_24px_rgba(15,23,42,0.08)] border border-surface-container-high/40 flex flex-col justify-between"
        >
          <div class="flex justify-between items-center mb-6">
            <div>
              <h2 class="font-headline-md text-headline-md text-on-surface">
                Tutores con más estudiantes
              </h2>
              <p class="font-body-sm text-body-sm text-on-surface-variant mt-1">
                Volumen de atención en el periodo seleccionado (conteo de sesiones ATENDIDAS)
              </p>
            </div>
            <button
              type="button"
              class="flex items-center gap-1 px-3 py-1.5 rounded-lg bg-surface-container-low hover:bg-surface-container text-on-surface-variant hover:text-primary transition-colors font-label-sm text-label-sm cursor-pointer"
              @click="selectedTab = 'tutores'"
            >
              <span>Ver tabla</span>
              <span class="material-symbols-outlined text-[16px]">arrow_forward</span>
            </button>
          </div>

          <!-- Gráfico de Barras -->
          <div
            v-if="tutoresReport.length > 0"
            class="w-full h-[340px] relative flex items-end justify-between px-4 pb-16 pt-4"
          >
            <div class="absolute inset-0 flex flex-col justify-between pb-16 pointer-events-none">
              <div class="w-full h-px bg-surface-container-high"></div>
              <div class="w-full h-px bg-surface-container-high"></div>
              <div class="w-full h-px bg-surface-container-high"></div>
              <div class="w-full h-px bg-surface-container-high"></div>
              <div class="w-full h-px bg-surface-container-high"></div>
            </div>

            <div
              v-for="(tutor, idx) in tutoresReport.slice(0, 6)"
              :key="tutor.tutorId"
              class="relative z-10 w-10 sm:w-12 rounded-t-lg mx-auto flex flex-col justify-end group transition-all cursor-pointer"
              :class="getBarColor(idx)"
              :style="{
                height: `${Math.max(12, Math.round((tutor.totalSesionesAtendidas / maxAtencionesTutor) * 88))}%`
              }"
            >
              <div
                class="opacity-0 group-hover:opacity-100 absolute -top-12 left-1/2 -translate-x-1/2 bg-inverse-surface text-inverse-on-surface px-3 py-1 rounded-lg font-label-sm text-label-sm whitespace-nowrap transition-opacity shadow-lg z-30 pointer-events-none"
              >
                <div class="font-semibold">{{ tutor.totalSesionesAtendidas }} sesiones</div>
                <div class="text-[11px] text-inverse-on-surface/80">
                  {{ tutor.totalEstudiantesAtendidos }} estudiantes
                </div>
              </div>

              <div
                class="absolute -bottom-14 left-1/2 -translate-x-1/2 flex flex-col items-center justify-start text-center w-20 sm:w-24 pointer-events-none"
                :title="tutor.nombreCompleto"
              >
                <span class="text-[11px] font-semibold text-on-surface leading-tight truncate w-full">
                  {{ formatTutorName(tutor.nombreCompleto).first }}
                </span>
                <span class="text-[10px] text-on-surface-variant leading-tight truncate w-full">
                  {{ formatTutorName(tutor.nombreCompleto).last }}
                </span>
              </div>
            </div>
          </div>

          <div
            v-else
            class="h-[340px] flex flex-col items-center justify-center text-center p-6 border-2 border-dashed border-surface-container-high rounded-xl"
          >
            <span class="material-symbols-outlined text-[48px] text-on-surface-variant mb-2">person_off</span>
            <p class="font-label-md text-label-md text-on-surface">
              No hay registros de atenciones completadas
            </p>
            <p class="font-body-sm text-body-sm text-on-surface-variant">
              Las sesiones deben estar en estado ATENDIDA para figurar en este reporte.
            </p>
          </div>
        </div>

        <!-- DONUT CHART: Materias con mayor demanda -->
        <div
          class="bg-surface-container-lowest rounded-2xl p-6 shadow-[0_8px_24px_rgba(15,23,42,0.08)] border border-surface-container-high/40 flex flex-col"
        >
          <div class="flex justify-between items-start mb-1">
            <div>
              <h2 class="font-headline-md text-headline-md text-on-surface">
                Materias con mayor demanda
              </h2>
              <p class="font-body-sm text-body-sm text-on-surface-variant mb-4">
                Distribución por solicitudes y sesiones
              </p>
            </div>
            <button
              type="button"
              class="flex items-center gap-1 px-3 py-1.5 rounded-lg bg-surface-container-low hover:bg-surface-container text-on-surface-variant hover:text-primary transition-colors font-label-sm text-label-sm cursor-pointer"
              @click="selectedTab = 'materias'"
            >
              <span>Ver tabla</span>
              <span class="material-symbols-outlined text-[16px]">arrow_forward</span>
            </button>
          </div>

          <div class="relative w-48 h-48 mx-auto mb-6 shrink-0">
            <svg class="w-full h-full transform -rotate-90" viewBox="0 0 100 100">
              <circle
                class="stroke-surface-container-high"
                cx="50"
                cy="50"
                fill="transparent"
                r="40"
                stroke-width="20"
              />
              <circle
                v-for="seg in donutSegments"
                :key="seg.materiaId"
                :class="seg.strokeClass"
                cx="50"
                cy="50"
                fill="transparent"
                r="40"
                stroke-width="20"
                :stroke-dasharray="seg.dashArray"
                :stroke-dashoffset="seg.dashOffset"
              />
            </svg>

            <div
              class="absolute inset-0 flex flex-col items-center justify-center pointer-events-none"
            >
              <span class="font-headline-md text-headline-md text-on-surface">
                {{
                  resumen.totalSesiones > 999
                    ? (resumen.totalSesiones / 1000).toFixed(1) + 'k'
                    : resumen.totalSesiones
                }}
              </span>
              <span class="font-label-sm text-label-sm text-on-surface-variant">Sesiones</span>
            </div>
          </div>

          <div class="space-y-2 mt-auto">
            <div
              v-for="seg in donutSegments.slice(0, 4)"
              :key="seg.materiaId"
              class="flex items-center justify-between p-2 rounded-lg hover:bg-surface-container transition-colors cursor-pointer group"
              @click="selectedTab = 'materias'"
            >
              <div class="flex items-center gap-3 min-w-0">
                <div class="w-3 h-3 rounded-full shrink-0" :class="seg.bgClass"></div>
                <span
                  class="font-label-md text-label-md text-on-surface truncate group-hover:text-primary"
                >
                  {{ seg.nombreMateria }}
                </span>
              </div>
              <div class="flex items-center gap-2 shrink-0">
                <span class="font-body-sm text-body-sm text-on-surface-variant font-semibold">
                  {{ seg.porcentajeDemanda }}%
                </span>
                <span class="text-[11px] text-on-surface-variant/70">({{ seg.totalSesiones }})</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Resumen Ejecutivo de Acceso Rápido -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4 xl:gap-6">
        <div
          class="bg-surface-container-lowest p-5 rounded-2xl border border-surface-container-high/40 shadow-xs flex items-center justify-between"
        >
          <div class="flex items-center gap-4">
            <div class="w-11 h-11 rounded-xl bg-primary-container text-on-primary-container flex items-center justify-center shrink-0">
              <span class="material-symbols-outlined text-[24px]">school</span>
            </div>
            <div class="min-w-0">
              <p class="font-label-sm text-label-sm text-on-surface-variant uppercase tracking-wider">Top Tutor</p>
              <p class="font-label-lg text-label-lg font-semibold text-on-surface truncate">{{ resumen.tutorTopNombre || 'Sin registros' }}</p>
              <p class="text-[12px] text-on-surface-variant">{{ resumen.tutorTopAtenciones }} sesiones</p>
            </div>
          </div>
          <button
            type="button"
            class="p-2 rounded-lg hover:bg-surface-container text-on-surface-variant hover:text-primary transition-colors cursor-pointer"
            @click="selectedTab = 'tutores'"
          >
            <span class="material-symbols-outlined text-[20px]">chevron_right</span>
          </button>
        </div>

        <div
          class="bg-surface-container-lowest p-5 rounded-2xl border border-surface-container-high/40 shadow-xs flex items-center justify-between"
        >
          <div class="flex items-center gap-4">
            <div class="w-11 h-11 rounded-xl bg-secondary-container text-on-secondary-container flex items-center justify-center shrink-0">
              <span class="material-symbols-outlined text-[24px]">auto_stories</span>
            </div>
            <div class="min-w-0">
              <p class="font-label-sm text-label-sm text-on-surface-variant uppercase tracking-wider">Materia Líder</p>
              <p class="font-label-lg text-label-lg font-semibold text-on-surface truncate">{{ resumen.materiaTopNombre || 'Sin registros' }}</p>
              <p class="text-[12px] text-on-surface-variant">{{ resumen.materiaTopSesiones }} sesiones</p>
            </div>
          </div>
          <button
            type="button"
            class="p-2 rounded-lg hover:bg-surface-container text-on-surface-variant hover:text-secondary transition-colors cursor-pointer"
            @click="selectedTab = 'materias'"
          >
            <span class="material-symbols-outlined text-[20px]">chevron_right</span>
          </button>
        </div>

        <div
          class="bg-surface-container-lowest p-5 rounded-2xl border border-surface-container-high/40 shadow-xs flex items-center justify-between"
        >
          <div class="flex items-center gap-4">
            <div class="w-11 h-11 rounded-xl bg-tertiary-container text-on-tertiary-container flex items-center justify-center shrink-0">
              <span class="material-symbols-outlined text-[24px]">groups</span>
            </div>
            <div class="min-w-0">
              <p class="font-label-sm text-label-sm text-on-surface-variant uppercase tracking-wider">Participación</p>
              <p class="font-label-lg text-label-lg font-semibold text-on-surface truncate">{{ totalEstudiantesAtendidos.toLocaleString() }} Estudiantes</p>
              <p class="text-[12px] text-on-surface-variant">Con tutorías completadas</p>
            </div>
          </div>
          <button
            type="button"
            class="p-2 rounded-lg hover:bg-surface-container text-on-surface-variant hover:text-tertiary transition-colors cursor-pointer"
            @click="selectedTab = 'estudiantes'"
          >
            <span class="material-symbols-outlined text-[20px]">chevron_right</span>
          </button>
        </div>
      </div>
    </div>

    <!-- PESTAÑA 2: TUTORES -->
    <div
      v-else-if="selectedTab === 'tutores'"
      class="bg-surface-container-lowest rounded-2xl shadow-[0_8px_24px_rgba(15,23,42,0.08)] border border-surface-container-high/40 p-6"
    >
      <div class="flex items-center justify-between mb-4">
        <div class="flex items-center gap-2">
          <span class="material-symbols-outlined text-primary text-[22px]">workspace_premium</span>
          <div>
            <h3 class="font-headline-md text-headline-md text-on-surface text-[18px]">
              Top 10 tutores por estudiantes atendidos
            </h3>
            <p class="font-body-sm text-body-sm text-on-surface-variant">
              Ranking ordenado por volumen de sesiones completadas y estudiantes únicos atendidos
            </p>
          </div>
        </div>
        <span class="font-label-sm text-label-sm text-on-surface-variant bg-surface-container-low px-3 py-1 rounded-full">
          {{ filteredTutores.length }} tutores registrados
        </span>
      </div>

      <div class="overflow-x-auto overflow-y-auto max-h-[360px] rounded-xl border border-surface-container-high">
        <table class="w-full text-left border-collapse">
          <thead class="sticky top-0 z-10 bg-surface-container-low shadow-xs">
            <tr class="text-on-surface-variant font-label-sm text-label-sm border-b border-surface-container-high">
              <th class="py-3.5 px-4 font-semibold w-16 text-center">Pos.</th>
              <th class="py-3.5 px-4 font-semibold">Tutor Académico</th>
              <th class="py-3.5 px-4 font-semibold">Carnet / ID</th>
              <th class="py-3.5 px-4 font-semibold">Correo Electrónico</th>
              <th class="py-3.5 px-4 font-semibold text-center">Sesiones Atendidas</th>
              <th class="py-3.5 px-4 font-semibold text-center">Estudiantes Únicos</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-surface-container-high font-body-sm text-body-sm">
            <tr
              v-for="(tutor, idx) in filteredTutores"
              :key="tutor.tutorId"
              class="hover:bg-surface-container-low/60 transition-colors"
            >
              <td class="py-3 px-4 text-center">
                <span
                  class="inline-flex items-center justify-center w-7 h-7 rounded-full font-bold text-xs"
                  :class="[
                    idx === 0
                      ? 'bg-amber-100 text-amber-800'
                      : idx === 1
                        ? 'bg-slate-200 text-slate-800'
                        : idx === 2
                          ? 'bg-orange-100 text-orange-800'
                          : 'bg-surface-container text-on-surface-variant'
                  ]"
                >
                  #{{ idx + 1 }}
                </span>
              </td>

              <td class="py-3 px-4">
                <div class="flex items-center gap-3">
                  <div
                    class="w-9 h-9 rounded-full bg-primary-container text-on-primary-container flex items-center justify-center font-bold text-xs overflow-hidden shrink-0"
                  >
                    <img
                      v-if="tutor.fotografiaUrl"
                      :src="tutor.fotografiaUrl"
                      :alt="tutor.nombreCompleto"
                      class="w-full h-full object-cover"
                    />
                    <span v-else>{{ getInitials(tutor.nombreCompleto) }}</span>
                  </div>
                  <div>
                    <p class="font-semibold text-on-surface">{{ tutor.nombreCompleto }}</p>
                    <p class="text-[12px] text-on-surface-variant">Tutor Activo</p>
                  </div>
                </div>
              </td>

              <td class="py-3 px-4 text-on-surface font-mono text-[13px]">
                {{ tutor.carnet }}
              </td>

              <td class="py-3 px-4 text-on-surface-variant">
                {{ tutor.correo }}
              </td>

              <td class="py-3 px-4 text-center">
                <span class="inline-flex items-center px-2.5 py-1 rounded-full text-xs font-semibold bg-primary/10 text-primary">
                  {{ tutor.totalSesionesAtendidas }} atendidas
                </span>
              </td>

              <td class="py-3 px-4 text-center font-semibold text-secondary">
                {{ tutor.totalEstudiantesAtendidos }}
              </td>
            </tr>

            <tr v-if="filteredTutores.length === 0">
              <td colspan="6" class="py-8 text-center text-on-surface-variant">
                No se encontraron tutores que coincidan con la búsqueda.
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- PESTAÑA 3: ESTUDIANTES -->
    <div
      v-else-if="selectedTab === 'estudiantes'"
      class="bg-surface-container-lowest rounded-2xl shadow-[0_8px_24px_rgba(15,23,42,0.08)] border border-surface-container-high/40 p-6"
    >
      <div class="flex items-center justify-between mb-4">
        <div class="flex items-center gap-2">
          <span class="material-symbols-outlined text-primary text-[22px]">groups</span>
          <div>
            <h3 class="font-headline-md text-headline-md text-on-surface text-[18px]">
              Top 10 estudiantes con más sesiones programadas
            </h3>
            <p class="font-body-sm text-body-sm text-on-surface-variant">
              Ranking ordenado por volumen de sesiones reservadas en el sistema
            </p>
          </div>
        </div>
        <span class="font-label-sm text-label-sm text-on-surface-variant bg-surface-container-low px-3 py-1 rounded-full">
          {{ filteredEstudiantes.length }} estudiantes registrados
        </span>
      </div>

      <div class="overflow-x-auto overflow-y-auto max-h-[360px] rounded-xl border border-surface-container-high">
        <table class="w-full text-left border-collapse">
          <thead class="sticky top-0 z-10 bg-surface-container-low shadow-xs">
            <tr class="text-on-surface-variant font-label-sm text-label-sm border-b border-surface-container-high">
              <th class="py-3.5 px-4 font-semibold w-16 text-center">Pos.</th>
              <th class="py-3.5 px-4 font-semibold">Estudiante</th>
              <th class="py-3.5 px-4 font-semibold">Carnet / ID</th>
              <th class="py-3.5 px-4 font-semibold">Correo Electrónico</th>
              <th class="py-3.5 px-4 font-semibold text-center">Sesiones Programadas</th>
              <th class="py-3.5 px-4 font-semibold text-center">Atendidas</th>
              <th class="py-3.5 px-4 font-semibold text-center">Canceladas</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-surface-container-high font-body-sm text-body-sm">
            <tr
              v-for="(estudiante, idx) in filteredEstudiantes"
              :key="estudiante.estudianteId"
              class="hover:bg-surface-container-low/60 transition-colors"
            >
              <td class="py-3 px-4 text-center">
                <span
                  class="inline-flex items-center justify-center w-7 h-7 rounded-full font-bold text-xs"
                  :class="[
                    idx === 0
                      ? 'bg-amber-100 text-amber-800'
                      : idx === 1
                        ? 'bg-slate-200 text-slate-800'
                        : idx === 2
                          ? 'bg-orange-100 text-orange-800'
                          : 'bg-surface-container text-on-surface-variant'
                  ]"
                >
                  #{{ idx + 1 }}
                </span>
              </td>

              <td class="py-3 px-4">
                <div class="flex items-center gap-3">
                  <div
                    class="w-9 h-9 rounded-full bg-secondary-container text-on-secondary-container flex items-center justify-center font-bold text-xs overflow-hidden shrink-0"
                  >
                    <img
                      v-if="estudiante.fotografiaUrl"
                      :src="estudiante.fotografiaUrl"
                      :alt="estudiante.nombreCompleto"
                      class="w-full h-full object-cover"
                    />
                    <span v-else>{{ getInitials(estudiante.nombreCompleto) }}</span>
                  </div>
                  <div>
                    <p class="font-semibold text-on-surface">{{ estudiante.nombreCompleto }}</p>
                    <p class="text-[12px] text-on-surface-variant">Estudiante Activo</p>
                  </div>
                </div>
              </td>

              <td class="py-3 px-4 text-on-surface font-mono text-[13px]">
                {{ estudiante.carnet }}
              </td>

              <td class="py-3 px-4 text-on-surface-variant">
                {{ estudiante.correo }}
              </td>

              <td class="py-3 px-4 text-center">
                <span class="inline-flex items-center px-2.5 py-1 rounded-full text-xs font-semibold bg-primary/10 text-primary">
                  {{ estudiante.totalSesionesProgramadas }}
                </span>
              </td>

              <td class="py-3 px-4 text-center font-semibold text-emerald-700">
                {{ estudiante.sesionesAtendidas }}
              </td>

              <td class="py-3 px-4 text-center font-semibold text-amber-700">
                {{ estudiante.sesionesCanceladas }}
              </td>
            </tr>

            <tr v-if="filteredEstudiantes.length === 0">
              <td colspan="7" class="py-8 text-center text-on-surface-variant">
                No se encontraron estudiantes que coincidan con la búsqueda.
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- PESTAÑA 4: MATERIAS Y EFECTIVIDAD -->
    <div
      v-else-if="selectedTab === 'materias'"
      class="bg-surface-container-lowest rounded-2xl shadow-[0_8px_24px_rgba(15,23,42,0.08)] border border-surface-container-high/40 p-6"
    >
      <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4 border-b border-surface-container-high pb-4 mb-6">
        <div class="flex items-center gap-2 overflow-x-auto">
          <button
            type="button"
            class="font-label-sm text-label-sm px-4 py-2 rounded-full transition-all cursor-pointer whitespace-nowrap"
            :class="
              selectedMateriaSubTab === 'demanda'
                ? 'bg-primary text-on-primary shadow-xs font-semibold'
                : 'bg-surface-container-low text-on-surface-variant hover:bg-surface-container hover:text-on-surface'
            "
            @click="selectedMateriaSubTab = 'demanda'"
          >
            Mayor Demanda
          </button>
          <button
            type="button"
            class="font-label-sm text-label-sm px-4 py-2 rounded-full transition-all cursor-pointer whitespace-nowrap"
            :class="
              selectedMateriaSubTab === 'cancelaciones'
                ? 'bg-primary text-on-primary shadow-xs font-semibold'
                : 'bg-surface-container-low text-on-surface-variant hover:bg-surface-container hover:text-on-surface'
            "
            @click="selectedMateriaSubTab = 'cancelaciones'"
          >
            Tasa de Cancelación
          </button>
          <button
            type="button"
            class="font-label-sm text-label-sm px-4 py-2 rounded-full transition-all cursor-pointer whitespace-nowrap flex items-center gap-1.5"
            :class="
              selectedMateriaSubTab === 'efectividad'
                ? 'bg-primary text-on-primary shadow-xs font-semibold'
                : 'bg-surface-container-low text-on-surface-variant hover:bg-surface-container hover:text-on-surface'
            "
            @click="selectedMateriaSubTab = 'efectividad'"
          >
            <span>Asistencia vs. Cancelación</span>
            <span class="text-[10px] uppercase font-bold px-1.5 py-0.5 rounded-full" :class="selectedMateriaSubTab === 'efectividad' ? 'bg-on-primary/20 text-on-primary' : 'bg-surface-container-high text-on-surface-variant'">Próx.</span>
          </button>
        </div>
      </div>

      <!-- Sub-pestaña 1: Mayor Demanda -->
      <div v-if="selectedMateriaSubTab === 'demanda'">
        <div class="flex items-center justify-between mb-4">
          <div class="flex items-center gap-2">
            <span class="material-symbols-outlined text-secondary text-[22px]">analytics</span>
            <div>
              <h3 class="font-headline-md text-headline-md text-on-surface text-[18px]">
                Top 10 materias con mayor demanda
              </h3>
              <p class="font-body-sm text-body-sm text-on-surface-variant">
                Distribución por solicitudes acumuladas y cumplimiento de sesiones
              </p>
            </div>
          </div>
          <span class="font-label-sm text-label-sm text-on-surface-variant bg-surface-container-low px-3 py-1 rounded-full">
            {{ filteredMaterias.length }} materias registradas
          </span>
        </div>

        <div class="overflow-x-auto overflow-y-auto max-h-[300px] rounded-xl border border-surface-container-high">
          <table class="w-full text-left border-collapse">
            <thead class="sticky top-0 z-10 bg-surface-container-low shadow-xs">
              <tr class="text-on-surface-variant font-label-sm text-label-sm border-b border-surface-container-high">
                <th class="py-3.5 px-4 font-semibold w-16 text-center">Pos.</th>
                <th class="py-3.5 px-4 font-semibold">Materia / Asignatura</th>
                <th class="py-3.5 px-4 font-semibold text-center">Total Sesiones</th>
                <th class="py-3.5 px-4 font-semibold text-center">Atendidas</th>
                <th class="py-3.5 px-4 font-semibold text-center">Pendientes</th>
                <th class="py-3.5 px-4 font-semibold">Participación en la Demanda</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-surface-container-high font-body-sm text-body-sm">
              <tr
                v-for="(materia, idx) in filteredMaterias"
                :key="materia.materiaId"
                class="hover:bg-surface-container-low/60 transition-colors"
              >
                <td class="py-3 px-4 text-center font-bold text-xs text-on-surface-variant">
                  #{{ idx + 1 }}
                </td>

                <td class="py-3 px-4 font-semibold text-on-surface">
                  {{ materia.nombreMateria }}
                </td>

                <td class="py-3 px-4 text-center font-bold text-primary">
                  {{ materia.totalSesiones }}
                </td>

                <td class="py-3 px-4 text-center text-emerald-700 font-medium">
                  {{ materia.sesionesAtendidas }}
                </td>

                <td class="py-3 px-4 text-center text-amber-700 font-medium">
                  {{ materia.sesionesPendientes }}
                </td>

                <td class="py-3 px-4">
                  <div class="flex items-center gap-3">
                    <div class="flex-1 bg-surface-container rounded-full h-2.5 overflow-hidden">
                      <div
                        class="bg-secondary h-2.5 rounded-full transition-all duration-500"
                        :style="{ width: `${materia.porcentajeDemanda}%` }"
                      />
                    </div>
                    <span class="font-label-sm text-label-sm font-semibold text-on-surface w-12 text-right">
                      {{ materia.porcentajeDemanda }}%
                    </span>
                  </div>
                </td>
              </tr>

              <tr v-if="filteredMaterias.length === 0">
                <td colspan="6" class="py-8 text-center text-on-surface-variant">
                  No se encontraron materias que coincidan con la búsqueda.
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Sub-pestaña 2: Mayor Tasa de Cancelación -->
      <div v-else-if="selectedMateriaSubTab === 'cancelaciones'">
        <div class="flex items-center justify-between mb-4">
          <div class="flex items-center gap-2">
            <span class="material-symbols-outlined text-error text-[22px]">event_busy</span>
            <div>
              <h3 class="font-headline-md text-headline-md text-on-surface text-[18px]">
                Top 10 materias con mayor tasa de cancelación
              </h3>
              <p class="font-body-sm text-body-sm text-on-surface-variant">
                Materias con mayor índice porcentual de cancelaciones sobre el total programado
              </p>
            </div>
          </div>
          <span class="font-label-sm text-label-sm text-on-surface-variant bg-surface-container-low px-3 py-1 rounded-full">
            {{ filteredCancelaciones.length }} materias registradas
          </span>
        </div>

        <div class="overflow-x-auto overflow-y-auto max-h-[300px] rounded-xl border border-surface-container-high">
          <table class="w-full text-left border-collapse">
            <thead class="sticky top-0 z-10 bg-surface-container-low shadow-xs">
              <tr class="text-on-surface-variant font-label-sm text-label-sm border-b border-surface-container-high">
                <th class="py-3.5 px-4 font-semibold w-16 text-center">Pos.</th>
                <th class="py-3.5 px-4 font-semibold">Materia / Asignatura</th>
                <th class="py-3.5 px-4 font-semibold text-center">Total Sesiones</th>
                <th class="py-3.5 px-4 font-semibold text-center">Canceladas</th>
                <th class="py-3.5 px-4 font-semibold text-center">Atendidas</th>
                <th class="py-3.5 px-4 font-semibold">Tasa de Cancelación</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-surface-container-high font-body-sm text-body-sm">
              <tr
                v-for="(item, idx) in filteredCancelaciones"
                :key="item.materiaId"
                class="hover:bg-surface-container-low/60 transition-colors"
              >
                <td class="py-3 px-4 text-center">
                  <span
                    class="inline-flex items-center justify-center w-7 h-7 rounded-full font-bold text-xs"
                    :class="[
                      idx === 0
                        ? 'bg-rose-100 text-rose-800'
                        : idx === 1
                          ? 'bg-orange-100 text-orange-800'
                          : idx === 2
                            ? 'bg-amber-100 text-amber-800'
                            : 'bg-surface-container text-on-surface-variant'
                    ]"
                  >
                    #{{ idx + 1 }}
                  </span>
                </td>

                <td class="py-3 px-4 font-semibold text-on-surface">
                  {{ item.nombreMateria }}
                </td>

                <td class="py-3 px-4 text-center font-medium text-on-surface">
                  {{ item.totalSesiones }}
                </td>

                <td class="py-3 px-4 text-center font-bold text-rose-600">
                  {{ item.sesionesCanceladas }}
                </td>

                <td class="py-3 px-4 text-center text-emerald-700 font-medium">
                  {{ item.sesionesAtendidas }}
                </td>

                <td class="py-3 px-4">
                  <div class="flex items-center gap-3">
                    <div class="flex-1 bg-surface-container rounded-full h-2.5 overflow-hidden">
                      <div
                        class="h-2.5 rounded-full transition-all duration-500"
                        :class="[
                          item.tasaCancelacion >= 40
                            ? 'bg-rose-600'
                            : item.tasaCancelacion >= 20
                              ? 'bg-amber-500'
                              : 'bg-slate-400'
                        ]"
                        :style="{ width: `${Math.min(100, item.tasaCancelacion)}%` }"
                      />
                    </div>
                    <span
                      class="font-label-sm text-label-sm font-semibold w-16 text-right"
                      :class="[
                        item.tasaCancelacion >= 40
                          ? 'text-rose-600 font-bold'
                          : item.tasaCancelacion >= 20
                            ? 'text-amber-600'
                            : 'text-on-surface-variant'
                      ]"
                    >
                      {{ item.tasaCancelacion }}%
                    </span>
                  </div>
                </td>
              </tr>

              <tr v-if="filteredCancelaciones.length === 0">
                <td colspan="6" class="py-8 text-center text-on-surface-variant">
                  No se encontraron materias con cancelaciones registradas.
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Sub-pestaña 3: Asistencia vs. Cancelación -->
      <div v-else-if="selectedMateriaSubTab === 'efectividad'">
        <div class="flex items-center justify-between mb-4">
          <div class="flex items-center gap-2">
            <span class="material-symbols-outlined text-secondary text-[22px]">compare_arrows</span>
            <div>
              <h3 class="font-headline-md text-headline-md text-on-surface text-[18px]">
                Tasa de asistencia vs. cancelación por materia
              </h3>
              <p class="font-body-sm text-body-sm text-on-surface-variant">
                Comparativo porcentual de efectividad y asistencia exitosa contra cancelaciones
              </p>
            </div>
          </div>
          <span class="font-label-sm text-label-sm bg-secondary/10 text-secondary px-3 py-1 rounded-full font-semibold">
            Próximamente
          </span>
        </div>

        <div class="rounded-xl border border-surface-container-high overflow-hidden bg-surface-container-low/20 p-8 flex flex-col items-center text-center">
          <div class="w-14 h-14 rounded-2xl bg-secondary-container/40 text-on-secondary-container flex items-center justify-center mb-3">
            <span class="material-symbols-outlined text-[30px]">analytics</span>
          </div>
          <h4 class="font-headline-sm text-headline-sm text-on-surface font-semibold mb-1">
            Reporte preparado para integración
          </h4>
          <p class="font-body-sm text-body-sm text-on-surface-variant max-w-md">
            La comparativa cruzada de efectividad por materia mostrará la relación directa entre asistencias y cancelaciones.
          </p>
        </div>
      </div>
    </div>
  </div>
</template>
