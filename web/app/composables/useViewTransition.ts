import { nextTick } from 'vue'

export function useViewTransition() {
  function canAnimate(): boolean {
    return (
      import.meta.client &&
      typeof document.startViewTransition === 'function' &&
      !window.matchMedia('(prefers-reduced-motion: reduce)').matches
    )
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
