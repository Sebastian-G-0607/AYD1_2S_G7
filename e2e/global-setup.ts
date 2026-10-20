/**
 * Prepara el identificador de la corrida de pruebas. Todos los archivos de prueba comparten
 * este valor, de modo que los datos de registro (carnet, DPI y correo) sean únicos dentro de
 * la corrida y Stable aunque Playwright reinicie el worker después de un fallo.
 */
export default function globalSetup(): void {
  process.env.E2E_RUN_ID = `${Date.now()}-${Math.floor(Math.random() * 1_000_000)}`
}
