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
          <img
            v-if="project"
            :src="cardUrl ?? 'https://via.placeholder.com/320x200'"
            :alt="`${project.title} Card Image`"
            class="h-[230px] w-full object-cover"
          >
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

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import type { StyleValue } from 'vue'
import { useVuelidate } from '@vuelidate/core'
import { cloneDeep, sortBy } from 'lodash-es'
import { ImageType } from '@api'
import type { CreateProject, CreateProjectImage, Project } from '@api'
import { useProjectStore } from '~/store/ProjectStore'
import useValidation from '~/composables/useValidation'
import { getImageUrl } from '~/utils/image-helper'
import { defaultProjectForm } from '~/utils/default-helper'
import FormGroup from '~/components/ui/form/FormGroup.vue'
import FileInput from '~/components/ui/input/FileInput.vue'
import FileInputList from '~/components/ui/input/FileInputList.vue'
import Toggle from '~/components/ui/input/Toggle.vue'

interface Props {
  modelValue?: CreateProject
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: () => cloneDeep(defaultProjectForm),
})

// Emits
const emit = defineEmits<{
  'update:modelValue': [value: CreateProject]
}>()

const projectStore = useProjectStore()
const validation = useValidation()

const form = ref<CreateProject>(props.modelValue)
const desktop = ref<FileList | null>(null)
const mobile = ref<FileList | null>(null)
const logo = ref<FileList | null>(null)
const banner = ref<FileList | null>(null)
const card = ref<FileList | null>(null)
const logoUrl = ref<string | undefined>(undefined)
const bannerUrl = ref<string | undefined>(undefined)
const cardUrl = ref<string | undefined>(undefined)
const desktopUrls = ref<string[]>([])
const mobileUrls = ref<string[]>([])
const showMockup = ref(false)

// Validation
const v$ = useVuelidate()

const project = computed((): Project | undefined => {
  return projectStore.project
})

const loading = computed((): boolean => {
  return projectStore.projectCreating || projectStore.projectLoading
})

const headerStyle = computed((): StyleValue => {
  return {
    backgroundImage: `url(${bannerUrl.value ?? 'https://via.placeholder.com/1500x300'})`,
  }
})

const createProjectImageValue = computed((): CreateProjectImage[] => {
  const images: CreateProjectImage[] = []

  if (logo.value) {
    images.push({ type: ImageType.LOGO, image: logo.value.item(0) as Blob })
  } else if (logoUrl.value && project.value?.logo) {
    images.push({ id: project.value.logo.id, type: ImageType.LOGO })
  }

  if (banner.value) {
    images.push({ type: ImageType.BANNER, image: banner.value.item(0) as Blob })
  } else if (bannerUrl.value && project.value?.banner) {
    images.push({ id: project.value.banner.id, type: ImageType.BANNER })
  }

  if (card.value) {
    images.push({ type: ImageType.CARD, image: card.value.item(0) as Blob })
  } else if (cardUrl.value && project.value?.card) {
    images.push({ id: project.value.card.id, type: ImageType.CARD })
  }

  if (desktop.value?.length) {
    for (let i = 0; i < desktop.value.length; i++) {
      images.push({ type: ImageType.DESKTOP, image: desktop.value.item(i) as Blob })
    }
  }
  const desktops = project.value?.images.filter(
    (image) => image.type === ImageType.DESKTOP && desktopUrls.value.includes(getImageUrl(image.image)),
  )
  if (desktops) {
    desktops.forEach((image) =>
      images.push({
        id: image.id,
        type: ImageType.DESKTOP,
        order: desktopUrls.value.indexOf(getImageUrl(image.image)) + 1,
      }),
    )
  }

  if (mobile.value?.length) {
    for (let i = 0; i < mobile.value.length; i++) {
      images.push({ type: ImageType.MOBILE, image: mobile.value.item(i) as Blob })
    }
  }
  const mobiles = project.value?.images.filter(
    (image) => image.type === ImageType.MOBILE && mobileUrls.value.includes(getImageUrl(image.image)),
  )
  if (mobiles) {
    mobiles.forEach((image) =>
      images.push({
        id: image.id,
        type: ImageType.MOBILE,
        order: mobileUrls.value.indexOf(getImageUrl(image.image)) + 1,
      }),
    )
  }

  return images
})

async function validate(): Promise<boolean> {
  return await validation.validate(v$)
}

// Expose methods for parent component
defineExpose({
  validate,
})

onMounted(() => {
  if (project.value) {
    form.value.images = project.value.images.map((image) => {
      return {
        id: image.id,
        type: image.type,
        order: image.order,
      }
    })
  }
})

// Watch methods
watch(project, () => {
  if (!project.value) return

  logoUrl.value = project.value.logoUrl ? getImageUrl(project.value.logoUrl) : undefined
  bannerUrl.value = project.value.bannerUrl ? getImageUrl(project.value.bannerUrl) : undefined
  cardUrl.value = project.value.cardUrl ? getImageUrl(project.value.cardUrl) : undefined
  desktopUrls.value = sortBy(
    project.value.images.filter((image) => image.type === ImageType.DESKTOP),
    'order',
  ).map((image) => getImageUrl(image.image))
  mobileUrls.value = sortBy(
    project.value.images.filter((image) => image.type === ImageType.MOBILE),
    'order',
  ).map((image) => getImageUrl(image.image))
}, { immediate: true })

watch(() => props.modelValue, (newValue) => {
  form.value = newValue
}, { immediate: true })

watch(form, (newValue) => {
  emit('update:modelValue', newValue)
}, { deep: true })

watch([logo, banner, card, desktop, mobile, desktopUrls, mobileUrls], () => {
  form.value.images = createProjectImageValue.value
})
</script>

<style scoped>
.header-container {
  position: relative;
  margin-left: -1.5rem;
  margin-right: -1.5rem;
  margin-top: -1.5rem;
  display: flex;
  height: 10rem;
  flex-direction: column;
  justify-content: flex-end;
  background-size: cover;
  background-position: center;
  padding-top: 2rem;
  padding-bottom: 2rem;
}

.header-container::after {
  content: '';
  position: absolute;
  left: 0;
  top: 0;
  display: block;
  height: 100%;
  width: 100%;
  background-image: linear-gradient(to bottom, transparent, var(--fallback-b1, oklch(var(--b1))));
}

.header-container-light::after {
  --tw-gradient-to: rgb(156 163 175);
}

.header-container-dark::after {
  --tw-gradient-to: rgb(0 0 0);
}

.title {
  position: relative;
  margin: 0;
  font-size: 2.25rem;
  line-height: 2.5rem;
}
</style>
