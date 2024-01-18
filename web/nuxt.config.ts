// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  devtools: { enabled: true },

  runtimeConfig: {
    public: {
      apiBase: '',
    }
  },

  modules: [
    '@nuxtjs/tailwindcss', 
    '@pinia/nuxt'
  ],

  // plugins: [
  //   '@/plugins/axios-config'
  // ],
})
