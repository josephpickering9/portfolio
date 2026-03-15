<template>
  <div class="text-center space-y-4 animate-fade-in-up max-w-3xl">
    <div class="avatar mb-2">
      <div class="w-32 rounded-full shadow-xl ring ring-primary ring-offset-base-100 ring-offset-2">
        <NuxtImg src="/profile.png" alt="Joseph Pickering" placeholder format="webp" />
      </div>
    </div>
    
    <div class="space-y-4">
      <h1 class="text-5xl md:text-7xl font-extrabold bg-gradient-to-r from-primary via-secondary to-accent bg-clip-text text-transparent pb-2">
        Joseph Pickering
      </h1>
      <p class="text-xl md:text-2xl font-medium text-base-content/80">
        Full Stack Developer
      </p>
       <p class="text-lg text-base-content/70 leading-relaxed max-w-2xl mx-auto">
          I'm a passionate developer building cleaner, faster, and more intuitive applications. 
          My journey spans from <span class="font-semibold text-primary">Vue, React &amp; TypeScript</span> to <span class="font-semibold text-secondary">.NET, C# &amp; Kotlin</span>. 
          I love solving complex problems and turning ideas into reality.
      </p>
    </div>

    <div class="flex justify-center gap-4 pt-4">
      <Tooltip text="LinkedIn" position="bottom">
        <a :href="linkedInUrl" target="_blank" aria-label="LinkedIn profile" class="btn btn-circle btn-lg btn-ghost hover:text-[#0077b5] hover:bg-[#0077b5]/10 transition-colors">
          <Icon name="mdi:linkedin" class="!w-6 !h-6" />
        </a>
      </Tooltip>
      <Tooltip text="GitHub" position="bottom">
        <a href="https://github.com/josephpickering9" target="_blank" aria-label="GitHub profile" class="btn btn-circle btn-lg btn-ghost hover:text-base-content hover:bg-base-content/10 transition-colors">
          <Icon name="mdi:github" class="!w-6 !h-6" />
        </a>
      </Tooltip>
      <Tooltip text="Download CV" position="bottom">
        <a href="/Joseph Pickering CV.pdf" download="Joseph Pickering CV" class="btn btn-circle btn-lg btn-ghost hover:text-secondary hover:bg-secondary/10 transition-colors" aria-label="Download CV">
          <Icon name="mdi:file-document-outline" class="!w-6 !h-6" />
        </a>
      </Tooltip>
      <Tooltip text="Email Me" position="bottom">
        <a href="mailto:josephpickering9@gmail.com" aria-label="Send email" class="btn btn-circle btn-lg btn-ghost hover:text-primary hover:bg-primary/10 transition-colors">
          <Icon name="heroicons:envelope" class="!w-6 !h-6" />
        </a>
      </Tooltip>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import Tooltip from '~/components/ui/tooltip/Tooltip.vue'

const runtimeConfig = useRuntimeConfig()

const linkedInUrl = computed((): string => {
  return runtimeConfig.public.linkedInUrl ?? 'https://www.linkedin.com/in/josephpickering'
})
</script>
