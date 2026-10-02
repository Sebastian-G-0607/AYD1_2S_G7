<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { RouterLink, useRoute, useRouter } from 'vue-router'
import { BaseButton, BaseAlert, BaseOtpInput } from '@/components/ui'
import { useAuth } from '../composables/useAuth'

interface Props {
  email?: string
}

const props = defineProps<Props>()

const emit = defineEmits<{
  (e: 'verify', code: string): void
}>()

const route = useRoute()
const router = useRouter()

const token = ref('')
const timeLeft = ref(60)
const redirectCountdown = ref(0)
let timerId: ReturnType<typeof setInterval> | null = null
let redirectTimerId: ReturnType<typeof setInterval> | null = null

const {
  isLoading,
  errorMessage,
  successMessage,
  clearError,
  clearSuccess,
  verifyEmail
} = useAuth()

const resolvedEmail = computed(() => {
  if (props.email) return props.email
  if (typeof route.query.email === 'string' && route.query.email.trim()) {
    return route.query.email.trim()
  }
  const stored = sessionStorage.getItem('edu_email_validation_email')
  if (stored && stored.trim()) {
    return stored.trim()
  }
  return ''
})

const maskedEmail = computed(() => {
  const email = resolvedEmail.value
  if (!email || !email.includes('@')) return ''
  const [local, domain] = email.split('@')
  if (!local || !domain) return email
  const visibleChar = local.charAt(0)
  return `${visibleChar}*****@${domain}`
})

const formattedCountdown = computed(() => {
  const minutes = Math.floor(timeLeft.value / 60)
  const seconds = timeLeft.value % 60
  const formattedMin = minutes < 10 ? `0${minutes}` : `${minutes}`
  const formattedSec = seconds < 10 ? `0${seconds}` : `${seconds}`
  return `${formattedMin}:${formattedSec}`
})

function startTimer(initialSeconds = 60) {
  stopTimer()
  timeLeft.value = initialSeconds
  timerId = setInterval(() => {
    if (timeLeft.value > 0) {
      timeLeft.value--
    } else {
      stopTimer()
    }
  }, 1000)
}

function stopTimer() {
  if (timerId !== null) {
    clearInterval(timerId)
    timerId = null
  }
}

function triggerSessionExpiredRedirect(baseMessage = 'Tu sesión de verificación ha expirado.', seconds = 6) {
  stopTimer()
  if (redirectTimerId !== null) {
    clearInterval(redirectTimerId)
  }
  redirectCountdown.value = seconds
  errorMessage.value = `${baseMessage} Serás redirigido al inicio de sesión en ${redirectCountdown.value} segundos...`

  redirectTimerId = setInterval(async () => {
    redirectCountdown.value--
    if (redirectCountdown.value > 0) {
      errorMessage.value = `${baseMessage} Serás redirigido al inicio de sesión en ${redirectCountdown.value} segundos...`
    } else {
      if (redirectTimerId !== null) {
        clearInterval(redirectTimerId)
        redirectTimerId = null
      }
      sessionStorage.removeItem('edu_email_validation_token')
      sessionStorage.removeItem('edu_email_validation_email')
      await router.push('/login')
    }
  }, 1000)
}

function handleResend() {
  if (redirectCountdown.value > 0) return
  startTimer(60)
}

function onComplete(code: string) {
  token.value = code
}

async function handleSubmit() {
  if (redirectCountdown.value > 0) return

  if (token.value.trim().length !== 6) {
    errorMessage.value = 'Debes ingresar el código completo de 6 dígitos.'
    return
  }

  clearError()
  clearSuccess()
  emit('verify', token.value.trim())
  const ok = await verifyEmail(token.value.trim())
  if (!ok) {
    const tempToken = sessionStorage.getItem('edu_email_validation_token')
    if (
      !tempToken ||
      (errorMessage.value && (
        errorMessage.value.toLowerCase().includes('sesión') ||
        errorMessage.value.toLowerCase().includes('no autorizado') ||
        errorMessage.value.toLowerCase().includes('token de validación es inválido')
      ))
    ) {
      triggerSessionExpiredRedirect(errorMessage.value || 'Tu sesión de verificación ha expirado.', 6)
    }
  }
}

onMounted(() => {
  const tempToken = sessionStorage.getItem('edu_email_validation_token')
  if (!tempToken) {
    triggerSessionExpiredRedirect('No se encontró una sesión de verificación activa.', 6)
    return
  }
  startTimer(60)
})

