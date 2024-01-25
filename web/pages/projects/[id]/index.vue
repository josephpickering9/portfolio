<template>
  <Skeleton v-if="loading" />
  <div v-else-if="project" class="flex flex-grow flex-col gap-2">
    <div
      v-if="project.backgroundImage"
      class="header-container"
      :style="`background-image: url('${getImageUrl(project.backgroundImage ?? '')}')`"
    ></div>
    <div v-else class="h-72"></div>

    <div class="prose z-10 -mt-28 flex max-w-full items-center justify-between gap-4 px-8">
      <div class="flex items-center gap-4">
        <img v-if="project.image" :src="getImageUrl(project.image)" class="h-10 w-10 rounded-full" />
        <h1 class="title">{{ project.title }}</h1>
        <small class="text-white">{{ project.year }}</small>
      </div>
      <NuxtLink :to="`/projects/${$route.params.id}/update`" class="text-xs">Edit</NuxtLink>
    </div>

    <div class="prose mx-auto flex w-full max-w-7xl flex-col justify-center gap-4 px-8">
      <div class="flex flex-col gap-6 lg:flex-row lg:items-end lg:justify-between">
        <div v-if="Object.keys(groupedTags).length > 0" class="flex flex-wrap items-center gap-4">
          <div v-for="(typeTags, type) in groupedTags" :key="type">
            <small class="m-0 italic">{{ type }}</small>
            <div class="flex flex-wrap gap-2">
              <Tag v-for="(tag, index) in typeTags" :key="index" :tag="tag" />
            </div>
          </div>
        </div>
        <div class="flex items-center gap-6">
          <IconLink v-if="project.website" :to="project.website" label="Website" icon="material-symbols:globe" />
          <!-- <IconLink v-if="project.repository" :to="project.repository" label="GitHub" icon="mdi:github" /> -->
          <IconLink v-if="project.company" to="#" :label="project.company" icon="material-symbols:business-center" />
        </div>
      </div>

      <h2 class="m-0 text-xl font-normal italic">{{ project.shortDescription }}</h2>

      <h3 class="m-0">Overview</h3>
      <div class="project-description" v-html="project.description" />

      <div v-if="project.website" class="flex flex-col items-center gap-4 md:flex-row">
        <MockupBrowser :url="project.website" />
        <MockupPhone :url="project.website" />
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import type { Project, Tag as TagModel } from '../../../api'
import { useProjectStore } from '../../../store/ProjectStore'
import Skeleton from '../../../components/loading/Skeleton.vue'
import Tag from '../../../components/tags/Tag.vue'
import IconLink from '../../../components/navigation/IconLink.vue'
import MockupBrowser from '../../../components/mockup/MockupBrowser.vue'

export default defineComponent({
  // eslint-disable-next-line vue/match-component-file-name
  name: 'Project',
  components: { Skeleton, Tag, IconLink, MockupBrowser },
  computed: {
    id(): number {
      return Number(this.$route.params.id)
    },
    project(): Project | undefined {
      return useProjectStore().project
    },
    loading(): boolean {
      return useProjectStore().projectLoading
    },
    error(): string | undefined {
      return useProjectStore().projectError
    },
    groupedTags(): Record<string, TagModel[]> {
      if (!this.project) return {}

      const tags = this.project.tags

      const groupedAndSorted = tags.reduce((acc: any, tag: TagModel) => {
        const type = (acc[tag.type] = acc[tag.type] || [])
        type.push(tag)
        acc[tag.type].sort((a: TagModel, b: TagModel) => a.title.localeCompare(b.title))
        return acc
      }, {})

      const sortedGroupTitles = Object.keys(groupedAndSorted).sort((a, b) => a.localeCompare(b))
      const sortedGroupedTags = sortedGroupTitles.reduce((acc: any, title: string) => {
        acc[title] = groupedAndSorted[title]
        return acc
      }, {})

      return sortedGroupedTags
    },
  },
  async mounted() {
    await useProjectStore().getProject(this.id)
  },
})
</script>

<style scoped>
.header-container {
  @apply relative h-72 bg-cover bg-center;
}

.header-container::after {
  content: '';
  @apply absolute left-0 top-0 block h-full w-full bg-gradient-to-b from-transparent to-black;
}

.title {
  @apply relative m-0 text-4xl;
}
</style>

<style>
.project-description p {
  @apply mt-0;
}
</style>
