import { beforeEach, describe, expect, it, vi } from 'vitest'
import { ref } from 'vue'
import type { Ref } from 'vue'
import { useSuggestedPrompts } from '~/composables/useSuggestedPrompts'
import { aiPrompts, SUGGESTED_PROMPT_COUNT } from '~/data/AiPrompts'

function texts(prompts: { text: string }[]): string[] {
  return prompts.map((prompt) => prompt.text)
}

describe('useSuggestedPrompts', () => {
  beforeEach(() => {
    const states = new Map<string, Ref<unknown>>()
    vi.stubGlobal('useState', (key: string, init: () => unknown) => {
      if (!states.has(key)) states.set(key, ref(init()))
      return states.get(key)
    })
  })

  it('starts with distinct prompts from the list', () => {
    const { prompts } = useSuggestedPrompts('test')

    expect(new Set(texts(prompts.value)).size).toBe(SUGGESTED_PROMPT_COUNT)
    expect(texts(aiPrompts)).toEqual(expect.arrayContaining(texts(prompts.value)))
  })

  it('never repeats a currently shown prompt when shuffling', () => {
    const { prompts, shufflePrompts } = useSuggestedPrompts('test')

    for (let i = 0; i < 50; i++) {
      const before = texts(prompts.value)
      shufflePrompts()
      expect(texts(prompts.value).filter((text) => before.includes(text))).toEqual([])
    }
  })

  it('shows every prompt before repeating any', () => {
    const { prompts, shufflePrompts } = useSuggestedPrompts('test')
    const rounds = Math.floor(aiPrompts.length / SUGGESTED_PROMPT_COUNT)
    const shown = texts(prompts.value)

    for (let i = 1; i < rounds; i++) {
      shufflePrompts()
      shown.push(...texts(prompts.value))
    }

    expect(new Set(shown).size).toBe(shown.length)
  })
})
