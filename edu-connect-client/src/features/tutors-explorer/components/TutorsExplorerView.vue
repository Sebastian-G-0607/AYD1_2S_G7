<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useTutorsExplorer } from '../composables/useTutorsExplorer'
import TutorCard from './TutorCard.vue'
import TutorFilterSidebar from './TutorFilterSidebar.vue'
import TutorFilterModal from './TutorFilterModal.vue'

interface ActiveFilterChip {
  id: string
  label: string
  clear: () => void
}

const { tutors, isLoading, error, viewMode, filters, resetFilters, fetchTutors } =
  useTutorsExplorer()

const isDesktopFilterSidebarOpen = ref(true)
const isMobileFilterModalOpen = ref(false)

const activeFilterChips = computed<ActiveFilterChip[]>(() => {
  const list: ActiveFilterChip[] = []

  if (filters.materia) {
    list.push({
      id: 'materia',
      label: `Materia: ${filters.materia}`,
      clear: () => {
        filters.materia = ''
      }
    })
  }

  if (filters.universidad.trim()) {
    list.push({
      id: 'universidad',
      label: `Univ: ${filters.universidad.trim()}`,
      clear: () => {
        filters.universidad = ''
      }
    })
  }

  if (filters.experienciaMinima > 0) {
    list.push({
      id: 'experiencia',
      label: `Exp: ≥ ${filters.experienciaMinima} años`,
      clear: () => {
        filters.experienciaMinima = 0
      }
    })
  }

  if (filters.edadMaxima < 65) {
    list.push({
      id: 'edad',
      label: `Edad: ≤ ${filters.edadMaxima} años`,
      clear: () => {
        filters.edadMaxima = 65
      }
    })
  }

  if (filters.genero !== 'any') {
    const genderLabel = filters.genero === 'female' ? 'Femenino' : 'Masculino'
    list.push({
      id: 'genero',
      label: `Género: ${genderLabel}`,
      clear: () => {
        filters.genero = 'any'
      }
    })
  }

  return list
})

const gridClasses = computed(() => {
  if (viewMode.value === 'list') {
    return 'grid-cols-1'
  }
  if (isDesktopFilterSidebarOpen.value) {
    return 'grid-cols-1 sm:grid-cols-2 lg:grid-cols-1 xl:grid-cols-2 2xl:grid-cols-3'
  }
  return 'grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4'
})

onMounted(() => {
  fetchTutors()
})
</script>

