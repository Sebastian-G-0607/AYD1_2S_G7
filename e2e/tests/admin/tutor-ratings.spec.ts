import { expect, test } from '@playwright/test'

const calificaciones = [
  {
    tutorId: 1,
    nombreCompleto: 'Ana Pérez',
    especialidad: 'Física',
    promedioCalificacion: 4.5,
    totalCalificaciones: 8
  },
  {
    tutorId: 2,
    nombreCompleto: 'Beto Ruiz',
    especialidad: 'Química',
    promedioCalificacion: null,
    totalCalificaciones: 0
  },
  {
    tutorId: 3,
    nombreCompleto: 'Carla Gómez',
    especialidad: 'Álgebra Lineal',
    promedioCalificacion: 2.25,
    totalCalificaciones: 4
  }
]

test.describe('Reporte de calificación de tutores (admin)', () => {
  test('muestra promedios, marca los tutores sin calificar y permite ordenar', async ({ page }) => {
    await page.addInitScript(() => {
      localStorage.setItem('edu_auth_token', 'e2e-admin-token')
      localStorage.setItem('edu_auth_user', JSON.stringify({ rol: 'Administrador' }))
    })

    await page.route('**/administrador/reportes/calificacion-tutores', route =>
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(calificaciones)
      })
    )

    await page.goto('/admin/calificaciones-tutores')

    const filas = page.getByRole('row')
    await expect(page.getByRole('row').filter({ hasText: 'Ana Pérez' })).toContainText('4.50')
    await expect(page.getByRole('row').filter({ hasText: 'Beto Ruiz' })).toContainText(
      'Sin calificar'
    )

    // Orden inicial: mayor promedio primero y sin calificar al final
    await expect(filas.nth(1)).toContainText('Ana Pérez')
    await expect(filas.nth(2)).toContainText('Carla Gómez')
    await expect(filas.nth(3)).toContainText('Beto Ruiz')

    await page.getByRole('button', { name: /Ordenar por promedio/ }).click()
    await expect(filas.nth(1)).toContainText('Carla Gómez')
    await expect(filas.nth(2)).toContainText('Ana Pérez')
    await expect(filas.nth(3)).toContainText('Beto Ruiz')
  })
})
