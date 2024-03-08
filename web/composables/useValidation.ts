export default function useValidation() {
  async function validate(form) {
    const isValid = await form.$validate()
    if (!isValid) scrollToFirstError()

    return isValid
  }

  function scrollToFirstError() {
    const error = document.querySelector('.error-msg')
    if (error) {
      error.scrollIntoView({ behavior: 'smooth', block: 'center' })
    }
  }

  return {
    validate,
  }
}
