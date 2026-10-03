# Pruebas de HU-22 y HU-23 — EduConnect Fase 2

| | |
|---|---|
| **Integrante** | Carnet 202201139 |
| **Historias** | HU-22 (fotografía y carnet PDF del estudiante) · HU-23 (CV PDF del tutor) |
| **Rama** | `feature/202201139-registro-documentos-carnet-cv` |
| **Fecha de ejecución** | 03/10/2026 |

## 1. Resumen

| Tipo de prueba | Requisito | Entregado | Resultado |
|---|---|---|---|
| Pruebas unitarias | mínimo 5 | **24 métodos (34 casos)** | 34 de 34 correctos |
| Pruebas E2E | mínimo 1 | **2** (una por historia) | 2 de 2 correctas |
| Pruebas manuales y de API | — | 13 casos | 13 de 13 correctos |

Suite completa del backend tras integrar ambas historias: **193 pruebas, 0 con errores**. Suite E2E completa: **8 pruebas, 8 correctas**.

## 2. Entorno de ejecución

| Elemento | Detalle |
|---|---|
| Sistema operativo | Windows |
| Backend | .NET 10.0.11 |
| Pruebas unitarias | xUnit 3.1.4, Moq 4.21, EF Core InMemory |
| E2E | Playwright 1.49.0, Chromium 131 |
| Entorno E2E | `docker-compose.e2e.yaml` (Oracle XE, API y frontend en contenedores) |
| Archivos de prueba | `e2e/fixtures/documents/carnet.pdf`, `e2e/fixtures/documents/cv.pdf`, `e2e/fixtures/images/` |

## 3. Cómo ejecutar las pruebas

**Unitarias (backend):**

```powershell
dotnet test edu-connect-service/tests/edu-connect-service.Api.UnitTests/edu-connect-service.Api.UnitTests.csproj
```

**E2E:**

```powershell
docker compose -f docker-compose.e2e.yaml up -d --build oracle-db api frontend
cd e2e
npm install
npx playwright install chromium
$env:E2E_BASE_URL = "http://localhost:5173"
npx playwright test --headed
cd ..
docker compose -f docker-compose.e2e.yaml down -v --remove-orphans
```

Requisitos previos del E2E: `.env.e2e` en la raíz del repositorio y el archivo `e2e/fixtures/docs/auth2-ayd1.txt` generado con las claves de ese entorno.

## 4. Pruebas unitarias

Ubicación: `edu-connect-service/tests/edu-connect-service.Api.UnitTests/`

**Resultado de la ejecución:** total 193, con errores 0, correctas 193. Todos los casos de las tablas siguientes se ejecutaron en esa corrida y pasaron.

### 4.1 `PdfFileValidatorTests` — validación de archivos PDF (20 casos)

Archivo: `Shared/Validation/PdfFileValidatorTests.cs`. Lo reutilizan HU-22 y HU-23.

| # | Método | Caso de prueba | Resultado esperado | Resultado obtenido |
|---|---|---|---|---|
| 1 | `IsValidPdf_WithValidPdf_ReturnsTrue` | `carnet.pdf` con `application/pdf` | Válido | Correcto |
| 2 | `IsValidPdf_WithValidPdf_ReturnsTrue` | `CARNET.PDF` en mayúsculas | Válido | Correcto |
| 3 | `IsValidPdf_WithValidPdf_ReturnsTrue` | `carnet.pdf` con `application/x-pdf` | Válido | Correcto |
| 4 | `IsValidPdf_WithValidPdf_ReturnsTrue` | `carnet.pdf` sin tipo de contenido | Válido | Correcto |
| 5 | `IsValidPdf_WithNullFile_ReturnsFalse` | Archivo nulo | Inválido | Correcto |
| 6 | `IsValidPdf_WithEmptyFile_ReturnsFalse` | Archivo de 0 bytes | Inválido | Correcto |
| 7 | `IsValidPdf_WithNonPdfExtension_ReturnsFalse` | `carnet.png` | Inválido | Correcto |
| 8 | `IsValidPdf_WithNonPdfExtension_ReturnsFalse` | `carnet.jpg` | Inválido | Correcto |
| 9 | `IsValidPdf_WithNonPdfExtension_ReturnsFalse` | `carnet.docx` | Inválido | Correcto |
| 10 | `IsValidPdf_WithNonPdfExtension_ReturnsFalse` | `carnet.exe` | Inválido | Correcto |
| 11 | `IsValidPdf_WithNonPdfExtension_ReturnsFalse` | `carnet.pdf.exe` (doble extensión) | Inválido | Correcto |
| 12 | `IsValidPdf_WithNonPdfExtension_ReturnsFalse` | `carnet` sin extensión | Inválido | Correcto |
| 13 | `IsValidPdf_WithNonPdfContentType_ReturnsFalse` | Extensión `.pdf` con `image/png` | Inválido | Correcto |
| 14 | `IsValidPdf_WithNonPdfContentType_ReturnsFalse` | Extensión `.pdf` con `text/plain` | Inválido | Correcto |
| 15 | `IsValidPdf_WithNonPdfContentType_ReturnsFalse` | Extensión `.pdf` con `application/octet-stream` | Inválido | Correcto |
| 16 | `IsValidPdf_WithPdfExtensionButWithoutPdfSignature_ReturnsFalse` | Texto plano renombrado a `.pdf` | Inválido: no empieza con `%PDF-` | Correcto |
| 17 | `IsValidPdf_WithFileShorterThanSignature_ReturnsFalse` | Contenido `%PD` (más corto que la firma) | Inválido | Correcto |
| 18 | `ExceedsMaxSize_WithFileExactlyAtLimit_ReturnsFalse` | Archivo de exactamente 5 MB | No excede el límite | Correcto |
| 19 | `ExceedsMaxSize_WithFileOverLimit_ReturnsTrue` | Archivo de 5 MB + 1 byte | Excede el límite | Correcto |
| 20 | `ExceedsMaxSize_WithNullFile_ReturnsFalse` | Archivo nulo | No excede el límite | Correcto |

