<template>
  <FormLayout
    :eyebrow="isUpdate ? 'Update project' : 'New project'"
    :title="form.title || (isUpdate ? 'Untitled project' : 'New project')"
    :save-label="isUpdate ? 'Save changes' : 'Create project'"
    delete-title="Delete this project?"
    :show-delete="isUpdate"
    :loading="loading"
    :saving="projectStore.projectCreating"
    @save="save"
    @delete="remove"
  >
    <div v-if="projectStore.projectLoading" class="skeleton-pulse h-130 rounded-panel border border-line bg-surface" aria-hidden="true" />
    <div v-else class="rounded-panel border border-line bg-surface">
      <Tabs v-model:active-tab="activeTab" :tabs="tabItems" />
      <div class="p-7">
        <div v-show="activeTab === 0" id="tab-panel-0" role="tabpanel" aria-labelledby="tab-0">
          <ProjectGeneralForm ref="general" v-model="form" :disabled="loading" />
        </div>
        <div v-show="activeTab === 1" id="tab-panel-1" role="tabpanel" aria-labelledby="tab-1">
          <ProjectDescriptionForm ref="description" v-model="form" :project-id="projectId" :disabled="loading" />
        </div>
        <div v-show="activeTab === 2" id="tab-panel-2" role="tabpanel" aria-labelledby="tab-2">
          <ProjectCaseStudyForm ref="caseStudy" v-model="form" :disabled="loading" />
        </div>
        <div v-show="activeTab === 3" id="tab-panel-3" role="tabpanel" aria-labelledby="tab-3">
          <ProjectImagesForm ref="images" v-model="form" v-model:images="imageSlots" :disabled="loading" />
        </div>
        <div v-show="activeTab === 4" id="tab-panel-4" role="tabpanel" aria-labelledby="tab-4">
          <ProjectRepositoriesForm ref="repositories" v-model="form" />
        </div>
      </div>
    </div>
  </FormLayout>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, useTemplateRef } from 'vue'
import { useRouter } from 'vue-router'
import { cloneDeep } from 'lodash-es'
import type { CreateProject } from '@api'
import { useProjectStore } from '~/store/ProjectStore'
import { useNotificationStore } from '~/store/NotificationStore'
import { defaultProjectForm } from '~/utils/default-helper'
import { toProjectImages } from '~/utils/upload-helper'
import type { ProjectImageSlots } from '~/utils/upload-helper'
import { projectToForm, projectToImageSlots } from '~/utils/project-form-helper'
import { getImageUrl } from '~/utils/image-helper'
import FormLayout from '~/components/ui/layout/FormLayout.vue'
import Tabs from '~/components/layout/Tabs.vue'
import ProjectGeneralForm from './ProjectGeneralForm.vue'
import ProjectDescriptionForm from './ProjectDescriptionForm.vue'
import ProjectCaseStudyForm from './ProjectCaseStudyForm.vue'
import ProjectImagesForm from './ProjectImagesForm.vue'
import ProjectRepositoriesForm from './ProjectRepositoriesForm.vue'

const TABS = ['General', 'Description', 'Case Study', 'Images', 'Repositories']

interface Props {
  slug?: string
}

const props = withDefaults(defineProps<Props>(), {
  slug: undefined,
})

const router = useRouter()
const projectStore = useProjectStore()
const notificationStore = useNotificationStore()

const sections = [
  useTemplateRef<InstanceType<typeof ProjectGeneralForm>>('general'),
  useTemplateRef<InstanceType<typeof ProjectDescriptionForm>>('description'),
  useTemplateRef<InstanceType<typeof ProjectCaseStudyForm>>('caseStudy'),
  useTemplateRef<InstanceType<typeof ProjectImagesForm>>('images'),
  useTemplateRef<InstanceType<typeof ProjectRepositoriesForm>>('repositories'),
]

const activeTab = ref(0)
const form = ref<CreateProject>(cloneDeep(defaultProjectForm))
const imageSlots = ref<ProjectImageSlots>({ banner: null, card: null, logo: null, desktop: [], mobile: [] })

const isUpdate = computed(() => Boolean(props.slug))
const projectId = computed(() => (isUpdate.value ? projectStore.project?.id : undefined))
const loading = computed(() => projectStore.projectCreating || projectStore.projectLoading)
const tabItems = computed(() => TABS.map((label, index) => ({ label, hasError: sections[index]?.value?.hasErrors ?? false })))

async function validateAll(): Promise<boolean> {
  const results = await Promise.all(sections.map((section) => section.value?.validate() ?? true))
  const firstInvalid = results.findIndex((valid) => !valid)
  if (firstInvalid === -1) return true

  activeTab.value = firstInvalid
  notificationStore.displayErrorNotification('Fix the highlighted fields')
  return false
}

async function save() {
  if (!(await validateAll())) return

  const payload = { ...form.value, images: toProjectImages(imageSlots.value) }
  const id = projectId.value
  if (isUpdate.value && !id) {
    notificationStore.displayErrorNotification("Couldn't save: the project didn't load. Reload and try again.")
    return
  }

  const saved = id ? await projectStore.updateProject(id, payload) : await projectStore.createProject(payload)

  if (projectStore.projectCreateError || !saved) {
    notificationStore.displayErrorNotification(projectStore.projectCreateError || "Couldn't save the project")
    return
  }

  notificationStore.displaySuccessNotification(`Project ${isUpdate.value ? 'updated' : 'created'} successfully`)
  router.push(`/projects/${saved.slug}`)
}

async function remove() {
  const id = projectId.value
  if (!id) return

  await projectStore.deleteProject(id)
  if (projectStore.projectCreateError) {
    notificationStore.displayErrorNotification(projectStore.projectCreateError)
    return
  }

  notificationStore.displaySuccessNotification('Project deleted')
  router.push('/projects')
}

onMounted(async () => {
  const slug = props.slug
  if (!slug) return

  const project = await projectStore.getProjectBySlug(slug)
  if (!project || projectStore.projectError) {
    notificationStore.displayErrorNotification(projectStore.projectError || "Couldn't load the project")
    return
  }

  form.value = projectToForm(project)
  imageSlots.value = projectToImageSlots(project, getImageUrl)
})
</script>
