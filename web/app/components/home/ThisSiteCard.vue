<template>
  <section class="mx-auto w-full max-w-300 px-7 py-10">
    <NuxtLink
      to="/projects/work-experience"
      class="grid grid-cols-[repeat(auto-fit,minmax(min(100%,340px),1fr))] items-center gap-8 rounded-[20px] border border-line bg-surface p-9 text-text transition-colors duration-300 hover:border-accent hover:text-text"
    >
      <div class="flex flex-col gap-3.5">
        <span class="eyebrow text-accent">05 / You're looking at it</span>
        <h3 class="m-0 font-display text-[40px] leading-[1.05] font-normal">Work Experience</h3>
        <p class="m-0 leading-[1.6] text-muted">
          This portfolio is a project in its own right: Nuxt 4 on the front, a .NET 8 API behind it, AI search through Vertex AI
          and a CMS for managing content.
        </p>
        <span class="text-[15px] font-medium text-accent">Read how it's built →</span>
      </div>
      <ul class="m-0 flex list-none flex-wrap gap-2.5 p-0 font-mono text-[13px]">
        <li v-for="tech in STACK" :key="tech.label" class="flex items-center gap-2 rounded-tag border border-line px-3 py-2">
          <Icon :name="tech.icon" />
          {{ tech.label }}
        </li>
      </ul>
    </NuxtLink>
  </section>
</template>

<script setup lang="ts">
const STACK = [
  { label: 'Nuxt 4', icon: 'simple-icons:nuxt' },
  { label: 'Vue 3', icon: 'simple-icons:vuedotjs' },
  { label: 'TypeScript', icon: 'simple-icons:typescript' },
  { label: '.NET 8', icon: 'simple-icons:dotnet' },
  { label: 'Vertex AI', icon: 'simple-icons:googlecloud' },
  { label: 'Auth0', icon: 'simple-icons:auth0' },
]
</script>
