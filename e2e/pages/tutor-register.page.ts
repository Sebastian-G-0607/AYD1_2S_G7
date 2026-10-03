import { type Locator, type Page, expect } from '@playwright/test'
import { BasePage } from './base.page'

export interface TutorFormData {
  nombre: string
  apellido: string
  carnetId: string
  numeroIdentificacion: string
  genero: string
  telefono: string
  fechaNacimiento: string
  direccion: string
  universidad: string
  anioInicio: string | number
  direccionTutoria: string
  materia?: string
  horaInicio?: string
  horaFin?: string
  correo: string
  password: string
  confirmPassword: string
  fotoPath?: string
  cvPdfPath?: string
}

export class TutorRegisterPage extends BasePage {
  readonly heading: Locator
  readonly avatarInput: Locator
  readonly avatarImage: Locator
  readonly cvPdfInput: Locator
  readonly cvPdfError: Locator
  readonly nombreInput: Locator
  readonly apellidoInput: Locator
  readonly carnetInput: Locator
  readonly dpiInput: Locator
  readonly generoSelect: Locator
  readonly telefonoInput: Locator
  readonly fechaNacimientoInput: Locator
  readonly direccionInput: Locator
  readonly universidadInput: Locator
  readonly anioInicioInput: Locator
  readonly direccionTutoriaInput: Locator
  readonly materiasInput: Locator
  readonly horaInicioInput: Locator
  readonly horaFinInput: Locator
  readonly correoInput: Locator
  readonly passwordInput: Locator
  readonly confirmPasswordInput: Locator
  readonly submitButton: Locator
  readonly alertError: Locator

  constructor(page: Page) {
    super(page)
    this.heading = page.locator('h1', { hasText: 'Registro de Tutor' })
    this.avatarInput = page.locator('input[type="file"][accept="image/*"]')
    this.avatarImage = page.locator('img[alt="Foto de perfil del tutor"]')
    this.cvPdfInput = page.locator('#cvPdf')
    this.cvPdfError = page.getByTestId('cvPdf-error')
    this.nombreInput = page.locator('#nombre')
    this.apellidoInput = page.locator('#apellido')
    this.carnetInput = page.locator('#carnetId')
    this.dpiInput = page.locator('#numeroIdentificacion')
    this.generoSelect = page.locator('#genero')
    this.telefonoInput = page.locator('#telefono')
    this.fechaNacimientoInput = page.locator('#fechaNacimiento')
    this.direccionInput = page.locator('#direccion')
    this.universidadInput = page.locator('#universidad')
    this.anioInicioInput = page.locator('#anioInicio')
    this.direccionTutoriaInput = page.locator('#direccionTutoria')
    this.materiasInput = page.locator('#materiasIds')
    this.horaInicioInput = page.locator('#horaInicio')
    this.horaFinInput = page.locator('#horaFin')
    this.correoInput = page.locator('#correo')
    this.passwordInput = page.locator('#password')
    this.confirmPasswordInput = page.locator('#confirmPassword')
    this.submitButton = page.locator('button[type="submit"]')
    this.alertError = page.locator('[role="alert"]')
  }

  async goto(): Promise<void> {
    await this.page.goto('/register/tutor')
    await expect(this.heading).toBeVisible()
  }

  async uploadPhoto(filePath: string): Promise<void> {
    await this.avatarInput.setInputFiles(filePath)
    await expect(this.avatarImage).toHaveAttribute('src', /^blob:/)
  }

  async uploadCvPdf(filePath: string): Promise<void> {
    await this.cvPdfInput.setInputFiles(filePath)
  }

  async selectMateria(materiaName: string): Promise<void> {
    await expect(this.materiasInput).toBeEnabled()
    await this.materiasInput.click()
    await this.materiasInput.fill(materiaName)
    const option = this.page.locator('div[role="button"]', { hasText: materiaName }).first()
    await expect(option).toBeVisible()
    await option.click()
  }

  async fillForm(data: TutorFormData): Promise<void> {
    if (data.fotoPath) {
      await this.uploadPhoto(data.fotoPath)
    }
    await this.nombreInput.fill(data.nombre)
    await this.apellidoInput.fill(data.apellido)
    await this.carnetInput.fill(data.carnetId)
    await this.dpiInput.fill(data.numeroIdentificacion)
    await this.generoSelect.selectOption(data.genero)
    await this.telefonoInput.fill(data.telefono)
    await this.fechaNacimientoInput.fill(data.fechaNacimiento)
    await this.direccionInput.fill(data.direccion)
    await this.universidadInput.fill(data.universidad)
    await this.anioInicioInput.fill(String(data.anioInicio))
    await this.direccionTutoriaInput.fill(data.direccionTutoria)
    if (data.materia) {
      await this.selectMateria(data.materia)
    }
    if (data.horaInicio) {
      await this.horaInicioInput.fill(data.horaInicio)
    }
    if (data.horaFin) {
      await this.horaFinInput.fill(data.horaFin)
    }
    await this.correoInput.fill(data.correo)
    await this.passwordInput.fill(data.password)
    if (data.cvPdfPath) {
      await this.uploadCvPdf(data.cvPdfPath)
    }
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
