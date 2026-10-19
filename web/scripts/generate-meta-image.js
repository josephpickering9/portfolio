/* eslint-disable no-console */
// generate-meta-image.js
// Renders the site's default Open Graph image (public/meta.png): a hero card in
// the Midnight theme with the name, a tagline and a row of project/company logos
// fetched from the API.
//
// Usage:
//   node scripts/generate-meta-image.js [--api <url>] [--out <path>] [--include <slug,slug>]
//
// Defaults:
//   --api  http://localhost:5105
//   --out  public/meta.png
//
// puppeteer is deliberately not a declared dependency (it downloads Chromium on
// install) — install it ad hoc and point NODE_PATH at it to run this script.

const puppeteer = require('puppeteer');
const https = require('https');
const http = require('http');
const fs = require('fs');
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
const FONTS_DIR = path.resolve(__dirname, '..', 'public', 'fonts');

const WIDTH = 1200;
const HEIGHT = 630;
const LOGO_SIZE = 40;

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

function fontFace(family, file, style = 'normal') {
  const data = fs.readFileSync(path.join(FONTS_DIR, file)).toString('base64');
  return `@font-face { font-family: '${family}'; font-style: ${style}; font-weight: 100 900;
    src: url(data:font/woff2;base64,${data}) format('woff2'); }`;
}

function collectLogos(projects, companies) {
  const companiesWithLogo = new Set(companies.filter(c => c.logo).map(c => c.id));
  const items = [];

  for (const company of companies) {
    if (company.logo) items.push({ name: company.name, url: logoUrl(company.logo) });
  }

  for (const project of projects) {
    if (project.logoUrl && !companiesWithLogo.has(project.companyId)) {
      items.push({ name: project.title, url: logoUrl(project.logoUrl) });
    }
  }

  const includedSlugs = [];
  for (const project of projects) {
    if (INCLUDE_SLUGS.has(project.slug) && project.logoUrl && !items.some(i => i.url === logoUrl(project.logoUrl))) {
      items.push({ name: project.title, url: logoUrl(project.logoUrl) });
      includedSlugs.push(project.slug);
    }
  }
  const missing = [...INCLUDE_SLUGS].filter(s => !includedSlugs.includes(s));
  if (missing.length) console.warn(`Warning: no logo found for slugs: ${missing.join(', ')}`);

  return items;
}

// --- build HTML -------------------------------------------------------
function buildHtml(logos) {
  const logoCells = logos.map(({ name, url }) => /* html */`
    <img src="${url}" alt="${name}" onerror="this.style.display='none'" />
  `).join('\n');

  return /* html */`<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8"/>
<style>
  ${fontFace('Instrument Serif', 'InstrumentSerif-Regular.woff2')}
  ${fontFace('Geist', 'Geist-Variable.woff2')}
  ${fontFace('Geist Mono', 'GeistMono-Variable.woff2')}

  :root {
    --bg: oklch(0.15 0.02 260);
    --line: oklch(0.3 0.02 260);
    --text: oklch(0.96 0.01 260);
    --muted: oklch(0.74 0.02 260);
    --accent: oklch(0.68 0.21 25);
    --glow: oklch(0.6 0.22 25 / 0.35);
  }

  * { box-sizing: border-box; margin: 0; padding: 0; }

  body {
    width: ${WIDTH}px;
    height: ${HEIGHT}px;
    overflow: hidden;
    position: relative;
    background: var(--bg);
    color: var(--text);
    font-family: 'Geist', sans-serif;
    padding: 72px 80px 64px;
    display: flex;
    flex-direction: column;
  }

  .glow {
    position: absolute;
    top: -160px;
    right: -120px;
    width: 760px;
    height: 620px;
    background: radial-gradient(closest-side, var(--glow), transparent);
    filter: blur(20px);
  }

  .content { position: relative; display: flex; flex-direction: column; gap: 26px; }

  .eyebrow {
    font-family: 'Geist Mono', monospace;
    font-size: 17px;
    letter-spacing: 0.06em;
    text-transform: uppercase;
    color: var(--muted);
  }

  h1 {
    font-family: 'Instrument Serif', serif;
    font-weight: 400;
    font-size: 132px;
    line-height: 0.9;
    letter-spacing: -0.025em;
    white-space: nowrap;
  }
  h1 span { color: var(--accent); }

  .lede {
    max-width: 820px;
    font-size: 27px;
    line-height: 1.5;
    color: var(--muted);
  }
  .lede span { color: var(--text); white-space: nowrap; }

  .footer {
    position: relative;
    margin-top: auto;
    padding-top: 28px;
    border-top: 1px solid var(--line);
    display: flex;
    align-items: center;
  }

  .logos { display: flex; align-items: center; gap: 14px; }
  .logos img { width: ${LOGO_SIZE}px; height: ${LOGO_SIZE}px; object-fit: contain; border-radius: 8px; }
</style>
</head>
<body>
  <div class="glow"></div>
  <div class="content">
    <div class="eyebrow">Full Stack Developer</div>
    <h1>Joseph <span>Pickering</span></h1>
    <p class="lede">
      Working across the stack, from <span>Vue, React &amp; TypeScript</span>
      to <span>.NET, C# &amp; Kotlin</span>.
    </p>
  </div>
  <div class="footer">
    <div class="logos">${logoCells}</div>
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

  const logos = collectLogos(projects, companies);
  if (logos.length === 0) {
    console.error('No logos found — check that the API is running and has data.');
    process.exit(1);
  }
  console.log(`Found ${logos.length} logos`);

  const browser = await puppeteer.launch({ headless: true });
  const page = await browser.newPage();
  await page.setViewport({ width: WIDTH, height: HEIGHT, deviceScaleFactor: 2 });
  await page.setContent(buildHtml(logos), { waitUntil: 'networkidle0' });
  await page.evaluate(() => document.fonts.ready);
  await page.screenshot({ path: OUT_PATH });
  await browser.close();

  console.log(`Saved ${WIDTH * 2}x${HEIGHT * 2} image to ${OUT_PATH}`);
}

main().catch((err) => {
  console.error(err);
  process.exit(1);
});
