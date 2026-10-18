export function ok<T>(data: T) {
  return { data, error: undefined }
}

export function fail(code: string, status: number) {
  return { data: undefined, error: { error: code }, response: { status } }
}

export function deferred<T>() {
  let resolve: (value: T) => void = () => undefined
  const promise = new Promise<T>((res) => (resolve = res))
  return { promise, resolve }
}
