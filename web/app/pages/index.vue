<template>
  <div
    class="flex flex-grow transform flex-col items-center justify-center gap-6 px-4 py-1 transition-all duration-300 ease-in-out"
    :class="{ 'justify-start': isSearchActive }"
  >
    <TagAutoComplete
      v-model="tags"
      v-model:model-search="search"
      type="text"
      class="z-50 w-full max-w-xs transform transition-all duration-300 ease-in-out"
      :class="{ 'max-w-5xl': isSearchActive }"
      :placeholder="placeholder"
      :show-empty-message="false"
      :open-on-focus="false"
      @focus="handleFocus"
      @blur="handleBlur"
    />
    <div v-if="isSearchActive" class="z-0 flex w-full justify-center">
      <ProjectList :model-search="search" :tags="tags" />
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { isEmpty } from 'lodash-es'
import { useProjectStore } from '../store/ProjectStore'
import ProjectList from '../components/lists/project/ProjectList.vue'
import TagAutoComplete from '../components/forms/tag/TagAutoComplete.vue'
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
    this.setValues()
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
    setValues() {
      const route = this.$route
      this.search = route.query.search?.toString() || this.search
      const tags = route.query.tags ? route.query.tags : []
      this.tags = Array.isArray(tags) ? tags.map((tag) => tag?.toString()).filter(notEmpty) : [tags]

      this.isSearchActive = !isEmpty(this.search) || this.tags.length > 0
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
    tags: {
      handler() {
        this.$nextTick(() => {
          this.updateQueryParams()
        })
      },
      deep: true,
    },
  },
})
</script>
