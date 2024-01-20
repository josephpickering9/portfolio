<template>
  <div class="flex w-full flex-col items-center space-y-4">
    <div class="prose w-full max-w-3xl space-y-5 rounded-md border border-gray-600 px-6 py-4">
      <h1>New Project</h1>
      <div class="flex flex-col gap-4 md:flex-row">
        <TextInput v-model="title" label="Title" class="w-full md:w-3/4" />
        <YearSelectList v-model="year" label="Year" class="w-full md:w-1/4" />
      </div>
      <TextInput v-model="shortDescription" label="Short Description" />
      <TextEditor v-model="description" label="Description" />
      <div class="flex flex-col gap-4 md:flex-row">
        <TextInput v-model="company" label="Company" class="w-full md:w-1/2" />
        <TextInput v-model="website" label="Website" class="w-full md:w-1/2" />
      </div>
      <div class="flex flex-col gap-4 md:flex-row">
        <FileInput label="Image" class="w-full md:w-1/2" @update:file="image = $event" />
        <FileInput label="Background Image" class="w-full md:w-1/2" @update:file="backgroundImage = $event" />
      </div>
      <!-- <TextInput v-model="tags" label="Tags" /> -->
      <button class="btn btn-primary" @click="save">Save</button>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { useProjectStore } from '../../store/ProjectStore'
import { useNotificationStore } from '../../store/NotificationStore'
import type { CreateProject } from '../../api/models/CreateProject'
import TextInput from '../../components/forms/TextInput.vue'
import TextEditor from '../../components/forms/TextEditor.vue'
import YearSelectList from '../../components/forms/global/YearSelectList.vue'
import FileInput from '../../components/forms/FileInput.vue'

interface Data {
  title: string
  shortDescription: string
  description: string
  company: string
  image: FileList | null
  backgroundImage: FileList | null
  year: number
  website: string
  tags: string[]
}

export default defineComponent({
  // eslint-disable-next-line vue/match-component-file-name
  name: 'NewProject',
  components: {
    TextEditor,
    TextInput,
    YearSelectList,
    FileInput,
  },
  data(): Data {
    return {
      title: '',
      shortDescription: '',
      description: '',
      company: '',
      image: null,
      backgroundImage: null,
      year: new Date().getFullYear(),
      website: '',
      tags: [],
    }
  },
  computed: {
    loading(): boolean {
      return useProjectStore().projectCreating
    },
    error(): string | undefined {
      return useProjectStore().projectCreateError
    },
  },
  methods: {
    async save() {
      const project: CreateProject = {
        title: this.title,
        shortDescription: this.shortDescription,
        description: this.description,
        company: this.company,
        image: this.image ? (this.image.item(0) as Blob) : null,
        backgroundImage: this.backgroundImage ? (this.backgroundImage.item(0) as Blob) : null,
        year: this.year,
        website: this.website,
        tags: ['tag'],
      }

      const response = await useProjectStore().createProject(project)

      if (!this.error && response) {
        this.$router.push(`/projects/${response.id}`)
        useNotificationStore().displaySuccessNotification('Project created successfully')
      } else {
        useNotificationStore().displayErrorNotification(this.error || 'An error occurred')
      }
    },
  },
})
</script>
