<template>
  <div v-if="form" class="flex flex-col gap-6 pb-4">
    <div class="flex flex-col gap-4 md:flex-row">
      <FormGroup :errors="v$.form?.title?.$errors" name="Title" class="md:w-3/4">
        <TextInput v-model="form.title" label="Title" :disabled="loading" />
      </FormGroup>
      <FormGroup :errors="v$.form?.year?.$errors" name="Year" class="md:w-1/4">
        <YearSelectList v-model="form.year" label="Year" :disabled="loading" />
      </FormGroup>
    </div>
    <FormGroup :errors="v$.form?.shortDescription?.$errors" name="Short Description">
      <TextInput v-model="form.shortDescription" label="Short Description" :disabled="loading" />
    </FormGroup>
    <div class="flex flex-col gap-4 md:flex-row">
      <FormGroup :errors="v$.form?.companyId?.$errors" name="Company" class="md:w-2/5">
        <CompanyAutoComplete v-model="form.companyId" label="Company" :disabled="loading" />
      </FormGroup>
      <FormGroup :errors="v$.form?.website?.$errors" name="Website" class="md:w-3/5">
        <TextInput v-model="form.website" label="Website" :disabled="loading" />
      </FormGroup>
    </div>
    <FormGroup :errors="v$.form?.tags?.$errors" name="Tags">
      <TagAutoComplete v-model="form.tags" label="Tags" placeholder="Search by tag name" />
    </FormGroup>
  </div>
</template>

<script lang="ts">
import { defineComponent, type PropType } from 'vue'
import { useVuelidate } from '@vuelidate/core'
import { required, numeric, url } from '@vuelidate/validators'
import { cloneDeep } from 'lodash-es'
import type { CreateProject, Project } from '../../../api'
import { useProjectStore } from '../../../store/ProjectStore'
import FormGroup from '../elements/FormGroup.vue'
import TextInput from '../elements/TextInput.vue'
import { defaultProjectForm } from '../../../mocks/Defaults'
import useValidation from '../../../composables/useValidation'
import YearSelectList from '../global/YearSelectList.vue'
import CompanyAutoComplete from '../company/CompanyAutoComplete.vue'
import TagAutoComplete from '../tag/TagAutoComplete.vue'

interface Data {
  form: CreateProject
}

export default defineComponent({
  name: 'ProjectGeneralForm',
  components: { FormGroup, TextInput, YearSelectList, CompanyAutoComplete, TagAutoComplete },
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
  validations() {
    return {
      form: {
        title: { required },
        year: { required, numeric },
        shortDescription: { required },
        website: { url },
        tags: { required },
      },
    }
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
