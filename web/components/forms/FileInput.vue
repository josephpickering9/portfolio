<template>
  <FormElementContainer :label="label">
    <input v-if="!image" ref="file" type="file" class="file-input file-input-bordered w-full" @change="inputChange" />
    <div v-else class="flex w-full items-center space-x-8">
      <img :src="image" class="m-0 h-12 w-12 rounded-full" />
      <button class="btn btn-error btn-xs" @click.stop.prevent="image = undefined">Remove</button>
    </div>
  </FormElementContainer>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import FormElementContainer from './FormElementContainer.vue'

interface Data {
  image?: string
}

export default defineComponent({
  name: 'FileInput',
  components: { FormElementContainer },
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
    imageUrl: {
      type: String,
      default: null,
    },
  },
  emits: ['update:file'],
  data(): Data {
    return {
      image: this.imageUrl,
    }
  },
  methods: {
    inputChange() {
      if (this.$refs.file) {
        const input = this.$refs.file as HTMLInputElement
        this.$emit('update:file', input.files)
      }
    },
  },
  watch: {
    imageUrl() {
      this.image = this.imageUrl
    },
  },
})
</script>
