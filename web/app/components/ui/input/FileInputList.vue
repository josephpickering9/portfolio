<template>
  <div class="flex flex-col gap-2" :style="sizeVars">
    <div class="flex gap-3 overflow-x-auto pb-1.5">
      <Draggable
        :model-value="modelValue"
        item-key="key"
        class="flex gap-3"
        ghost-class="opacity-40"
        :disabled="disabled"
        @update:model-value="emit('update:modelValue', $event)"
        @start="onDragStart"
        @end="dragging = undefined"
      >
        <template #item="{ element, index }: { element: UploadItem; index: number }">
          <div
            class="relative aspect-(--aspect) w-(--thumb-width) shrink-0 cursor-grab rounded-[10px] border bg-bg bg-(image:--preview) bg-cover bg-top"
            :class="dragging === index ? 'border-accent' : 'border-line'"
            :style="{ '--preview': `url('${element.previewUrl}')` }"
          >
            <span class="absolute top-1.5 left-1.5 rounded-md bg-[oklch(0_0_0/0.6)] px-1.5 py-0.5 font-mono text-[11px] text-white">
              {{ index + 1 }}
            </span>
            <button
              type="button"
              class="absolute top-1.5 right-1.5 flex size-7 items-center justify-center rounded-full bg-[oklch(0_0_0/0.6)] text-white"
              :aria-label="`Remove image ${index + 1}`"
              @click="remove(index)"
            >
              <Icon name="mdi:close" />
            </button>
          </div>
        </template>
      </Draggable>
      <label
        class="relative flex aspect-(--aspect) w-(--thumb-width) shrink-0 cursor-pointer flex-col items-center justify-center gap-1 rounded-[10px] border border-dashed border-line text-muted hover:text-text"
      >
        <input type="file" accept="image/*" multiple class="sr-only" :disabled="disabled" @change="add">
        <Icon name="mdi:plus" size="24" />
        <span class="text-xs">Add</span>
      </label>
    </div>
    <ul v-if="fitWarnings.length" class="m-0 flex list-none flex-col gap-1 p-0 text-[13px] leading-normal text-muted" role="status">
      <li v-for="warning in fitWarnings" :key="warning.key" class="flex gap-1.5">
        <Icon name="heroicons:exclamation-triangle" class="mt-0.5 shrink-0" />
        <span>Image {{ warning.position }}: {{ warning.message }}</span>
      </li>
    </ul>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import Draggable from 'vuedraggable'
import type { UploadItem } from '~/types/UploadItem'
import type { ImageSpec } from '~/data/ImageSpecs'
import { fromFile } from '~/utils/upload-helper'
import { removeAt } from '~/utils/array-helper'
import { describeImageFit } from '~/utils/image-fit-helper'
import { useImageSizes } from '~/composables/useImageSizes'

interface Props {
  modelValue: UploadItem[]
  width: number
  aspect: string
  disabled?: boolean
  spec?: ImageSpec
}

const props = withDefaults(defineProps<Props>(), {
  disabled: false,
  spec: undefined,
})

const emit = defineEmits<{ (e: 'update:modelValue', value: UploadItem[]): void }>()

const sizes = useImageSizes(() => props.modelValue.map((item) => item.previewUrl))

const dragging = ref<number>()

const sizeVars = computed(() => ({ '--thumb-width': `${props.width}px`, '--aspect': props.aspect }))

const fitWarnings = computed(() =>
  props.modelValue.flatMap((item, index) => {
    const size = sizes[item.previewUrl]
    const message = props.spec && size ? describeImageFit(size, props.spec) : undefined
    return message ? [{ key: item.key, position: index + 1, message }] : []
  }),
)

function onDragStart(event: { oldIndex?: number }) {
  dragging.value = event.oldIndex
}

function add(event: Event) {
  const input = event.target as HTMLInputElement
  const files = Array.from(input.files ?? [])
  emit('update:modelValue', [...props.modelValue, ...files.map((file) => fromFile(file, URL.createObjectURL(file)))])
  input.value = ''
}

function remove(index: number) {
  const removed = props.modelValue[index]
  if (removed?.file) URL.revokeObjectURL(removed.previewUrl)
  emit('update:modelValue', removeAt(props.modelValue, index))
}
</script>
