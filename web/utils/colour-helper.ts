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

export default {
  hexToRgb,
  luminance,
  setTextColourForBackground,
}
