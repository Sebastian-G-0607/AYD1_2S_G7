import { type Locator, type Page, expect } from '@playwright/test'
import { BasePage } from './base.page'

export class AdminUserReportsPage extends BasePage {
  readonly heading: Locator
  readonly tutorsTab: Locator
  readonly studentsTab: Locator
  readonly reports: Locator
  readonly confirmationDialog: Locator

  constructor(page: Page) {
    super(page)
    this.heading = page.getByRole('heading', { name: 'Gestión de Denuncias' })
    this.tutorsTab = page.getByRole('button', { name: /Reportes hacia tutores/ })
    this.studentsTab = page.getByRole('button', { name: /Reportes hacia estudiantes/ })
    this.reports = page.locator('article')
    this.confirmationDialog = page.getByRole('dialog')
  }

  async goto(): Promise<void> {
    await this.page.goto('/admin/denuncias')
    await expect(this.heading).toBeVisible()
  }

  async expectHeadingVisible(): Promise<void> {
    await expect(this.heading).toBeVisible()
  }

  async selectTutorsTab(): Promise<void> {
    await this.tutorsTab.click()
    await expect(this.tutorsTab).toHaveClass(/bg-surface-container-lowest/)
  }

  async selectStudentsTab(): Promise<void> {
    await this.studentsTab.click()
    await expect(this.studentsTab).toHaveClass(/bg-surface-container-lowest/)
  }

  getVisibleReports(): Locator {
    return this.reports
  }

  getReportById(reportId: number): Locator {
    return this.reports.filter({ hasText: `Reporte #${reportId}` })
  }

  async clickRejectReport(report: Locator): Promise<void> {
    await report.getByRole('button', { name: 'Rechazar denuncia' }).click()
    await expect(this.confirmationDialog).toBeVisible()
  }

  async confirmRejection(): Promise<void> {
    await this.confirmationDialog
      .getByRole('button', { name: 'Rechazar denuncia' })
      .click()
  }

  async expectSuccessMessage(message: string): Promise<void> {
    await expect(this.page.getByText(message, { exact: true })).toBeVisible()
  }

  async expectReportStatus(reportId: number, status: string): Promise<void> {
    await expect(
      this.getReportById(reportId).getByText(status, { exact: true })
    ).toBeVisible()
  }

  async expectReportActionsHidden(reportId: number): Promise<void> {
    const report = this.getReportById(reportId)
    await expect(report.getByRole('button', { name: 'Dar de baja' })).toHaveCount(0)
    await expect(report.getByRole('button', { name: 'Rechazar denuncia' })).toHaveCount(0)
  }
}
