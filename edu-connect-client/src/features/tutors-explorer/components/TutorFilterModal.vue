<script setup lang="ts">
import BaseModal from '@/components/ui/BaseModal.vue'
import BaseButton from '@/components/ui/BaseButton.vue'
import TutorFilterForm from './TutorFilterForm.vue'

interface Props {
  modelValue: boolean
}

interface Emits {
  (e: 'update:modelValue', value: boolean): void
  (e: 'reset'): void
}

defineProps<Props>()
const emit = defineEmits<Emits>()

const materia = defineModel<string>('materia', { default: '' })
const universidad = defineModel<string>('universidad', { default: '' })
const experienciaMinima = defineModel<number>('experienciaMinima', { default: 0 })
const edadMaxima = defineModel<number>('edadMaxima', { default: 65 })
const genero = defineModel<'any' | 'female' | 'male'>('genero', { default: 'any' })

function closeModal() {
  emit('update:modelValue', false)
}

function handleReset() {
  emit('reset')
}
</script>

<template>
  <BaseModal
    :model-value="modelValue"
    title="Filtros de Búsqueda"
    max-width="md"
    @update:model-value="emit('update:modelValue', $event)"
    @close="closeModal"
  >
    <div class="py-2">
      <TutorFilterForm
        v-model:materia="materia"
        v-model:universidad="universidad"
        v-model:experiencia-minima="experienciaMinima"
        v-model:edad-maxima="edadMaxima"
        v-model:genero="genero"
      />
    </div>

    <template #footer>
      <BaseButton variant="outline" size="md" @click="handleReset">
        <span>Restablecer</span>
      </BaseButton>
      <BaseButton variant="primary" size="md" @click="closeModal">
        <span>Ver Resultados</span>
      </BaseButton>
    </template>
  </BaseModal>
</template>
