import { ref, computed, watch, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { isEmpty, orderBy } from 'lodash-es'
import { useCompanyStore } from '~/store/CompanyStore'
import { useTagStore } from '~/store/TagStore'
import type { Project } from '@api'
import type { Filter } from '~/types/Filter'
import { FilterType } from '~/types/FilterType'
import { tagTypes } from '~/data/TagTypes'

export function useProjectFilters(projects: () => Project[], setProjects: () => Project[], modelSearch: () => string | undefined) {
  const route = useRoute()
  const router = useRouter()
  const companyStore = useCompanyStore()
  const tagStore = useTagStore()

  const filters = ref<Filter[]>([])
  const sortColumn = ref<string | null>('dateRange')
  const sortDirection = ref<'asc' | 'desc'>('desc')

  const companies = computed(() => companyStore.companies)
  const tags = computed(() => tagStore.tags)

  const filteredProjects = computed((): Project[] => {
    let result: Project[] = setProjects().length ? [...setProjects()] : [...projects()]

    filters.value.forEach(filter => {
      if (filter.type === FilterType.SEARCH && !isEmpty(filter.value)) {
        result = result.filter(p =>
          p.title.toLowerCase().includes(filter.value.toLowerCase()) ||
          p.shortDescription.toLowerCase().includes(filter.value.toLowerCase()) ||
          p.description.toLowerCase().includes(filter.value.toLowerCase())
        )
      } else if (filter.type === FilterType.COMPANY) {
        result = result.filter(p => p.companyId === filter.value)
      } else if (filter.type === FilterType.TAG_TYPE) {
        result = result.filter(p => p.tags.some(t => t.type === filter.value))
      } else if (filter.type === FilterType.TAG) {
        result = result.filter(p => p.tags.some(t => t.id === filter.value))
      }
    })

    const search = modelSearch()
    if (!isEmpty(search) && (search?.length ?? 0) > 3) {
      result = result.filter(p =>
        search && (
          p.title.toLowerCase().includes(search.toLowerCase()) ||
          p.shortDescription.toLowerCase().includes(search.toLowerCase()) ||
          p.description.toLowerCase().includes(search.toLowerCase())
        )
      )
    }

    if (tags.value.length > 0) {
      const tagTitles = tags.value.map(t => t.title)
      result = result.filter(p => p.tags.some(t => tagTitles.includes(t.title)))
    }

    return result
  })

  const sortedProjects = computed((): Project[] => {
    if (!sortColumn.value) return [...filteredProjects.value]

    return orderBy(
      filteredProjects.value,
      [(p) => {
        if (sortColumn.value === 'project') return p.title.toLowerCase()
        if (sortColumn.value === 'dateRange') return new Date(p.startDate).getTime()
        return ''
      }],
      [sortDirection.value]
    )
  })

  function handleSort(column: string) {
    sortDirection.value = sortColumn.value === column && sortDirection.value === 'asc' ? 'desc' : 'asc'
    sortColumn.value = column
  }

  function loadFiltersFromQuery() {
    const newFilters: Filter[] = []

    const search = route.query['search']?.toString()
    if (search) newFilters.push({ type: FilterType.SEARCH, value: search, label: 'Search' })

    const company = route.query['company']?.toString()
    if (company) {
      const companyObj = companies.value.find(c => c.id === company)
      newFilters.push({ type: FilterType.COMPANY, value: company, label: 'Company', displayValue: companyObj?.name, logo: companyObj?.logo })
    }

    const tagType = route.query['type']?.toString()
    if (tagType) {
      const tagTypeValue = tagTypes.find(t => t.value === tagType)
      if (tagTypeValue) newFilters.push({ type: FilterType.TAG_TYPE, value: tagTypeValue.value, label: 'Tag Type', displayValue: tagTypeValue.label, icon: tagTypeValue.icon })
    }

    const tag = route.query['tag']?.toString()
    if (tag) {
      const tagObj = tags.value.find(t => t.id === tag)
      newFilters.push({ type: FilterType.TAG, value: tag, label: 'Tag', displayValue: tagObj?.title, icon: tagObj?.icon })
    }

    filters.value = newFilters
  }

  function updateQueryParams() {
    const query: Record<string, string | undefined> = {}

    filters.value.forEach(filter => {
      if (filter.type === FilterType.SEARCH) query['search'] = filter.value
      else if (filter.type === FilterType.COMPANY) query['company'] = filter.value
      else if (filter.type === FilterType.TAG_TYPE) query['type'] = filter.displayValue || filter.value
      else if (filter.type === FilterType.TAG) query['tag'] = filter.value
    })

    router.push({ path: route.path, query })
  }

  watch(companies, () => {
    filters.value = filters.value.map(filter => {
      if (filter.type === FilterType.COMPANY && !filter.displayValue) {
        const companyObj = companies.value.find(c => c.id === filter.value)
        if (companyObj) return { ...filter, displayValue: companyObj.name, logo: companyObj.logo }
      }
      return filter
    })
  })

  watch(tags, () => {
    filters.value = filters.value.map(filter => {
      if (filter.type === FilterType.TAG && !filter.displayValue) {
        const tagObj = tags.value.find(t => t.id === filter.value)
        if (tagObj) return { ...filter, displayValue: tagObj.title, icon: tagObj.icon }
      }
      return filter
    })
  })

  watch(() => route.query, loadFiltersFromQuery)

  watch(filters, updateQueryParams, { deep: true })

  onMounted(async () => {
    loadFiltersFromQuery()
    if (companies.value.length === 0) await companyStore.getCompanies()
    if (tags.value.length === 0) await tagStore.getTags()
  })

  return { filters, sortColumn, sortDirection, filteredProjects, sortedProjects, handleSort }
}
