import { expect, test } from '@playwright/test'

const sesiones = [
  {
    sesionId: 7,
    fechaSesion: '2026-10-03',
    tutor: 'Carlos Mendoza',
    materia: 'Física',
    direccionTutoria: 'USAC T-3',
    motivo: 'Repaso de cinemática',
    resumen: 'Se repasó MRU y MRUA',
    estado: 'ATENDIDA',
    yaReportada: false
  },
  {
    sesionId: 8,
    fechaSesion: '2026-10-02',
    tutor: 'Gabriela Silva',
    materia: 'Química',
    direccionTutoria: 'USAC T-4',
    motivo: 'Balanceo de ecuaciones',
    resumen: null,
    estado: 'CANCELADA_ESTUDIANTE',
    yaReportada: false
  }
]

const categorias = [
  { id: 1, nombre: 'Ética y profesionalismo', descripcion: null },
  { id: 2, nombre: 'Negligencia académica', descripcion: null },
  { id: 3, nombre: 'Abuso y conducta inapropiada', descripcion: null },
  { id: 4, nombre: 'Falsificación de información', descripcion: null }
]

test.describe('Reportar tutor desde el historial de sesiones', () => {
  test('valida el formulario y envía el reporte de una sesión atendida', async ({ page }) => {
    let submitted: unknown

    await page.addInitScript(() => {
      localStorage.setItem('edu_auth_token', 'e2e-student-token')
      localStorage.setItem('edu_auth_user', JSON.stringify({ rol: 'Estudiante' }))
    })

    await page.route('**/estudiantes/historial', route =>
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(sesiones)
      })
    )
    await page.route('**/estudiantes/reportes-tutor/categorias', route =>
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(categorias)
      })
    )
    await page.route('**/estudiantes/sesiones/7/reportar-tutor', async route => {
      submitted = route.request().postDataJSON()
      await route.fulfill({
        status: 201,
        contentType: 'application/json',
        body: JSON.stringify({
          id: 1,
          sesionId: 7,
          categoria: 'Negligencia académica',
          estado: 'PENDIENTE',
          mensaje: 'El reporte fue enviado al administrador.'
        })
      })
    })

    await page.goto('/estudiante/historial')

    const atendida = page.getByRole('row').filter({ hasText: 'Carlos Mendoza' })
    const cancelada = page.getByRole('row').filter({ hasText: 'Gabriela Silva' })
    await expect(atendida.getByRole('button', { name: 'Reportar' })).toBeVisible()
    await expect(cancelada.getByRole('button', { name: 'Reportar' })).toHaveCount(0)

    await atendida.getByRole('button', { name: 'Reportar' }).click()
    await page.getByRole('button', { name: 'Enviar reporte' }).click()
    await expect(page.getByText('Debes seleccionar una categoría.')).toBeVisible()
    await expect(page.getByText('Debes explicar el motivo del reporte.')).toBeVisible()

    await page.getByLabel('Categoría del reporte').selectOption({ label: 'Negligencia académica' })
    await page.getByLabel('Explicación del motivo').fill('Corto')
    await page.getByRole('button', { name: 'Enviar reporte' }).click()
    await expect(page.getByText('al menos 10 caracteres')).toBeVisible()

    await page
      .getByLabel('Explicación del motivo')
      .fill('El tutor no se presentó a la hora acordada y no avisó')
    await page.getByRole('button', { name: 'Enviar reporte' }).click()

    await expect(page.getByText('Tu reporte fue enviado al administrador.')).toBeVisible()
    await expect(atendida.getByText('Reportada')).toBeVisible()
    expect(submitted).toEqual({
      categoriaId: 2,
      motivo: 'El tutor no se presentó a la hora acordada y no avisó'
    })
  })
})
