<template>
  <div ref="root" class="relative">
    <button
      type="button"
      class="flex size-9 items-center justify-center rounded-full border border-line text-muted hover:text-text"
      aria-label="Admin menu"
      aria-haspopup="menu"
      :aria-expanded="open"
      @click="open = !open"
    >
      <Icon name="mdi:account-cog-outline" size="18" />
    </button>
    <div
      v-if="open"
      role="menu"
      class="absolute top-full right-0 mt-2 flex w-52 flex-col rounded-[14px] border border-line bg-surface p-1.5 text-sm shadow-glow"
    >
      <NuxtLink to="/projects/new" role="menuitem" class="flex items-center gap-2.5 rounded-lg px-3 py-2 hover:bg-surface2" @click="open = false">
        <Icon name="heroicons:plus" />
        New project
      </NuxtLink>
      <button
        type="button"
        role="menuitem"
        class="flex items-center gap-2.5 rounded-lg px-3 py-2 text-left hover:bg-surface2 disabled:opacity-60"
        :disabled="projectImageStore.optimising"
        @click="optimiseImages"
      >
        <Icon :name="projectImageStore.optimising ? 'mdi:loading' : 'mdi:file-image-box'" :class="{ 'animate-spin': projectImageStore.optimising }" />
        Optimise images
      </button>
      <button
        type="button"
        role="menuitem"
        class="flex items-center gap-2.5 rounded-lg px-3 py-2 text-left text-danger hover:bg-surface2"
        @click="emit('logout')"
      >
        <Icon name="mdi:logout" />
        Log out
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onBeforeUnmount, onMounted, ref } from 'vue'
import { useProjectImageStore } from '~/store/ProjectImageStore'
import { useNotificationStore } from '~/store/NotificationStore'

const emit = defineEmits<{ (e: 'logout'): void }>()

const projectImageStore = useProjectImageStore()
const notificationStore = useNotificationStore()

const open = ref(false)
const root = ref<HTMLElement>()

async function optimiseImages() {
  await projectImageStore.optimiseImages()
  if (projectImageStore.optimiseError) {
    notificationStore.displayErrorNotification(projectImageStore.optimiseError)
  } else {
    notificationStore.displaySuccessNotification('Images optimised')
  }
}

function closeOnOutsideClick(event: MouseEvent) {
  if (root.value && !root.value.contains(event.target as Node)) open.value = false
}

onMounted(() => document.addEventListener('click', closeOnOutsideClick))
onBeforeUnmount(() => document.removeEventListener('click', closeOnOutsideClick))
</script>
