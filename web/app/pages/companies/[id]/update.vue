<template>
  <div class="flex w-full flex-col items-center px-4">
    <CompanyForm :id="Number($route.params.id)" />
  </div>
</template>

<script setup lang="ts">
import useMeta from '~/composables/useMeta'
import CompanyForm from '~/components/company/form/CompanyForm.vue'

definePageMeta({ middleware: 'auth' })

const { updateMeta } = useMeta()

updateMeta({ title: 'Update Company', description: 'Update an existing company' })
</script>
