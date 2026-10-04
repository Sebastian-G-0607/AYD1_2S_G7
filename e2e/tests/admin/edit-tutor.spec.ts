import { test, expect } from '@playwright/test'
import { LoginPage } from '../../pages/login.page'
import { AdminUsersPage } from '../../pages/admin-users.page'
import { AdminEditTutorPage } from '../../pages/admin-edit-tutor.page'
import { AdminApprovalsPage } from '../../pages/admin-approvals.page'
import { TutorRegisterPage } from '../../pages/tutor-register.page'
import { TUTOR_TEST_DATA } from '../../fixtures/test-data'

test.describe('HU-35 - Ver y Actualizar Tutor (Módulo Administrador)', () => {
  test('Debe acceder al formulario de edición de tutor, comprobar bloqueo de correo y actualizar perfil con éxito', async ({ page }) => {
    const loginPage = new LoginPage(page)
    await loginPage.loginAsAdmin()

    const adminUsersPage = new AdminUsersPage(page)
    await adminUsersPage.goto()
    await adminUsersPage.switchToTutorsTab()

    // Si aún no hay tutores aprobados en la tabla (ej. corrida aislada), se aprueba uno
    const hasTutors = await adminUsersPage.hasTutors()
    if (!hasTutors) {
      const adminApprovalsPage = new AdminApprovalsPage(page)
      await adminApprovalsPage.goto()
      await adminApprovalsPage.switchToTutorsTab()

      const pendingRows = page.locator('tbody tr')
      const count = await pendingRows.count()
      const text = count > 0 ? await pendingRows.first().textContent() : ''

      if (count > 0 && !text?.includes('No se encontraron')) {
        await pendingRows.first().locator('button', { hasText: 'Aceptar' }).click()
        await adminApprovalsPage.approveModal.locator('button', { hasText: 'Aceptar Tutor' }).click()
      } else {
        await page.evaluate(() => localStorage.clear())
        const tutorRegisterPage = new TutorRegisterPage(page)
        await tutorRegisterPage.goto()
        await tutorRegisterPage.fillForm(TUTOR_TEST_DATA)
        await tutorRegisterPage.submit()
        await tutorRegisterPage.expectRegistrationSuccess()

        await loginPage.loginAsAdmin()
        await adminApprovalsPage.goto()
        await adminApprovalsPage.approveTutor(TUTOR_TEST_DATA.carnetId)
      }

      await adminUsersPage.goto()
      await adminUsersPage.switchToTutorsTab()
    }

    // Navegar al formulario de edición del primer tutor activo disponible
    await adminUsersPage.clickEditFirstTutor()

    const adminEditTutorPage = new AdminEditTutorPage(page)
    await adminEditTutorPage.waitForPage()

    // Regla de Negocio Crítica (Criterio 2.3.3): El correo electrónico permanece estrictamente deshabilitado
    await adminEditTutorPage.expectEmailDisabled()
    await expect(adminEditTutorPage.emailInput).toHaveAttribute('disabled', '')

    // Actualización de campos permitidos
    await adminEditTutorPage.fillForm({
      telefono: '55443322',
      direccion: 'Avenida Las Américas 15-20, Zona 13, Guatemala',
      universidad: 'Universidad de San Carlos de Guatemala',
      direccionTutoria: 'Edificio T-1, Salón 102'
    })

    // Enviar y validar confirmación de éxito
    await adminEditTutorPage.submit()
    await adminEditTutorPage.expectSuccess()

    // Volver a la vista de usuarios activos
    await adminEditTutorPage.goBack()
    await expect(adminUsersPage.heading).toBeVisible()
  })
})
