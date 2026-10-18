<template>
  <section class="mx-auto w-full max-w-300 px-7 pt-20 pb-24">
    <div class="flex flex-col items-start gap-5.5 border-t border-line pt-14">
      <h2 class="m-0 max-w-200 font-display text-[clamp(40px,6vw,72px)] leading-none font-normal tracking-[-0.02em]">
        Got a project in mind? <em class="text-accent">Let's talk.</em>
      </h2>
      <div class="flex flex-wrap gap-3">
        <a
          :href="`mailto:${EMAIL}`"
          class="flex items-center gap-2 rounded-full bg-accent px-5.5 py-3.25 font-medium text-accent-ink hover:text-accent-ink hover:opacity-90"
        >
          <Icon name="heroicons:envelope" />
          {{ EMAIL }}
        </a>
        <a
          :href="linkedInUrl"
          target="_blank"
          rel="noopener noreferrer"
          class="flex items-center gap-2 rounded-full border border-line px-5.5 py-3.25 font-medium text-text"
        >
          <Icon name="mdi:linkedin" />
          LinkedIn
        </a>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import useLinkedInUrl from '~/composables/useLinkedInUrl'
import { EMAIL } from '~/data/NavItems'

const linkedInUrl = useLinkedInUrl()
</script>
