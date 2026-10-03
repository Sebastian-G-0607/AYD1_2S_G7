import { test, expect } from '@playwright/test'
import { LoginPage } from '../../pages/login.page'
import { StudentRegisterPage } from '../../pages/student-register.page'
import { TutorRegisterPage } from '../../pages/tutor-register.page'
import { AdminApprovalsPage } from '../../pages/admin-approvals.page'
import { STUDENT_TEST_DATA, TUTOR_TEST_DATA } from '../../fixtures/test-data'

test.describe('Módulo de Autenticación - Registro y Aprobación de Usuarios', () => {
  test('Registro de estudiante', async ({ page }) => {
    const studentRegisterPage = new StudentRegisterPage(page)
    await studentRegisterPage.goto()

    await studentRegisterPage.fillForm(STUDENT_TEST_DATA)

    await expect(page.getByText('carnet.pdf', { exact: true })).toBeVisible()
    await expect(studentRegisterPage.nombreInput).toHaveValue(STUDENT_TEST_DATA.nombre)
    await expect(studentRegisterPage.apellidoInput).toHaveValue(STUDENT_TEST_DATA.apellido)
    await expect(studentRegisterPage.carnetInput).toHaveValue(STUDENT_TEST_DATA.carnet)
    await expect(studentRegisterPage.generoSelect).toHaveValue(STUDENT_TEST_DATA.genero)
    await expect(studentRegisterPage.telefonoInput).toHaveValue(STUDENT_TEST_DATA.telefono)
    await expect(studentRegisterPage.fechaNacimientoInput).toHaveValue(STUDENT_TEST_DATA.fechaNacimiento)
    await expect(studentRegisterPage.direccionInput).toHaveValue(STUDENT_TEST_DATA.direccion)
    await expect(studentRegisterPage.correoInput).toHaveValue(STUDENT_TEST_DATA.correo)
    await expect(studentRegisterPage.passwordInput).toHaveValue(STUDENT_TEST_DATA.password)
    await expect(studentRegisterPage.confirmPasswordInput).toHaveValue(STUDENT_TEST_DATA.confirmPassword)

    await studentRegisterPage.submit()
    await studentRegisterPage.expectRegistrationSuccess()
  })

  test('HU-22: el registro de estudiante exige fotografía y carnet en PDF', async ({ page }) => {
    const studentRegisterPage = new StudentRegisterPage(page)
    await studentRegisterPage.goto()

    // Criterios 1 y 4: sin fotografía ni carnet PDF no se permite el registro.
    await studentRegisterPage.fillForm({
      ...STUDENT_TEST_DATA,
      fotoPath: undefined,
      carnetPdfPath: undefined
    })
    await studentRegisterPage.submit()
    await expect(studentRegisterPage.photoError).toContainText('fotografía reciente es obligatoria')
    await expect(studentRegisterPage.carnetPdfError).toContainText('PDF con tu carnet escaneado')
    await expect(page).toHaveURL(/\/register\/student/)

    // Criterio 2: el carnet solo acepta PDF, cualquier otro tipo de archivo se rechaza.
    await studentRegisterPage.uploadCarnetPdf(STUDENT_TEST_DATA.fotoPath)
    await expect(studentRegisterPage.carnetPdfError).toContainText('Solo se permiten archivos PDF')
    await expect(page.getByText('estudiante.png', { exact: true })).toHaveCount(0)

    // Criterio 3: se puede corregir el archivo elegido (cambiar y quitar).
    await studentRegisterPage.uploadCarnetPdf(STUDENT_TEST_DATA.carnetPdfPath)
    await expect(studentRegisterPage.carnetPdfError).toHaveCount(0)
    await expect(page.getByText('carnet.pdf', { exact: true })).toBeVisible()
    await page.getByRole('button', { name: 'Quitar Carnet escaneado (PDF)' }).click()
    await expect(page.getByText('carnet.pdf', { exact: true })).toHaveCount(0)

    await studentRegisterPage.uploadPhoto(STUDENT_TEST_DATA.fotoPath)
    await expect(studentRegisterPage.photoError).toHaveCount(0)
    await page.getByRole('button', { name: 'Quitar fotografía' }).click()
    await expect(studentRegisterPage.avatarImage).not.toHaveAttribute('src', /^blob:/)
  })

  test('Registro de tutor', async ({ page }) => {
    const tutorRegisterPage = new TutorRegisterPage(page)
    await tutorRegisterPage.goto()

    await tutorRegisterPage.fillForm(TUTOR_TEST_DATA)

    await expect(tutorRegisterPage.nombreInput).toHaveValue(TUTOR_TEST_DATA.nombre)
    await expect(tutorRegisterPage.apellidoInput).toHaveValue(TUTOR_TEST_DATA.apellido)
    await expect(tutorRegisterPage.carnetInput).toHaveValue(TUTOR_TEST_DATA.carnetId)
    await expect(tutorRegisterPage.dpiInput).toHaveValue(TUTOR_TEST_DATA.numeroIdentificacion)
    await expect(tutorRegisterPage.generoSelect).toHaveValue(TUTOR_TEST_DATA.genero)
    await expect(tutorRegisterPage.telefonoInput).toHaveValue(TUTOR_TEST_DATA.telefono)
    await expect(tutorRegisterPage.fechaNacimientoInput).toHaveValue(TUTOR_TEST_DATA.fechaNacimiento)
    await expect(tutorRegisterPage.direccionInput).toHaveValue(TUTOR_TEST_DATA.direccion)
    await expect(tutorRegisterPage.universidadInput).toHaveValue(TUTOR_TEST_DATA.universidad)
    await expect(tutorRegisterPage.anioInicioInput).toHaveValue(String(TUTOR_TEST_DATA.anioInicio))
    await expect(tutorRegisterPage.direccionTutoriaInput).toHaveValue(TUTOR_TEST_DATA.direccionTutoria)
    await expect(tutorRegisterPage.horaInicioInput).toHaveValue(TUTOR_TEST_DATA.horaInicio)
    await expect(tutorRegisterPage.horaFinInput).toHaveValue(TUTOR_TEST_DATA.horaFin)
    await expect(tutorRegisterPage.correoInput).toHaveValue(TUTOR_TEST_DATA.correo)
    await expect(tutorRegisterPage.passwordInput).toHaveValue(TUTOR_TEST_DATA.password)
    await expect(tutorRegisterPage.confirmPasswordInput).toHaveValue(TUTOR_TEST_DATA.confirmPassword)

    await tutorRegisterPage.submit()
    await tutorRegisterPage.expectRegistrationSuccess()
  })

  test('Aprobación de estudiante y tutor por el administrador', async ({ page }) => {
    const loginPage = new LoginPage(page)
    await loginPage.loginAsAdmin()

    const adminApprovalsPage = new AdminApprovalsPage(page)
    await adminApprovalsPage.goto()

    await adminApprovalsPage.approveStudent(STUDENT_TEST_DATA.carnet)
    await adminApprovalsPage.approveTutor(TUTOR_TEST_DATA.carnetId)
  })
})
