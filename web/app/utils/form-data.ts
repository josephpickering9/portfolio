export function objectToFormData(
    obj: any,
    form?: FormData,
    namespace?: string
): FormData {
    const fd = form || new FormData()
    let formKey: string

    for (const property in obj) {
        if (!Object.prototype.hasOwnProperty.call(obj, property)) continue

        if (namespace) {
            formKey = namespace + '[' + property + ']'
        } else {
            formKey = property
        }

        // if the property is an object, but not a File, use recursivity.
        if (
            typeof obj[property] === 'object' &&
            !(obj[property] instanceof File) &&
            !(obj[property] instanceof Blob) &&
            !(obj[property] instanceof Date) &&
            obj[property] !== null
        ) {
            objectToFormData(obj[property], fd, formKey)
        } else {
            if (obj[property] !== undefined && obj[property] !== null) {
                if (obj[property] instanceof Date) {
                    fd.append(formKey, obj[property].toISOString())
                } else {
                    fd.append(formKey, obj[property])
                }
            }
        }
    }

    return fd
}

export function serializeToFormData(body: any): FormData {
    const formData = new FormData()

    const append = (data: any, parentKey: string) => {
        if (data === null || data === undefined) return

        if (data instanceof Blob || data instanceof File) {
            formData.append(parentKey, data)
        } else if (data instanceof Date) {
            formData.append(parentKey, data.toISOString())
        } else if (Array.isArray(data)) {
            data.forEach((item, index) => {
                append(item, `${parentKey}[${index}]`)
            })
        } else if (typeof data === 'object' && data !== null) {
            Object.keys(data).forEach(key => {
                const fullKey = parentKey ? `${parentKey}.${key}` : key
                append(data[key], fullKey)
            })
        } else {
            formData.append(parentKey, String(data))
        }
    }

    if (typeof body === 'object' && body !== null && !Array.isArray(body)) {
        Object.keys(body).forEach(key => {
            append(body[key], key)
        })
    }

    return formData
}
