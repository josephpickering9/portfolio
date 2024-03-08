<template>
  <div class="prose w-full max-w-3xl space-y-5 rounded-md border border-gray-600 px-6 py-4">
    <h1>{{ isUpdate ? 'Update' : 'New' }} Project</h1>
    <Tabs v-model:active-tab="activeTab" :tabs="['General', 'Description', 'Images', 'Repositories']">
      <template #default="{ index }">
        <ProjectGeneralForm v-if="index === 0" ref="projectGeneralForm" v-model="form" />
        <ProjectDescriptionForm v-if="index === 1" ref="projectDescriptionForm" v-model="form" />
        <ProjectImagesForm v-if="index === 2" ref="projectImagesForm" v-model="form" />
        <ProjectRepositoriesForm v-if="index === 3" ref="projectRepositoriesForm" v-model="form" />
      </template>
    </Tabs>
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
import { useVuelidate } from '@vuelidate/core'
import { cloneDeep } from 'lodash-es'
import { useProjectStore } from '../../../store/ProjectStore'
import { useNotificationStore } from '../../../store/NotificationStore'
import type { CreateProject } from '../../../api/models/CreateProject'
import type { Project } from '../../../api/models/Project'
import FormButton from '../elements/FormButton.vue'
import Tabs from '../../layouts/Tabs.vue'
import { defaultProjectForm } from '../../../mocks/Defaults'
import ProjectGeneralForm from './ProjectGeneralForm.vue'
import ProjectImagesForm from './ProjectImagesForm.vue'
import ProjectRepositoriesForm from './ProjectRepositoriesForm.vue'
import ProjectDescriptionForm from './ProjectDescriptionForm.vue'

interface Data {
  activeTab: number
  form: CreateProject
  validated: boolean
}

export default defineComponent({
  name: 'ProjectForm',
  components: {
    FormButton,
    Tabs,
    ProjectGeneralForm,
    ProjectDescriptionForm,
    ProjectImagesForm,
    ProjectRepositoriesForm,
  },
  props: {
    slug: {
      type: String,
      default: null,
    },
  },
  setup() {
    return { v$: useVuelidate() }
  },
  data(): Data {
    return {
      activeTab: 0,
      form: cloneDeep(defaultProjectForm),
      validated: false,
    }
  },
  computed: {
    isUpdate(): boolean {
      return this.slug !== null
    },
    project(): Project | undefined {
      return useProjectStore().project
    },
    projectError(): string | undefined {
      return useProjectStore().projectError
    },
    loading(): boolean {
      return useProjectStore().projectCreating || useProjectStore().projectLoading
    },
    error(): string | undefined {
      return useProjectStore().projectCreateError
    },
  },
  async mounted() {
    if (this.isUpdate) {
      await useProjectStore().getProjectBySlug(this.slug)

      if (!this.projectError && this.project) {
        this.form.title = this.project.title
        this.form.shortDescription = this.project.shortDescription
        this.form.description = this.project.description
        this.form.companyId = this.project.companyId ?? undefined
        this.form.year = this.project.year
        this.form.website = this.project.website ?? ''
        this.form.showMockup = this.project.showMockup
        this.form.tags = this.project.tags.map((tag) => tag.title) ?? []
        this.form.repositories = this.project.repositories ?? []
        this.form.images = this.project.images.map((image) => {
          return {
            id: image.id,
            type: image.type,
            image: undefined,
            order: image.order,
          }
        })
      } else {
        useNotificationStore().displayErrorNotification(this.projectError || 'An error occurred')
      }
    }
  },
  methods: {
    async validate(tab: number): Promise<boolean> {
      switch (tab) {
        case 0:
          return await this.$refs.projectGeneralForm?.validate()
        case 1:
          return await this.$refs.projectDescriptionForm?.validate()
        case 2:
          return await this.$refs.projectImagesForm?.validate()
        case 3:
          return await this.$refs.projectRepositoriesForm?.validate()
        default:
          return false
      }
    },
    async save() {
      const isValid = await this.validate(this.activeTab)
      if (!isValid) return

      let response: Project | undefined

      if (this.isUpdate) {
        if (!this.project) return useNotificationStore().displayErrorNotification('Project not found')

        response = await useProjectStore().updateProject(this.project.id, this.form)
      } else {
        response = await useProjectStore().createProject(this.form)
      }

      if (!this.error && response) {
        await useProjectStore().getProjectBySlug(response.slug)
        this.$router.push(`/projects/${response.slug}`)
        useNotificationStore().displaySuccessNotification(`Project ${this.isUpdate ? 'updated' : 'created'} successfully`)
      } else {
        useNotificationStore().displayErrorNotification(this.error || 'An error occurred')
      }
    },
    async remove() {
      if (!this.project) return useNotificationStore().displayErrorNotification('Project not found')

      await useProjectStore().deleteProject(this.project.id)

      if (!this.error) {
        this.$router.push(`/projects`)
        useNotificationStore().displaySuccessNotification(`Project deleted successfully`)
      } else {
        useNotificationStore().displayErrorNotification(this.error || 'An error occurred')
      }
    },
  },
  watch: {
    async activeTab(newValue: number, oldValue: number) {
      if (oldValue === newValue) return
      if (this.validated) {
        this.validated = false
        return
      }

      const isValid = await this.validate(oldValue)
      if (!isValid) {
        this.validated = true
        this.activeTab = oldValue
      }
    },
  },
})
</script>
