<template>
  <div class="mobile-menu fixed left-0 top-16 z-50 flex w-full flex-col overflow-y-auto bg-base-100/95 backdrop-blur-md lg:hidden animate-slide-in-right">
    <div class="flex flex-1 flex-col items-center p-4">
      <ul class="menu flex w-full flex-col items-stretch justify-start gap-2 p-0 text-lg">
        <li class="animate-fade-in-up" style="animation-delay: 100ms">
          <ThemeController variant="menu-item" />
        </li>
        
        <div class="divider my-4 animate-fade-in-up" style="animation-delay: 150ms" />

        <li class="animate-fade-in-up" style="animation-delay: 200ms">
          <NuxtLink to="/search" class="active:bg-primary/10 active:text-primary flex items-center gap-4" @click="$emit('close')">
            <Icon name="mdi:robot-happy-outline" size="1.5em" class="text-primary" />
            <span>AI Search</span>
          </NuxtLink>
        </li>
        <li class="animate-fade-in-up" style="animation-delay: 250ms">
          <NuxtLink to="/projects" class="active:bg-primary/10 active:text-primary flex items-center gap-4" @click="$emit('close')">
            <Icon name="mdi:view-dashboard-outline" size="1.5em" class="text-secondary" />
            <span>Projects</span>
          </NuxtLink>
        </li>
        <li class="animate-fade-in-up" style="animation-delay: 300ms">
          <NuxtLink to="/companies" class="active:bg-primary/10 active:text-primary flex items-center gap-4" @click="$emit('close')">
            <Icon name="mdi:domain" size="1.5em" class="text-accent" />
            <span>Companies</span>
          </NuxtLink>
        </li>
        <li class="animate-fade-in-up" style="animation-delay: 350ms">
          <NuxtLink to="/technologies" class="active:bg-primary/10 active:text-primary flex items-center gap-4" @click="$emit('close')">
            <Icon name="heroicons:code-bracket" size="1.5em" class="text-info" />
            <span>Technologies</span>
          </NuxtLink>
        </li>

        <div class="divider my-4 animate-fade-in-up" style="animation-delay: 400ms" />

        <li class="animate-fade-in-up" style="animation-delay: 450ms">
          <a
            :href="linkedInUrl"
            target="_blank"
            rel="noopener noreferrer"
            class="btn btn-outline border-base-content/10 hover:bg-[#0077b5] hover:border-[#0077b5] hover:text-white justify-start gap-4 normal-case text-base font-normal"
          >
            <Icon name="mdi:linkedin" size="1.5em" class="text-[#0077b5] group-hover:text-white" />
            <span>LinkedIn</span>
            <Icon name="heroicons:arrow-top-right-on-square" class="ml-auto w-4 h-4 opacity-50" />
          </a>
        </li>
        <li class="animate-fade-in-up" style="animation-delay: 500ms">
          <a
            href="/Joseph Pickering CV.pdf"
            download="Joseph-Pickering-CV"
            class="btn btn-outline border-base-content/10 hover:bg-[#FF5722] hover:border-[#FF5722] hover:text-white justify-start gap-4 normal-case text-base font-normal mt-2"
          >
            <Icon name="mdi:file-pdf" size="1.5em" class="text-[#FF5722] group-hover:text-white" />
            Download CV
            <Icon name="heroicons:arrow-down-tray" class="ml-auto w-4 h-4 opacity-50" />
          </a>
        </li>
        
        <li class="animate-fade-in-up" style="animation-delay: 550ms">
          <button type="button" class="btn btn-ghost bg-base-200 hover:bg-success hover:text-white justify-start gap-4 normal-case text-base font-normal mt-2" @click="$emit('installPwa')">
            <Icon name="ic:round-install-desktop" size="1.5em" class="text-success group-hover:text-white" />
            Install PWA
          </button>
        </li>

        <ClientOnly>
          <div class="divider my-4 animate-fade-in-up" style="animation-delay: 600ms" />
          
          <li v-if="isAuthenticated" class="animate-fade-in-up" style="animation-delay: 650ms">
            <button type="button" class="btn btn-ghost bg-base-200 justify-start gap-4 normal-case text-base font-normal" @click="$emit('optimiseImages')">
              <Spinner v-if="optimising" />
              <template v-else>
                <Icon name="mdi:file-image-box" size="1.5em" />
                Optimise Images
              </template>
            </button>
          </li>
          
          <li v-if="!isAuthenticated" class="animate-fade-in-up" style="animation-delay: 650ms">
            <FormButton type="primary" class="w-full" label="Login" @click="$emit('login')" />
          </li>
          <li v-else class="animate-fade-in-up" style="animation-delay: 700ms">
            <FormButton type="error" outline class="w-full mt-2" label="Logout" @click="$emit('logout')" />
          </li>
        </ClientOnly>
      </ul>
    </div>
  </div>
</template>

<script setup lang="ts">
import ThemeController from '~/components/layout/ThemeController.vue'
import FormButton from '~/components/ui/form/FormButton.vue'
import Spinner from '~/components/feedback/loading/Spinner.vue'

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
  height: calc(100vh - 64px);
}

@keyframes slideInRight {
  from { opacity: 0; transform: translateX(100%); }
  to { opacity: 1; transform: translateX(0); }
}

.animate-slide-in-right {
  animation: slideInRight 0.3s ease-out forwards;
}

@keyframes fadeInUp {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

.animate-fade-in-up {
  opacity: 0; /* Important for staggered delay to work without flashing */
  animation: fadeInUp 0.4s ease-out forwards;
}
</style>
