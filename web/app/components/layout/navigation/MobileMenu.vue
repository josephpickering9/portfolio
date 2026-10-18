<template>
  <nav id="mobile-menu" data-testid="mobile-menu" class="flex flex-col border-t border-line px-7 pt-2 pb-6 min-[860px]:hidden" aria-label="Main">
    <NuxtLink
      v-for="item in navItems"
      :key="item.to"
      :to="item.to"
      class="border-b border-line py-3 font-display text-[30px]"
      :class="isActive(item.to) ? 'text-accent' : 'text-text'"
      :aria-current="isActive(item.to) ? 'page' : undefined"
    >
      {{ item.label }}
    </NuxtLink>
    <div class="flex items-center justify-between gap-3 pt-5">
      <ThemeController />
      <CvButton />
    </div>
    <ClientOnly>
      <div v-if="isAuthenticated" class="flex flex-wrap gap-2 pt-5">
        <NuxtLink to="/projects/new" class="rounded-full border border-line px-4 py-2.25 text-sm">New project</NuxtLink>
        <button type="button" class="rounded-full border border-line px-4 py-2.25 text-sm text-danger" @click="logout">
          Log out
        </button>
      </div>
    </ClientOnly>
  </nav>
</template>

<script setup lang="ts">
import useAuth from '~/composables/useAuth'
import { navItems } from '~/data/NavItems'
import ThemeController from '~/components/layout/ThemeController.vue'
import CvButton from '~/components/layout/navigation/CvButton.vue'

interface Props {
  isActive: (to: string) => boolean
}

defineProps<Props>()

const { isAuthenticated, logout } = useAuth()
</script>
