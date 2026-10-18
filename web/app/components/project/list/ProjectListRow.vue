<template>
  <NuxtLink
    :to="`/projects/${project.slug}`"
    class="flex flex-wrap items-baseline gap-x-7 gap-y-2 border-b border-line py-5.5 text-text transition-[padding] duration-200 hover:pl-2.5 hover:text-text"
    data-testid="project-row"
  >
    <span class="flex-[0_0_28px] font-mono text-[13px] text-accent">{{ padNumber(index + 1) }}</span>
    <span class="flex-[0_0_200px] text-[22px] font-semibold">{{ project.title }}</span>
    <span class="flex-[1_1_320px] leading-[1.55] text-muted">{{ project.shortDescription }}</span>
    <span class="flex-none font-mono text-xs text-muted">{{ getTagSummary(project) }}</span>
  </NuxtLink>
</template>

<script setup lang="ts">
import type { Project } from '@api'
import { getTagSummary } from '~/utils/project-helper'
import { padNumber } from '~/utils/string-helper'

interface Props {
  project: Project
  index: number
}

defineProps<Props>()
</script>
