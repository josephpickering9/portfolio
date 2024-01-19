<template>
  <div class="navbar bg-base-100">
    <div class="flex-1">
      <NuxtLink to="/" class="btn btn-ghost text-xl">Work Experience</NuxtLink>
      <NuxtLink to="/projects/new" class="flex h-6 w-6 items-center justify-center rounded-full border border-dashed">
        +
      </NuxtLink>
    </div>
    <div class="flex-none gap-2">
      <div v-if="$route.path !== '/'" class="form-control">
        <input type="text" placeholder="Search" class="input input-bordered w-24 md:w-auto" />
      </div>
      <div class="dropdown dropdown-end">
        <div tabindex="0" role="button" class="avatar btn btn-circle btn-ghost">
          <div class="w-10 rounded-full">
            <img alt="Profile" src="~/assets/img/joe.png" />
          </div>
        </div>
        <ul tabindex="0" class="menu dropdown-content menu-sm z-[1] mt-3 w-52 rounded-box border bg-base-100 p-2 shadow">
          <li><a>Profile</a></li>
          <li><a>Settings</a></li>
          <li><a>Logout</a></li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'

export default defineComponent({
  name: 'HeaderBar',
})
</script>
