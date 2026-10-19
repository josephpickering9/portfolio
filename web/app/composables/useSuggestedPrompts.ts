import { sampleSize } from 'lodash-es'
import { useState } from '#imports'
import { aiPrompts, SUGGESTED_PROMPT_COUNT } from '~/data/AiPrompts'
import type { AiPrompt } from '~/data/AiPrompts'

function pickPrompts(pool: AiPrompt[] = aiPrompts): AiPrompt[] {
  return sampleSize(pool, SUGGESTED_PROMPT_COUNT)
}

function texts(prompts: AiPrompt[]): string[] {
  return prompts.map((prompt) => prompt.text)
}

export function useSuggestedPrompts(stateKey: string) {
  const prompts = useState<AiPrompt[]>(stateKey, pickPrompts)
  const seen = useState<string[]>(`${stateKey}-seen`, () => texts(prompts.value))

  function shufflePrompts() {
    let unseen = aiPrompts.filter((prompt) => !seen.value.includes(prompt.text))
    if (unseen.length < SUGGESTED_PROMPT_COUNT) {
      const shown = texts(prompts.value)
      seen.value = shown
      unseen = aiPrompts.filter((prompt) => !shown.includes(prompt.text))
    }
    prompts.value = pickPrompts(unseen)
    seen.value = [...seen.value, ...texts(prompts.value)]
  }

  return { prompts, shufflePrompts }
}
