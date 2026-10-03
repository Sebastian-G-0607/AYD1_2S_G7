import { type Locator, type Page, expect } from '@playwright/test'
import { BasePage } from './base.page'

export class StudentStudyPlanPage extends BasePage {
  readonly heading: Locator
  readonly downloadButton: Locator
  readonly tutorName: Locator

  constructor(page: Page) {
    super(page)
    this.heading = page.locator('h1', { hasText: 'Plan de Estudio' })
    this.downloadButton = page.locator('button', { hasText: 'Descargar constancia' })
    this.tutorName = page.getByText('Ana Lucía Pérez')
  }

  async goto(): Promise<void> {
    await this.page.goto('/estudiante/plan-estudio')
    await expect(this.heading).toBeVisible()
  }

  async downloadPdf() {
    const downloadPromise = this.page.waitForEvent('download')
    await this.downloadButton.click()
    return downloadPromise
  }
}