# Sprint Backlog

> El Sprint Backlog contiene las historias de usuario seleccionadas para cada sprint, junto con el objetivo del sprint (Sprint Goal) y el trabajo planificado para alcanzarlo.

Las tareas derivadas de cada historia y el responsable de cada tarea se encuentran en las tarjetas del tablero Kanban: [https://edu-connect-g7.atlassian.net/jira/software/projects/ED/list?jql=project%20%3D%20ED%20ORDER%20BY%20cf%5B10019%5D%20ASC](https://edu-connect-g7.atlassian.net/jira/software/projects/ED/list?jql=project%20%3D%20ED%20ORDER%20BY%20cf%5B10019%5D%20ASC).
La estimación, las dependencias y los criterios de aceptación de cada historia se encuentran en el Product Backlog (`product-backlog.md`).

## Resumen de sprints

| Sprint | Fase | Nombre | Historias | Puntos |
| :---: | :---: | :--- | :---: | :---: |
| 1 | 1 | Fundaciones, Autenticación y Flujo Base de Agendamiento | 10 | 47 |
| 2 | 1 | Gestión de Sesiones, Perfiles, Bajas y Reportería | 11 | 52 |
| 3 | 2 | Verificación de Identidad, Plan de Estudio y Aprobación con Visor PDF | 9 + 4 tareas | 35 |
| 4 | 2 | Calificaciones, Reportes entre Usuarios y Analítica | 10 | 35 |

---

## Sprint 1: Fundaciones, Autenticación y Flujo Base de Agendamiento  (Fase 1)

**Sprint Goal:** Dejar operativo el flujo base de extremo a extremo: usuarios registrados y aprobados por el administrador, con sesión iniciada, y un estudiante que encuentra un tutor, consulta su disponibilidad y agenda una sesión.

**Trabajo planificado:**

- Registro de estudiantes y tutores, e inicio de sesión del administrador con doble autenticación (2FA).
- Aprobación de registros por el administrador e inicio de sesión de estudiantes y tutores.
- Configuración de los horarios de atención del tutor.
- Exploración y búsqueda de tutores, consulta de horarios y programación de sesiones.

**Historias seleccionadas:**

| Código | Módulo | Título de la Historia de Usuario | Prioridad | Dependencias | Puntos | Talla |
| :--- | :--- | :--- | :--- | :--- | :---: | :---: |
| **HU-01** | Registro y Autenticación | Registro de Estudiante | 🟥 Alta | Ninguna | 5 | L |
| **HU-02** | Registro y Autenticación | Registro de Tutor | 🟥 Alta | Ninguna | 5 | L |
| **HU-04** | Registro y Autenticación | Inicio de Sesión de Administrador (2FA) | 🟥 Alta | Ninguna | 5 | L |
| **HU-05** | Administrador | Aprobación de Registro de Estudiantes | 🟥 Alta | HU-01 | 3 | M |
| **HU-06** | Administrador | Aprobación de Registro de Tutores | 🟥 Alta | HU-02 | 3 | M |
| **HU-03** | Registro y Autenticación | Inicio de Sesión (Estudiantes y Tutores) | 🟥 Alta | HU-01, HU-02, HU-05, HU-06 | 3 | M |
| **HU-10** | Tutor | Establecer Horarios de Atención | 🟥 Alta | HU-03 | 5 | L |
| **HU-16** | Estudiante | Exploración y Búsqueda de Tutores | 🟥 Alta | HU-02, HU-06 | 5 | L |
| **HU-17** | Estudiante | Consulta de Horarios y Disponibilidad | 🟥 Alta | HU-10, HU-16 | 5 | L |
| **HU-18** | Estudiante | Programar Sesión de Tutoría | 🟥 Alta | HU-17 | 8 | XL |
| **Total** | | | | | **47** | |



---

## Sprint 2: Gestión de Sesiones, Perfiles, Bajas y Reportería  (Fase 1)

**Sprint Goal:** Completar el ciclo de vida de las sesiones (atención, cancelación e historial), la gestión de perfiles y usuarios activos, y entregar los primeros reportes del administrador.

**Trabajo planificado:**

- Atención y cancelación de sesiones por el tutor, y cancelación de sesiones activas por el estudiante.
- Actualización de horarios de atención e historiales de sesiones de estudiante y tutor.
- Consulta y actualización de perfil de estudiantes y tutores.
- Gestión de usuarios activos, visualización de usuarios dados de baja y generación de reportes.

**Historias seleccionadas:**

| Código | Módulo | Título de la Historia de Usuario | Prioridad | Dependencias | Puntos | Talla |
| :--- | :--- | :--- | :--- | :--- | :---: | :---: |
| **HU-12** | Tutor | Gestión y Atención de Sesiones Pendientes | 🟥 Alta | HU-18 | 5 | L |
| **HU-13** | Tutor | Cancelación de Sesión por el Tutor | 🟥 Alta | HU-12 | 5 | L |
| **HU-19** | Estudiante | Gestión de Sesiones Activas | 🟨 Media | HU-18 | 5 | L |
| **HU-11** | Tutor | Actualizar Horarios de Atención | 🟨 Media | HU-10 | 5 | L |
| **HU-14** | Tutor | Historial de Sesiones | 🟥 Alta | HU-12, HU-13 | 3 | M |
| **HU-20** | Estudiante | Visualización de Historial de Sesiones | 🟦 Baja | HU-12, HU-13, HU-19 | 3 | M |
| **HU-15** | Tutor | Ver y actualizar perfil | 🟥 Alta | HU-02 | 5 | L |
| **HU-21** | Estudiante | Gestión de Perfil | 🟦 Baja | HU-01 | 5 | L |
| **HU-07** | Administrador | Gestión de Usuarios Activos | 🟨 Media | HU-05, HU-06 | 5 | L |
| **HU-09** | Administrador | Visualización de usuarios dados de baja | 🟨 Media | HU-05, HU-06, HU-07 | 3 | M |
| **HU-08** | Administrador | Generación de Reportes | 🟨 Media | HU-12, HU-18 | 8 | XL |
| **Total** | | | | | **52** | |



---

## Sprint 3: Verificación de Identidad, Plan de Estudio y Aprobación con Visor PDF  (Fase 2)

**Sprint Goal:** Reforzar el ingreso a la plataforma con documentos y correo verificado, habilitar la aprobación con visor PDF, el registro y la consulta del plan de estudio, y actualizar la documentación del proyecto.

**Trabajo planificado:**

- Carga obligatoria de fotografía, carnet y CV en PDF, y verificación del correo con token.
- Aprobación de estudiantes y tutores con visor PDF embebido.
- Atención de estudiantes con registro del plan de estudio, consulta del plan e impresión en PDF.
- Gestión de tutores por el administrador (edición y baja).
- Documentación: manual de usuario, casos de uso, documentación restante y diagrama de arquitectura.

**Historias seleccionadas:**

| Código | Módulo | Título de la Historia de Usuario | Prioridad | Dependencias | Puntos | Talla |
| :--- | :--- | :--- | :--- | :--- | :---: | :---: |
| **HU-22** | Registro y autenticación | [EXT - HU-01] Fotografía y carnet del estudiante | 🟥 Alta | Ninguna | 3 | M |
| **HU-23** | Registro y autenticación | [EXT - HU-02] CV del tutor | 🟥 Alta | Ninguna | 3 | M |
| **HU-24** | Registro y autenticación | [EXT - HU-03] Validación de correo electrónico con token | 🟥 Alta | HU-22, HU-23 | 3 | M |
| **HU-32** | Administrador | [EXT - HU-05] Aprobación de estudiantes con visor PDF incrustado | 🟥 Alta | HU-22, HU-24 | 5 | L |
| **HU-33** | Administrador | [EXT - HU-06] Aprobación de tutores con visor PDF incrustado | 🟥 Alta | HU-23, HU-24 | 5 | L |
| **HU-29** | Tutor | [EXT - HU-12] Atender estudiante | 🟥 Alta | HU-33 | 3 | M |
| **HU-25** | Estudiante | Plan de estudio de estudiante | 🟥 Alta | HU-29 | 3 | M |
| **HU-26** | Estudiante | [EXT - HU-25] Imprimir plan de estudio del estudiante | 🟨 Media | HU-25 | 2 | S |
| **HU-35** | Administrador | [EXT - HU-07] Ver, actualizar y dar de baja tutores | 🟨 Media | HU-33 | 3 | M |
| **EXTRA-1** | Documentación | Manual de usuario | N/A | N/A | 1 | XS |
| **EXTRA-2** | Documentación | Actualizar diagramas de casos de uso | N/A | N/A | 1 | XS |
| **EXTRA-3** | Documentación | Actualizar documentación restante | N/A | N/A | 1 | XS |
| **EXTRA-4** | Documentación | Diagrama de arquitectura | N/A | N/A | 2 | S |
| **Total** | | | | | **35** | |

**Distribución del trabajo:**

| Integrante | Historias / Tareas | Puntos |
| :--- | :--- | :---: |
| Keitlyn | HU-22, HU-23 | 6 |
| Josué | HU-25, HU-26, EXTRA-1 (Manual de usuario) | 6 |
| Christian | HU-32, HU-33 | 10 |
| Carlos | HU-35, EXTRA-2 (Actualizar diagramas de casos de uso) | 4 |
| Sebas | HU-29, EXTRA-3 (Actualizar documentación restante) | 4 |
| Eduardo | HU-24, EXTRA-4 (Diagrama de arquitectura) | 5 |



---

## Sprint 4: Calificaciones, Reportes entre Usuarios y Analítica  (Fase 2)

**Sprint Goal:** Cerrar el ciclo de calidad del servicio: calificaciones y reportes entre estudiantes y tutores, su resolución por el administrador, la gestión de estudiantes y los reportes analíticos.

**Trabajo planificado:**

- Calificación de tutorías por el estudiante y de estudiantes por el tutor.
- Reporte de tutores por estudiantes y de estudiantes por tutores.
- Gestión de reportes y de estudiantes (edición y baja) por el administrador.
- Reportes consolidados de calificación y reportes analíticos avanzados.

**Historias seleccionadas:**

| Código | Módulo | Título de la Historia de Usuario | Prioridad | Dependencias | Puntos | Talla |
| :--- | :--- | :--- | :--- | :--- | :---: | :---: |
| **HU-27** | Estudiante | Calificar atención de tutoría | 🟨 Media | HU-29 | 2 | S |
| **HU-28** | Estudiante | Reportar tutor | 🟨 Media | HU-29 | 2 | S |
| **HU-30** | Tutor | Calificar estudiante | 🟨 Media | HU-29 | 2 | S |
| **HU-31** | Tutor | Reportar estudiante | 🟨 Media | HU-29 | 2 | S |
| **HU-34** | Administrador | [EXT - HU-07] Ver, actualizar y dar de baja estudiantes | 🟨 Media | HU-32 | 5 | L |
| **HU-36** | Administrador | Gestión de reportes hacia tutores | 🟨 Media | HU-28 | 3 | M |
| **HU-37** | Administrador | Gestión de reportes hacia estudiantes | 🟨 Media | HU-31 | 3 | M |
| **HU-38** | Administrador | Reporte consolidado de calificación de tutores | 🟦 Baja | HU-27 | 3 | M |
| **HU-39** | Administrador | Reporte consolidado de calificación de estudiantes | 🟦 Baja | HU-30 | 5 | L |
| **HU-40** | Administrador | [EXT - HU-08] Reportes analíticos avanzados para toma de decisiones | 🟦 Baja | HU-29, HU-32, HU-33 | 8 | XL |
| **Total** | | | | | **35** | |

**Distribución del trabajo:**

| Integrante | Historias / Tareas | Puntos |
| :--- | :--- | :---: |
| Sebas | HU-27 + extra documentacion | 10 |
| Keitlyn | HU-28, HU-38 | 5 |
| Carlos | HU-34, HU-39 | 10 |
| Josué | HU-30, HU-31 | 4 |
| Christian | HU-36, HU-37 | 6 |
| Eduardo | HU-40 | 8 |

