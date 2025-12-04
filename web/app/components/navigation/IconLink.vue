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

<script setup lang="ts">

interface Props {
  to: string
  label: string
  icon?: string | null
  iconSize?: string
  target?: string
}

withDefaults(defineProps<Props>(), {
  icon: null,
  iconSize: '1em',
  target: '_blank',
})
</script>
