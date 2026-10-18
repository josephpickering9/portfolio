import type { Project } from '@api'

export interface AiAnswer {
  question: string
  thinking: boolean
  failed: boolean
  answer: string
  citations: Project[]
  citationsLoading: boolean
}
