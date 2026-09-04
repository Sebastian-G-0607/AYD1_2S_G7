<script setup lang="ts">
import { useRouter } from 'vue-router'
import BaseButton from '@/components/ui/BaseButton.vue'
import type { Tutor } from '../types'

interface Props {
  tutor: Tutor
}

const props = defineProps<Props>()

const router = useRouter()

function goToTutorDetail() {
  router.push(`/estudiante/tutores/${props.tutor.id}`)
}
</script>

<template>
  <article
    class="bg-surface-container-lowest rounded-xl p-6 shadow-sm hover:shadow-xl transition-all duration-300 group flex flex-col gap-4 relative overflow-hidden transform hover:-translate-y-1 border border-outline-variant/20"
  >
    <div
      class="absolute top-0 right-0 w-24 h-24 bg-primary-fixed/20 rounded-bl-full pointer-events-none opacity-0 group-hover:opacity-100 transition-opacity duration-500"
    />

    <div class="flex gap-4 items-start relative z-10">
      <div class="relative flex-shrink-0">
        <img
          v-if="tutor.fotografiaUrl"
          :src="tutor.fotografiaUrl"
          :alt="tutor.nombre"
          class="w-16 h-16 rounded-full object-cover shadow-sm ring-2 ring-surface-container-lowest"
        />

        <div
          v-else
          class="w-16 h-16 rounded-full bg-primary-container text-on-primary-container flex items-center justify-center font-bold text-lg"
        >
          {{ tutor.nombre.charAt(0) }}
        </div>

        <span
          :class="[
            'absolute bottom-0 right-0 w-3.5 h-3.5 rounded-full border-2 border-surface-container-lowest',
            tutor.isOnline ? 'bg-secondary' : 'bg-outline-variant'
          ]"
          :title="tutor.isOnline ? 'En línea' : 'Desconectado'"
        />
      </div>

      <div class="flex flex-col min-w-0 flex-1">
        <h3
          class="text-lg font-bold font-headline text-on-surface truncate group-hover:text-secondary transition-colors"
        >
          {{ tutor.nombre }}
        </h3>

        <span class="text-xs font-semibold text-secondary uppercase tracking-wider mt-0.5">
          {{ tutor.especialidad }}
        </span>

        <div class="flex items-center gap-1.5 mt-2 text-on-surface-variant">
          <span class="material-symbols-outlined text-[16px] text-[#eab308]">
            star
          </span>

          <span class="text-xs font-bold text-on-surface">
            {{ tutor.rating.toFixed(1) }}
          </span>

          <span class="text-xs text-on-surface-variant">
            ({{ tutor.totalResenas }} reseñas)
          </span>
        </div>
      </div>
    </div>

    <div class="flex flex-col gap-2 mt-1">
      <div class="flex items-center gap-2 text-on-surface-variant text-xs">
        <span class="material-symbols-outlined text-[18px] text-on-surface-variant">
          location_on
        </span>
        <span class="truncate">{{ tutor.ubicacion }}</span>
      </div>

      <div class="flex items-center gap-2 text-on-surface-variant text-xs">
        <span class="material-symbols-outlined text-[18px] text-on-surface-variant">
          school
        </span>

        <span class="truncate">
          {{ tutor.universidad }} • {{ tutor.aniosExperiencia }} años exp.
        </span>
      </div>
    </div>

    <div class="mt-auto pt-3 flex flex-wrap gap-1.5">
      <span
        v-for="tag in tutor.tags"
        :key="tag"
        class="bg-surface-container-low text-on-surface-variant px-2.5 py-0.5 rounded-full text-xs font-medium border border-outline-variant/30"
      >
        {{ tag }}
      </span>
    </div>

    <BaseButton
      variant="primary"
      size="md"
      block
      class="mt-2"
      @click="goToTutorDetail"
    >
      <span>Ver Perfil y Horarios</span>

      <template #iconRight>
        <span class="material-symbols-outlined text-[18px]">
          arrow_forward
        </span>
      </template>
    </BaseButton>
  </article>
</template>