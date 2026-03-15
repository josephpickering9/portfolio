<template>
  <div ref="containerRef" class="relative w-full">
    <FormElementContainer :label="label">
      <div class="relative">
        <button
          type="button"
          class="select w-full"
          :class="selectClass"
          :disabled="disabled"
          @click="toggle"
        >
          <span class="flex items-center gap-2 truncate" :class="{ 'text-gray-500 dark:text-gray-400': !selectedOption }">
          <Icon 
            v-if="selectedOption?.icon" 
            :name="selectedOption.icon" 
            class="h-5 w-5 flex-shrink-0" 
            :class="selectedOption.iconClass || 'text-gray-400'" 
          />
          {{ selectedOption ? selectedOption.text : placeholder || 'Select an option' }}
        </span>
        </button>

        <button
          v-if="selectedOption && clearable && !disabled"
          type="button"
          class="absolute cursor-pointer right-2 top-1/2 -translate-y-1/2 p-1 text-gray-400 hover:text-gray-600 dark:hover:text-gray-300"
          @click.stop="clear"
        >
          <Icon name="heroicons:x-mark" class="h-4 w-4" />
        </button>

        <Transition
          enter-active-class="transition ease-out duration-200"
          enter-from-class="transform opacity-0 scale-95"
          enter-to-class="transform opacity-100 scale-100"
          leave-active-class="transition ease-in duration-75"
          leave-from-class="transform opacity-100 scale-100"
          leave-to-class="transform opacity-0 scale-95"
        >
          <div
            v-if="isOpen"
            class="absolute z-50 mt-1 max-h-60 w-full overflow-auto rounded-xl bg-base-100 py-1 text-base shadow-lg ring-1 ring-black ring-opacity-5 focus:outline-none dark:ring-white/10 sm:text-sm"
          >
            <div
              v-for="option in options"
              :key="option.value.toString()"
              class="relative cursor-pointer select-none px-4 py-2 bg-base-100 hover:bg-base-200"
              @click="select(option)"
            >
              <div class="flex items-center gap-3">
              <Icon 
                v-if="option.icon" 
                :name="option.icon" 
                class="h-5 w-5 flex-shrink-0 transition-colors duration-200" 
                :class="[
                  isSelected(option) ? 'text-primary-500' : (option.iconClass || 'text-gray-400'),
                  { 'group-hover:text-primary-500': !option.iconClass }
                ]" 
              />
              <span class="block truncate" :class="{ 'font-semibold': isSelected(option), 'font-normal': !isSelected(option) }">
                {{ option.text }}
              </span>
            </div>
              <span v-if="isSelected(option)" class="absolute inset-y-0 right-0 flex items-center pr-4 text-primary-600 dark:text-primary-400">
                <Icon name="heroicons:check" class="h-5 w-5" aria-hidden="true" />
              </span>
            </div>
            <div v-if="options.length === 0" class="px-4 py-2 text-sm text-gray-500">
              No options available
            </div>
          </div>
        </Transition>
      </div>
    </FormElementContainer>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import type { SelectListItem } from '~/types/SelectListItem'
import FormElementContainer from '~/components/ui/form/FormElementContainer.vue'

interface Props {
  label?: string | null
  modelValue?: string | number | boolean | null
  options?: SelectListItem[]
  disabled?: boolean
  size?: 'sm' | 'md' | 'lg' | null
  placeholder?: string | null
  clearable?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  label: undefined,
  modelValue: undefined,
  options: () => [],
  disabled: false,
  size: 'md',
  placeholder: undefined,
  clearable: true,
})

const emit = defineEmits<{
  'update:modelValue': [value: string | number | boolean | null]
}>()

const isOpen = ref(false)
const containerRef = ref<HTMLElement | null>(null)

const selectedOption = computed(() => {
  return props.options.find(opt => opt.value === props.modelValue)
})

const selectClass = computed(() => ({
  'select-sm': props.size === 'sm',
  'select-md': props.size === 'md',
  'select-lg': props.size === 'lg',
  'bg-none': selectedOption.value && props.clearable,
}))

function toggle() {
  if (!props.disabled) {
    isOpen.value = !isOpen.value
  }
}

function select(option: SelectListItem) {
  emit('update:modelValue', option.value)
  isOpen.value = false
}

function clear() {
  emit('update:modelValue', null)
}

function isSelected(option: SelectListItem) {
  return props.modelValue === option.value
}

function closeOnClickOutside(event: MouseEvent) {
  if (containerRef.value && !containerRef.value.contains(event.target as Node)) {
    isOpen.value = false
  }
}

onMounted(() => {
  document.addEventListener('click', closeOnClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', closeOnClickOutside)
})
</script>
