<script setup lang="ts">
import type { TutorDashboardStats } from '../types'

interface Props {
  stats: TutorDashboardStats
}

defineProps<Props>()
</script>

<template>
  <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
    <div
      class="bg-surface-container-lowest rounded-2xl p-6 relative overflow-hidden border border-outline-variant/20 shadow-sm group"
    >
      <div
        class="absolute -right-4 -top-4 w-24 h-24 bg-primary/5 rounded-full group-hover:scale-110 transition-transform duration-500 pointer-events-none"
      />
      <div class="flex items-center gap-4 mb-4">
        <div
          class="w-12 h-12 rounded-xl bg-surface-container flex items-center justify-center text-on-surface shadow-xs"
        >
          <span class="material-symbols-outlined text-[24px]">calendar_today</span>
        </div>
        <h3 class="text-3xl font-bold font-headline text-on-surface m-0">
          {{ stats.sesionesPendientes }}
        </h3>
      </div>
      <p class="text-sm font-medium text-on-surface-variant m-0 font-body">Sesiones Pendientes</p>
      <div class="mt-4 flex items-center gap-2">
        <span class="w-2 h-2 rounded-full bg-secondary" />
        <span class="text-xs font-semibold text-secondary">{{ stats.pendientesHoy }} hoy</span>
      </div>
    </div>

    <div
      class="bg-surface-container-lowest rounded-2xl p-6 relative overflow-hidden border border-outline-variant/20 shadow-sm group"
    >
      <div
        class="absolute -right-4 -top-4 w-24 h-24 bg-secondary/5 rounded-full group-hover:scale-110 transition-transform duration-500 pointer-events-none"
      />
      <div class="flex items-center gap-4 mb-4">
        <div
          class="w-12 h-12 rounded-xl bg-secondary-container flex items-center justify-center text-on-secondary-container shadow-xs"
        >
          <span class="material-symbols-outlined text-[24px]">check_circle</span>
        </div>
        <h3 class="text-3xl font-bold font-headline text-on-surface m-0">
          {{ stats.sesionesAtendidasMes }}
        </h3>
      </div>
      <p class="text-sm font-medium text-on-surface-variant m-0 font-body">
        Sesiones Atendidas (Mes)
      </p>
      <div class="mt-4 flex items-center gap-2">
        <span class="w-2 h-2 rounded-full bg-[#16a34a]" />
        <span class="text-xs font-semibold text-[#16a34a]">Completadas con éxito</span>
      </div>
    </div>

    <div
      class="bg-surface-container-lowest rounded-2xl p-6 relative overflow-hidden border border-outline-variant/20 shadow-sm group"
    >
      <div
        class="absolute -right-4 -top-4 w-24 h-24 bg-error/5 rounded-full group-hover:scale-110 transition-transform duration-500 pointer-events-none"
      />
      <div class="flex items-center gap-4 mb-4">
        <div
          class="w-12 h-12 rounded-xl bg-error-container flex items-center justify-center text-on-error-container shadow-xs"
        >
          <span class="material-symbols-outlined text-[24px]">cancel</span>
        </div>
        <h3 class="text-3xl font-bold font-headline text-on-surface m-0">
          {{ stats.sesionesCanceladas }}
        </h3>
      </div>
      <p class="text-sm font-medium text-on-surface-variant m-0 font-body">Sesiones Canceladas</p>
      <div class="mt-4 flex items-center gap-2">
        <span class="w-2 h-2 rounded-full bg-error" />
        <span class="text-xs font-semibold text-error">Cancelaciones totales</span>
      </div>
    </div>
  </div>
</template>
