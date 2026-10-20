<script setup lang="ts">
import { computed, reactive, watch } from 'vue'
import { BaseButton, BaseModal } from '@/components/ui'
import type { TutorSession } from '../types'

interface StudyResourceForm {
  nombre: string
  tipo: string
  descripcionUso: string
}

interface Props {
  modelValue: boolean
  session: TutorSession | null
  loading?: boolean
}

interface Emits {
  (e: 'update:modelValue', value: boolean): void
  (
    e: 'submit',
    payload: {
      dificultadesIdentificadas: string
      recursos: StudyResourceForm[]
    }
  ): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

function createResource(): StudyResourceForm {
  return { nombre: '', tipo: '', descripcionUso: '' }
}

const form = reactive({
  dificultadesIdentificadas: '',
  recursos: [createResource()]
})

const isFormValid = computed(() =>
  form.dificultadesIdentificadas.trim().length > 0 &&
  form.recursos.length > 0 &&
  form.recursos.every(
    resource =>
      resource.nombre.trim().length > 0 &&
      resource.tipo.trim().length > 0 &&
      resource.descripcionUso.trim().length > 0
  )
)

watch(
  () => props.modelValue,
  isOpen => {
    if (isOpen) {
      form.dificultadesIdentificadas = ''
      form.recursos.splice(0, form.recursos.length, createResource())
    }
  }
)

function addResource() {
  form.recursos.push(createResource())
}

function removeResource(index: number) {
  if (form.recursos.length > 1) {
    form.recursos.splice(index, 1)
  }
}

function handleSubmit() {
  if (!isFormValid.value) return
  emit('submit', {
    dificultadesIdentificadas: form.dificultadesIdentificadas.trim(),
    recursos: form.recursos.map(resource => ({
      nombre: resource.nombre.trim(),
      tipo: resource.tipo.trim(),
      descripcionUso: resource.descripcionUso.trim()
    }))
  })
}
</script>

<template>
  <BaseModal
    :model-value="modelValue"
    title="Marcar como Atendida"
    max-width="lg"
    @update:model-value="emit('update:modelValue', $event)"
  >
    <div class="flex flex-col gap-5">
      <div class="flex items-center gap-3">
        <div
          class="w-12 h-12 rounded-full bg-[#16a34a]/10 text-[#16a34a] flex items-center justify-center"
        >
          <span class="material-symbols-outlined text-[26px]">task_alt</span>
        </div>
        <div>
          <h4 class="font-semibold text-on-surface text-base">
            Sesión con {{ session?.estudianteNombre }}
          </h4>
          <p class="text-xs text-on-surface-variant">
            {{ session?.materia }} • {{ session?.fecha }} ({{ session?.hora }})
          </p>
        </div>
      </div>

      <div class="flex flex-col gap-4">
        <div class="flex flex-col gap-1.5">
          <label for="complete-difficulties" class="text-xs font-semibold text-on-surface"
            >Dificultades identificadas *</label
          >
          <textarea
            id="complete-difficulties"
            v-model="form.dificultadesIdentificadas"
            rows="3"
            placeholder="Describe las dificultades observadas durante la tutoría..."
            required
            class="w-full bg-surface-container-low text-on-surface px-4 py-3 rounded-lg border border-outline-variant/40 focus:outline-none focus:ring-2 focus:ring-secondary/40 focus:border-secondary text-sm resize-none"
          />
        </div>

        <section class="flex flex-col gap-3" aria-labelledby="study-resources-title">
          <div class="flex items-center justify-between gap-3">
            <h5 id="study-resources-title" class="text-sm font-semibold text-on-surface">
              Recursos recomendados
            </h5>
            <BaseButton variant="outline" size="sm" icon="add" @click="addResource">
              Agregar recurso
            </BaseButton>
          </div>

          <div
            v-for="(resource, index) in form.recursos"
            :key="index"
            class="flex flex-col gap-3 border border-outline-variant/40 rounded-lg p-4"
          >
            <div class="flex items-center justify-between gap-3">
              <h6 class="text-sm font-semibold text-on-surface">Recurso {{ index + 1 }}</h6>
              <button
                type="button"
                class="p-2 rounded-md text-on-surface-variant hover:text-error hover:bg-error-container/40 disabled:opacity-50 disabled:cursor-not-allowed"
                :disabled="form.recursos.length === 1"
                :aria-label="`Eliminar recurso ${index + 1}`"
                :title="form.recursos.length === 1 ? 'Debe conservar al menos un recurso' : 'Eliminar recurso'"
                @click="removeResource(index)"
              >
                <span class="material-symbols-outlined text-[20px]">delete</span>
              </button>
            </div>

            <div class="grid grid-cols-1 sm:grid-cols-2 gap-3">
              <div class="flex flex-col gap-1.5">
                <label :for="`resource-name-${index}`" class="text-xs font-semibold text-on-surface">
                  Nombre del recurso {{ index + 1 }} *
                </label>
                <input
                  :id="`resource-name-${index}`"
                  v-model="resource.nombre"
                  required
                  type="text"
                  class="w-full bg-surface-container-low text-on-surface px-3 py-2.5 rounded-md border border-outline-variant/40 focus:outline-none focus:ring-2 focus:ring-secondary/40 focus:border-secondary text-sm"
                />
              </div>

              <div class="flex flex-col gap-1.5">
                <label :for="`resource-type-${index}`" class="text-xs font-semibold text-on-surface">
                  Tipo de recurso {{ index + 1 }} *
                </label>
                <input
                  :id="`resource-type-${index}`"
                  v-model="resource.tipo"
                  required
                  type="text"
                  class="w-full bg-surface-container-low text-on-surface px-3 py-2.5 rounded-md border border-outline-variant/40 focus:outline-none focus:ring-2 focus:ring-secondary/40 focus:border-secondary text-sm"
                />
              </div>
            </div>

            <div class="flex flex-col gap-1.5">
              <label
                :for="`resource-usage-${index}`"
                class="text-xs font-semibold text-on-surface"
              >
                Descripción de uso del recurso {{ index + 1 }} *
              </label>
              <textarea
                :id="`resource-usage-${index}`"
                v-model="resource.descripcionUso"
                required
                rows="2"
                class="w-full bg-surface-container-low text-on-surface px-3 py-2.5 rounded-md border border-outline-variant/40 focus:outline-none focus:ring-2 focus:ring-secondary/40 focus:border-secondary text-sm resize-y"
              />
            </div>
          </div>
        </section>
      </div>
    </div>

    <template #footer>
      <BaseButton
        variant="outline"
        size="md"
        :disabled="loading"
        @click="emit('update:modelValue', false)"
      >
        Cancelar
      </BaseButton>
      <BaseButton
        variant="primary"
        size="md"
        :loading="loading"
        :disabled="!isFormValid"
        @click="handleSubmit"
      >
        <template #iconLeft>
          <span class="material-symbols-outlined text-[18px]">save</span>
        </template>
        Guardar y Finalizar
      </BaseButton>
    </template>
  </BaseModal>
</template>
