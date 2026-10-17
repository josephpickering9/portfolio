<template>
  <div class="flex flex-col gap-5.5">
    <FormGroup v-for="field in fields" :key="field.key" :label="field.label" :errors="v$.form[field.key].$errors">
      <template #aside>
        <span class="font-mono text-xs" :class="length(field.key) > CASE_STUDY_MAX_LENGTH ? 'text-danger' : 'text-muted'">
          {{ length(field.key) }} / {{ CASE_STUDY_MAX_LENGTH }}
        </span>
      </template>
      <TextArea v-model="form[field.key]" :placeholder="field.placeholder" :disabled="disabled" />
    </FormGroup>

    <FormGroup
      label="Stats"
      label-tag="span"
      :errors="statErrorsList"
      helper="Only add numbers that are real and say something about outcome or scale. The row is hidden when empty."
    >
      <StatInput v-model="stats" :invalid-fields="statErrors" />
    </FormGroup>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import { useVuelidate } from '@vuelidate/core'
import type { ErrorObject } from '@vuelidate/core'
import { helpers, maxLength } from '@vuelidate/validators'
import type { CreateProject, CreateProjectStat } from '@api'
import { CASE_STUDY_MAX_LENGTH, validateStats } from '~/utils/validation-helper'
import FormGroup from '~/components/ui/form/FormGroup.vue'
import TextArea from '~/components/ui/input/TextArea.vue'
import StatInput from '~/components/stat/StatInput.vue'

type CaseStudyField = 'problemStatement' | 'keyDecision' | 'outcome'

defineProps<{
  disabled?: boolean
}>()

const form = defineModel<CreateProject>({ required: true })

const fields: { key: CaseStudyField; label: string; placeholder: string }[] = [
  { key: 'problemStatement', label: 'Problem', placeholder: 'What problem did this project solve?' },
  { key: 'keyDecision', label: 'Decisions', placeholder: 'What was the key decision or tradeoff?' },
  { key: 'outcome', label: 'Outcome', placeholder: 'What was the result?' },
]

const submitted = ref(false)

const stats = computed({
  get: () => form.value.stats ?? [],
  set: (value: CreateProjectStat[]) => {
    form.value.stats = value
  },
})

const statErrors = computed(() => (submitted.value ? validateStats(stats.value) : {}))
const statErrorsList = computed((): ErrorObject[] =>
  Object.keys(statErrors.value).length ? [{ $message: 'Each stat needs a value and a label' } as ErrorObject] : [],
)

const tooLong = helpers.withMessage(`Keep this under ${CASE_STUDY_MAX_LENGTH} characters`, maxLength(CASE_STUDY_MAX_LENGTH))
const rules = {
  form: {
    problemStatement: { maxLength: tooLong },
    keyDecision: { maxLength: tooLong },
    outcome: { maxLength: tooLong },
  },
}

const v$ = useVuelidate(rules, { form })

function length(key: CaseStudyField): number {
  return form.value[key]?.length ?? 0
}

async function validate(): Promise<boolean> {
  submitted.value = true
  const fieldsValid = await v$.value.$validate()
  return fieldsValid && Object.keys(validateStats(stats.value)).length === 0
}

defineExpose({
  validate,
  hasErrors: computed(() => v$.value.$error || statErrorsList.value.length > 0),
})
</script>
