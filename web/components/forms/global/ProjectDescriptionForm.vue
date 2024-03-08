<template>
  <div v-if="form" class="flex flex-col gap-4 pb-4">
    <FormGroup :errors="v$.form?.description?.$errors" name="Description">
      <TextEditor v-model="form.description" label="Description" :disabled="loading" />
    </FormGroup>
  </div>
</template>

<script lang="ts">
import { defineComponent, type PropType } from 'vue'
import { useVuelidate } from '@vuelidate/core'
import { cloneDeep } from 'lodash-es'
import type { CreateProject, Project } from '../../../api'
import { useProjectStore } from '../../../store/ProjectStore'
import FormGroup from '../FormGroup.vue'
import TextEditor from '../TextEditor.vue'
import useValidation from '../../../composables/useValidation'
import { defaultProjectForm } from '../../../mocks/Defaults'

interface Data {
  form: CreateProject
}

export default defineComponent({
  name: 'ProjectDescriptionForm',
  components: { FormGroup, TextEditor },
  props: {
    modelValue: {
      type: Object as PropType<CreateProject>,
      default: () => cloneDeep(defaultProjectForm),
    },
  },
  emits: ['update:modelValue'],
  setup() {
    return { v$: useVuelidate() }
  },
  data(): Data {
    return {
      form: this.modelValue,
    }
  },
  computed: {
    project(): Project | undefined {
      return useProjectStore().project
    },
    loading(): boolean {
      return useProjectStore().projectCreating || useProjectStore().projectLoading
    },
  },
  methods: {
    async validate(): Promise<boolean> {
      return await useValidation().validate(this.v$)
    },
  },
  watch: {
    modelValue: {
      handler(value: CreateProject) {
        this.form = value
      },
      immediate: true,
    },
    form: {
      handler(value: CreateProject) {
        this.$emit('update:modelValue', value)
      },
      deep: true,
    },
  },
})
</script>
