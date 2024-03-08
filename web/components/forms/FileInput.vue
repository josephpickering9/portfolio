<template>
  <FormElementContainer :label="label">
    <div v-if="!image" class="flex items-center gap-2">
      <input
        ref="file"
        type="file"
        class="file-input file-input-bordered w-full"
        :multiple="multiple"
        @change="inputChange"
      />
      <button v-if="previousImageUrl" type="button" @click.stop.prevent="reset">
        <Icon name="material-symbols:undo" size="2em" />
      </button>
    </div>
    <div v-else class="flex h-12 w-full items-center space-x-8">
      <img :src="image" class="m-0 h-12 w-12 rounded-full" />
      <button class="btn btn-error btn-xs" @click.stop.prevent="image = undefined">Remove</button>
    </div>
  </FormElementContainer>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import FormElementContainer from './FormElementContainer.vue'
import { Icon } from '#components'

interface Data {
  image?: string
  previousImageUrl?: string
}

export default defineComponent({
  name: 'FileInput',
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
    imageUrl: {
      type: String,
      default: null,
    },
    multiple: {
      type: Boolean,
      default: false,
    },
  },
  emits: ['update:modelValue', 'update:file', 'update:imageUrl'],
  data(): Data {
    return {
      image: this.imageUrl,
      previousImageUrl: this.imageUrl,
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
    reset() {
      this.image = this.previousImageUrl
      if (this.$refs.file) (this.$refs.file as HTMLInputElement).value = ''
    },
  },
  watch: {
    imageUrl() {
      this.image = this.imageUrl
    },
    image(newValue: string, oldValue: string) {
      if (oldValue) this.previousImageUrl = oldValue

      this.$emit('update:imageUrl', this.image)
    },
  },
})
</script>
