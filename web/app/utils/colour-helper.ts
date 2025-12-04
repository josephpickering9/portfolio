export const hexToRgb = (hex: string) => {
  // Remove the hash at the start if it's there
  hex = hex.replace(/^#/, '')

  // Parse the hex color
  const bigint = parseInt(hex, 16)
  const r = (bigint >> 16) & 255
  const g = (bigint >> 8) & 255
  const b = bigint & 255

  return { r, g, b }
}

export const luminance = (r: number, g: number, b: number) => {
  const a = [r, g, b].map(function (v) {
    v /= 255
    return v <= 0.03928 ? v / 12.92 : Math.pow((v + 0.055) / 1.055, 2.4)
  })
  return a[0] * 0.2126 + a[1] * 0.7152 + a[2] * 0.0722
}

export const setTextColourForBackground = (hexColor: string) => {
  const rgb = hexToRgb(hexColor)
  const lum = luminance(rgb.r, rgb.g, rgb.b)
  if (lum > 0.1) {
    return '' // bright colors - black font
  } else {
    return 'text-gray-200' // dark colors - white font
  }
}

export const getShadeFromOklsh = (oklchString: string) => {
  const hexColor = oklchStringToHex(oklchString)
  if (!hexColor) return 'dark'

  const rgb = hexToRgb(hexColor)
  const lum = luminance(rgb.r, rgb.g, rgb.b)
  if (lum > 0.1) {
    return 'dark' // bright colors - black font
  } else {
    return 'light' // dark colors - white font
  }
}

export const oklchStringToHex = (oklchString: string): string | undefined => {
  const match = oklchString.match(/oklch\((\d+(\.\d+)?) (\d+(\.\d+)?) (\d+(\.\d+)?)\/1\)/)

  if (!match) return undefined

  const [, L, , C, , H] = match.map(Number)
  const hueRadians = H * (Math.PI / 180)

  const [l, a, ab] = oklchToOklab(L, C, hueRadians)
  const [x, y, z] = oklabToXyz(l, a, ab)
  const [lr, lg, lb] = xyzToLinearRgb(x, y, z)
  const [r, g, b] = linearRgbToRgb(lr, lg, lb)

  return rgbToHex(r, g, b)
}

const oklchToOklab = (L: number, C: number, h: number): [number, number, number] => {
  const a = C * Math.cos(h)
  const b = C * Math.sin(h)
  return [L, a, b]
}

const oklabToXyz = (L: number, a: number, b: number): [number, number, number] => {
  const l_ = L + 0.3963377774 * a + 0.2158037573 * b
  const m_ = L - 0.1055613458 * a - 0.0638541728 * b
  const s_ = L - 0.0894841775 * a - 1.291485548 * b

  const l = Math.pow(l_, 3)
  const m = Math.pow(m_, 3)
  const s = Math.pow(s_, 3)

  return [
    +4.0767416621 * l - 3.3077115913 * m + 0.2309699292 * s,
    -1.2684380046 * l + 2.6097574011 * m - 0.3413193965 * s,
    -0.0041960863 * l - 0.7034186147 * m + 1.707614701 * s,
  ]
}

const xyzToLinearRgb = (x: number, y: number, z: number): [number, number, number] => {
  const r = 3.2404542 * x - 1.5371385 * y - 0.4985314 * z
  const g = -0.969266 * x + 1.8760108 * y + 0.041556 * z
  const b = 0.0556434 * x - 0.2040259 * y + 1.0572252 * z
  return [r, g, b]
}

const linearRgbToRgb = (r: number, g: number, b: number): [number, number, number] => {
  const correctGamma = (c: number) => (c <= 0.0031308 ? 12.92 * c : 1.055 * Math.pow(c, 1 / 2.4) - 0.055)
  return [correctGamma(r), correctGamma(g), correctGamma(b)]
}

const rgbToHex = (r: number, g: number, b: number): string => {
  const toHex = (c: number) => {
    const hex = Math.min(255, Math.max(c * 255, 0)).toString(16)
    return hex.length === 1 ? '0' + hex : hex
  }
  return `#${toHex(r)}${toHex(g)}${toHex(b)}`
}

export default {
  hexToRgb,
  luminance,
  setTextColourForBackground,
  getShadeFromOklsh,
}
