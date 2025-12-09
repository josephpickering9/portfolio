import type { Ref } from 'vue';
import type { AsyncForm } from '~/types/AsyncForm';
import { extractError } from '~/utils/error-helper';

export async function tryCatchFinally<T>(
    form: Ref<AsyncForm<T>>,
    fun: () => Promise<T | undefined>
): Promise<T | undefined> {
    try {
        form.value.error = undefined;
        form.value.loading = true;

        const result = await fun();
        form.value.data = result;
    } catch (error: any) {
        if (error?.name === 'CanceledError') return;
        form.value.error = extractError(error);
    } finally {
        form.value.loading = false;
    }

    return form.value.data;
}

export function asyncForm<T>(): AsyncForm<T> {
    return {
        data: undefined,
        error: undefined,
        loading: false,
    };
}