### 4.2 `RegistrarEstudianteEndpointTests` — HU-22 (7 casos)

Archivo: `Features/Estudiantes/RegistrarEstudiante/RegistrarEstudianteEndpointTests.cs`

| # | Método | Caso de prueba | Resultado esperado | Resultado obtenido |
|---|---|---|---|---|
| 1 | `HandleAsync_WithoutFotografia_Returns400AndDoesNotUploadFiles` | Registro sin fotografía | 400 «Fotografía obligatoria»; no se sube ningún archivo; no se crea el estudiante | Correcto |
| 2 | `HandleAsync_WithoutDocumentoCarnet_Returns400AndDoesNotUploadFiles` | Registro sin carnet PDF | 400 «Documento PDF obligatorio»; no se sube ningún archivo | Correcto |
| 3 | `HandleAsync_WithCarnetThatIsNotPdf_Returns400AndDoesNotUploadFiles` | Carnet enviado como imagen PNG | 400 «Documento PDF inválido»; no se sube ningún archivo | Correcto |
| 4 | `HandleAsync_WithFileRenamedToPdf_Returns400` | Texto renombrado a `.pdf` | 400 «Documento PDF inválido» | Correcto |
| 5 | `HandleAsync_WithCarnetPdfOverMaxSize_Returns400` | PDF de más de 5 MB | 400 «Documento PDF demasiado grande» | Correcto |
| 6 | `HandleAsync_WithValidFiles_Returns201AndStoresBothFileKeys` | Foto y carnet PDF válidos | 201; se guardan las rutas de ambos archivos; la respuesta incluye la URL firmada del carnet; estado «PENDIENTE» | Correcto |
| 7 | `HandleAsync_WhenCarnetUploadFails_DeletesUploadedPhotoAndReturns500` | Falla la subida del carnet tras subir la foto | 500; se elimina la foto ya subida; no se crea el estudiante | Correcto |

### 4.3 `RegistrarTutorEndpointTests` — HU-23 (7 casos)

Archivo: `Features/Tutores/RegistrarTutor/RegistrarTutorEndpointTests.cs`

| # | Método | Caso de prueba | Resultado esperado | Resultado obtenido |
|---|---|---|---|---|
| 1 | `HandleAsync_WithoutDocumentoCv_Returns400AndDoesNotUploadFiles` | Registro sin CV | 400 «Currículum PDF obligatorio»; no se sube ningún archivo; no se crea el tutor | Correcto |
| 2 | `HandleAsync_WithEmptyDocumentoCv_Returns400` | CV de 0 bytes | 400 «Currículum PDF obligatorio» | Correcto |
| 3 | `HandleAsync_WithCvThatIsNotPdf_Returns400AndDoesNotUploadFiles` | CV enviado como imagen PNG | 400 «Currículum PDF inválido»; no se sube ningún archivo | Correcto |
| 4 | `HandleAsync_WithFileRenamedToPdf_Returns400` | Texto renombrado a `.pdf` | 400 «Currículum PDF inválido» | Correcto |
| 5 | `HandleAsync_WithCvPdfOverMaxSize_Returns400` | PDF de más de 5 MB | 400 «Currículum PDF demasiado grande» | Correcto |
| 6 | `HandleAsync_WithValidFiles_Returns201AndStoresBothFileKeys` | Foto y CV PDF válidos | 201; se guardan las rutas de ambos archivos; la respuesta incluye la URL firmada del CV; estado «PENDIENTE» | Correcto |
| 7 | `HandleAsync_WhenCvUploadFails_DeletesUploadedPhotoAndReturns500` | Falla la subida del CV tras subir la foto | 500; se elimina la foto ya subida; no se crea el tutor | Correcto |

