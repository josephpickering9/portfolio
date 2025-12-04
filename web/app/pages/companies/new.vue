<template>
  <div class="flex w-full flex-col items-center px-4">
    <CompanyForm />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import useMeta from '../../composables/useMeta'
import CompanyForm from '../../components/forms/company/CompanyForm.vue'

export default defineComponent({
  // eslint-disable-next-line vue/match-component-file-name
  name: 'NewCompany',
  components: { CompanyForm },
  setup() {
    const { updateMeta } = useMeta()

    updateMeta({ title: 'New Company', description: 'Create a new company' })

    // eslint-disable-next-line no-undef
    definePageMeta({ middleware: 'auth' })
  },
})
</script>
