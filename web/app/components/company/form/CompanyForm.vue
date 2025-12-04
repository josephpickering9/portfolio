<template>
  <div class="prose w-full max-w-xl space-y-5 rounded-md border border-gray-600 px-6 py-4">
    <h1>{{ isUpdate ? 'Update' : 'New' }} Company</h1>
    <TextInput v-model="name" label="Name" :disabled="loading" />
    <TextInput v-model="description" label="Description" :disabled="loading" />
    <FileInput label="Image" :image-url="logoUrl" :disabled="loading" @update:file="logo = $event" />
    <TextInput v-model="website" label="Website" :disabled="loading" />
    <div class="flex items-center justify-between space-x-2">
      <FormButton label="Save" type="primary" size="sm" :disabled="loading" @click="save" />
      <FormButton
        v-if="isUpdate"
        label="Delete"
        type="error"
        size="sm"
        icon="material-symbols:delete"
        :disabled="loading"
        @click="remove"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useCompanyStore } from '~/store/CompanyStore'
import { useNotificationStore } from '~/store/NotificationStore'
import type { CreateCompany } from '@api/models/CreateCompany'
import type { Company } from '@api/models/Company'
import { getImageUrl } from '~/utils/image-helper'
import TextInput from '~/components/ui/input/TextInput.vue'
import FormButton from '~/components/ui/form/FormButton.vue'
import FileInput from '~/components/ui/input/FileInput.vue'

interface Props {
  id?: number | null
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
  return companyStore.companyCreating || companyStore.companyLoading
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
    } else {
      notificationStore.displayErrorNotification(companyError.value || 'An error occurred')
    }
  }
})
</script>
