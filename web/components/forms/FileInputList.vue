<template>
  <FormElementContainer :label="label">
    <div class="flex flex-col gap-4">
      <input
        ref="file"
        type="file"
        class="file-input file-input-bordered w-full"
        :multiple="multiple"
        @change="inputChange"
      />
      <div v-if="images.length" class="flex w-full flex-wrap items-center gap-4">
        <button
          v-for="(url, index) in images"
          :key="index"
          type="button"
          class="group relative rounded-full"
          @click.stop.prevent="removeImage(url)"
        >
          <img :src="url" class="m-0 h-12 w-12 rounded-full" />
          <div
            class="absolute top-0 z-10 !hidden h-full w-full items-center justify-center rounded-full group-hover:!flex group-hover:bg-black group-hover:bg-opacity-70"
          >
            <Icon name="material-symbols:delete" size="2em" />
          </div>
        </button>
      </div>
    </div>
  </FormElementContainer>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import type { PropType } from 'vue'
import FormElementContainer from './FormElementContainer.vue'
import { Icon } from '#components'

interface Data {
  images: string[]
}

export default defineComponent({
  name: 'FileInputList',
  components: { FormElementContainer, Icon },
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
