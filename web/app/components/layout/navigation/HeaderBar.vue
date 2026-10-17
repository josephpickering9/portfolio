<template>
  <header class="sticky top-0 z-20 border-b border-line bg-[color-mix(in_oklch,var(--bg)_78%,transparent)] backdrop-blur-[14px]">
    <div class="mx-auto flex max-w-300 items-center gap-6 px-7 py-4">
      <NuxtLink to="/" class="font-display text-[26px] tracking-[-0.01em] text-text">Joseph Pickering</NuxtLink>

      <nav class="hidden flex-1 flex-wrap gap-6 text-sm min-[860px]:flex" aria-label="Main">
        <NuxtLink
          v-for="item in navItems"
          :key="item.to"
          :to="item.to"
          class="border-b pb-0.5"
          :class="isActive(item.to) ? 'border-accent text-text' : 'border-transparent text-muted'"
          :aria-current="isActive(item.to) ? 'page' : undefined"
        >
          {{ item.label }}
        </NuxtLink>
      </nav>

      <div class="hidden items-center gap-3 min-[860px]:flex">
        <ThemeController />
        <CvButton />
        <ClientOnly>
          <AdminMenu v-if="isAuthenticated" @logout="logout?.()" />
        </ClientOnly>
      </div>

      <button
        type="button"
        class="ml-auto flex size-11 items-center justify-center rounded-full border border-line text-[22px] text-text min-[860px]:hidden"
        :aria-label="menuOpen ? 'Close navigation menu' : 'Open navigation menu'"
        :aria-expanded="menuOpen"
        aria-controls="mobile-menu"
        @click="menuOpen = !menuOpen"
      >
        <Icon :name="menuOpen ? 'mdi:close' : 'mdi:menu'" />
      </button>
    </div>

    <MobileMenu v-if="menuOpen" :is-active="isActive" />
  </header>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import useAuth from '~/composables/useAuth'
import { navItems } from '~/data/NavItems'
import ThemeController from '~/components/layout/ThemeController.vue'
import CvButton from '~/components/layout/navigation/CvButton.vue'
import AdminMenu from '~/components/layout/navigation/AdminMenu.vue'
import MobileMenu from '~/components/layout/navigation/MobileMenu.vue'

const route = useRoute()
const { isAuthenticated, logout } = useAuth()

const menuOpen = ref(false)

function isActive(to: string): boolean {
  return route.path === to || route.path.startsWith(`${to}/`)
}

watch(
  () => route.fullPath,
  () => {
    menuOpen.value = false
  },
)
</script>
