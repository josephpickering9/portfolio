import { watch, type Ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { isEmpty } from 'lodash-es'

interface QueryOptions<T> {
    key: string
    initialValue: T
    transform?: (value: string | undefined) => T
    serialize?: (value: T) => string | undefined
}

export function useQuerySync<T>(
    value: Ref<T>,
    options: QueryOptions<T>
) {
    const route = useRoute()
    const router = useRouter()
    const { key, initialValue, transform, serialize } = options

    const loadFromQuery = () => {
        const queryValue = route.query[key] as string | undefined
        if (queryValue !== undefined) {
            value.value = transform ? transform(queryValue) : (queryValue as unknown as T)
        } else {
            value.value = initialValue
        }
    }

    const updateQuery = () => {
        const serializedValue = serialize ? serialize(value.value) : (value.value as unknown as string)
        const newQuery: Record<string, any> = { ...route.query }

        if (serializedValue && !isEmpty(serializedValue)) {
            newQuery[key] = serializedValue
        } else {
            newQuery[key] = undefined
        }

        if (route.query[key] !== newQuery[key]) {
            router.push({ query: newQuery })
        }
    }

    loadFromQuery()

    watch(() => route.query[key], loadFromQuery)
    watch(value, updateQuery)
}
