<template>
  <FormElementContainer :label="label">
    <div ref="container" class="dropdown w-full" :class="{ 'dropdown-open': isOpen }">
      <div class="relative flex items-center">
        <input
          ref="inputRef"
          type="text"
          :placeholder="placeholder"
          :value="inputValue"
          :disabled="disabled"
          class="input input-bordered w-full pr-10"
          :class="{ 'input-error': hasInputError }"
          @input="handleInput"
          @focus="handleFocus"
          @blur="handleBlur"
          @keydown="handleKeydown"
        >
        <button
          v-if="value"
          type="button"
          class="absolute right-10 btn btn-circle btn-ghost btn-xs text-base-content/50 hover:text-base-content"
          @click.stop="clear"
        >
          <Icon name="heroicons:x-mark" size="1.2em" />
        </button>
        <button
          type="button"
          class="absolute right-3 text-base-content/50 hover:text-base-content btn btn-circle btn-ghost btn-xs"
          @click="toggleCalendar"
        >
          <Icon name="heroicons:calendar" size="1.5em" />
        </button>
      </div>

      <div v-if="isOpen" class="dropdown-content z-[1] mt-2 w-72 rounded-box bg-base-100 p-4 shadow-xl ring-1 ring-base-content/10" @click.stop>
        <div class="flex items-center justify-between mb-4">
          <button type="button" class="btn btn-circle btn-ghost btn-sm" @click="prevMonth">
            <Icon name="heroicons:chevron-left" />
          </button>
          <span class="font-bold text-base-content">{{ format(currentDate, 'MMMM yyyy') }}</span>
          <button type="button" class="btn btn-circle btn-ghost btn-sm" @click="nextMonth">
            <Icon name="heroicons:chevron-right" />
          </button>
        </div>

        <div class="grid grid-cols-7 mb-2 text-center">
          <span v-for="day in weekdays" :key="day" class="text-xs font-bold text-base-content/50 uppercase">{{ day }}</span>
        </div>

        <div class="grid grid-cols-7 gap-1">
          <div v-for="n in paddingDays" :key="'pad-' + n" />
          
          <button
            v-for="day in daysInMonth"
            :key="day.toString()"
            type="button"
            class="btn btn-sm btn-ghost w-full h-9 min-h-0 p-0 font-normal"
            :class="{
              'btn-primary text-primary-content': isSelected(day),
              'text-base-content': !isSelected(day) && !isToday(day),
              'text-primary font-bold': isToday(day) && !isSelected(day)
            }"
            @click.stop.prevent="selectDate(day)"
          >
            {{ format(day, 'd') }}
          </button>
        </div>
      </div>
    </div>
  </FormElementContainer>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted, onUnmounted } from 'vue'
import { 
  format, 
  startOfMonth, 
  endOfMonth, 
  eachDayOfInterval, 
  addMonths, 
  subMonths, 
  getDay, 
  isSameDay, 
  parseISO,
  isValid,
  parse 
} from 'date-fns'
import FormElementContainer from '~/components/ui/form/FormElementContainer.vue'
import { Icon } from '#components'

interface Props {
  modelValue?: string | undefined
  label?: string
  placeholder?: string
  disabled?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: undefined,
  label: undefined,
  placeholder: 'Select date (e.g., MM/DD/YYYY)',
  disabled: false,
})

const emit = defineEmits<{
  'update:modelValue': [value: string | undefined]
}>()

const isOpen = ref(false)
const currentDate = ref(new Date()) // Navigation state
const value = ref<string | undefined>(props.modelValue)
const container = ref<HTMLElement | null>(null)
const inputRef = ref<HTMLInputElement | null>(null)
const inputValue = ref('')
const hasInputError = ref(false)
const isInputFocused = ref(false)

const weekdays = ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa']

const displayValue = computed(() => {
  if (!value.value) return ''
  const date = parseISO(value.value)
  return isValid(date) ? format(date, 'MMM d, yyyy') : ''
})

const daysInMonth = computed(() => {
  const start = startOfMonth(currentDate.value)
  const end = endOfMonth(currentDate.value)
  return eachDayOfInterval({ start, end })
})

const paddingDays = computed(() => {
  const start = startOfMonth(currentDate.value)
  return getDay(start) // 0 for Sunday, etc.
})

