import { readFileSync } from 'node:fs'
import { fileURLToPath } from 'node:url'
import { describe, expect, it } from 'vitest'
import { ErrorCodes } from '~/types/errors'

const backendErrorCodesPath = fileURLToPath(
  new URL('../../../api/Portfolio.Api/Types/ErrorCodes.cs', import.meta.url),
)
const backendConstPattern = /public const string \w+ = "([a-z_]+)";/g

function readBackendErrorCodes(): string[] {
  const source = readFileSync(backendErrorCodesPath, 'utf8')
  return [...source.matchAll(backendConstPattern)].map((match) => match[1] ?? '')
}

describe('ErrorCodes contract', () => {
  it('matches the backend ErrorCodes.cs exactly', () => {
    const backendCodes = readBackendErrorCodes()

    expect(backendCodes.length).toBeGreaterThan(0)
    expect([...Object.values(ErrorCodes)].sort()).toEqual([...backendCodes].sort())
  })
})
