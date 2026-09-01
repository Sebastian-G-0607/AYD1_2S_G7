<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import BaseBadge from './BaseBadge.vue'
import type { SelectOption } from './BaseSelect.vue'

interface Props {
  modelValue?: (string | number)[]
  options: SelectOption[]
  id?: string
  name?: string
  label?: string
  placeholder?: string
  searchPlaceholder?: string
  required?: boolean
  disabled?: boolean
  loading?: boolean
  error?: string
  hint?: string
  icon?: string
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: () => [],
  options: () => [],
  id: undefined,
  name: undefined,
  label: undefined,
  placeholder: 'Buscar o seleccionar materias...',
  searchPlaceholder: 'Escribe para filtrar opciones...',
  required: false,
  disabled: false,
  loading: false,
  error: undefined,
  hint: undefined,
  icon: 'search'
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: (string | number)[]): void
  (e: 'change', value: (string | number)[]): void
}>()

const isOpen = ref(false)
const searchQuery = ref('')
const containerRef = ref<HTMLElement | null>(null)
const inputRef = ref<HTMLInputElement | null>(null)

const normalizeText = (text: string): string => {
  return text
    .toLowerCase()
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
}

const filteredOptions = computed(() => {
  const query = normalizeText(searchQuery.value.trim())
  if (!query) {
    return props.options
  }
  return props.options.filter(opt => normalizeText(opt.label).includes(query))
})

const selectedItems = computed(() => {
  return props.options.filter(opt => props.modelValue.includes(opt.value))
})

const isSelected = (value: string | number) => {
  return props.modelValue.includes(value)
}

const toggleOption = (value: string | number) => {
  if (props.disabled) return
  const current = [...props.modelValue]
  const index = current.indexOf(value)
  if (index === -1) {
    current.push(value)
  } else {
    current.splice(index, 1)
  }
  emit('update:modelValue', current)
  emit('change', current)
}

const removeOption = (value: string | number) => {
  if (props.disabled) return
  const updated = props.modelValue.filter(v => v !== value)
  emit('update:modelValue', updated)
  emit('change', updated)
}

const clearAll = () => {
  if (props.disabled) return
  emit('update:modelValue', [])
  emit('change', [])
}

const openDropdown = () => {
  if (props.disabled || props.loading) return
  isOpen.value = true
  inputRef.value?.focus()
}

const toggleDropdown = () => {
  if (props.disabled || props.loading) return
  isOpen.value = !isOpen.value
  if (isOpen.value) {
    inputRef.value?.focus()
  }
}

