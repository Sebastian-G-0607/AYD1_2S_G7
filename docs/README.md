# Portal de Documentación Oficial — EduConnect

**Universidad de San Carlos de Guatemala (USAC)**  
**Facultad de Ingeniería — Escuela de Ciencias y Sistemas**  
**Análisis y Diseño de Sistemas 1 (AYD1) — Segundo Semestre 2026**  
**Grupo 7**

---

Este directorio centraliza toda la documentación técnica, funcional, arquitectónica y de gestión ágil desarrollada para la plataforma **EduConnect**.

## Directorio de Documentación

### 1. Requerimientos del Sistema
- [Requerimientos Funcionales (RF)](requerimientos/RF.md) — 40 Requerimientos Funcionales documentados (Fase 1 y 2).
- [Requerimientos No Funcionales — Calidad (EAC)](requerimientos/RNF-Calidad%20(EAC).md) — Escenarios de Atributos de Calidad.
- [Requerimientos No Funcionales — Restricción](requerimientos/RNF-Restriccion.md) — Limitaciones y directrices de plataforma.

### 2. Casos de Uso y Modelado de Datos
- [Diagrama de Casos de Uso Core](casos-de-uso/core.md) — Actores y casos de uso del núcleo de la aplicación.
- [Casos de Uso del Negocio Expandidos (CUN)](casos-de-uso/cun-expandidos.md) — Especificación paso a paso de cursos normales y alternos.
- [Primera Descomposición de Casos de Uso](casos-de-uso/primera-descomposicion.md) — Desglose funcional inicial.
- [Diagrama Entidad-Relación (PDF)](diagrama-er/edu-connect-er.pdf) — Modelo conceptual y lógico de Oracle Database 19c XE.

### 3. Planificación Ágil, Backlog e Historias de Usuario
- [Product Backlog](product-backlog/product-backlog.md) — Historias de usuario priorizadas con criterios de aceptación Gherkin y Story Points.
- [Sprint Backlog](product-backlog/sprint-backlog.md) — Historias asignadas por iteración y subtareas técnicas.
- [Tablero Kanban y Evidencias Scrum (Fase 1 y Fase 2)](tablero-kanban/tablero.md) — Flujo en Jira, capturas obligatorias y ceremonias.

### 4. Registro de Reuniones y Ceremonias SCRUM
- **Fase 1:**
  - Sprint 1: [Sprint Planning](../scrum/sprint-1/sprint-planning.md) · [Daily Scrums](../scrum/sprint-1/sprint-dailies.md) · [Retrospectiva](../scrum/sprint-1/sprint-retrospective.md) · [Calificación](../scrum/sprint-1/calificacion-equipo.md)
  - Sprint 2: [Sprint Planning](../scrum/sprint-2/sprint-planning.md) · [Daily Scrums](../scrum/sprint-2/sprint-dailies.md) · [Retrospectiva](../scrum/sprint-2/sprint-retrospective.md) · [Calificación](../scrum/sprint-2/calificacion-equipo.md)
- **Fase 2:**
  - Sprint 1 (Fase 2): [Daily Scrums](tablero-kanban/Fase%202%20-%20Dailies%20sprint%201.md) · [Grabación Planning 1 (Drive)](https://drive.google.com/file/d/1HbWsrEQJ48EdGgmKzNFBxe9j-RGiSIfL/view?usp=drivesdk) · [Grabación Retrospective 1 (Drive)](https://drive.google.com/file/d/16kqTk8sTwLZ7XVo4j3HdBpANuAM7rgf1/view?usp=drivesdk)
  - Sprint 2 (Fase 2): [Daily Scrums](tablero-kanban/Fase%202%20-%20Dailies%20sprint%202.md) · [Grabación Planning 2 (Drive)](https://drive.google.com/file/d/1vwCyVQ9oFVsBaT03VRKmn_90KmrDpwCf/view?usp=drivesdk) · [Grabación Retrospective 2 (Drive)](https://drive.google.com/file/d/1eyTxsxvzjNZxjRDQUI6gPWD27x0rPAJd/view?usp=drive_link)
  - Evaluación Scrum Master: [Calificación del Equipo — Fase 2 (1 a 100)](tablero-kanban/calificacion-equipo-fase2.md) · [Sección en Tablero Kanban](tablero-kanban/tablero.md#5-sección-oficial-del-scrum-master-calificación-del-equipo-de-desarrollo-escala-de-1-a-100)

### 5. Prototipado y Experiencia de Usuario (UX / UI)
- [Prototipado Interactivo con Google Stitch](prototipos/prototipos-stitch.md) — Vistas, flujos y diseño de interfaces.
- [Principios de Usabilidad de Nielsen](ux-ui/principios-nielsen.md) — Análisis de las 10 heurísticas en EduConnect.

### 6. Arquitectura, Implementación y Operación
- [Arquitectura del Sistema y Modelo de Implementación](implementacion/arquitectura-modelo.md) — Vertical Slice (.NET 10), Feature-Driven (Vue 3) y persistencia en Oracle XE.
- [Manual Técnico de Arquitectura e Instalación](manual-tecnico/manual-tecnico.md) — Parámetros de entorno, instalación Docker, diccionario y endpoints.
- [Manual de Usuario](manual-usuario/manual-usuario.md) — Manual ilustrado para los roles Administrador, Tutor y Estudiante.
