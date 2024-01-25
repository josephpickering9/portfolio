<template>
  <div :class="{ 'search-active': isSearchActive }" class="search-box-container">
    <TextInput
      v-model="search"
      type="text"
      class="search-input"
      placeholder="Search"
      @focus="handleFocus"
      @blur="handleBlur"
      @keyup.enter="getProjects"
    />
    <div v-if="isSearchActive && !isEmpty(search)" class="search-results">
      <ProjectList />
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { isEmpty } from 'lodash'
import { useProjectStore } from '../store/ProjectStore'
import ProjectList from '../components/lists/ProjectList.vue'
import TextInput from '../components/forms/TextInput.vue'

interface Data {
  isSearchActive: boolean
  search: string
}

export default defineComponent({
  name: 'Index',
  components: {
    ProjectList,
    TextInput,
  },
  data(): Data {
    return {
      isSearchActive: false,
      search: '',
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
      if (isEmpty(this.search)) {
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
  @apply w-full max-w-xs transform;
  transition: all 0.3s ease;
}

.search-box-container.search-active .search-input {
  @apply max-w-5xl;
}

.search-results {
  @apply flex w-full justify-center;
}
</style>
