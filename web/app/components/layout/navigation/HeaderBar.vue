<template>
  <header class="navbar bg-base-100">
    <div class="flex-1 flex items-center gap-2">
      <NuxtLink to="/" class="btn btn-ghost text-xl">Joseph Pickering</NuxtLink>
      <ClientOnly>
        <NuxtLink
          v-if="isAuthenticated"
          to="/projects/new"
          class="flex h-6 w-6 items-center justify-center rounded-full border border-dashed border-gray-300"
        >
          <Icon name="mdi:plus" />
        </NuxtLink>
      </ClientOnly>
    </div>
    <div class="hidden flex-none gap-2 md:flex md:items-center">
      <ul class="menu menu-horizontal px-1">
        <li><NuxtLink to="/projects">Projects</NuxtLink></li>
        <li><NuxtLink to="/companies">Companies</NuxtLink></li>
        <li><NuxtLink to="/tags">Tags</NuxtLink></li>
      </ul>
      <ThemeController />
      <UserDropdown
        :linked-in-url="linkedInUrl"
        :is-authenticated="isAuthenticated ?? false"
        :optimising="optimising"
        :optimise-error="optimiseError"
        @login="login"
        @logout="logout"
        @install-pwa="installPwa"
        @optimise-images="optimiseImages"
      />
    </div>
    <div class="flex-none md:hidden">
      <button class="btn-square" @click="toggleMobileMenu">
        <Icon v-if="!showMobileMenu" name="mdi:hamburger-menu" size="2em" />
        <Icon v-else name="mdi:close" size="2em" />
      </button>
    </div>

    <MobileMenu
      v-if="showMobileMenu"
      :linked-in-url="linkedInUrl"
      :is-authenticated="isAuthenticated ?? false"
      :optimising="optimising"
      :optimise-error="optimiseError"
      @close="toggleMobileMenu"
      @login="login"
      @logout="logout"
      @install-pwa="installPwa"
      @optimise-images="optimiseImages"
    />
  </header>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import useAuth from '~/composables/useAuth'
import { useProjectImageStore } from '~/store/ProjectImageStore'
import { useNotificationStore } from '~/store/NotificationStore'
import { NotificationPosition } from '~/types/NotificationPosition'
import ThemeController from '~/components/layout/ThemeController.vue'
import UserDropdown from '~/components/layout/navigation/UserDropdown.vue'
import MobileMenu from '~/components/layout/navigation/MobileMenu.vue'

// Interfaces
interface BeforeInstallPromptEvent extends Event {
  readonly platforms: string[]
  readonly userChoice: Promise<{
    outcome: 'accepted' | 'dismissed'
    platform: string
  }>
  prompt(): Promise<void>
}

const route = useRoute()
const { isAuthenticated, login, logout } = useAuth()
const projectImageStore = useProjectImageStore()
const notificationStore = useNotificationStore()
const runtimeConfig = useRuntimeConfig()

const showMobileMenu = ref(false)
const installPrompt = ref<BeforeInstallPromptEvent | undefined>(undefined)

const linkedInUrl = computed((): string => {
  return runtimeConfig.public.linkedInUrl ?? 'https://www.linkedin.com/in/josephpickering'
})

const optimising = computed((): boolean => {
  return projectImageStore.optimising
})

const optimiseError = computed((): string | undefined => {
  return projectImageStore.optimiseError
})

function toggleMobileMenu() {
  showMobileMenu.value = !showMobileMenu.value
}

async function installPwa() {
  if (installPrompt.value) {
    installPrompt.value.prompt()
    await installPrompt.value.userChoice
    installPrompt.value = undefined
  }
}

async function optimiseImages() {
  await projectImageStore.optimiseImages()

  if (optimiseError.value) {
    notificationStore.displayErrorNotification(optimiseError.value, NotificationPosition.TOP_LEFT)
  } else {
    notificationStore.displaySuccessNotification('Images optimised', NotificationPosition.TOP_LEFT)
  }
}

onMounted(() => {
  window.addEventListener('beforeinstallprompt', (e) => {
    e.preventDefault()
    installPrompt.value = e as BeforeInstallPromptEvent
  })
})


watch(() => route.path, () => {
  showMobileMenu.value = false
})

watch(showMobileMenu, (newValue) => {
  if (newValue) {
    document.body.style.overflow = 'hidden'
  } else {
    document.body.style.overflow = 'auto'
  }
})
</script>
