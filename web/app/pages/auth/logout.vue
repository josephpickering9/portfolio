<template>
  <p>Logout</p>
</template>

<script setup lang="ts">
import useAuth from '~/app/composables/useAuth'

const { logout } = useAuth()

if (logout) logout()
</script>
