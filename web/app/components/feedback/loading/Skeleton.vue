<template>
  <div v-if="type === typeProfile" class="flex w-full max-w-5xl flex-col gap-4">
    <div class="flex items-center gap-4">
      <div class="skeleton h-16 w-16 shrink-0 rounded-full"/>
      <div class="flex flex-col gap-4">
        <div class="skeleton h-4 w-20"/>
        <div class="skeleton h-4 w-28"/>
      </div>
    </div>
    <div class="skeleton h-40 w-full"/>
  </div>
  <div v-else-if="type === typeCard" class="flex w-full flex-col gap-4">
    <div class="skeleton h-40 w-full"/>
    <div class="skeleton h-4 w-28"/>
    <div class="skeleton h-4 w-full"/>
    <div class="skeleton h-4 w-full"/>
  </div>
</template>

<script setup lang="ts">
import { LoadingType } from '~/types/LoadingType'

interface Props {
  type?: LoadingType
}

withDefaults(defineProps<Props>(), {
  type: LoadingType.PROFILE,
})

const typeCard = LoadingType.CARD
const typeProfile = LoadingType.PROFILE
</script>
