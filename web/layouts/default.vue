<template>
  <div class="flex min-h-screen flex-col">
    <HeaderBar />
    <main class="container mx-auto flex flex-grow flex-col">
      <div class="flex flex-grow items-center justify-center py-4">
        <slot />
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
