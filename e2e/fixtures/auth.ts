import { type Page } from '@playwright/test'

export interface CredencialesUsuario {
  correo: string
  password: string
}

interface RespuestaLogin {
  correo_validado: boolean
  token: string
  tokenType: string
  expiresIn: number
  idUsuario: number
  correo: string
  rol: string
  nombre: string
  apellido: string
  fotografiaUrl: string | null
}

function apiBaseUrl(): string {
  return process.env.E2E_API_URL || 'http://localhost:5000/api'
}

/**
 * Inicia sesión contra la API real con un usuario sembrado en la base de datos (estado
 * APROBADO y con el correo validado) y deja la sesión persistida en el navegador, de
 * modo que las pruebas no dependan de tokens ficticios ni de tokens de confirmación de
 * correo.
 */
export async function iniciarSesion(page: Page, credenciales: CredencialesUsuario): Promise<void> {
  const respuesta = await page.request.post(`${apiBaseUrl()}/login`, { data: credenciales })

  if (!respuesta.ok()) {
    throw new Error(
      `No se pudo iniciar sesión con ${credenciales.correo}: ${respuesta.status()} ${await respuesta.text()}`
    )
  }

  const datos = (await respuesta.json()) as RespuestaLogin

  if (!datos.correo_validado) {
    throw new Error(
      `El usuario ${credenciales.correo} no tiene el correo validado y requiere un token de confirmación.`
    )
  }

  await page.addInitScript(
    ({ token, usuario }) => {
      window.localStorage.setItem('edu_auth_token', token)
      window.localStorage.setItem('edu_auth_user', JSON.stringify(usuario))
    },
    {
      token: datos.token,
      usuario: {
        id: datos.idUsuario,
        correo: datos.correo,
        rol: datos.rol,
        nombre: datos.nombre,
        apellido: datos.apellido,
        fotografiaUrl: datos.fotografiaUrl
      }
    }
  )
}
