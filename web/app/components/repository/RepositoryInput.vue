<template>
  <div class="flex flex-col gap-3">
    <div v-for="(repository, index) in modelValue" :key="index" class="flex flex-col gap-1.5">
      <div class="flex flex-wrap items-center gap-2.5">
        <input
          :value="repository.title"
          placeholder="Frontend"
          :aria-label="`Repository ${index + 1} title`"
          class="min-w-0 flex-[0_1_180px] rounded-field border bg-bg px-3.5 py-2.75 text-[15px] text-text outline-none focus:border-accent"
          :class="errorFor(index, 'title') ? 'border-danger' : 'border-line'"
          @input="update(index, 'title', ($event.target as HTMLInputElement).value)"
        >
        <div
          class="flex flex-[1_1_300px] items-center gap-2.5 rounded-field border bg-bg px-3.5 focus-within:border-accent"
          :class="errorFor(index, 'url') ? 'border-danger' : 'border-line'"
        >
          <Icon name="mdi:github" class="shrink-0 text-muted" />
          <input
            :value="repository.url"
            placeholder="https://github.com/…"
            :aria-label="`Repository ${index + 1} URL`"
            class="min-w-0 flex-1 border-0 bg-transparent py-2.75 text-[15px] text-text outline-none"
            @input="update(index, 'url', ($event.target as HTMLInputElement).value)"
          >
        </div>
        <RowRemoveButton label="Remove repository" @click="remove(index)" />
      </div>
      <span v-if="errorFor(index, 'url') || errorFor(index, 'title')" class="text-[13px] text-danger" role="alert">
        {{ errorFor(index, 'title') || errorFor(index, 'url') }}
      </span>
    </div>
    <span v-if="!modelValue.length" class="text-sm text-muted">
      No repositories linked. Public repos appear as buttons on the case study.
    </span>
    <FormButton label="Add repository" variant="dashed" icon="mdi:plus" @click="add" />
  </div>
</template>

<script setup lang="ts">
import type { CreateProjectRepository } from '@api'
import FormButton from '~/components/ui/form/FormButton.vue'
import RowRemoveButton from '~/components/ui/button/RowRemoveButton.vue'

type RepositoryField = 'title' | 'url'

interface Props {
  modelValue: CreateProjectRepository[]
  errors?: Record<number, Partial<Record<RepositoryField, string>>>
}

const props = withDefaults(defineProps<Props>(), {
  errors: () => ({}),
})

const emit = defineEmits<{ (e: 'update:modelValue', value: CreateProjectRepository[]): void }>()

function errorFor(index: number, field: RepositoryField): string | undefined {
  return props.errors[index]?.[field]
}

function update(index: number, field: RepositoryField, value: string) {
  emit(
    'update:modelValue',
    props.modelValue.map((repository, repoIndex) => (repoIndex === index ? { ...repository, [field]: value } : repository)),
  )
}

function add() {
  emit('update:modelValue', [...props.modelValue, { title: '', url: '' }])
}

function remove(index: number) {
  emit(
    'update:modelValue',
    props.modelValue.filter((_, repoIndex) => repoIndex !== index),
  )
}
</script>
