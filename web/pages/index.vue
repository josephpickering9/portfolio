<template>
  <div :class="{ 'search-active': isSearchActive }" class="search-box-container">
    <TagAutoComplete
      v-model="tags"
      v-model:model-search="search"
      type="text"
      class="search-input"
      :placeholder="placeholder"
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
import { notEmpty } from '../utils/array-helper'

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

    updateMeta({ title: 'Work Experience', description: "Work experience portfolio for my life's work" })
  },
  data(): Data {
    return {
      isSearchActive: false,
      search: '',
      tags: [],
    }
  },
  computed: {
    placeholder(): string {
      return this.isSearchActive ? 'Search by tag or keyword' : 'Search'
    },
  },
  mounted() {
    const route = this.$route
    this.search = route.query.search?.toString() || this.search
    const tags = route.query.tags ? route.query.tags : []
    this.tags = Array.isArray(tags) ? tags.map((tag) => tag?.toString()).filter(notEmpty) : [tags]

    if (!isEmpty(this.search) || this.tags.length) this.isSearchActive = true
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
    updateQueryParams() {
      this.$router.push({
        path: this.$route.path,
        query: {
          search: !isEmpty(this.search) ? this.search : undefined,
          tags: this.tags ? this.tags : undefined,
        },
      })
    },
  },
  watch: {
    search() {
      this.updateQueryParams()
    },
    tags() {
      this.updateQueryParams()
    },
  },
})
</script>

<style scoped>
.search-box-container {
  @apply flex flex-grow transform flex-col items-center justify-center space-y-6 px-4 py-1;
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
