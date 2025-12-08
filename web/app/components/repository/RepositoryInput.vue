<template>
  <FormElementContainer :label="label">
    <div class="flex flex-col gap-8">
      <div class="flex items-end gap-2">
        <FormGroup :errors="v$.title?.$errors" name="Title">
          <TextInput v-model="title" label="Title" @keyup.enter="addRepository" />
        </FormGroup>
        <FormGroup :errors="v$.url?.$errors" name="URL">
          <TextInput v-model="url" label="URL" @keyup.enter="addRepository" />
        </FormGroup>
        <FormButton label="Add" type="primary" size="sm" icon="mdi:plus" class="mb-2" @click="addRepository" />
      </div>
      <div class="flex min-h-8 items-center px-4">
        <div v-if="value && value.length" class="flex flex-col items-start gap-4">
          <div v-for="(repository, index) in value" :key="index" class="flex w-full items-center gap-2">
            <Icon name="mdi:github" />
            <span class="text-sm font-semibold">{{ repository.title }}</span>
            <span class="flex-1 text-sm text-gray-500">({{ repository.url }})</span>
            <FormButton
              label="Remove"
              class="self-end"
              type="error"
              size="xs"
              icon="mdi:minus"
              @click="removeRepository(index)"
            />
          </div>
        </div>
        <div v-else-if="showEmptyMessage" class="flex items-center text-sm italic text-gray-500">No repositories added</div>
      </div>
    </div>
  </FormElementContainer>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { useVuelidate } from '@vuelidate/core'
import { required, url as urlValidator } from '@vuelidate/validators'
import type { CreateProjectRepository } from '@api'
import useValidation from '~/composables/useValidation'
import TextInput from '~/components/ui/input/TextInput.vue'
import FormButton from '~/components/ui/form/FormButton.vue'
import FormElementContainer from '~/components/ui/form/FormElementContainer.vue'
import FormGroup from '~/components/ui/form/FormGroup.vue'

interface Props {
  label?: string | null
  modelValue?: CreateProjectRepository[]
  showEmptyMessage?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  label: undefined,
  modelValue: () => [],
  showEmptyMessage: true,
})


const emit = defineEmits<{
  'update:modelValue': [value: CreateProjectRepository[]]
}>()

const validation = useValidation()

const value = ref<CreateProjectRepository[]>([])
const title = ref('')
const url = ref('')


const rules = {
  title: { required },
  url: { required, url: urlValidator },
}

const v$ = useVuelidate(rules, { title, url })

async function addRepository() {
  const isValid = await validation.validate(v$)
  if (!isValid) return

  value.value.push({ title: title.value, url: url.value })
  title.value = ''
  url.value = ''
  v$.value.$reset()
}

function removeRepository(index: number) {
  value.value.splice(index, 1)
}


watch(() => props.modelValue, (newValue) => {
  value.value = newValue
}, { immediate: true })

watch(value, (newValue) => {
  emit('update:modelValue', newValue)
}, { deep: true })
</script>
