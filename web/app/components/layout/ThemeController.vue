<template>
  <div class="flex items-center rounded-full border border-line p-0.5" role="group" aria-label="Theme">
    <button
      v-for="theme in THEMES"
      :key="theme"
      type="button"
      :title="theme"
      :aria-label="`${theme} theme`"
      :aria-pressed="theme === themeStore.theme"
      :data-testid="`theme-${theme}`"
      class="grid size-6 place-items-center rounded-full pointer-coarse:size-11"
      @click="themeStore.setSelectedTheme(theme)"
    >
      <span
        class="block size-4.5 overflow-hidden rounded-full"
        :class="theme === themeStore.theme ? 'border-2 border-text' : 'border border-line'"
      >
        <span :data-theme="theme" class="block size-full bg-[linear-gradient(135deg,var(--bg)_50%,var(--accent)_50%)]" />
      </span>
    </button>
  </div>
</template>

<script setup lang="ts">
import { useThemeStore } from '~/store/ThemeStore'
import { Theme } from '~/types/Theme'

const THEMES = Object.values(Theme)

const themeStore = useThemeStore()
</script>
