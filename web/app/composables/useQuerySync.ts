import { watch, type Ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { isEmpty } from 'lodash-es'

// Updates are chained so that several keys changing in one tick each build on the URL the previous update produced.
let pendingUpdate: Promise<void> = Promise.resolve()

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
        pendingUpdate = pendingUpdate.then(async () => {
            const serializedValue = serialize ? serialize(value.value) : (value.value as unknown as string)
            const currentQuery = router.currentRoute.value.query
            const nextValue = serializedValue && !isEmpty(serializedValue) ? serializedValue : undefined

            if (currentQuery[key] !== nextValue) {
                await router.replace({ query: { ...currentQuery, [key]: nextValue } })
            }
        }).catch(() => undefined)
    }

    loadFromQuery()

    watch(() => route.query[key], loadFromQuery)
    watch(value, updateQuery)
}
