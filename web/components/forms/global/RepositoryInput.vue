<template>
  <FormElementContainer :label="label">
    <div class="flex flex-col gap-2">
      <div class="flex items-end gap-2">
        <TextInput v-model="title" label="Title" />
        <TextInput v-model="url" label="URL" />
        <FormButton label="Add" type="primary" size="sm" icon="mdi:plus" class="mb-2" @click="addRepository" />
      </div>
      <div class="flex min-h-8 items-center px-4">
        <div v-if="value && value.length" class="flex flex-wrap items-center gap-2">
          <div v-for="(repository, index) in value" :key="index" class="flex items-end gap-2">
            <Icon name="mdi:github" />
            <span class="text-sm font-semibold">{{ repository.title }}</span>
            <span class="text-sm text-gray-500">({{ repository.url }})</span>
            <FormButton label="Remove" type="error" size="xs" icon="mdi:minus" @click="removeRepository(index)" />
          </div>
        </div>
        <div v-else-if="showEmptyMessage" class="flex items-center text-sm italic text-gray-500">No repositories added</div>
      </div>
    </div>
  </FormElementContainer>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import type { PropType } from 'vue'
import { type CreateProjectRepository } from '../../../api'
import TextInput from '../TextInput.vue'
import FormButton from '../FormButton.vue'
import FormElementContainer from '../FormElementContainer.vue'
import { Icon } from '#components'

interface Data {
  value: CreateProjectRepository[]
  title: string
  url: string
}

export default defineComponent({
  name: 'RepositoryInput',
  components: { TextInput, FormButton, FormElementContainer, Icon },
  props: {
    label: {
      type: String,
      default: null,
    },
    modelValue: {
      type: Array as PropType<CreateProjectRepository[]>,
      default: () => [],
    },
    showEmptyMessage: {
      type: Boolean,
      default: true,
    },
  },
  emits: ['update:modelValue'],
  data(): Data {
    return {
      value: [],
      title: '',
      url: '',
    }
  },
  methods: {
    addRepository() {
      this.value.push({ title: this.title, url: this.url })
      this.title = ''
      this.url = ''
    },
    removeRepository(index: number) {
      this.value.splice(index, 1)
    },
  },
  watch: {
    modelValue() {
      this.value = this.modelValue
    },
    value() {
      this.$emit('update:modelValue', this.value)
    },
  },
})
</script>
