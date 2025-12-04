<template>
  <p>Login</p>
</template>

<script setup lang="ts">
import useAuth from '~/app/composables/useAuth'

const { login } = useAuth()

if (login) login()
</script>
