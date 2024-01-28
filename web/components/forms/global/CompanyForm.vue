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

<script lang="ts">
import { defineComponent } from 'vue'
import { useCompanyStore } from '../../../store/CompanyStore'
import { useNotificationStore } from '../../../store/NotificationStore'
import type { CreateCompany } from '../../../api/models/CreateCompany'
import type { Company } from '../../../api/models/Company'
import TextInput from '../TextInput.vue'
import FormButton from '../FormButton.vue'
import FileInput from '../FileInput.vue'
import { getImageUrl } from '../../../utils/image-helper'

interface Data {
  name: string
  description: string
  logo?: FileList | null
  logoUrl?: string
  website?: string
}

export default defineComponent({
  name: 'CompanyForm',
  components: {
    TextInput,
    FormButton,
    FileInput,
  },
  props: {
    id: {
      type: Number,
      default: null,
    },
  },
  data(): Data {
    return {
      name: '',
      description: '',
      logo: undefined,
      logoUrl: undefined,
      website: undefined,
    }
  },
  computed: {
    isUpdate(): boolean {
      return this.id !== null
    },
    company(): Company | undefined {
      return useCompanyStore().company
    },
    companyError(): string | undefined {
      return useCompanyStore().companyError
    },
    loading(): boolean {
      return useCompanyStore().companyCreating || useCompanyStore().companyLoading
    },
    error(): string | undefined {
      return useCompanyStore().companyCreateError
    },
    createCompanyValue(): CreateCompany {
      return {
        name: this.name,
        description: this.description,
        logo: this.logo ? (this.logo.item(0) as Blob) : undefined,
        website: this.website,
      }
    },
  },
  async mounted() {
    if (this.isUpdate) {
      await useCompanyStore().getCompany(this.id)

      if (!this.companyError && this.company) {
        this.name = this.company.name
        this.description = this.company.description
        this.logoUrl = this.company.logo ? getImageUrl(this.company.logo) : undefined
        this.website = this.company.website ?? ''
      } else {
        useNotificationStore().displayErrorNotification(this.companyError || 'An error occurred')
      }
    }
  },
  methods: {
    async save() {
      let response: Company | undefined

      if (this.isUpdate) {
        response = await useCompanyStore().updateCompany(this.id, this.createCompanyValue)
      } else {
        response = await useCompanyStore().createCompany(this.createCompanyValue)
      }

      if (!this.error && response) {
        this.$router.push(`/companies`)
        useNotificationStore().displaySuccessNotification(`Company ${this.isUpdate ? 'updated' : 'created'} successfully`)
      } else {
        useNotificationStore().displayErrorNotification(this.error || 'An error occurred')
      }
    },
    async remove() {
      if (this.isUpdate) {
        await useCompanyStore().deleteCompany(this.id)

        if (!this.error) {
          this.$router.push(`/companies`)
          useNotificationStore().displaySuccessNotification('Company deleted successfully')
        } else {
          useNotificationStore().displayErrorNotification(this.error || 'An error occurred')
        }
      }
    },
  },
})
</script>
