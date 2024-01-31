<template>
  <div class="flex w-full flex-col items-center">
    <CompanyList />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import useMeta from '../../composables/useMeta'
import CompanyList from '../../components/lists/CompanyList.vue'

export default defineComponent({
  // eslint-disable-next-line vue/match-component-file-name
  name: 'Companies',
  components: { CompanyList },
  setup() {
    const { updateMeta } = useMeta()

    updateMeta({ title: 'Companies', description: "View all of the companies that I've done work for" })
  },
})
</script>
