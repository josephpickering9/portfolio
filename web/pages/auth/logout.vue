<template>
  <p>Logout</p>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import useAuth from '../../composables/useAuth'

export default defineComponent({
  name: 'Logout',
  setup() {
    const { logout } = useAuth()

    if (logout) logout()
  },
})
</script>
