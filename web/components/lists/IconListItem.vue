<template>
  <div v-if="icon" class="flex w-full items-center gap-2">
    <Icon :name="icon" />
    <span>{{ icon.replace('mdi:', '') }}</span>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { Icon } from '#components'

export default defineComponent({
  name: 'IconListItem',
  components: { Icon },
  props: {
    icon: {
      type: String,
      default: null,
    },
  },
})
</script>
