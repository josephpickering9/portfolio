<template>
  <div v-if="form" class="relative flex flex-col gap-8 pb-4">
    <div class="header-container header-container-dark" :style="headerStyle" />

    <div class="flex gap-6 md:flex-row">
      <div class="flex flex-col md:w-1/2 md:gap-20">
        <FormGroup :errors="v$.banner?.$errors" name="Banner">
          <FileInput v-model:image-url="bannerUrl" label="Banner" :disabled="loading" @update:file="banner = $event" />
        </FormGroup>
        <FormGroup :errors="v$.logo?.$errors" name="Logo">
          <FileInput v-model:image-url="logoUrl" label="Logo" :disabled="loading" @update:file="logo = $event" />
        </FormGroup>
      </div>
      <div class="card card-bordered w-full bg-base-100 shadow-xl md:w-1/2">
        <figure class="m-0">
          <img :src="cardUrl ?? 'https://via.placeholder.com/320x200'" class="h-[230px] w-full object-cover" />
        </figure>
        <div class="card-body px-6 py-6">
          <FormGroup :errors="v$.card?.$errors" name="Card">
            <FileInput v-model:image-url="cardUrl" label="Card" :disabled="loading" @update:file="card = $event" />
          </FormGroup>
        </div>
      </div>
    </div>
    <div class="flex gap-4 md:flex-row">
      <FormGroup :errors="v$.desktop?.$errors" name="Desktop">
        <FileInputList
          v-model:image-urls="desktopUrls"
          label="Desktop"
          :disabled="loading"
          :multiple="true"
          @update:file="desktop = $event"
        />
      </FormGroup>
    </div>
    <div class="flex gap-4 md:flex-row">
      <FormGroup :errors="v$.mobile?.$errors" name="Mobile">
        <FileInputList
          v-model:image-urls="mobileUrls"
          label="Mobile"
          :disabled="loading"
          :multiple="true"
          @update:file="mobile = $event"
        />
      </FormGroup>
    </div>
    <div class="flex">
      <FormGroup :errors="v$.showMockup?.$errors" name="Show Mockup">
        <Toggle v-model="showMockup" label="Show Mockup" :disabled="loading" />
      </FormGroup>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, type PropType, type StyleValue } from 'vue'
import { useVuelidate } from '@vuelidate/core'
import { cloneDeep, sortBy } from 'lodash-es'
import { ImageType } from '../../../api'
import type { CreateProject, CreateProjectImage, Project } from '../../../api'
import { useProjectStore } from '../../../store/ProjectStore'
import FormGroup from '../FormGroup.vue'
import FileInput from '../FileInput.vue'
import FileInputList from '../FileInputList.vue'
import Toggle from '../Toggle.vue'
import useValidation from '../../../composables/useValidation'
import { getImageUrl } from '../../../utils/image-helper'
import { defaultProjectForm } from '../../../mocks/Defaults'

interface Data {
  form: CreateProject
  desktop: FileList | null
  mobile: FileList | null
  logo: FileList | null
  banner: FileList | null
  card: FileList | null
  logoUrl?: string
  bannerUrl?: string
  cardUrl?: string
  desktopUrls: string[]
  mobileUrls: string[]
  showMockup: boolean
}

