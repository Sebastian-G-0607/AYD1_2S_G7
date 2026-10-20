<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { BaseAlert, BaseButton, BaseModal, BaseSelect } from '@/components/ui'
import type { ReportCategory, ReportTutorPayload, StudentHistorySession } from '../types'

const MOTIVO_MIN = 10
const MOTIVO_MAX = 1000

interface Props {
  modelValue: boolean
  session: StudentHistorySession | null
  categories: ReportCategory[]
  loading?: boolean
  errorMessage?: string | null
}

interface Emits {
  (e: 'update:modelValue', value: boolean): void
  (e: 'submit', payload: ReportTutorPayload): void
}

const props = withDefaults(defineProps<Props>(), {
  loading: false,
  errorMessage: null
})
const emit = defineEmits<Emits>()

const categoriaId = ref<string | number>('')
const motivo = ref('')
const categoriaError = ref('')
const motivoError = ref('')

const categoryOptions = computed(() =>
  props.categories.map(category => ({ value: category.id, label: category.nombre }))
)

const selectedDescription = computed(
  () => props.categories.find(category => category.id === Number(categoriaId.value))?.descripcion
)

watch(
  () => props.modelValue,
  isOpen => {
    if (isOpen) {
      categoriaId.value = ''
      motivo.value = ''
      categoriaError.value = ''
      motivoError.value = ''
    }
  }
)

function onCategoryChange(value: string | number | (string | number)[]) {
  categoriaId.value = Array.isArray(value) ? (value[0] ?? '') : value
  categoriaError.value = ''
}

function validate(): boolean {
  const text = motivo.value.trim()

  categoriaError.value = categoriaId.value === '' ? 'Debes seleccionar una categoría.' : ''

  if (!text) {
    motivoError.value = 'Debes explicar el motivo del reporte.'
  } else if (text.length < MOTIVO_MIN) {
    motivoError.value = `La explicación debe tener al menos ${MOTIVO_MIN} caracteres.`
  } else if (text.length > MOTIVO_MAX) {
    motivoError.value = `La explicación no puede superar los ${MOTIVO_MAX} caracteres.`
  } else {
    motivoError.value = ''
  }

  return !categoriaError.value && !motivoError.value
}

function handleSubmit() {
  if (!validate()) return

  emit('submit', {
    categoriaId: Number(categoriaId.value),
    motivo: motivo.value.trim()
  })
}

function formatDate(date?: string) {
  if (!date) return ''
  const [year, month, day] = date.split('-')
  return year && month && day ? `${day}/${month}/${year}` : date
}

function handleClose() {
  emit('update:modelValue', false)
}
</script>

<template>
  <BaseModal
    :model-value="modelValue"
    title="Reportar tutor"
    max-width="lg"
    @update:model-value="handleClose"
  >
    <form
      id="report-tutor-form"
      class="flex flex-col gap-5"
      novalidate
      @submit.prevent="handleSubmit"
    >
      <div class="flex items-center gap-3">
        <div
          class="w-12 h-12 rounded-full bg-error-container text-on-error-container flex items-center justify-center shrink-0"
        >
          <span class="material-symbols-outlined text-[26px]">flag</span>
        </div>
        <div>
          <h4 class="font-semibold text-on-surface text-base">
            Reporte sobre {{ session?.tutor }}
          </h4>
          <p class="text-xs text-on-surface-variant">
            {{ session?.materia }} • {{ formatDate(session?.fechaSesion) }}
          </p>
        </div>
      </div>

      <p class="text-sm text-on-surface-variant">
        Tu reporte será enviado al administrador, quien podrá tomar las acciones correspondientes.
      </p>

      <BaseAlert v-if="errorMessage" type="error" :message="errorMessage" />

      <BaseSelect
        id="report-categoria"
        label="Categoría del reporte"
        placeholder="Selecciona una categoría"
        required
        :model-value="categoriaId"
        :options="categoryOptions"
        :disabled="loading"
        :error="categoriaError"
        :hint="selectedDescription ?? undefined"
        @update:model-value="onCategoryChange"
      />

      <div class="flex flex-col gap-1.5">
        <label for="report-motivo" class="text-sm font-semibold text-on-surface">
          Explicación del motivo <span class="text-error">*</span>
        </label>
        <textarea
          id="report-motivo"
          v-model="motivo"
          rows="5"
          :maxlength="MOTIVO_MAX"
          :disabled="loading"
          placeholder="Describe con detalle lo ocurrido durante la tutoría"
          :class="[
            'w-full bg-surface-container-low text-on-surface px-4 py-3 rounded-lg border focus:outline-none focus:ring-2 text-sm resize-none disabled:opacity-60',
            motivoError
              ? 'border-error focus:ring-error/40'
              : 'border-outline-variant/40 focus:ring-primary/40 focus:border-primary'
          ]"
          @input="motivoError = ''"
        />
        <div class="flex items-start justify-between gap-3">
          <p v-if="motivoError" class="text-xs text-error font-medium flex items-center gap-1">
            <span class="material-symbols-outlined text-[16px]">info</span>
            {{ motivoError }}
          </p>
          <span v-else />
          <span class="text-xs text-on-surface-variant shrink-0">
            {{ motivo.length }}/{{ MOTIVO_MAX }}
          </span>
        </div>
      </div>
    </form>

    <template #footer>
      <BaseButton variant="outline" size="md" :disabled="loading" @click="handleClose">
        Cancelar
      </BaseButton>
      <BaseButton variant="danger" size="md" :loading="loading" @click="handleSubmit">
        <template #iconLeft>
          <span class="material-symbols-outlined text-[18px]">send</span>
        </template>
        Enviar reporte
      </BaseButton>
    </template>
  </BaseModal>
</template>
