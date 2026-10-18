<template>
  <FormLayout
    :eyebrow="id ? 'Update technology' : 'New technology'"
    :title="form.title || (id ? 'Untitled technology' : 'New technology')"
    :save-label="id ? 'Save changes' : 'Create technology'"
    delete-title="Delete this technology?"
    :show-delete="Boolean(id)"
    :loading="loading"
    :saving="tagStore.tagCreating"
    @save="save"
    @delete="id && remove(id)"
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
        <TagListItem :tag="{ id: id ?? 'preview', title: form.title, type: form.type, icon: form.icon }" :count="usage" preview highlighted />
        <span class="text-[13px] leading-[1.55] text-muted">Shown under {{ form.type }} on the Technologies page and in project stacks.</span>
      </aside>
    </div>
  </FormLayout>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useVuelidate } from '@vuelidate/core'
import { required } from '@vuelidate/validators'
import { TagType } from '@api'
import type { CreateTag, Tag } from '@api'
import { useTagStore } from '~/store/TagStore'
import { useProjectStore } from '~/store/ProjectStore'
import { useEntityForm } from '~/composables/useEntityForm'
import { tagTypes } from '~/data/TagTypes'
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

const tagStore = useTagStore()
const projectStore = useProjectStore()

const form = ref<CreateTag>({ title: '', type: TagType.FRONTEND, icon: '', customColour: undefined })

const loading = computed(() => (tagStore.tagCreating || tagStore.tagLoading) ?? false)
const previewIcon = computed(() => form.value.icon || tagTypes.find((type) => type.value === form.value.type)?.icon || 'heroicons:tag')
const usage = computed(() => projectStore.projects.filter((project) => project.tags.some((tag) => tag.id === props.id)).length)

const v$ = useVuelidate({ form: { title: { required } } }, { form })

const entityForm = useEntityForm<Tag, CreateTag>({
  entityName: 'technology',
  redirectRoute: '/technologies',
  actions: {
    get: tagStore.getTag,
    create: tagStore.createTag,
    update: tagStore.updateTag,
    delete: tagStore.deleteTag,
  },
  state: {
    loadError: () => tagStore.tagError,
    saveError: () => tagStore.tagCreateError,
  },
})
const { remove } = entityForm

async function save() {
  if (!(await v$.value.$validate())) return entityForm.invalid()
  await entityForm.save(props.id, form.value)
}

onMounted(async () => {
  if (!projectStore.projectsForm.data) projectStore.getProjects()
  if (!props.id) return

  const tag = await entityForm.fetch(props.id)
  if (tag) form.value = { title: tag.title, type: tag.type, icon: tag.icon ?? '', customColour: tag.customColour }
})
</script>
