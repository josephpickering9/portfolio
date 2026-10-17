export interface AiPrompt {
  text: string
  icon: string
}

export const aiPrompts: AiPrompt[] = [
  { text: 'What React projects has Joe worked on?', icon: 'simple-icons:react' },
  { text: 'What technologies does Joe use most?', icon: 'heroicons:code-bracket' },
  { text: 'What has Joe built with Next.js?', icon: 'simple-icons:nextdotjs' },
  { text: "Tell me about Joe's .NET background.", icon: 'simple-icons:dotnet' },
  { text: 'Which projects use Vue?', icon: 'simple-icons:vuedotjs' },
  { text: 'Show me projects using TypeScript.', icon: 'simple-icons:typescript' },
  { text: 'What Flutter apps has Joe built?', icon: 'simple-icons:flutter' },
  { text: 'What about mobile development with Android?', icon: 'simple-icons:android' },
  { text: 'What PHP projects has Joe worked on?', icon: 'simple-icons:php' },
  { text: 'What AI projects has Joe worked on?', icon: 'heroicons:cpu-chip' },
  { text: 'Has Joe worked on any mobile apps?', icon: 'heroicons:device-phone-mobile' },
  { text: 'What projects involved document processing?', icon: 'heroicons:document-text' },
  { text: "What's Joe's experience with Nuxt?", icon: 'simple-icons:nuxtdotjs' },
  { text: "Tell me about Joe's CI/CD experience.", icon: 'simple-icons:githubactions' },
  { text: 'What Kotlin projects has Joe built?', icon: 'simple-icons:kotlin' },
  { text: 'Has Joe worked with WordPress or CMS platforms?', icon: 'simple-icons:wordpress' },
]

export const SUGGESTED_PROMPT_COUNT = 3
