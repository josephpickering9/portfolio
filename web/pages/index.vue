<template>
    <div class="flex flex-grow justify-center items-center">
        <input type="text" v-model="search" placeholder="Search" class="input input-bordered w-full max-w-xs" />
    </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'

export default defineComponent({
  name: 'App',
  data() {
    return {
      search: ''
    }
  }
})
</script>