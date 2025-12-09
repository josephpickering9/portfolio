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
            if (Array.isArray(obj[property])) {
                // Special handle for arrays to match dotnet/standard conventions: key[0], key[1]
                // The loop above produces key[0], key[1] naturally if we treat array as object with numeric keys?
                // JS arrays are objects. property will be '0', '1'.
                // So namespace + '[' + '0' + ']' -> namespace[0]. This is correct for PHP/Rails often.
                // But for dot notation?
                // Let's refine.

                // Actually, let's use a specific approach for arrays if needed.
                // But treating array as object with indices works for bracket notation: key[0]

                objectToFormData(obj[property], fd, formKey)
            } else {
                // Nested object.
                // Usually we want key.subkey for ASP.NET? Or key[subkey]?
                // User example was tags[0].
                // If we use brackets everywhere: images[0][id]
                // ASP.NET Core model binder accepts both images[0].id and images[0][id].
                // Let's stick to brackets as it's safer for strict parsers.
                objectToFormData(obj[property], fd, formKey)
            }
        } else {
            // if it's a string, number, boolean, or file
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
                // ASP.NET convention: key[0]
                append(item, `${parentKey}[${index}]`)
            })
        } else if (typeof data === 'object' && data !== null) {
            Object.keys(data).forEach(key => {
                // Dot notation for objects usually preferred in .NET but brackets work too.
                // If parentKey is empty (root), just key.
                // If parentKey has brackets project[images], then project[images].id ?? 
                // actually project[images][0].id is often rendered as project.images[0].id in some systems.

                // Let's look at the user example again:
                // name="tags[0]"
                // This implies the ROOT `tags` is an array.

                // So if root is object:
                // title -> title
                // tags -> tags[0]

                const fullKey = parentKey ? `${parentKey}.${key}` : key
                if (Array.isArray(data)) {
                    // Should verify if we hit this since handled above
                }
                append(data[key], fullKey)
            })
        } else {
            formData.append(parentKey, String(data))
        }
    }

    // We need a specific iteration for the root object to avoid root.key
    if (typeof body === 'object' && body !== null && !Array.isArray(body)) {
        Object.keys(body).forEach(key => {
            append(body[key], key)
        })
    } else {
        // unexpected root array or primitive
    }

    return formData
}
