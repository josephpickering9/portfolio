<template>
  <div class="flex w-full flex-col items-center space-y-4">
    <CompanyForm :id="Number($route.params.id)" />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import useMeta from '../../../composables/useMeta'
import CompanyForm from '../../../components/forms/global/CompanyForm.vue'

export default defineComponent({
  // eslint-disable-next-line vue/match-component-file-name
  name: 'UpdateCompany',
  components: {
    CompanyForm,
  },
  setup() {
    const { updateMeta } = useMeta()

    updateMeta({ title: 'Update Company', description: 'Update an existing company' })
  },
})
</script>
