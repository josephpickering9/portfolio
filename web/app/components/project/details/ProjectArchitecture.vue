<template>
  <div class="flex flex-col gap-4">
    <ol class="m-0 flex list-none flex-nowrap items-stretch gap-2.5 overflow-x-auto p-0 pb-1" aria-label="Request flow">
      <template v-for="(node, index) in nodes" :key="node.title">
        <li v-if="index > 0" class="self-center font-mono text-accent" aria-hidden="true">→</li>
        <li
          class="flex flex-[1_0_140px] flex-col gap-1.5 rounded-xl border bg-surface p-4"
          :class="node.primary ? 'border-accent' : 'border-line'"
        >
          <span class="flex items-center gap-2 font-semibold">
            <Icon v-if="node.icon" :name="node.icon" />
            {{ node.title }}
          </span>
          <span class="text-[13px] text-muted">{{ node.caption }}</span>
        </li>
      </template>
    </ol>
    <div class="grid grid-cols-[repeat(auto-fit,minmax(min(100%,220px),1fr))] gap-2.5">
      <div v-for="note in annotations" :key="note.title" class="flex flex-col gap-1.5 rounded-xl border border-dashed border-line p-4">
        <span class="flex items-center gap-2 font-semibold">
          <Icon :name="note.icon" />
          {{ note.title }}
        </span>
        <span class="text-[13px] leading-normal text-muted">{{ note.body }}</span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const nodes = [
  { title: 'Browser', caption: 'Installable PWA with offline support' },
  { title: 'Nuxt 4', caption: 'SSR, Vue 3, Pinia stores', icon: 'simple-icons:nuxt', primary: true },
  { title: '.NET 8 API', caption: 'Projects, companies, technologies', icon: 'simple-icons:dotnet' },
  { title: 'Vertex AI', caption: 'Natural-language project search', icon: 'simple-icons:googlecloud' },
]

const annotations = [
  {
    title: 'Auth0',
    icon: 'simple-icons:auth0',
    body: 'Bearer token attached to every API request; CMS pages gated by route middleware',
  },
  {
    title: 'OpenAPI codegen',
    icon: 'simple-icons:openapiinitiative',
    body: 'Types and client generated from the backend Swagger spec',
  },
  {
    title: 'Deploy',
    icon: 'simple-icons:githubactions',
    body: 'GitHub Actions on push to main → Digital Ocean droplet, run with PM2',
  },
]
</script>
