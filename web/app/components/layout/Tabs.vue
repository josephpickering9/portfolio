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
      >

      <div role="tabpanel" class="tab-content w-full overflow-x-hidden rounded-box border-gray-600 p-6">
        <slot :index="index" />
      </div>
    </template>
  </div>
</template>

<script setup lang="ts">
interface Props {
  tabs?: string[]
  activeTab?: number
}

withDefaults(defineProps<Props>(), {
  tabs: () => [],
  activeTab: 0,
})


const emit = defineEmits<{
  'update:activeTab': [value: number]
}>()

function setActiveTab(index: number) {
  emit('update:activeTab', index)
}
</script>
