<script setup lang="ts">
import { computed } from 'vue'
import { useMaterias } from '@/composables/useMaterias'
import BaseSelect, { type SelectOption } from '@/components/ui/BaseSelect.vue'
import BaseInput from '@/components/ui/BaseInput.vue'

const { materias } = useMaterias()

const materia = defineModel<string>('materia', { default: '' })
const universidad = defineModel<string>('universidad', { default: '' })
const experienciaMinima = defineModel<number>('experienciaMinima', { default: 0 })
const edadMaxima = defineModel<number>('edadMaxima', { default: 65 })
const genero = defineModel<'any' | 'female' | 'male'>('genero', { default: 'any' })

const materiaOptions = computed<SelectOption[]>(() => {
  const options = materias.value.map(item => ({
    value: item.nombre,
    label: item.nombre
  }))
  return [{ value: '', label: 'Todas las materias' }, ...options]
})
</script>

<template>
  <div class="flex flex-col gap-6 w-full">
    <div class="flex flex-col gap-2">
      <BaseSelect
        id="filter-materia"
        v-model="materia"
        label="Materia"
        placeholder=""
        :options="materiaOptions"
      />
    </div>

    <div class="flex flex-col gap-2">
      <BaseInput
        id="filter-universidad"
        v-model="universidad"
        label="Universidad de Origen"
        placeholder="Ej. USAC, URL..."
        icon="school"
      />
    </div>

    <div class="flex flex-col gap-3 pt-2">
      <div class="flex justify-between items-center">
        <label for="filter-exp" class="text-sm font-semibold text-on-surface">
          Experiencia mínima
        </label>
        <span
          class="text-xs font-bold bg-secondary-fixed text-on-secondary-fixed px-2 py-0.5 rounded-md"
        >
          {{ experienciaMinima }} años
        </span>
      </div>
      <input
        id="filter-exp"
        v-model.number="experienciaMinima"
        type="range"
        min="0"
        max="10"
        class="w-full h-1.5 bg-surface-variant rounded-lg appearance-none cursor-pointer accent-secondary"
      />
      <div class="flex justify-between text-xs text-on-surface-variant px-1 font-medium">
        <span>0 años</span>
        <span>10+ años</span>
      </div>
    </div>

    <div class="flex flex-col gap-3 pt-2">
      <div class="flex justify-between items-center">
        <label for="filter-edad" class="text-sm font-semibold text-on-surface">
          Rango de Edad Máxima
        </label>
        <span
          class="text-xs font-bold bg-secondary-fixed text-on-secondary-fixed px-2 py-0.5 rounded-md"
        >
          {{ edadMaxima }} años
        </span>
      </div>
      <input
        id="filter-edad"
        v-model.number="edadMaxima"
        type="range"
        min="18"
        max="65"
        class="w-full h-1.5 bg-surface-variant rounded-lg appearance-none cursor-pointer accent-secondary"
      />
      <div class="flex justify-between text-xs text-on-surface-variant px-1 font-medium">
        <span>18 años</span>
        <span>65 años</span>
      </div>
    </div>

    <div class="flex flex-col gap-3 pt-2">
      <span class="text-sm font-semibold text-on-surface">Preferencia de Género</span>
      <div class="grid grid-cols-3 gap-2">
        <label
          :class="[
            'flex flex-col items-center justify-center p-2.5 rounded-xl border cursor-pointer transition-all text-xs font-medium text-center gap-1.5 select-none',
            genero === 'any'
              ? 'bg-secondary-fixed/50 border-secondary text-on-secondary-fixed font-bold shadow-xs'
              : 'bg-surface-container-low border-outline-variant/30 text-on-surface hover:bg-surface-container'
          ]"
        >
          <input v-model="genero" type="radio" name="filter-gender" value="any" class="sr-only" />
          <span class="material-symbols-outlined text-[18px]">group</span>
          <span>Indistinto</span>
        </label>

        <label
          :class="[
            'flex flex-col items-center justify-center p-2.5 rounded-xl border cursor-pointer transition-all text-xs font-medium text-center gap-1.5 select-none',
            genero === 'female'
              ? 'bg-secondary-fixed/50 border-secondary text-on-secondary-fixed font-bold shadow-xs'
              : 'bg-surface-container-low border-outline-variant/30 text-on-surface hover:bg-surface-container'
          ]"
        >
          <input
            v-model="genero"
            type="radio"
            name="filter-gender"
            value="female"
            class="sr-only"
          />
          <span class="material-symbols-outlined text-[18px]">female</span>
          <span>Femenino</span>
        </label>

        <label
          :class="[
            'flex flex-col items-center justify-center p-2.5 rounded-xl border cursor-pointer transition-all text-xs font-medium text-center gap-1.5 select-none',
            genero === 'male'
              ? 'bg-secondary-fixed/50 border-secondary text-on-secondary-fixed font-bold shadow-xs'
              : 'bg-surface-container-low border-outline-variant/30 text-on-surface hover:bg-surface-container'
          ]"
        >
          <input v-model="genero" type="radio" name="filter-gender" value="male" class="sr-only" />
          <span class="material-symbols-outlined text-[18px]">male</span>
          <span>Masculino</span>
        </label>
      </div>
    </div>
  </div>
</template>
