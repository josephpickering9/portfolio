<template>
  <div class="overflow-hidden rounded-xl border bg-bg" :class="invalid ? 'border-danger' : 'border-line'">
    <div v-if="editor" class="flex flex-wrap gap-0.5 border-b border-line p-1.5" role="toolbar" aria-label="Formatting">
      <button
        v-for="action in actions"
        :key="action.label"
        type="button"
        class="flex size-9 items-center justify-center rounded-lg text-lg hover:bg-surface2"
        :class="action.active?.() ? 'bg-surface2 text-accent' : 'text-text'"
        :title="action.label"
        :aria-label="action.label"
        :aria-pressed="action.active?.() ?? undefined"
        :disabled="disabled"
        @mousedown.prevent="action.run()"
      >
        <Icon :name="action.icon" />
      </button>
    </div>
    <form v-if="linkOpen" class="flex items-center gap-2 border-b border-line p-2" @submit.prevent="applyLink">
      <input
        ref="linkInput"
        v-model="linkUrl"
        type="url"
        placeholder="https://"
        aria-label="Link URL"
        class="min-w-0 flex-1 rounded-lg border border-line bg-surface px-3 py-2 text-sm text-text outline-none focus:border-accent"
      >
      <button type="submit" class="rounded-full bg-accent px-3.5 py-2 text-sm font-medium text-accent-ink">Apply</button>
      <button type="button" class="rounded-full border border-line px-3.5 py-2 text-sm" @click="linkOpen = false">Cancel</button>
    </form>
    <EditorContent :id="id" :editor="editor" class="rich-text-editor" :style="{ minHeight: `${minHeight}px` }" />
  </div>
</template>

<script setup lang="ts">
import { nextTick, onBeforeUnmount, onMounted, ref, watch } from 'vue'
import { Editor, EditorContent } from '@tiptap/vue-3'
import StarterKit from '@tiptap/starter-kit'
import { Placeholder } from '@tiptap/extensions'
import { useFormField } from '~/composables/useFormField'

interface Props {
  modelValue: string
  placeholder?: string
  disabled?: boolean
  minHeight?: number
}

const props = withDefaults(defineProps<Props>(), {
  placeholder: '',
  disabled: false,
  minHeight: 320,
})

const emit = defineEmits<{ (e: 'update:modelValue', value: string): void }>()

const { id, invalid } = useFormField()
const editor = ref<Editor>()
const linkOpen = ref(false)
const linkUrl = ref('')
const linkInput = ref<HTMLInputElement | null>(null)

const chain = () => editor.value?.chain().focus()

const actions = [
  { label: 'Bold', icon: 'mdi:format-bold', run: () => chain()?.toggleBold().run(), active: () => editor.value?.isActive('bold') },
  { label: 'Italic', icon: 'mdi:format-italic', run: () => chain()?.toggleItalic().run(), active: () => editor.value?.isActive('italic') },
  {
    label: 'Bullet list',
    icon: 'mdi:format-list-bulleted',
    run: () => chain()?.toggleBulletList().run(),
    active: () => editor.value?.isActive('bulletList'),
  },
  {
    label: 'Numbered list',
    icon: 'mdi:format-list-numbered',
    run: () => chain()?.toggleOrderedList().run(),
    active: () => editor.value?.isActive('orderedList'),
  },
  { label: 'Link', icon: 'mdi:link-variant', run: openLink, active: () => editor.value?.isActive('link') },
  { label: 'Clear formatting', icon: 'mdi:format-clear', run: () => chain()?.unsetAllMarks().clearNodes().run() },
]

async function openLink() {
  linkUrl.value = editor.value?.getAttributes('link')['href'] ?? ''
  linkOpen.value = true
  await nextTick()
  linkInput.value?.focus()
}

function applyLink() {
  const href = linkUrl.value.trim()
  if (href) chain()?.extendMarkRange('link').setLink({ href }).run()
  else chain()?.extendMarkRange('link').unsetLink().run()
  linkOpen.value = false
}

onMounted(() => {
  editor.value = new Editor({
    content: props.modelValue,
    editable: !props.disabled,
    extensions: [StarterKit.configure({ link: { openOnClick: false } }), Placeholder.configure({ placeholder: props.placeholder })],
    onUpdate: ({ editor: instance }) => emit('update:modelValue', instance.getHTML()),
  })
})

onBeforeUnmount(() => editor.value?.destroy())

watch(
  () => props.modelValue,
  (value) => {
    if (editor.value && !editor.value.isFocused && value !== editor.value.getHTML()) editor.value.commands.setContent(value)
  },
)

watch(
  () => props.disabled,
  (disabled) => editor.value?.setEditable(!disabled),
)
</script>

<style>
.rich-text-editor .ProseMirror {
  min-height: inherit;
  padding: 18px 20px;
  outline: none;
  font-size: 16px;
  line-height: 1.7;
  color: var(--text);
  display: flex;
  flex-direction: column;
  gap: 14px;
}

.rich-text-editor .ProseMirror p {
  margin: 0;
}

.rich-text-editor .ProseMirror ul,
.rich-text-editor .ProseMirror ol {
  margin: 0;
  padding-left: 20px;
}

.rich-text-editor .ProseMirror ul {
  list-style: disc;
}

.rich-text-editor .ProseMirror ol {
  list-style: decimal;
}

.rich-text-editor .ProseMirror a {
  text-decoration: underline;
  text-decoration-color: var(--accent);
}

.rich-text-editor .ProseMirror p.is-editor-empty:first-child::before {
  content: attr(data-placeholder);
  color: var(--muted);
  float: left;
  height: 0;
  pointer-events: none;
}
</style>
