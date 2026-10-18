<template>
  <div class="flex flex-col gap-4">
    <FormGroup label="Description" label-tag="span" helper="Shown as the Overview section of the case study.">
      <TextEditor v-model="form.description" placeholder="Describe the project, your role and the stack…" :disabled="disabled || aiDescriptionStore.suggesting" />
    </FormGroup>
    <div v-if="projectId" class="flex flex-wrap items-center gap-2.5">
      <div class="min-w-0 flex-[1_1_260px]">
        <TextInput v-model="extraContext" placeholder="Extra context for the AI (optional)" :disabled="disabled || aiDescriptionStore.suggesting" />
      </div>
      <FormButton
        label="Suggest with AI"
        variant="outline"
        icon="mdi:robot-happy-outline"
        :loading="aiDescriptionStore.suggesting"
        :disabled="disabled"
        @click="suggest"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import type { CreateProject } from '@api'
import { useAiDescriptionStore } from '~/store/AiDescriptionStore'
import { useNotificationStore } from '~/store/NotificationStore'
import FormGroup from '~/components/ui/form/FormGroup.vue'
import TextEditor from '~/components/ui/input/TextEditor.vue'
import TextInput from '~/components/ui/input/TextInput.vue'
import FormButton from '~/components/ui/form/FormButton.vue'

interface Props {
  projectId?: string
  disabled?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  projectId: undefined,
  disabled: false,
})

const form = defineModel<CreateProject>({ required: true })

const SUGGESTION_FAILED_MESSAGE = "Couldn't get a suggestion right now"

const aiDescriptionStore = useAiDescriptionStore()
const notificationStore = useNotificationStore()

const extraContext = ref('')

async function suggest() {
  const projectId = props.projectId
  if (!projectId) return

  const suggestion = await aiDescriptionStore.suggestDescription(projectId, { additionalContext: extraContext.value || null })
  if (aiDescriptionStore.suggestionError || !suggestion) {
    notificationStore.displayErrorNotification(aiDescriptionStore.suggestionError ?? SUGGESTION_FAILED_MESSAGE)
    return
  }
  form.value.description = suggestion
}

defineExpose({
  validate: async () => true,
  hasErrors: false,
})
</script>
