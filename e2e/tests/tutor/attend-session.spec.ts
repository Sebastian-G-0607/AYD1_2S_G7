import { expect, test } from '@playwright/test'

test.describe('Atender sesión de tutoría', () => {
  test('envía el plan de estudio y quita la sesión de pendientes', async ({ page }) => {
    let submittedPlan: unknown

    await page.addInitScript(() => {
      localStorage.setItem('edu_auth_token', 'e2e-tutor-token')
      localStorage.setItem('edu_auth_user', JSON.stringify({ rol: 'Tutor' }))
    })

    await page.route('**/tutores/dashboard/estadisticas', route =>
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({
          sesionesPendientes: 1,
          pendientesHoy: 1,
          sesionesAtendidasMes: 0,
          sesionesCanceladas: 0
        })
      })
    )

    await page.route('**/sesiones/pendientes', route =>
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify([
          {
            id: 42,
            fecha: '03 Oct, 2026',
            hora: '10:00 AM',
            estudianteNombre: 'Ana López',
            estudianteId: '202200001',
            materia: 'Matemática',
            motivo: 'Repaso de ecuaciones',
            estado: 'PENDIENTE'
          }
        ])
      })
    )

    await page.route('**/sesiones/42/atender', async route => {
      submittedPlan = route.request().postDataJSON()
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({ id: 42, estado: 'ATENDIDA', resumen: '' })
      })
    })

    await page.goto('/tutor/dashboard')
    const sessionRow = page.getByRole('row').filter({ hasText: 'Ana López' })
    await expect(sessionRow).toBeVisible()
    await sessionRow.getByRole('button', { name: 'Atendido' }).click()

    await page.getByLabel('Dificultades identificadas *').fill('Se le dificulta despejar variables')
    await page.getByLabel('Nombre del recurso 1 *').fill('Guía de ejercicios')
    await page.getByLabel('Tipo de recurso 1 *').fill('Documento')
    await page
      .getByLabel('Descripción de uso del recurso 1 *')
      .fill('Resolver los ejercicios del 1 al 5')
    await page.getByRole('button', { name: 'Agregar recurso' }).click()
    await page.getByLabel('Nombre del recurso 2 *').fill('Video de apoyo')
    await page.getByLabel('Tipo de recurso 2 *').fill('Video')
    await page
      .getByLabel('Descripción de uso del recurso 2 *')
      .fill('Repasar el ejemplo de factorización')
    await page.getByRole('button', { name: 'Guardar y Finalizar' }).click()

    await expect(page.getByText('No tienes sesiones pendientes')).toBeVisible()
    expect(submittedPlan).toEqual({
      dificultadesIdentificadas: 'Se le dificulta despejar variables',
      recursos: [
        {
          nombre: 'Guía de ejercicios',
          tipo: 'Documento',
          descripcionUso: 'Resolver los ejercicios del 1 al 5'
        },
        {
          nombre: 'Video de apoyo',
          tipo: 'Video',
          descripcionUso: 'Repasar el ejemplo de factorización'
        }
      ]
    })
  })
})