<template>
  <FormLayout
    :copy="copy"
    :show-delete="isUpdate"
    :loading="loading"
    :saving="projectStore.projectSaving"
    :deleting="projectStore.projectDeleting"
    @save="save"
    @delete="remove"
  >
    <div v-if="projectStore.projectLoading" class="skeleton-pulse h-130 rounded-panel border border-line bg-surface" aria-hidden="true" />
    <div v-else class="rounded-panel border border-line bg-surface">
      <Tabs v-model:active-tab="activeTab" :tabs="tabItems" />
      <div class="p-7">
        <div v-show="activeTab === Tab.General" :id="tabPanelId(Tab.General)" role="tabpanel" :aria-labelledby="tabId(Tab.General)">
          <ProjectGeneralForm ref="general" v-model="form" :disabled="loading" />
        </div>
        <div v-show="activeTab === Tab.Description" :id="tabPanelId(Tab.Description)" role="tabpanel" :aria-labelledby="tabId(Tab.Description)">
          <ProjectDescriptionForm ref="description" v-model="form" :project-id="projectId" :disabled="loading" />
        </div>
        <div v-show="activeTab === Tab.CaseStudy" :id="tabPanelId(Tab.CaseStudy)" role="tabpanel" :aria-labelledby="tabId(Tab.CaseStudy)">
          <ProjectCaseStudyForm ref="caseStudy" v-model="form" :disabled="loading" />
        </div>
        <div v-show="activeTab === Tab.Images" :id="tabPanelId(Tab.Images)" role="tabpanel" :aria-labelledby="tabId(Tab.Images)">
          <ProjectImagesForm ref="images" v-model="form" v-model:images="imageSlots" :disabled="loading" />
        </div>
        <div
          v-show="activeTab === Tab.Repositories"
          :id="tabPanelId(Tab.Repositories)"
          role="tabpanel"
          :aria-labelledby="tabId(Tab.Repositories)"
        >
          <ProjectRepositoriesForm ref="repositories" v-model="form" />
        </div>
      </div>
    </div>
  </FormLayout>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, useTemplateRef } from 'vue'
import { cloneDeep } from 'lodash-es'
import type { CreateProject, Project } from '@api'
import { useProjectStore } from '~/store/ProjectStore'
import { useNotificationStore } from '~/store/NotificationStore'
import { useEntityForm } from '~/composables/useEntityForm'
import { defaultProjectForm, emptyImageSlots } from '~/utils/default-helper'
import { getEntityFormCopy } from '~/utils/entity-form-helper'
import { toProjectImages } from '~/utils/upload-helper'
import type { ProjectImageSlots } from '~/utils/upload-helper'
import { projectToForm, projectToImageSlots } from '~/utils/project-form-helper'
import { useImageUrl } from '~/composables/useImageUrl'
import FormLayout from '~/components/ui/layout/FormLayout.vue'
import Tabs from '~/components/ui/layout/Tabs.vue'
import { tabId, tabPanelId } from '~/utils/tab-helper'
import ProjectGeneralForm from './ProjectGeneralForm.vue'
import ProjectDescriptionForm from './ProjectDescriptionForm.vue'
import ProjectCaseStudyForm from './ProjectCaseStudyForm.vue'
import ProjectImagesForm from './ProjectImagesForm.vue'
import ProjectRepositoriesForm from './ProjectRepositoriesForm.vue'

interface Props {
  slug?: string
}

const props = withDefaults(defineProps<Props>(), {
  slug: undefined,
})

const ENTITY_NAME = 'project'
const NOT_LOADED_MESSAGE = "Couldn't save: the project didn't load. Reload and try again."
const Tab = { General: 0, Description: 1, CaseStudy: 2, Images: 3, Repositories: 4 } as const
const TAB_LABELS = ['General', 'Description', 'Case Study', 'Images', 'Repositories']

const projectStore = useProjectStore()
const notificationStore = useNotificationStore()
const { getImageUrl } = useImageUrl()
const entityForm = useEntityForm<Project, CreateProject>({
  entityName: ENTITY_NAME,
  listRoute: '/projects',
  savedRoute: (saved) => `/projects/${saved.slug}`,
  actions: {
    get: projectStore.getProjectBySlug,
    create: projectStore.createProject,
    update: projectStore.updateProject,
    delete: projectStore.deleteProject,
  },
  state: {
    loadError: () => projectStore.projectError,
    saveError: () => projectStore.projectSaveError,
    deleteError: () => projectStore.projectDeleteError,
  },
})
const sections = [
  useTemplateRef<InstanceType<typeof ProjectGeneralForm>>('general'),
  useTemplateRef<InstanceType<typeof ProjectDescriptionForm>>('description'),
  useTemplateRef<InstanceType<typeof ProjectCaseStudyForm>>('caseStudy'),
  useTemplateRef<InstanceType<typeof ProjectImagesForm>>('images'),
  useTemplateRef<InstanceType<typeof ProjectRepositoriesForm>>('repositories'),
]

const activeTab = ref<number>(Tab.General)
const form = ref<CreateProject>(cloneDeep(defaultProjectForm))
const imageSlots = ref<ProjectImageSlots>(cloneDeep(emptyImageSlots))

const isUpdate = computed(() => Boolean(props.slug))
const projectId = computed(() => (isUpdate.value ? projectStore.project?.id : undefined))
const loading = computed(() => Boolean(projectStore.projectSaving || projectStore.projectLoading))
const copy = computed(() => getEntityFormCopy(ENTITY_NAME, isUpdate.value, form.value.title))
const tabItems = computed(() =>
  TAB_LABELS.map((label, index) => ({ label, hasError: sections[index]?.value?.hasErrors ?? false })),
)

async function validateAll(): Promise<boolean> {
  const results = await Promise.all(sections.map((section) => section.value?.validate() ?? true))
  const firstInvalid = results.findIndex((valid) => !valid)
  if (firstInvalid === -1) return true

  activeTab.value = firstInvalid
  entityForm.invalid()
  return false
}

async function save() {
  if (!(await validateAll())) return

  const id = projectId.value
  if (isUpdate.value && !id) {
    notificationStore.displayErrorNotification(NOT_LOADED_MESSAGE)
    return
  }
  await entityForm.save(id, { ...form.value, images: toProjectImages(imageSlots.value) })
}

async function remove() {
  const id = projectId.value
  if (id) await entityForm.remove(id)
}

onMounted(async () => {
  const slug = props.slug
  if (!slug) return

  const project = await entityForm.fetch(slug)
  if (!project) return

  form.value = projectToForm(project)
  imageSlots.value = projectToImageSlots(project, getImageUrl)
})
</script>
