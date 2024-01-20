<template>
  <FormElementContainer :label="label">
    <input ref="file" type="file" class="file-input file-input-bordered w-full" @change="inputChange" />
  </FormElementContainer>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import FormElementContainer from './FormElementContainer.vue'

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
  },
  emits: ['update:file'],
  methods: {
    inputChange() {
      if (this.$refs.file) {
        const input = this.$refs.file as HTMLInputElement
        this.$emit('update:file', input.files)
      }
    },
  },
})
</script>
