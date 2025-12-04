<template>
  <div class="flex min-h-dvh flex-col">
    <HeaderBar />
    <main class="flex flex-grow overflow-hidden overflow-y-auto">
      <div class="container mx-auto flex flex-grow flex-col overflow-hidden">
        <div class="relative flex w-full flex-grow pb-4">
          <slot />
        </div>
      </div>
    </main>
    <FooterBar />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import HeaderBar from '../components/navigation/HeaderBar.vue'
import FooterBar from '../components/navigation/FooterBar.vue'

export default defineComponent({
  name: 'Default',
  components: {
    HeaderBar,
    FooterBar,
  },
})
</script>
