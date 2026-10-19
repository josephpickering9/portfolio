export interface AiPrompt {
  text: string
  icon: string
}

export const aiPrompts: AiPrompt[] = [
  { text: 'What has Joe worked on most recently?', icon: 'heroicons:clock' },
  { text: 'Which companies has Joe worked for?', icon: 'heroicons:building-office-2' },
  { text: 'What technologies does Joe use most?', icon: 'heroicons:code-bracket' },
  { text: "What measurable impact have Joe's projects had?", icon: 'heroicons:chart-bar' },
  { text: 'What has Joe built with Vue and Nuxt?', icon: 'simple-icons:nuxtdotjs' },
  { text: 'What React projects has Joe worked on?', icon: 'simple-icons:react' },
  { text: 'Show me projects using TypeScript.', icon: 'simple-icons:typescript' },
  { text: "Tell me about Joe's .NET background.", icon: 'simple-icons:dotnet' },
  { text: 'What backend work has Joe done in Kotlin?', icon: 'simple-icons:kotlin' },
  { text: 'What PHP and Laravel work has Joe done?', icon: 'simple-icons:php' },
  { text: 'What AI projects has Joe worked on?', icon: 'heroicons:cpu-chip' },
  { text: 'What projects involved document processing?', icon: 'heroicons:document-text' },
  { text: 'Has Joe worked on any mobile apps?', icon: 'heroicons:device-phone-mobile' },
  { text: 'Has Joe built a design system or component library?', icon: 'simple-icons:storybook' },
  { text: 'What databases has Joe worked with?', icon: 'heroicons:circle-stack' },
  { text: 'Which cloud platforms has Joe deployed to?', icon: 'heroicons:cloud' },
  { text: "Tell me about Joe's CI/CD experience.", icon: 'simple-icons:githubactions' },
  { text: 'Has Joe worked with WordPress or CMS platforms?', icon: 'simple-icons:wordpress' },
  { text: 'What tourism and heritage websites has Joe built?', icon: 'heroicons:building-library' },
]

export const SUGGESTED_PROMPT_COUNT = 3
