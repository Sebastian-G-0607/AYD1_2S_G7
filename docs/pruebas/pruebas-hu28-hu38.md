# Pruebas de HU-28 y HU-38 — EduConnect Fase 2

| | |
|---|---|
| **Integrante** | Carnet 202201139 |
| **Historias** | HU-28 (reportar tutor) · HU-38 (reporte consolidado de calificación de tutores) |
| **Rama** | `feature/202201139-reportes-y-calificaciones-tutores` |
| **Fecha de ejecución** | _(completar)_ |

## 1. Resumen

| Tipo de prueba | Requisito | Entregado |
|---|---|---|
| Pruebas unitarias | mínimo 5 | 10 de HU-28 y 7 casos de HU-38 |
| Pruebas E2E | mínimo 1 | 2 (una por historia) |

## 2. Cómo ejecutar

```powershell
dotnet test edu-connect-service/tests/edu-connect-service.Api.UnitTests/edu-connect-service.Api.UnitTests.csproj
```

E2E (con `.env.e2e` y el archivo de la clave 2FA ya creados):

```powershell
cd e2e
npx.cmd playwright test tests/student/report-tutor.spec.ts tests/admin/tutor-ratings.spec.ts
```

Las dos pruebas E2E interceptan las respuestas de la API (`page.route`), igual que `attend-session.spec.ts`, por lo que no dependen de datos previos en la base.

## 3. Pruebas unitarias

### 3.1 `ReportarTutorEndpointTests` (HU-28)

| # | Caso | Resultado esperado | Obtenido |
|---|---|---|---|
| 1 | Reporte de sesión atendida propia con categoría y explicación válidas | 201 Created; reporte guardado con estado `PENDIENTE` | |
| 2 | Reporte de sesión pendiente y de sesión cancelada | 409 Conflict; no se crea reporte | |
| 3 | Explicación vacía o con menos de 10 caracteres | 400 Bad Request; no se crea reporte | |
| 4 | Categoría sin seleccionar o inexistente | 400 Bad Request; no se crea reporte | |
| 5 | Sesión de otro estudiante | 403 Forbidden; no se crea reporte | |
| 6 | Usuario con rol distinto de Estudiante | 403 Forbidden | |
| 7 | Segundo reporte sobre la misma sesión | 409 Conflict; solo existe un reporte | |
| 8 | Sesión inexistente | 404 Not Found | |
| 9 | Listado de categorías | 200 OK con las categorías ordenadas por id | |
| 10 | Datos iniciales de categorías | al menos 4 categorías, con nombres distintos | |

### 3.2 `CalificacionTutoresEndpointTests` (HU-38)

| # | Caso | Resultado esperado | Obtenido |
|---|---|---|---|
| 1 | Promedio sin calificaciones | `null` (se muestra "Sin calificar") | |
| 2 | Promedio de una sola calificación (5) | 5.0 | |
| 3 | Promedio de 4 y 5 | 4.5 | |
| 4 | Promedio de 0, 5 y 5 | 3.33 (dos decimales) | |
| 5 | Promedio de 0 y 0 | 0.0 (distinto de "sin calificar") | |
| 6 | Tutor con calificaciones 5, 4 y 3 y tutor sin ninguna | primer tutor 4.0 con 3 calificaciones; segundo con `null`, 0 calificaciones y "Sin especialidad registrada" | |
| 7 | Tutor activo y tutor dado de baja | solo aparece el tutor activo | |

## 4. Pruebas E2E

| Archivo | Flujo | Resultado esperado | Obtenido |
|---|---|---|---|
| `e2e/tests/student/report-tutor.spec.ts` | El estudiante abre el historial, intenta enviar vacío, con explicación corta y luego con datos válidos | "Reportar" solo aparece en sesiones atendidas; se muestran los mensajes de validación; se envía `{categoriaId: 2, motivo}`; la fila queda como "Reportada" | |
| `e2e/tests/admin/tutor-ratings.spec.ts` | El administrador abre el reporte de calificación de tutores | Promedios con dos decimales; "Sin calificar" para el tutor sin notas; orden descendente inicial y se invierte al pulsar "Promedio" | |

Los reportes creados con HU-28 se consultan en el módulo **Denuncias** del administrador (`/admin/denuncias`), que ya existe en `develop`.


