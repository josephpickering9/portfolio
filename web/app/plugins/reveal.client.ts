import { defineNuxtPlugin } from '#app'

/**
 * v-reveal directive — adds an IntersectionObserver that toggles .is-revealed
 * on an element when it enters the viewport.
 *
 * Usage:
 *   <div v-reveal class="reveal">...</div>
 *   <div v-reveal="{ delay: 120 }" class="reveal">...</div>
 *
 * Pair with .reveal / .reveal-from-left / .reveal-from-right CSS classes.
 * prefers-reduced-motion is handled globally in main.css.
 */
export default defineNuxtPlugin((nuxtApp) => {
  nuxtApp.vueApp.directive('reveal', {
    mounted(el: HTMLElement, binding) {
      const delay: number = (binding.value as { delay?: number } | undefined)?.delay ?? 0

      const observer = new IntersectionObserver(
        (entries) => {
          entries.forEach((entry) => {
            if (entry.isIntersecting) {
              if (delay > 0) {
                setTimeout(() => el.classList.add('is-revealed'), delay)
              } else {
                el.classList.add('is-revealed')
              }
              observer.unobserve(el)
            }
          })
        },
        {
          threshold: 0.1,
          rootMargin: '0px 0px -32px 0px',
        },
      )

      observer.observe(el)
      ;(el as HTMLElement & { _revealObserver?: IntersectionObserver })._revealObserver = observer
    },

    unmounted(el: HTMLElement) {
      ;(el as HTMLElement & { _revealObserver?: IntersectionObserver })._revealObserver?.disconnect()
    },
  })
})
