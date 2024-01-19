<template>
    <div v-if="notification" class="alert" :class="typeClass">
        <span>{{ notification.message }}</span>
    </div>
</template>

<script lang="ts">
import { type PropType, defineComponent } from 'vue'
import { NotificationType } from '../../types/NotificationType'
import type { Notification } from '../../types/Notification'
import { formatDistanceToNow } from 'date-fns'
import { useNotificationStore } from '../../store/NotificationStore'

interface Data {
    createdAt: Date | null
    timeSinceCreated: string
}

export default defineComponent({
    name: 'Toast',
    props: {
        notification: {
            type: Object as PropType<Notification>,
            required: true,
            default: null,
        }
    },
    data(): Data {
        return {
            createdAt: new Date(),
            timeSinceCreated: 'Just now'
        }
    },
    computed: {
        typeClass(): string {
            switch (this.notification.type) {
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
        }
    },
    mounted() {
        if (!this.notification) return

        const thirtySeconds = 30 * 1000
        setTimeout(() => 
            () => setInterval(() => (this.timeSinceCreated = `${formatDistanceToNow(this.createdAt as Date)} ago`), thirtySeconds),
            thirtySeconds,
        )

        if (this.notification.seconds) {
            setTimeout(() => this.close(), this.notification.seconds * 1000)
        }
    },
    methods: {
        close() {
            useNotificationStore().removeNotification(this.notification.id)
        }
    }
})
</script>