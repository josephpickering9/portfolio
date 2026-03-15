<template>
  <div>
    <FormElementContainer :label="label" />
    <div class="textarea textarea-bordered min-h-[20rem] w-full space-y-4">
      <div v-if="editor" class="flex flex-wrap gap-2">
        <button
          :disabled="!editor.can().chain().focus().toggleBold().run()"
          class="btn btn-outline btn-xs"
          :class="{ 'btn-active': editor.isActive('bold') }"
          @click="editor.chain().focus().toggleBold().run()"
        >
          bold
        </button>
        <button
          :disabled="!editor.can().chain().focus().toggleItalic().run()"
          class="btn btn-outline btn-xs"
          :class="{ 'btn-active': editor.isActive('italic') }"
          @click="editor.chain().focus().toggleItalic().run()"
        >
          italic
        </button>
        <button class="btn btn-outline btn-xs" :class="{ 'btn-active': editor.isActive('paragraph') }" @click="editor.chain().focus().setParagraph().run()">
          paragraph
        </button>
        <button
          class="btn btn-outline btn-xs"
          :class="{ 'btn-active': editor.isActive('heading', { level: 1 }) }"
          @click="editor.chain().focus().toggleHeading({ level: 1 }).run()"
        >
          h1
        </button>
        <button
          class="btn btn-outline btn-xs"
          :class="{ 'btn-active': editor.isActive('heading', { level: 2 }) }"
          @click="editor.chain().focus().toggleHeading({ level: 2 }).run()"
        >
          h2
        </button>
        <button
          class="btn btn-outline btn-xs"
          :class="{ 'btn-active': editor.isActive('heading', { level: 3 }) }"
          @click="editor.chain().focus().toggleHeading({ level: 3 }).run()"
        >
          h3
        </button>
        <button
          class="btn btn-outline btn-xs"
          :class="{ 'btn-active': editor.isActive('heading', { level: 4 }) }"
          @click="editor.chain().focus().toggleHeading({ level: 4 }).run()"
        >
          h4
        </button>
        <button
          class="btn btn-outline btn-xs"
          :class="{ 'btn-active': editor.isActive('heading', { level: 5 }) }"
          @click="editor.chain().focus().toggleHeading({ level: 5 }).run()"
        >
          h5
        </button>
        <button
          class="btn btn-outline btn-xs"
          :class="{ 'btn-active': editor.isActive('heading', { level: 6 }) }"
          @click="editor.chain().focus().toggleHeading({ level: 6 }).run()"
        >
          h6
        </button>
        <button
          class="btn btn-outline btn-xs"
          :class="{ 'btn-active': editor.isActive('bulletList') }"
          @click="editor.chain().focus().toggleBulletList().run()"
        >
          bullet list
        </button>
        <button
          class="btn btn-outline btn-xs"
          :class="{ 'btn-active': editor.isActive('orderedList') }"
          @click="editor.chain().focus().toggleOrderedList().run()"
        >
          ordered list
        </button>
        <button
          :disabled="!editor.can().chain().focus().toggleCode().run()"
          class="btn btn-outline btn-xs"
          :class="{ 'btn-active': editor.isActive('code') }"
          @click="editor.chain().focus().toggleCode().run()"
        >
          code
        </button>
        <button
          class="btn btn-outline btn-xs"
          :class="{ 'btn-active': editor.isActive('codeBlock') }"
          @click="editor.chain().focus().toggleCodeBlock().run()"
        >
          code block
        </button>
        <button
          class="btn btn-outline btn-xs"
          :class="{ 'btn-active': editor.isActive('blockquote') }"
          @click="editor.chain().focus().toggleBlockquote().run()"
        >
          blockquote
        </button>
        <button class="btn btn-outline btn-xs" @click="editor.chain().focus().setHorizontalRule().run()">horizontal rule</button>
      </div>
      <EditorContent :editor="editor" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted, onBeforeUnmount } from 'vue'
import { Editor, EditorContent } from '@tiptap/vue-3'
import StarterKit from '@tiptap/starter-kit'
import FormElementContainer from '~/components/ui/form/FormElementContainer.vue'

interface Props {
  label?: string | null
  modelValue: string
  placeholder?: string | null
  disabled?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  label: undefined,
  placeholder: undefined,
})

const emit = defineEmits<{
  'update:modelValue': [value: string]
}>()

const editor = ref<Editor | undefined>(undefined)

onMounted(() => {
  editor.value = new Editor({
    content: props.modelValue,
    extensions: [StarterKit],
    editable: !props.disabled,
    onUpdate: ({ editor }) => {
      emit('update:modelValue', editor.getHTML())
    },
  })
})

onBeforeUnmount(() => {
  if (editor.value) {
    editor.value.destroy()
    editor.value = undefined
  }
})


watch(() => props.modelValue, (value) => {
  if (editor.value && !editor.value.isFocused) {
    editor.value.commands.setContent(value)
  }
})
</script>

<style scoped>
.tiptap > * + * {
  margin-top: 0.75em;
}

.tiptap ul,
.tiptap ol {
  padding: 0 1rem;
}

.tiptap h1,
.tiptap h2,
.tiptap h3,
.tiptap h4,
.tiptap h5,
.tiptap h6 {
  line-height: 1.1;
}

.tiptap p {
  margin: 1em 0;
}

.tiptap code {
  background-color: oklch(var(--bc) / 0.1);
  color: oklch(var(--bc) / 0.7);
  border-radius: 0.25rem;
  padding: 0.1em 0.3em;
}

.tiptap pre {
  background: oklch(var(--n));
  color: oklch(var(--nc));
  font-family: 'JetBrainsMono', monospace;
  padding: 0.75rem 1rem;
  border-radius: 0.5rem;
}

.tiptap pre code {
  color: inherit;
  padding: 0;
  background: none;
  font-size: 0.8rem;
}

.tiptap img {
  max-width: 100%;
  height: auto;
}

.tiptap blockquote {
  padding-left: 1rem;
  border-left: 2px solid oklch(var(--bc) / 0.15);
}

.tiptap hr {
  border: none;
  border-top: 2px solid oklch(var(--bc) / 0.15);
  margin: 2rem 0;
}
</style>

<style>
.ProseMirror-focused {
  outline: none;
}
</style>
