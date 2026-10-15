# Product Backlog: historias de usuario y trabajo asociado

> El Product Backlog es una lista ordenada y evolutiva de todo el trabajo necesario para desarrollar y mejorar un producto. Puede incluir historias de usuario, tareas técnicas, pruebas, documentación, despliegue y otros elementos necesarios para entregar valor.
>
> En este documento se presentan principalmente las historias de usuario del proyecto EduConnect. Las tareas derivadas de cada historia se gestionan en el tablero Kanban, mientras que la estimación, las dependencias y los criterios de aceptación se consultan en este Product Backlog.
>
> El proyecto se organizó en dos fases. La Fase 1 contiene las funcionalidades iniciales y la Fase 2 amplía historias existentes y agrega nuevas funcionalidades. La nomenclatura `[EXT - HUxxx]` indica que una historia extiende una historia creada anteriormente.

Este documento contiene las historias de usuario para el desarrollo de la plataforma EduConnect, organizadas por fases.
Cada historia describe una funcionalidad desde la perspectiva del usuario final y está priorizada según su importancia dentro del proyecto; la priorización se hizo al inicio de cada fase. Las prioridades están indicadas con cuadros de colores:

🟥 **Alta:** Funcionalidades críticas que deben implementarse primero, ya que en base a estas se desarrollarán otras más adelante.

🟨 **Media:** Funcionalidades importantes, pero que pueden desarrollarse después de las críticas.

🟦 **Baja:** Funcionalidades opcionales que agregan valor, pero no están contempladas como obligatorias en la primera fase del proyecto.

## Estimación del tamaño (T-Shirt Size)

> La estimación representa el tamaño relativo de una historia de usuario, no una cantidad exacta de horas. Las tallas permiten comparar la complejidad y comunicar el nivel de esfuerzo esperado.

Para estimar el tamaño de las historias de usuario se utilizó el método T-Shirt Size.

- Durante la Fase 1, cada miembro del equipo etiquetó las actividades del backlog de cada historia con las categorías XS, S, M, L y XL según su complejidad, siendo «XS» la de menor complejidad y «XL» la de mayor. El tamaño final de cada historia se asignó con la opinión conjunta del equipo, consolidando las estimaciones individuales en una única etiqueta.
- Durante la Fase 2, el equipo asignó una talla de forma global a cada una de las nuevas historias, considerando desde un principio el peso individual de las actividades del backlog. Igualmente, el tamaño final de cada historia se asignó con la opinión conjunta del equipo, consolidando las estimaciones individuales en una única etiqueta.

> **XS:** Historia de baja complejidad y rápida implementación.
> **S:** Historia sencilla, con pocos criterios de aceptación.
> **M:** Historia de complejidad media, que puede requerir integración con otras funcionalidades.
> **L:** Historia compleja, que involucra varios módulos o dependencias.
> **XL:** Historia muy compleja, que podría dividirse en varias historias más pequeñas.

Para poder sumar el esfuerzo de cada sprint, cada talla se expresa en puntos:

| Talla | XS | S | M | L | XL |
| :--- | :---: | :---: | :---: | :---: | :---: |
| **Puntos** | 1 | 2 | 3 | 5 | 8 |

---

## Product Backlog (Historias de Usuario)

