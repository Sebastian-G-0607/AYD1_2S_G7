<script setup lang="ts">
import { BaseAlert, BaseBadge, BaseButton, BaseCard } from '@/components/ui'
import { useStudentStudyPlan } from '../composables/useStudentStudyPlan'
import type { RecursoTipo } from '../types'

const { studyPlan, isLoading, errorMessage } = useStudentStudyPlan()

function formatDate(date: string) {
  if (!date) return '-'

  const [year, month, day] = date.split('-')

  if (!year || !month || !day) return date

  return `${day}/${month}/${year}`
}

function resourceIcon(tipo: RecursoTipo) {
  switch (tipo) {
    case 'VIDEO':
      return 'play_circle'
    case 'PDF':
      return 'picture_as_pdf'
    case 'ENLACE':
      return 'link'
    case 'TEXTO':
    default:
      return 'article'
  }
}

function resourceLabel(tipo: RecursoTipo) {
  switch (tipo) {
    case 'VIDEO':
      return 'Video'
    case 'PDF':
      return 'PDF'
    case 'ENLACE':
      return 'Enlace'
    case 'TEXTO':
    default:
      return 'Texto'
  }
}

function handlePrint() {
  // HU-26: la generación real del PDF se implementa aparte.
  window.print()
}
</script>

<template>
  <div class="flex flex-col w-full gap-8">
    <!-- Encabezado -->
    <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
      <div>
        <h1 class="text-3xl font-bold font-headline text-primary-container tracking-tight">
          Plan de Estudio
        </h1>

        <p class="text-base text-on-surface-variant mt-2">
          Consulta las dificultades identificadas y los recursos recomendados por tu tutor.
        </p>
      </div>

      <BaseButton
        v-if="studyPlan"
        variant="outline"
        icon="print"
        :disabled="isLoading"
        @click="handlePrint"
      >
        Imprimir constancia
      </BaseButton>
    </div>

    <!-- Error -->
    <BaseAlert
      v-if="errorMessage"
      type="error"
      :message="errorMessage"
      :dismissible="false"
    />

    <!-- Cargando -->
    <BaseCard v-else-if="isLoading" padding="lg">
      <p class="text-sm text-on-surface-variant text-center py-8">Cargando tu plan de estudio…</p>
    </BaseCard>

    <!-- Sin plan activo -->
    <BaseCard v-else-if="!studyPlan" padding="lg">
      <div class="flex flex-col items-center text-center py-8 gap-2">
        <span class="material-symbols-outlined text-[40px] text-on-surface-variant/50">
          menu_book
        </span>
        <p class="text-base font-medium text-on-surface">Aún no tienes un plan de estudio</p>
        <p class="text-sm text-on-surface-variant max-w-md">
          Cuando un tutor marque una de tus sesiones como atendida y registre tu plan de estudio,
          lo verás reflejado aquí.
        </p>
      </div>
    </BaseCard>

    <!-- Plan de estudio -->
    <div v-else class="flex flex-col gap-6">
      <BaseCard padding="lg">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div>
            <p class="text-xs font-medium text-on-surface-variant uppercase tracking-wide">
              Última sesión
            </p>
            <p class="text-base text-on-surface mt-1">{{ formatDate(studyPlan.fechaUltimaSesion) }}</p>
          </div>

          <div>
            <p class="text-xs font-medium text-on-surface-variant uppercase tracking-wide">
              Tutor
            </p>
            <p class="text-base text-on-surface mt-1">{{ studyPlan.tutorNombre }}</p>
            <p class="text-sm text-on-surface-variant">
              {{ studyPlan.tutorEspecialidad }} · ID {{ studyPlan.tutorIdentificacion }}
            </p>
          </div>
        </div>
      </BaseCard>

      <BaseCard padding="lg">
        <template #header>
          <h2 class="text-lg font-semibold text-on-surface">Dificultades identificadas</h2>
        </template>

        <p class="text-sm text-on-surface-variant whitespace-pre-line">
          {{ studyPlan.dificultadesIdentificadas }}
        </p>
      </BaseCard>

      <BaseCard padding="lg">
        <template #header>
          <h2 class="text-lg font-semibold text-on-surface">Recursos recomendados</h2>
        </template>

        <ul class="flex flex-col divide-y divide-outline-variant/20">
          <li
            v-for="recurso in studyPlan.recursos"
            :key="recurso.recursoId"
            class="flex items-start gap-3 py-4 first:pt-0 last:pb-0"
          >
            <span class="material-symbols-outlined text-primary text-[22px] mt-0.5">
              {{ resourceIcon(recurso.tipo) }}
            </span>

            <div class="flex-1">
              <div class="flex items-center gap-2 flex-wrap">
                <p class="text-sm font-medium text-on-surface">{{ recurso.nombre }}</p>
                <BaseBadge variant="neutral" size="sm">{{ resourceLabel(recurso.tipo) }}</BaseBadge>
              </div>
              <p class="text-sm text-on-surface-variant mt-1">{{ recurso.descripcionUso }}</p>
            </div>
          </li>
        </ul>
      </BaseCard>
    </div>
  </div>
</template>