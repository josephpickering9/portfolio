<template>
  <dialog ref="dialog" class="modal" @cancel="emit('update:open', false)">
    <div class="modal-box">
      <h3 class="text-lg font-bold">{{ title }}</h3>
      <p class="py-4 text-base-content/70">{{ message }}</p>
      <div class="modal-action">
        <FormButton label="Cancel" type="ghost" size="sm" :disabled="loading" @click="cancel" />
        <FormButton :label="confirmLabel" type="error" size="sm" :loading="loading" @click="confirm" />
      </div>
    </div>
    <form method="dialog" class="modal-backdrop">
      <button @click="cancel">close</button>
    </form>
  </dialog>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import FormButton from '~/components/ui/form/FormButton.vue'

interface Props {
  open: boolean
  title?: string
  message?: string
  confirmLabel?: string
  loading?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  title: 'Are you sure?',
  message: 'This action cannot be undone.',
  confirmLabel: 'Delete',
  loading: false,
})

const emit = defineEmits<{
  'update:open': [value: boolean]
  'confirm': []
  'cancel': []
}>()

const dialog = ref<HTMLDialogElement | null>(null)

function confirm() {
  emit('confirm')
}

function cancel() {
  emit('update:open', false)
  emit('cancel')
}

watch(() => props.open, (isOpen) => {
  if (isOpen) dialog.value?.showModal()
  else dialog.value?.close()
})
</script>
