import { computed, inject, provide, useId } from 'vue'
import type { ComputedRef, InjectionKey } from 'vue'

interface FormFieldContext {
  id: string
  invalid: ComputedRef<boolean>
}

const FORM_FIELD_KEY: InjectionKey<FormFieldContext> = Symbol('form-field')

const BASE_FIELD_CLASS =
  'w-full rounded-field border bg-bg px-3.5 py-3 text-[15px] text-text outline-none transition-colors duration-200 [color-scheme:inherit] focus:border-accent disabled:opacity-60'

export function provideFormField(invalid: ComputedRef<boolean>): FormFieldContext {
  const context = { id: useId(), invalid }
  provide(FORM_FIELD_KEY, context)
  return context
}

export function useFormField() {
  const context = inject(FORM_FIELD_KEY, undefined)
  const invalid = computed(() => context?.invalid.value ?? false)
  const fieldClass = computed(() => `${BASE_FIELD_CLASS} ${invalid.value ? 'border-danger' : 'border-line'}`)

  return { id: context?.id, invalid, fieldClass }
}
