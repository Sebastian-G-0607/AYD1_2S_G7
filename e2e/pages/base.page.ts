import { type Page } from '@playwright/test'

export class BasePage {
  readonly page: Page

  constructor(page: Page) {
    this.page = page
  }

  async waitForPageLoaded(): Promise<void> {
    await this.page.waitForLoadState('networkidle')
  }

  async getCurrentUrl(): Promise<string> {
    return this.page.url()
  }
}
