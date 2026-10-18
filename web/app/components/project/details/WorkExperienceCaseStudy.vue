<template>
  <CaseStudySection label="01 / Problem">
    <p class="m-0 max-w-170 text-lg leading-[1.65]">
      Originally built as a personal tool to search projects by technology, it's now a full portfolio with AI search, company
      timelines, and a CMS for managing content.
    </p>
  </CaseStudySection>

  <CaseStudySection label="02 / How it's built">
    <ProjectArchitecture />
  </CaseStudySection>

  <CaseStudySection label="03 / Decisions">
    <div class="flex max-w-170 flex-col gap-5.5">
      <div class="rounded-card bg-surface2 px-6 py-5.5">
        <div class="mb-2 text-lg font-semibold">The API contract is generated, never hand-written</div>
        <p class="m-0 leading-[1.6] text-muted">
          API types and the HTTP client are auto-generated from the backend's Swagger spec. A backend change surfaces as TypeScript
          errors in the stores and components that consume it.
        </p>
      </div>
      <div v-for="decision in decisions" :key="decision.title" class="flex flex-col gap-1.5">
        <span class="font-semibold">{{ decision.title }}</span>
        <span class="leading-[1.6] text-muted">{{ decision.body }}</span>
      </div>
    </div>
  </CaseStudySection>

  <CaseStudySection label="04 / Stack">
    <ul class="m-0 grid list-none grid-cols-[repeat(auto-fit,minmax(min(100%,200px),1fr))] gap-x-6 gap-y-0.5 p-0 text-[15px]">
      <li v-for="row in stack" :key="row.layer" class="flex justify-between gap-3 border-b border-line py-3">
        <span class="pt-0.5 font-mono text-xs tracking-[0.05em] text-muted uppercase">{{ row.layer }}</span>
        <span class="flex items-center gap-2 text-right">
          <Icon :name="row.icon" />
          {{ row.tech }}
        </span>
      </li>
    </ul>
  </CaseStudySection>
</template>

<script setup lang="ts">
import CaseStudySection from './CaseStudySection.vue'
import ProjectArchitecture from './ProjectArchitecture.vue'

const decisions = [
  { title: 'Server-side rendering', body: 'Nuxt 4 SSR for fast load times and SEO.' },
  {
    title: 'All API calls go through Pinia stores',
    body: 'Components never call the API directly; state is persisted where it helps.',
  },
  {
    title: 'URL-persisted filters',
    body: 'Search and filter by technology type, and share the exact view by link.',
  },
]

const stack = [
  { layer: 'Framework', tech: 'Nuxt 4, Vue 3', icon: 'simple-icons:nuxt' },
  { layer: 'Language', tech: 'TypeScript', icon: 'simple-icons:typescript' },
  { layer: 'Backend', tech: '.NET 8', icon: 'simple-icons:dotnet' },
  { layer: 'AI', tech: 'Vertex AI', icon: 'simple-icons:googlecloud' },
  { layer: 'State', tech: 'Pinia', icon: 'simple-icons:pinia' },
  { layer: 'Auth', tech: 'Auth0', icon: 'simple-icons:auth0' },
  { layer: 'Styling', tech: 'Tailwind CSS v4', icon: 'simple-icons:tailwindcss' },
  { layer: 'Rich text', tech: 'Tiptap', icon: 'mdi:format-text' },
  { layer: 'Unit tests', tech: 'Vitest', icon: 'simple-icons:vitest' },
  { layer: 'E2E tests', tech: 'Playwright', icon: 'mdi:drama-masks' },
  { layer: 'PWA', tech: '@vite-pwa/nuxt', icon: 'simple-icons:pwa' },
  { layer: 'Hosting', tech: 'Digital Ocean, PM2', icon: 'simple-icons:digitalocean' },
]
</script>
