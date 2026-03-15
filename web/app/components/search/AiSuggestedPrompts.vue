<template>
  <div
    class="mt-8 transition-all duration-500 ease-in-out"
    :class="[hasSearched ? 'opacity-0 h-0 overflow-hidden mt-0' : 'opacity-100']"
  >
    <p v-if="!loading" class="text-xs text-center text-base-content/30 font-semibold uppercase tracking-widest mb-5">Try asking</p>
    <div class="flex flex-wrap justify-center gap-2 md:gap-3">
      <button
        v-for="(prompt, index) in prompts"
        :key="prompt.text"
        class="prompt-btn animate-slide-up group"
        :style="{
          '--brand': prompt.color ?? '#888888',
          animationDelay: `${index * 100}ms`
        }"
        @click="$emit('select', prompt.text)"
      >
        <span class="icon-wrap">
          <span class="inline-flex transition-transform duration-300 group-hover:scale-[1.25] group-hover:-rotate-6">
            <Icon
              :name="prompt.icon"
              class="w-4 h-4 transition-colors duration-300"
              :style="prompt.color ? { color: prompt.color } : { color: 'oklch(var(--bc) / 0.5)' }"
            />
          </span>
        </span>
        <span class="text-sm text-base-content/70 group-hover:text-base-content transition-colors duration-300">{{ prompt.text }}</span>
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  prompts: Array<{ text: string, icon: string, color?: string }>
  loading: boolean
  hasSearched: boolean
}>()

defineEmits<{
  (e: 'select', text: string): void
}>()
</script>

<style scoped>
@keyframes slideUp {
  from { opacity: 0; transform: translateY(16px); }
  to { opacity: 1; transform: translateY(0); }
}

.animate-slide-up {
  animation: slideUp 0.4s ease-out forwards;
  opacity: 0;
}

.prompt-btn {
  display: inline-flex;
  align-items: center;
  gap: 0.625rem;
  padding: 0.5rem 1rem 0.5rem 0.625rem;
  border-radius: 0.875rem;
  border: 1px solid color-mix(in srgb, var(--brand) 20%, transparent);
  background: color-mix(in srgb, var(--brand) 4%, oklch(var(--b1)));
  transition: all 0.25s ease;
  cursor: pointer;
  text-align: left;
  font-weight: 400;
}

.prompt-btn:hover {
  border-color: color-mix(in srgb, var(--brand) 45%, transparent);
  background: color-mix(in srgb, var(--brand) 9%, oklch(var(--b1)));
  transform: translateY(-1px);
  box-shadow: 0 4px 16px color-mix(in srgb, var(--brand) 15%, transparent);
}

.icon-wrap {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 2rem;
  height: 2rem;
  border-radius: 0.5rem;
  background: color-mix(in srgb, var(--brand) 14%, oklch(var(--b2)));
  transition: background 0.25s ease;
  flex-shrink: 0;
}

.prompt-btn:hover .icon-wrap {
  background: color-mix(in srgb, var(--brand) 24%, oklch(var(--b2)));
}
</style>
