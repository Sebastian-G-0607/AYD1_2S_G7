<script setup lang="ts">
import { computed, ref, watch } from 'vue'

interface Props {
  modelValue?: string
  minDate?: string
  maxDate?: string
  allowedDaysOfWeek?: number[]
  disabledDates?: string[]
  label?: string
  required?: boolean
  disabled?: boolean
  error?: string
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: '',
  minDate: undefined,
  maxDate: undefined,
  allowedDaysOfWeek: () => [],
  disabledDates: () => [],
  label: undefined,
  required: false,
  disabled: false,
  error: undefined
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
}>()

const pad = (n: number) => String(n).padStart(2, '0')

const getInitialDate = () => {
  if (props.modelValue && /^\d{4}-\d{2}-\d{2}$/.test(props.modelValue)) {
    const [y, m] = props.modelValue.split('-').map(Number)
    return { year: y, month: m - 1 }
  }
  if (props.minDate && /^\d{4}-\d{2}-\d{2}$/.test(props.minDate)) {
    const [y, m] = props.minDate.split('-').map(Number)
    return { year: y, month: m - 1 }
  }
  const now = new Date()
  return { year: now.getFullYear(), month: now.getMonth() }
}

const initial = getInitialDate()
const displayedYear = ref(initial.year)
const displayedMonth = ref(initial.month)

watch(
  () => props.modelValue,
  newVal => {
    if (newVal && /^\d{4}-\d{2}-\d{2}$/.test(newVal)) {
      const [y, m] = newVal.split('-').map(Number)
      displayedYear.value = y
      displayedMonth.value = m - 1
    }
  }
)

const monthNames = [
  'Enero',
  'Febrero',
  'Marzo',
  'Abril',
  'Mayo',
  'Junio',
  'Julio',
  'Agosto',
  'Septiembre',
  'Octubre',
  'Noviembre',
  'Diciembre'
]

const weekDays = ['Lun', 'Mar', 'Mié', 'Jue', 'Vie', 'Sáb', 'Dom']

const currentMonthLabel = computed(() => {
  return `${monthNames[displayedMonth.value]} ${displayedYear.value}`
})

const todayStr = computed(() => {
  const now = new Date()
  return `${now.getFullYear()}-${pad(now.getMonth() + 1)}-${pad(now.getDate())}`
})

const canGoPrev = computed(() => {
  if (!props.minDate) return true
  const [minY, minM] = props.minDate.split('-').map(Number)
  if (displayedYear.value < minY) return false
  if (displayedYear.value === minY && displayedMonth.value <= minM - 1) return false
  return true
})

const canGoNext = computed(() => {
  if (!props.maxDate) return true
  const [maxY, maxM] = props.maxDate.split('-').map(Number)
  if (displayedYear.value > maxY) return false
  if (displayedYear.value === maxY && displayedMonth.value >= maxM - 1) return false
  return true
})

const prevMonth = () => {
  if (!canGoPrev.value) return
  if (displayedMonth.value === 0) {
    displayedMonth.value = 11
    displayedYear.value--
  } else {
    displayedMonth.value--
  }
}

const nextMonth = () => {
  if (!canGoNext.value) return
  if (displayedMonth.value === 11) {
    displayedMonth.value = 0
    displayedYear.value++
  } else {
    displayedMonth.value++
  }
}

interface CalendarCell {
  dayNumber: number
  dateStr: string
  isCurrentMonth: boolean
  isDisabled: boolean
  isNotAllowedDay: boolean
  isSelected: boolean
  isToday: boolean
}

const calendarGrid = computed<CalendarCell[]>(() => {
  const cells: CalendarCell[] = []
  const year = displayedYear.value
  const month = displayedMonth.value

  const firstDayOfMonth = new Date(year, month, 1)
  const firstDayOfWeek = firstDayOfMonth.getDay() === 0 ? 7 : firstDayOfMonth.getDay()
  const leadingBlanks = firstDayOfWeek - 1

  const prevMonthTotalDays = new Date(year, month, 0).getDate()
  const prevMonthYear = month === 0 ? year - 1 : year
  const prevMonthIndex = month === 0 ? 11 : month - 1

  for (let i = leadingBlanks - 1; i >= 0; i--) {
    const dayNum = prevMonthTotalDays - i
    const dateStr = `${prevMonthYear}-${pad(prevMonthIndex + 1)}-${pad(dayNum)}`
    cells.push({
      dayNumber: dayNum,
      dateStr,
      isCurrentMonth: false,
      isDisabled: true,
      isNotAllowedDay: false,
      isSelected: false,
      isToday: dateStr === todayStr.value
    })
  }

  const daysInCurrentMonth = new Date(year, month + 1, 0).getDate()

  for (let d = 1; d <= daysInCurrentMonth; d++) {
    const dateStr = `${year}-${pad(month + 1)}-${pad(d)}`
    const dayDate = new Date(year, month, d)
    const isoDay = dayDate.getDay() === 0 ? 7 : dayDate.getDay()

    const isPast = props.minDate ? dateStr < props.minDate : false
    const isFuture = props.maxDate ? dateStr > props.maxDate : false
    const isDayAllowed =
      !props.allowedDaysOfWeek ||
      props.allowedDaysOfWeek.length === 0 ||
      props.allowedDaysOfWeek.includes(isoDay)
    const isDisabledDate = props.disabledDates.includes(dateStr)

    const isDisabled = props.disabled || isPast || isFuture || !isDayAllowed || isDisabledDate

    cells.push({
      dayNumber: d,
      dateStr,
      isCurrentMonth: true,
      isDisabled,
      isNotAllowedDay: !isDayAllowed && !isPast && !isFuture,
      isSelected: props.modelValue === dateStr,
      isToday: dateStr === todayStr.value
    })
  }

  const remaining = 7 - (cells.length % 7)
  if (remaining < 7) {
    const nextMonthYear = month === 11 ? year + 1 : year
    const nextMonthIndex = month === 11 ? 0 : month + 1
    for (let d = 1; d <= remaining; d++) {
      const dateStr = `${nextMonthYear}-${pad(nextMonthIndex + 1)}-${pad(d)}`
      cells.push({
        dayNumber: d,
        dateStr,
        isCurrentMonth: false,
        isDisabled: true,
        isNotAllowedDay: false,
        isSelected: false,
        isToday: dateStr === todayStr.value
      })
    }
  }

  return cells
})

