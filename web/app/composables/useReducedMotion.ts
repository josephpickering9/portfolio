const REDUCED_MOTION_QUERY = '(prefers-reduced-motion: reduce)'

export function useReducedMotion() {
  function prefersReducedMotion(): boolean {
    return import.meta.client && window.matchMedia(REDUCED_MOTION_QUERY).matches
  }

  return { prefersReducedMotion }
}
