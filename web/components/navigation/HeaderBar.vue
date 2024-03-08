<template>
  <header class="navbar bg-base-100">
    <div class="flex-1">
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
    <div class="hidden flex-none gap-2 md:flex">
      <ul class="menu menu-horizontal px-1">
        <li><NuxtLink to="/projects">Projects</NuxtLink></li>
        <li><NuxtLink to="/companies">Companies</NuxtLink></li>
        <li><NuxtLink to="/tags">Tags</NuxtLink></li>
      </ul>
      <ThemeController />
      <div class="dropdown dropdown-end">
        <div tabindex="0" role="button" class="avatar btn btn-circle btn-ghost">
          <div class="w-10 rounded-full">
            <img alt="Profile" src="~/assets/img/joe.png" />
          </div>
        </div>
        <ClientOnly>
          <ul
            tabindex="0"
            class="menu dropdown-content menu-sm z-[1] mt-3 w-52 rounded-box border border-gray-600 bg-base-100 p-2 shadow"
          >
            <li>
              <a
                :href="linkedInUrl"
                target="_blank"
                rel="noopener noreferrer"
                class="btn btn-sm justify-start bg-[#2682BE] text-black"
              >
                <Icon name="mdi:linkedin" size="1.5em" />
                LinkedIn
              </a>
            </li>
            <li>
              <a
                href="/Joseph Pickering CV.pdf"
                download="Joseph-Pickering-CV"
                class="btn btn-sm justify-start bg-[#F84C4D] text-black"
              >
                <Icon name="mdi:file-pdf" size="1.5em" />
                Download CV
              </a>
            </li>
            <li class="w-full"><div class="divider" /></li>
            <li v-if="!isAuthenticated">
              <FormButton type="primary" size="sm" label="Login" @click="login" />
            </li>
            <li v-else>
              <FormButton type="accent" label="Logout" size="sm" @click="logout" />
            </li>
          </ul>
        </ClientOnly>
      </div>
    </div>
    <div class="flex-none md:hidden">
      <button class="btn-square" @click="toggleMobileMenu">
        <Icon v-if="!showMobileMenu" name="mdi:hamburger-menu" size="2em" />
        <Icon v-else name="mdi:close" size="2em" />
      </button>
    </div>

    <div v-if="showMobileMenu" class="mobile-menu md:hidden">
      <div class="flex h-full w-full flex-col items-center p-4">
        <ul class="menu flex w-full flex-col items-center justify-start p-4 py-10 text-xl">
          <li><NuxtLink to="/projects" @click="toggleMobileMenu">Projects</NuxtLink></li>
          <li><NuxtLink to="/companies" @click="toggleMobileMenu">Companies</NuxtLink></li>
          <li><NuxtLink to="/tags" @click="toggleMobileMenu">Tags</NuxtLink></li>
          <li class="w-full"><div class="divider" /></li>
          <li>
            <a
              :href="linkedInUrl"
              target="_blank"
              rel="noopener noreferrer"
              class="btn btn-wide items-center justify-center bg-[#2682BE] text-lg text-black"
            >
              <Icon name="mdi:linkedin" size="1.5em" />
              <span>LinkedIn</span>
            </a>
          </li>
          <li>
            <a
              href="/Joseph Pickering CV.pdf"
              download="Joseph-Pickering-CV"
              class="btn btn-wide mt-4 items-center justify-center bg-[#F84C4D] text-lg text-black"
            >
              <Icon name="mdi:file-pdf" size="1.5em" />
              Download CV
            </a>
          </li>
          <li class="w-full"><div class="divider" /></li>
          <li><ThemeController /></li>
          <ClientOnly>
            <li v-if="!isAuthenticated">
              <FormButton type="primary" size="sm" label="Login" @click="login" />
            </li>
            <li v-else>
              <FormButton type="accent" label="Logout" size="sm" @click="logout" />
            </li>
          </ClientOnly>
        </ul>
        <div class="mt-auto flex flex-col gap-4">
          <div class="w-20 overflow-hidden rounded-full">
            <img alt="Profile" src="~/assets/img/joe.png" />
          </div>
        </div>
      </div>
    </div>
  </header>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import useAuth from '../../composables/useAuth'
import ThemeController from '../theme/ThemeController.vue'
import FormButton from '../forms/elements/FormButton.vue'
import { Icon } from '#components'
import { useRuntimeConfig } from '#app'

interface Data {
  showMobileMenu: boolean
}

export default defineComponent({
  name: 'HeaderBar',
  components: { Icon, ThemeController, FormButton },
  setup() {
    const { isAuthenticated, login, logout } = useAuth()

    return {
      isAuthenticated,
      login,
      logout,
    }
  },
  data(): Data {
    return {
      showMobileMenu: false,
    }
  },
  computed: {
    linkedInUrl(): string {
      return useRuntimeConfig().public.linkedInUrl ?? 'https://www.linkedin.com/in/josephpickering'
    },
  },
  methods: {
    toggleMobileMenu() {
      this.showMobileMenu = !this.showMobileMenu
    },
  },
})
</script>

<style scoped>
.mobile-menu {
  @apply fixed left-0 top-16 z-50 w-full overflow-y-auto bg-base-100;
  height: calc(100% - 116px);
  transition: top 0.3s;
}
</style>