<template>
  <div class="flex flex-col w-full relative">
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-6">
      <div class="flex flex-col gap-1.5">
        <h1 class="text-2xl sm:text-3xl font-bold font-headline text-on-surface tracking-tight">
          Explorar Tutores
        </h1>
        <p class="text-sm sm:text-base text-on-surface-variant font-body max-w-3xl">
          Encuentra al experto ideal para potenciar tu aprendizaje. Filtra por especialidad,
          experiencia y disponibilidad.
        </p>
      </div>

      <div class="flex items-center gap-2.5 self-start sm:self-auto shrink-0 flex-wrap">
        <button
          type="button"
          class="lg:hidden flex items-center gap-2 px-3.5 py-2 rounded-lg bg-surface-container-low hover:bg-surface-container text-on-surface text-sm font-semibold border border-outline-variant/30 transition-colors"
          @click="isMobileFilterModalOpen = true"
        >
          <span class="material-symbols-outlined text-[20px] text-secondary">tune</span>
          <span>Filtros</span>
          <span
            v-if="activeFilterChips.length > 0"
            class="w-5 h-5 rounded-full bg-secondary text-on-secondary text-xs font-bold flex items-center justify-center"
          >
            {{ activeFilterChips.length }}
          </span>
        </button>

        <button
          type="button"
          class="hidden lg:flex items-center gap-2 px-3.5 py-2 rounded-lg bg-surface-container-low hover:bg-surface-container text-on-surface text-sm font-semibold border border-outline-variant/30 transition-colors"
          @click="isDesktopFilterSidebarOpen = !isDesktopFilterSidebarOpen"
        >
          <span class="material-symbols-outlined text-[20px] text-secondary">tune</span>
          <span
            v-if="!isDesktopFilterSidebarOpen && activeFilterChips.length > 0"
            class="w-5 h-5 rounded-full bg-secondary text-on-secondary text-xs font-bold flex items-center justify-center"
          >
            {{ activeFilterChips.length }}
          </span>
        </button>

        <div class="flex bg-surface-container-low p-1 rounded-lg border border-outline-variant/20">
          <button
            type="button"
            aria-label="Vista cuadrícula"
            :class="[
              'p-1.5 rounded-md transition-all',
              viewMode === 'grid'
                ? 'bg-surface-container-lowest text-secondary shadow-xs'
                : 'text-on-surface-variant hover:text-on-surface'
            ]"
            @click="viewMode = 'grid'"
          >
            <span class="material-symbols-outlined text-[20px] block">grid_view</span>
          </button>
          <button
            type="button"
            aria-label="Vista lista"
            :class="[
              'p-1.5 rounded-md transition-all',
              viewMode === 'list'
                ? 'bg-surface-container-lowest text-secondary shadow-xs'
                : 'text-on-surface-variant hover:text-on-surface'
            ]"
            @click="viewMode = 'list'"
          >
            <span class="material-symbols-outlined text-[20px] block">view_list</span>
          </button>
        </div>
      </div>
    </div>

    <div
      v-if="activeFilterChips.length > 0"
      class="flex flex-wrap items-center gap-2 mb-6 p-3 rounded-xl bg-surface-container-low/70 border border-outline-variant/20"
    >
      <span class="text-xs font-semibold text-on-surface-variant mr-1">Filtros activos:</span>
      <span
        v-for="chip in activeFilterChips"
        :key="chip.id"
        class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-lg bg-secondary-fixed text-on-secondary-fixed text-xs font-medium shadow-2xs"
      >
        <span>{{ chip.label }}</span>
        <button
          type="button"
          :aria-label="`Quitar filtro ${chip.label}`"
          class="hover:bg-secondary/20 rounded-full p-0.5 transition-colors flex items-center justify-center"
          @click="chip.clear"
        >
          <span class="material-symbols-outlined text-[14px]">close</span>
        </button>
      </span>
      <button
        type="button"
        class="text-xs font-semibold text-secondary hover:underline ml-auto pl-2"
        @click="resetFilters"
      >
        Limpiar todos
      </button>
    </div>

    <div class="flex flex-col lg:flex-row gap-8 items-start w-full">
      <TutorFilterSidebar
        v-if="isDesktopFilterSidebarOpen"
        v-model:materia="filters.materia"
        v-model:universidad="filters.universidad"
        v-model:experiencia-minima="filters.experienciaMinima"
        v-model:edad-maxima="filters.edadMaxima"
        v-model:genero="filters.genero"
        class="hidden lg:flex"
        @reset="resetFilters"
      />

      <div class="flex-1 flex flex-col w-full min-w-0">
        <div
          v-if="error"
          class="bg-error-container text-on-error-container rounded-xl p-6 text-sm font-body"
        >
          {{ error }}
        </div>

        <div
          v-else-if="isLoading"
          class="bg-surface-container-lowest rounded-xl p-12 text-center border border-outline-variant/20 flex flex-col items-center justify-center min-h-[300px]"
        >
          <div
            class="w-10 h-10 border-3 border-secondary/30 border-t-secondary rounded-full animate-spin mb-4"
          />
          <p class="text-sm font-medium text-on-surface-variant">Cargando tutores disponibles...</p>
        </div>

        <div v-else-if="tutors.length > 0" :class="['grid gap-6 w-full', gridClasses]">
          <TutorCard v-for="tutor in tutors" :key="tutor.tutorId" :tutor="tutor" />
        </div>

        <div
          v-else
          class="bg-surface-container-lowest rounded-xl p-12 text-center border border-outline-variant/20 flex flex-col items-center justify-center min-h-[300px]"
        >
          <div
            class="w-16 h-16 rounded-full bg-surface-container-high flex items-center justify-center text-on-surface-variant mb-4"
          >
            <span class="material-symbols-outlined text-[32px]">search_off</span>
          </div>
          <h3 class="text-xl font-bold font-headline text-on-surface mb-2">
            No se encontraron tutores
          </h3>
          <p class="text-sm text-on-surface-variant max-w-md font-body mb-6">
            No hay tutores que coincidan con los filtros seleccionados. Intenta ajustar o reiniciar
            los criterios de búsqueda.
          </p>
          <button
            type="button"
            class="px-5 py-2.5 rounded-lg bg-primary text-on-primary text-sm font-semibold hover:bg-primary/90 transition-colors"
            @click="resetFilters"
          >
            Restablecer Filtros
          </button>
        </div>
      </div>
    </div>

    <TutorFilterModal
      v-model="isMobileFilterModalOpen"
      v-model:materia="filters.materia"
      v-model:universidad="filters.universidad"
      v-model:experiencia-minima="filters.experienciaMinima"
      v-model:edad-maxima="filters.edadMaxima"
      v-model:genero="filters.genero"
      @reset="resetFilters"
    />
  </div>
</template>
