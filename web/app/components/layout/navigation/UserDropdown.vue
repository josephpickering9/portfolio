<template>
  <div class="dropdown dropdown-end">
    <div tabindex="0" role="button" class="avatar btn btn-circle btn-ghost">
      <div class="w-10 rounded-full">
        <img alt="Profile" src="~/assets/img/joe.png" >
      </div>
    </div>
    <ClientOnly>
      <ul
        tabindex="0"
        class="menu dropdown-content menu-sm z-[1] w-52 gap-1 rounded-box border border-gray-600 bg-base-100 p-2 shadow"
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
        <li>
          <button type="button" class="btn btn-success btn-sm justify-start" @click="$emit('installPwa')">
            <Icon name="ic:round-install-desktop" size="1.5em" />
            Install PWA
          </button>
        </li>
        <li v-if="isAuthenticated">
          <button type="button" class="btn btn-secondary btn-sm justify-start" @click="$emit('optimiseImages')">
            <Spinner v-if="optimising" />
            <span v-else>
              <Icon name="mdi:file-image-box" size="1.5em" />
              Optimise Images
            </span>
          </button>
        </li>
        <li class="w-full"><div class="divider m-0 flex items-center" /></li>
        <li v-if="!isAuthenticated">
          <FormButton type="primary" size="sm" label="Login" @click="$emit('login')" />
        </li>
        <li v-else>
          <FormButton type="accent" label="Logout" size="sm" @click="$emit('logout')" />
        </li>
      </ul>
    </ClientOnly>
  </div>
</template>

<script setup lang="ts">
import FormButton from '~/components/ui/form/FormButton.vue'
import Spinner from '~/components/feedback/loading/Spinner.vue'

defineProps<{
  linkedInUrl: string
  isAuthenticated: boolean
  optimising: boolean
  optimiseError?: string
}>()

defineEmits<{
  (e: 'login' | 'logout' | 'installPwa' | 'optimiseImages'): void
}>()
</script>
