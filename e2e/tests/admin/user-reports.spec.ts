import { expect, test } from '@playwright/test'
import { AdminUserReportsPage } from '../../pages/admin-user-reports.page'
import { LoginPage } from '../../pages/login.page'

const TUTOR_REPORT_ID = 3601
const STUDENT_REPORT_ID = 3701
const REJECTION_MESSAGE =
  'La denuncia fue rechazada sin modificar el estado del tutor.'

test.describe('HU-36 / HU-37 - Gestión de denuncias (Módulo Administrador)', () => {
  test('permite revisar reportes y rechazar una denuncia hacia un tutor', async ({ page }) => {
    await page.route('**/api/administrador/reportes/tutores', route =>
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify([
          {
            id: TUTOR_REPORT_ID,
            sesionId: 4601,
            categoria: 'Incumplimiento',
            motivo: 'El tutor no asistió a la sesión programada.',
            tutorId: 1101,
            tutorNombreCompleto: 'Ana Tutora',
            tutorCorreo: 'ana.tutora@educonnect.com',
            estudianteDenuncianteId: 2101,
            estudianteDenuncianteNombreCompleto: 'Luis Estudiante',
            estudianteDenuncianteCorreo: 'luis.estudiante@educonnect.com',
            fechaSesion: '2026-10-01',
            fechaReporte: '2026-10-02T10:30:00Z',
            estado: 'PENDIENTE'
          }
        ])
      })
    )

    await page.route('**/api/administrador/reportes/estudiantes', route =>
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify([
          {
            id: STUDENT_REPORT_ID,
            sesionId: 4602,
            categoria: 'Conducta',
            motivo: 'El estudiante tuvo un comportamiento inadecuado.',
            estudianteId: 2102,
            estudianteNombreCompleto: 'Sofia Estudiante',
            estudianteCorreo: 'sofia.estudiante@educonnect.com',
            tutorDenuncianteId: 1102,
            tutorDenuncianteNombreCompleto: 'Carlos Tutor',
            tutorDenuncianteCorreo: 'carlos.tutor@educonnect.com',
            fechaSesion: '2026-10-03',
            fechaReporte: '2026-10-04T11:45:00Z',
            estado: 'PENDIENTE'
          }
        ])
      })
    )

    await page.route(
      `**/api/administrador/reportes/tutores/${TUTOR_REPORT_ID}/rechazar`,
      async route => {
        expect(route.request().method()).toBe('PUT')
        await route.fulfill({
          status: 200,
          contentType: 'application/json',
          body: JSON.stringify({
            reporteId: TUTOR_REPORT_ID,
            estadoReporte: 'DESESTIMADO',
            usuarioId: 1101,
            estadoUsuario: 'APROBADO',
            mensaje: REJECTION_MESSAGE
          })
        })
      }
    )

    const loginPage = new LoginPage(page)
    await loginPage.loginAsAdmin()

    const reportsPage = new AdminUserReportsPage(page)
    await reportsPage.goto()
    await reportsPage.expectHeadingVisible()

    await expect(reportsPage.tutorsTab).toBeVisible()
    await expect(reportsPage.studentsTab).toBeVisible()

    await reportsPage.selectTutorsTab()
    await expect(reportsPage.getVisibleReports()).toHaveCount(1)
    await reportsPage.selectStudentsTab()
    await expect(reportsPage.getVisibleReports()).toHaveCount(1)

    await reportsPage.selectTutorsTab()
    const pendingTutorReport = reportsPage.getReportById(TUTOR_REPORT_ID)
    await expect(pendingTutorReport).toContainText('PENDIENTE')
    await reportsPage.clickRejectReport(pendingTutorReport)
    await reportsPage.confirmRejection()

    await reportsPage.expectSuccessMessage(REJECTION_MESSAGE)
    await reportsPage.expectReportStatus(TUTOR_REPORT_ID, 'DESESTIMADO')
    await reportsPage.expectReportActionsHidden(TUTOR_REPORT_ID)
  })
})
