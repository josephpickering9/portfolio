<template>
  <FormLayout
    :title="isUpdate ? 'Update Company' : 'New Company'"
    :loading="loading"
    :show-delete="isUpdate"
    @save="save"
    @delete="remove"
  >
    <div class="grid grid-cols-1 gap-6 lg:grid-cols-3">
      <div class="card bg-base-100 shadow-sm ring-1 ring-base-content/10 lg:col-span-2">
        <div class="card-body gap-4">
          <h2 class="card-title text-base font-semibold opacity-60">General Information</h2>
          <TextInput v-model="name" label="Name" :disabled="loading" placeholder="Company Name" />
          <TextInput v-model="website" label="Website" :disabled="loading" placeholder="https://example.com" />
          <TextEditor 
            v-model="description" 
            label="Description"
            placeholder="Brief description of the company..."
            :disabled="loading"
          />
        </div>
      </div>

      <div class="space-y-6">
        <div class="card bg-base-100 shadow-sm ring-1 ring-base-content/10">
          <div class="card-body gap-4">
            <h2 class="card-title text-base font-semibold opacity-60">Branding</h2>
            <div class="flex flex-col items-center justify-center p-4">
               <div class="mb-4 flex h-32 w-32 items-center justify-center rounded-lg border-2 border-dashed border-base-content/20 bg-base-200">
                  <img v-if="logoUrl" :src="logoUrl" alt="Preview" class="h-full w-full object-contain p-2" >
                  <Icon v-else name="mdi:image-outline" class="h-12 w-12 opacity-20" />
               </div>
               <FileInput label="Upload Logo" :disabled="loading" @update:file="logo = $event" />
            </div>
          </div>
        </div>

        <div class="card bg-base-100 shadow-sm ring-1 ring-base-content/10">
          <div class="card-body gap-4">
            <h2 class="card-title text-base font-semibold opacity-60">Timeline</h2>
            <TextInput v-model="startDate" label="Start Date" type="date" :disabled="loading" />
            <TextInput v-model="endDate" label="End Date" type="date" :disabled="loading" />
          </div>
        </div>
      </div>
    </div>
  </FormLayout>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useCompanyStore } from '~/store/CompanyStore'
import { useNotificationStore } from '~/store/NotificationStore'
import type { CreateCompany } from '@api/models/CreateCompany'
import type { Company } from '@api/models/Company'
import { getImageUrl } from '~/utils/image-helper'
import { format, parseISO } from 'date-fns'
import TextInput from '~/components/ui/input/TextInput.vue'
import FileInput from '~/components/ui/input/FileInput.vue'
import TextEditor from '~/components/ui/input/TextEditor.vue'
import FormLayout from '~/components/ui/layout/FormLayout.vue'

interface Props {
  id?: string | null
}

const props = withDefaults(defineProps<Props>(), {
  id: null,
})

const router = useRouter()
const companyStore = useCompanyStore()
const notificationStore = useNotificationStore()

const name = ref('')
const description = ref('')
const logo = ref<FileList | null | undefined>(undefined)
const logoUrl = ref<string | undefined>(undefined)
const website = ref<string | undefined>(undefined)
const startDate = ref<string | undefined>(undefined)
const endDate = ref<string | undefined>(undefined)

const isUpdate = computed((): boolean => {
  return props.id !== null
})

const company = computed((): Company | undefined => {
  return companyStore.company
})

const companyError = computed((): string | undefined => {
  return companyStore.companyError
})

const loading = computed((): boolean => {
  return companyStore.companyCreating || companyStore.companyLoading || false
})

const error = computed((): string | undefined => {
  return companyStore.companyCreateError
})

const createCompanyValue = computed((): CreateCompany => {
  return {
    name: name.value,
    description: description.value,
    logo: logo.value ? (logo.value.item(0) as Blob) : undefined,
    website: website.value,
    startDate: startDate.value,
    endDate: endDate.value,
  }
})

async function save() {
  let response: Company | undefined

  if (isUpdate.value) {
    response = await companyStore.updateCompany(props.id!, createCompanyValue.value)
  } else {
    response = await companyStore.createCompany(createCompanyValue.value)
  }

  if (!error.value && response) {
    router.push(`/companies`)
    notificationStore.displaySuccessNotification(`Company ${isUpdate.value ? 'updated' : 'created'} successfully`)
  } else {
    notificationStore.displayErrorNotification(error.value || 'An error occurred')
  }
}

async function remove() {
  if (isUpdate.value) {
    await companyStore.deleteCompany(props.id!)

    if (!error.value) {
      router.push(`/companies`)
      notificationStore.displaySuccessNotification('Company deleted successfully')
    } else {
      notificationStore.displayErrorNotification(error.value || 'An error occurred')
    }
  }
}

onMounted(async () => {
  if (isUpdate.value) {
    await companyStore.getCompany(props.id!)

    if (!companyError.value && company.value) {
      name.value = company.value.name
      description.value = company.value.description
      logoUrl.value = company.value.logo ? getImageUrl(company.value.logo) : undefined
      website.value = company.value.website ?? ''
      startDate.value = company.value.startDate ? format(parseISO(company.value.startDate), 'yyyy-MM-dd') : undefined
      endDate.value = company.value.endDate ? format(parseISO(company.value.endDate), 'yyyy-MM-dd') : undefined
    } else {
      notificationStore.displayErrorNotification(companyError.value || 'An error occurred')
    }
  }
})
</script>
