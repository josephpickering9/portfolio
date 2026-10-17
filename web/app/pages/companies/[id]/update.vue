<template>
  <CompanyForm :id="id" />
</template>

<script setup lang="ts">
import { useRoute } from 'vue-router'
import useMeta from '~/composables/useMeta'
import CompanyForm from '~/components/company/form/CompanyForm.vue'

definePageMeta({ middleware: 'auth' })

const route = useRoute()
const id = String(route.params['id'])

const { updateMeta } = useMeta()
updateMeta({ title: 'Update company' })
</script>
