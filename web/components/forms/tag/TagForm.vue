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

<script lang="ts">
import { defineComponent } from 'vue'
import { useTagStore } from '../../../store/TagStore'
import { useNotificationStore } from '../../../store/NotificationStore'
import type { CreateTag } from '../../../api/models/CreateTag'
import type { Tag } from '../../../api/models/Tag'
import TextInput from '../elements/TextInput.vue'
import FormButton from '../elements/FormButton.vue'
import { TagType } from '../../../api'
import ColourPicker from '../elements/ColourPicker.vue'
import IconAutoComplete from '../global/IconAutoComplete.vue'
import TagTypeSelectList from './TagTypeSelectList.vue'

interface Data {
  title: string
  type: TagType
  icon?: string
  customColour?: string
}

export default defineComponent({
  name: 'TagForm',
  components: {
    TextInput,
    FormButton,
    TagTypeSelectList,
    IconAutoComplete,
    ColourPicker,
  },
  props: {
    id: {
      type: Number,
      default: null,
    },
  },
  data(): Data {
    return {
      title: '',
      type: TagType.DEFAULT,
      icon: undefined,
      customColour: undefined,
    }
  },
  computed: {
    isUpdate(): boolean {
      return this.id !== null
    },
    tag(): Tag | undefined {
      return useTagStore().tag
    },
    tagError(): string | undefined {
      return useTagStore().tagError
    },
    loading(): boolean {
      return useTagStore().tagCreating || useTagStore().tagLoading
    },
    error(): string | undefined {
      return useTagStore().tagCreateError
    },
    createTagValue(): CreateTag {
      return {
        title: this.title,
        type: this.type,
        icon: this.icon,
        customColour: this.customColour,
      }
    },
  },
  async mounted() {
    if (this.isUpdate) {
      await useTagStore().getTag(this.id)

      if (!this.tagError && this.tag) {
        this.title = this.tag.title
        this.type = this.tag.type
        this.icon = this.tag.icon ?? ''
        this.customColour = this.tag.customColour ?? ''
      } else {
        useNotificationStore().displayErrorNotification(this.tagError || 'An error occurred')
      }
    }
  },
  methods: {
    async save() {
      let response: Tag | undefined

      if (this.isUpdate) {
        response = await useTagStore().updateTag(this.id, this.createTagValue)
      } else {
        response = await useTagStore().createTag(this.createTagValue)
      }

      if (!this.error && response) {
        this.$router.push(`/tags`)
        useNotificationStore().displaySuccessNotification(`Tag ${this.isUpdate ? 'updated' : 'created'} successfully`)
      } else {
        useNotificationStore().displayErrorNotification(this.error || 'An error occurred')
      }
    },
    async remove() {
      if (this.isUpdate) {
        await useTagStore().deleteTag(this.id)

        if (!this.error) {
          this.$router.push(`/tags`)
          useNotificationStore().displaySuccessNotification('Tag deleted successfully')
        } else {
          useNotificationStore().displayErrorNotification(this.error || 'An error occurred')
        }
      }
    },
  },
})
</script>
