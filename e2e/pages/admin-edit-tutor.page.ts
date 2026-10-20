import { type Locator, type Page, expect } from '@playwright/test'
import { BasePage } from './base.page'

export interface EditTutorFormData {
  nombre?: string
  apellido?: string
  carnetId?: string
  numeroIdentificacion?: string
  genero?: string
  fechaNacimiento?: string
  telefono?: string
  direccion?: string
  universidad?: string
  anioInicio?: number | string
  direccionTutoria?: string
}

export class AdminEditTutorPage extends BasePage {
  readonly heading: Locator
  readonly backButton: Locator
  readonly nombreInput: Locator
  readonly apellidoInput: Locator
  readonly carnetInput: Locator
  readonly dpiInput: Locator
  readonly generoSelect: Locator
  readonly fechaNacimientoInput: Locator
  readonly telefonoInput: Locator
  readonly direccionInput: Locator
  readonly universidadInput: Locator
  readonly anioInicioInput: Locator
  readonly direccionTutoriaInput: Locator
  readonly emailInput: Locator
  readonly submitButton: Locator

  constructor(page: Page) {
    super(page)
    this.heading = page.locator('h1', { hasText: 'Perfil de Tutor' })
    this.backButton = page.locator('button', { hasText: 'Volver' }).first()
    this.nombreInput = page.locator('input#nombres')
    this.apellidoInput = page.locator('input#apellidos')
    this.carnetInput = page.locator('input#carnet')
    this.dpiInput = page.locator('input#numeroIdentificacion')
    this.generoSelect = page.locator('select#genero')
    this.fechaNacimientoInput = page.locator('input#fechaNacimiento')
    this.telefonoInput = page.locator('input#telefono')
    this.direccionInput = page.locator('input#direccion')
    this.universidadInput = page.locator('input#universidad')
    this.anioInicioInput = page.locator('input#anioInicio')
    this.direccionTutoriaInput = page.locator('input#direccionTutoria')
    this.emailInput = page.locator('input#correo')
    this.submitButton = page.locator('button[type="submit"]')
  }

  async waitForPage(): Promise<void> {
    await this.page.waitForURL(/.*\/admin\/tutores\/\d+\/editar.*/, { timeout: 15000 })
    await expect(this.heading).toBeVisible()
    await expect(this.emailInput).toBeVisible()
  }

  async expectEmailDisabled(): Promise<void> {
    await expect(this.emailInput).toBeDisabled()
  }

  async fillForm(data: EditTutorFormData): Promise<void> {
    if (data.nombre !== undefined) {
      await this.nombreInput.fill(data.nombre)
    }
    if (data.apellido !== undefined) {
      await this.apellidoInput.fill(data.apellido)
    }
    if (data.carnetId !== undefined) {
      await this.carnetInput.fill(data.carnetId)
    }
    if (data.numeroIdentificacion !== undefined) {
      await this.dpiInput.fill(data.numeroIdentificacion)
    }
    if (data.genero !== undefined) {
      await this.generoSelect.selectOption(data.genero)
    }
    if (data.fechaNacimiento !== undefined) {
      await this.fechaNacimientoInput.fill(data.fechaNacimiento)
    }
    if (data.telefono !== undefined) {
      await this.telefonoInput.fill(data.telefono)
    }
    if (data.direccion !== undefined) {
      await this.direccionInput.fill(data.direccion)
    }
    if (data.universidad !== undefined) {
      await this.universidadInput.fill(data.universidad)
    }
    if (data.anioInicio !== undefined) {
      await this.anioInicioInput.fill(String(data.anioInicio))
    }
    if (data.direccionTutoria !== undefined) {
      await this.direccionTutoriaInput.fill(data.direccionTutoria)
    }
  }

  async submit(): Promise<void> {
    await this.submitButton.click()
  }

  async expectSuccess(): Promise<void> {
    await expect(
      this.page.getByText(/actualizada correctamente|exitosamente/i)
    ).toBeVisible({ timeout: 10000 })
  }

  async goBack(): Promise<void> {
    await this.backButton.click()
    await this.page.waitForURL(/.*\/admin\/usuarios.*/, { timeout: 15000 })
  }
}