// Parse date from various input formats
function parseInputDate(input: string): Date | null {
  const trimmed = input.trim()
  
  // Try various date formats
  const formats = [
    'yyyy-MM-dd',     // 2024-12-08
    'MM/dd/yyyy',     // 12/08/2024
    'M/d/yyyy',       // 12/8/2024
    'dd/MM/yyyy',     // 08/12/2024
    'd/M/yyyy',       // 8/12/2024
    'MM-dd-yyyy',     // 12-08-2024
    'M-d-yyyy',       // 12-8-2024
    'yyyy/MM/dd',     // 2024/12/08
    'MMM d, yyyy',    // Dec 8, 2024
    'MMMM d, yyyy',   // December 8, 2024
    'd MMM yyyy',     // 8 Dec 2024
    'd MMMM yyyy',    // 8 December 2024
  ]
  
  const referenceDate = new Date()
  
  for (const formatString of formats) {
    try {
      const parsed = parse(trimmed, formatString, referenceDate)
      if (isValid(parsed)) {
        return parsed
      }
    } catch {
      // Continue to next format
    }
  }
  
  // Try ISO parse as fallback
  try {
    const parsed = parseISO(trimmed)
    if (isValid(parsed)) {
      return parsed
    }
  } catch {
    // Invalid date
  }
  
  return null
}

function handleInput(event: Event) {
  const target = event.target as HTMLInputElement
  inputValue.value = target.value
  
  if (!target.value.trim()) {
    hasInputError.value = false
    value.value = undefined
    emit('update:modelValue', undefined)
    return
  }
  
  const parsedDate = parseInputDate(target.value)
  
  if (parsedDate) {
    hasInputError.value = false
    const formatted = format(parsedDate, 'yyyy-MM-dd')
    value.value = formatted
    emit('update:modelValue', formatted)
    currentDate.value = parsedDate // Update calendar view
  } else {
    hasInputError.value = true
  }
}

function handleFocus() {
  isInputFocused.value = true
}

function handleBlur() {
  isInputFocused.value = false
  
  // Format the display value properly on blur if valid
  if (value.value && !hasInputError.value) {
    inputValue.value = displayValue.value
  }
}

function handleKeydown(event: KeyboardEvent) {
  if (event.key === 'Escape') {
    inputRef.value?.blur()
    close()
  } else if (event.key === 'Enter') {
    event.preventDefault()
    if (value.value && !hasInputError.value) {
      inputRef.value?.blur()
    }
  }
}

function toggleCalendar() {
  if (props.disabled) return
  if (!isOpen.value) {
    // Reset calendar view to selected date or current date on open
    if (value.value && isValid(parseISO(value.value))) {
      currentDate.value = parseISO(value.value)
    } else {
      currentDate.value = new Date()
    }
  }
  isOpen.value = !isOpen.value
}

function close() {
  isOpen.value = false
}

function prevMonth() {
  currentDate.value = subMonths(currentDate.value, 1)
}

function nextMonth() {
  currentDate.value = addMonths(currentDate.value, 1)
}

function selectDate(date: Date) {
  const formatted = format(date, 'yyyy-MM-dd')
  value.value = formatted
  inputValue.value = format(date, 'MMM d, yyyy')
  hasInputError.value = false
  emit('update:modelValue', formatted)
  close()
}

function clear() {
  value.value = undefined
  inputValue.value = ''
  hasInputError.value = false
  emit('update:modelValue', undefined)
}

function isSelected(date: Date) {
  if (!value.value) return false
  return isSameDay(date, parseISO(value.value))
}

function isToday(date: Date) {
  return isSameDay(date, new Date())
}

function handleClickOutside(event: MouseEvent) {
  if (container.value && !container.value.contains(event.target as Node)) {
    close()
  }
}

// Sync inputValue with value changes
watch(() => props.modelValue, (newValue) => {
  value.value = newValue
  
  // Don't overwrite the input value while the user is typing
  if (isInputFocused.value) return

  if (newValue) {
    const date = parseISO(newValue)
    if (isValid(date)) {
      inputValue.value = format(date, 'MMM d, yyyy')
      hasInputError.value = false
    }
  } else {
    inputValue.value = ''
    hasInputError.value = false
  }
}, { immediate: true })

watch(value, (newValue) => {
  if (!isInputFocused.value && newValue) {
    const date = parseISO(newValue)
    if (isValid(date)) {
      inputValue.value = format(date, 'MMM d, yyyy')
    }
  }
})

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})
</script>
