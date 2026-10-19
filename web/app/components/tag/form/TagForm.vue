<template>
  <FormLayout
    :copy="copy"
    :show-delete="Boolean(id)"
    :loading="loading"
    :saving="tagStore.tagSaving"
    :deleting="tagStore.tagDeleting"
    @save="save"
    @delete="remove"
  >
    <div v-if="tagStore.tagLoading" class="skeleton-pulse h-90 rounded-panel border border-line bg-surface" aria-hidden="true" />
    <div v-else class="grid grid-cols-[repeat(auto-fit,minmax(min(100%,380px),1fr))] items-start gap-5">
      <section class="flex flex-col gap-5.5 rounded-panel border border-line bg-surface p-6.5">
        <FormGroup label="Title" required :errors="v$.form.title.$errors">
          <TextInput v-model="form.title" placeholder="e.g. Vue.js" :disabled="loading" />
        </FormGroup>
        <FormGroup label="Type" label-tag="span">
          <div class="flex flex-wrap gap-2" role="radiogroup" aria-label="Type">
            <FilterChip
              v-for="type in tagTypes"
              :key="type.value"
              :label="type.label"
              :icon="type.icon"
              :active="form.type === type.value"
              @select="form.type = type.value"
            />
          </div>
        </FormGroup>
        <FormGroup label="Icon" helper="Any Iconify name. Prefer simple-icons for brand logos.">
          <TextInput v-model="form.icon" :icon="previewIcon" placeholder="simple-icons:vuedotjs" mono :disabled="loading" />
        </FormGroup>
      </section>
      <aside class="sticky top-24 flex flex-col gap-3.5">
        <span class="eyebrow tracking-[0.05em] text-muted">Live preview</span>
        <TagListItem :tag="previewTag" :count="usage" preview highlighted />
        <span class="text-[13px] leading-[1.55] text-muted">
          Shown under {{ getTagTypeLabel(form.type) }} on the Technologies page and in project stacks.
        </span>
      </aside>
    </div>
  </FormLayout>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useVuelidate } from '@vuelidate/core'
import { required } from '@vuelidate/validators'
import type { CreateTag, Tag } from '@api'
import { useTagStore } from '~/store/TagStore'
import { useProjectStore } from '~/store/ProjectStore'
import { useEntityForm } from '~/composables/useEntityForm'
import { tagTypes } from '~/data/TagTypes'
import { defaultTagForm } from '~/utils/default-helper'
import { getEntityFormCopy } from '~/utils/entity-form-helper'
import { countProjectsUsingTag } from '~/utils/project-helper'
import { getTagIcon, getTagTypeLabel, tagToForm } from '~/utils/tag-helper'
import FormLayout from '~/components/ui/layout/FormLayout.vue'
import FormGroup from '~/components/ui/form/FormGroup.vue'
import TextInput from '~/components/ui/input/TextInput.vue'
import FilterChip from '~/components/ui/filter/FilterChip.vue'
import TagListItem from '~/components/tag/list/TagListItem.vue'

interface Props {
  id?: string
}

const props = withDefaults(defineProps<Props>(), {
  id: undefined,
})

const ENTITY_NAME = 'technology'
const PREVIEW_ID = 'preview'
const RULES = { form: { title: { required } } }

const tagStore = useTagStore()
const projectStore = useProjectStore()
const entityForm = useEntityForm<Tag, CreateTag>({
  entityName: ENTITY_NAME,
  listRoute: '/technologies',
  actions: {
    get: tagStore.getTag,
    create: tagStore.createTag,
    update: tagStore.updateTag,
    delete: tagStore.deleteTag,
  },
  state: {
    loadError: () => tagStore.tagError,
    saveError: () => tagStore.tagSaveError,
    deleteError: () => tagStore.tagDeleteError,
  },
})

const form = ref<CreateTag>({ ...defaultTagForm })
const v$ = useVuelidate(RULES, { form })

const loading = computed(() => Boolean(tagStore.tagSaving || tagStore.tagLoading))
const copy = computed(() => getEntityFormCopy(ENTITY_NAME, Boolean(props.id), form.value.title))
const previewIcon = computed(() => getTagIcon(form.value))
const previewTag = computed(() => ({ id: props.id ?? PREVIEW_ID, title: form.value.title, type: form.value.type, icon: form.value.icon }))
const usage = computed(() => countProjectsUsingTag(projectStore.projects, props.id))

async function save() {
  if (!(await v$.value.$validate())) return entityForm.invalid()
  await entityForm.save(props.id, form.value)
}

async function remove() {
  if (props.id) await entityForm.remove(props.id)
}

onMounted(async () => {
  projectStore.ensureProjects()
  if (!props.id) return

  const tag = await entityForm.fetch(props.id)
  if (tag) form.value = tagToForm(tag)
})
</script>
