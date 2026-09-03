<script setup lang="ts">
import { onMounted } from 'vue'
import { useTutorsExplorer } from '../composables/useTutorsExplorer'
import TutorCard from './TutorCard.vue'
import TutorFilterSidebar from './TutorFilterSidebar.vue'

const { tutors, isLoading, error, viewMode, filters, resetFilters, fetchTutors } =
  useTutorsExplorer()

onMounted(() => {
  fetchTutors()
})
</script>

<template>
  <div class="flex flex-col w-full relative">
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-8">
      <div class="flex flex-col gap-2">
        <h1 class="text-3xl font-bold font-headline text-on-surface tracking-tight">
          Explorar Tutores
        </h1>
        <p class="text-base text-on-surface-variant font-body max-w-3xl">
          Encuentra al experto ideal para potenciar tu aprendizaje. Filtra por especialidad,
          experiencia y disponibilidad.
        </p>
      </div>

      <div
        class="flex bg-surface-container-low p-1 rounded-lg border border-outline-variant/20 self-start sm:self-auto"
      >
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

    <div class="flex flex-col lg:flex-row gap-8 items-start w-full">
      <TutorFilterSidebar
        v-model:materia="filters.materia"
        v-model:universidad="filters.universidad"
        v-model:experiencia-minima="filters.experienciaMinima"
        v-model:edad-maxima="filters.edadMaxima"
        v-model:genero="filters.genero"
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
          v-else-if="tutors.length > 0"
          :class="[
            'grid gap-6 w-full',
            viewMode === 'grid' ? 'grid-cols-1 md:grid-cols-2 xl:grid-cols-3' : 'grid-cols-1'
          ]"
        >
          <TutorCard v-for="tutor in tutors" :key="tutor.tutorId" :tutor="tutor" />
        </div>

        <div
          v-else-if="!isLoading"
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
  </div>
</template>
