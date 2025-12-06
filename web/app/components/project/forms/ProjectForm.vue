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

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useVuelidate } from '@vuelidate/core'
import { cloneDeep } from 'lodash-es'
import { useProjectStore } from '~/store/ProjectStore'
import { useNotificationStore } from '~/store/NotificationStore'
import type { CreateProject } from '@api/models/CreateProject'
import type { Project } from '@api/models/Project'
import { defaultProjectForm } from '~/utils/default-helper'
import FormButton from '~/components/ui/form/FormButton.vue'
import Tabs from '~/components/layout/Tabs.vue'
import ProjectGeneralForm from './ProjectGeneralForm.vue'
import ProjectImagesForm from './ProjectImagesForm.vue'
import ProjectRepositoriesForm from './ProjectRepositoriesForm.vue'
import ProjectDescriptionForm from './ProjectDescriptionForm.vue'

interface Props {
  slug?: string | null
}

const props = withDefaults(defineProps<Props>(), {
  slug: null,
})

const router = useRouter()
const projectStore = useProjectStore()
const notificationStore = useNotificationStore()

const activeTab = ref(0)
const form = ref<CreateProject>(cloneDeep(defaultProjectForm))
const validated = ref(false)

// Template Refs
const projectGeneralForm = ref<InstanceType<typeof ProjectGeneralForm> | null>(null)
const projectDescriptionForm = ref<InstanceType<typeof ProjectDescriptionForm> | null>(null)
const projectImagesForm = ref<InstanceType<typeof ProjectImagesForm> | null>(null)
const projectRepositoriesForm = ref<InstanceType<typeof ProjectRepositoriesForm> | null>(null)

// Validation
const v$ = useVuelidate()

const isUpdate = computed((): boolean => {
  return props.slug !== null
})

const project = computed((): Project | undefined => {
  return projectStore.project
})

const projectError = computed((): string | undefined => {
  return projectStore.projectError
})

const loading = computed((): boolean => {
  return projectStore.projectCreating || projectStore.projectLoading
})

const error = computed((): string | undefined => {
  return projectStore.projectCreateError
})

async function validate(tab: number): Promise<boolean> {
  switch (tab) {
    case 0:
      return (await projectGeneralForm.value?.validate()) ?? false
    case 1:
      return (await projectDescriptionForm.value?.validate()) ?? false
    case 2:
      return (await projectImagesForm.value?.validate()) ?? false
    case 3:
      return (await projectRepositoriesForm.value?.validate()) ?? false
    default:
      return false
  }
}

async function save() {
  const isValid = await validate(activeTab.value)
  if (!isValid) return

  let response: Project | undefined

  if (isUpdate.value) {
    if (!project.value) return notificationStore.displayErrorNotification('Project not found')

    response = await projectStore.updateProject(project.value.id, form.value)
  } else {
    response = await projectStore.createProject(form.value)
  }

  if (!error.value && response) {
    await projectStore.getProjectBySlug(response.slug)
    router.push(`/projects/${response.slug}`)
    notificationStore.displaySuccessNotification(`Project ${isUpdate.value ? 'updated' : 'created'} successfully`)
  } else {
    notificationStore.displayErrorNotification(error.value || 'An error occurred')
  }
}

async function remove() {
  if (!project.value) return notificationStore.displayErrorNotification('Project not found')

  await projectStore.deleteProject(project.value.id)

  if (!error.value) {
    router.push(`/projects`)
    notificationStore.displaySuccessNotification(`Project deleted successfully`)
  } else {
    notificationStore.displayErrorNotification(error.value || 'An error occurred')
  }
}

onMounted(async () => {
  if (isUpdate.value) {
    await projectStore.getProjectBySlug(props.slug!)

    if (!projectError.value && project.value) {
      form.value.title = project.value.title
      form.value.shortDescription = project.value.shortDescription
      form.value.description = project.value.description
      form.value.companyId = project.value.companyId ?? undefined
      form.value.year = project.value.year
      form.value.website = project.value.website ?? ''
      form.value.showMockup = project.value.showMockup
      form.value.tags = project.value.tags.map((tag) => tag.title) ?? []
      form.value.repositories = project.value.repositories ?? []
      form.value.images = project.value.images.map((image) => {
        return {
          id: image.id,
          type: image.type,
          image: undefined,
          order: image.order,
        }
      })
    } else {
      notificationStore.displayErrorNotification(projectError.value || 'An error occurred')
    }
  }
})


watch(activeTab, async (newValue: number, oldValue: number) => {
  if (oldValue === newValue) return
  if (validated.value) {
    validated.value = false
    return
  }

  const isValid = await validate(oldValue)
  if (!isValid) {
    validated.value = true
    activeTab.value = oldValue
  }
})
</script>
