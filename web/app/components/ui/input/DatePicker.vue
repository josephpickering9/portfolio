<template>
  <FormElementContainer :label="label">
    <div ref="container" class="dropdown w-full" :class="{ 'dropdown-open': isOpen }">
      <div class="relative flex items-center">
        <input
          readonly
          type="text"
          :placeholder="placeholder"
          :value="displayValue"
          :disabled="disabled"
          class="input input-bordered w-full cursor-pointer pr-10"
          @click="toggle"
        >
        <button
          v-if="value"
          type="button"
          class="absolute right-10 btn btn-circle btn-ghost btn-xs text-base-content/50 hover:text-base-content"
          @click.stop="clear"
        >
          <Icon name="heroicons:x-mark" size="1.2em" />
        </button>
        <div class="absolute right-3 pointer-events-none text-base-content/50">
          <Icon name="heroicons:calendar" size="1.5em" />
        </div>
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
  isValid 
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
  placeholder: 'Select date',
  disabled: false,
})

const emit = defineEmits<{
  'update:modelValue': [value: string | undefined]
}>()

const isOpen = ref(false)
const currentDate = ref(new Date()) // Navigation state
const value = ref<string | undefined>(props.modelValue)
const container = ref<HTMLElement | null>(null)

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

function toggle() {
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
  emit('update:modelValue', formatted)
  close()
}

function clear() {
  value.value = undefined
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

watch(() => props.modelValue, (newValue) => {
  value.value = newValue
})

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})
</script>
