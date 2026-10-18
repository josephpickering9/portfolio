<template>
  <CompanyForm />
</template>

<script setup lang="ts">
import useMeta from '~/composables/useMeta'
import CompanyForm from '~/components/company/form/CompanyForm.vue'

definePageMeta({ middleware: 'auth' })

const { updateMeta } = useMeta()
updateMeta({ title: 'New company' })
</script>
