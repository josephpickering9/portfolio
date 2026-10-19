import { readFileSync } from 'node:fs'

const METRICS = ['lines', 'statements', 'functions', 'branches']
const { total } = JSON.parse(readFileSync(new URL('../coverage/coverage-summary.json', import.meta.url), 'utf8'))
const rows = METRICS.map((metric) => `| ${metric} | ${total[metric].pct}% |`)

process.stdout.write(['### Web unit test coverage', '', '| Metric | Coverage |', '|---|---|', ...rows, ''].join('\n'))
