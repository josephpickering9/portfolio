<template>
  <input v-model="search" type="text" placeholder="Search" class="input input-bordered w-full max-w-xs" />
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { useProjectStore } from '../store/ProjectStore'

export default defineComponent({
  name: 'Index',
  data() {
    return {
      search: '',
    }
  },
  methods: {
    async getProjects() {
      await useProjectStore().getProjects(this.search)
    },
  },
  watch: {
    async search() {
      await this.getProjects()
    },
  },
})
</script>
