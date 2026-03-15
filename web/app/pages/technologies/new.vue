<template>
  <div class="flex w-full flex-col items-center px-4">
    <TagForm />
  </div>
</template>

<script setup lang="ts">
import useMeta from '~/composables/useMeta'
import TagForm from '~/components/tag/form/TagForm.vue'

definePageMeta({ middleware: 'auth' })

const { updateMeta } = useMeta()

updateMeta({ title: 'New Tag', description: 'Create a new tag' })
</script>
