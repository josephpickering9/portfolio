<template>
  <div class="flex flex-col gap-7">
    <div class="grid grid-cols-[repeat(auto-fit,minmax(min(100%,260px),1fr))] gap-5">
      <FormGroup v-for="single in SINGLE_SLOTS" :key="single.key" :label="single.label" :helper="single.hint">
        <FileInput v-model="images[single.key]" :aspect="single.aspect" :spec="single.spec" :disabled="disabled" />
      </FormGroup>
    </div>

    <div v-for="gallery in GALLERIES" :key="gallery.key" class="flex flex-col gap-3">
      <div class="flex items-baseline justify-between gap-3">
        <span class="eyebrow tracking-[0.05em] text-muted">{{ gallery.label }} ({{ images[gallery.key].length }})</span>
        <span class="text-[13px] text-muted">Drag to reorder</span>
      </div>
      <FileInputList
        v-model="images[gallery.key]"
        :width="gallery.width"
        :aspect="gallery.aspect"
        :spec="gallery.spec"
        :disabled="disabled"
      />
      <span class="text-[13px] text-muted">{{ gallery.hint }}</span>
    </div>

    <Toggle
      v-model="showMockup"
      label="Show device mockup"
      description="Frames screenshots in a browser and phone on the case study"
      :disabled="disabled"
    />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { CreateProject } from '@api'
import type { ProjectImageSlots } from '~/utils/upload-helper'
import { imageSpecs } from '~/data/ImageSpecs'
import type { ImageSpec } from '~/data/ImageSpecs'
import { describeSpec } from '~/utils/image-fit-helper'
import FormGroup from '~/components/ui/form/FormGroup.vue'
import FileInput from '~/components/ui/input/FileInput.vue'
import FileInputList from '~/components/ui/input/FileInputList.vue'
import Toggle from '~/components/ui/input/Toggle.vue'

interface Props {
  disabled?: boolean
}

withDefaults(defineProps<Props>(), {
  disabled: false,
})

const form = defineModel<CreateProject>({ required: true })
const images = defineModel<ProjectImageSlots>('images', { required: true })

const SINGLE_SLOTS: { key: 'banner' | 'card'; label: string; aspect: string; hint: string; spec: ImageSpec }[] = [
  {
    key: 'banner',
    label: 'Banner',
    aspect: '16 / 9',
    hint: `Case-study hero when there are no desktop screenshots. ${describeSpec(imageSpecs.banner)}.`,
    spec: imageSpecs.banner,
  },
  {
    key: 'card',
    label: 'Card',
    aspect: '16 / 10',
    hint: `Shown on project tiles and search results. ${describeSpec(imageSpecs.card)}.`,
    spec: imageSpecs.card,
  },
]

const GALLERIES: { key: 'desktop' | 'mobile'; label: string; width: number; aspect: string; hint: string; spec: ImageSpec }[] = [
  {
    key: 'desktop',
    label: 'Desktop',
    width: 240,
    aspect: '2 / 1',
    hint: `${describeSpec(imageSpecs.desktop)}. The first one is the case-study hero.`,
    spec: imageSpecs.desktop,
  },
  { key: 'mobile', label: 'Mobile', width: 110, aspect: '9 / 19.5', hint: `${describeSpec(imageSpecs.mobile)}.`, spec: imageSpecs.mobile },
]

const showMockup = computed({
  get: () => form.value.showMockup ?? false,
  set: (value: boolean) => {
    form.value.showMockup = value
  },
})

defineExpose({
  validate: async () => true,
  hasErrors: false,
})
</script>
