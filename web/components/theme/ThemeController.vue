<template>
  <div class="dropdown dropdown-end">
    <div tabindex="0" role="button" class="btn m-1">
      Theme
      <Icon name="mdi:chevron-down" />
    </div>
    <div
      class="dropdown-content top-px z-50 mt-16 h-[28.6rem] max-h-[calc(100vh-10rem)] w-56 overflow-y-auto rounded-box border border-white/5 text-base-content shadow-2xl outline outline-1 outline-black/5"
    >
      <div class="grid grid-cols-1 gap-3 p-3">
        <button
          v-for="(theme, index) in themes"
          :key="index"
          class="text-start outline-offset-4 outline-base-content"
          :data-set-theme="theme"
          @click="setSelectedTheme(theme)"
        >
          <span :data-theme="theme" class="block w-full cursor-pointer rounded-btn bg-base-100 font-sans text-base-content">
            <span class="grid grid-cols-5 grid-rows-3">
              <span class="col-span-5 row-span-3 row-start-1 flex items-center gap-2 px-4 py-3">
                <svg
                  v-if="theme === selectedTheme"
                  xmlns="http://www.w3.org/2000/svg"
                  width="16"
                  height="16"
                  viewBox="0 0 24 24"
                  fill="currentColor"
                  class="h-3 w-3 shrink-0"
                >
                  <path d="M20.285 2l-11.285 11.567-5.286-5.011-3.714 3.716 9 8.728 15-15.285z"></path>
                </svg>
                <span class="flex-grow text-sm">{{ theme }}</span>
                <span class="flex h-full shrink-0 flex-wrap gap-1">
                  <span class="w-2 rounded-badge bg-primary"></span>
                  <span class="w-2 rounded-badge bg-secondary"></span>
                  <span class="w-2 rounded-badge bg-accent"></span>
                  <span class="w-2 rounded-badge bg-neutral"></span>
                </span>
              </span>
            </span>
          </span>
        </button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { toTitleCase } from '../../utils/string-helper'
import { useThemeStore } from '../../store/ThemeStore'
import { Icon } from '#components'

interface Data {
  themes: string[]
}

export default defineComponent({
  name: 'ThemeController',
  components: { Icon },
  data(): Data {
    return {
      themes: [
        'light',
        'dark',
        'cupcake',
        'bumblebee',
        'emerald',
        'corporate',
        'synthwave',
        'retro',
        'cyberpunk',
        'valentine',
        'halloween',
        'garden',
        'forest',
        'aqua',
        'lofi',
        'pastel',
        'fantasy',
        'wireframe',
        'black',
        'luxury',
        'dracula',
        'cmyk',
        'autumn',
        'business',
        'acid',
        'lemonade',
        'night',
        'coffee',
        'winter',
        'dim',
        'nord',
        'sunset',
      ],
    }
  },
  computed: {
    selectedTheme(): string {
      return useThemeStore().selectedTheme
    },
  },
  methods: {
    toTitleCase,
    setSelectedTheme(theme: string) {
      useThemeStore().setSelectedTheme(theme)
    },
  },
})
</script>
