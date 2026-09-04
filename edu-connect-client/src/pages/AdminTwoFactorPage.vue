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

function onFileChange(e: Event) {
  const input = e.target as HTMLInputElement
  const f = input.files && input.files[0]
  if (f && f.name.endsWith('.txt')) {
    fileRef.value = f
    error.value = null
  } else {
    error.value = 'Selecciona un archivo .txt válido llamado auth2-ayd1.txt'
    fileRef.value = null
  }
}

function onDrop(e: DragEvent) {
  e.preventDefault()
  const f = e.dataTransfer?.files?.[0]
  if (f) {
    if (f.name.endsWith('.txt')) fileRef.value = f
    else error.value = 'Solo archivos .txt'
  }
}

async function submit() {
  if (!fileRef.value) {
    error.value = 'Por favor, selecciona el archivo de llave.'
    return
  }

  loading.value = true
  error.value = null

  // obtener tempToken de sessionStorage o query param
  const qs = new URLSearchParams(window.location.search)
  const temp = qs.get('tempToken') || sessionStorage.getItem('edu_temp_token') || ''

  try {
    const result = await authService.uploadAdmin2Fa(fileRef.value, temp || undefined)
    // guardar token final y user
    const token = result.token
    // parse payload
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
  <div class="w-full min-h-screen flex items-center justify-center p-6 bg-surface">
    <div class="w-full max-w-md bg-white rounded-2xl shadow p-8">
      <div class="flex flex-col items-center text-center mb-6">
        <div class="w-16 h-16 bg-secondary-fixed rounded-full flex items-center justify-center mb-4 text-primary-container">
          <span class="material-symbols-outlined">vpn_key</span>
        </div>
        <h2 class="text-xl font-bold">Verificación de Dos Pasos</h2>
        <p class="text-sm text-on-surface-variant">Sube tu archivo de llave (auth2-ayd1.txt) para continuar.</p>
      </div>

      <div class="mb-4">
        <div
          class="drop-zone border-2 border-dashed rounded-lg p-6 flex flex-col items-center justify-center text-center cursor-pointer"
          @drop.prevent="onDrop"
          @dragover.prevent
        >
          <p class="text-sm mb-2">Arrastra y suelta tu archivo aquí</p>
          <label class="px-4 py-2 bg-surface-container-high rounded cursor-pointer">
            <input id="file" type="file" accept=".txt" class="hidden" @change="onFileChange" />
            Explorar archivos
          </label>
        </div>
        <div v-if="fileRef" class="mt-3 text-sm">Archivo seleccionado: {{ fileRef.name }}</div>
        <div v-if="error" class="mt-3 text-sm text-red-600">{{ error }}</div>
      </div>

      <div class="flex gap-3">
        <BaseButton variant="primary" :loading="loading" class="flex-1" @click="submit">Verificar y Entrar</BaseButton>
        <BaseButton variant="ghost" class="flex-1" @click="$router.push('/login')">Volver</BaseButton>
      </div>
    </div>
  </div>
</template>

<style scoped>
.drop-zone:hover { background: #f5f7fb }
</style>
