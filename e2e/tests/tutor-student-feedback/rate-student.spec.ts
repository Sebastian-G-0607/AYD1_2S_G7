import { expect, test } from '@playwright/test'

test.describe('Calificar estudiante', () => {
  test('el tutor califica a un estudiante desde una sesión atendida', async ({ page }) => {
    let submittedRating: unknown

    await page.addInitScript(() => {
      localStorage.setItem('edu_auth_token', 'e2e-tutor-token')
      localStorage.setItem('edu_auth_user', JSON.stringify({ rol: 'Tutor' }))
    })

    await page.route('**/tutores/historial*', route =>
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({
          sessions: [
            {
              sesionId: 55,
              fecha: '2026-10-01',
              hora: '10:00 AM',
              estudiante: 'Ana López',
              correo: 'ana.lopez@educonnect.test',
              estado: 'ATENDIDA'
            }
          ]
        })
      })
    )

    await page.route('**/tutores/sesiones/55/calificar-estudiante', async route => {
      submittedRating = route.request().postDataJSON()
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({ sesionId: 55, estrellas: 4, comentario: 'Buena participación' })
      })
    })

    await page.goto('/tutor/historial')

    const sessionRow = page.getByRole('row').filter({ hasText: 'Ana López' })
    await expect(sessionRow).toBeVisible()
    await sessionRow.getByRole('button', { name: 'Calificar' }).click()

    await expect(page.getByText('Calificar estudiante')).toBeVisible()

    const stars = page.locator('button:has(span.material-symbols-outlined:text("star"))')
    await stars.nth(3).click()

    await page.getByPlaceholder('Describe brevemente el motivo de tu calificación').fill('Buena participación')
    await page.getByRole('button', { name: 'Guardar calificación' }).click()

    await expect(page.getByText('Calificar estudiante')).not.toBeVisible()
    expect(submittedRating).toEqual({
      estrellas: 4,
      comentario: 'Buena participación'
    })
  })
})