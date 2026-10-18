<template>
  <button
    type="button"
    class="flex items-center gap-1.75 rounded-full border px-3.5 py-2 text-[13px] transition-colors duration-200"
    :class="active ? 'border-accent bg-accent text-accent-ink' : 'border-line text-muted hover:text-text'"
    :aria-pressed="active"
    @click="emit('select')"
  >
    <Icon v-if="icon" :name="icon" />
    {{ label }}
  </button>
</template>

<script setup lang="ts">
interface Props {
  label: string
  icon?: string
  active?: boolean
}

withDefaults(defineProps<Props>(), {
  icon: undefined,
  active: false,
})

const emit = defineEmits<{ (e: 'select'): void }>()
</script>
