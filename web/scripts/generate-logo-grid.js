/* eslint-disable no-console */
// generate-logo-grid.js
// Fetches project and company logos from the API, renders them in a uniform
// grid on a dark background, and saves a screenshot to public/meta.png.
//
// Usage:
//   node scripts/generate-logo-grid.js [--api <url>] [--out <path>]
//
// Defaults:
//   --api  http://localhost:5105
//   --out  public/meta.png

const puppeteer = require('puppeteer');
const https = require('https');
const http = require('http');
const path = require('path');

// --- config -----------------------------------------------------------
const args = process.argv.slice(2);
const getArg = (flag, fallback) => {
  const i = args.indexOf(flag);
  return i !== -1 ? args[i + 1] : fallback;
};
const API_BASE = getArg('--api', 'http://localhost:5105');
const OUT_PATH = path.resolve(__dirname, '..', getArg('--out', 'public/meta.png'));
const INCLUDE_SLUGS = new Set(
  (getArg('--include', '') || '').split(',').map(s => s.trim()).filter(Boolean)
);

// --- helpers ----------------------------------------------------------
function fetchJson(url) {
  return new Promise((resolve, reject) => {
    const mod = url.startsWith('https') ? https : http;
    mod.get(url, (res) => {
      let data = '';
      res.on('data', (c) => (data += c));
      res.on('end', () => {
        try { resolve(JSON.parse(data)); }
        catch (e) { reject(new Error(`JSON parse error for ${url}: ${e.message}`)); }
      });
    }).on('error', reject);
  });
}

function logoUrl(filename) {
  return `${API_BASE}/media/uploads/${filename}`;
}

// --- grid layout constants (must match CSS below) ---------------------
const CELL_SIZE = 140;
const GAP = 28;
const MAX_WIDTH = 1100;
const VIEWPORT_WIDTH = 1200;
const COLUMNS = Math.floor((MAX_WIDTH + GAP) / (CELL_SIZE + GAP)); // 6

// --- build HTML -------------------------------------------------------
function buildHtml(items) {
  const cells = items.map(({ name, url }) => /* html */`
    <div class="cell">
      <img src="${url}" alt="${name}" onerror="this.parentElement.style.display='none'" />
    </div>
  `).join('\n');

  return /* html */`<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8"/>
<style>
  * { box-sizing: border-box; margin: 0; padding: 0; }

  body {
    background: #1D232A;
    display: flex;
    align-items: center;
    justify-content: center;
    min-height: 100vh;
    padding: 48px;
  }

  .grid {
    display: grid;
    grid-template-columns: repeat(${COLUMNS}, ${CELL_SIZE}px);
    gap: ${GAP}px;
    justify-content: center;
    max-width: ${MAX_WIDTH}px;
    width: 100%;
  }

  .cell {
    width: ${CELL_SIZE}px;
    height: ${CELL_SIZE}px;
    display: flex;
    align-items: center;
    justify-content: center;
  }

  .cell img {
    width: 100px;
    height: 100px;
    object-fit: contain;
  }
</style>
</head>
<body>
  <div class="grid">
    ${cells}
  </div>
</body>
</html>`;
}

// --- main -------------------------------------------------------------
async function main() {
  console.log(`Fetching data from ${API_BASE}…`);

  const [projects, companies] = await Promise.all([
    fetchJson(`${API_BASE}/project`),
    fetchJson(`${API_BASE}/company`),
  ]);

  // Build a set of company IDs that have a logo, so we can skip their projects
  const companiesWithLogo = new Set(companies.filter(c => c.logo).map(c => c.id));

  const items = [];

  // One entry per company (with a logo)
  for (const company of companies) {
    if (company.logo) {
      items.push({ name: company.name, url: logoUrl(company.logo) });
    }
  }

  // Projects that have no company, or whose company has no logo
  for (const project of projects) {
    if (project.logoUrl && !companiesWithLogo.has(project.companyId)) {
      items.push({ name: project.title, url: logoUrl(project.logoUrl) });
    }
  }

  // Explicitly included projects by slug (even if their company already has a logo)
  if (INCLUDE_SLUGS.size > 0) {
    const includedSlugs = [];
    for (const project of projects) {
      if (INCLUDE_SLUGS.has(project.slug) && project.logoUrl) {
        // Avoid exact file duplicates but allow overriding company deduplication
        if (!items.some(i => i.url === logoUrl(project.logoUrl))) {
          items.push({ name: project.title, url: logoUrl(project.logoUrl) });
          includedSlugs.push(project.slug);
        }
      }
    }
    const missing = [...INCLUDE_SLUGS].filter(s => !includedSlugs.includes(s));
    if (missing.length) console.warn(`Warning: no logo found for slugs: ${missing.join(', ')}`);
  }

  // Auto-fill remaining slots to complete the last row
  const usedUrls = new Set(items.map(i => i.url));
  for (const project of projects) {
    if (items.length % COLUMNS === 0) break;
    if (project.logoUrl && !usedUrls.has(logoUrl(project.logoUrl))) {
      items.push({ name: project.title, url: logoUrl(project.logoUrl) });
      usedUrls.add(logoUrl(project.logoUrl));
    }
  }

  if (items.length === 0) {
    console.error('No logos found — check that the API is running and has data.');
    process.exit(1);
  }

  console.log(`Found ${items.length} logos (${companies.filter(c => c.logo).length} companies + ${items.length - companies.filter(c => c.logo).length} projects)`);

  const html = buildHtml(items);

  const browser = await puppeteer.launch({ headless: true });
  const page = await browser.newPage();

  // Set a wide initial viewport so the grid fills out before we measure
  await page.setViewport({ width: VIEWPORT_WIDTH, height: 800, deviceScaleFactor: 2 });
  await page.setContent(html, { waitUntil: 'load' });

  // Measure actual content height and lock the viewport to it
  const bodyHeight = await page.evaluate(() => document.body.scrollHeight);
  await page.setViewport({ width: VIEWPORT_WIDTH, height: bodyHeight, deviceScaleFactor: 2 });

  await page.screenshot({ path: OUT_PATH, fullPage: true });

  await browser.close();

  console.log(`Screenshot saved to ${OUT_PATH}`);
}

main().catch((err) => {
  console.error(err);
  process.exit(1);
});
