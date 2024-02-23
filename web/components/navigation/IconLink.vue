<template>
  <div class="flex items-center gap-x-1">
    <div class="flex items-center gap-x-2">
      <Icon v-if="icon" :name="icon" :size="iconSize" />
      <a :href="to" :target="target" :rel="target === '_blank' ? 'noreferrer' : ''">
        {{ label }}
      </a>
    </div>
    <!-- <Icon name="mdi:launch" /> -->
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { Icon } from '#components'

export default defineComponent({
  name: 'IconLink',
  components: { Icon },
  props: {
    to: {
      type: String,
      required: true,
    },
    label: {
      type: String,
      required: true,
    },
    icon: {
      type: String,
      default: null,
    },
    iconSize: {
      type: String,
      default: '1em',
    },
    target: {
      type: String,
      default: '_blank',
    },
  },
})
</script>
