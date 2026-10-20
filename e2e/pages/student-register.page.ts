import { type Locator, type Page, expect } from '@playwright/test'
import { BasePage } from './base.page'

export interface StudentFormData {
  nombre: string
  apellido: string
  carnet: string
  genero: string
  telefono: string
  fechaNacimiento: string
  direccion: string
  correo: string
  password: string
  confirmPassword: string
  fotoPath?: string
  carnetPdfPath?: string
}

export class StudentRegisterPage extends BasePage {
  readonly heading: Locator
  readonly avatarInput: Locator
  readonly avatarImage: Locator
  readonly carnetPdfInput: Locator
  readonly carnetPdfError: Locator
  readonly photoError: Locator
  readonly nombreInput: Locator
  readonly apellidoInput: Locator
  readonly carnetInput: Locator
  readonly generoSelect: Locator
  readonly telefonoInput: Locator
  readonly fechaNacimientoInput: Locator
  readonly direccionInput: Locator
  readonly correoInput: Locator
  readonly passwordInput: Locator
  readonly confirmPasswordInput: Locator
  readonly submitButton: Locator
  readonly alertError: Locator

  constructor(page: Page) {
    super(page)
    this.heading = page.locator('h1', { hasText: 'Registro de Estudiante' })
    // Hay dos inputs de archivo (fotografía y carnet PDF), por eso se distinguen por su atributo accept.
    this.avatarInput = page.locator('input[type="file"][accept="image/*"]')
    this.avatarImage = page.locator('img[alt="Foto de perfil del estudiante"]')
    this.carnetPdfInput = page.locator('#carnetPdf')
    this.carnetPdfError = page.getByTestId('carnetPdf-error')
    this.photoError = page.getByTestId('foto-error')
    this.nombreInput = page.locator('#nombre')
    this.apellidoInput = page.locator('#apellido')
    this.carnetInput = page.locator('#carnet')
    this.generoSelect = page.locator('#genero')
    this.telefonoInput = page.locator('#telefono')
    this.fechaNacimientoInput = page.locator('#fechaNacimiento')
    this.direccionInput = page.locator('#direccion')
    this.correoInput = page.locator('#correo')
    this.passwordInput = page.locator('#password')
    this.confirmPasswordInput = page.locator('#confirmPassword')
    this.submitButton = page.locator('button[type="submit"]')
    this.alertError = page.locator('[role="alert"]')
  }

  async goto(): Promise<void> {
    await this.page.goto('/register/student')
    await expect(this.heading).toBeVisible()
  }

  async uploadPhoto(filePath: string): Promise<void> {
    await this.avatarInput.setInputFiles(filePath)
    await expect(this.avatarImage).toHaveAttribute('src', /^blob:/)
  }

  async uploadCarnetPdf(filePath: string): Promise<void> {
    await this.carnetPdfInput.setInputFiles(filePath)
  }

  async fillForm(data: StudentFormData): Promise<void> {
    if (data.fotoPath) {
      await this.uploadPhoto(data.fotoPath)
    }
    if (data.carnetPdfPath) {
      await this.uploadCarnetPdf(data.carnetPdfPath)
    }
    await this.nombreInput.fill(data.nombre)
    await this.apellidoInput.fill(data.apellido)
    await this.carnetInput.fill(data.carnet)
    await this.generoSelect.selectOption(data.genero)
    await this.telefonoInput.fill(data.telefono)
    await this.fechaNacimientoInput.fill(data.fechaNacimiento)
    await this.direccionInput.fill(data.direccion)
    await this.correoInput.fill(data.correo)
    await this.passwordInput.fill(data.password)
    await this.confirmPasswordInput.fill(data.confirmPassword)
  }

  async submit(): Promise<void> {
    await this.submitButton.click()
  }

  async expectRegistrationSuccess(): Promise<void> {
    await this.page.waitForURL(/.*\/login\?registered=success.*/, { timeout: 15000 })
    await expect(this.page.locator('h1', { hasText: 'Iniciar Sesión' })).toBeVisible()
    await expect(this.page.locator('text=Registro exitoso')).toBeVisible()
  }
}
