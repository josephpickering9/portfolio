<template>
  <div class="flex flex-col gap-4">
    <FormGroup label="Description" label-tag="span" helper="Shown as the Overview section of the case study.">
      <TextEditor v-model="form.description" placeholder="Describe the project, your role and the stack…" :disabled="disabled || suggesting" />
    </FormGroup>
    <div v-if="projectId" class="flex flex-wrap items-center gap-2.5">
      <div class="min-w-0 flex-[1_1_260px]">
        <TextInput v-model="extraContext" placeholder="Extra context for the AI (optional)" :disabled="disabled || suggesting" />
      </div>
      <FormButton
        label="Suggest with AI"
        variant="outline"
        icon="mdi:robot-happy-outline"
        :loading="suggesting"
        :disabled="disabled"
        @click="suggest"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import type { CreateProject } from '@api'
import { useAiStore } from '~/store/AiStore'
import { useNotificationStore } from '~/store/NotificationStore'
import FormGroup from '~/components/ui/form/FormGroup.vue'
import TextEditor from '~/components/ui/input/TextEditor.vue'
import TextInput from '~/components/ui/input/TextInput.vue'
import FormButton from '~/components/ui/form/FormButton.vue'

const props = defineProps<{
  projectId?: string
  disabled?: boolean
}>()

const form = defineModel<CreateProject>({ required: true })

const aiStore = useAiStore()
const notificationStore = useNotificationStore()
const extraContext = ref('')

const suggesting = computed(() => aiStore.aiForm.loading ?? false)

async function suggest() {
  const projectId = props.projectId
  if (!projectId) return
  await aiStore.getSuggestedDescription(projectId, extraContext.value || null, null, null)
  const suggestion = aiStore.aiForm.data?.suggestedDescription
  if (aiStore.aiForm.error || !suggestion) {
    notificationStore.displayErrorNotification("Couldn't get a suggestion right now")
    return
  }
  form.value.description = suggestion
}

defineExpose({
  validate: async () => true,
  hasErrors: computed(() => false),
})
</script>
