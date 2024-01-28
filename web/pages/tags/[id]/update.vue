<template>
  <div class="flex w-full flex-col items-center space-y-4">
    <TagForm :id="Number($route.params.id)" />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import TagForm from '../../../components/forms/global/TagForm.vue'

export default defineComponent({
  // eslint-disable-next-line vue/match-component-file-name
  name: 'UpdateTag',
  components: {
    TagForm,
  },
})
</script>
