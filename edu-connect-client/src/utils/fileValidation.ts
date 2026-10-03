const BYTES_PER_MB = 1024 * 1024

export const MAX_PDF_SIZE_MB = 5
export const MAX_IMAGE_SIZE_MB = 5

const PDF_MIME_TYPES = ['application/pdf', 'application/x-pdf']
const IMAGE_EXTENSIONS = ['.jpg', '.jpeg', '.png', '.webp']

function getExtension(fileName: string): string {
  const dotIndex = fileName.lastIndexOf('.')
  return dotIndex >= 0 ? fileName.slice(dotIndex).toLowerCase() : ''
}

export function isPdfFile(file: File): boolean {
  const hasPdfExtension = getExtension(file.name) === '.pdf'
  const hasPdfMimeType = file.type === '' || PDF_MIME_TYPES.includes(file.type)
  return hasPdfExtension && hasPdfMimeType
}

export function isImageFile(file: File): boolean {
  const hasImageExtension = IMAGE_EXTENSIONS.includes(getExtension(file.name))
  const hasImageMimeType = file.type === '' || file.type.startsWith('image/')
  return hasImageExtension && hasImageMimeType
}

/** Retorna un mensaje de error, o `null` si el archivo es un PDF válido. */
export function validatePdfFile(file: File): string | null {
  if (!isPdfFile(file)) {
    return 'Solo se permiten archivos PDF. No se aceptan otros tipos de archivo.'
  }
  if (file.size === 0) {
    return 'El archivo PDF está vacío.'
  }
  if (file.size > MAX_PDF_SIZE_MB * BYTES_PER_MB) {
    return `El archivo PDF no puede pesar más de ${MAX_PDF_SIZE_MB} MB.`
  }
  return null
}

/** Retorna un mensaje de error, o `null` si el archivo es una imagen válida. */
export function validateImageFile(file: File): string | null {
  if (!isImageFile(file)) {
    return 'La fotografía debe ser una imagen en formato JPG, PNG o WEBP.'
  }
  if (file.size === 0) {
    return 'El archivo de la fotografía está vacío.'
  }
  if (file.size > MAX_IMAGE_SIZE_MB * BYTES_PER_MB) {
    return `La fotografía no puede pesar más de ${MAX_IMAGE_SIZE_MB} MB.`
  }
  return null
}

export function formatFileSize(bytes: number): string {
  if (bytes < 1024) return `${bytes} B`
  if (bytes < BYTES_PER_MB) return `${(bytes / 1024).toFixed(1)} KB`
  return `${(bytes / BYTES_PER_MB).toFixed(1)} MB`
}
