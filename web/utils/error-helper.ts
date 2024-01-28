const genericMessage = 'There was an issue with your request'

export const extractError = (error: any): string => {
  if (error.isAxiosError) {
    if (!error.response || typeof error.response.data !== 'string') return genericMessage

    return error.response.data.toString()
  } else if (error.name === 'ApiError') {
    if (!error.body) return genericMessage

    if (error.body.errors) {
      return (Object.values(error.body.errors) as string[][])[0][0] || genericMessage
    } else if (error.body.status === 404) {
      return 'Record could not be found'
    } else {
      return error.body.message || genericMessage
    }
  } else if (typeof error === 'string') {
    return error
  } else {
    return genericMessage
  }
}

export default {
  extractError,
}
