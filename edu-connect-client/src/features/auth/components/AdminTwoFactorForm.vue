<script setup lang="ts">
import { ref } from 'vue'
import { RouterLink } from 'vue-router'
import { BaseButton, BaseAlert } from '@/components/ui'
import { useAuth } from '../composables/useAuth'

const fileRef = ref<File | null>(null)
const fileInputRef = ref<HTMLInputElement | null>(null)
const isOver = ref(false)

const { isLoading, errorMessage, clearError, verifyAdmin2Fa } = useAuth()

function onFileChange(e: Event) {
  const input = e.target as HTMLInputElement
  const f = input.files && input.files[0]
  handleFileSelection(f)
}

function onDragEnter(e: DragEvent) {
  e.preventDefault()
  isOver.value = true
}

function onDragLeave(e: DragEvent) {
  e.preventDefault()
  isOver.value = false
}

function onDrop(e: DragEvent) {
  e.preventDefault()
  isOver.value = false
  const f = e.dataTransfer?.files?.[0]
  handleFileSelection(f)
}

function handleFileSelection(f: File | undefined | null) {
  if (!f) return
  if (f.name.endsWith('.txt')) {
    fileRef.value = f
    clearError()
  } else {
    errorMessage.value = 'Solo se permiten archivos .txt'
    fileRef.value = null
  }
}

function removeFile() {
  fileRef.value = null
  if (fileInputRef.value) {
    fileInputRef.value.value = ''
  }
}

async function handleSubmit() {
  if (!fileRef.value) {
    errorMessage.value = 'Por favor, selecciona el archivo de llave.'
    return
  }
  await verifyAdmin2Fa(fileRef.value)
}
</script>

<template>
  <div class="w-full flex flex-col gap-8 relative z-10">
    <div>
      <RouterLink
        to="/login"
        class="inline-flex items-center gap-2 text-sm font-semibold text-on-surface-variant hover:text-primary transition-colors group"
      >
        <span
          class="material-symbols-outlined text-[18px] group-hover:-translate-x-1 transition-transform"
        >
          arrow_back
        </span>
        <span>Volver al Login</span>
      </RouterLink>
    </div>

    <div class="flex flex-col gap-2">
      <h1 class="text-3xl font-bold text-primary font-headline tracking-tight">
        Verificación de Dos Pasos
      </h1>
      <p class="text-base text-on-surface-variant font-body">
        Sube tu archivo de llave (auth2-ayd1.txt) para continuar.
      </p>
    </div>

    <form class="flex flex-col gap-6" @submit.prevent="handleSubmit">
      <BaseAlert
        v-if="errorMessage"
        id="error-message"
        type="error"
        title="Error de verificación"
        :message="errorMessage"
        @dismiss="clearError"
      />

      <div
        :class="[
          'flex flex-col items-center justify-center w-full min-h-[190px] p-6 border-2 border-dashed rounded-xl cursor-pointer transition-all duration-200 text-center',
          isOver && !fileRef
            ? 'border-primary bg-surface-container-high'
            : fileRef
              ? 'border-primary/40 bg-surface-container-low'
              : 'border-outline-variant hover:border-primary/50 bg-surface-container-lowest hover:bg-surface-container-low'
        ]"
        @dragenter.prevent="onDragEnter"
        @dragover.prevent
        @dragleave.prevent="onDragLeave"
        @drop.prevent="onDrop"
        @click="!fileRef && fileInputRef?.click()"
      >
        <input
          id="file"
          ref="fileInputRef"
          accept=".txt"
          class="hidden"
          type="file"
          @change="onFileChange"
        />

        <template v-if="!fileRef">
          <div
            class="w-12 h-12 rounded-full bg-surface-container-high flex items-center justify-center mb-3 text-primary"
          >
            <span class="material-symbols-outlined text-2xl">upload_file</span>
          </div>
          <span class="font-body text-sm font-medium text-on-surface mb-1">
            Arrastra y suelta tu archivo aquí
          </span>
          <span class="text-xs text-on-surface-variant mb-3">o</span>
          <button
            type="button"
            class="px-4 py-2 bg-surface-container-high hover:bg-surface-variant text-on-surface text-xs font-semibold rounded-lg transition-colors"
            @click.stop="fileInputRef?.click()"
          >
            Explorar archivos
          </button>
        </template>

        <template v-else>
          <div class="flex flex-col items-center gap-2">
            <div
              class="w-12 h-12 rounded-full bg-primary/10 text-primary flex items-center justify-center"
            >
              <span class="material-symbols-outlined text-2xl">vpn_key</span>
            </div>
            <div class="text-sm font-semibold text-on-surface">{{ fileRef.name }}</div>
            <div class="text-xs text-on-surface-variant">
              {{ (fileRef.size / 1024).toFixed(1) }} KB
            </div>
            <button
              type="button"
              class="mt-2 inline-flex items-center gap-1 text-xs text-error hover:underline transition-colors"
              @click.stop="removeFile"
            >
              <span class="material-symbols-outlined text-[16px]">delete</span>
              <span>Quitar archivo</span>
            </button>
          </div>
        </template>
      </div>

      <BaseButton
        type="submit"
        variant="primary"
        size="md"
        block
        :loading="isLoading"
        :disabled="!fileRef"
      >
        Verificar y Entrar
      </BaseButton>
    </form>
  </div>
</template>
