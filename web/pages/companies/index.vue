<template>
  <div class="flex w-full flex-col items-center">
    <CompanyList />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import CompanyList from '../../components/lists/CompanyList.vue'

export default defineComponent({
  // eslint-disable-next-line vue/match-component-file-name
  name: 'Companies',
  components: { CompanyList },
})
</script>
