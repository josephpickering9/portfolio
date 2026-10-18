import { nextTick } from 'vue'
import { useReducedMotion } from '~/composables/useReducedMotion'

export function useViewTransition() {
  const { prefersReducedMotion } = useReducedMotion()

  function canAnimate(): boolean {
    return import.meta.client && typeof document.startViewTransition === 'function' && !prefersReducedMotion()
  }

  async function withViewTransition(update: () => void): Promise<void> {
    if (!canAnimate()) {
      update()
      return
    }
    const transition = document.startViewTransition(async () => {
      update()
      await nextTick()
    })
    await transition.updateCallbackDone
  }

  return { withViewTransition }
}
