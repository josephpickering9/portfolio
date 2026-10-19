<template>
  <div>
    <HeroSection />
    <FeaturedProjects />
    <ThisSiteCard />
    <ContactCta />
  </div>
</template>

<script setup lang="ts">
import useMeta from '~/composables/useMeta'
import { usePersonDetails, useStructuredData } from '~/composables/useStructuredData'
import useProjectsData from '~/composables/useProjectsData'
import HeroSection from '~/components/home/HeroSection.vue'
import FeaturedProjects from '~/components/home/FeaturedProjects.vue'
import ThisSiteCard from '~/components/home/ThisSiteCard.vue'
import ContactCta from '~/components/home/ContactCta.vue'
import { buildPersonJsonLd } from '~/utils/structured-data-helper'

const { updateMeta } = useMeta()
const projectsData = useProjectsData()
const person = usePersonDetails()

if (import.meta.server) await projectsData

updateMeta({
  title: 'Home',
  description: 'Joseph Pickering, full stack developer. Selected work, case studies and an AI search over my projects.',
})

useStructuredData(() => buildPersonJsonLd(person.value))
</script>
