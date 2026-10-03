import { test, expect } from '@playwright/test'
import { LoginPage } from '../../pages/login.page'
import { StudentStudyPlanPage } from '../../pages/student-study-plan.page'
import { STUDENT_TEST_DATA } from '../../fixtures/test-data'

test.describe('Plan de Estudio del Estudiante', () => {
  test('Debe mostrar el plan de estudio y permitir descargar la constancia en PDF', async ({ page }) => {
    const loginPage = new LoginPage(page)
    await loginPage.goto()
    await loginPage.login(STUDENT_TEST_DATA.correo, STUDENT_TEST_DATA.password)
    await page.waitForURL(/.*\/estudiante.*/, { timeout: 20000 })

    const studyPlanPage = new StudentStudyPlanPage(page)
    await studyPlanPage.goto()
    await expect(studyPlanPage.tutorName).toBeVisible()

    const download = await studyPlanPage.downloadPdf()

    expect(download.suggestedFilename()).toContain('.pdf')
  })
})