<template>
  <div class="flex w-full flex-col items-center px-4">
    <TagForm :id="Number($route.params.id)" />
  </div>
</template>

<script setup lang="ts">
import useMeta from '~/composables/useMeta'
import TagForm from '~/components/tag/form/TagForm.vue'

definePageMeta({ middleware: 'auth' })

const { updateMeta } = useMeta()

updateMeta({ title: 'Update Tag', description: 'Update an existing tag' })
</script>