**Salida de la ejecución** (`dotnet test`, extracto):

```text
[xUnit.net 00:00:00.00] xUnit.net VSTest Adapter v3.1.4+50e68bbb8b (64-bit .NET 10.0.11)
[xUnit.net 00:00:00.17]   Discovering: edu-connect-service.Api.UnitTests
[xUnit.net 00:00:00.26]   Discovered:  edu-connect-service.Api.UnitTests
[xUnit.net 00:00:00.32]   Starting:    edu-connect-service.Api.UnitTests
[xUnit.net 00:00:03.12]   Finished:    edu-connect-service.Api.UnitTests
  edu-connect-service.Api.UnitTests net10.0 realizado correctamente prueba (4.1s)

Resumen de pruebas: total: 193; con errores: 0; correcto: 193; omitido: 0; duración: 4.0 s
```

## 5. Pruebas E2E (Playwright)

Ubicación: `e2e/tests/auth/`

### 5.1 HU-22 — `register.spec.ts`

**Prueba:** «HU-22: el registro de estudiante exige fotografía y carnet en PDF»

| Paso | Acción | Resultado esperado |
|---|---|---|
| 1 | Abrir `/register/student`, llenar los datos y enviar sin foto ni carnet | Se muestran los errores de fotografía y de carnet obligatorios; no se registra y la URL no cambia |
| 2 | Subir una imagen PNG en el campo del carnet | Se rechaza con «Solo se permiten archivos PDF» y el archivo no queda seleccionado |
| 3 | Subir un PDF válido | Desaparece el error y se muestra `carnet.pdf` |
| 4 | Pulsar **Quitar** en el carnet | El archivo deja de mostrarse |
| 5 | Subir la fotografía y pulsar **Quitar fotografía** | La vista previa vuelve a la imagen por defecto |

**Resultado obtenido:** correcta (1.6 s).

Además, la prueba existente «Registro de estudiante» se actualizó para adjuntar el carnet PDF: correcta (3.1 s).

### 5.2 HU-23 — `register-tutor-cv.spec.ts`

**Prueba:** «HU-23 - Registro de tutor con currículum en PDF › exige el CV, solo acepta PDF y permite corregir el archivo»

| Paso | Acción | Resultado esperado |
|---|---|---|
| 1 | Abrir `/register/tutor`, llenar todo salvo el CV y enviar | Error «currículum vitae (CV) es obligatorio»; no se registra |
| 2 | Subir una imagen en el campo del CV | Se rechaza con «Solo se permiten archivos PDF» |
| 3 | Subir un PDF válido | Desaparece el error y se muestra `cv.pdf` |
| 4 | Pulsar **Quitar** | El archivo deja de mostrarse |

**Resultado obtenido:** correcta (2.1 s). La prueba existente «Registro de tutor» se actualizó para adjuntar el CV: correcta (2.4 s).

### 5.3 Resultado de la suite E2E completa

| # | Prueba | Resultado |
|---|---|---|
| 1 | Login: muestra la página con sus elementos principales | Correcta |
| 2 | Login: falla con contraseña incorrecta y muestra el error | Correcta |
| 3 | Login: administrador con autenticación en 2 pasos | Correcta |
| 4 | **HU-23:** exige el CV, solo acepta PDF y permite corregir el archivo | Correcta |
| 5 | Registro de estudiante | Correcta |
| 6 | **HU-22:** exige fotografía y carnet en PDF | Correcta |
| 7 | Registro de tutor | Correcta |
| 8 | Aprobación de estudiante y tutor por el administrador | Correcta |

**Total: 8 correctas en 28.4 s.**

**Salida de la ejecución** (`npx playwright test --headed`, extracto):

