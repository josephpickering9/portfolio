type GlobalFn = (...args: unknown[]) => unknown

function stubbedGlobal(name: string): GlobalFn {
  return (...args: unknown[]) => {
    const fn = (globalThis as Record<string, unknown>)[name]
    if (typeof fn !== 'function') throw new Error(`${name} must be stubbed with vi.stubGlobal in this test`)
    return (fn as GlobalFn)(...args)
  }
}

export const useRuntimeConfig = stubbedGlobal('useRuntimeConfig')
export const useState = stubbedGlobal('useState')
export const useAsyncData = stubbedGlobal('useAsyncData')
export const useHead = stubbedGlobal('useHead')
export const useSeoMeta = stubbedGlobal('useSeoMeta')
export const navigateTo = stubbedGlobal('navigateTo')
export const createError = stubbedGlobal('createError')
export const showError = stubbedGlobal('showError')
export const clearError = stubbedGlobal('clearError')
export const definePageMeta = stubbedGlobal('definePageMeta')
export const defineNuxtPlugin = stubbedGlobal('defineNuxtPlugin')
export const defineNuxtRouteMiddleware = stubbedGlobal('defineNuxtRouteMiddleware')
export const addRouteMiddleware = stubbedGlobal('addRouteMiddleware')
