<template>
  <dialog
    ref="dialog"
    class="m-auto w-[min(100%-48px,440px)] rounded-[18px] border border-line bg-surface p-7 text-text shadow-[0_40px_120px_-30px_oklch(0_0_0/0.8)] backdrop:bg-[oklch(0_0_0/0.6)] backdrop:backdrop-blur-xs"
    @cancel.prevent="cancel"
    @click.self="cancel"
  >
    <div class="flex flex-col gap-3.5">
      <h2 class="m-0 font-display text-3xl leading-[1.1] font-normal">{{ title }}</h2>
      <p class="m-0 leading-[1.6] text-muted">{{ message }}</p>
      <div class="flex justify-end gap-2.5 pt-2">
        <FormButton label="Cancel" variant="outline" :disabled="loading" @click="cancel" />
        <button
          type="button"
          class="flex items-center gap-2 rounded-full bg-danger px-4.5 py-2.5 text-sm font-medium text-[oklch(0.14_0.03_25)] disabled:opacity-60"
          :disabled="loading"
          @click="emit('confirm')"
        >
          <Icon v-if="loading" name="mdi:loading" class="animate-spin" />
          {{ confirmLabel }}
        </button>
      </div>
    </div>
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
  message: "This can't be undone.",
  confirmLabel: 'Delete',
  loading: false,
})

const emit = defineEmits<{
  (e: 'update:open', value: boolean): void
  (e: 'confirm'): void
}>()

const dialog = ref<HTMLDialogElement | null>(null)

function cancel() {
  emit('update:open', false)
}

watch(
  () => props.open,
  (isOpen) => {
    if (isOpen) dialog.value?.showModal()
    else dialog.value?.close()
  },
)
</script>
