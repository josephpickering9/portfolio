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
      <div v-if="false" class="form-control">
        <!-- TODO: Hook this up -->
        <input type="text" placeholder="Search" class="input input-bordered w-24 md:w-auto" />
      </div>
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
            <li v-if="!isAuthenticated"><button type="button" @click="login">Login</button></li>
            <li v-if="isAuthenticated"><button type="button" @click="logout">Logout</button></li>
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
        <ul class="menu flex w-full flex-col items-center justify-start p-4 py-20 text-xl">
          <li><NuxtLink to="/projects" @click="toggleMobileMenu">Projects</NuxtLink></li>
          <li><NuxtLink to="/companies" @click="toggleMobileMenu">Companies</NuxtLink></li>
          <li><NuxtLink to="/tags" @click="toggleMobileMenu">Tags</NuxtLink></li>
          <li class="w-full"><div class="divider" /></li>
          <ClientOnly>
            <li v-if="!isAuthenticated"><button type="button" @click="login">Login</button></li>
            <li v-else><button type="button" @click="logout">Logout</button></li>
          </ClientOnly>
        </ul>
        <div class="mt-auto flex flex-col gap-4">
          <div class="avatar btn btn-circle btn-ghost">
            <div class="w-32 rounded-full">
              <img alt="Profile" src="~/assets/img/joe.png" />
            </div>
          </div>
        </div>
      </div>
    </div>
  </header>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import useAuth from '../../composables/useAuth'
import { Icon } from '#components'

interface Data {
  showMobileMenu: boolean
}

export default defineComponent({
  name: 'HeaderBar',
  components: { Icon },
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
  height: calc(100% - 64px);
  transition: top 0.3s;
}
</style>
