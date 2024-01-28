<template>
  <div v-if="type === typeProfile" class="flex w-full max-w-5xl flex-col gap-4">
    <div class="flex items-center gap-4">
      <div class="skeleton h-16 w-16 shrink-0 rounded-full"></div>
      <div class="flex flex-col gap-4">
        <div class="skeleton h-4 w-20"></div>
        <div class="skeleton h-4 w-28"></div>
      </div>
    </div>
    <div class="skeleton h-40 w-full"></div>
  </div>
  <div v-else-if="type === typeCard" class="flex w-full flex-col gap-4">
    <div class="skeleton h-40 w-full"></div>
    <div class="skeleton h-4 w-28"></div>
    <div class="skeleton h-4 w-full"></div>
    <div class="skeleton h-4 w-full"></div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { LoadingType } from '../../types/LoadingType'

interface Data {
  typeCard: LoadingType
  typeProfile: LoadingType
}

export default defineComponent({
  name: 'Skeleton',
  props: {
    type: {
      type: String as () => LoadingType,
      default: LoadingType.PROFILE,
    },
  },
  data(): Data {
    return {
      typeCard: LoadingType.CARD,
      typeProfile: LoadingType.PROFILE,
    }
  },
})
</script>
