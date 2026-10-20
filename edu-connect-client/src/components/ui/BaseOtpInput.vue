<script setup lang="ts">
import { ref, watch, onMounted, nextTick } from 'vue'

interface Props {
  modelValue?: string
  length?: number
  disabled?: boolean
  error?: boolean
  autofocus?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: '',
  length: 6,
  disabled: false,
  error: false,
  autofocus: true
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
  (e: 'complete', value: string): void
  (e: 'change', value: string): void
}>()

const digits = ref<string[]>(Array.from({ length: props.length }, () => ''))
const inputRefs = ref<(HTMLInputElement | null)[]>([])

function setInputRef(el: unknown, index: number) {
  if (el instanceof HTMLInputElement) {
    inputRefs.value[index] = el
  } else {
    inputRefs.value[index] = null
  }
}

function syncDigitsFromValue(val: string) {
  const cleanVal = (val || '').replace(/\D/g, '').slice(0, props.length)
  digits.value = Array.from({ length: props.length }, (_, i) => cleanVal[i] || '')
}

watch(
  () => props.modelValue,
  newVal => {
    const current = digits.value.join('')
    if (newVal !== current) {
      syncDigitsFromValue(newVal)
    }
  },
  { immediate: true }
)

watch(
  () => props.length,
  newLength => {
    inputRefs.value = Array.from({ length: newLength }, () => null)
    syncDigitsFromValue(props.modelValue)
  }
)

function handleInput(index: number, event: Event) {
  const target = event.target as HTMLInputElement
  const rawValue = target.value
  const sanitized = rawValue.replace(/\D/g, '')

  if (sanitized.length > 1) {
    handlePastedDigits(sanitized, index)
    return
  }

  const char = sanitized.slice(-1)
  digits.value[index] = char
  target.value = char

  const currentValue = digits.value.join('')
  emit('update:modelValue', currentValue)
  emit('change', currentValue)

  if (char && index < props.length - 1) {
    nextTick(() => {
      inputRefs.value[index + 1]?.focus()
      inputRefs.value[index + 1]?.select()
    })
  }

  if (currentValue.length === props.length) {
    emit('complete', currentValue)
  }
}

function handleKeyDown(index: number, event: KeyboardEvent) {
  if (event.key === 'Backspace') {
    if (!digits.value[index] && index > 0) {
      digits.value[index - 1] = ''
      emit('update:modelValue', digits.value.join(''))
      emit('change', digits.value.join(''))
      nextTick(() => {
        inputRefs.value[index - 1]?.focus()
      })
    } else {
      digits.value[index] = ''
      emit('update:modelValue', digits.value.join(''))
      emit('change', digits.value.join(''))
    }
    return
  }

  if (event.key === 'ArrowLeft' && index > 0) {
    event.preventDefault()
    inputRefs.value[index - 1]?.focus()
    inputRefs.value[index - 1]?.select()
    return
  }

  if (event.key === 'ArrowRight' && index < props.length - 1) {
    event.preventDefault()
    inputRefs.value[index + 1]?.focus()
    inputRefs.value[index + 1]?.select()
  }
}

function handlePastedDigits(pastedText: string, startIndex = 0) {
  const cleanDigits = pastedText.replace(/\D/g, '')
  if (!cleanDigits) return

  const newDigits = [...digits.value]
  for (let i = 0; i < cleanDigits.length && startIndex + i < props.length; i++) {
    newDigits[startIndex + i] = cleanDigits[i]
  }

  digits.value = newDigits
  const currentValue = newDigits.join('')
  emit('update:modelValue', currentValue)
  emit('change', currentValue)

  const nextFocusIndex = Math.min(startIndex + cleanDigits.length, props.length - 1)
  nextTick(() => {
    inputRefs.value[nextFocusIndex]?.focus()
    inputRefs.value[nextFocusIndex]?.select()
  })

  if (currentValue.length === props.length) {
    emit('complete', currentValue)
  }
}

function handlePaste(event: ClipboardEvent) {
  event.preventDefault()
  const pastedData = event.clipboardData?.getData('text') || ''
  handlePastedDigits(pastedData, 0)
}

onMounted(() => {
  if (props.autofocus) {
    nextTick(() => {
      inputRefs.value[0]?.focus()
    })
  }
})
</script>

<template>
  <div class="flex items-center justify-between gap-2 sm:gap-3 w-full max-w-sm">
    <input
      v-for="(_, index) in length"
      :key="index"
      :ref="el => setInputRef(el, index)"
      :value="digits[index]"
      type="text"
      inputmode="numeric"
      pattern="[0-9]*"
      maxlength="1"
      autocomplete="one-time-code"
      :aria-label="`Dígito ${index + 1}`"
      :disabled="disabled"
      :class="[
        'w-12 h-14 sm:w-14 sm:h-14 bg-surface-container-low text-center font-headline text-2xl font-bold text-primary rounded-xl border border-transparent focus:border-outline-variant focus:bg-surface-container-lowest focus:shadow-md focus:outline-none focus:ring-2 focus:ring-primary transition-all duration-200 select-all disabled:opacity-60 disabled:cursor-not-allowed',
        error ? 'border-error bg-error-container/20 focus:ring-error' : ''
      ]"
      @input="handleInput(index, $event)"
      @keydown="handleKeyDown(index, $event)"
      @paste="handlePaste"
      @focus="($event.target as HTMLInputElement)?.select()"
    />
  </div>
</template>
