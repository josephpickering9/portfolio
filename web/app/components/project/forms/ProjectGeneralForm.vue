<template>
  <div v-if="form" class="flex flex-col gap-6 pb-4">
    <div class="flex flex-col gap-4 md:flex-row">
      <FormGroup :errors="v$['form']?.title?.$errors" name="Title" class="w-full">
        <TextInput v-model="form.title" label="Title" :disabled="loading" />
      </FormGroup>
    </div>
    
    <div class="flex flex-col gap-4 md:flex-row">
      <FormGroup :errors="v$['form']?.startDate?.$errors" name="Start Date" class="md:w-1/2">
        <DatePicker v-model="form.startDate" label="Start Date" :disabled="loading" />
      </FormGroup>
      <FormGroup :errors="v$['form']?.endDate?.$errors" name="End Date" class="md:w-1/2">
        <DatePicker v-model="form.endDate" label="End Date" :disabled="loading" />
      </FormGroup>
    </div>

    <FormGroup :errors="v$['form']?.shortDescription?.$errors" name="Short Description">
      <TextInput v-model="form.shortDescription" label="Short Description" :disabled="loading" />
    </FormGroup>
    <div class="flex flex-col gap-4 md:flex-row">
      <FormGroup :errors="v$['form']?.companyId?.$errors" name="Company" class="md:w-2/5">
        <CompanyAutoComplete v-model="form.companyId" label="Company" :disabled="loading" />
      </FormGroup>
      <FormGroup :errors="v$['form']?.website?.$errors" name="Website" class="md:w-3/5">
        <TextInput v-model="form.website" label="Website" :disabled="loading" />
      </FormGroup>
    </div>
    <FormGroup :errors="v$['form']?.tags?.$errors" name="Technologies">
      <TagAutoComplete v-model="form.tags" label="Technologies" placeholder="Search by tag name" />
    </FormGroup>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useVuelidate } from '@vuelidate/core'
import { required, url } from '@vuelidate/validators'
import { cloneDeep } from 'lodash-es'
import type { CreateProject } from '@api'
import { useProjectStore } from '~/store/ProjectStore'
import useValidation from '~/composables/useValidation'
import { defaultProjectForm } from '~/utils/default-helper'
import FormGroup from '~/components/ui/form/FormGroup.vue'
import TextInput from '~/components/ui/input/TextInput.vue'
import DatePicker from '~/components/ui/input/DatePicker.vue'
import CompanyAutoComplete from '~/components/company/form/CompanyAutoComplete.vue'
import TagAutoComplete from '~/components/tag/form/TagAutoComplete.vue'

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
const validation = useValidation()

const form = ref<CreateProject>(props.modelValue)

const rules = {
  form: {
    title: { required },
    startDate: { required },
    shortDescription: { required },
    website: { url },
    tags: { required },
  },
}

const v$ = useVuelidate(rules, { form })

const loading = computed((): boolean => {
  return projectStore.projectCreating || projectStore.projectLoading
})

async function validate(): Promise<boolean> {
  return await validation.validate(v$.value)
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
