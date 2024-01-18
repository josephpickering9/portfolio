<template>
    <div class="flex flex-grow justify-center items-center">
        <input type="text" v-model="search" placeholder="Search" class="input input-bordered w-full max-w-xs" />
    </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { useProjectStore } from '../store/ProjectStore'

export default defineComponent({
  name: 'App',
  data() {
    return {
      search: ''
    }
  },
  methods: {
    async getProjects() {
      await useProjectStore().getProjects(this.search)
    }
  },
  watch: {
    async search() {
      await this.getProjects()
    }
  }
})
</script>