<template>
  <FormLayout
    :eyebrow="id ? 'Update company' : 'New company'"
    :title="form.name || (id ? 'Untitled company' : 'New company')"
    :save-label="id ? 'Save changes' : 'Create company'"
    delete-title="Delete this company?"
    :show-delete="Boolean(id)"
    :loading="loading"
    :saving="companyStore.companyCreating"
    @save="save"
    @delete="id && remove(id)"
  >
    <div v-if="companyStore.companyLoading" class="skeleton-pulse h-120 rounded-panel border border-line bg-surface" aria-hidden="true" />
    <div v-else class="grid items-start gap-5 lg:grid-cols-3">
      <section class="flex min-w-0 flex-col gap-5 rounded-panel border border-line bg-surface p-6.5 lg:col-span-2">
        <h2 class="m-0 font-display text-[26px] font-normal">General</h2>
        <FormGroup label="Name" required :errors="v$.form.name.$errors">
          <TextInput v-model="form.name" placeholder="Company name" :disabled="loading" />
        </FormGroup>
        <FormGroup label="Website" :errors="v$.form.website.$errors">
          <TextInput v-model="form.website" placeholder="https://example.com" :disabled="loading" />
        </FormGroup>
        <FormGroup label="Description" label-tag="span">
          <TextEditor v-model="form.description" placeholder="Brief description of the company…" :min-height="260" :disabled="loading" />
        </FormGroup>
      </section>
      <div class="flex min-w-0 flex-col gap-5">
        <section class="flex flex-col gap-4 rounded-panel border border-line bg-surface p-6.5">
          <h2 class="m-0 font-display text-[26px] font-normal">Branding</h2>
          <FormGroup helper="Square image, shown filling a circle in the timeline. Use a transparent background so it sits on the theme colour.">
            <FileInput v-model="logo" round empty-label="Upload logo" :disabled="loading" />
          </FormGroup>
        </section>
        <section class="flex flex-col gap-4 rounded-panel border border-line bg-surface p-6.5">
          <h2 class="m-0 font-display text-[26px] font-normal">Timeline</h2>
          <FormGroup label="Start date">
            <DatePicker v-model="form.startDate" :disabled="loading" />
          </FormGroup>
          <FormGroup label="End date" helper="Leave empty if you still work here">
            <DatePicker v-model="form.endDate" :disabled="loading" />
          </FormGroup>
        </section>
      </div>
    </div>
  </FormLayout>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useVuelidate } from '@vuelidate/core'
import { helpers, required } from '@vuelidate/validators'
import type { Company, CreateCompany } from '@api'
import { useCompanyStore } from '~/store/CompanyStore'
import { useEntityForm } from '~/composables/useEntityForm'
import { getImageUrl } from '~/utils/image-helper'
import { isHttpUrl, URL_MESSAGE } from '~/utils/validation-helper'
import type { UploadItem } from '~/types/UploadItem'
import FormLayout from '~/components/ui/layout/FormLayout.vue'
import FormGroup from '~/components/ui/form/FormGroup.vue'
import TextInput from '~/components/ui/input/TextInput.vue'
import TextEditor from '~/components/ui/input/TextEditor.vue'
import FileInput from '~/components/ui/input/FileInput.vue'
import DatePicker from '~/components/ui/input/DatePicker.vue'

const DATE_LENGTH = 10

interface Props {
  id?: string
}

const props = withDefaults(defineProps<Props>(), {
  id: undefined,
})

const companyStore = useCompanyStore()

const form = ref<CreateCompany>({ name: '', description: '', website: '', startDate: undefined, endDate: undefined })
const logo = ref<UploadItem | null>(null)

const loading = computed(() => (companyStore.companyCreating || companyStore.companyLoading) ?? false)

const rules = {
  form: {
    name: { required },
    website: { url: helpers.withMessage(URL_MESSAGE, (value: string | null | undefined) => isHttpUrl(value)) },
  },
}
const v$ = useVuelidate(rules, { form })

const entityForm = useEntityForm<Company, CreateCompany>({
  entityName: 'company',
  redirectRoute: '/companies',
  actions: {
    get: companyStore.getCompany,
    create: companyStore.createCompany,
    update: companyStore.updateCompany,
    delete: companyStore.deleteCompany,
  },
  state: {
    loadError: () => companyStore.companyError,
    saveError: () => companyStore.companyCreateError,
  },
})
const { remove } = entityForm

async function save() {
  if (!(await v$.value.$validate())) return entityForm.invalid()
  await entityForm.save(props.id, { ...form.value, logo: logo.value?.file })
}

onMounted(async () => {
  if (!props.id) return
  const company = await entityForm.fetch(props.id)
  if (!company) return

  form.value = {
    name: company.name,
    description: company.description,
    website: company.website ?? '',
    startDate: company.startDate?.substring(0, DATE_LENGTH),
    endDate: company.endDate?.substring(0, DATE_LENGTH),
  }
  logo.value = company.logo ? { key: company.logo, previewUrl: getImageUrl(company.logo) } : null
})
</script>
