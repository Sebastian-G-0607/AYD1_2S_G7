<script setup lang="ts">
import TutorFilterForm from './TutorFilterForm.vue'

const materia = defineModel<string>('materia', { default: '' })
const universidad = defineModel<string>('universidad', { default: '' })
const experienciaMinima = defineModel<number>('experienciaMinima', { default: 0 })
const edadMaxima = defineModel<number>('edadMaxima', { default: 65 })
const genero = defineModel<'any' | 'female' | 'male'>('genero', { default: 'any' })

interface Emits {
  (e: 'reset'): void
}

const emit = defineEmits<Emits>()
</script>

<template>
  <aside
    class="w-full lg:w-72 shrink-0 flex flex-col gap-6 bg-surface-container-lowest p-6 rounded-xl shadow-sm border border-outline-variant/20 lg:sticky lg:top-28 lg:max-h-[calc(100vh-8.5rem)] lg:overflow-y-auto"
  >
    <div class="flex items-center justify-between pb-4 border-b border-surface-container">
      <div class="flex items-center gap-2">
        <span class="material-symbols-outlined text-secondary text-[22px]">tune</span>
        <span class="font-headline text-lg font-bold text-on-surface">Filtros</span>
      </div>
      <button
        type="button"
        aria-label="Limpiar filtros"
        title="Limpiar filtros"
        class="text-secondary hover:text-secondary-container transition-colors p-2 rounded-full hover:bg-secondary-fixed/50 group"
        @click="emit('reset')"
      >
        <span
          class="material-symbols-outlined text-[20px] group-hover:rotate-180 transition-transform duration-500"
        >
          restart_alt
        </span>
      </button>
    </div>

    <TutorFilterForm
      v-model:materia="materia"
      v-model:universidad="universidad"
      v-model:experiencia-minima="experienciaMinima"
      v-model:edad-maxima="edadMaxima"
      v-model:genero="genero"
    />
  </aside>
</template>
