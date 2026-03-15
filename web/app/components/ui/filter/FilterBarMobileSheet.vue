<template>
  <Teleport to="body">
    <Transition name="sheet-backdrop">
      <div v-if="isOpen" class="fixed inset-0 z-50">
        <Transition name="sheet-panel">
          <div v-if="isOpen" class="absolute inset-0 bg-base-100 flex flex-col">
            <div
              class="flex items-center justify-between px-4 flex-shrink-0"
              style="padding-top: max(env(safe-area-inset-top, 0px), 16px); padding-bottom: 12px;"
            >
              <span class="font-semibold text-base">
                {{ activeSubmenu ? submenuTitle : 'Filter' }}
              </span>
              <button type="button" class="btn btn-sm btn-ghost btn-circle" @click="$emit('close')">
                <Icon name="heroicons:x-mark" size="1.2em" />
              </button>
            </div>

            <div class="divider my-0 flex-shrink-0" />

            <div
              v-if="activeSubmenu"
              class="flex flex-col flex-1 overflow-hidden animate-slide-in-right"
              style="padding-bottom: env(safe-area-inset-bottom, 0px);"
            >
              <div class="px-3 pt-2 flex-shrink-0">
                <button
                  class="btn btn-sm btn-ghost gap-2 justify-start text-base-content/60 hover:text-base-content w-full"
                  @click="$emit('submenu:close')"
                >
                  <Icon name="heroicons:chevron-left" size="1em" />
                  Back
                </button>
              </div>

              <div class="flex-1 overflow-y-auto px-2 pb-2">
                <CompanyFilterList
                  v-if="activeSubmenu === FilterType.COMPANY"
                  mobile
                  @select="$emit('company:select', $event)"
                  @close="$emit('close')"
                />
                <TagTypeFilterList
                  v-else-if="activeSubmenu === FilterType.TAG_TYPE"
                  mobile
                  @select="$emit('tagType:select', $event)"
                  @close="$emit('close')"
                />
                <TagFilterList
                  v-else-if="activeSubmenu === FilterType.TAG"
                  mobile
                  @select="$emit('tag:select', $event)"
                  @close="$emit('close')"
                />
              </div>
            </div>

            <div
              v-else
              class="flex flex-col flex-1 overflow-hidden"
              style="padding-bottom: env(safe-area-inset-bottom, 0px);"
            >
              <div class="px-4 pb-3 pt-1 flex-shrink-0">
                <TextInput
                  ref="mobileSearchInputRef"
                  :model-value="searchValue"
                  placeholder="Search projects..."
                  size="md"
                  @update:model-value="$emit('update:searchValue', $event)"
                />
              </div>

              <div class="flex-1 overflow-y-auto px-2 pb-2">
                <button
                  v-for="filterType in availableFilterTypes"
                  :key="filterType.value"
                  type="button"
                  class="btn btn-ghost w-full justify-start gap-4 font-normal h-auto py-4 text-base"
                  @click="$emit('submenu:open', filterType.value)"
                >
                  <Icon :name="filterType.icon" size="1.4em" class="text-base-content/70" />
                  <span class="flex-1 text-left">{{ filterType.label }}</span>
                  <Icon name="heroicons:chevron-right" size="1.1em" class="opacity-40" />
                </button>
              </div>
            </div>
          </div>
        </Transition>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
import { ref, nextTick } from 'vue'
import { FilterType } from '~/types/FilterType'
import type { TagTypeValue } from '~/types/TagTypeValue'
import TextInput from '~/components/ui/input/TextInput.vue'
import CompanyFilterList from '~/components/company/filter/CompanyFilterList.vue'
import TagTypeFilterList from '~/components/tag/filter/TagTypeFilterList.vue'
import TagFilterList from '~/components/tag/filter/TagFilterList.vue'

interface FilterTypeItem {
  value: string
  label: string
  icon: string
}

interface Props {
  isOpen: boolean
  activeSubmenu: string | null
  searchValue: string
  availableFilterTypes: FilterTypeItem[]
  submenuTitle: string
  filtersCount: number
}

defineProps<Props>()

const emit = defineEmits<{
  'close': []
  'update:searchValue': [value: string]
  'submenu:open': [filterType: string]
  'submenu:close': []
  'company:select': [companyId: string]
  'tagType:select': [tagType: TagTypeValue]
  'tag:select': [tagId: string]
}>()

const mobileSearchInputRef = ref<InstanceType<typeof TextInput> | null>(null)

function focusSearch() {
  nextTick(() => mobileSearchInputRef.value?.focus())
}

defineExpose({ focusSearch })
</script>

<style scoped>
@keyframes slideInRight {
  from { opacity: 0; transform: translateX(20px); }
  to { opacity: 1; transform: translateX(0); }
}

.animate-slide-in-right {
  animation: slideInRight 0.2s ease-out forwards;
}

.sheet-backdrop-enter-active,
.sheet-backdrop-leave-active {
  transition: opacity 0.2s ease;
}
.sheet-backdrop-enter-from,
.sheet-backdrop-leave-to {
  opacity: 0;
}

.sheet-panel-enter-active,
.sheet-panel-leave-active {
  transition: transform 0.28s cubic-bezier(0.32, 0.72, 0, 1);
}
.sheet-panel-enter-from,
.sheet-panel-leave-to {
  transform: translateX(100%);
}
</style>
