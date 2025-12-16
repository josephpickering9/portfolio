<template>
  <FormElementContainer :label="label">
    <div v-if="!image" class="flex items-center gap-2">
      <input
        ref="file"
        type="file"
        class="file-input w-full"
        :multiple="multiple"
        @change="inputChange"
      >
      <button v-if="previousImageUrl" type="button" @click.stop.prevent="reset">
        <Icon name="material-symbols:undo" size="2em" />
      </button>
    </div>
    <div v-else class="flex h-12 w-full items-center space-x-8">
      <NuxtImg :src="image" alt="File input image" placeholder format="webp" class="m-0 h-12 w-12 rounded-full" />
      <button class="btn btn-error btn-xs" @click.stop.prevent="image = undefined">Remove</button>
    </div>
  </FormElementContainer>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import FormElementContainer from '~/components/ui/form/FormElementContainer.vue'

interface Props {
  label?: string | null
  modelValue?: FileList | null
  required?: boolean
  disabled?: boolean
  imageUrl?: string | null
  multiple?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  label: undefined,
  modelValue: undefined,
  required: false,
  disabled: false,
  imageUrl: undefined,
  multiple: false,
})


const emit = defineEmits<{
  'update:modelValue': [value: FileList | null]
  'update:file': [value: FileList | null]
  'update:imageUrl': [value: string | undefined]
}>()

const file = ref<HTMLInputElement | null>(null)
const image = ref<string | undefined>(props.imageUrl ?? undefined)
const previousImageUrl = ref<string | undefined>(props.imageUrl ?? undefined)

function inputChange() {
  if (file.value) {
    emit('update:file', file.value.files)
    emit('update:modelValue', file.value.files)
  }
}

function reset() {
  image.value = previousImageUrl.value
  if (file.value) file.value.value = ''
}


watch(() => props.imageUrl, (newValue) => {
  image.value = newValue ?? undefined
})

watch(image, (newValue, oldValue) => {
  if (oldValue) previousImageUrl.value = oldValue

  emit('update:imageUrl', image.value)
})
</script>