onUnmounted(() => {
  stopTimer()
  if (redirectTimerId !== null) {
    clearInterval(redirectTimerId)
    redirectTimerId = null
  }
})
</script>

<template>
  <div class="w-full flex flex-col items-center relative z-10">
    <div
      class="w-14 h-14 rounded-full bg-secondary-fixed flex items-center justify-center mb-6 shadow-sm"
    >
      <span
        class="material-symbols-outlined text-secondary text-[28px]"
        style="font-variation-settings: 'FILL' 1"
      >
        verified_user
      </span>
    </div>

    <div class="text-center mb-8">
      <h1 class="text-3xl font-bold text-primary font-headline tracking-tight mb-2">
        Verificación en dos pasos
      </h1>
      <p class="text-base text-on-surface-variant font-body max-w-sm mx-auto leading-relaxed">
        <template v-if="maskedEmail">
          Hemos enviado un código de 6 dígitos a tu correo electrónico
          <span class="font-semibold text-primary">{{ maskedEmail }}</span
          >. Ingrésalo para continuar.
        </template>
        <template v-else>
          Hemos enviado un código de 6 dígitos a tu correo electrónico. Ingrésalo para continuar.
        </template>
      </p>
    </div>

    <BaseAlert
      v-if="errorMessage"
      type="error"
      :title="redirectCountdown > 0 ? 'Sesión expirada' : 'Error de verificación'"
      :message="errorMessage"
      class="w-full mb-6"
      @dismiss="clearError"
    />

    <BaseAlert
      v-if="successMessage"
      type="success"
      title="Código enviado"
      :message="successMessage"
      class="w-full mb-6"
      @dismiss="clearSuccess"
    />

    <form class="w-full flex flex-col items-center" @submit.prevent="handleSubmit">
      <div class="w-full flex justify-center mb-8">
        <BaseOtpInput
          v-model="token"
          :length="6"
          :disabled="isLoading || redirectCountdown > 0"
          :error="Boolean(errorMessage)"
          @complete="onComplete"
        />
      </div>

      <BaseButton
        type="submit"
        variant="primary"
        size="lg"
        block
        :loading="isLoading"
        :disabled="token.trim().length !== 6 || isLoading || redirectCountdown > 0"
        class="group cursor-pointer !rounded-xl !py-4 shadow-md hover:shadow-lg"
      >
        <span>Verificar e Ingresar</span>
        <span
          class="material-symbols-outlined text-[18px] transition-transform duration-200 group-hover:translate-x-1"
        >
          arrow_forward
        </span>
      </BaseButton>

      <div class="mt-8 flex flex-col items-center gap-2 text-center">
        <p
          class="text-sm font-body text-on-surface-variant flex items-center justify-center gap-1.5"
        >
          <span class="material-symbols-outlined text-[16px] text-outline">schedule</span>
          <span>¿No recibiste el código?</span>
        </p>
        <div>
          <span v-if="timeLeft > 0" class="text-sm font-semibold text-secondary">
            Reenviar código en <span>{{ formattedCountdown }}</span>
          </span>
          <button
            v-else
            type="button"
            class="text-sm font-semibold text-secondary hover:text-secondary-container transition-colors underline underline-offset-4 cursor-pointer focus:outline-none"
            @click="handleResend"
          >
            Reenviar código ahora
          </button>
        </div>
      </div>

      <div
        class="mt-12 pt-6 w-full flex flex-col items-center border-t-0 bg-gradient-to-r from-transparent via-surface-container to-transparent h-[1px]"
      />

      <div class="w-full mt-6 flex items-center justify-between">
        <RouterLink
          to="/login"
          class="inline-flex items-center gap-2 text-on-surface-variant hover:text-primary text-sm font-semibold transition-colors duration-150 group"
        >
          <span
            class="material-symbols-outlined text-[18px] transition-transform duration-150 group-hover:-translate-x-1"
          >
            arrow_back
          </span>
          <span>Volver al inicio de sesión</span>
        </RouterLink>
        <a
          href="mailto:soporte@educonnect.edu"
          class="inline-flex items-center gap-1 text-outline hover:text-secondary text-xs font-medium transition-colors duration-150"
          title="Ayuda técnica de EduConnect"
        >
          <span class="material-symbols-outlined text-[16px]">support_agent</span>
          <span>Soporte</span>
        </a>
      </div>
    </form>
  </div>
</template>
