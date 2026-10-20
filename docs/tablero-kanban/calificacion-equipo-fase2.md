# Evaluación del Equipo Scrum — Fase 2 (Sprints 1 y 2)

### Documentos relacionados

- [README Principal del Proyecto](../../README.md)
- [Portal Maestro de Documentación](../README.md)
- [Tablero Kanban y Evidencias Ágiles (Fase 1 y Fase 2)](tablero.md)
- [Daily Scrums del Sprint 1 (Fase 2)](Fase%202%20-%20Dailies%20sprint%201.md)
- [Daily Scrums del Sprint 2 (Fase 2)](Fase%202%20-%20Dailies%20sprint%202.md)
- [Grabación Sprint Planning 1 (Drive)](https://drive.google.com/file/d/1HbWsrEQJ48EdGgmKzNFBxe9j-RGiSIfL/view?usp=drivesdk)
- [Grabación Sprint Retrospective 1 (Drive)](https://drive.google.com/file/d/16kqTk8sTwLZ7XVo4j3HdBpANuAM7rgf1/view?usp=drivesdk)
- [Grabación Sprint Planning 2 (Drive)](https://drive.google.com/file/d/1vwCyVQ9oFVsBaT03VRKmn_90KmrDpwCf/view?usp=drivesdk)
- [Grabación Sprint Retrospective 2 (Drive)](https://drive.google.com/file/d/1eyTxsxvzjNZxjRDQUI6gPWD27x0rPAJd/view?usp=drive_link)
- [Evaluación del Equipo — Sprint 1 (Fase 1)](../../scrum/sprint-1/calificacion-equipo.md)
- [Evaluación del Equipo — Sprint 2 (Fase 1)](../../scrum/sprint-2/calificacion-equipo.md)

---

La presente evaluación formal ha sido elaborada de manera objetiva por el **Scrum Master (Carlos Eduardo Lau López)** basándose en el historial verificado de contribuciones del repositorio Git en las ramas dedicadas (`feature/<historia>_<carnet>`), el cumplimiento riguroso de la Definición de Terminado (DoD), la puntualidad en las 12 reuniones Daily Scrum y ceremonias de Sprint Planning y Sprint Retrospective, así como la cobertura de pruebas unitarias y pruebas End-to-End (E2E) con Playwright. Los datos analizados contemplan el periodo de la **Fase 2 (del 28 de septiembre al 19 de octubre de 2026)**.

---

## 1. Criterios de Evaluación y Ponderación del Scrum Master

| Criterio de Evaluación | Ponderación | Descripción |
| :--- | :---: | :--- |
| **Cumplimiento de Historias y DoD** | **30%** | Entrega a tiempo de las historias asignadas cumpliendo criterios de aceptación, sin regresiones ni deuda técnica. |
| **Calidad de Código y Pruebas Técnicas** | **25%** | Arquitectura limpia (Vertical Slice en .NET 10 y Feature-Driven en Vue 3), suite de pruebas unitarias y pruebas E2E. |
| **Asistencia y Ceremonias Scrum** | **20%** | Participación puntual y activa en las 12 Daily Scrums, Sprint Plannings y Retrospectivas, reportando bloqueos oportunamente. |
| **Colaboración Cruzada y Soporte** | **15%** | Apoyo mutuo en resolución de incidencias en staging/Docker, code reviews constructivos y sincronización de contratos API. |
| **Trazabilidad en Jira y Documentación** | **10%** | Mantenimiento riguroso de las tarjetas Kanban, vinculación de commits con identificadores de tareas y documentación clara. |

---

## 2. Tabla Cuantitativa de Contribución y Calificación — Fase 2

| Integrante | Carnet | Rol Principal | Commits F2 | Líneas (+) | Líneas (-) | Fixes / Tests | Rango Fechas F2 | Puntuación (1-100) |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: |
| **Eduardo Sebastian Gutierrez Felipe** | 202300694 | Product Owner / Dev | 18 | 9,883 | 643 | 4 fixes / 5 tests | 30/09 - 19/10 | **100 / 100** |
| **Carlos Eduardo Lau López** | 202202812 | Scrum Master / Dev | 12 | 5,027 | 193 | 3 fixes / 2 tests | 30/09 - 07/10 | **98 / 100** |
| **Christian David Chinchilla Santos** | 202308227 | Desarrollador | 13 | 3,348 | 233 | 0 fixes / 2 tests | 02/10 - 07/10 | **95 / 100** |
| **Josue Daniel Revolorio Martínez** | 202102984 | Desarrollador | 9 | 1,965 | 84 | 1 fix / 1 test | 30/09 - 14/10 | **95 / 100** |
| **Keitlyn Valentina Tunchez Castañeda** | 202201139 | Desarrolladora | 12 | 3,364 | 52 | 1 fix / 2 tests | 03/10 - 08/10 | **94 / 100** |
| **Sebastian Antonio Romero Tzitzimit** | 202201690 | Desarrollador | 12 | 7,054 | 2,203 | 0 fixes / 4 tests | 03/10 - 19/10 | **92 / 100** |

---

## 3. Justificación Individual del Desempeño por el Scrum Master

### 1. Eduardo Sebastián Gutiérrez Felipe (Carnet: 202300694)
* **Rol:** Product Owner / Desarrollador
* **Evaluación:** Excelente (100 / 100).
* **Justificación:** Eduardo mantuvo un nivel sobresaliente de productividad e iniciativa técnica durante toda la fase. Diseñó e implementó la verificación de correo electrónico mediante token OTP de 6 dígitos (HU-24 / ED-120 a ED-122), garantizando el flujo seguro del primer inicio de sesión. En el segundo sprint, lideró la reportería analítica avanzada para el administrador (HU-40) incorporando rankings dinámicos y métricas de cancelación y asistencia. Adicionalmente, estructuró la pipeline de CI/CD en GitHub Actions y dio soporte técnico fundamental en la configuración del entorno Docker para el equipo. **Puntuación: 100 puntos.**

### 2. Carlos Eduardo Lau López (Carnet: 202202812)
* **Rol:** Scrum Master / Desarrollador
* **Evaluación:** Sobresaliente (98 / 100).
* **Justificación:** En su rol de facilitador ágil, garantizó el cumplimiento puntual de las 12 reuniones Daily Scrum, así como la moderación y registro de las grabaciones de Sprint Planning y Sprint Retrospective en Google Drive. En el apartado de desarrollo, implementó de punta a punta la actualización y baja de tutores (HU-35) y la visualización, edición y baja de estudiantes (HU-34), así como el reporte consolidado de calificación de estudiantes (HU-39) con su respectiva suite de pruebas unitarias en backend y componentes modulares en Vue 3. **Puntuación: 98 puntos.**

### 3. Christian David Chinchilla Santos (Carnet: 202308227)
* **Rol:** Desarrollador
* **Evaluación:** Sobresaliente (95 / 100).
* **Justificación:** Cumplió puntualmente con requerimientos críticos de integración. Desarrolló la arquitectura backend y frontend para el visor incrustado de documentos PDF (Carnet de estudiantes y CV de tutores en HU-32 y HU-33), eliminando la necesidad de descargas externas en las pantallas de aprobación de usuarios. Lideró además el módulo de administración para la gestión y resolución de denuncias contra tutores y estudiantes (HU-36 y HU-37), entregando código limpio, estructurado y con pruebas asociadas. **Puntuación: 95 puntos.**

### 4. Josue Daniel Revolorio Martínez (Carnet: 202102984)
* **Rol:** Desarrollador
* **Evaluación:** Sobresaliente — Mayor Crecimiento del Equipo (95 / 100).
* **Justificación:** Josue demostró la evolución más destacada del grupo respecto a la Fase 1. Mantuvo constancia en sus entregas desde el inicio de la fase. Implementó la interfaz visual del Plan de Estudio del estudiante (HU-25) y su exportación profesional a formato PDF institucional USAC (HU-26). En el segundo sprint, construyó el selector de calificación por estrellas del estudiante (HU-30) y el formulario con categorización de quejas hacia estudiantes (HU-31), complementando la entrega con documentación exhaustiva y capturas en el Manual de Usuario. **Puntuación: 95 puntos.**

### 5. Keitlyn Valentina Tunchez Castañeda (Carnet: 202201139)
* **Rol:** Desarrolladora
* **Evaluación:** Sobresaliente (94 / 100).
* **Justificación:** Keitlyn consolidó un trabajo sumamente prolijo y ordenado en el tratamiento de archivos binarios y formularios multipart. Desarrolló la obligatoriedad y validación estricta de extensiones y tamaños de fotografía y carnet en PDF para estudiantes (HU-22), así como el currículum vitae en PDF para tutores (HU-23). En el segundo sprint completó la funcionalidad de reporte de tutores (HU-28) y el reporte consolidado de calificación de tutores para administración (HU-38), respaldando sus componentes con pruebas unitarias y E2E que aseguraron estabilidad. **Puntuación: 94 puntos.**

### 6. Sebastian Antonio Romero Tzitzimit (Carnet: 202201690)
* **Rol:** Desarrollador
* **Evaluación:** Muy Bueno (92 / 100).
* **Justificación:** Sebastian cumplió satisfactoriamente con sus asignaciones funcionales, desarrollando el flujo de calificación y reseñas hacia tutores por parte de los alumnos (HU-27) y brindando soporte al visor PDF en el frontend (HU-33). Durante el cierre de la fase, asumió un rol protagónico en la consolidación del tablero Kanban, la estructuración de la documentación técnica y arquitectónica del proyecto y la verificación cruzada de pruebas Playwright. **Puntuación: 92 puntos.**

---

## 4. Tabla Consolidada Final del Proyecto (Fase 1 + Fase 2)

| Integrante | Carnet | Rol Principal | Promedio Fase 1 | Calificación Fase 2 | Calificación Final Consolidada (1-100) |
| :--- | :---: | :---: | :---: | :---: | :---: |
| **Eduardo Sebastian Gutierrez Felipe** | 202300694 | Product Owner / Dev | 100 | 100 | **100 / 100** |
| **Carlos Eduardo Lau López** | 202202812 | Scrum Master / Dev | 98 | 98 | **98 / 100** |
| **Christian David Chinchilla Santos** | 202308227 | Desarrollador | 93 | 95 | **94 / 100** |
| **Josue Daniel Revolorio Martínez** | 202102984 | Desarrollador | 83 | 95 | **89 / 100** |
| **Keitlyn Valentina Tunchez Castañeda** | 202201139 | Desarrolladora | 85 | 94 | **90 / 100** |
| **Sebastian Antonio Romero Tzitzimit** | 202201690 | Desarrollador | 93 | 92 | **93 / 100** |

---

## 5. Firma y Certificación del Scrum Master

Por medio de la presente, certifico en calidad de **Scrum Master del Grupo 7** que los puntajes y justificaciones expuestas reflejan fielmente el desempeño, compromiso y aportes técnicos de cada integrante en el cumplimiento de los objetivos de la Fase 2 del proyecto **EduConnect**.

**Scrum Master:** Carlos Eduardo Lau López  
**Carnet USAC:** 202202812  
**Fecha de Emisión:** 10 de octubre de 2026 (Actualizado al 19 de octubre de 2026)  
**Facultad de Ingeniería — Universidad de San Carlos de Guatemala**
