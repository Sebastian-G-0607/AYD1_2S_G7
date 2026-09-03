<script setup lang="ts">
import { ref } from 'vue'
import { BaseButton } from '@/components/ui'
import { AvailabilityModal } from '@/features/tutor-availability'
import type { TutorExplorerItem } from '../types'

interface Props {
  tutor: TutorExplorerItem
}

defineProps<Props>()

const showAvailability = ref(false)
</script>

<template>
  <article
    class="bg-surface-container-lowest rounded-xl p-6 shadow-sm hover:shadow-xl transition-all duration-300 group flex flex-col gap-4 relative overflow-hidden transform hover:-translate-y-1 border border-outline-variant/20"
  >
    <div class="flex gap-4 items-start relative z-10">
      <img
        v-if="tutor.fotografiaUrl"
        :src="tutor.fotografiaUrl"
        :alt="tutor.nombreCompleto"
        class="w-16 h-16 rounded-full object-cover shadow-sm ring-2 ring-surface-container-lowest flex-shrink-0"
      />
      <div
        v-else
        class="w-16 h-16 rounded-full bg-primary-container text-on-primary-container flex items-center justify-center font-bold text-lg flex-shrink-0"
      >
        {{ tutor.nombreCompleto.charAt(0) }}
      </div>

      <div class="flex flex-col min-w-0 flex-1">
        <h3
          class="text-lg font-bold font-headline text-on-surface truncate group-hover:text-secondary transition-colors"
        >
          {{ tutor.nombreCompleto }}
        </h3>
        <span class="text-xs font-semibold text-secondary uppercase tracking-wider mt-0.5 truncate">
          {{ tutor.materias.join(', ') }}
        </span>
      </div>
    </div>

    <div class="flex flex-col gap-2 mt-1">
      <div class="flex items-center gap-2 text-on-surface-variant text-xs">
        <span class="material-symbols-outlined text-[18px]">location_on</span>
        <span class="truncate">{{ tutor.direccionTutoria }}</span>
      </div>

      <div class="flex items-center gap-2 text-on-surface-variant text-xs">
        <span class="material-symbols-outlined text-[18px]">school</span>
        <span class="truncate"
          >{{ tutor.universidad }} • {{ tutor.aniosExperiencia }} años exp.</span
        >
      </div>
    </div>

    <BaseButton variant="primary" size="md" block class="mt-2" @click="showAvailability = true">
      <span>Ver Perfil y Horarios</span>
      <template #iconRight>
        <span class="material-symbols-outlined text-[18px]">arrow_forward</span>
      </template>
    </BaseButton>
    <AvailabilityModal v-model="showAvailability" :tutor-id="tutor.tutorId" />
  </article>
</template>
