<template>
  <div class="overflow-x-auto rounded-lg border border-base-200 bg-base-100 shadow-sm">
    <table class="table" :class="{ 'table-zebra': zebra }">
      <thead>
        <tr>
          <th
            v-for="(column, index) in columns"
            :key="index"
            :class="column.thClass"
          >
            <button
              v-if="column.sortable"
              type="button"
              class="flex items-center gap-2 hover:text-primary transition-colors"
              @click="$emit('sort', column.key)"
            >
              <span>{{ column.label }}</span>
              <Icon
                v-if="sortColumn === column.key"
                :name="sortDirection === 'asc' ? 'heroicons:chevron-up' : 'heroicons:chevron-down'"
                class="h-4 w-4"
              />
            </button>
            <span v-else>{{ column.label }}</span>
          </th>
        </tr>
      </thead>
      <tbody>
        <tr
          v-for="(row, rowIndex) in data"
          :key="getRowKey(row, rowIndex)"
          :class="rowClass"
        >
          <td
            v-for="(column, colIndex) in columns"
            :key="colIndex"
            :class="column.tdClass"
          >
            <slot
              :name="`cell-${column.key}`"
              :row="row"
              :value="row[column.key]"
              :index="rowIndex"
            >
              {{ row[column.key] }}
            </slot>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup lang="ts" generic="T extends Record<string, any>">
export interface TableColumn {
  key: string
  label: string
  sortable?: boolean
  thClass?: string
  tdClass?: string
}

interface Props {
  columns: TableColumn[]
  data: T[]
  sortColumn?: string | null
  sortDirection?: 'asc' | 'desc'
  zebra?: boolean
  rowClass?: string
  rowKey?: string
}

const props = withDefaults(defineProps<Props>(), {
  sortColumn: null,
  sortDirection: 'asc',
  zebra: true,
  rowClass: 'group hover:bg-base-200/50 transition-colors duration-200',
  rowKey: 'id',
})

defineEmits<{
  sort: [column: string]
}>()

function getRowKey(row: T, index: number): string | number {
  return props.rowKey in row ? row[props.rowKey] : index
}
</script>
