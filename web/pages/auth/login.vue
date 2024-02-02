<template>
  <p>Login</p>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import useAuth from '../../composables/useAuth'

export default defineComponent({
  name: 'Login',
  setup() {
    const { login } = useAuth()

    if (login) login()
  },
})
</script>
