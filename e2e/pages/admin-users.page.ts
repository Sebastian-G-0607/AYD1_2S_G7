import { type Locator, type Page, expect } from '@playwright/test'
import { BasePage } from './base.page'

export class AdminUsersPage extends BasePage {
  readonly heading: Locator
  readonly activeUsersTab: Locator
  readonly inactiveUsersTab: Locator
  readonly studentsSubTab: Locator
  readonly tutorsSubTab: Locator
  readonly searchInput: Locator
  readonly tutorsTable: Locator

  constructor(page: Page) {
    super(page)
    this.heading = page.locator('h1', { hasText: 'Gestión de Usuarios' })
    this.activeUsersTab = page.locator('button', { hasText: 'Usuarios Activos' })
    this.inactiveUsersTab = page.locator('button', { hasText: 'Usuarios Dados de Baja' })
    this.studentsSubTab = page.locator('button', { hasText: 'Estudiantes' })
    this.tutorsSubTab = page.locator('button', { hasText: 'Tutores' })
    this.searchInput = page.locator('input[placeholder*="Buscar por"]')
    this.tutorsTable = page.locator('table')
  }

  async goto(): Promise<void> {
    await this.page.goto('/admin/usuarios')
    await expect(this.heading).toBeVisible()
  }

  async switchToTutorsTab(): Promise<void> {
    await this.tutorsSubTab.click()
    await expect(this.tutorsSubTab).toHaveClass(/bg-primary/)
  }

  async switchToStudentsTab(): Promise<void> {
    await this.studentsSubTab.click()
    await expect(this.studentsSubTab).toHaveClass(/bg-primary/)
  }

  findTutorRow(identifier: string): Locator {
    return this.page.locator('tbody tr', { hasText: identifier })
  }

  async hasTutors(): Promise<boolean> {
    const rows = this.page.locator('tbody tr')
    const count = await rows.count()
    if (count === 0) return false
    const text = await rows.first().textContent()
    return !text?.includes('No se encontraron tutores activos')
  }

  async clickEditFirstTutor(): Promise<void> {
    const editButton = this.page.locator('tbody tr button', { hasText: 'Editar' }).first()
    await expect(editButton).toBeVisible()
    await editButton.click()
    await this.page.waitForURL(/.*\/admin\/tutores\/\d+\/editar.*/, { timeout: 15000 })
  }

  async clickEditTutor(identifier: string): Promise<void> {
    const row = this.findTutorRow(identifier)
    await expect(row).toBeVisible()
    const editButton = row.locator('button', { hasText: 'Editar' })
    await expect(editButton).toBeVisible()
    await editButton.click()
    await this.page.waitForURL(/.*\/admin\/tutores\/\d+\/editar.*/, { timeout: 15000 })
  }
}
