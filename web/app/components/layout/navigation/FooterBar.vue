<template>
  <footer class="border-t border-line">
    <div class="mx-auto flex max-w-300 flex-wrap items-center justify-between gap-4 px-7 py-6 font-mono text-xs text-muted">
      <span>© {{ year }} {{ SITE_NAME }}</span>
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
import { useContactLinks } from '~/composables/useContactLinks'
import { formatYear } from '~/utils/date-helper'
import { SITE_NAME } from '~/utils/meta-helper'

const contactLinks = useContactLinks()

const year = formatYear(new Date())
</script>