```text
Running 8 tests using 1 worker

  ✓  1 …tenticación - Inicio de Sesión › Debe mostrar la página de inicio de sesión con sus elementos principales (3.1s)
  ✓  2 …enticación - Inicio de Sesión › Debe fallar el login con contraseña incorrecta y mostrar mensaje de error (2.6s)
  ✓  3 …de Sesión › Debe iniciar sesión exitosamente con credenciales de administrador y autenticación en 2 pasos (2.7s)
  ✓  4 …23 - Registro de tutor con currículum en PDF › exige el CV, solo acepta PDF y permite corregir el archivo (2.1s)
  ✓  5 …gister.spec.ts:9:7 › Módulo de Autenticación - Registro y Aprobación de Usuarios › Registro de estudiante (3.1s)
  ✓  6 …n - Registro y Aprobación de Usuarios › HU-22: el registro de estudiante exige fotografía y carnet en PDF (1.6s)
  ✓  7 …h\register.spec.ts:64:7 › Módulo de Autenticación - Registro y Aprobación de Usuarios › Registro de tutor (2.4s)
  ✓  8 …Autenticación - Registro y Aprobación de Usuarios › Aprobación de estudiante y tutor por el administrador (8.3s)

  8 passed (28.4s)
```

## 6. Cobertura de los criterios de aceptación

### HU-22 — Fotografía y carnet PDF del estudiante

| Criterio de aceptación | Unitaria | E2E | Manual / API | Resultado |
|---|---|---|---|---|
| Solicitar la fotografía e impedir el registro sin ella | 4.2 #1 | 5.1 paso 1 | Formulario vacío | Cumple |
| El carnet solo admite PDF y rechaza cualquier otro tipo | 4.1 #7–17, 4.2 #3–4 | 5.1 paso 2 | API: A2 y A3 | Cumple |
| Poder modificar la fotografía o el PDF elegidos | — | 5.1 pasos 3–5 | Botones Cambiar y Quitar | Cumple |
| El carnet PDF es obligatorio e impide el registro | 4.2 #2 | 5.1 paso 1 | API: A1 | Cumple |

### HU-23 — CV PDF del tutor

| Criterio de aceptación | Unitaria | E2E | Manual / API | Resultado |
|---|---|---|---|---|
| El CV es obligatorio e impide el registro | 4.3 #1–2 | 5.2 paso 1 | API: A4 | Cumple |
| Solo admite PDF y rechaza cualquier otra extensión | 4.1 #7–17, 4.3 #3–5 | 5.2 paso 2 | API: A5 y A6 | Cumple |
| Poder modificar el PDF elegido | — | 5.2 pasos 3–4 | Botones Cambiar y Quitar | Cumple |

## 7. Pruebas manuales en el navegador

| # | Historia | Caso | Resultado esperado | Resultado obtenido |
|---|---|---|---|---|
| M1 | HU-22 | Enviar el registro sin foto ni carnet | Errores inline en ambos campos; no se envía | Correcto |
| M2 | HU-22 | Registrar con foto y carnet PDF válidos | Registro exitoso | Correcto |
| M3 | HU-22 | El administrador abre **Ver carnet** | El PDF se muestra embebido sin descargarlo | Correcto |
| M4 | HU-23 | Enviar el registro de tutor sin CV | Error de CV obligatorio | Correcto |
| M5 | HU-23 | Subir un archivo `.txt` como CV | Se rechaza e indica el motivo | Correcto |
| M6 | HU-23 | Subir un PDF y usar Cambiar y Quitar | Se muestra nombre y tamaño; se puede cambiar o quitar | Correcto |
| M7 | HU-23 | Registrar un tutor con CV válido y abrir **Ver CV** como administrador | Registro exitoso; el CV se ve en el visor | Correcto |

## 8. Pruebas directas al API

Se enviaron peticiones `multipart/form-data` con `curl` para comprobar que el servidor valida por sí mismo, sin depender del frontend.

| # | Endpoint | Caso | Resultado esperado | Resultado obtenido |
|---|---|---|---|---|
| A1 | `POST /api/estudiantes/registro` | Sin carnet | HTTP 400 «Documento PDF obligatorio» | HTTP 400, «Documento PDF obligatorio» |
| A2 | `POST /api/estudiantes/registro` | Texto renombrado a `.pdf` | HTTP 400 «Documento PDF inválido» | HTTP 400, «Documento PDF inválido» |
| A3 | `POST /api/estudiantes/registro` | Imagen como carnet | HTTP 400 «Documento PDF inválido» | HTTP 400, «Documento PDF inválido» |
| A4 | `POST /api/tutores/registro` | Sin CV | HTTP 400 «Currículum PDF obligatorio» | HTTP 400, «Currículum PDF obligatorio» |
| A5 | `POST /api/tutores/registro` | Texto renombrado a `.pdf` | HTTP 400 «Currículum PDF inválido» | HTTP 400, «Currículum PDF inválido» |
| A6 | `POST /api/tutores/registro` | Imagen como CV | HTTP 400 «Currículum PDF inválido» | HTTP 400, «Currículum PDF inválido» |

