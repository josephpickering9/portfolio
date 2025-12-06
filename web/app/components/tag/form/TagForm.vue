<template>
  <div class="w-full max-w-5xl">
    <div class="mb-8">
      <h1>
        <span v-if="initialLoad" class="skeleton block h-10 w-48 rounded-lg"></span>
        <span v-else class="text-4xl font-extrabold text-primary">{{ isUpdate ? 'Update' : 'Create' }} Tag</span>
      </h1>
      <p v-if="initialLoad" class="skeleton mt-2 h-5 w-64 rounded"></p>
      <p v-else class="text-base-content/60">
        {{ isUpdate ? 'Modify existing tag details' : 'Add a new tag to the collection' }}
      </p>
    </div>

    <!-- Skeleton Loading State -->
    <div v-if="initialLoad" class="grid gap-8 lg:grid-cols-2">
      <!-- Form Skeleton -->
      <div class="card bg-base-100 p-8 shadow-xl">
        <div class="flex flex-col gap-6">
          <div class="space-y-4">
            <div class="skeleton h-4 w-16 rounded"></div>
            <div class="skeleton h-10 w-full rounded-lg"></div>
          </div>
          <div class="space-y-4">
            <div class="skeleton h-4 w-16 rounded"></div>
            <div class="skeleton h-10 w-full rounded-lg"></div>
          </div>
          <div class="space-y-4">
            <div class="skeleton h-4 w-16 rounded"></div>
            <div class="skeleton h-10 w-full rounded-lg"></div>
          </div>
          <div class="space-y-4">
            <div class="skeleton h-4 w-28 rounded"></div>
            <div class="skeleton h-10 w-full rounded-lg"></div>
          </div>
          <div class="flex justify-end gap-3 pt-4">
            <div class="skeleton h-10 w-24 rounded-lg"></div>
            <div class="skeleton h-10 w-32 rounded-lg"></div>
          </div>
        </div>
      </div>

      <!-- Preview Skeleton -->
      <div class="flex flex-col space-y-4">
        <div class="skeleton h-4 w-24 rounded"></div>
        <div class="sticky top-24 space-y-8">
          <div class="skeleton h-48 w-full rounded-xl"></div>
          <div class="skeleton h-24 w-full rounded-xl"></div>
        </div>
      </div>
    </div>

    <!-- Actual Content -->
    <div v-else class="grid gap-8 lg:grid-cols-2">
      <!-- Form Section -->
      <div class="card bg-base-100 p-8 shadow-xl">
        <div class="flex flex-col gap-6">
          <TextInput v-model="title" label="Title" placeholder="e.g. Vue.js" :disabled="loading" />
          <TagTypeSelectList v-model="type" label="Type" :disabled="loading" />
          <IconAutoComplete v-model="icon" label="Icon" :disabled="loading" />
          <ColourPicker v-model="customColour" label="Custom Colour" :disabled="loading" />

          <div class="flex items-center justify-between space-x-3 pt-4">
             <FormButton
              v-if="isUpdate"
              label="Delete"
              type="error"
              size="md"
              icon="material-symbols:delete"
              :disabled="loading"
              variant="ghost"
              @click="remove"
            />
            <FormButton label="Save Tag" type="primary" size="md" :loading="loading" icon="heroicons:check" @click="save" />
          </div>
        </div>
      </div>

      <!-- Preview Section -->
      <div class="flex flex-col space-y-4">
        <h3 class="ml-1 text-sm font-medium uppercase tracking-wider text-base-content/60">Live Preview</h3>
        <div class="sticky top-24">
          <TagListItem :tag="previewTag" preview />
          
          <div class="alert alert-info mt-8">
            <Icon name="heroicons:information-circle" class="mr-2 h-5 w-5" />
            <div>
              <h3 class="font-bold">Pro Tip</h3>
              <div class="text-xs">Tags are used to categorize projects. Selecting the correct type ensures it appears in the right section.</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useTagStore } from '~/store/TagStore'
import { useNotificationStore } from '~/store/NotificationStore'
import { TagType, type CreateTag, type Tag } from '@api'
import TextInput from '~/components/ui/input/TextInput.vue'
import FormButton from '~/components/ui/form/FormButton.vue'
import ColourPicker from '~/components/ui/input/ColourPicker.vue'
import IconAutoComplete from '~/components/ui/input/IconAutoComplete.vue'
import TagListItem from '~/components/tag/list/TagListItem.vue'
import TagTypeSelectList from './TagTypeSelectList.vue'

interface Props {
  id?: string
}

const props = withDefaults(defineProps<Props>(), {
  id: undefined,
})

const router = useRouter()
const tagStore = useTagStore()
const notificationStore = useNotificationStore()

const title = ref('')
const type = ref<TagType>(TagType.DEFAULT)
const icon = ref<string | undefined>(undefined)
const customColour = ref<string | undefined>(undefined)
const initialLoad = ref(!!props.id)

const isUpdate = computed((): boolean => {
  return props.id != null
})

const tag = computed((): Tag | undefined => {
  return tagStore.tag
})

const tagError = computed((): string | undefined => {
  return tagStore.tagError
})

const loading = computed((): boolean => {
  return tagStore.tagCreating || tagStore.tagLoading
})

const error = computed((): string | undefined => {
  return tagStore.tagCreateError
})

const createTagValue = computed((): CreateTag => {
  return {
    title: title.value,
    type: type.value,
    icon: icon.value,
    customColour: customColour.value,
  }
})

const previewTag = computed((): Tag => {
  return {
    id: 'preview',
    title: title.value || 'New Tag',
    type: type.value,
    icon: icon.value,
    customColour: customColour.value,
    slug: 'preview-slug'
  }
})

async function save() {
  let response: Tag | undefined

  if (isUpdate.value && props.id) {
    response = await tagStore.updateTag(props.id, createTagValue.value)
  } else {
    response = await tagStore.createTag(createTagValue.value)
  }

  if (!error.value && response) {
    router.push(`/tags`)
    notificationStore.displaySuccessNotification(`Tag ${isUpdate.value ? 'updated' : 'created'} successfully`)
  } else {
    notificationStore.displayErrorNotification(error.value || 'An error occurred')
  }
}

async function remove() {
  if (isUpdate.value && props.id) {
    await tagStore.deleteTag(props.id)

    if (!error.value) {
      router.push(`/tags`)
      notificationStore.displaySuccessNotification('Tag deleted successfully')
    } else {
      notificationStore.displayErrorNotification(error.value || 'An error occurred')
    }
  }
}

onMounted(async () => {
  if (isUpdate.value && props.id) {
    // initialLoad is already true
    await tagStore.getTag(props.id)

    if (!tagError.value && tag.value) {
      title.value = tag.value.title
      type.value = tag.value.type
      icon.value = tag.value.icon ?? ''
      customColour.value = tag.value.customColour ?? ''
    } else {
      notificationStore.displayErrorNotification(tagError.value || 'An error occurred')
    }
    initialLoad.value = false
  } else {
    initialLoad.value = false
  }
})
</script>
