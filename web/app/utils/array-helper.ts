export function replaceAt<T>(items: readonly T[], index: number, item: T): T[] {
  return items.map((existing, itemIndex) => (itemIndex === index ? item : existing))
}

export function removeAt<T>(items: readonly T[], index: number): T[] {
  return items.filter((_, itemIndex) => itemIndex !== index)
}

export function typedEntries<K extends string, V>(record: Record<K, V>): [K, V][] {
  return Object.entries(record) as [K, V][]
}
