<template>
  <FormElementContainer :label="label">
    <div class="textarea textarea-bordered w-full">
      <div v-if="editor" class="editor-buttons">
        <button
          :disabled="!editor.can().chain().focus().toggleBold().run()"
          :class="{ 'is-active': editor.isActive('bold') }"
          @click="editor.chain().focus().toggleBold().run()"
        >
          bold
        </button>
        <button
          :disabled="!editor.can().chain().focus().toggleItalic().run()"
          :class="{ 'is-active': editor.isActive('italic') }"
          @click="editor.chain().focus().toggleItalic().run()"
        >
          italic
        </button>
        <button :class="{ 'is-active': editor.isActive('paragraph') }" @click="editor.chain().focus().setParagraph().run()">
          paragraph
        </button>
        <button
          :class="{ 'is-active': editor.isActive('heading', { level: 1 }) }"
          @click="editor.chain().focus().toggleHeading({ level: 1 }).run()"
        >
          h1
        </button>
        <button
          :class="{ 'is-active': editor.isActive('heading', { level: 2 }) }"
          @click="editor.chain().focus().toggleHeading({ level: 2 }).run()"
        >
          h2
        </button>
        <button
          :class="{ 'is-active': editor.isActive('heading', { level: 3 }) }"
          @click="editor.chain().focus().toggleHeading({ level: 3 }).run()"
        >
          h3
        </button>
        <button
          :class="{ 'is-active': editor.isActive('heading', { level: 4 }) }"
          @click="editor.chain().focus().toggleHeading({ level: 4 }).run()"
        >
          h4
        </button>
        <button
          :class="{ 'is-active': editor.isActive('heading', { level: 5 }) }"
          @click="editor.chain().focus().toggleHeading({ level: 5 }).run()"
        >
          h5
        </button>
        <button
          :class="{ 'is-active': editor.isActive('heading', { level: 6 }) }"
          @click="editor.chain().focus().toggleHeading({ level: 6 }).run()"
        >
          h6
        </button>
        <button
          :class="{ 'is-active': editor.isActive('bulletList') }"
          @click="editor.chain().focus().toggleBulletList().run()"
        >
          bullet list
        </button>
        <button
          :class="{ 'is-active': editor.isActive('orderedList') }"
          @click="editor.chain().focus().toggleOrderedList().run()"
        >
          ordered list
        </button>
        <button
          :disabled="!editor.can().chain().focus().toggleCode().run()"
          :class="{ 'is-active': editor.isActive('code') }"
          @click="editor.chain().focus().toggleCode().run()"
        >
          code
        </button>
        <button
          :class="{ 'is-active': editor.isActive('codeBlock') }"
          @click="editor.chain().focus().toggleCodeBlock().run()"
        >
          code block
        </button>
        <button
          :class="{ 'is-active': editor.isActive('blockquote') }"
          @click="editor.chain().focus().toggleBlockquote().run()"
        >
          blockquote
        </button>
        <button @click="editor.chain().focus().setHorizontalRule().run()">horizontal rule</button>
      </div>
      <EditorContent :editor="editor" />
    </div>
  </FormElementContainer>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { Editor, EditorContent } from '@tiptap/vue-3'
import StarterKit from '@tiptap/starter-kit'
import FormElementContainer from './FormElementContainer.vue'

interface Data {
  editor: Editor | null
}

export default defineComponent({
  name: 'TextEditor',
  components: {
    EditorContent,
    FormElementContainer,
  },
  props: {
    label: {
      type: String,
      default: null,
    },
    modelValue: {
      type: String,
      required: true,
    },
  },
  emits: ['update:modelValue'],
  data(): Data {
    return {
      editor: null,
    }
  },
  mounted() {
    // @ts-ignore
    this.editor = new Editor({
      extensions: [StarterKit],
      onUpdate: ({ editor }) => {
        this.$emit('update:modelValue', editor.getHTML())
      },
    })
  },
  beforeUnmount() {
    if (this.editor) {
      this.editor.destroy()
      this.editor = null
    }
  },
})
</script>

<style scoped>
.editor-buttons {
  @apply flex flex-wrap gap-2;
}

.editor-buttons button {
  @apply btn btn-outline btn-xs;
}

.editor-buttons button.is-active {
  @apply btn-active;
}

.tiptap {
  > * + * {
    margin-top: 0.75em;
  }

  ul,
  ol {
    padding: 0 1rem;
  }

  h1,
  h2,
  h3,
  h4,
  h5,
  h6 {
    line-height: 1.1;
  }

  code {
    background-color: rgba(#616161, 0.1);
    color: #616161;
  }

  pre {
    background: #0d0d0d;
    color: #fff;
    font-family: 'JetBrainsMono', monospace;
    padding: 0.75rem 1rem;
    border-radius: 0.5rem;

    code {
      color: inherit;
      padding: 0;
      background: none;
      font-size: 0.8rem;
    }
  }

  img {
    max-width: 100%;
    height: auto;
  }

  blockquote {
    padding-left: 1rem;
    border-left: 2px solid rgba(#0d0d0d, 0.1);
  }

  hr {
    border: none;
    border-top: 2px solid rgba(#0d0d0d, 0.1);
    margin: 2rem 0;
  }
}

.tiptap:focus {
  outline: none;
}
</style>
