<template>
  <template v-if="variant === 'menu-item'">
    <button
      type="button"
      class="group flex w-full px-3 cursor-pointer items-center gap-4 py-2 text-lg"
      aria-label="Change Theme"
      @click="isOpen = !isOpen"
    >
      <div
        class="grid flex-shrink-0 grid-cols-2 gap-0.5 rounded-md border border-base-content/10 bg-base-100 p-1 transition-colors group-hover:border-base-content/20"
      >
        <div class="size-2 rounded-full bg-base-content" />
        <div class="size-2 rounded-full bg-primary" />
        <div class="size-2 rounded-full bg-secondary" />
        <div class="size-2 rounded-full bg-accent" />
      </div>
      <span>Theme</span>
      <span v-if="selectedTheme" class="ml-auto mr-1 text-sm font-normal capitalize opacity-50">{{ selectedTheme }}</span>
      <svg
        width="14px"
        height="14px"
        class="fill-current opacity-40 transition-transform duration-200 mr-1"
        :class="{ 'rotate-180': isOpen }"
        xmlns="http://www.w3.org/2000/svg"
        viewBox="0 0 2048 2048"
      >
        <path d="M1799 349l242 241-1017 1017L7 590l242-241 775 775 775-775z" />
      </svg>
    </button>

    <Transition
      enter-active-class="transition-all duration-300 ease-out"
      enter-from-class="opacity-0 -translate-y-2"
      enter-to-class="opacity-100 translate-y-0"
      leave-active-class="transition-all duration-200 ease-in"
      leave-from-class="opacity-100 translate-y-0"
      leave-to-class="opacity-0 -translate-y-2"
    >
      <div v-if="isOpen" class="mt-3 overflow-hidden rounded-xl border border-base-content/10 bg-base-200 px-0">
        <div class="grid grid-cols-4 gap-1 p-2">
          <button
            v-for="theme in themes"
            :key="theme"
            type="button"
            class="flex flex-col items-center gap-1.5 rounded-lg p-2 text-center transition-colors"
            :class="selectedTheme === theme
              ? 'bg-primary/15 ring-1 ring-primary/40'
              : 'hover:bg-base-content/5 active:bg-base-content/10'"
            @click="changeTheme(theme)"
          >
            <div
              :data-theme="theme"
              class="grid shrink-0 grid-cols-2 gap-0.5 rounded-md bg-base-100 p-1 shadow-sm"
            >
              <div class="size-2 rounded-full bg-base-content" />
              <div class="size-2 rounded-full bg-primary" />
              <div class="size-2 rounded-full bg-secondary" />
              <div class="size-2 rounded-full bg-accent" />
            </div>
            <span class="w-full truncate text-[10px] leading-tight capitalize opacity-70">{{ theme }}</span>
          </button>
        </div>
      </div>
    </Transition>
  </template>

  <div v-else class="dropdown dropdown-end">
    <div
      tabindex="0"
      role="button"
      class="btn btn-ghost group gap-1.5 px-1.5"
      aria-label="Change Theme"
    >
      <div
        class="grid flex-shrink-0 grid-cols-2 gap-0.5 rounded-md border border-base-content/10 bg-base-100 p-1 transition-colors group-hover:border-base-content/20"
      >
        <div class="size-1.5 rounded-full bg-base-content" />
        <div class="size-1.5 rounded-full bg-primary" />
        <div class="size-1.5 rounded-full bg-secondary" />
        <div class="size-1.5 rounded-full bg-accent" />
      </div>
      Theme
      <svg
        width="12px"
        height="12px"
        class="mt-px hidden size-2 fill-current opacity-60 sm:inline-block"
        xmlns="http://www.w3.org/2000/svg"
        viewBox="0 0 2048 2048"
      >
        <path d="M1799 349l242 241-1017 1017L7 590l242-241 775 775 775-775z" />
      </svg>
    </div>
    <div
      tabindex="0"
      class="dropdown-content z-50 h-[30.5rem] max-h-[calc(100vh-8.6rem)] w-56 overflow-y-auto rounded-box border border-white/5 bg-base-200 text-base-content shadow-2xl outline outline-1 outline-black/5"
    >
      <ul class="menu w-56">
        <li class="menu-title text-xs">Theme</li>
        <li v-for="theme in themes" :key="theme">
          <button
            class="gap-3 px-2"
            :class="{ '[&_svg]:visible': selectedTheme === theme }"
            @click="changeTheme(theme)"
          >
            <div
              :data-theme="theme"
              class="grid shrink-0 grid-cols-2 gap-0.5 rounded-md bg-base-100 p-1 shadow-sm"
            >
              <div class="size-1 rounded-full bg-base-content" />
              <div class="size-1 rounded-full bg-primary" />
              <div class="size-1 rounded-full bg-secondary" />
              <div class="size-1 rounded-full bg-accent" />
            </div>
            <div class="w-32 truncate">{{ theme }}</div>
            <svg
              xmlns="http://www.w3.org/2000/svg"
              width="16"
              height="16"
              viewBox="0 0 24 24"
              fill="currentColor"
              class="invisible h-3 w-3 shrink-0"
            >
              <path d="M20.285 2l-11.285 11.567-5.286-5.011-3.714 3.716 9 8.728 15-15.285z" />
            </svg>
          </button>
        </li>
      </ul>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue'
import { useThemeStore } from '~/store/ThemeStore'

const props = withDefaults(defineProps<{
  variant?: 'button' | 'menu-item'
}>(), { variant: 'button' })

const isOpen = ref(false)

const themes = [
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
]

const themeStore = useThemeStore()
const selectedTheme = computed(() => themeStore.selectedTheme)

const changeTheme = (theme: string) => {
  themeStore.selectedTheme = theme
  if (import.meta.client) {
    document.documentElement.setAttribute('data-theme', theme)
  }
}

onMounted(() => {
  if (import.meta.client) {
    document.documentElement.setAttribute('data-theme', selectedTheme.value)
  }
})
</script>
