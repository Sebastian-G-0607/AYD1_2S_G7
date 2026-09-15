<script setup lang="ts">
import type { StudentSession } from '../types'

interface Props {
  session: StudentSession
}

defineProps<Props>()

defineEmits<{
  (e: 'cancel', session: StudentSession): void
}>()
</script>

<template>
  <div
    class="bg-surface-container-lowest rounded-2xl p-6 shadow-sm hover:shadow-md transition-shadow duration-300 flex flex-col h-full relative overflow-hidden group border border-outline-variant/20"
  >
    <div
      v-if="session.isUpcoming"
      class="absolute top-0 right-0 w-24 h-24 bg-primary-fixed/20 rounded-bl-full -mr-4 -mt-4 transition-transform group-hover:scale-110 pointer-events-none"
    />

    <div class="flex justify-between items-start mb-6 relative">
      <div class="flex flex-col gap-1">
        <div class="flex items-center gap-2">
          <span class="material-symbols-outlined text-primary text-lg"> schedule </span>
          <span class="text-sm font-semibold text-on-surface">
            {{ session.timeLabel }}
          </span>
        </div>
        <div class="flex items-center gap-2">
          <span class="material-symbols-outlined text-on-surface-variant text-lg"> event </span>
          <span class="text-xs text-on-surface-variant">
            {{ session.dateLabel }}
          </span>
        </div>
      </div>

      <div
        v-if="session.isUpcoming"
        class="bg-secondary-container text-on-secondary-container px-3 py-1 rounded-full text-xs font-semibold flex items-center gap-1 shadow-sm"
      >
        <span class="w-1.5 h-1.5 rounded-full bg-on-secondary-container animate-pulse" />
        {{ session.statusLabel || 'Próxima' }}
      </div>
      <div
        v-else
        class="bg-surface-container text-on-surface-variant px-3 py-1 rounded-full text-xs font-semibold flex items-center gap-1"
      >
        {{ session.statusLabel || 'Confirmada' }}
      </div>
    </div>

    <div class="flex items-center gap-4 mb-6">
      <img
        v-if="session.avatarUrl"
        class="w-14 h-14 rounded-full object-cover shadow-sm ring-1 ring-outline-variant/30"
        :src="session.avatarUrl"
        :alt="session.tutorName"
      />
      <div
        v-else
        class="w-14 h-14 rounded-full bg-tertiary-fixed flex items-center justify-center text-on-tertiary-fixed font-headline font-bold text-xl shadow-sm"
      >
        {{ session.initials || 'TU' }}
      </div>

      <div class="flex flex-col min-w-0">
        <h3 class="font-headline text-lg font-bold text-on-surface truncate">
          {{ session.tutorName }}
        </h3>
        <span class="text-sm font-medium text-secondary truncate">
          {{ session.subject }}
        </span>
      </div>
    </div>

    <div class="flex flex-col gap-3 mb-8 flex-grow">
      <div
        class="flex items-start gap-3 bg-surface p-3 rounded-xl border border-outline-variant/20"
      >
        <span class="material-symbols-outlined text-on-surface-variant mt-0.5 text-lg">
          {{ session.locationType === 'virtual' ? 'videocam' : 'location_on' }}
        </span>
        <div class="flex flex-col min-w-0">
          <span class="text-xs font-semibold text-on-surface">
            {{ session.locationTitle }}
          </span>
          <a
            v-if="session.meetingUrl"
            :href="session.meetingUrl"
            target="_blank"
            rel="noopener noreferrer"
            class="text-xs text-primary hover:underline cursor-pointer truncate"
          >
            {{ session.locationSubtitle }}
          </a>
          <span v-else class="text-xs text-on-surface-variant truncate">
            {{ session.locationSubtitle }}
          </span>
        </div>
      </div>

      <div class="flex items-start gap-3 px-1">
        <span class="material-symbols-outlined text-on-surface-variant mt-0.5 text-lg">
          lightbulb
        </span>
        <div class="flex flex-col min-w-0">
          <span class="text-xs font-semibold text-on-surface"> Motivo de la sesión </span>
          <p class="text-xs text-on-surface-variant line-clamp-2 leading-relaxed">
            {{ session.reason }}
          </p>
        </div>
      </div>
    </div>

    <div class="flex items-center gap-3 mt-auto pt-4 border-t border-surface-variant/50">
      <button
        type="button"
        class="w-full px-4 py-2.5 rounded-lg text-error font-semibold text-sm hover:bg-error-container/50 transition-colors flex items-center justify-center gap-2 group/btn"
        @click="$emit('cancel', session)"
      >
        <span
          class="material-symbols-outlined text-sm group-hover/btn:rotate-90 transition-transform"
        >
          close
        </span>
        Cancelar
      </button>
    </div>
  </div>
</template>
