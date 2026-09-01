<script setup lang="ts">
const materia = defineModel<string>('materia', { default: '' })
const universidad = defineModel<string>('universidad', { default: '' })
const expMinima = defineModel<number>('expMinima', { default: 0 })
const rangoEdad = defineModel<number>('rangoEdad', { default: 65 })
const genero = defineModel<'any' | 'female' | 'male'>('genero', { default: 'any' })

interface Emits {
  (e: 'reset'): void
}

const emit = defineEmits<Emits>()
</script>

<template>
  <aside
    class="w-full lg:w-72 flex-shrink-0 flex flex-col gap-6 bg-surface-container-lowest p-6 rounded-xl shadow-sm border border-outline-variant/20 sticky top-28 z-10"
  >
    <div class="flex items-center justify-between pb-4 border-b border-surface-container">
      <span class="font-headline text-lg font-bold text-on-surface">Filtros</span>
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

    <div class="flex flex-col gap-2">
      <label for="filter-materia" class="text-sm font-semibold text-on-surface">Materia</label>
      <div class="relative">
        <select
          id="filter-materia"
          v-model="materia"
          class="w-full appearance-none bg-surface-container-lowest text-on-surface text-sm rounded-lg py-2.5 pl-4 pr-10 border border-outline-variant focus:outline-none focus:border-secondary focus:ring-1 focus:ring-secondary transition-all cursor-pointer"
        >
          <option value="">Todas las materias</option>
          <option value="matemáticas">Matemáticas Avanzadas</option>
          <option value="física">Física Cuántica</option>
          <option value="computación">Ciencias de la Computación</option>
          <option value="literatura">Literatura Contemporánea</option>
        </select>
        <span
          class="material-symbols-outlined absolute right-3 top-1/2 -translate-y-1/2 text-on-surface-variant pointer-events-none text-[20px]"
        >
          expand_more
        </span>
      </div>
    </div>

    <div class="flex flex-col gap-2">
      <label for="filter-universidad" class="text-sm font-semibold text-on-surface"
        >Universidad de Origen</label
      >
      <div class="relative">
        <select
          id="filter-universidad"
          v-model="universidad"
          class="w-full appearance-none bg-surface-container-lowest text-on-surface text-sm rounded-lg py-2.5 pl-4 pr-10 border border-outline-variant focus:outline-none focus:border-secondary focus:ring-1 focus:ring-secondary transition-all cursor-pointer"
        >
          <option value="">Cualquier Universidad</option>
          <option value="unam">UNAM</option>
          <option value="tec">Tecnológico de Monterrey</option>
          <option value="ibero">Universidad Iberoamericana</option>
        </select>
        <span
          class="material-symbols-outlined absolute right-3 top-1/2 -translate-y-1/2 text-on-surface-variant pointer-events-none text-[20px]"
        >
          expand_more
        </span>
      </div>
    </div>

    <div class="flex flex-col gap-3 pt-2">
      <div class="flex justify-between items-center">
        <label for="filter-exp" class="text-sm font-semibold text-on-surface"
          >Experiencia mínima</label
        >
        <span
          class="text-xs font-bold bg-secondary-fixed text-on-secondary-fixed px-2 py-0.5 rounded-md"
        >
          {{ expMinima }} años
        </span>
      </div>
      <input
        id="filter-exp"
        v-model.number="expMinima"
        type="range"
        min="0"
        max="10"
        class="w-full h-1.5 bg-surface-variant rounded-lg appearance-none cursor-pointer accent-secondary"
      />
      <div class="flex justify-between text-xs text-on-surface-variant px-1 font-medium">
        <span>0</span>
        <span>10+</span>
      </div>
    </div>

    <div class="flex flex-col gap-3 pt-2">
      <div class="flex justify-between items-center">
        <label for="filter-edad" class="text-sm font-semibold text-on-surface"
          >Rango de Edad Máxima</label
        >
        <span
          class="text-xs font-bold bg-secondary-fixed text-on-secondary-fixed px-2 py-0.5 rounded-md"
        >
          {{ rangoEdad }} años
        </span>
      </div>
      <input
        id="filter-edad"
        v-model.number="rangoEdad"
        type="range"
        min="18"
        max="65"
        class="w-full h-1.5 bg-surface-variant rounded-lg appearance-none cursor-pointer accent-secondary"
      />
      <div class="flex justify-between text-xs text-on-surface-variant px-1 font-medium">
        <span>18</span>
        <span>65+</span>
      </div>
    </div>

    <div class="flex flex-col gap-3 pt-2">
      <span class="text-sm font-semibold text-on-surface">Preferencia de Género</span>
      <div class="flex flex-col gap-2">
        <label class="flex items-center gap-3 cursor-pointer group">
          <input
            v-model="genero"
            type="radio"
            name="gender"
            value="any"
            class="w-4 h-4 text-secondary bg-surface border-outline focus:ring-secondary accent-secondary cursor-pointer"
          />
          <span class="text-sm text-on-surface group-hover:text-secondary transition-colors"
            >Indistinto</span
          >
        </label>
        <label class="flex items-center gap-3 cursor-pointer group">
          <input
            v-model="genero"
            type="radio"
            name="gender"
            value="female"
            class="w-4 h-4 text-secondary bg-surface border-outline focus:ring-secondary accent-secondary cursor-pointer"
          />
          <span class="text-sm text-on-surface group-hover:text-secondary transition-colors"
            >Femenino</span
          >
        </label>
        <label class="flex items-center gap-3 cursor-pointer group">
          <input
            v-model="genero"
            type="radio"
            name="gender"
            value="male"
            class="w-4 h-4 text-secondary bg-surface border-outline focus:ring-secondary accent-secondary cursor-pointer"
          />
          <span class="text-sm text-on-surface group-hover:text-secondary transition-colors"
            >Masculino</span
          >
        </label>
      </div>
    </div>
  </aside>
</template>
