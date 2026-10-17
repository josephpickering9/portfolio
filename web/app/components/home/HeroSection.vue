<template>
  <section class="relative mx-auto w-full max-w-300 px-7 pt-24 pb-18">
    <div
      class="pointer-events-none absolute -top-20 right-0 left-[40%] h-130 bg-[radial-gradient(closest-side,var(--glow),transparent)] blur-[20px]"
      aria-hidden="true"
    />
    <div class="relative flex max-w-285 flex-col gap-7">
      <div class="flex items-center gap-3.5">
        <NuxtImg
          src="/profile.png"
          alt="Joseph Pickering"
          format="webp"
          width="104"
          height="104"
          loading="eager"
          class="size-13 rounded-full border border-line object-cover"
        />
        <span class="font-mono text-[13px] tracking-[0.06em] text-muted uppercase">Full Stack Developer</span>
      </div>

      <h1 class="m-0 font-display text-[clamp(56px,10vw,136px)] leading-[0.9] font-normal tracking-tight">
        Joseph <span class="text-accent">Pickering</span>
      </h1>

      <p class="m-0 max-w-155 text-[19px] leading-[1.6] text-muted">
        I work across the stack, from <span class="text-text">Vue, React &amp; TypeScript</span> to
        <span class="text-text">.NET, C# &amp; Kotlin</span>.
      </p>

      <HeroAiPrompt />

      <div class="flex flex-wrap gap-5.5 text-sm text-muted">
        <a :href="linkedInUrl" target="_blank" rel="noopener noreferrer" class="flex items-center gap-1.75">
          <Icon name="mdi:linkedin" />
          LinkedIn
        </a>
        <a :href="GITHUB_URL" target="_blank" rel="noopener noreferrer" class="flex items-center gap-1.75">
          <Icon name="mdi:github" />
          GitHub
        </a>
        <a :href="`mailto:${EMAIL}`" class="flex items-center gap-1.75">
          <Icon name="heroicons:envelope" />
          Email me
        </a>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import useLinkedInUrl from '~/composables/useLinkedInUrl'
import { EMAIL, GITHUB_URL } from '~/data/NavItems'
import HeroAiPrompt from '~/components/home/HeroAiPrompt.vue'

const linkedInUrl = useLinkedInUrl()
</script>
