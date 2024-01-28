<template>
  <div :class="{ 'search-active': isSearchActive }" class="search-box-container">
    <TagAutoComplete
      v-model="tags"
      type="text"
      class="search-input"
      placeholder="Search"
      :show-empty-message="false"
      @focus="handleFocus"
      @blur="handleBlur"
    />
    <div v-if="isSearchActive" class="search-results">
      <ProjectList :tags="tags" />
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { isEmpty } from 'lodash'
import { useProjectStore } from '../store/ProjectStore'
import ProjectList from '../components/lists/ProjectList.vue'
import TagAutoComplete from '../components/forms/global/TagAutoComplete.vue'

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
      await useProjectStore().getProjects(this.search)
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
