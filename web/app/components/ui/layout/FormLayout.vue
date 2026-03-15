<template>
  <div class="flex w-full max-w-5xl flex-col gap-6">
    <div class="flex items-center justify-between">
      <h1 class="text-3xl font-bold">{{ title }}</h1>
      <div class="flex items-center space-x-2">
        <slot name="actions-start" />
        
        <FormButton
          v-if="showDelete"
          label="Delete"
          type="error"
          size="sm"
          icon="material-symbols:delete"
          :disabled="loading"
          @click="$emit('delete')"
        />
        
        <slot name="actions-middle" />

        <FormButton 
          :label="saveLabel" 
          type="primary" 
          size="sm" 
          :disabled="loading" 
          :loading="loading"
          @click="$emit('save')" 
        />
        
        <slot name="actions-end" />
      </div>
    </div>

    <div class="w-full">
      <slot />
    </div>
  </div>
</template>

<script setup lang="ts">
import FormButton from '~/components/ui/form/FormButton.vue'

interface Props {
  title: string
  loading?: boolean
  showDelete?: boolean
  saveLabel?: string
}

withDefaults(defineProps<Props>(), {
  loading: false,
  showDelete: false,
  saveLabel: 'Save',
})

defineEmits<{
  'save': []
  'delete': []
}>()
</script>
