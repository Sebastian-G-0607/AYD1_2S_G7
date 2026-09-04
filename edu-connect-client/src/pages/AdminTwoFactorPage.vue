<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { BaseButton } from '@/components/ui'
import { authService } from '@/features/auth/services/auth.service'
import { useAuthStore } from '@/features/auth/store'

const router = useRouter()
const fileRef = ref<File | null>(null)
const error = ref<string | null>(null)
const loading = ref(false)
const isOver = ref(false)

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
    error.value = null
  } else {
    error.value = 'Solo se permiten archivos .txt'
    fileRef.value = null
  }
}

function removeFile() {
  fileRef.value = null
  error.value = null
  ;(document.getElementById('file') as HTMLInputElement | null)?.value && ((document.getElementById('file') as HTMLInputElement).value = '')
}

async function submit() {
  if (!fileRef.value) {
    error.value = 'Por favor, selecciona el archivo de llave.'
    return
  }

  loading.value = true
  error.value = null

  const qs = new URLSearchParams(window.location.search)
  const temp = qs.get('tempToken') || sessionStorage.getItem('edu_temp_token') || ''

  try {
    const result = await authService.uploadAdmin2Fa(fileRef.value, temp || undefined)
    const token = result.token
    const payload = JSON.parse(atob(token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/')))
    const user = { id: payload.sub || 0, correo: payload.email || payload.correo || '', rol: payload.rol || payload.role || 'Administrador' }
    const authStore = useAuthStore()
    authStore.setAuth(token, { id: Number(user.id), correo: user.correo, rol: user.rol })
    await router.push('/admin/aprobaciones')
  } catch (err: any) {
    error.value = err?.response?.data?.detail || err?.message || 'Error al validar el archivo.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <main class="w-full min-h-screen bg-background flex flex-col md:flex-row font-['Plus_Jakarta_Sans']">
    <!-- Left Pane -->
    <div class="hidden md:flex flex-col justify-between w-1/2 lg:w-[40%] bg-gradient-to-br from-[#091426] to-[#1e293b] text-white p-12 lg:p-20 relative overflow-hidden">
      <div class="absolute inset-0 bg-[url('https://www.transparenttextures.com/patterns/cubes.png')] opacity-10"></div>
      <div class="relative z-10 flex-1 flex flex-col justify-center">
        <div class="">
          <div class="text-display-lg font-bold tracking-tight text-white mb-4">EduConnect</div>
        </div>
        <h1 class="text-headline-lg font-bold leading-tight mb-4">Asegura tu Cuenta</h1>
        <p class="text-body-lg text-primary-fixed-dim max-w-md">
          Protegemos tu información académica con estándares de seguridad de nivel institucional.
        </p>
      </div>
      <div class="relative z-10 flex items-center gap-4 text-sm text-primary-fixed-dim">
        <span class="material-symbols-outlined">shield_lock</span>
        <span class="font-label-md text-sm tracking-wider uppercase">Seguridad de Nivel Institucional</span>
      </div>
    </div>

    <!-- Right Pane -->
    <div class="w-full md:w-1/2 lg:w-[60%] flex items-center justify-center p-6 sm:p-12 bg-surface">
      <div class="w-full max-w-md bg-surface-container-lowest rounded-2xl shadow-sm border border-outline-variant/30 p-8 sm:p-10">
        <div class="md:hidden flex items-center gap-3 mb-10 justify-center">
          <span class="text-2xl font-extrabold tracking-tighter text-primary">EduConnect</span>
        </div>

        <div class="flex flex-col items-center text-center mb-8">
          <div class="w-16 h-16 bg-secondary-fixed rounded-full flex items-center justify-center mb-6 text-primary-container">
            <span class="material-symbols-outlined text-3xl" style="font-variation-settings: 'FILL' 1;">vpn_key</span>
          </div>
          <h2 class="text-headline-lg text-on-surface mb-2 font-bold">Verificación de Dos Pasos</h2>
          <p class="font-body-md text-on-surface-variant">Sube tu archivo de llave (auth2-ayd1.txt) para continuar.</p>
        </div>

        <form class="flex flex-col gap-8" @submit.prevent="submit">
          <div :class="['drop-zone flex flex-col items-center justify-center w-full h-48 border-2 border-dashed border-outline-variant rounded-xl bg-surface-container-lowest cursor-pointer transition-all', { 'drop-zone--over': isOver && !fileRef }]"
               @dragenter.prevent="onDragEnter" @dragover.prevent @dragleave.prevent="onDragLeave" @drop.prevent="onDrop">

            <template v-if="!fileRef">
              <span class="material-symbols-outlined text-4xl text-outline mb-3">upload_file</span>
              <span class="font-body-md text-on-surface mb-1">Arrastra y suelta tu archivo aquí</span>
              <span class="text-xs text-on-surface-variant mb-4">o</span>
              <label class="px-4 py-2 bg-surface-container-high hover:bg-surface-variant text-on-surface text-sm font-semibold rounded-lg cursor-pointer transition-colors" for="file">
                Explorar archivos
                <input id="file" accept=".txt" class="hidden" type="file" @change="onFileChange" />
              </label>
            </template>

            <template v-else>
              <div class="flex flex-col items-center gap-2">
                <div class="w-14 h-14 rounded-full bg-secondary-fixed-dim flex items-center justify-center">
                  <span class="material-symbols-outlined text-3xl text-secondary">check</span>
                </div>
                <div class="text-sm font-medium text-on-surface">Archivo cargado</div>
                <div class="text-xs text-on-surface-variant">{{ fileRef.name }}</div>
              </div>
            </template>
          </div>

          <div v-if="error" class="flex items-start gap-2 p-4 bg-error-container rounded-lg text-on-error-container" id="error-message">
            <span class="material-symbols-outlined mt-0.5">error</span>
            <span class="text-sm">{{ error }}</span>
          </div>

          <div class="flex items-center justify-between gap-4">
            <BaseButton variant="primary" :loading="loading" class="flex-1" :disabled="!fileRef" type="submit">Verificar y Entrar</BaseButton>
            <div class="flex flex-col items-end gap-2">
              <button v-if="fileRef" type="button" class="text-sm text-outline hover:text-error" @click="removeFile">Quitar archivo</button>
              <button type="button" class="text-sm text-primary hover:text-secondary" @click="$router.push('/login')">Volver al login principal</button>
            </div>
          </div>
        </form>
      </div>
    </div>
  </main>
</template>

<style scoped>
.drop-zone:hover { background: #f5f7fb }
</style>
