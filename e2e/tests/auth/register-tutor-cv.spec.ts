import { test, expect } from '@playwright/test'
import { TutorRegisterPage } from '../../pages/tutor-register.page'
import { STUDENT_TEST_DATA, TUTOR_TEST_DATA } from '../../fixtures/test-data'

test.describe('HU-23 - Registro de tutor con currículum en PDF', () => {
  test('exige el CV, solo acepta PDF y permite corregir el archivo', async ({ page }) => {
    const tutorRegisterPage = new TutorRegisterPage(page)
    await tutorRegisterPage.goto()

    await tutorRegisterPage.fillForm({ ...TUTOR_TEST_DATA, cvPdfPath: undefined })
    await tutorRegisterPage.submit()
    await expect(tutorRegisterPage.cvPdfError).toContainText('currículum vitae (CV) es obligatorio')
    await expect(page).toHaveURL(/\/register\/tutor/)

    await tutorRegisterPage.uploadCvPdf(STUDENT_TEST_DATA.fotoPath)
    await expect(tutorRegisterPage.cvPdfError).toContainText('Solo se permiten archivos PDF')
    await expect(page.getByText('estudiante.png', { exact: true })).toHaveCount(0)

    await tutorRegisterPage.uploadCvPdf(TUTOR_TEST_DATA.cvPdfPath)
    await expect(tutorRegisterPage.cvPdfError).toHaveCount(0)
    await expect(page.getByText('cv.pdf', { exact: true })).toBeVisible()

    await page.getByRole('button', { name: 'Quitar Currículum vitae (PDF)' }).click()
    await expect(page.getByText('cv.pdf', { exact: true })).toHaveCount(0)
  })
})
