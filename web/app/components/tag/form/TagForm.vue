<template>
  <div class="prose w-full max-w-xl space-y-5 rounded-md border border-gray-600 px-6 py-4">
    <h1>{{ isUpdate ? 'Update' : 'New' }} Tag</h1>
    <TextInput v-model="title" label="Title" :disabled="loading" />
    <TagTypeSelectList v-model="type" label="Type" :disabled="loading" />
    <IconAutoComplete v-model="icon" label="Icon" :disabled="loading" />
    <ColourPicker v-model="customColour" label="Custom Colour" :disabled="loading" />
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
import { useTagStore } from '~/store/TagStore'
import { useNotificationStore } from '~/store/NotificationStore'
import { TagType, type CreateTag, type Tag } from '@api'
import TextInput from '~/components/ui/input/TextInput.vue'
import FormButton from '~/components/ui/form/FormButton.vue'
import ColourPicker from '~/components/ui/input/ColourPicker.vue'
import IconAutoComplete from '~/components/ui/input/IconAutoComplete.vue'
import TagTypeSelectList from './TagTypeSelectList.vue'

interface Props {
  id?: number | null
}

const props = withDefaults(defineProps<Props>(), {
  id: null,
})

const router = useRouter()
const tagStore = useTagStore()
const notificationStore = useNotificationStore()

const title = ref('')
const type = ref<TagType>(TagType.DEFAULT)
const icon = ref<string | undefined>(undefined)
const customColour = ref<string | undefined>(undefined)

const isUpdate = computed((): boolean => {
  return props.id !== null
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

async function save() {
  let response: Tag | undefined

  if (isUpdate.value) {
    response = await tagStore.updateTag(props.id!, createTagValue.value)
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
  if (isUpdate.value) {
    await tagStore.deleteTag(props.id!)

    if (!error.value) {
      router.push(`/tags`)
      notificationStore.displaySuccessNotification('Tag deleted successfully')
    } else {
      notificationStore.displayErrorNotification(error.value || 'An error occurred')
    }
  }
}

onMounted(async () => {
  if (isUpdate.value) {
    await tagStore.getTag(props.id!)

    if (!tagError.value && tag.value) {
      title.value = tag.value.title
      type.value = tag.value.type
      icon.value = tag.value.icon ?? ''
      customColour.value = tag.value.customColour ?? ''
    } else {
      notificationStore.displayErrorNotification(tagError.value || 'An error occurred')
    }
  }
})
</script>
