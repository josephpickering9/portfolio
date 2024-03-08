<template>
  <div class="tabs tabs-lifted">
    <template v-for="(tab, index) in tabs" :key="index">
      <input
        type="radio"
        role="tab"
        class="tab [--tab-border-color:#4B5563]"
        :class="{ 'tab-active': index === activeTab }"
        :aria-label="tab"
        :checked="index === activeTab"
        @click="setActiveTab(index)"
      />

      <div role="tabpanel" class="tab-content w-full overflow-x-hidden rounded-box border-gray-600 p-6">
        <slot :index="index" />
      </div>
    </template>
  </div>
</template>

<script lang="ts">
import { defineComponent, type PropType } from 'vue'

export default defineComponent({
  name: 'Tabs',
  props: {
    tabs: {
      type: Array as PropType<string[]>,
      default: () => [],
    },
    activeTab: {
      type: Number,
      default: 0,
    },
  },
  emits: ['update:activeTab'],
  methods: {
    setActiveTab(index: number) {
      this.$emit('update:activeTab', index)
    },
  },
})
</script>
