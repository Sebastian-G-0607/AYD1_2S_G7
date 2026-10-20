<script setup lang="ts">
import { computed, onMounted, onUnmounted, watch } from 'vue'

interface Props {
  modelValue: boolean
  title: string
  documentUrl?: string | null
  userName?: string
  documentLabel?: string
}

const props = withDefaults(defineProps<Props>(), {
  documentUrl: null,
  userName: '',
  documentLabel: 'Documento PDF'
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'close'): void
}>()

const hasDocument = computed(() => {
  return Boolean(props.documentUrl?.trim())
})

function closeModal() {
  emit('update:modelValue', false)
  emit('close')
}

function openInNewTab() {
  if (!props.documentUrl) return

  window.open(
    props.documentUrl,
    '_blank',
    'noopener,noreferrer'
  )
}

function handleKeydown(event: KeyboardEvent) {
  if (event.key === 'Escape' && props.modelValue) {
    closeModal()
  }
}

watch(
  () => props.modelValue,
  isOpen => {
    document.body.style.overflow = isOpen ? 'hidden' : ''
  }
)

onMounted(() => {
  window.addEventListener('keydown', handleKeydown)
})

onUnmounted(() => {
  window.removeEventListener('keydown', handleKeydown)
  document.body.style.overflow = ''
})
</script>

<template>
  <Teleport to="body">
    <Transition
      enter-active-class="transition duration-200 ease-out"
      enter-from-class="opacity-0"
      enter-to-class="opacity-100"
      leave-active-class="transition duration-150 ease-in"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
    >
      <div
        v-if="modelValue"
        class="fixed inset-0 z-[70] flex items-center justify-center bg-on-background/50 backdrop-blur-sm p-4"
        @click.self="closeModal"
      >
        <div
          class="w-full max-w-5xl h-[90vh] bg-surface-container-lowest rounded-2xl shadow-2xl border border-outline-variant/30 overflow-hidden flex flex-col"
          role="dialog"
          aria-modal="true"
        >
          <!-- Encabezado -->
          <div
            class="flex items-center justify-between gap-4 px-6 py-4 border-b border-surface-container-high"
          >
            <div class="flex items-center gap-3 min-w-0">
              <div
                class="w-10 h-10 rounded-xl bg-error-container text-error flex items-center justify-center shrink-0"
              >
                <span class="material-symbols-outlined">
                  picture_as_pdf
                </span>
              </div>

              <div class="min-w-0">
                <h2
                  class="font-headline-md text-headline-md text-on-surface truncate"
                >
                  {{ title }}
                </h2>

                <p
                  v-if="userName"
                  class="text-sm text-on-surface-variant truncate"
                >
                  {{ userName }}
                </p>
              </div>
            </div>

            <div class="flex items-center gap-2">
              <button
                v-if="hasDocument"
                type="button"
                title="Abrir PDF en una nueva pestaña"
                class="hidden sm:inline-flex items-center gap-2 h-10 px-4 rounded-lg bg-surface-container-highest text-on-surface hover:bg-surface-dim transition-colors font-label-md text-label-md"
                @click="openInNewTab"
              >
                <span
                  class="material-symbols-outlined text-[19px]"
                >
                  open_in_new
                </span>
                Abrir aparte
              </button>

              <button
                type="button"
                title="Cerrar visor"
                aria-label="Cerrar visor"
                class="w-10 h-10 rounded-full flex items-center justify-center text-on-surface-variant hover:text-on-surface hover:bg-surface-container-high transition-colors"
                @click="closeModal"
              >
                <span class="material-symbols-outlined">
                  close
                </span>
              </button>
            </div>
          </div>

          <!-- Visor -->
          <div class="flex-1 min-h-0 bg-surface-container-low p-3 sm:p-5">
            <iframe
              v-if="hasDocument"
              :src="documentUrl || undefined"
              :title="documentLabel"
              class="w-full h-full bg-white rounded-xl border border-outline-variant/30"
            />

            <div
              v-else
              class="w-full h-full flex flex-col items-center justify-center text-center bg-surface-container-lowest rounded-xl border border-outline-variant/30 p-8"
            >
              <div
                class="w-16 h-16 rounded-full bg-surface-container-high flex items-center justify-center mb-4"
              >
                <span
                  class="material-symbols-outlined text-[32px] text-on-surface-variant"
                >
                  scan_delete
                </span>
              </div>

              <h3
                class="font-headline-md text-headline-md text-on-surface mb-2"
              >
                Documento no disponible
              </h3>

              <p
                class="text-sm text-on-surface-variant max-w-md"
              >
                No existe un archivo PDF asociado a esta solicitud.
              </p>
            </div>
          </div>

          <!-- Pie -->
          <div
            class="px-6 py-4 border-t border-surface-container-high flex items-center justify-between gap-4"
          >
            <div class="flex items-center gap-2 text-on-surface-variant">
              <span
                class="material-symbols-outlined text-[19px]"
              >
                visibility
              </span>

              <span class="text-sm">
                {{ documentLabel }} incrustado en EduConnect
              </span>
            </div>

            <button
              type="button"
              class="h-10 px-5 rounded-lg bg-primary text-on-primary hover:opacity-90 transition-opacity font-label-md text-label-md"
              @click="closeModal"
            >
              Cerrar
            </button>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>