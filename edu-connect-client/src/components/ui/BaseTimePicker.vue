<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref, watch } from 'vue'

export interface TimeSlot {
  horaInicio: string
  horaFin: string
}

interface Props {
  modelValue?: string
  minTime?: string
  maxTime?: string
  stepMinutes?: number
  durationMinutes?: number
  occupiedSlots?: TimeSlot[]
  label?: string
  placeholder?: string
  id?: string
  name?: string
  required?: boolean
  disabled?: boolean
  error?: string
  hint?: string
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: '',
  minTime: '08:00',
  maxTime: '18:00',
  stepMinutes: 15,
  durationMinutes: 60,
  occupiedSlots: () => [],
  label: undefined,
  placeholder: 'Selecciona una hora',
  id: undefined,
  name: undefined,
  required: false,
  disabled: false,
  error: undefined,
  hint: undefined
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
  (e: 'change', value: string): void
}>()

const isOpen = ref(false)
const containerRef = ref<HTMLElement | null>(null)

const pad = (n: number) => String(n).padStart(2, '0')

function timeToMinutes(timeStr: string): number {
  if (!timeStr || !timeStr.includes(':')) return 0
  const [h, m] = timeStr.split(':').map(Number)
  return (h || 0) * 60 + (m || 0)
}

const parsedMinTime = computed(() => {
  return props.minTime ? props.minTime.slice(0, 5) : '08:00'
})

const parsedMaxTime = computed(() => {
  return props.maxTime ? props.maxTime.slice(0, 5) : '18:00'
})

const minMinutes = computed(() => timeToMinutes(parsedMinTime.value))
const maxMinutes = computed(() => timeToMinutes(parsedMaxTime.value))

const startHourNum = computed(() => Math.floor(minMinutes.value / 60))
const endHourNum = computed(() => Math.floor(maxMinutes.value / 60))

const availableHours = computed(() => {
  const list: string[] = []
  for (let h = startHourNum.value; h <= endHourNum.value; h++) {
    list.push(pad(h))
  }
  return list
})

const selectedHour = ref('')
const selectedMinute = ref('')

function syncFromModelValue(val?: string) {
  if (val && /^\d{2}:\d{2}/.test(val)) {
    const [h, m] = val.slice(0, 5).split(':')
    selectedHour.value = h
    selectedMinute.value = m
  } else {
    selectedHour.value = ''
    selectedMinute.value = ''
  }
}

syncFromModelValue(props.modelValue)

watch(
  () => props.modelValue,
  newVal => {
    syncFromModelValue(newVal)
  }
)

const activeHour = computed(() => {
  if (selectedHour.value && availableHours.value.includes(selectedHour.value)) {
    return selectedHour.value
  }
  return availableHours.value[0] || '08'
})

const minuteOptions = computed(() => {
  const list: string[] = []
  const step = props.stepMinutes > 0 ? props.stepMinutes : 15
  for (let m = 0; m < 60; m += step) {
    list.push(pad(m))
  }
  return list
})

function isMinuteDisabled(hourStr: string, minStr: string): boolean {
  const h = Number(hourStr)
  const m = Number(minStr)
  const totalMin = h * 60 + m
  if (totalMin < minMinutes.value || totalMin > maxMinutes.value) {
    return true
  }
  const sessionStart = totalMin
  const sessionEnd = totalMin + props.durationMinutes
  for (const slot of props.occupiedSlots) {
    const slotStart = timeToMinutes(slot.horaInicio.slice(0, 5))
    const slotEnd = timeToMinutes(slot.horaFin.slice(0, 5))
    if (sessionStart < slotEnd && sessionEnd > slotStart) {
      return true
    }
  }
  return false
}

function isMinuteOccupied(hourStr: string, minStr: string): boolean {
  const totalMin = Number(hourStr) * 60 + Number(minStr)
  const sessionStart = totalMin
  const sessionEnd = totalMin + props.durationMinutes
  for (const slot of props.occupiedSlots) {
    const slotStart = timeToMinutes(slot.horaInicio.slice(0, 5))
    const slotEnd = timeToMinutes(slot.horaFin.slice(0, 5))
    if (sessionStart < slotEnd && sessionEnd > slotStart) {
      return true
    }
  }
  return false
}

