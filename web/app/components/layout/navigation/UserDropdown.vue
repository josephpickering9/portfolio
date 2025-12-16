<template>
  <div class="dropdown dropdown-end">
    <div tabindex="0" role="button" class="avatar btn btn-circle btn-ghost">
      <div class="w-10 rounded-full">
        <NuxtImg alt="Profile" src="/joe.png" placeholder format="webp" />
      </div>
    </div>
    <ClientOnly>
      <ul tabindex="0" class="menu dropdown-content menu-sm z-[1] mt-3 w-52 rounded-box bg-base-100 p-2 shadow-2xl ring-1 ring-base-content/10">
        <li v-if="isAuthenticated">
          <div class="flex flex-col items-start gap-1 p-2">
            <span class="font-bold">Signed in as</span>
            <span class="text-xs text-base-content/70 truncate w-full">{{ user?.email }}</span>
          </div>
        </li>
        <li v-if="isAuthenticated" class="mt-1 border-t border-base-200"/>
        
        <li v-if="!isAuthenticated">
          <a @click="$emit('login')">
            <Icon name="mdi:login" size="1.2em" />
            Login
          </a>
        </li>
        <li v-else>
          <a @click="$emit('logout')">
            <Icon name="mdi:logout" size="1.2em" />
            Logout
          </a>
        </li>
        
        <li class="mt-1 border-t border-base-200"/>
        
        <li>
          <a :href="linkedInUrl" target="_blank" class="justify-between">
            <div class="flex items-center gap-2">
              <Icon name="mdi:linkedin" size="1.2em" />
              LinkedIn
            </div>
            <Icon name="mdi:external-link" size="0.8em" class="opacity-50" />
          </a>
        </li>
        <li>
          <a @click="$emit('installPwa')">
            <Icon name="ic:round-install-desktop" size="1.2em" />
            Install App
          </a>
        </li>
        <li v-if="isAuthenticated">
          <a class="justify-between" @click="$emit('optimiseImages')">
             <div class="flex items-center gap-2">
               <Icon name="mdi:file-image-box" size="1.2em" />
               Optimise Images
             </div>
             <Spinner v-if="optimising" size="xs" />
          </a>
        </li>
      </ul>
    </ClientOnly>
  </div>
</template>

<script setup lang="ts">
import Spinner from '~/components/feedback/loading/Spinner.vue'

defineProps<{
  linkedInUrl: string
  isAuthenticated: boolean
  optimising: boolean
  optimiseError?: string
  user?: { email: string }
}>()

defineEmits<{
  (e: 'login' | 'logout' | 'installPwa' | 'optimiseImages'): void
}>()
</script>
