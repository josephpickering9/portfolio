<template>
  <div class="flex flex-col gap-4">
    <FormElementContainer :label="label">
      <div class="flex flex-col gap-4">
        <input
          ref="file"
          type="file"
          class="file-input file-input-bordered w-full"
          :multiple="multiple"
          @change="inputChange"
        />
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

<script lang="ts">
import { defineComponent } from 'vue'
import type { PropType } from 'vue'
import Carousel from '../lists/Carousel.vue'
import FormElementContainer from './FormElementContainer.vue'

interface Data {
  images: string[]
}

export default defineComponent({
  name: 'FileInputList',
  components: { FormElementContainer, Carousel },
  props: {
    label: {
      type: String,
      default: null,
    },
    modelValue: {
      type: process.server ? Object : FileList,
      default: null,
    },
    required: {
      type: Boolean,
      default: false,
    },
    disabled: {
      type: Boolean,
      default: false,
    },
    imageUrls: {
      type: Array as PropType<string[]>,
      default: null,
    },
    multiple: {
      type: Boolean,
      default: false,
    },
  },
  emits: ['update:modelValue', 'update:file', 'update:imageUrls'],
  data(): Data {
    return {
      images: this.imageUrls,
    }
  },
  methods: {
    inputChange() {
      if (this.$refs.file) {
        const input = this.$refs.file as HTMLInputElement
        this.$emit('update:file', input.files)
        this.$emit('update:modelValue', input.files)
      }
    },
    removeImage(url: string) {
      this.images = this.images.filter((image) => image !== url)
    },
  },
  watch: {
    imageUrls() {
      this.images = this.imageUrls
    },
    images() {
      this.$emit('update:imageUrls', this.images)
    },
  },
})
</script>
