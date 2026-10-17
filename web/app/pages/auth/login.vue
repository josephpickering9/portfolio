<template>
  <div class="relative flex min-h-[70vh] items-center justify-center px-7 py-24">
    <div
      class="pointer-events-none absolute top-2/5 left-1/2 h-105 w-160 max-w-full -translate-1/2 bg-[radial-gradient(closest-side,var(--glow),transparent)]"
      aria-hidden="true"
    />
    <div
      class="relative flex w-full max-w-105 flex-col items-center gap-5 rounded-[20px] border border-line bg-surface px-8 py-10 text-center shadow-[0_40px_120px_-40px_var(--glow)]"
    >
      <NuxtImg src="/profile.png" alt="" format="webp" width="128" height="128" class="size-16 rounded-full border border-line object-cover" />
      <div class="flex flex-col gap-2">
        <h1 class="m-0 font-display text-[40px] leading-none font-normal">Sign in</h1>
        <p class="m-0 leading-[1.55] text-muted">Manage projects, companies and technologies.</p>
      </div>
      <button
        type="button"
        class="flex w-full items-center justify-center gap-2 rounded-full bg-accent px-5 py-3 text-[15px] font-medium text-accent-ink"
        @click="login?.()"
      >
        <Icon name="simple-icons:auth0" />
        Continue with Auth0
      </button>
      <NuxtLink to="/" class="text-sm text-muted">← Back to site</NuxtLink>
    </div>
  </div>
</template>

<script setup lang="ts">
import useAuth from '~/composables/useAuth'
import useMeta from '~/composables/useMeta'

const { login } = useAuth()
const { updateMeta } = useMeta()

updateMeta({ title: 'Sign in' })
</script>
