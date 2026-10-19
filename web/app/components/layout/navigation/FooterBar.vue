<template>
  <footer class="border-t border-line">
    <div class="mx-auto flex max-w-300 flex-wrap items-center justify-between gap-4 px-7 py-6 font-mono text-xs text-muted">
      <div class="flex flex-wrap items-center gap-x-5 gap-y-2">
        <span>© {{ year }} {{ SITE_NAME }}</span>
        <a :href="SOURCE_REPO_URL" target="_blank" rel="noopener noreferrer" data-testid="footer-source-link">Source</a>
        <a
          v-if="commitSha"
          :href="`${SOURCE_REPO_URL}/commit/${commitSha}`"
          target="_blank"
          rel="noopener noreferrer"
          :aria-label="`Deployed commit ${shortCommitSha}`"
          data-testid="footer-commit-link"
        >
          {{ shortCommitSha }}
        </a>
      </div>
      <div class="flex flex-wrap gap-5">
        <a
          v-for="link in contactLinks"
          :key="link.label"
          :href="link.href"
          :target="link.external ? '_blank' : undefined"
          :rel="link.external ? 'noopener noreferrer' : undefined"
          class="flex items-center gap-1.5"
        >
          <Icon :name="link.icon" size="14" aria-hidden="true" />
          {{ link.label }}
        </a>
      </div>
    </div>
  </footer>
</template>

<script setup lang="ts">
import { useRuntimeConfig } from '#imports'
import { useContactLinks } from '~/composables/useContactLinks'
import { SOURCE_REPO_URL } from '~/data/Site'
import { formatYear } from '~/utils/date-helper'
import { SITE_NAME } from '~/utils/meta-helper'
import { formatShortSha } from '~/utils/string-helper'

const commitSha = useRuntimeConfig().public.commitSha
const shortCommitSha = formatShortSha(commitSha)

const contactLinks = useContactLinks()

const year = formatYear(new Date())
</script>
