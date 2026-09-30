import { type Locator, type Page, expect } from '@playwright/test'
import { BasePage } from './base.page'

export class LoginPage extends BasePage {
  readonly emailInput: Locator
  readonly passwordInput: Locator
  readonly submitButton: Locator
  readonly heading: Locator
  readonly alertError: Locator

  constructor(page: Page) {
    super(page)
    this.emailInput = page.locator('#correo')
    this.passwordInput = page.locator('#password')
    this.submitButton = page.locator('button[type="submit"]')
    this.heading = page.locator('h1', { hasText: 'Iniciar Sesión' })
    this.alertError = page.locator('[role="alert"]')
  }

  async goto(): Promise<void> {
    await this.page.goto('/login')
    await expect(this.heading).toBeVisible()
  }

  async fillEmail(email: string): Promise<void> {
    await this.emailInput.fill(email)
  }

  async fillPassword(password: string): Promise<void> {
    await this.passwordInput.fill(password)
  }

  async submit(): Promise<void> {
    await this.submitButton.click()
  }

  async login(email: string, pass: string): Promise<void> {
    await this.fillEmail(email)
    await this.fillPassword(pass)
    await this.submit()
  }

  async expectErrorMessage(messageSnippet?: string): Promise<void> {
    await expect(this.alertError).toBeVisible()
    if (messageSnippet) {
      await expect(this.alertError).toContainText(messageSnippet)
    }
  }

  async expectNoError(): Promise<void> {
    await expect(this.alertError).not.toBeVisible()
  }

  async uploadTwoFactorKey(filePath: string): Promise<void> {
    const fileInput = this.page.locator('input#file')
    await fileInput.setInputFiles(filePath)
  }

  async submitTwoFactor(): Promise<void> {
    const submitButton = this.page.locator('button[type="submit"]')
    await submitButton.click()
  }

  async completeTwoFactor(filePath: string): Promise<void> {
    await this.page.waitForURL(/.*\/admin\/2fa.*/, { timeout: 15000 })
    await this.uploadTwoFactorKey(filePath)
    await this.submitTwoFactor()
  }
}
