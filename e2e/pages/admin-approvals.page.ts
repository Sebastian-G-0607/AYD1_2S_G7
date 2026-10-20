import { type Locator, type Page, expect } from '@playwright/test'
import { BasePage } from './base.page'

export class AdminApprovalsPage extends BasePage {
  readonly heading: Locator
  readonly studentsTab: Locator
  readonly tutorsTab: Locator
  readonly searchInput: Locator
  readonly approveModal: Locator

  constructor(page: Page) {
    super(page)
    this.heading = page.locator('h1', { hasText: 'Aprobaciones Pendientes' })
    this.studentsTab = page.locator('button', { hasText: 'Estudiantes Pendientes' })
    this.tutorsTab = page.locator('button', { hasText: 'Tutores Pendientes' })
    this.searchInput = page.locator('input[placeholder*="Buscar por"]')
    this.approveModal = page.locator('div.fixed.inset-0', { hasText: 'Confirmar Aprobación' })
  }

  async goto(): Promise<void> {
    await this.page.goto('/admin/aprobaciones')
    await expect(this.heading).toBeVisible()
  }

  async switchToStudentsTab(): Promise<void> {
    await this.studentsTab.click()
  }

  async switchToTutorsTab(): Promise<void> {
    await this.tutorsTab.click()
  }

  findStudentRow(identifier: string): Locator {
    return this.page.locator('tr', { hasText: identifier })
  }

  findTutorRow(identifier: string): Locator {
    return this.page.locator('tr', { hasText: identifier })
  }

  async approveStudent(identifier: string): Promise<void> {
    await this.switchToStudentsTab()
    const row = this.findStudentRow(identifier)
    await expect(row).toBeVisible()
    await row.locator('button', { hasText: 'Aceptar' }).click()
    await expect(this.approveModal).toBeVisible()
    await this.approveModal.locator('button', { hasText: 'Aceptar Estudiante' }).click()
    await expect(this.approveModal).not.toBeVisible()
    await expect(this.page.locator('text=El estudiante').filter({ hasText: 'ha sido aprobado exitosamente' })).toBeVisible()
    await expect(row).not.toBeVisible()
  }

  async approveTutor(identifier: string): Promise<void> {
    await this.switchToTutorsTab()
    const row = this.findTutorRow(identifier)
    await expect(row).toBeVisible()
    await row.locator('button', { hasText: 'Aceptar' }).click()
    await expect(this.approveModal).toBeVisible()
    await this.approveModal.locator('button', { hasText: 'Aceptar Tutor' }).click()
    await expect(this.approveModal).not.toBeVisible()
    await expect(this.page.locator('text=El tutor').filter({ hasText: 'ha sido aprobado exitosamente' })).toBeVisible()
    await expect(row).not.toBeVisible()
  }
}
