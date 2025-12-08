<template>
  <div v-if="form" class="flex flex-col gap-4 pb-4">
    <Tabs v-model:active-tab="activeTab" :tabs="tabs">
      <template #default="{ index }">
        <div v-if="index === 0" class="flex flex-col gap-4">
          <div
            class="relative h-64 w-full overflow-hidden rounded-xl bg-base-200 shadow-sm"
            :style="bannerStyle"
          >
            <div v-if="!bannerUrl" class="flex h-full items-center justify-center text-base-content/30">
              <Icon name="material-symbols:image" size="4em" />
            </div>
          </div>
          <FormGroup :errors="v$['banner']?.$errors">
            <FileInput v-model:image-url="bannerUrl" label="Select Banner" :disabled="loading" @update:file="banner = $event" />
          </FormGroup>
        </div>

        <div v-if="index === 1" class="flex flex-col gap-4">
          <div class="flex items-center gap-6">
            <div class="relative flex h-24 w-24 flex-shrink-0 items-center justify-center overflow-hidden rounded-full bg-base-200 shadow-sm ring-1 ring-base-content/10">
              <img v-if="logoUrl" :src="logoUrl" alt="Logo Preview" class="h-full w-full object-cover">
              <Icon v-else name="material-symbols:image" size="2em" class="text-base-content/30" />
            </div>
            <div class="flex-grow">
              <FormGroup :errors="v$['logo']?.$errors">
                <FileInput v-model:image-url="logoUrl" label="Select Logo" :disabled="loading" @update:file="logo = $event" />
              </FormGroup>
            </div>
          </div>
        </div>

        <div v-if="index === 2" class="flex flex-col gap-4">
          <div class="card card-compact card-bordered w-full bg-base-100 shadow-sm transition-all hover:shadow-md">
            <figure class="m-0 aspect-video w-full bg-base-200">
              <img v-if="cardUrl" :src="cardUrl" alt="Card Preview" class="h-full w-full object-cover">
              <div v-else class="flex h-full items-center justify-center text-base-content/30">
                <Icon name="material-symbols:image" size="3em" />
              </div>
            </figure>
            <div class="card-body">
              <strong class="card-title text-sm">{{ form.title || 'Project Title' }}</strong>
              <p class="text-xs text-base-content/60 line-clamp-2">{{ form.shortDescription || 'Short description preview will appear here...' }}</p>
            </div>
          </div>
          <FormGroup :errors="v$['card']?.$errors">
            <FileInput v-model:image-url="cardUrl" label="Select Card Image" :disabled="loading" @update:file="card = $event" />
          </FormGroup>
        </div>

        <div v-if="index === 3" class="flex flex-col gap-10">
          <div class="grid gap-10 md:grid-cols-2">
            <FormGroup :errors="v$['desktop']?.$errors" name="Desktop Screenshots">
              <FileInputList
                v-model:image-urls="desktopUrls"
                label="Desktop Images"
                :disabled="loading"
                :multiple="true"
                @update:file="desktop = $event"
              />
            </FormGroup>

            <FormGroup :errors="v$['mobile']?.$errors" name="Mobile Screenshots">
              <FileInputList
                v-model:image-urls="mobileUrls"
                label="Mobile Images"
                :disabled="loading"
                :multiple="true"
                @update:file="mobile = $event"
              />
            </FormGroup>
          </div>

          <div class="rounded-lg bg-base-200/50 p-6">
            <FormGroup :errors="v$['showMockup']?.$errors" name="Display Options">
              <Toggle v-model="showMockup" label="Show Device Mockup on Details Page" :disabled="loading" />
            </FormGroup>
          </div>
        </div>
      </template>
    </Tabs>
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
import Tabs from '~/components/layout/Tabs.vue'
import { Icon } from '#components'

interface Props {
  modelValue?: CreateProject
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: () => cloneDeep(defaultProjectForm),
})


const emit = defineEmits<{
  'update:modelValue': [value: CreateProject]
}>()

const projectStore = useProjectStore()
const validation = useValidation()
const v$ = useVuelidate()

const tabs = ['Banner', 'Logo', 'Card', 'Gallery']
const activeTab = ref(0)
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

const project = computed((): Project | undefined => {
  return projectStore.project
})

const loading = computed((): boolean => {
  return projectStore.projectCreating || projectStore.projectLoading
})

const bannerStyle = computed((): StyleValue => {
  if (!bannerUrl.value) return {}
  return {
    backgroundImage: `url(${bannerUrl.value})`,
    backgroundSize: 'cover',
    backgroundPosition: 'center',
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
  return await validation.validate(v$.value)
}

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
