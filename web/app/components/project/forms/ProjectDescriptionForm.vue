<template>
  <div v-if="form" class="flex flex-col gap-4 pb-4">
    <FormGroup :errors="v$['form']?.description?.$errors" name="Description">
      <TextEditor v-model="form.description" label="Description" :disabled="loading" />
    </FormGroup>
    <div v-if="project?.id" class="flex justify-end items-center gap-2">
      <TextInput
        v-model="extraDescription"
        placeholder="Extra Description"
        :disabled="loading || aiLoading"
        size="sm"
      />
      <FormButton
        label="Suggest Description"
        type="secondary"
        size="sm"
        icon="material-symbols:smart-toy-outline"
        :loading="aiLoading"
        :disabled="loading || aiLoading"
        @click="suggestDescription"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useVuelidate } from '@vuelidate/core'
import { cloneDeep } from 'lodash-es'
import type { CreateProject, Project } from '@api'
import { useProjectStore } from '~/store/ProjectStore'
import { useAiStore } from '~/store/AiStore'
import useValidation from '~/composables/useValidation'
import { defaultProjectForm } from '~/utils/default-helper'
import FormGroup from '~/components/ui/form/FormGroup.vue'
import TextEditor from '~/components/ui/input/TextEditor.vue'
import FormButton from '~/components/ui/form/FormButton.vue'
import TextInput from '~/components/ui/input/TextInput.vue'

interface Props {
  modelValue?: CreateProject
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: () => cloneDeep(defaultProjectForm),
})

const emit = defineEmits<{
  'update:modelValue': [value: CreateProject]
}>()

const projectStore = useProjectStore()
const aiStore = useAiStore()
const validation = useValidation()

const form = ref<CreateProject>(props.modelValue)
const extraDescription = ref<string>('')

const v$ = useVuelidate()

const loading = computed((): boolean => {
  return projectStore.projectCreating || projectStore.projectLoading || false
})

const aiLoading = computed((): boolean => {
  return aiStore.aiForm.loading || false
})

const project = computed((): Project | undefined => {
  return projectStore.project
})

async function validate(): Promise<boolean> {
  return await validation.validate(v$.value)
}

async function suggestDescription() {
  if (!project.value?.id) return

  await aiStore.getSuggestedDescription(project.value.id, extraDescription.value, null, null)

  if (aiStore.aiForm.data) form.value.description = aiStore.aiForm.data.suggestedDescription ?? ''
}

defineExpose({
  validate,
})

watch(() => props.modelValue, (newValue) => {
  form.value = newValue
}, { immediate: true })

watch(form, (newValue) => {
  emit('update:modelValue', newValue)
}, { deep: true })
</script>
