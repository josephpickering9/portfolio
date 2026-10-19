<template>
  <div class="grid grid-cols-[repeat(auto-fit,minmax(min(100%,280px),1fr))] gap-5.5">
    <FormGroup label="Title" required :errors="v$.form.title.$errors" class="col-span-full">
      <TextInput v-model="form.title" placeholder="Project title" :disabled="disabled" />
    </FormGroup>
    <FormGroup label="Start date" required :errors="v$.form.startDate.$errors">
      <DatePicker v-model="form.startDate" :disabled="disabled" />
    </FormGroup>
    <FormGroup label="End date" helper="Leave empty for ongoing work">
      <DatePicker v-model="form.endDate" :disabled="disabled" />
    </FormGroup>
    <FormGroup label="Short description" required :errors="v$.form.shortDescription.$errors" class="col-span-full">
      <TextInput v-model="form.shortDescription" placeholder="One line shown on cards and search results" :disabled="disabled" />
    </FormGroup>
    <FormGroup label="Company">
      <SelectList v-model="form.companyId" :options="companyOptions" placeholder="No company" :disabled="disabled" />
    </FormGroup>
    <FormGroup label="Website" :errors="v$.form.website.$errors">
      <TextInput v-model="form.website" placeholder="https://" :disabled="disabled" />
    </FormGroup>
    <FormGroup label="Technologies" required :errors="v$.form.tags.$errors" class="col-span-full">
      <TagAutoComplete v-model="tags" :disabled="disabled" />
    </FormGroup>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useVuelidate } from '@vuelidate/core'
import { helpers, required } from '@vuelidate/validators'
import type { CreateProject } from '@api'
import { useCompanyStore } from '~/store/CompanyStore'
import { sortCompaniesNewestFirst } from '~/utils/company-helper'
import { isHttpUrl, URL_MESSAGE } from '~/utils/validation-helper'
import FormGroup from '~/components/ui/form/FormGroup.vue'
import TextInput from '~/components/ui/input/TextInput.vue'
import DatePicker from '~/components/ui/input/DatePicker.vue'
import SelectList from '~/components/ui/input/SelectList.vue'
import TagAutoComplete from '~/components/tag/form/TagAutoComplete.vue'

interface Props {
  disabled?: boolean
}

withDefaults(defineProps<Props>(), {
  disabled: false,
})

const form = defineModel<CreateProject>({ required: true })

const RULES = {
  form: {
    title: { required },
    startDate: { required },
    shortDescription: { required },
    website: { url: helpers.withMessage(URL_MESSAGE, (value: string | null | undefined) => isHttpUrl(value)) },
    tags: { required: helpers.withMessage('Add at least one technology', (value?: string[]) => Boolean(value?.length)) },
  },
}

const companyStore = useCompanyStore()
const v$ = useVuelidate(RULES, { form })

const tags = computed({
  get: () => form.value.tags ?? [],
  set: (value: string[]) => {
    form.value.tags = value
  },
})

const companyOptions = computed(() =>
  sortCompaniesNewestFirst(companyStore.companies).map((company) => ({ value: company.id, label: company.name })),
)

const hasErrors = computed(() => v$.value.$error)

onMounted(() => companyStore.ensureCompanies())

defineExpose({
  validate: () => v$.value.$validate(),
  hasErrors,
})
</script>
