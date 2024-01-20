<template>
  <div class="prose w-full max-w-3xl space-y-5 rounded-md border border-gray-600 px-6 py-4">
    <h1>{{ isUpdate ? 'Update' : 'New' }} Project</h1>
    <div class="flex flex-col gap-4 md:flex-row">
      <TextInput v-model="title" label="Title" class="w-full md:w-3/4" :disabled="loading" />
      <YearSelectList v-model="year" label="Year" class="w-full md:w-1/4" :disabled="loading" />
    </div>
    <TextInput v-model="shortDescription" label="Short Description" :disabled="loading" />
    <TextEditor v-model="description" label="Description" :disabled="loading" />
    <div class="flex flex-col gap-4 md:flex-row">
      <TextInput v-model="company" label="Company" class="w-full md:w-1/2" :disabled="loading" />
      <TextInput v-model="website" label="Website" class="w-full md:w-1/2" :disabled="loading" />
    </div>
    <div class="flex flex-col gap-4 md:flex-row">
      <FileInput
        label="Image"
        class="w-full md:w-1/2"
        :image-url="imageUrl"
        :disabled="loading"
        @update:file="image = $event"
      />
      <FileInput
        label="Background Image"
        class="w-full md:w-1/2"
        :image-url="backgroundImageUrl"
        :disabled="loading"
        @update:file="backgroundImage = $event"
      />
    </div>
    <!-- <TextInput v-model="tags" label="Tags" /> -->
    <button class="btn btn-primary" @click="save">Save</button>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { useProjectStore } from '../../../store/ProjectStore'
import { useNotificationStore } from '../../../store/NotificationStore'
import type { CreateProject } from '../../../api/models/CreateProject'
import type { Project } from '../../../api/models/Project'
import type { Tag } from '../../../api/models/Tag'
import TextInput from '../TextInput.vue'
import TextEditor from '../TextEditor.vue'
import FileInput from '../FileInput.vue'
import { getImageUrl } from '../../../utils/image-helper'
import YearSelectList from './YearSelectList.vue'

interface Data {
  title: string
  shortDescription: string
  description: string
  company: string
  image: FileList | null
  backgroundImage: FileList | null
  imageUrl?: string
  backgroundImageUrl?: string
  year: number
  website: string
  tags: Tag[]
}

export default defineComponent({
  name: 'ProjectForm',
  components: {
    TextEditor,
    TextInput,
    YearSelectList,
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
      title: '',
      shortDescription: '',
      description: '',
      company: '',
      image: null,
      backgroundImage: null,
      imageUrl: undefined,
      backgroundImageUrl: undefined,
      year: new Date().getFullYear(),
      website: '',
      tags: [],
    }
  },
  computed: {
    isUpdate(): boolean {
      return this.id !== null
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
    createProjectValue(): CreateProject {
      return {
        title: this.title,
        shortDescription: this.shortDescription,
        description: this.description,
        company: this.company,
        image: this.image ? (this.image.item(0) as Blob) : undefined,
        backgroundImage: this.backgroundImage ? (this.backgroundImage.item(0) as Blob) : undefined,
        year: this.year,
        website: this.website,
        tags: ['tag'],
      }
    },
  },
  async mounted() {
    if (this.isUpdate) {
      await useProjectStore().getProject(this.id)

      if (!this.projectError && this.project) {
        this.title = this.project.title
        this.shortDescription = this.project.shortDescription
        this.description = this.project.description
        this.company = this.project.company
        this.year = this.project.year
        this.website = this.project.website ?? ''
        this.tags = this.project.tags ?? []
        this.imageUrl = this.project.image ? getImageUrl(this.project.image) : undefined
        this.backgroundImageUrl = this.project.backgroundImage ? getImageUrl(this.project.backgroundImage) : undefined
      } else {
        useNotificationStore().displayErrorNotification(this.projectError || 'An error occurred')
      }
    }
  },
  methods: {
    async save() {
      let response: Project | undefined

      if (this.isUpdate) {
        response = await useProjectStore().updateProject(this.id, this.createProjectValue)
      } else {
        response = await useProjectStore().createProject(this.createProjectValue)
      }

      if (!this.error && response) {
        this.$router.push(`/projects/${response.id}`)
        useNotificationStore().displaySuccessNotification(`Project ${this.isUpdate ? 'updated' : 'created'} successfully`)
      } else {
        useNotificationStore().displayErrorNotification(this.error || 'An error occurred')
      }
    },
  },
})
</script>
