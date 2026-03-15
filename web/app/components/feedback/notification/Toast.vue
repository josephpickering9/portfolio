<template>
  <div v-if="notification" class="alert" :class="typeClass">
    <span>{{ notification.message }}</span>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { formatDistanceToNow } from 'date-fns'
import { NotificationType } from '~/types/NotificationType'
import type { Notification } from '~/types/Notification'
import { useNotificationStore } from '~/store/NotificationStore'

interface Props {
  notification?: Notification | null
}

const props = withDefaults(defineProps<Props>(), {
  notification: null,
})

const createdAt = ref<Date | null>(new Date())
const timeSinceCreated = ref('Just now')

const typeClass = computed((): string => {
  if (!props.notification) return 'alert-info'

  switch (props.notification.type) {
    case NotificationType.SUCCESS:
      return 'alert-success'
    case NotificationType.ERROR:
      return 'alert-error'
    case NotificationType.WARNING:
      return 'alert-warning'
    case NotificationType.INFO:
      return 'alert-info'
    default:
      return 'alert-info'
  }
})

function close() {
  if (props.notification) {
    useNotificationStore().removeNotification(props.notification.id)
  }
}

onMounted(() => {
  if (!props.notification) return

  const thirtySeconds = 30 * 1000
  setTimeout(
    () => () =>
        setInterval(() => (timeSinceCreated.value = `${formatDistanceToNow(createdAt.value as Date)} ago`), thirtySeconds),
    thirtySeconds,
  )

  if (props.notification.seconds) {
    setTimeout(() => close(), props.notification.seconds * 1000)
  }
})
</script>
