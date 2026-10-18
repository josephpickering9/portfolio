<template>
  <p>Logout</p>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import useAuth from '~/composables/useAuth'

const { logout } = useAuth()

onMounted(logout)
</script>
