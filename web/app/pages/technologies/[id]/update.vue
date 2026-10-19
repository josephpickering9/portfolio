<template>
  <TagForm :id="id" />
</template>

<script setup lang="ts">
import { useRoute } from 'vue-router'
import { definePageMeta } from '#imports'
import useMeta from '~/composables/useMeta'
import TagForm from '~/components/tag/form/TagForm.vue'

definePageMeta({ middleware: 'auth' })

const route = useRoute()
const id = String(route.params['id'])

const { updateMeta } = useMeta()
updateMeta({ title: 'Update technology' })
</script>
