<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { RouterLink, useRoute } from 'vue-router'
import { BaseButton, BaseAlert, BaseOtpInput } from '@/components/ui'

interface Props {
  email?: string
}

const props = defineProps<Props>()

const emit = defineEmits<{
  (e: 'verify', code: string): void
  (e: 'resend'): void
}>()

const route = useRoute()

const token = ref('')
const isLoading = ref(false)
const errorMessage = ref<string | null>(null)
const successMessage = ref<string | null>(null)
const timeLeft = ref(60)
let timerId: ReturnType<typeof setInterval> | null = null

const resolvedEmail = computed(() => {
  if (props.email) return props.email
  if (typeof route.query.email === 'string' && route.query.email.trim()) {
    return route.query.email.trim()
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
  const seconds = timeLeft.value
  const formattedSec = seconds < 10 ? `0${seconds}` : `${seconds}`
  return `00:${formattedSec}`
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

function handleResend() {
  errorMessage.value = null
  successMessage.value = 'Se ha generado un nuevo código de verificación.'
  startTimer(60)
  emit('resend')
}

function onComplete(code: string) {
  token.value = code
}

async function handleSubmit() {
  if (token.value.trim().length !== 6) {
    errorMessage.value = 'Debes ingresar el código completo de 6 dígitos.'
    return
  }

  errorMessage.value = null
  isLoading.value = true

  emit('verify', token.value.trim())

  setTimeout(() => {
    isLoading.value = false
  }, 1200)
}

onMounted(() => {
  startTimer(60)
})

onUnmounted(() => {
  stopTimer()
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
          Hemos enviado un código de 6 dígitos a tu correo institucional
          <span class="font-semibold text-primary">{{ maskedEmail }}</span
          >. Ingrésalo para continuar.
        </template>
        <template v-else>
          Hemos enviado un código de 6 dígitos a tu correo institucional. Ingrésalo para continuar.
        </template>
      </p>
    </div>

    <BaseAlert
      v-if="errorMessage"
      type="error"
      title="Error de verificación"
      :message="errorMessage"
      class="w-full mb-6"
      @dismiss="errorMessage = null"
    />

    <BaseAlert
      v-if="successMessage"
      type="success"
      title="Código enviado"
      :message="successMessage"
      class="w-full mb-6"
      @dismiss="successMessage = null"
    />

    <form class="w-full flex flex-col items-center" @submit.prevent="handleSubmit">
      <div class="w-full flex justify-center mb-8">
        <BaseOtpInput
          v-model="token"
          :length="6"
          :disabled="isLoading"
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
        :disabled="token.trim().length !== 6 || isLoading"
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
