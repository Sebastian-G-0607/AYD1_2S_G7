<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { BaseAlert, BaseButton, BaseModal, BaseSelect, type SelectOption } from '@/components/ui'
import { tutorStudentFeedbackService } from '../services/tutorStudentFeedback.service'

interface Props {
  modelValue: boolean
  sesionId: number
  estudianteNombre: string
}

const props = defineProps<Props>()

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
  reported: []
}>()

const categorias = ref<SelectOption[]>([])
const categoriaId = ref<string | number>('')
const explicacion = ref('')
const isLoadingCategorias = ref(false)
const isSubmitting = ref(false)
const errorMessage = ref('')

async function loadCategorias() {
  isLoadingCategorias.value = true

  try {
    const data = await tutorStudentFeedbackService.getCategoriasReporte()
    categorias.value = data.map(categoria => ({ value: categoria.id, label: categoria.nombre }))
  } catch {
    errorMessage.value = 'No fue posible cargar las categorías de reporte.'
  } finally {
    isLoadingCategorias.value = false
  }
}

onMounted(loadCategorias)

function close() {
  emit('update:modelValue', false)
  categoriaId.value = ''
  explicacion.value = ''
  errorMessage.value = ''
}

async function handleSubmit() {
  if (!categoriaId.value || !explicacion.value.trim()) return

  isSubmitting.value = true
  errorMessage.value = ''

  try {
    await tutorStudentFeedbackService.reportarEstudiante(props.sesionId, {
      categoriaId: Number(categoriaId.value),
      explicacion: explicacion.value.trim()
    })

    emit('reported')
    close()
  } catch (error) {
    errorMessage.value = error instanceof Error ? error.message : 'No fue posible enviar el reporte.'
  } finally {
    isSubmitting.value = false
  }
}
</script>

<template>
  <BaseModal :model-value="modelValue" title="Reportar estudiante" @update:model-value="close">
    <div class="flex flex-col gap-5">
      <p class="text-sm text-on-surface-variant">
        Reporta una conducta inapropiada de <span class="font-medium text-on-surface">{{ estudianteNombre }}</span>.
        El administrador revisará este caso.
      </p>

      <BaseAlert v-if="errorMessage" type="error" :message="errorMessage" :dismissible="false" />

      <BaseSelect
        v-model="categoriaId"
        label="Categoría del reporte"
        placeholder="Selecciona una categoría"
        :options="categorias"
        :disabled="isLoadingCategorias || isSubmitting"
        required
      />

      <div class="flex flex-col gap-2">
        <label class="text-sm font-semibold text-on-surface" for="explicacion-reporte">
          Explicación <span class="text-error">*</span>
        </label>
        <textarea
          id="explicacion-reporte"
          v-model="explicacion"
          rows="4"
          :disabled="isSubmitting"
          placeholder="Describe detalladamente el motivo del reporte"
          class="w-full rounded-lg border border-outline-variant px-3 py-2 text-sm text-on-surface placeholder:text-on-surface-variant/60 focus:outline-none focus:ring-2 focus:ring-primary disabled:opacity-60"
        />
      </div>
    </div>

    <template #footer>
      <BaseButton variant="outline" :disabled="isSubmitting" @click="close">Cancelar</BaseButton>
      <BaseButton
        :loading="isSubmitting"
        :disabled="!categoriaId || !explicacion.trim()"
        @click="handleSubmit"
      >
        Enviar reporte
      </BaseButton>
    </template>
  </BaseModal>
</template>