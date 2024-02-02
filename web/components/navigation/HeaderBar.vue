<template>
  <div class="navbar bg-base-100">
    <div class="flex-1">
      <NuxtLink to="/" class="btn btn-ghost text-xl">Work Experience</NuxtLink>
      <ClientOnly>
        <NuxtLink
          v-if="isAuthenticated"
          to="/projects/new"
          class="flex h-6 w-6 items-center justify-center rounded-full border border-dashed"
        >
          +
        </NuxtLink>
      </ClientOnly>
    </div>
    <div class="flex-none gap-2">
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
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import useAuth from '../../composables/useAuth'

export default defineComponent({
  name: 'HeaderBar',
  setup() {
    const { isAuthenticated, login, logout } = useAuth()

    return {
      isAuthenticated,
      login,
      logout,
    }
  },
  methods: {
    // login() {
    //   this.$auth0?.loginWithRedirect()
    // },
    // logout() {
    //   this.$auth0?.logout({ logoutParams: { returnTo: window.location.origin } })
    // },
  },
})
</script>
