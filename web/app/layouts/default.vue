<template>
  <div class="flex min-h-dvh flex-col">
    <a href="#main-content" class="sr-only focus:not-sr-only focus:absolute focus:z-50 focus:top-2 focus:left-2 focus:px-4 focus:py-2 focus:rounded-lg focus:bg-primary focus:text-primary-content focus:font-semibold">
      Skip to content
    </a>
    <HeaderBar />
    <main id="main-content" class="flex flex-grow overflow-hidden overflow-y-auto">
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
