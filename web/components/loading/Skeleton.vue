<template>
  <div class="flex w-full max-w-3xl flex-col gap-4">
    <div class="flex items-center gap-4">
      <div class="skeleton h-16 w-16 shrink-0 rounded-full"></div>
      <div class="flex flex-col gap-4">
        <div class="skeleton h-4 w-20"></div>
        <div class="skeleton h-4 w-28"></div>
      </div>
    </div>
    <div class="skeleton h-32 w-full"></div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'

export default defineComponent({
  name: 'Skeleton',
})
</script>
