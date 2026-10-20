import { test, expect } from '@playwright/test'
import { LoginPage } from '../../pages/login.page'
import { StudentStudyPlanPage } from '../../pages/student-study-plan.page'
import { E2E_STUDENT_USER } from '../../fixtures/test-data'

test.describe('Plan de Estudio del Estudiante', () => {
  test('Debe mostrar el plan de estudio y permitir descargar la constancia en PDF', async ({ page }) => {
    const loginPage = new LoginPage(page)
    await loginPage.goto()
    await loginPage.login(E2E_STUDENT_USER.correo, E2E_STUDENT_USER.password)
    await page.waitForURL(/.*\/estudiante.*/, { timeout: 20000 })

    const studyPlanPage = new StudentStudyPlanPage(page)
    await studyPlanPage.goto()
    await expect(studyPlanPage.tutorName).toBeVisible()

    const download = await studyPlanPage.downloadPdf()

    expect(download.suggestedFilename()).toContain('.pdf')
  })
})
