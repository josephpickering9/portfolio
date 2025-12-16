<template>
  <div class="flex min-h-dvh flex-col">
    <HeaderBar />
    <main class="flex flex-grow overflow-hidden overflow-y-auto">
      <div class="container mx-auto flex flex-grow flex-col overflow-hidden">
        <div class="relative flex w-full flex-grow">
          <slot />
        </div>
      </div>
    </main>
    <FooterBar />
  </div>
</template>

<script setup lang="ts">
import HeaderBar from '~/components/layout/navigation/HeaderBar.vue'
import FooterBar from '~/components/layout/navigation/FooterBar.vue'
</script>