const onSelectDate = (cell: CalendarCell) => {
  if (cell.isDisabled || !cell.isCurrentMonth) return
  emit('update:modelValue', cell.dateStr)
}
</script>

<template>
  <div class="flex flex-col gap-2 w-full">
    <div v-if="label" class="flex justify-between items-center">
      <label class="text-sm font-semibold text-on-surface">
        {{ label }}
        <span v-if="required" class="text-error">*</span>
      </label>
      <slot name="labelAction" />
    </div>

    <div
      :class="[
        'rounded-xl border bg-surface-container-lowest p-4 flex flex-col gap-3 transition-all',
        error ? 'border-error ring-1 ring-error/30' : 'border-outline-variant/30'
      ]"
    >
      <div class="flex items-center justify-between">
        <h3 class="text-sm font-bold text-on-surface capitalize">
          {{ currentMonthLabel }}
        </h3>
        <div class="flex items-center gap-1">
          <button
            type="button"
            :disabled="!canGoPrev"
            class="w-8 h-8 rounded-lg flex items-center justify-center text-on-surface-variant hover:bg-surface-container-low hover:text-on-surface disabled:opacity-30 disabled:cursor-not-allowed transition-colors"
            aria-label="Mes anterior"
            @click="prevMonth"
          >
            <span class="material-symbols-outlined text-[20px]">chevron_left</span>
          </button>
          <button
            type="button"
            :disabled="!canGoNext"
            class="w-8 h-8 rounded-lg flex items-center justify-center text-on-surface-variant hover:bg-surface-container-low hover:text-on-surface disabled:opacity-30 disabled:cursor-not-allowed transition-colors"
            aria-label="Mes siguiente"
            @click="nextMonth"
          >
            <span class="material-symbols-outlined text-[20px]">chevron_right</span>
          </button>
        </div>
      </div>

      <div class="grid grid-cols-7 gap-1 text-center">
        <span
          v-for="wd in weekDays"
          :key="wd"
          class="text-xs font-bold text-on-surface-variant/80 py-1"
        >
          {{ wd }}
        </span>
      </div>

      <div class="grid grid-cols-7 gap-1">
        <button
          v-for="(cell, index) in calendarGrid"
          :key="`${cell.dateStr}-${index}`"
          type="button"
          :disabled="cell.isDisabled"
          :title="
            cell.isNotAllowedDay
              ? 'El tutor no atiende este día'
              : cell.isDisabled
                ? 'Día no disponible'
                : cell.dateStr
          "
          :class="[
            'h-9 rounded-lg text-sm flex items-center justify-center transition-all relative font-medium',
            !cell.isCurrentMonth
              ? 'text-outline-variant/30 bg-transparent opacity-20 pointer-events-none'
              : '',
            cell.isCurrentMonth && cell.isDisabled && cell.isNotAllowedDay
              ? 'bg-surface-container-low/40 text-outline-variant/40 cursor-not-allowed opacity-40 select-none'
              : '',
            cell.isCurrentMonth && cell.isDisabled && !cell.isNotAllowedDay
              ? 'bg-transparent text-outline-variant/30 cursor-not-allowed opacity-30 select-none'
              : '',
            cell.isCurrentMonth && !cell.isDisabled && !cell.isSelected
              ? 'bg-surface-container-low text-on-surface hover:bg-primary-container hover:text-on-primary-container border border-outline-variant/30 hover:border-primary/50 cursor-pointer font-semibold'
              : '',
            cell.isCurrentMonth && cell.isSelected
              ? 'bg-primary text-on-primary font-bold shadow-sm ring-2 ring-primary ring-offset-2 ring-offset-surface z-10'
              : '',
            cell.isCurrentMonth && cell.isToday && !cell.isSelected
              ? 'border-primary/60 font-bold'
              : ''
          ]"
          @click="onSelectDate(cell)"
        >
          {{ cell.dayNumber }}
        </button>
      </div>

      <div
        class="flex flex-wrap items-center justify-between gap-2 pt-2 border-t border-outline-variant/20 text-xs text-on-surface-variant"
      >
        <div class="flex items-center gap-3">
          <span class="flex items-center gap-1.5">
            <span
              class="w-2.5 h-2.5 rounded-sm bg-surface-container-low border border-outline-variant/50"
            />
            Atención
          </span>
          <span class="flex items-center gap-1.5">
            <span
              class="w-2.5 h-2.5 rounded-sm bg-surface-container-low/40 border border-transparent opacity-40"
            />
            No atiende
          </span>
          <span class="flex items-center gap-1.5">
            <span class="w-2.5 h-2.5 rounded-sm bg-primary" />
            Seleccionado
          </span>
        </div>
      </div>
    </div>

    <p v-if="error" class="text-xs text-error font-medium flex items-center gap-1 mt-0.5">
      <span class="material-symbols-outlined text-[16px]">info</span>
      {{ error }}
    </p>
  </div>
</template>
