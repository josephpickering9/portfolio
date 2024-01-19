<template>
    <div class="flex flex-col min-h-screen">
        <HeaderBar />
        <main class="container mx-auto flex flex-col flex-grow">
            <div class="flex flex-grow justify-center items-center py-4">
                <slot />
            </div>
        </main>
        <FooterBar />
    </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import HeaderBar from '../components/navigation/HeaderBar.vue'
import FooterBar from '../components/navigation/FooterBar.vue'

export default defineComponent({
    name: 'DefaultLayout',
    components: {
        HeaderBar,
        FooterBar
    }
})
</script>
