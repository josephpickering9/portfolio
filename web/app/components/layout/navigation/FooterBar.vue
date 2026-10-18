<template>
  <footer class="border-t border-line">
    <div class="mx-auto flex max-w-300 flex-wrap items-center justify-between gap-4 px-7 py-6 font-mono text-xs text-muted">
      <span>© {{ year }} Joseph Pickering</span>
      <div class="flex flex-wrap gap-5">
        <a :href="linkedInUrl" target="_blank" rel="noopener noreferrer" class="flex items-center gap-1.5">
          <Icon name="mdi:linkedin" size="14" aria-hidden="true" />
          LinkedIn
        </a>
        <a :href="GITHUB_URL" target="_blank" rel="noopener noreferrer" class="flex items-center gap-1.5">
          <Icon name="mdi:github" size="14" aria-hidden="true" />
          GitHub
        </a>
        <a :href="`mailto:${EMAIL}`" class="flex items-center gap-1.5">
          <Icon name="heroicons:envelope" size="14" aria-hidden="true" />
          Email me
        </a>
      </div>
    </div>
  </footer>
</template>

<script setup lang="ts">
import useLinkedInUrl from '~/composables/useLinkedInUrl'
import { EMAIL, GITHUB_URL } from '~/data/NavItems'

const year = new Date().getFullYear()
const linkedInUrl = useLinkedInUrl()
</script>
