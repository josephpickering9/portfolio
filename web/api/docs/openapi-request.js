const fs = require('fs')

// Path to the file you want to modify
const filePath = './api/core/request.ts'

// New getFormData method to insert
const newGetFormDataMethod = `
export const getFormData = (options: ApiRequestOptions): FormData | undefined => {
  if (options.formData) {
    const formData = new FormData()

    const process = (key: string, value: any, parentKey?: string) => {
      const fullKey = parentKey ? \`\${parentKey}[\${key}]\` : key

      if (isBlob(value)) {
        const fullKeyBlob = parentKey ? \`\${parentKey}.\${key}\` : key
        formData.append(fullKeyBlob, value)
      } else if (isString(value)) {
        formData.append(fullKey, value)
      } else if (Array.isArray(value)) {
        value.forEach((item, index) => {
          if (isString(item) || isBlob(item)) {
            formData.append(\`\${fullKey}[\${index}]\`, item)
          } else {
            process(index.toString(), item, fullKey)
          }
        })
      } else if (typeof value === 'object' && value !== null) {
        Object.entries(value).forEach(([nestedKey, nestedValue]) => {
          process(nestedKey, nestedValue, fullKey)
        })
      } else {
        formData.append(fullKey, JSON.stringify(value))
      }
    }

    Object.entries(options.formData)
      .filter(([_, value]) => value !== undefined && value !== null)
      .forEach(([key, value]) => {
        process(key, value)
      })

    return formData
  }

  return undefined
}
`

// Read the original file
fs.readFile(filePath, 'utf8', (err, data) => {
  if (err) {
    // eslint-disable-next-line no-console
    console.error(err)
    return
  }

  // Find the start of the original getFormData method
  const startMatch = data.match(/export const getFormData =/)

  if (!startMatch) {
    // eslint-disable-next-line no-console
    console.error('getFormData method not found.')
    return
  }

  const startIndex = startMatch.index

  // Find the end of the method by matching braces
  let openBraces = 0
  let endIndex = startIndex

  for (let i = startIndex; i < data.length; i++) {
    if (data[i] === '{') {
      openBraces++
    } else if (data[i] === '}') {
      openBraces--
      if (openBraces === 0) {
        endIndex = i + 1 // Include the closing brace
        break
      }
    }
  }

  // Replace the original getFormData method
  const result = data.substring(0, startIndex) + newGetFormDataMethod + data.substring(endIndex)

  // Write the modified content back to the file
  fs.writeFile(filePath, result, 'utf8', (err) => {
    if (err) {
      // eslint-disable-next-line no-console
      console.error(err)
    } else {
      // eslint-disable-next-line no-console
      console.log('File has been updated successfully.')
    }
  })
})