const handleClickOutside = (event: MouseEvent) => {
  if (containerRef.value && !containerRef.value.contains(event.target as Node)) {
    isOpen.value = false
  }
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
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
        <span
          v-if="selectedItems.length > 0"
          class="text-xs font-semibold text-secondary bg-secondary-container/40 px-2 py-0.5 rounded-full ml-1"
        >
          {{ selectedItems.length }}
        </span>
      </label>
      <slot name="labelAction" />
    </div>

    <div class="relative w-full">
      <div
        :class="[
          'w-full min-h-[48px] bg-surface-container-low rounded-lg border transition-all flex items-center cursor-pointer',
          isOpen
            ? 'ring-2 ring-primary border-transparent bg-surface-container-lowest'
            : 'border-outline-variant hover:border-outline',
          error ? 'border-error bg-error-container/20 ring-error' : '',
          disabled ? 'opacity-60 cursor-not-allowed' : ''
        ]"
        @click="openDropdown"
      >
        <div class="pl-3.5 pr-2 flex items-center pointer-events-none text-on-surface-variant">
          <span class="material-symbols-outlined text-[20px]">
            {{ icon }}
          </span>
        </div>

        <input
          :id="id"
          ref="inputRef"
          v-model="searchQuery"
          type="text"
          :name="name"
          :placeholder="
            selectedItems.length > 0 ? `Buscar entre ${options.length} materias...` : placeholder
          "
          :disabled="disabled || loading"
          autocomplete="off"
          class="flex-1 bg-transparent py-2.5 text-base text-on-surface placeholder:text-outline-variant outline-none font-body min-w-0"
          @focus="isOpen = true"
        />

        <div class="pr-3 flex items-center gap-1.5 shrink-0">
          <button
            v-if="searchQuery"
            type="button"
            class="text-on-surface-variant hover:text-on-surface p-1 rounded-full transition-colors focus:outline-none"
            @click.stop="searchQuery = ''"
          >
            <span class="material-symbols-outlined text-[18px]">close</span>
          </button>

          <button
            type="button"
            class="text-on-surface-variant hover:text-on-surface p-0.5 transition-transform duration-200 focus:outline-none"
            :class="{ 'rotate-180': isOpen }"
            @click.stop="toggleDropdown"
          >
            <span class="material-symbols-outlined text-[20px]">expand_more</span>
          </button>
        </div>
      </div>

      <div
        v-if="isOpen && !loading"
        class="absolute top-full left-0 right-0 mt-1.5 z-30 rounded-xl bg-surface-container-lowest border border-outline-variant/40 shadow-xl overflow-hidden animate-in fade-in slide-in-from-top-1 duration-150"
      >
        <div
          class="p-2 border-b border-outline-variant/20 flex items-center justify-between text-xs font-semibold text-on-surface-variant bg-surface-container-low/50"
        >
          <span>
            {{ filteredOptions.length }}
            {{ filteredOptions.length === 1 ? 'materia disponible' : 'materias disponibles' }}
          </span>
          <button
            v-if="selectedItems.length > 0"
            type="button"
            class="text-error hover:underline font-medium text-xs focus:outline-none"
            @click.stop="clearAll"
          >
            Deseleccionar todas
          </button>
        </div>

        <div
          v-if="filteredOptions.length > 0"
          class="max-h-56 overflow-y-auto divide-y divide-outline-variant/10 py-1"
        >
          <div
            v-for="option in filteredOptions"
            :key="option.value"
            role="button"
            tabindex="0"
            :class="[
              'w-full px-3.5 py-2.5 text-left text-sm transition-colors flex items-center justify-between cursor-pointer group',
              isSelected(option.value)
                ? 'bg-primary/10 text-primary font-semibold'
                : 'text-on-surface hover:bg-surface-container-high font-medium'
            ]"
            @click.stop="toggleOption(option.value)"
          >
            <div class="flex items-center gap-2.5 min-w-0 pr-2">
              <span
                class="material-symbols-outlined text-[18px] shrink-0 transition-colors"
                :class="
                  isSelected(option.value)
                    ? 'text-primary'
                    : 'text-outline-variant group-hover:text-on-surface-variant'
                "
              >
                {{ isSelected(option.value) ? 'check_box' : 'check_box_outline_blank' }}
              </span>
              <span class="truncate">{{ option.label }}</span>
            </div>

            <span
              v-if="isSelected(option.value)"
              class="text-xs font-semibold text-primary bg-primary/15 px-2 py-0.5 rounded-full shrink-0"
            >
              Seleccionada
            </span>
          </div>
        </div>

        <div v-else class="py-6 px-4 text-center text-sm text-on-surface-variant">
          <span class="material-symbols-outlined text-[24px] text-outline-variant block mb-1">
            search_off
          </span>
          No se encontraron materias que coincidan con "{{ searchQuery }}".
        </div>
      </div>
    </div>

    <div v-if="selectedItems.length > 0" class="flex flex-wrap items-center gap-1.5 pt-1">
      <BaseBadge
        v-for="item in selectedItems"
        :key="item.value"
        variant="primary"
        size="md"
        removable
        @remove="removeOption(item.value)"
      >
        {{ item.label }}
      </BaseBadge>

      <button
        type="button"
        class="text-xs text-error hover:underline font-medium ml-1.5 py-0.5 focus:outline-none"
        @click="clearAll"
      >
        Limpiar todas
      </button>
    </div>

    <p v-else-if="hint" class="text-xs text-on-surface-variant mt-0.5">
      {{ hint }}
    </p>

    <p v-if="error" class="text-xs text-error font-medium flex items-center gap-1 mt-0.5">
      <span class="material-symbols-outlined text-[16px]">info</span>
      {{ error }}
    </p>
  </div>
</template>
