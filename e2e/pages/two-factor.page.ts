import { type Locator, type Page, expect } from '@playwright/test'
import { BasePage } from './base.page'

export class TwoFactorPage extends BasePage {
  readonly heading: Locator
  readonly fileInput: Locator
  readonly submitButton: Locator
  readonly alertError: Locator
  readonly fileNameDisplay: Locator

  constructor(page: Page) {
    super(page)
    this.heading = page.locator('h1', { hasText: 'Verificación de Dos Pasos' })
    this.fileInput = page.locator('input#file')
    this.submitButton = page.locator('button[type="submit"]')
    this.alertError = page.locator('#error-message')
    this.fileNameDisplay = page.locator('.text-sm.font-semibold.text-on-surface')
  }

  async waitForPage(): Promise<void> {
    await this.page.waitForURL(/.*\/admin\/2fa.*/, { timeout: 15000 })
    await expect(this.heading).toBeVisible()
  }

  async uploadKeyFile(filePath: string): Promise<void> {
    await this.fileInput.setInputFiles(filePath)
    await expect(this.submitButton).toBeEnabled()
  }

  async submit(): Promise<void> {
    await this.submitButton.click()
  }

  async completeTwoFactor(filePath: string): Promise<void> {
    await this.waitForPage()
    await this.uploadKeyFile(filePath)
    await this.submit()
  }

  async expectErrorMessage(messageSnippet?: string): Promise<void> {
    await expect(this.alertError).toBeVisible()
    if (messageSnippet) {
      await expect(this.alertError).toContainText(messageSnippet)
    }
  }
}
