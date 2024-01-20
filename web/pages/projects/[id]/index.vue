<template>
  <div class="flex flex-col gap-4">
    <h1>Project</h1>

    <NuxtLink :to="`/projects/${$route.params.id}/update`">Edit</NuxtLink>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'

export default defineComponent({
  // eslint-disable-next-line vue/match-component-file-name
  name: 'Project',
})
</script>
