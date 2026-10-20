import crypto from 'crypto'

/**
 * Identificador de la corrida de pruebas. Lo establece el global setup para que todos los
 * archivos de prueba compartan el mismo valor, incluso si Playwright reinicia el worker
 * después de un fallo.
 */
const corridaId =
  process.env.E2E_RUN_ID ?? `${Date.now()}-${Math.floor(Math.random() * 1_000_000)}`

/**
 * Genera un sufijo numérico estable y único para un archivo de prueba dentro de la corrida
 * actual. El carnet, el DPI y el correo son únicos en la base de datos, por lo que cada
 * archivo necesita los suyos para que la suite pueda repetirse sin limpiar la base.
 */
export function identificadorUnico(origen: string, digitos = 6): string {
  const hash = crypto.createHash('sha256').update(`${corridaId}:${origen}`).digest('hex')
  return (parseInt(hash.slice(0, 12), 16) % 10 ** digitos).toString().padStart(digitos, '0')
}
