<template>
  <NuxtLayout>
    <div class="relative mx-auto flex min-h-[70vh] w-full max-w-300 flex-col justify-center gap-6 px-7 py-24">
      <div
        class="pointer-events-none absolute inset-x-0 top-0 left-[30%] h-120 bg-[radial-gradient(closest-side,var(--glow),transparent)]"
        aria-hidden="true"
      />
      <span class="relative font-mono text-[13px] tracking-[0.06em] text-accent uppercase">Error {{ error.statusCode }}</span>
      <h1 class="relative m-0 font-display text-[clamp(56px,10vw,136px)] leading-[0.9] font-normal tracking-tight">
        <template v-if="isNotFound">Page not <span class="text-accent">found</span></template>
        <template v-else>Something went <span class="text-accent">wrong</span></template>
      </h1>
      <p class="relative m-0 max-w-140 text-[19px] leading-[1.55] text-muted">
        {{ isNotFound ? "The page you're looking for doesn't exist or has moved." : 'An unexpected error occurred. Try again in a moment.' }}
      </p>
      <div class="relative flex flex-wrap gap-3">
        <button
          type="button"
          class="flex items-center gap-2 rounded-full bg-accent px-5 py-3 text-[15px] font-medium text-accent-ink"
          @click="goTo('/')"
        >
          Back to home
        </button>
        <button
          type="button"
          class="flex items-center gap-2 rounded-full border border-line px-5 py-3 text-[15px] font-medium text-text"
          @click="goTo('/projects')"
        >
          All projects
        </button>
        <button
          type="button"
          class="flex items-center gap-2 rounded-full border border-line px-5 py-3 text-[15px] font-medium text-text"
          @click="goTo('/search')"
        >
          <Icon name="mdi:robot-happy-outline" />
          Ask AI Search
        </button>
      </div>
    </div>
  </NuxtLayout>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { NuxtError } from '#app'
import { useThemeStore } from '~/store/ThemeStore'

const HTTP_NOT_FOUND = 404

const props = defineProps<{
  error: NuxtError
}>()

const themeStore = useThemeStore()
const isNotFound = computed(() => props.error.statusCode === HTTP_NOT_FOUND)

useHead({
  title: computed(() => (isNotFound.value ? 'Page not found | Joseph Pickering' : 'Error | Joseph Pickering')),
  htmlAttrs: { 'data-theme': computed(() => themeStore.theme) },
})

function goTo(path: string) {
  clearError({ redirect: path })
}
</script>
