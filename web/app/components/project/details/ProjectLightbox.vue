<template>
  <dialog
    ref="dialog"
    aria-label="Screenshot viewer"
    class="m-0 h-dvh max-h-none w-screen max-w-none overflow-hidden bg-transparent p-0 text-text backdrop:bg-[oklch(0_0_0/0.88)] backdrop:backdrop-blur-sm"
    data-testid="project-lightbox"
    @cancel.prevent="close"
    @keydown="onKeydown"
  >
    <div v-if="current" class="flex h-full flex-col">
      <header class="flex items-center justify-between gap-4 px-4 py-3 sm:px-6">
        <p class="m-0 font-mono text-xs tracking-wide text-muted" aria-live="polite" data-testid="lightbox-position">
          <span class="text-text">{{ index + 1 }}</span> / {{ images.length }}
          <span class="ml-2 hidden sm:inline">— {{ current.alt }}</span>
        </p>
        <div class="flex items-center gap-1">
          <a
            :href="current.src"
            target="_blank"
            rel="noopener noreferrer"
            class="grid size-10 place-items-center rounded-full text-muted transition-colors hover:bg-surface2 hover:text-text"
            aria-label="Open original image in new tab"
          >
            <Icon name="mdi:open-in-new" class="size-5" />
          </a>
          <button
            type="button"
            class="grid size-10 place-items-center rounded-full text-muted transition-colors hover:bg-surface2 hover:text-text"
            aria-label="Close viewer"
            data-testid="lightbox-close"
            @click="close"
          >
            <Icon name="mdi:close" class="size-6" />
          </button>
        </div>
      </header>

      <div
        class="relative flex min-h-0 flex-1 items-center justify-center px-4 sm:px-20"
        @click.self="close"
        @touchstart.passive="onTouchStart"
        @touchend="onTouchEnd"
      >
        <NuxtImg
          :key="current.src"
          format="webp"
          :src="current.src"
          :alt="current.alt"
          class="max-h-full max-w-full rounded-card object-contain shadow-glow-lg select-none"
          :class="{ 'border border-line': current.kind === 'mobile' }"
          draggable="false"
          data-testid="lightbox-image"
        />

        <template v-if="hasMultiple">
          <button
            type="button"
            class="absolute top-1/2 left-2 grid size-11 -translate-y-1/2 place-items-center rounded-full border border-line bg-surface/80 text-text backdrop-blur-sm transition-colors hover:border-accent hover:text-accent sm:left-5"
            aria-label="Previous screenshot"
            data-testid="lightbox-prev"
            @click="step(-1)"
          >
            <Icon name="mdi:chevron-left" class="size-7" />
          </button>
          <button
            type="button"
            class="absolute top-1/2 right-2 grid size-11 -translate-y-1/2 place-items-center rounded-full border border-line bg-surface/80 text-text backdrop-blur-sm transition-colors hover:border-accent hover:text-accent sm:right-5"
            aria-label="Next screenshot"
            data-testid="lightbox-next"
            @click="step(1)"
          >
            <Icon name="mdi:chevron-right" class="size-7" />
          </button>
        </template>
      </div>

      <nav v-if="hasMultiple" aria-label="Screenshots" class="px-4 pt-3 pb-4 sm:px-6">
        <ul ref="strip" class="m-0 flex list-none justify-start gap-2 overflow-x-auto p-1 sm:justify-center">
          <li v-for="(image, i) in images" :key="image.src" class="shrink-0">
            <button
              type="button"
              class="block overflow-hidden rounded-[8px] border-2 transition-[opacity,border-color] duration-200"
              :class="i === index ? 'border-accent opacity-100' : 'border-transparent opacity-45 hover:opacity-80'"
              :aria-label="`View screenshot ${i + 1}`"
              :aria-current="i === index ? 'true' : undefined"
              @click="emit('update:index', i)"
            >
              <NuxtImg
                format="webp"
                :src="image.src"
                alt=""
                :width="image.kind === 'mobile' ? 24 : 76"
                height="48"
                fit="cover"
                :modifiers="{ position: 'top' }"
                loading="lazy"
                class="block h-12 object-cover object-top"
                :class="image.kind === 'mobile' ? 'w-6' : 'w-19'"
              />
            </button>
          </li>
        </ul>
      </nav>
    </div>
  </dialog>
</template>

<script setup lang="ts">
import { computed, nextTick, onBeforeUnmount, ref, watch } from 'vue'
import type { LightboxImage } from '~/types/LightboxImage'

interface Props {
  images: LightboxImage[]
  index: number | null
}

const props = defineProps<Props>()

const emit = defineEmits<{
  (e: 'update:index', value: number | null): void
}>()

const SWIPE_THRESHOLD_PX = 50
const SCROLL_LOCK_CLASS = 'overflow-hidden'

const dialog = ref<HTMLDialogElement | null>(null)
const strip = ref<HTMLUListElement | null>(null)
const touchStartX = ref<number | null>(null)

const index = computed(() => props.index ?? 0)
const current = computed(() => (props.index === null ? undefined : props.images[props.index]))
const hasMultiple = computed(() => props.images.length > 1)

function setPageScrollLocked(locked: boolean) {
  document.documentElement.classList.toggle(SCROLL_LOCK_CLASS, locked)
}

function close() {
  emit('update:index', null)
}

function step(delta: number) {
  const count = props.images.length
  emit('update:index', (index.value + delta + count) % count)
}

function onKeydown(event: KeyboardEvent) {
  if (event.key === 'ArrowRight') step(1)
  else if (event.key === 'ArrowLeft') step(-1)
  else if (event.key === 'Home') emit('update:index', 0)
  else if (event.key === 'End') emit('update:index', props.images.length - 1)
  else return
  event.preventDefault()
}

function onTouchStart(event: TouchEvent) {
  touchStartX.value = event.touches[0]?.clientX ?? null
}

function onTouchEnd(event: TouchEvent) {
  const startX = touchStartX.value
  const endX = event.changedTouches[0]?.clientX
  touchStartX.value = null
  if (startX === null || endX === undefined || !hasMultiple.value) return
  const deltaX = endX - startX
  if (Math.abs(deltaX) >= SWIPE_THRESHOLD_PX) step(deltaX < 0 ? 1 : -1)
}

onBeforeUnmount(() => setPageScrollLocked(false))

watch(
  () => props.index,
  async (value, previous) => {
    const el = dialog.value
    if (!el) return
    if (value === null) {
      el.close()
      setPageScrollLocked(false)
      return
    }
    if (previous === null || !el.open) {
      el.showModal()
      setPageScrollLocked(true)
    }
    await nextTick()
    strip.value?.querySelector('[aria-current="true"]')?.scrollIntoView({ block: 'nearest', inline: 'center' })
  },
)
</script>

