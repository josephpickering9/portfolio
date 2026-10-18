<template>
  <div class="relative" :class="round ? 'size-35' : 'w-full'">
    <label
      :for="inputId"
      class="flex cursor-pointer items-center justify-center overflow-hidden border border-dashed bg-cover bg-top bg-no-repeat transition-colors duration-200"
      :class="[
        round ? 'size-35 rounded-full bg-surface2 bg-cover bg-center' : 'w-full rounded-xl bg-bg',
        invalid ? 'border-danger' : dragging ? 'border-accent' : 'border-line',
      ]"
      :style="{ aspectRatio: round ? undefined : aspect, backgroundImage: modelValue ? `url('${modelValue.previewUrl}')` : undefined }"
      @dragover.prevent="dragging = true"
      @dragleave="dragging = false"
      @drop.prevent="onDrop"
    >
      <span v-if="!modelValue" class="flex flex-col items-center gap-1.5 text-[13px] text-muted">
        <Icon name="heroicons:arrow-up-tray" size="24" />
        {{ emptyLabel }}
      </span>
    </label>
    <input :id="inputId" type="file" accept="image/*" class="sr-only" :disabled="disabled" @change="onPick">
    <button
      v-if="modelValue"
      type="button"
      class="absolute top-2 right-2 flex size-7 items-center justify-center rounded-full bg-[oklch(0_0_0/0.6)] text-white"
      aria-label="Remove image"
      :disabled="disabled"
      @click="emit('update:modelValue', null)"
    >
      <Icon name="mdi:close" />
    </button>
    <p v-if="fitWarning" class="m-0 mt-2 flex gap-1.5 text-[13px] leading-normal text-muted" role="status">
      <Icon name="heroicons:exclamation-triangle" class="mt-0.5 shrink-0" />
      {{ fitWarning }}
    </p>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, useId } from 'vue'
import type { UploadItem } from '~/types/UploadItem'
import type { ImageSpec } from '~/data/ImageSpecs'
import { fromFile } from '~/utils/upload-helper'
import { describeImageFit } from '~/utils/image-fit-helper'
import { useFormField } from '~/composables/useFormField'
import { useImageSizes } from '~/composables/useImageSizes'

interface Props {
  modelValue: UploadItem | null
  aspect?: string
  round?: boolean
  emptyLabel?: string
  disabled?: boolean
  spec?: ImageSpec
}

const props = withDefaults(defineProps<Props>(), {
  aspect: '16 / 9',
  round: false,
  emptyLabel: 'Drop or choose an image',
  disabled: false,
  spec: undefined,
})

const emit = defineEmits<{ (e: 'update:modelValue', value: UploadItem | null): void }>()

const { id, invalid } = useFormField()
const inputId = id ?? useId()
const dragging = ref(false)
const sizes = useImageSizes(() => (props.modelValue ? [props.modelValue.previewUrl] : []))

const fitWarning = computed(() => {
  const url = props.modelValue?.previewUrl
  const size = url ? sizes[url] : undefined
  return props.spec && size ? describeImageFit(size, props.spec) : undefined
})

function select(file?: File | null) {
  if (file) emit('update:modelValue', fromFile(file, URL.createObjectURL(file)))
}

function onPick(event: Event) {
  const input = event.target as HTMLInputElement
  select(input.files?.item(0))
  input.value = ''
}

function onDrop(event: DragEvent) {
  dragging.value = false
  select(event.dataTransfer?.files.item(0))
}
</script>
