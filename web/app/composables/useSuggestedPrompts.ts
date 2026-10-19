import { sampleSize } from 'lodash-es'
import { useState } from '#imports'
import { aiPrompts, SUGGESTED_PROMPT_COUNT } from '~/data/AiPrompts'
import type { AiPrompt } from '~/data/AiPrompts'

function pickPrompts(): AiPrompt[] {
  return sampleSize(aiPrompts, SUGGESTED_PROMPT_COUNT)
}

export function useSuggestedPrompts(stateKey: string) {
  const prompts = useState<AiPrompt[]>(stateKey, pickPrompts)

  function shufflePrompts() {
    prompts.value = pickPrompts()
  }

  return { prompts, shufflePrompts }
}
