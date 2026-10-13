import { expect, test } from '@playwright/test'

test.describe('Calificar atención de tutoría', () => {
  test('permite puntuar una sesión atendida y bloquea sesiones canceladas', async ({ page }) => {
    let submittedRating: unknown

    await page.addInitScript(() => {
      localStorage.setItem('edu_auth_token', 'e2e-student-token')
      localStorage.setItem('edu_auth_user', JSON.stringify({ rol: 'Estudiante' }))
    })

    await page.route('**/estudiantes/historial', route =>
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify([
          {
            sesionId: 42,
            fechaSesion: '2026-10-01',
            tutor: 'Sofía Ramírez',
            materia: 'Matemática',
            direccionTutoria: 'USAC',
            motivo: 'Repaso de ecuaciones',
            resumen: 'Se revisaron ecuaciones lineales',
            estado: 'ATENDIDA',
            yaReportada: false,
            yaCalificada: false,
            calificacionEstrellas: null
          },
          {
            sesionId: 43,
            fechaSesion: '2026-09-20',
            tutor: 'Carlos Méndez',
            materia: 'Física',
            direccionTutoria: 'USAC',
            motivo: 'Cinemática',
            resumen: null,
            estado: 'CANCELADA_TUTOR',
            yaReportada: false,
            yaCalificada: false,
            calificacionEstrellas: null
          }
        ])
      })
    )

    await page.route('**/estudiantes/sesiones/42/calificar-tutor', async route => {
      submittedRating = route.request().postDataJSON()
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({ sesionId: 42, estrellas: 0, comentario: 'Atención deficiente' })
      })
    })

    await page.goto('/estudiante/historial')

    const attendedRow = page.getByRole('row').filter({ hasText: 'Sofía Ramírez' })
    const cancelledRow = page.getByRole('row').filter({ hasText: 'Carlos Méndez' })
    await expect(attendedRow.getByRole('button', { name: 'Calificar atención' })).toBeVisible()
    await expect(cancelledRow.getByRole('button', { name: 'Calificar atención' })).toHaveCount(0)

    await attendedRow.getByRole('button', { name: 'Calificar atención' }).click()
    const submitButton = page.getByRole('button', { name: 'Guardar calificación' })
    await expect(submitButton).toBeDisabled()
    await page.getByRole('radio', { name: '0 estrellas' }).click()
    await page.getByLabel('Comentario (opcional)').fill('Atención deficiente')
    await submitButton.click()

    await expect(attendedRow.getByText('0 / 5 estrellas')).toBeVisible()
    expect(submittedRating).toEqual({ estrellas: 0, comentario: 'Atención deficiente' })
  })
})