export default defineComponent({
  name: 'ProjectImagesForm',
  components: { FormGroup, FileInput, FileInputList, Toggle },
  props: {
    modelValue: {
      type: Object as PropType<CreateProject>,
      default: () => cloneDeep(defaultProjectForm),
    },
  },
  emits: ['update:modelValue'],
  setup() {
    return { v$: useVuelidate() }
  },
  data(): Data {
    return {
      form: this.modelValue,
      logo: null,
      banner: null,
      card: null,
      desktop: null,
      mobile: null,
      logoUrl: undefined,
      bannerUrl: undefined,
      cardUrl: undefined,
      desktopUrls: [],
      mobileUrls: [],
      showMockup: false,
    }
  },
  computed: {
    project(): Project | undefined {
      return useProjectStore().project
    },
    loading(): boolean {
      return useProjectStore().projectCreating || useProjectStore().projectLoading
    },
    headerStyle(): StyleValue {
      return {
        backgroundImage: `url(${this.bannerUrl ?? 'https://via.placeholder.com/1500x300'})`,
      }
    },
    createProjectImageValue(): CreateProjectImage[] {
      const images: CreateProjectImage[] = []

      if (this.logo) {
        images.push({ type: ImageType.LOGO, image: this.logo.item(0) as Blob })
      } else if (this.logoUrl && this.project?.logo) {
        images.push({ id: this.project.logo.id, type: ImageType.LOGO })
      }

      if (this.banner) {
        images.push({ type: ImageType.BANNER, image: this.banner.item(0) as Blob })
      } else if (this.bannerUrl && this.project?.banner) {
        images.push({ id: this.project.banner.id, type: ImageType.BANNER })
      }

      if (this.card) {
        images.push({ type: ImageType.CARD, image: this.card.item(0) as Blob })
      } else if (this.cardUrl && this.project?.card) {
        images.push({ id: this.project.card.id, type: ImageType.CARD })
      }

      if (this.desktop?.length) {
        for (let i = 0; i < this.desktop.length; i++) {
          images.push({ type: ImageType.DESKTOP, image: this.desktop.item(i) as Blob })
        }
      }
      const desktops = this.project?.images.filter(
        (image) => image.type === ImageType.DESKTOP && this.desktopUrls.includes(getImageUrl(image.image)),
      )
      if (desktops) {
        desktops.forEach((image) =>
          images.push({
            id: image.id,
            type: ImageType.DESKTOP,
            order: this.desktopUrls.indexOf(getImageUrl(image.image)) + 1,
          }),
        )
      }

      if (this.mobile?.length) {
        for (let i = 0; i < this.mobile.length; i++) {
          images.push({ type: ImageType.MOBILE, image: this.mobile.item(i) as Blob })
        }
      }
      const mobiles = this.project?.images.filter(
        (image) => image.type === ImageType.MOBILE && this.mobileUrls.includes(getImageUrl(image.image)),
      )
      if (mobiles) {
        mobiles.forEach((image) =>
          images.push({
            id: image.id,
            type: ImageType.MOBILE,
            order: this.mobileUrls.indexOf(getImageUrl(image.image)) + 1,
          }),
        )
      }

      return images
    },
  },
  mounted() {
    if (this.project) {
      this.form.images = this.project.images.map((image) => {
        return {
          id: image.id,
          type: image.type,
          order: image.order,
        }
      })
    }
  },
  methods: {
    async validate(): Promise<boolean> {
      return await useValidation().validate(this.v$)
    },
  },
  watch: {
    project: {
      handler() {
        if (!this.project) return

        this.logoUrl = this.project.logoUrl ? getImageUrl(this.project.logoUrl) : undefined
        this.bannerUrl = this.project.bannerUrl ? getImageUrl(this.project.bannerUrl) : undefined
        this.cardUrl = this.project.cardUrl ? getImageUrl(this.project.cardUrl) : undefined
        this.desktopUrls = sortBy(
          this.project.images.filter((image) => image.type === ImageType.DESKTOP),
          'order',
        ).map((image) => getImageUrl(image.image))
        this.mobileUrls = sortBy(
          this.project.images.filter((image) => image.type === ImageType.MOBILE),
          'order',
        ).map((image) => getImageUrl(image.image))
      },
      immediate: true,
    },
    modelValue: {
      handler(value: CreateProject) {
        this.form = value
      },
      immediate: true,
    },
    form: {
      handler(value: CreateProject) {
        this.$emit('update:modelValue', value)
      },
      deep: true,
    },
    logo() {
      this.form.images = this.createProjectImageValue
    },
    banner() {
      this.form.images = this.createProjectImageValue
    },
    card() {
      this.form.images = this.createProjectImageValue
    },
    desktop() {
      this.form.images = this.createProjectImageValue
    },
    mobile() {
      this.form.images = this.createProjectImageValue
    },
    desktopUrls() {
      this.form.images = this.createProjectImageValue
    },
    mobileUrls() {
      this.form.images = this.createProjectImageValue
    },
  },
})
</script>

<style scoped>
.header-container {
  @apply relative -mx-6 -mt-6 flex h-40 flex-col justify-end bg-cover bg-center py-8;
}

.header-container::after {
  content: '';
  @apply absolute left-0 top-0 block h-full w-full bg-gradient-to-b from-transparent;
}

.header-container-light::after {
  @apply to-gray-400;
}

.header-container-dark::after {
  @apply to-black;
}

.title {
  @apply relative m-0 text-4xl;
}
</style>
