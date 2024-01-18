<template>
    <div class="flex flex-col h-screen">
        <HeaderBar />
        <main class="container mx-auto flex flex-col flex-grow">
            <slot />
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
