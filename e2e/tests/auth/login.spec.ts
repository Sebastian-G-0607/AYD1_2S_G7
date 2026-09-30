import path from 'path'
import { test, expect } from '@playwright/test'
import { LoginPage } from '../../pages/login.page'
import { TwoFactorPage } from '../../pages/two-factor.page'

test.describe('Módulo de Autenticación - Inicio de Sesión', () => {
  let loginPage: LoginPage

  test.beforeEach(async ({ page }) => {
    loginPage = new LoginPage(page)
    await loginPage.goto()
  })

  test('Debe mostrar la página de inicio de sesión con sus elementos principales', async ({ page }) => {
    await expect(loginPage.heading).toBeVisible()
    await expect(loginPage.emailInput).toBeVisible()
    await expect(loginPage.passwordInput).toBeVisible()
    await expect(loginPage.submitButton).toBeVisible()
    await expect(loginPage.submitButton).toContainText('Ingresar a la Plataforma')
  })

  test('Debe fallar el login con contraseña incorrecta y mostrar mensaje de error', async ({ page }) => {
    await loginPage.login('admin@educonnect.com', 'ContrasenaErronea123!')

    await loginPage.expectErrorMessage()
    expect(page.url()).toContain('/login')
  })

  test('Debe iniciar sesión exitosamente con credenciales de administrador y autenticación en 2 pasos', async ({ page }) => {
    const adminEmail = process.env.E2E_ADMIN_EMAIL || 'admin@educonnect.com'
    const adminPassword = process.env.E2E_ADMIN_PASSWORD || 'admin123'
    const keyFilePath = path.resolve(__dirname, '../../fixtures/docs/auth2-ayd1.txt')

    await loginPage.login(adminEmail, adminPassword)

    const twoFactorPage = new TwoFactorPage(page)
    await twoFactorPage.waitForPage()
    expect(page.url()).toContain('/admin/2fa')

    await twoFactorPage.uploadKeyFile(keyFilePath)
    await twoFactorPage.submit()

    await page.waitForURL(/.*\/admin\/aprobaciones.*/, { timeout: 15000 })
    expect(page.url()).toContain('/admin/aprobaciones')
  })
})
