<template>
  <div 
    class="text-center pt-2 space-y-2 md:space-y-4 transition-all duration-500 delay-100"
    :class="[hasSearched ? 'opacity-0 h-0 overflow-hidden' : 'opacity-100 h-auto']"
  >
    <div class="inline-flex items-center justify-center p-4 bg-gradient-to-br from-primary/10 to-secondary/10 rounded-3xl mb-4 animate-float">
      <Icon name="heroicons:sparkles" class="w-12 h-12 text-primary" />
    </div>
    <h1 class="text-3xl md:text-5xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-primary to-secondary">
      Ask my AI Assistant
    </h1>
    <p class="text-lg text-base-content/60 max-w-md mx-auto">
      Explore my experience, projects, and skills.
    </p>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  hasSearched: boolean
}>()
</script>

<style scoped>
@keyframes float {
  0% { transform: translateY(0px); }
  50% { transform: translateY(-10px); }
  100% { transform: translateY(0px); }
}

.animate-float {
  animation: float 6s ease-in-out infinite;
}
</style>