El Sprint Backlog contiene las historias de usuario seleccionadas para cada sprint (ver `sprint-backlog.md`). Las tareas derivadas de cada historia y el responsable de cada tarea se encuentran en las tarjetas del tablero Kanban [https://edu-connect-g7.atlassian.net/jira/software/projects/ED/list?jql=project%20%3D%20ED%20ORDER%20BY%20cf%5B10019%5D%20ASC](https://edu-connect-g7.atlassian.net/jira/software/projects/ED/list?jql=project%20%3D%20ED%20ORDER%20BY%20cf%5B10019%5D%20ASC). La estimación, las dependencias y los criterios de aceptación se encuentran en este Product Backlog.

### Resumen general

| Fase | Sprints | Historias | Puntos |
| :--- | :---: | :---: | :---: |
| Fase 1 | 1 y 2 | 21 | 99 |
| Fase 2 | 3 y 4 | 19 | 65 (más 5 de tareas de documentación) |
| **Total** | 4 | **40** | **164** |

## FASE 1

| Código | Sprint | Módulo | Título de la Historia de Usuario | Prioridad | Dependencias | Puntos | Talla |
| :--- | :---: | :--- | :--- | :--- | :--- | :---: | :---: |
| **HU-01** | Sprint 1 | Registro y Autenticación | Registro de Estudiante | 🟥 Alta | Ninguna | 5 | L |
| **HU-02** | Sprint 1 | Registro y Autenticación | Registro de Tutor | 🟥 Alta | Ninguna | 5 | L |
| **HU-04** | Sprint 1 | Registro y Autenticación | Inicio de Sesión de Administrador (2FA) | 🟥 Alta | Ninguna | 5 | L |
| **HU-05** | Sprint 1 | Administrador | Aprobación de Registro de Estudiantes | 🟥 Alta | HU-01 | 3 | M |
| **HU-06** | Sprint 1 | Administrador | Aprobación de Registro de Tutores | 🟥 Alta | HU-02 | 3 | M |
| **HU-03** | Sprint 1 | Registro y Autenticación | Inicio de Sesión (Estudiantes y Tutores) | 🟥 Alta | HU-01, HU-02, HU-05, HU-06 | 3 | M |
| **HU-10** | Sprint 1 | Tutor | Establecer Horarios de Atención | 🟥 Alta | HU-03 | 5 | L |
| **HU-16** | Sprint 1 | Estudiante | Exploración y Búsqueda de Tutores | 🟥 Alta | HU-02, HU-06 | 5 | L |
| **HU-17** | Sprint 1 | Estudiante | Consulta de Horarios y Disponibilidad | 🟥 Alta | HU-10, HU-16 | 5 | L |
| **HU-18** | Sprint 1 | Estudiante | Programar Sesión de Tutoría | 🟥 Alta | HU-17 | 8 | XL |
| **HU-12** | Sprint 2 | Tutor | Gestión y Atención de Sesiones Pendientes | 🟥 Alta | HU-18 | 5 | L |
| **HU-13** | Sprint 2 | Tutor | Cancelación de Sesión por el Tutor | 🟥 Alta | HU-12 | 5 | L |
| **HU-19** | Sprint 2 | Estudiante | Gestión de Sesiones Activas | 🟨 Media | HU-18 | 5 | L |
| **HU-11** | Sprint 2 | Tutor | Actualizar Horarios de Atención | 🟨 Media | HU-10 | 5 | L |
| **HU-14** | Sprint 2 | Tutor | Historial de Sesiones | 🟥 Alta | HU-12, HU-13 | 3 | M |
| **HU-20** | Sprint 2 | Estudiante | Visualización de Historial de Sesiones | 🟦 Baja | HU-12, HU-13, HU-19 | 3 | M |
| **HU-15** | Sprint 2 | Tutor | Ver y actualizar perfil | 🟥 Alta | HU-02 | 5 | L |
| **HU-21** | Sprint 2 | Estudiante | Gestión de Perfil | 🟦 Baja | HU-01 | 5 | L |
| **HU-07** | Sprint 2 | Administrador | Gestión de Usuarios Activos | 🟨 Media | HU-05, HU-06 | 5 | L |
| **HU-09** | Sprint 2 | Administrador | Visualización de usuarios dados de baja | 🟨 Media | HU-05, HU-06, HU-07 | 3 | M |
| **HU-08** | Sprint 2 | Administrador | Generación de Reportes | 🟨 Media | HU-12, HU-18 | 8 | XL |
| **Total** | | | | | | **99** | |

## FASE 2

| Código | Sprint | Módulo | Título de la Historia de Usuario | Prioridad | Dependencias | Puntos | Talla |
| :--- | :---: | :--- | :--- | :--- | :--- | :---: | :---: |
| **HU-22** | Sprint 3 | Registro y autenticación | [EXT - HU-01] Fotografía y carnet del estudiante | 🟥 Alta | Ninguna | 3 | M |
| **HU-23** | Sprint 3 | Registro y autenticación | [EXT - HU-02] CV del tutor | 🟥 Alta | Ninguna | 3 | M |
| **HU-24** | Sprint 3 | Registro y autenticación | [EXT - HU-03] Validación de correo electrónico con token | 🟥 Alta | HU-22, HU-23 | 3 | M |
| **HU-32** | Sprint 3 | Administrador | [EXT - HU-05] Aprobación de estudiantes con visor PDF incrustado | 🟥 Alta | HU-22, HU-24 | 5 | L |
| **HU-33** | Sprint 3 | Administrador | [EXT - HU-06] Aprobación de tutores con visor PDF incrustado | 🟥 Alta | HU-23, HU-24 | 5 | L |
| **HU-29** | Sprint 3 | Tutor | [EXT - HU-12] Atender estudiante | 🟥 Alta | HU-33 | 3 | M |
| **HU-25** | Sprint 3 | Estudiante | Plan de estudio de estudiante | 🟥 Alta | HU-29 | 3 | M |
| **HU-26** | Sprint 3 | Estudiante | [EXT - HU-25] Imprimir plan de estudio del estudiante | 🟨 Media | HU-25 | 2 | S |
| **HU-35** | Sprint 3 | Administrador | [EXT - HU-07] Ver, actualizar y dar de baja tutores | 🟨 Media | HU-33 | 3 | M |
| **HU-27** | Sprint 4 | Estudiante | Calificar atención de tutoría | 🟨 Media | HU-29 | 2 | S |
| **HU-28** | Sprint 4 | Estudiante | Reportar tutor | 🟨 Media | HU-29 | 2 | S |
| **HU-30** | Sprint 4 | Tutor | Calificar estudiante | 🟨 Media | HU-29 | 2 | S |
| **HU-31** | Sprint 4 | Tutor | Reportar estudiante | 🟨 Media | HU-29 | 2 | S |
| **HU-34** | Sprint 4 | Administrador | [EXT - HU-07] Ver, actualizar y dar de baja estudiantes | 🟨 Media | HU-32 | 5 | L |
| **HU-36** | Sprint 4 | Administrador | Gestión de reportes hacia tutores | 🟨 Media | HU-28 | 3 | M |
| **HU-37** | Sprint 4 | Administrador | Gestión de reportes hacia estudiantes | 🟨 Media | HU-31 | 3 | M |
| **HU-38** | Sprint 4 | Administrador | Reporte consolidado de calificación de tutores | 🟦 Baja | HU-27 | 3 | M |
| **HU-39** | Sprint 4 | Administrador | Reporte consolidado de calificación de estudiantes | 🟦 Baja | HU-30 | 5 | L |
| **HU-40** | Sprint 4 | Administrador | [EXT - HU-08] Reportes analíticos avanzados para toma de decisiones | 🟦 Baja | HU-29, HU-32, HU-33 | 8 | XL |
| **Total** | | | | | | **65** | |

## Relación entre historias originales y extensiones de la Fase 2

La Fase 2 amplía historias ya construidas. Las historias nuevas, sin historia original, son HU-25, HU-27, HU-28, HU-30, HU-31, HU-36, HU-37, HU-38 y HU-39.

| Historia original | Extensión | Qué amplía |
| :--- | :--- | :--- |
| **HU-01** Registro de Estudiante | **HU-22** Fotografía y carnet del estudiante | Fotografía obligatoria y carnet escaneado en PDF para validar la identidad del estudiante. |
| **HU-02** Registro de Tutor | **HU-23** CV del tutor | CV en PDF obligatorio para respaldar el perfil del tutor. |
| **HU-03** Inicio de Sesión (Estudiantes y Tutores) | **HU-24** Validación de correo electrónico con token | Verificación del correo con un token de 6 dígitos en el primer inicio de sesión. |
| **HU-05** Aprobación de Registro de Estudiantes | **HU-32** Aprobación de estudiantes con visor PDF incrustado | Visor PDF embebido para revisar el carnet y activación de la cuenta al aprobar. |
| **HU-06** Aprobación de Registro de Tutores | **HU-33** Aprobación de tutores con visor PDF incrustado | Visor PDF embebido para revisar el CV y habilitación de la cuenta al aprobar. |
| **HU-07** Gestión de Usuarios Activos | **HU-34** Ver, actualizar y dar de baja estudiantes | Edición de los datos del estudiante y confirmación explícita antes de la baja. |
| **HU-07** Gestión de Usuarios Activos | **HU-35** Ver, actualizar y dar de baja tutores | Edición de los datos del tutor, confirmación antes de la baja y bloqueo de nuevas reservas. |
| **HU-08** Generación de Reportes | **HU-40** Reportes analíticos avanzados para toma de decisiones | Reportes analíticos adicionales, con gráficos y datos en tiempo real. |
| **HU-12** Gestión y Atención de Sesiones Pendientes | **HU-29** Atender estudiante | El formulario de atención registra el plan de estudio (dificultades y recursos recomendados). |
| **HU-25** Plan de estudio de estudiante | **HU-26** Imprimir plan de estudio del estudiante | Generación del plan de estudio en un archivo PDF con encabezado, tabla de recursos y pie. |

## Estimación detallada de la Fase 2

Cada integrante votó el esfuerzo de cada historia y el equipo consolidó el valor final. Los votos están en puntos.

| ID | Historia / Tarea | Keitlyn | Josué | Christian | Eduardo | Sebas | [Tu nombre] | Promedio | Puntos Finales | Justificación del Consenso |
| :--- | :--- | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :--- |
| **HU-22** | Subida de foto y carnet PDF | 3 | 3 | 3 | 2 | 5 | 3 | **3.17** | **3** | Manejo de subida obligatoria de archivos en base de datos/almacenamiento y validación estricta de extensiones. |
| **HU-23** | Subida de CV del tutor | 3 | 3 | 2 | 3 | 3 | 5 | **3.17** | **3** | Complejidad idéntica a la HU-22 con reglas de reemplazo y validación de PDF. |
| **HU-24** | Verificación con token de 6 dígitos | 3 | 5 | 3 | 3 | 2 | 3 | **3.17** | **3** | Integración del servicio de envío de correos, generación de tokens y flujo de bloqueo/reintento. |
| **HU-25** | Plan de estudio de estudiante | 3 | 3 | 5 | 2 | 3 | 3 | **3.17** | **3** | Consulta relacional de sesiones, recursos dinámicos asociados y dificultades. |
| **HU-26** | Generación de reporte en PDF | 2 | 2 | 3 | 2 | 2 | 2 | **2.17** | **2** | Maquetación con librería PDF usando encabezados, tablas y pie de firma. |
| **HU-27** | Calificar tutoría | 2 | 2 | 2 | 3 | 1 | 2 | **2.00** | **2** | Formulario simple de estrellas (0 a 5) con validación de estado «Atendido». |
| **HU-28** | Reportar tutor | 2 | 2 | 2 | 2 | 3 | 2 | **2.17** | **2** | Formulario de reporte con catálogo de categorías y notificación al admin. |
| **HU-29** | Atender estudiante | 3 | 3 | 3 | 5 | 3 | 2 | **3.17** | **3** | Registro dinámico múltiple de recursos y cambio transaccional de estado de la tutoría. |
| **HU-30** | Calificar estudiante | 2 | 2 | 2 | 2 | 2 | 3 | **2.17** | **2** | Mecánica estándar idéntica a la HU-27 para tutores. |
| **HU-31** | Reportar estudiante | 2 | 2 | 3 | 2 | 2 | 2 | **2.17** | **2** | Mecánica análoga a la HU-28. |
| **HU-32** | Aprobación estudiante con visor PDF | 5 | 5 | 5 | 5 | 3 | 5 | **4.67** | **5** | Componente visor embebido sin descarga, lógica de aprobación/rechazo y mails transaccionales. |
| **HU-33** | Aprobación tutor con visor PDF | 5 | 5 | 5 | 5 | 5 | 3 | **4.67** | **5** | Alta carga visual, integración del PDF embebido y notificación por correo. |
| **HU-34** | CRUD y baja de estudiantes | 5 | 5 | 3 | 5 | 5 | 5 | **4.67** | **5** | Mantenimiento completo de perfiles, restricciones de email readonly y modal de confirmación. |
| **HU-35** | CRUD y baja de tutores | 3 | 3 | 5 | 3 | 3 | 3 | **3.33** | **3** | Reutiliza los patrones de la HU-34, reduciendo el esfuerzo relativo. |
| **HU-36** | Gestión de reportes hacia tutores | 3 | 3 | 3 | 3 | 5 | 2 | **3.17** | **3** | Bandeja de auditoría, desestimación y ejecución de bajas. |
| **HU-37** | Gestión de reportes hacia estudiantes | 3 | 3 | 3 | 2 | 3 | 3 | **2.83** | **3** | Flujo de resolución de quejas del administrador similar a la HU-36. |
| **HU-38** | Consolidado de calificaciones tutores | 3 | 3 | 2 | 3 | 5 | 3 | **3.17** | **3** | Consulta agregada (AVG, COUNT) con filtros de ordenamiento. |
| **HU-39** | Consolidado de notas estudiantes | 5 | 5 | 5 | 3 | 5 | 5 | **4.67** | **5** | Requiere paginación de padrón completo y cálculo en tiempo real. |
| **HU-40** | Reportes analíticos avanzados | 8 | 8 | 8 | 8 | 5 | 8 | **7.50** | **8** | Implementación de librerías de charts interactivos y agregaciones complejas. |
| **EXTRA-1** | Manual de usuario (Josué) | 1 | 1 | 2 | 1 | 1 | 1 | **1.17** | **1** | Documentación de pantallas y pasos funcionales agregados. |
| **EXTRA-2** | Actualizar casos de uso (Eduardo) | 1 | 1 | 1 | 2 | 1 | 1 | **1.17** | **1** | Ajustes visuales en Enterprise Architect / Lucidchart. |
| **EXTRA-3** | Actualizar doc. restante (Sebas) | 1 | 1 | 1 | 1 | 2 | 1 | **1.17** | **1** | Revisión de entregables teóricos de la fase. |
| **EXTRA-4** | Diagrama de arquitectura ([Tu nombre]) | 2 | 2 | 3 | 2 | 2 | 2 | **2.17** | **2** | Diagrama en capas (frontend, backend, DB, almacenamiento y servicios de correo). |

---

## Detalle de las historias de usuario

### Fase 1

#### HU-01: Registro de Estudiante

- **Descripción:** Yo como estudiante quiero registrarme en el sistema ingresando mis datos personales para poder acceder a las funcionalidades de la plataforma y agendar tutorías.
- **Criterios de Aceptación:**
  - El formulario debe solicitar: nombre, apellido, carnet universitario, género, dirección, teléfono, fecha de nacimiento, correo electrónico y contraseña.
  - El formulario permite subir una fotografía de perfil opcional.
  - La contraseña debe tener un mínimo de 8 caracteres, incluyendo al menos una letra minúscula, una mayúscula y un número.
  - La contraseña debe almacenarse encriptada en la base de datos.
  - El sistema debe validar que el correo electrónico no esté duplicado.
- **Prioridad:** 🟥 Alta
- **Dependencias:** Ninguna
- **Módulo:** Registro y Autenticación
- **Estimación:** 5 puntos (L)
- **Sprint:** 1

#### HU-02: Registro de Tutor

- **Descripción:** Yo como tutor quiero registrarme en el sistema ingresando mi información profesional y personal para publicar mi perfil y ofrecer sesiones de tutoría.
- **Criterios de Aceptación:**
  - El formulario debe solicitar: nombre, apellido, carnet o ID, fecha de nacimiento, género, dirección, teléfono, número de identificación de tutor, especialidad (materias que imparte), dirección de tutoría (física u online), correo electrónico, año de inicio de tutorías, universidad de graduación y contraseña.
  - El formulario solicita subir una fotografía de perfil obligatoria.
  - Se debe validar la unicidad del correo electrónico y del número de identificación del tutor.
  - La contraseña debe tener un mínimo de 8 caracteres, incluir minúscula, mayúscula y número, y almacenarse de forma encriptada.
- **Prioridad:** 🟥 Alta
- **Dependencias:** Ninguna
- **Módulo:** Registro y Autenticación
- **Estimación:** 5 puntos (L)
- **Sprint:** 1

#### HU-04: Inicio de Sesión de Administrador (2FA)

- **Descripción:** Yo como administrador quiero iniciar sesión mediante un proceso de doble autenticación para acceder de forma segura al panel de administración.
- **Criterios de Aceptación:**
  - El administrador debe ingresar primero con un usuario y contraseña predeterminados.
  - Tras el primer paso, debe ser redirigido a una página para subir un archivo llamado `auth2-ayd1.txt`.
  - El archivo debe contener una contraseña encriptada que, al ser validada por el sistema, permitirá el acceso a la página principal.
  - La contraseña del primer inicio de sesión y la del archivo deben ser diferentes.
- **Prioridad:** 🟥 Alta
- **Dependencias:** Ninguna
- **Módulo:** Registro y Autenticación
- **Estimación:** 5 puntos (L)
- **Sprint:** 1

#### HU-05: Aprobación de Registro de Estudiantes

- **Descripción:** Yo como administrador quiero ver la lista de estudiantes pendientes de aprobación para aceptar o rechazar sus solicitudes de ingreso a la plataforma.
- **Criterios de Aceptación:**
  - El sistema debe mostrar una lista con fotografía (o una por defecto si no tiene), nombre completo, carnet, género, fecha de nacimiento y correo del estudiante.
  - Debe existir un botón para aceptar o rechazar la solicitud a la par de cada registro.
  - Se debe enviar un correo de notificación al usuario informando si su cuenta fue aprobada o rechazada.
- **Prioridad:** 🟥 Alta
- **Dependencias:** HU-01
- **Módulo:** Administrador
- **Estimación:** 3 puntos (M)
- **Sprint:** 1

#### HU-06: Aprobación de Registro de Tutores

- **Descripción:** Yo como administrador quiero visualizar los tutores pendientes de aprobación para verificar sus perfiles y permitirles usar el sistema.
- **Criterios de Aceptación:**
  - La lista debe mostrar fotografía, nombre completo, carnet, género, especialidad, número de identificación y correo electrónico.
  - Debe incluir botones para aceptar o rechazar a cada tutor.
  - Se debe enviar un correo electrónico al tutor notificando si fue aprobado o rechazado.
- **Prioridad:** 🟥 Alta
- **Dependencias:** HU-02
- **Módulo:** Administrador
- **Estimación:** 3 puntos (M)
- **Sprint:** 1

#### HU-03: Inicio de Sesión (Estudiantes y Tutores)

- **Descripción:** Yo como usuario quiero iniciar sesión con mi correo y contraseña para acceder a mi panel correspondiente.
- **Criterios de Aceptación:**
  - El sistema debe verificar que el usuario haya sido aprobado previamente por el administrador; si no está aprobado, debe impedir el acceso.
  - Si hay un error de autenticación, el sistema debe mostrar un mensaje con la información del problema.
  - Debe existir un enlace visible para registrarse si el usuario no tiene cuenta.
- **Prioridad:** 🟥 Alta
- **Dependencias:** HU-01, HU-02, HU-05, HU-06
- **Módulo:** Registro y Autenticación
- **Estimación:** 3 puntos (M)
- **Sprint:** 1

#### HU-10: Establecer Horarios de Atención

- **Descripción:** Yo como tutor quiero configurar mis días y horas de disponibilidad para que los estudiantes puedan agendar sesiones conmigo.
- **Criterios de Aceptación:**
  - El sistema debe permitir seleccionar los días de la semana en los que se atenderá.
  - El sistema debe permitir establecer un rango de horario (ej. 8 am a 5 pm) que aplicará uniformemente para todos los días seleccionados.
- **Prioridad:** 🟥 Alta
- **Dependencias:** HU-03
- **Módulo:** Tutor
- **Estimación:** 5 puntos (L)
- **Sprint:** 1

#### HU-16: Exploración y Búsqueda de Tutores

- **Descripción:** Yo como estudiante quiero ver la lista de tutores disponibles y usar filtros avanzados para encontrar al tutor que mejor se adapte a mis necesidades.
- **Criterios de Aceptación:**
  - La página principal debe mostrar los tutores registrados (exceptuando aquellos con los que ya se tiene sesión), exhibiendo nombre completo, especialidad (materias), dirección de tutoría y foto.
  - Debe existir una búsqueda avanzada para filtrar por: materia, años de experiencia, sexo, edad y universidad.
  - Los filtros deben implementarse de manera intuitiva y eficiente, permitiendo al usuario una experiencia de usuario agradable.
- **Prioridad:** 🟥 Alta
- **Dependencias:** HU-02, HU-06
- **Módulo:** Estudiante
- **Estimación:** 5 puntos (L)
- **Sprint:** 1

#### HU-17: Consulta de Horarios y Disponibilidad

- **Descripción:** Yo como estudiante quiero ver los horarios de un tutor específico para saber en qué momentos está disponible para una sesión.
- **Criterios de Aceptación:**
  - Al seleccionar un tutor, se deben mostrar los días y horarios que atiende.
  - El sistema debe permitir filtrar por fecha y mostrar claramente los horarios ocupados y disponibles para ese día específico.
  - Debe indicar si el tutor no atiende en la fecha seleccionada.
- **Prioridad:** 🟥 Alta
- **Dependencias:** HU-10, HU-16
- **Módulo:** Estudiante
- **Estimación:** 5 puntos (L)
- **Sprint:** 1

#### HU-18: Programar Sesión de Tutoría

- **Descripción:** Yo como estudiante quiero agendar una sesión con un tutor llenando un formulario para reservar mi espacio.
- **Criterios de Aceptación:**
  - El formulario debe solicitar: fecha, hora, materia (impartida por el tutor) y motivo de la sesión.
  - El sistema debe validar que la fecha y hora seleccionadas estén dentro del horario del tutor y que el espacio esté disponible.
  - El sistema debe impedir agendar más de una sesión con el mismo tutor simultáneamente, y evitar traslapes de horario en sesiones del mismo día.
  - Si falla alguna validación, se debe notificar al usuario el motivo específico.
- **Prioridad:** 🟥 Alta
- **Dependencias:** HU-17
- **Módulo:** Estudiante
- **Estimación:** 8 puntos (XL)
- **Sprint:** 1

#### HU-12: Gestión y Atención de Sesiones Pendientes

- **Descripción:** Yo como tutor quiero ver mis sesiones pendientes y marcarlas como atendidas para llevar el control del progreso de mis estudiantes.
- **Criterios de Aceptación:**
  - La vista debe mostrar las sesiones ordenadas por fecha más próxima, indicando fecha, hora, nombre del estudiante, motivo y materia.
  - Debe existir un botón para marcar al estudiante como "Atendido".
  - Al marcar como atendido, debe desplegarse un formulario para ingresar el resumen o recomendaciones de la sesión antes de hacerla desaparecer de la lista de pendientes.
- **Prioridad:** 🟥 Alta
- **Dependencias:** HU-18
- **Módulo:** Tutor
- **Estimación:** 5 puntos (L)
- **Sprint:** 2

#### HU-13: Cancelación de Sesión por el Tutor

- **Descripción:** Yo como tutor quiero poder cancelar una sesión agendada para notificar al estudiante si me surge algún inconveniente.
- **Criterios de Aceptación:**
  - El tutor debe poder seleccionar una sesión pendiente y cancelarla, quitándola de su lista.
  - Al confirmar la cancelación, el horario asociado debe liberarse inmediatamente en la agenda del tutor para que vuelva a estar disponible.
  - El sistema debe enviar un correo automático al estudiante notificando la cancelación.
  - El correo debe incluir: fecha, hora, motivo de la sesión cancelada, nombre del tutor, materia y un mensaje de disculpa.
- **Prioridad:** 🟥 Alta
- **Dependencias:** HU-12
- **Módulo:** Tutor
- **Estimación:** 5 puntos (L)
- **Sprint:** 2

#### HU-19: Gestión de Sesiones Activas

- **Descripción:** Yo como estudiante quiero ver la lista de mis próximas sesiones y tener la opción de cancelarlas si no podré asistir.
- **Criterios de Aceptación:**
  - Debe existir una vista de "Sesiones Activas" mostrando fecha, hora, tutor, materia, dirección y motivo.
  - Debe haber una opción para cancelar la sesión, requiriendo un mensaje de confirmación antes de removerla de la lista de activas.
  - Al confirmar la cancelación, la sesión debe cambiar a estado "Cancelada por el estudiante" y liberar inmediatamente ese horario en la agenda del tutor para que esté disponible para otros alumnos.
- **Prioridad:** 🟨 Media
- **Dependencias:** HU-18
- **Módulo:** Estudiante
- **Estimación:** 5 puntos (L)
- **Sprint:** 2

#### HU-11: Actualizar Horarios de Atención

- **Descripción:** Yo como tutor quiero modificar mis horarios y días de atención para adaptar mi disponibilidad a mis necesidades actuales.
- **Criterios de Aceptación:**
  - El tutor puede cambiar los días y la hora de atención en el sistema.
  - El sistema debe validar que no existan sesiones activas fuera del nuevo rango de horario.
  - Si existen conflictos, no se debe permitir la actualización hasta que las sesiones afectadas sean reprogramadas o canceladas.
- **Prioridad:** 🟨 Media
- **Dependencias:** HU-10
- **Módulo:** Tutor
- **Estimación:** 5 puntos (L)
- **Sprint:** 2

#### HU-14: Historial de Sesiones

- **Descripción:** Yo como tutor quiero poder ver el historial de todas mis sesiones para tener un registro de mi actividad.
- **Criterios de Aceptación:**
  - El sistema debe mostrar un listado de todas las sesiones que he tenido, incluyendo fecha, hora, nombre del estudiante y estado de la sesión.
  - Debe permitir filtrar por fechas o por estudiante.
  - El historial debe ser accesible desde mi panel de usuario.
- **Prioridad:** 🟥 Alta
- **Dependencias:** HU-12, HU-13
- **Módulo:** Tutor
- **Estimación:** 3 puntos (M)
- **Sprint:** 2

#### HU-20: Visualización de Historial de Sesiones

- **Descripción:** Yo como estudiante quiero ver un historial de mis sesiones pasadas o canceladas para llevar un control de mi actividad en la plataforma.
- **Criterios de Aceptación:**
  - El estudiante debe ver sesiones atendidas y canceladas mostrando: fecha, tutor, materia, motivo, dirección, resumen de la sesión (si fue atendida) y estado.
- **Prioridad:** 🟦 Baja
- **Dependencias:** HU-12, HU-13, HU-19
- **Módulo:** Estudiante
- **Estimación:** 3 puntos (M)
- **Sprint:** 2

#### HU-15: Ver y actualizar perfil

- **Descripción:** Yo como tutor quiero poder visualizar los datos de mi perfil y actualizarlos si es necesario.
- **Criterios de Aceptación:**
  - El sistema debe permitir ver todos los datos registrados en mi perfil.
  - Debe permitir modificar cualquiera de los campos mostrados, a excepción del correo electrónico.
  - Debe permitir subir una nueva fotografía de perfil.
  - Si se solicita cambio de contraseña, el formulario debe exigir el ingreso de la contraseña original para validarla contra la base de datos antes de proceder.
  - La nueva contraseña debe cumplir con las validaciones de seguridad (mínimo 8 caracteres, mayúscula, minúscula y número) y guardarse encriptada en la base de datos.
- **Prioridad:** 🟥 Alta
- **Dependencias:** HU-02
- **Módulo:** Tutor
- **Estimación:** 5 puntos (L)
- **Sprint:** 2

#### HU-21: Gestión de Perfil

- **Descripción:** Yo como estudiante quiero ver y editar mis datos personales para mantener mi información actualizada.
- **Criterios de Aceptación:**
  - El estudiante debe poder visualizar todos los datos registrados en su perfil.
  - El estudiante debe poder modificar cualquiera de los campos mostrados, a excepción del correo electrónico.
  - Si se solicita cambio de contraseña, el sistema debe requerir obligatoriamente la contraseña original y validarla antes de permitir el cambio.
  - La nueva contraseña debe cumplir con los requisitos mínimos de seguridad (8 caracteres, mayúscula, minúscula y número) y almacenarse encriptada en la base de datos.
- **Prioridad:** 🟦 Baja
- **Dependencias:** HU-01
- **Módulo:** Estudiante
- **Estimación:** 5 puntos (L)
- **Sprint:** 2

#### HU-07: Gestión de Usuarios Activos

- **Descripción:** Yo como administrador quiero ver a todos los estudiantes y tutores activos en el sistema para poder darlos de baja si es necesario.
- **Criterios de Aceptación:**
  - Debe existir una vista para ver todos los estudiantes ya aceptados y una opción para darlos de baja.
  - Debe existir una vista separada para ver todos los tutores aceptados y una opción para darlos de baja.
  - Al dar de baja a un usuario, se le debe enviar automáticamente una notificación por correo electrónico informándole sobre la baja de su cuenta.
  - El usuario dado de baja no debe poder iniciar sesión y su estado debe pasar a inactivo.
- **Prioridad:** 🟨 Media
- **Dependencias:** HU-05, HU-06
- **Módulo:** Administrador
- **Estimación:** 5 puntos (L)
- **Sprint:** 2

#### HU-09: Visualización de usuarios dados de baja

- **Descripción:** Yo como administrador quiero ver la lista de usuarios que han sido dados de baja del sistema.
- **Criterios de Aceptación:**
  - El sistema debe permitir ver la lista de estudiantes y tutores que han sido dados de baja, mostrando su nombre, correo electrónico, fecha de baja y motivo.
- **Prioridad:** 🟨 Media
- **Dependencias:** HU-05, HU-06, HU-07
- **Módulo:** Administrador
- **Estimación:** 3 puntos (M)
- **Sprint:** 2

#### HU-08: Generación de Reportes

- **Descripción:** Yo como administrador quiero generar reportes del sistema para tomar decisiones estratégicas basadas en el uso de la plataforma.
- **Criterios de Aceptación:**
  - El sistema debe permitir generar al menos dos reportes relevantes (ej. tutores que más estudiantes han atendido o materia con más demanda).
  - Los reportes deben presentarse de forma gráfica incluyendo tablas y elementos visuales (como gráficos de barras o circulares).
  - Los datos consumidos para los reportes y gráficos deben ser precisos y actualizados al momento de la consulta.
- **Prioridad:** 🟨 Media
- **Dependencias:** HU-12, HU-18
- **Módulo:** Administrador
- **Estimación:** 8 puntos (XL)
- **Sprint:** 2

### Fase 2

#### HU-22: [EXT - HU-01] Fotografía y carnet del estudiante

- **Descripción:** Como estudiante quiero registrarme subiendo una fotografía personal reciente y un archivo tipo PDF con mi carnet escaneado para que el administrador pueda validar mi identidad.
- **Criterios de Aceptación:**
  - El sistema debe solicitar la fotografía del estudiante y debe impedir su registro si esta no se sube.
  - El apartado para subir el carnet debe permitir subir únicamente documentos de tipo PDF, y debe rechazar archivos de cualquier otro tipo o extensión.
  - El sistema permite modificar la fotografía o el documento PDF en caso de haber seleccionado el archivo incorrecto.
  - El archivo PDF con el carnet escaneado es obligatorio y el sistema debe impedir el registro del estudiante si el archivo no ha sido cargado.
- **Prioridad:** 🟥 Alta
- **Dependencias:** Ninguna
- **Módulo:** Registro y autenticación
- **Estimación:** 3 puntos (M)
- **Sprint:** 3
- **Extiende a:** HU-01 Registro de Estudiante

#### HU-23: [EXT - HU-02] CV del tutor

- **Descripción:** Como tutor quiero subir mi CV actualizado al registrarme en el sistema para que el administrador valide y apruebe mi perfil con un documento de respaldo.
- **Criterios de Aceptación:**
  - El sistema debe solicitar el CV del tutor obligatoriamente y debe impedir su registro si esta no se sube.
  - El apartado para subir el CV debe permitir únicamente documentos de tipo PDF, y debe rechazar archivos de cualquier otra extensión.
  - El sistema permite modificar el documento PDF en caso de que el tutor seleccione el archivo incorrecto.
- **Prioridad:** 🟥 Alta
- **Dependencias:** Ninguna
- **Módulo:** Registro y autenticación
- **Estimación:** 3 puntos (M)
- **Sprint:** 3
- **Extiende a:** HU-02 Registro de Tutor

#### HU-24: [EXT - HU-03] Validación de correo electrónico con token

- **Descripción:** Como usuario de EduConnect, quiero que el sistema valide la autenticidad de mi correo electrónico a través de un token de registro.
- **Criterios de Aceptación:**
  - El sistema envía un token de verificación de 6 dígitos automáticamente luego de que un usuario finalice el registro en la plataforma.
  - El usuario debe ingresar el token de verificación en el primer inicio de sesión que realice dentro de la plataforma.
  - El sistema debe impedir el primer inicio de sesión si el token ingresado es incorrecto.
  - El sistema debe tener la opción de volver a enviar un token de autenticación en caso de que el primero se haya perdido o no se haya recibido.
  - Luego del primer inicio de sesión exitoso, el sistema debe marcar el correo del usuario como válido y ya no debe solicitar el token en futuros inicios de sesión.
  - El correo debe tener el logo de la plataforma, un mensaje de bienvenida y el token de verificación.
- **Prioridad:** 🟥 Alta
- **Dependencias:** HU-22, HU-23
- **Módulo:** Registro y autenticación
- **Estimación:** 3 puntos (M)
- **Sprint:** 3
- **Extiende a:** HU-03 Inicio de Sesión (Estudiantes y Tutores)

#### HU-32: [EXT - HU-05] Aprobación de estudiantes con visor PDF incrustado

- **Descripción:** Como administrador quiero revisar las solicitudes de registro de estudiantes y visualizar directamente su carnet escaneado en un visor PDF incrustado para validar su identidad y aprobar o rechazar su acceso a la plataforma sin necesidad de descargar archivos.
- **Criterios de Aceptación:**
  - El sistema despliega la lista de estudiantes pendientes de aprobación con los datos: fotografía reciente (o foto por defecto si no posee), nombre completo, carnet, género, fecha de nacimiento y correo electrónico.
  - La interfaz integra un visor de PDF embebido que permite previsualizar el documento del carnet directamente en el navegador sin forzar la descarga.
  - Cada registro dispone de botones de acción claramente diferenciados: "Aceptar" y "Rechazar".
  - Al hacer clic en "Aceptar", el usuario pasa a estado activo, permitiéndole autenticarse con sus credenciales y token; si se rechaza, el acceso queda revocado.
  - El sistema envía un correo automático de notificación al estudiante informándole sobre la aprobación o rechazo de su cuenta.
- **Prioridad:** 🟥 Alta
- **Dependencias:** HU-22, HU-24
- **Módulo:** Administrador
- **Estimación:** 5 puntos (L)
- **Sprint:** 3
- **Extiende a:** HU-05 Aprobación de Registro de Estudiantes

#### HU-33: [EXT - HU-06] Aprobación de tutores con visor PDF incrustado

- **Descripción:** Como administrador quiero validar el perfil profesional de los tutores aspirantes y consultar su currículum vítae mediante un visor PDF integrado para admitir o denegar su incorporación al cuerpo docente.
- **Criterios de Aceptación:**
  - El sistema despliega la lista de tutores pendientes mostrando: fotografía obligatoria, nombre completo, carnet o ID, género, especialidad (materias), número de identificación y correo electrónico.
  - El documento PDF del CV debe visualizarse directamente dentro de la interfaz a través de un componente embebido sin descargas locales.
  - Cuenta con botones de acción para "Aceptar" o "Rechazar" la solicitud del tutor.
  - Al aprobar al tutor, se habilita su cuenta en el sistema para que pueda completar su autenticación; si es rechazado, se le deniega el ingreso.
  - Se genera y envía automáticamente un correo electrónico al tutor notificándole el resultado de su solicitud (aprobada o rechazada).
- **Prioridad:** 🟥 Alta
- **Dependencias:** HU-23, HU-24
- **Módulo:** Administrador
- **Estimación:** 5 puntos (L)
- **Sprint:** 3
- **Extiende a:** HU-06 Aprobación de Registro de Tutores

#### HU-29: [EXT - HU-12] Atender estudiante

- **Descripción:** Como tutor quiero poder marcar una sesión como atendida después de impartir una tutoría a un estudiante.
- **Criterios de Aceptación:**
  - El sistema cuenta con un botón que despliega un formulario donde deba ingresar el plan de estudio del estudiante.
  - El formulario de plan de estudio debe solicitar las dificultades identificadas de forma obligatoria.
  - El formulario de plan de estudio debe solicitar uno o más recursos recomendados, permitiendo ingresar el nombre, tipo y descripción de uso de cada uno de forma dinámica.
  - Después de ingresar el plan de estudio, el sistema debe marcar la sesión como atendida y la sesión desaparece de la lista de sesiones pendientes del tutor.
- **Prioridad:** 🟥 Alta
- **Dependencias:** HU-33
- **Módulo:** Tutor
- **Estimación:** 3 puntos (M)
- **Sprint:** 3
- **Extiende a:** HU-12 Gestión y Atención de Sesiones Pendientes

#### HU-25: Plan de estudio de estudiante

- **Descripción:** Como estudiante de EduConnect quiero ver mi plan de estudio para tener una guía de qué temas debo reforzar y conocer en qué áreas he presentado dificultades.
- **Criterios de Aceptación:**
  - El plan de estudio debe mostrar la fecha de la última sesión del estudiante con el nombre del tutor, identificación y su especialidad.
  - El plan de estudio debe mostrar las dificultades identificadas y recursos recomendados, con su nombre, tipo y descripción de uso.
- **Prioridad:** 🟥 Alta
- **Dependencias:** HU-29
- **Módulo:** Estudiante
- **Estimación:** 3 puntos (M)
- **Sprint:** 3

#### HU-26: [EXT - HU-25] Imprimir plan de estudio del estudiante

- **Descripción:** Como estudiante quiero generar un archivo PDF para guardar evidencia de las tutorías.
- **Criterios de Aceptación:**
  - El PDF generado debe tener un encabezado con el nombre de la plataforma, la fecha de emisión, y el teléfono de contacto.
  - El PDF generado debe tener un cuerpo o apartado principal con la tabla de los recursos recomendados, conteniendo nombre, tipo y descripción de uso.
  - El pie de documento del PDF generado debe incluir nombre del tutor, especialidad, y número de identificación.
- **Prioridad:** 🟨 Media
- **Dependencias:** HU-25
- **Módulo:** Estudiante
- **Estimación:** 2 puntos (S)
- **Sprint:** 3
- **Extiende a:** HU-25 Plan de estudio de estudiante

#### HU-35: [EXT - HU-07] Ver, actualizar y dar de baja tutores

- **Descripción:** Como administrador quiero administrar el listado de tutores registrados para editar su información académica o dar de baja sus cuentas ante faltas operativas.
- **Criterios de Aceptación:**
  - El sistema lista todos los tutores aceptados junto con su información personal y profesional detallada.
  - Permite la edición de todos los campos del tutor, excepto el correo electrónico, el cual permanece de solo lectura.
  - Incluye la opción "Dar de baja", exigiendo un cuadro de confirmación preventiva antes de proceder.
  - Un tutor dado de baja queda inhabilitado en la base de datos y no puede iniciar sesión ni recibir nuevas reservas de tutorías.
- **Prioridad:** 🟨 Media
- **Dependencias:** HU-33
- **Módulo:** Administrador
- **Estimación:** 3 puntos (M)
- **Sprint:** 3
- **Extiende a:** HU-07 Gestión de Usuarios Activos

#### HU-27: Calificar atención de tutoría

- **Descripción:** Como estudiante quiero calificar la atención recibida en una tutoría pasada.
- **Criterios de Aceptación:**
  - El sistema debe permitir seleccionar una calificación obligatoria de 0 a 5 estrellas.
  - El sistema permite agregar un comentario opcional con el motivo de la calificación.
  - Se debe validar que solo las sesiones con estado "Atendido" puedan ser calificadas.
  - El sistema no permite calificar sesiones canceladas o pendientes.
- **Prioridad:** 🟨 Media
- **Dependencias:** HU-29
- **Módulo:** Estudiante
- **Estimación:** 2 puntos (S)
- **Sprint:** 4

#### HU-28: Reportar tutor

- **Descripción:** Como estudiante quiero poder reportar un tutor luego de haber sido atendido.
- **Criterios de Aceptación:**
  - El formulario debe incluir una lista predefinida de categorías de reporte, como Ética, Negligencia académica, etc.
  - El formulario debe solicitar la explicación del motivo del reporte.
  - El reporte debe enviarse automáticamente al administrador para que pueda tomar acciones.
  - El sistema solo permite reportar sesiones con estado "Atendido".
  - El sistema no permite reportar sesiones canceladas o pendientes.
- **Prioridad:** 🟨 Media
- **Dependencias:** HU-29
- **Módulo:** Estudiante
- **Estimación:** 2 puntos (S)
- **Sprint:** 4

#### HU-30: Calificar estudiante

- **Descripción:** Como tutor quiero poder calificar a los estudiantes a los que he atendido.
- **Criterios de Aceptación:**
  - El formulario de calificación solicita una calificación obligatoria de 0 a 5 estrellas.
  - El formulario solicita un comentario con el motivo de la calificación.
  - El sistema valida que la sesión a calificar tenga estado "Atendido".
  - El sistema impide que se califiquen sesiones canceladas o pendientes.
- **Prioridad:** 🟨 Media
- **Dependencias:** HU-29
- **Módulo:** Tutor
- **Estimación:** 2 puntos (S)
- **Sprint:** 4

#### HU-31: Reportar estudiante

- **Descripción:** Como tutor quiero poder reportar a los estudiantes problemáticos a los que he atendido.
- **Criterios de Aceptación:**
  - El formulario despliega una lista predefinida de categorías del reporte.
  - El formulario solicita una descripción detallada del reporte.
  - Al enviar el formulario, el reporte debe ser enviado automáticamente al administrador para que pueda tomar acciones en el caso.
  - El sistema valida que la sesión a reportar tenga estado "Atendido".
  - El sistema impide que se reporten sesiones canceladas o pendientes.
- **Prioridad:** 🟨 Media
- **Dependencias:** HU-29
- **Módulo:** Tutor
- **Estimación:** 2 puntos (S)
- **Sprint:** 4

#### HU-34: [EXT - HU-07] Ver, actualizar y dar de baja estudiantes

- **Descripción:** Como administrador quiero gestionar el padrón de estudiantes admitidos para mantener actualizados sus registros o deshabilitar cuentas cuando sea necesario.
- **Criterios de Aceptación:**
  - El sistema despliega la lista completa de estudiantes con estado aprobado/activo.
  - Permite editar cualquier campo del perfil del estudiante (datos personales, carnet, teléfono, dirección, etc.), manteniendo el campo de correo electrónico bloqueado contra edición.
  - Dispone de la acción "Dar de baja", la cual solicita una confirmación explícita previa antes de ejecutar la desactivación.
  - El estudiante dado de baja pasa a estado inactivo y el sistema bloquea cualquier intento posterior de inicio de sesión.
- **Prioridad:** 🟨 Media
- **Dependencias:** HU-32
- **Módulo:** Administrador
- **Estimación:** 5 puntos (L)
- **Sprint:** 4
- **Extiende a:** HU-07 Gestión de Usuarios Activos

#### HU-36: Gestión de reportes hacia tutores

- **Descripción:** Como administrador quiero revisar los reportes interpuestos por los estudiantes en contra de tutores para evaluar incidentes y aplicar sanciones o desestimar el caso.
- **Criterios de Aceptación:**
  - La vista lista todos los reportes recibidos con los campos: categoría del reporte, motivo/explicación detallada, nombre completo del tutor denunciado, nombre completo del estudiante denunciante y fecha de emisión.
  - Cada reporte presenta dos opciones de resolución: "Dar de baja al tutor" y "Rechazar denuncia".
  - Si se selecciona dar de baja al tutor, su cuenta queda inmediatamente desactivada y suspendida en el sistema.
  - Si se rechaza la denuncia, el reporte cambia a estado desestimado sin alterar el estado del tutor.
- **Prioridad:** 🟨 Media
- **Dependencias:** HU-28
- **Módulo:** Administrador
- **Estimación:** 3 puntos (M)
- **Sprint:** 4

#### HU-37: Gestión de reportes hacia estudiantes

- **Descripción:** Como administrador quiero auditar las quejas presentadas por los tutores respecto a conductas de estudiantes para resolver incidencias disciplinarias.
- **Criterios de Aceptación:**
  - El sistema muestra los reportes emitidos por los tutores indicando: categoría del reporte, explicación detallada del motivo, nombre del estudiante involucrado, nombre del tutor denunciante y fecha del incidente.
  - El administrador cuenta con las acciones de "Dar de baja al estudiante" o "Rechazar denuncia".
  - Aplicar la baja desactiva la cuenta del estudiante de inmediato impidiéndole iniciar sesión en la plataforma.
  - Rechazar la denuncia archiva el caso manteniendo al estudiante en estado activo.
- **Prioridad:** 🟨 Media
- **Dependencias:** HU-31
- **Módulo:** Administrador
- **Estimación:** 3 puntos (M)
- **Sprint:** 4

#### HU-38: Reporte consolidado de calificación de tutores

- **Descripción:** Como administrador quiero consultar un reporte con el desempeño evaluativo de los tutores para monitorear la calidad del servicio docente brindado.
- **Criterios de Aceptación:**
  - El reporte incluye la totalidad de tutores activos en la plataforma.
  - Por cada tutor se presenta: nombre completo, especialidad y promedio general de calificación (en escala de 0 a 5 estrellas).
  - El promedio se calcula automáticamente a partir de todas las valoraciones reales emitidas por los estudiantes en sesiones atendidas.
  - Permite visualizar de forma clara los tutores con mejor y menor rendimiento.
- **Prioridad:** 🟦 Baja
- **Dependencias:** HU-27
- **Módulo:** Administrador
- **Estimación:** 3 puntos (M)
- **Sprint:** 4

#### HU-39: Reporte consolidado de calificación de estudiantes

- **Descripción:** Como administrador quiero ver las evaluaciones asignadas a los estudiantes para identificar patrones de conducta o estudiantes con desempeño sobresaliente.
- **Criterios de Aceptación:**
  - El reporte muestra la lista completa de estudiantes activos en el sistema.
  - Cada fila detalla el nombre completo del estudiante y su promedio de calificación (rango de 0 a 5 estrellas).
  - El cálculo se deriva de las calificaciones asignadas por los tutores al finalizar las sesiones con estado atendido.
- **Prioridad:** 🟦 Baja
- **Dependencias:** HU-30
- **Módulo:** Administrador
- **Estimación:** 5 puntos (L)
- **Sprint:** 4

#### HU-40: [EXT - HU-08] Reportes analíticos avanzados para toma de decisiones

- **Descripción:** Como administrador quiero generar y visualizar reportes analíticos con métricas operativas de la plataforma para respaldar la toma de decisiones estratégicas.
- **Criterios de Aceptación:**
  - El sistema incluye al menos dos reportes analíticos adicionales y diferentes a los de la fase previa (ej. Top 10 tutores con más sesiones atendidas, materias con mayor tasa de cancelación, horarios más demandados o evolución de sesiones programadas).
  - La presentación de los datos utiliza componentes visuales (gráficos de barras/líneas, charts interactivos o tablas estructuradas).
  - Los reportes reflejan datos consolidados, precisos y calculados en tiempo real según la actividad de la plataforma.
- **Prioridad:** 🟦 Baja
- **Dependencias:** HU-29, HU-32, HU-33
- **Módulo:** Administrador
- **Estimación:** 8 puntos (XL)
- **Sprint:** 4
- **Extiende a:** HU-08 Generación de Reportes