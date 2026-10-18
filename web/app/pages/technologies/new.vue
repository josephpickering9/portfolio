<template>
  <TagForm />
</template>

<script setup lang="ts">
import useMeta from '~/composables/useMeta'
import TagForm from '~/components/tag/form/TagForm.vue'

definePageMeta({ middleware: 'auth' })

const { updateMeta } = useMeta()
updateMeta({ title: 'New technology' })
</script>