En ninguno de estos casos se creó un usuario.

**Respuestas obtenidas** (extracto: se omiten los campos `type` y `traceId`):

```text
# A1  POST /api/estudiantes/registro  (sin carnet)
{"title":"Documento PDF obligatorio","status":400,"detail":"El archivo PDF con el carnet escaneado es obligatorio para completar el registro."}
HTTP 400

# A2  POST /api/estudiantes/registro  (texto renombrado a .pdf)
{"title":"Documento PDF inválido","status":400,"detail":"El carnet escaneado debe ser un archivo PDF válido. No se aceptan otros tipos de archivo."}
HTTP 400

# A3  POST /api/estudiantes/registro  (imagen como carnet)
{"title":"Documento PDF inválido","status":400,"detail":"El carnet escaneado debe ser un archivo PDF válido. No se aceptan otros tipos de archivo."}
HTTP 400

# A4  POST /api/tutores/registro  (sin CV)
{"title":"Currículum PDF obligatorio","status":400,"detail":"El archivo PDF con el currículum vitae (CV) es obligatorio para completar el registro."}
HTTP 400

# A5  POST /api/tutores/registro  (texto renombrado a .pdf)
{"title":"Currículum PDF inválido","status":400,"detail":"El currículum vitae debe ser un archivo PDF válido. No se aceptan otros tipos de archivo."}
HTTP 400

# A6  POST /api/tutores/registro  (imagen como CV)
{"title":"Currículum PDF inválido","status":400,"detail":"El currículum vitae debe ser un archivo PDF válido. No se aceptan otros tipos de archivo."}
HTTP 400
```

## 9. Pruebas de regresión

Los cambios de HU-22 modificaron `S3Service` y `BaseAvatarUpload`, que usan otras funcionalidades. Se verificó que siguen funcionando:

| Funcionalidad | Resultado esperado | Resultado obtenido |
|---|---|---|
| Registro de tutor con foto y vista previa | Se elige la foto y se registra | Correcto |
| Cambio de foto de perfil del tutor | Se guarda y se muestra la foto nueva | Correcto |
| Cambio de foto de perfil del estudiante | Se guarda y se muestra la foto nueva | Correcto |
| Pruebas unitarias existentes (186 previas) | Siguen pasando | Correcto |

## 10. Defecto encontrado durante las pruebas

| | |
|---|---|
| **Caso** | Enviar el formulario de registro vacío y luego elegir un archivo que no es PDF en el campo del carnet |
| **Resultado esperado** | Mostrar «Solo se permiten archivos PDF…» |
| **Resultado obtenido** | Seguía visible el mensaje anterior, «el archivo es obligatorio» |
| **Causa** | `BaseFileUpload` daba prioridad al error del formulario sobre el error de la última acción del usuario |
| **Corrección** | Se prioriza el error local (commit `2e0f1f2`: *fix(ui): priorizar el motivo de rechazo del archivo en BaseFileUpload*) |
| **Verificación** | Las pruebas E2E de HU-22 y HU-23 reproducen esta secuencia y pasan |

## 11. Observaciones

- **Fuera del alcance de HU-22 y HU-23:** el inicio de sesión busca el correo tal como se escribe, mientras que el registro lo guarda en minúsculas. Un usuario que escribe su correo con mayúsculas recibe «credenciales incorrectas». Se recomienda una historia aparte para normalizar el correo en `LoginEndpoint`.
- Las pruebas unitarias también se ejecutan automáticamente en el pipeline de GitHub Actions al hacer push a `develop` o `main`.

## 12. Evidencias

Las evidencias de ejecución son los registros (logs) incluidos en este documento:

| Evidencia | Dónde |
|---|---|
| Ejecución de las pruebas unitarias: 193 correctas, 0 con errores | Sección 4 |
| Ejecución de la suite E2E: 8 correctas | Sección 5.3 |
| Respuestas del API en las pruebas directas | Sección 8 |
| Código de las pruebas unitarias | `edu-connect-service/tests/edu-connect-service.Api.UnitTests/` |
| Código de las pruebas E2E | `e2e/tests/auth/` |

Al integrar los cambios a `develop`, las pruebas unitarias también se ejecutan en el job `test` del workflow `.github/workflows/backend-deploy.yaml`. El reporte HTML de Playwright se genera localmente en `e2e/playwright-report/` y no se versiona.
