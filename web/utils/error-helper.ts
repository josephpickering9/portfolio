const genericMessage = 'There was an issue with your request'

export const extractError = (error: any): string => {
  if (error.isAxiosError) {
    if (!error.response || typeof error.response.data !== 'string') return genericMessage

    return error.response.data.toString()
  } else if (error.name === 'ApiError') {
    if (!error.body || typeof error.body !== 'string') return genericMessage

    return error.body.toString()
  } else {
    return genericMessage
  }
}

export default {
  extractError,
}