function selectHour(h: string) {
  selectedHour.value = h
  if (selectedMinute.value && !isMinuteDisabled(h, selectedMinute.value)) {
    const fullTime = `${h}:${selectedMinute.value}`
    emit('update:modelValue', fullTime)
    emit('change', fullTime)
  } else {
    const firstValid = minuteOptions.value.find(m => !isMinuteDisabled(h, m))
    if (firstValid) {
      selectedMinute.value = firstValid
      const fullTime = `${h}:${firstValid}`
      emit('update:modelValue', fullTime)
      emit('change', fullTime)
    }
  }
}

function selectMinute(m: string) {
  if (isMinuteDisabled(activeHour.value, m)) return
  selectedHour.value = activeHour.value
  selectedMinute.value = m
  const fullTime = `${activeHour.value}:${m}`
  emit('update:modelValue', fullTime)
  emit('change', fullTime)
  isOpen.value = false
}

function toggleDropdown() {
  if (props.disabled) return
  isOpen.value = !isOpen.value
  if (isOpen.value && !selectedHour.value) {
    const initialH = availableHours.value[0] || '08'
    selectedHour.value = initialH
    const initialM = minuteOptions.value.find(m => !isMinuteDisabled(initialH, m)) || '00'
    selectedMinute.value = initialM
    const fullTime = `${initialH}:${initialM}`
    emit('update:modelValue', fullTime)
    emit('change', fullTime)
  }
}

function clearValue(event: MouseEvent) {
  event.stopPropagation()
  selectedHour.value = ''
  selectedMinute.value = ''
  emit('update:modelValue', '')
  emit('change', '')
}

function handleClickOutside(event: MouseEvent) {
  if (containerRef.value && !containerRef.value.contains(event.target as Node)) {
    isOpen.value = false
  }
}

function handleKeydown(event: KeyboardEvent) {
  if (event.key === 'Escape' && isOpen.value) {
    isOpen.value = false
  }
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
  document.addEventListener('keydown', handleKeydown)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
  document.removeEventListener('keydown', handleKeydown)
})
</script>

