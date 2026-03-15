<template>
  <div class="text-center space-y-4 max-w-3xl">
    <div class="avatar mb-2 animate-fade-in-up" style="animation-delay: 0ms">
      <div class="w-32 rounded-full shadow-xl ring ring-primary ring-offset-base-100 ring-offset-2">
        <NuxtImg src="/profile.png" alt="Joseph Pickering" format="webp" loading="eager" />
      </div>
    </div>

    <div class="space-y-4">
      <h1 class="text-5xl md:text-7xl font-extrabold text-base-content pb-2 animate-fade-in-up" style="animation-delay: 120ms">
        Joseph Pickering
      </h1>
      <p class="text-xl md:text-2xl font-medium text-base-content/80 animate-fade-in-up" style="animation-delay: 220ms">
        Full Stack Developer
      </p>
      <p class="text-lg text-base-content/70 leading-relaxed max-w-2xl mx-auto animate-fade-in-up" style="animation-delay: 320ms">
        I'm a passionate developer building cleaner, faster, and more intuitive applications.
        My journey spans from <span class="font-semibold text-primary">Vue, React &amp; TypeScript</span> to <span class="font-semibold text-secondary">.NET, C# &amp; Kotlin</span>.
        I love solving complex problems and turning ideas into reality.
      </p>
    </div>

    <div class="flex flex-wrap justify-center gap-3 pt-4 animate-fade-in-up" style="animation-delay: 420ms">
      <a :href="linkedInUrl" target="_blank" aria-label="LinkedIn profile" class="btn btn-ghost btn-sm rounded-full gap-2 hover:text-[var(--color-linkedin)] hover:bg-[var(--color-linkedin)]/10 transition-colors">
        <Icon name="mdi:linkedin" class="!w-4 !h-4" />
        LinkedIn
      </a>
      <a href="https://github.com/josephpickering9" target="_blank" aria-label="GitHub profile" class="btn btn-ghost btn-sm rounded-full gap-2 hover:text-base-content hover:bg-base-content/10 transition-colors">
        <Icon name="mdi:github" class="!w-4 !h-4" />
        GitHub
      </a>
      <a href="/Joseph Pickering CV.pdf" download="Joseph Pickering CV" aria-label="Download CV" class="btn btn-ghost btn-sm rounded-full gap-2 hover:text-secondary hover:bg-secondary/10 transition-colors">
        <Icon name="mdi:file-document-outline" class="!w-4 !h-4" />
        Download CV
      </a>
      <a href="mailto:josephpickering9@gmail.com" aria-label="Send email" class="btn btn-ghost btn-sm rounded-full gap-2 hover:text-primary hover:bg-primary/10 transition-colors">
        <Icon name="heroicons:envelope" class="!w-4 !h-4" />
        Email Me
      </a>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

const runtimeConfig = useRuntimeConfig()

const linkedInUrl = computed((): string => {
  return runtimeConfig.public.linkedInUrl ?? 'https://www.linkedin.com/in/josephpickering'
})
</script>

<style scoped>
.social-linkedin :deep(svg),
.social-github :deep(svg),
.social-cv :deep(svg),
.social-email :deep(svg) {
  transition: transform 0.25s cubic-bezier(0.25, 1, 0.5, 1);
}

.social-linkedin:hover :deep(svg) {
  transform: scale(1.2) translateY(-2px);
}

.social-github:hover :deep(svg) {
  transform: rotate(-15deg) scale(1.15);
}

.social-cv:hover :deep(svg) {
  transform: translateY(-3px) scale(1.1);
}

.social-email:hover :deep(svg) {
  transform: rotate(8deg) scale(1.1);
}
</style>
