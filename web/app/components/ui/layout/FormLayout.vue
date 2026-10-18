<template>
  <div class="mx-auto flex w-full max-w-300 flex-col gap-7 px-7 pt-10 pb-30">
    <div class="flex flex-wrap items-end justify-between gap-5">
      <div class="flex min-w-0 flex-col gap-2.5">
        <span class="eyebrow text-accent">{{ eyebrow }}</span>
        <h1 class="m-0 font-display text-[clamp(40px,5vw,60px)] leading-none font-normal tracking-[-0.02em]">{{ title }}</h1>
      </div>
      <div class="flex flex-wrap items-center gap-2.5">
        <FormButton
          v-if="showDelete"
          label="Delete"
          variant="danger"
          icon="heroicons:trash"
          :disabled="loading"
          @click="confirmOpen = true"
        />
        <FormButton :label="saveLabel" :loading="saving" :disabled="loading" @click="emit('save')" />
      </div>
    </div>

    <slot />

    <ConfirmDialog v-model:open="confirmOpen" :title="deleteTitle" :loading="saving" @confirm="confirmDelete" />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import FormButton from '~/components/ui/form/FormButton.vue'
import ConfirmDialog from '~/components/ui/layout/ConfirmDialog.vue'

interface Props {
  eyebrow: string
  title: string
  saveLabel: string
  deleteTitle?: string
  showDelete?: boolean
  loading?: boolean
  saving?: boolean
}

withDefaults(defineProps<Props>(), {
  deleteTitle: 'Delete this?',
  showDelete: false,
  loading: false,
  saving: false,
})

const emit = defineEmits<{ (e: 'save' | 'delete'): void }>()

const confirmOpen = ref(false)

function confirmDelete() {
  confirmOpen.value = false
  emit('delete')
}
</script>