<template>
  <div ref="containerRef" class="flex flex-col gap-2 relative w-full select-none">
    <div v-if="label || $slots.labelAction" class="flex justify-between items-center">
      <label
        v-if="label"
        :for="id"
        class="text-sm font-semibold text-on-surface transition-colors flex items-center gap-1.5"
      >
        {{ label }}
        <span v-if="required" class="text-error">*</span>
      </label>
      <slot name="labelAction" />
    </div>

    <div class="relative">
      <button
        :id="id"
        :name="name"
        type="button"
        :disabled="disabled"
        :class="[
          'w-full bg-surface-container-low py-3 px-4 rounded-lg text-base font-body border border-outline-variant focus:outline-none focus:ring-2 focus:ring-primary focus:bg-surface-container-lowest focus:border-transparent transition-all flex items-center justify-between text-left disabled:opacity-60 disabled:cursor-not-allowed',
          error ? 'border-error bg-error-container/20 focus:ring-error' : '',
          isOpen ? 'ring-2 ring-primary border-transparent bg-surface-container-lowest' : ''
        ]"
        @click="toggleDropdown"
      >
        <div class="flex items-center gap-3 min-w-0">
          <span
            :class="[
              'material-symbols-outlined text-[20px] transition-colors',
              error ? 'text-error' : isOpen ? 'text-primary' : 'text-on-surface-variant'
            ]"
          >
            schedule
          </span>
          <span
            v-if="modelValue"
            class="text-on-surface font-semibold tracking-wide"
          >
            {{ modelValue }} <span class="text-xs font-normal text-on-surface-variant ml-1">hrs</span>
          </span>
          <span v-else class="text-outline-variant">
            {{ placeholder }}
          </span>
        </div>

        <div class="flex items-center gap-1.5 shrink-0">
          <span
            v-if="modelValue && !disabled"
            class="material-symbols-outlined text-on-surface-variant hover:text-error text-[18px] p-0.5 rounded-full hover:bg-surface-container-high transition-colors"
            title="Borrar hora seleccionada"
            @click="clearValue"
          >
            close
          </span>
          <span
            class="material-symbols-outlined text-on-surface-variant text-[20px] transition-transform duration-200"
            :class="isOpen ? 'rotate-180 text-primary' : ''"
          >
            expand_more
          </span>
        </div>
      </button>

      <div
        v-if="isOpen"
        class="absolute z-50 mt-1.5 left-0 right-0 sm:right-auto sm:w-[320px] bg-surface rounded-xl border border-outline-variant/60 shadow-xl p-3 flex flex-col gap-3"
      >
        <div class="flex items-center justify-between border-b border-outline-variant/30 pb-2">
          <div class="flex items-center gap-1.5 text-xs font-bold text-on-surface">
            <span class="material-symbols-outlined text-[16px] text-primary">schedule</span>
            <span>Horario de inicio</span>
          </div>
          <span class="text-[11px] font-semibold text-on-surface-variant bg-surface-container-high px-2 py-0.5 rounded-md">
            {{ parsedMinTime }} - {{ parsedMaxTime }}
          </span>
        </div>

        <div class="grid grid-cols-[1fr_1fr] gap-3">
          <div class="flex flex-col gap-1.5">
            <span class="text-[11px] font-bold text-on-surface-variant uppercase tracking-wider text-center">
              Hora (24h)
            </span>
            <div class="flex flex-col gap-1 max-h-[180px] overflow-y-auto pr-1">
              <button
                v-for="h in availableHours"
                :key="h"
                type="button"
                :class="[
                  'py-2 px-3 rounded-lg text-xs font-bold text-center transition-all',
                  activeHour === h
                    ? 'bg-primary text-on-primary shadow-xs ring-2 ring-primary ring-offset-1'
                    : 'bg-surface-container-low text-on-surface hover:bg-primary-container/20 hover:text-primary border border-outline-variant/30'
                ]"
                @click="selectHour(h)"
              >
                {{ h }}:00
              </button>
            </div>
          </div>

          <div class="flex flex-col gap-1.5">
            <span class="text-[11px] font-bold text-on-surface-variant uppercase tracking-wider text-center">
              Minuto
            </span>
            <div class="flex flex-col gap-1 max-h-[180px] overflow-y-auto pr-1">
              <button
                v-for="m in minuteOptions"
                :key="m"
                type="button"
                :disabled="isMinuteDisabled(activeHour, m)"
                :title="
                  isMinuteOccupied(activeHour, m)
                    ? 'Horario con sesión ocupada'
                    : isMinuteDisabled(activeHour, m)
                      ? 'Fuera de horario'
                      : `Seleccionar :${m}`
                "
                :class="[
                  'py-2 px-2.5 rounded-lg text-xs font-semibold text-center border transition-all flex items-center justify-between',
                  isMinuteDisabled(activeHour, m)
                    ? 'bg-surface-container-high/60 text-outline-variant border-outline-variant/20 line-through cursor-not-allowed opacity-50'
                    : selectedHour === activeHour && selectedMinute === m
                      ? 'bg-primary text-on-primary border-primary font-bold shadow-xs'
                      : 'bg-surface-container-low text-on-surface border-outline-variant/30 hover:border-primary/50 hover:bg-primary-container/20 cursor-pointer'
                ]"
                @click="selectMinute(m)"
              >
                <span>:{{ m }}</span>
                <span
                  v-if="isMinuteOccupied(activeHour, m)"
                  class="text-[9px] text-error font-bold uppercase no-underline inline-block ml-1"
                >
                  Ocupado
                </span>
                <span
                  v-else-if="selectedHour === activeHour && selectedMinute === m"
                  class="material-symbols-outlined text-[14px] text-on-primary"
                >
                  check
                </span>
              </button>
            </div>
          </div>
        </div>

        <div class="flex items-center justify-between pt-2 border-t border-outline-variant/30 text-xs">
          <span class="text-on-surface-variant font-medium">
            Seleccionado: <strong class="text-primary">{{ modelValue || '--:--' }}</strong>
          </span>
          <button
            type="button"
            class="px-3 py-1 rounded-md text-xs font-semibold text-primary hover:bg-primary-container/20 transition-colors"
            @click="isOpen = false"
          >
            Listo
          </button>
        </div>
      </div>
    </div>

    <p v-if="error" class="text-xs text-error font-medium flex items-center gap-1 mt-0.5">
      <span class="material-symbols-outlined text-[16px]">info</span>
      {{ error }}
    </p>
    <p v-else-if="hint" class="text-xs text-on-surface-variant mt-0.5">
      {{ hint }}
    </p>
  </div>
</template>
