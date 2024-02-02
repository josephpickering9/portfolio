<template>
  <div :class="{ 'search-active': isSearchActive }" class="search-box-container">
    <TagAutoComplete
      v-model="tags"
      v-model:model-search="search"
      type="text"
      class="search-input"
      placeholder="Search"
      :show-empty-message="false"
      :open-on-focus="false"
      @focus="handleFocus"
      @blur="handleBlur"
    />
    <div v-if="isSearchActive" class="search-results">
      <ProjectList :model-search="search" :tags="tags" />
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { isEmpty } from 'lodash-es'
import { useProjectStore } from '../store/ProjectStore'
import ProjectList from '../components/lists/ProjectList.vue'
import TagAutoComplete from '../components/forms/global/TagAutoComplete.vue'
import useMeta from '../composables/useMeta'

interface Data {
  isSearchActive: boolean
  search: string
  tags: string[]
}

export default defineComponent({
  name: 'Index',
  components: {
    ProjectList,
    TagAutoComplete,
  },
  setup() {
    const { updateMeta } = useMeta()

    updateMeta({ title: 'Home', description: "Work experience portfolio for my life's work" })
  },
  data(): Data {
    return {
      isSearchActive: false,
      search: '',
      tags: [],
    }
  },
  methods: {
    isEmpty,
    async getProjects() {
      await useProjectStore().getProjects()
    },
    handleFocus() {
      this.isSearchActive = true
    },
    handleBlur() {
      if (isEmpty(this.search) && isEmpty(this.tags)) {
        this.isSearchActive = false
      }
    },
  },
})
</script>

<style scoped>
.search-box-container {
  @apply flex flex-grow transform flex-col items-center justify-center space-y-6;
  transition: all 0.3s ease;
}

.search-box-container.search-active {
  @apply justify-start;
}

.search-box-container .search-input {
  @apply z-50 w-full max-w-xs transform;
  transition: all 0.3s ease;
}

.search-box-container.search-active .search-input {
  @apply max-w-5xl;
}

.search-results {
  @apply z-0 flex w-full justify-center;
}
</style>
