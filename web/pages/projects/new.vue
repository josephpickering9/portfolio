<template>
    <div class="w-full max-w-3xl border rounded-md px-6 py-4 prose space-y-5">
        <h1>New Project</h1>
        <label class="form-control w-full">
            <div class="label">
                <span class="label-text">Title</span>
            </div>
            <input v-model="title" type="text" class="input input-bordered w-full" />
        </label>
        <label class="form-control w-full">
            <div class="label">
                <span class="label-text">Description</span>
            </div>
            <TextEditor v-model="description" />
        </label>
        <label class="form-control w-full">
            <div class="label">
                <span class="label-text">Website</span>
            </div>
            <input v-model="website" type="text" class="input input-bordered w-full" />
        </label>
        <label class="form-control w-full">
            <div class="label">
                <span class="label-text">Tags</span>
            </div>
            <input type="text" class="input input-bordered w-full" />
        </label>
        <button class="btn btn-primary" @click="save">Save</button>
    </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import TextEditor from '../../components/forms/TextEditor.vue'
import { useProjectStore } from '../../store/ProjectStore'
import { useNotificationStore } from '../../store/NotificationStore'
import type { Project } from '../../api/models/Project';

interface Data {
    title: string
    description: string
    website: string
    tags: string[]
}

export default defineComponent({
    name: 'NewProject',
    components: {
        TextEditor,
    },
    data(): Data {
        return {
            title: '',
            description: '',
            website: '',
            tags: [],
        }
    },
    computed: {
        loading(): boolean {
            return useProjectStore().projectCreating
        },
        error(): string | undefined {
            return useProjectStore().projectCreateError
        }
    },
    methods: {
        async save() {
            const project: Project = {
                title: this.title,
                description: this.description,
                website: this.website,
            }

            const response = await useProjectStore().createProject(project)

            if (!this.error && response) {
                this.$router.push(`/projects/${response.id}`)
                useNotificationStore().displaySuccessNotification('Project created successfully')
            } else {
                useNotificationStore().displayErrorNotification(this.error || 'An error occurred')
            }
        }
    }
})
</script>