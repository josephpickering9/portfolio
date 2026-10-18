<template>
  <div
    role="status"
    data-testid="toast"
    class="flex items-center gap-2.5 rounded-full border bg-surface2 px-4.5 py-3 text-sm whitespace-nowrap text-text shadow-[0_20px_60px_-20px_oklch(0_0_0/0.7)]"
    :class="isError ? 'border-danger' : 'border-ok'"
  >
    <Icon :name="isError ? 'heroicons:exclamation-triangle' : 'heroicons:check-circle'" size="20" :class="isError ? 'text-danger' : 'text-ok'" />
    <span>{{ notification.message }}</span>
  </div>
</template>

<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted } from 'vue'
import { NotificationType } from '~/types/NotificationType'
import type { Notification } from '~/types/Notification'
import { useNotificationStore } from '~/store/NotificationStore'

interface Props {
  notification: Notification
}

const props = defineProps<Props>()

const MS_PER_SECOND = 1000
const ERROR_TYPES = [NotificationType.ERROR, NotificationType.WARNING]

const notificationStore = useNotificationStore()

let dismissTimer: ReturnType<typeof setTimeout> | undefined

const isError = computed(() => ERROR_TYPES.includes(props.notification.type))

onMounted(() => {
  dismissTimer = setTimeout(
    () => notificationStore.removeNotification(props.notification.id),
    props.notification.seconds * MS_PER_SECOND,
  )
})

onBeforeUnmount(() => clearTimeout(dismissTimer))
</script>
