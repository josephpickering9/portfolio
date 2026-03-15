<template>
  <div class="flex flex-col gap-4">
    <FormElementContainer :label="label">
      <div class="flex flex-col gap-4">
        <input
          ref="file"
          type="file"
          class="file-input w-full"
          :multiple="multiple"
          @change="inputChange"
        >
      </div>
    </FormElementContainer>
    <div v-if="images && images.length" class="flex w-full flex-wrap items-center gap-4">
      <Carousel
        v-model="images"
        width="180px"
        :show-hover-button="true"
        :draggable="true"
        :show-arrows="true"
        @hover-button="removeImage"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import Carousel from '~/components/layout/Carousel.vue'
import FormElementContainer from '~/components/ui/form/FormElementContainer.vue'

interface Props {
  label?: string | null
  modelValue?: FileList | null
  required?: boolean
  disabled?: boolean
  imageUrls?: string[] | null
  multiple?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  label: undefined,
  modelValue: undefined,
  required: false,
  disabled: false,
  imageUrls: null,
  multiple: false,
})


const emit = defineEmits<{
  'update:modelValue': [value: FileList | null]
  'update:file': [value: FileList | null]
  'update:imageUrls': [value: string[]]
}>()

const file = ref<HTMLInputElement | null>(null)
const images = ref<string[]>(props.imageUrls ?? [])

function inputChange() {
  if (file.value) {
    emit('update:file', file.value.files)
    emit('update:modelValue', file.value.files)
  }
}

function removeImage(url: string) {
  images.value = images.value.filter((image) => image !== url)
}


watch(() => props.imageUrls, (newValue) => {
  images.value = newValue ?? []
})

watch(images, (newValue) => {
  emit('update:imageUrls', newValue)
})
</script>
