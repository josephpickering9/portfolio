<template>
  <div class="mobile-menu fixed left-0 top-16 z-50 flex w-full flex-col overflow-y-auto bg-base-100 md:hidden">
    <div class="flex flex-1 flex-col items-center px-4">
      <ul class="menu flex w-full flex-col items-center justify-start p-4 pt-0 text-xl">
        <li><ThemeController /></li>
        <li><NuxtLink to="/projects" @click="$emit('close')">Projects</NuxtLink></li>
        <li><NuxtLink to="/companies" @click="$emit('close')">Companies</NuxtLink></li>
        <li><NuxtLink to="/tags" @click="$emit('close')">Tags</NuxtLink></li>
        <li class="w-full"><div class="divider" /></li>
        <li>
          <a
            :href="linkedInUrl"
            target="_blank"
            rel="noopener noreferrer"
            class="btn btn-wide bg-[#2682BE] text-lg text-black"
          >
            <Icon name="mdi:linkedin" size="1.5em" />
            <span>LinkedIn</span>
          </a>
        </li>
        <li>
          <a
            href="/Joseph Pickering CV.pdf"
            download="Joseph-Pickering-CV"
            class="btn btn-wide mt-4 bg-[#F84C4D] text-lg text-black"
          >
            <Icon name="mdi:file-pdf" size="1.5em" />
            Download CV
          </a>
        </li>
        <li>
          <button type="button" class="btn btn-success btn-wide mt-4 text-lg" @click="$emit('installPwa')">
            <Icon name="ic:round-install-desktop" size="1.5em" />
            Install PWA
          </button>
        </li>
        <ClientOnly>
          <li v-if="isAuthenticated">
            <button type="button" class="btn btn-secondary btn-wide mt-4 text-lg" @click="$emit('optimiseImages')">
              <Spinner v-if="optimising" />
              <span v-else>
                <Icon name="mdi:file-image-box" size="1.5em" />
                Optimise Images
              </span>
            </button>
          </li>
          <li class="w-full"><div class="divider" /></li>
          <li v-if="!isAuthenticated">
            <FormButton type="primary" size="sm" label="Login" @click="$emit('login')" />
          </li>
          <li v-else>
            <FormButton type="accent" label="Logout" size="sm" @click="$emit('logout')" />
          </li>
        </ClientOnly>
      </ul>
      <div class="mt-auto flex flex-col gap-4">
        <div class="w-20 overflow-hidden rounded-full">
          <img alt="Profile" src="~/assets/img/joe.png" >
        </div>
      </div>
    </div>
    <FooterBar />
  </div>
</template>

<script setup lang="ts">
import ThemeController from '~/components/layout/ThemeController.vue'
import FormButton from '~/components/ui/form/FormButton.vue'
import Spinner from '~/components/feedback/loading/Spinner.vue'
import FooterBar from '~/components/layout/navigation/FooterBar.vue'

defineProps<{
  linkedInUrl: string
  isAuthenticated: boolean
  optimising: boolean
  optimiseError?: string
}>()

defineEmits<{
  (e: 'close' | 'login' | 'logout' | 'installPwa' | 'optimiseImages'): void
}>()
</script>

<style scoped>
.mobile-menu {
  height: calc(100% - 64px);
  transition: top 0.3s;
}
</style>
