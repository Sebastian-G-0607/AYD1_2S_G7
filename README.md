# EduConnect — Plataforma de Gestión de Tutorías Académicas

**Universidad de San Carlos de Guatemala (USAC)**  
**Facultad de Ingeniería — Escuela de Ciencias y Sistemas**  
**Análisis y Diseño de Sistemas 1 (AYD1) — Segundo Semestre 2026**  
**Grupo 7**

---

##  Integrantes del Equipo de Desarrollo

| Nombre Completo | Carnet | Rol Principal en el Proyecto |
| :--- | :---: | :---: |
| **Carlos Eduardo Lau López** | 202202812 | **Scrum Master** / Desarrollador Backend & Frontend |
| **Eduardo Sebastián Gutiérrez Felipe** | 202300694 | **Product Owner** / Desarrollador Full-Stack |
| **Christian David Chinchilla Santos** | 202308227 | Equipo de Desarrollo (Backend & QA) |
| **Josue Daniel Revolorio Martinez** | 202102984 | Equipo de Desarrollo (Frontend & Reportes) |
| **Sebastian Antonio Romero Tzitzimit** | 202201690 | Equipo de Desarrollo (Full-Stack & Seguridad) |
| **Keitlyn Valentina Tunchez Castañeda** | 202201139 | Equipo de Desarrollo (Frontend & Validaciones) |

---

##  Índice General de la Documentación del Proyecto

A continuación se presenta el directorio maestro de entregables del sistema **EduConnect**, organizado cronológicamente según las fases del ciclo de vida del software:

### 1. Análisis y Especificación de Requerimientos
* **[Requerimientos Funcionales (RF)](docs/requerimientos/RF.md):** Especificación completa de los 40 requerimientos funcionales del sistema (Fase 1 y Fase 2).
* **[Requerimientos No Funcionales — Calidad (EAC)](docs/requerimientos/RNF-Calidad%20(EAC).md):** Escenarios de Atributos de Calidad (seguridad criptográfica, rendimiento, usabilidad, mantenibilidad).
* **[Requerimientos No Funcionales — Restricción](docs/requerimientos/RNF-Restriccion.md):** Limitaciones tecnológicas, estándares arquitectónicos y directrices del curso.
* **[Diagrama de Casos de Uso Core](docs/casos-de-uso/core.md):** Actores principales y casos de uso centrales de la plataforma.
* **[Casos de Uso del Negocio Expandidos (CUN)](docs/casos-de-uso/cun-expandidos.md):** Especificaciones narrativas expandidas con cursos normales, alternos y excepciones.
* **[Primera Descomposición de Casos de Uso](docs/casos-de-uso/primera-descomposicion.md):** Mapeo de descomposición funcional inicial.
* **[Diagrama Entidad-Relación (PDF)](docs/diagrama-er/edu-connect-er.pdf):** Esquema relacional formal en tercera forma normal (3FN) para Oracle Database.

### 2. Planificación, Backlog e Historias de Usuario
* **[Product Backlog](docs/product-backlog/product-backlog.md):** Listado priorizado de Historias de Usuario con criterios de aceptación (Given-When-Then) y estimación en Story Points.
* **[Sprint Backlog](docs/product-backlog/sprint-backlog.md):** Distribución de historias y desglose de tareas técnicas asignadas a cada iteración de desarrollo.
* **[Tablero Kanban y Evidencias de Gestión Ágil](docs/tablero-kanban/tablero.md):** Flujo de trabajo en Jira, políticas de columnas, capturas de seguimiento y métricas globales.

### 3. Registro de Reuniones y Ceremonias SCRUM

#### Fase 1 (Sprints 1 y 2)
* **Sprint 1:**
  - [Sprint Planning 1](scrum/sprint-1/sprint-planning.md) · *(Grabación en Drive)*
  - [Daily Scrums del Sprint 1](scrum/sprint-1/sprint-dailies.md) · *(Minutas diarias completas)*
  - [Sprint Retrospective 1](scrum/sprint-1/sprint-retrospective.md) · *(Grabación en Drive)*
  - [Calificación del Equipo Scrum — Sprint 1](scrum/sprint-1/calificacion-equipo.md)
* **Sprint 2:**
  - [Sprint Planning 2](scrum/sprint-2/sprint-planning.md) · *(Grabación en Drive)*
  - [Daily Scrums del Sprint 2](scrum/sprint-2/sprint-dailies.md) · *(Minutas diarias completas)*
  - [Sprint Retrospective 2](scrum/sprint-2/sprint-retrospective.md) · *(Grabación en Drive)*
  - [Calificación del Equipo Scrum — Sprint 2](scrum/sprint-2/calificacion-equipo.md)

#### Fase 2 (Sprints 1 y 2 / Sprints 3 y 4 Globales)
* **Sprint 1 (Fase 2):**
  - [Daily Scrums del Sprint 1 (Fase 2)](docs/tablero-kanban/Fase%202%20-%20Dailies%20sprint%201.md)
  - [Grabación Sprint Planning 1 (Drive)](https://drive.google.com/file/d/1HbWsrEQJ48EdGgmKzNFBxe9j-RGiSIfL/view?usp=drivesdk)
  - [Grabación Sprint Retrospective 1 (Drive)](https://drive.google.com/file/d/16kqTk8sTwLZ7XVo4j3HdBpANuAM7rgf1/view?usp=drivesdk)
* **Sprint 2 (Fase 2):**
  - [Daily Scrums del Sprint 2 (Fase 2)](docs/tablero-kanban/Fase%202%20-%20Dailies%20sprint%202.md)
  - [Grabación Sprint Planning 2 (Drive)](https://drive.google.com/file/d/1vwCyVQ9oFVsBaT03VRKmn_90KmrDpwCf/view?usp=drivesdk)
  - [Grabación Sprint Retrospective 2 (Drive)](https://drive.google.com/file/d/1eyTxsxvzjNZxjRDQUI6gPWD27x0rPAJd/view?usp=drive_link)
* **Evaluación Oficial del Scrum Master (Rango 1 a 100):**
  - [Evaluación y Calificación Integral Consolidada](docs/tablero-kanban/calificacion-equipo-fase2.md) · [Detalle en Tablero Kanban](docs/tablero-kanban/tablero.md#5-sección-oficial-del-scrum-master-calificación-del-equipo-de-desarrollo-escala-de-1-a-100)

### 4. Diseño y Experiencia de Usuario (UX / UI)
* **[Prototipado Interactivo con Google Stitch](docs/prototipos/prototipos-stitch.md):** Flujos interactivos, pantallas de alta fidelidad y sistema de diseño visual.
* **[Principios de Usabilidad de Jakob Nielsen](docs/ux-ui/principios-nielsen.md):** Evaluación y cumplimiento de las 10 heurísticas de usabilidad aplicadas en EduConnect.

### 5. Arquitectura, Implementación y Operación
* **[Arquitectura del Sistema y Modelo de Implementación](docs/implementacion/arquitectura-modelo.md):** Descripción formal de la arquitectura Vertical Slice (.NET 10 Minimal APIs), Feature-Driven Component Architecture (Vue 3 + TypeScript), seguridad criptográfica y persistencia en Oracle XE.
* **[Manual Técnico de Arquitectura e Instalación](docs/manual-tecnico/manual-tecnico.md):** Guía exhaustiva de despliegue, configuración de variables de entorno, diccionario de datos y especificación de endpoints.
* **[Manual de Usuario](docs/manual-usuario/manual-usuario.md):** Guía visual paso a paso para estudiantes, tutores y administradores.

---

##  Grabaciones Oficiales de Ceremonias SCRUM (Google Drive)

| Fase | Ceremonia Scrum | Fecha | Plataforma | Enlace de Acceso en Google Drive |
| :---: | :--- | :---: | :---: | :--- |
| **Fase 1** | **Sprint Planning 1** | 28/08/2026 | Google Drive | [Ver Video en Drive](https://drive.google.com/file/d/1ri4orJFaydUjhzw5CqiIwb76Mna0RGkg/view?usp=drive_link) |
| **Fase 1** | **Sprint Retrospective 1** | 03/09/2026 | Google Drive | [Ver Video en Drive](https://drive.google.com/file/d/1_vpaSY4U2ffFfP7xb4tgmE0YvEiARGTP/view?usp=drive_link) |
| **Fase 1** | **Sprint Planning 2** | 06/09/2026 | Google Drive | [Ver Video en Drive](https://drive.google.com/file/d/1beKELHINHqWvxiWkoaeZViC2a6F17LKB/view?usp=drive_link) |
| **Fase 1** | **Sprint Retrospective 2** | 13/09/2026 | Google Drive | [Ver Video en Drive](https://drive.google.com/file/d/1cs33HjPimm_d2zusLpagXPFDeMIj5nEp/view?usp=drive_link) |
| **Fase 2** | **Sprint Planning 1 (Fase 2)** | 28/09/2026 | Google Drive | [Ver Video en Drive](https://drive.google.com/file/d/1HbWsrEQJ48EdGgmKzNFBxe9j-RGiSIfL/view?usp=drivesdk) |
| **Fase 2** | **Sprint Retrospective 1 (Fase 2)** | 03/10/2026 | Google Drive | [Ver Video en Drive](https://drive.google.com/file/d/16kqTk8sTwLZ7XVo4j3HdBpANuAM7rgf1/view?usp=drivesdk) |
| **Fase 2** | **Sprint Planning 2 (Fase 2)** | 05/10/2026 | Google Drive | [Ver Video en Drive](https://drive.google.com/file/d/1vwCyVQ9oFVsBaT03VRKmn_90KmrDpwCf/view?usp=drivesdk) |
| **Fase 2** | **Sprint Retrospective 2 (Fase 2)** | 10/10/2026 | Google Drive | [Ver Video en Drive](https://drive.google.com/file/d/1eyTxsxvzjNZxjRDQUI6gPWD27x0rPAJd/view?usp=drive_link) |

---

##  Enlaces Importantes de Gestión y Herramientas

* **Tablero Jira Kanban:** [EduConnect G7 en Jira Cloud](https://edu-connect-g7.atlassian.net/?continue=https%3A%2F%2Fedu-connect-g7.atlassian.net%2Fwelcome%2Fsoftware%3FprojectId%3D10000&atlOrigin=eyJpIjoiZDI4NjFkNjZlMDg2NDU4NTkwODAyZTU3MDE)
* **Prototipo Interactivo en Google Stitch:** [EduConnect Stitch Project](https://stitch.withgoogle.com/projects/11044310364948617047)
* **Sala de Reuniones Virtuales (Google Meet):** [https://meet.google.com/bxn-iweb-mby](https://meet.google.com/bxn-iweb-mby)

---

##  Despliegue Rápido con Docker Compose

La plataforma completa está contenerizada y lista para su ejecución local:

```bash
# 1. Clonar el repositorio
git clone https://github.com/Sebastian-G-0607/AYD1_2S_G7.git
cd AYD1_2S_G7

# 2. Configurar variables de entorno desde la plantilla base
cp .env.example .env

# 3. Compilar y levantar los servicios en segundo plano
docker compose up --build -d

# 4. Monitorear el arranque de los contenedores
docker compose ps
docker compose logs -f api
```

### Puertos y URLs de Acceso Local

| Servicio | Contenedor | Puerto Anfitrión | URL de Acceso |
| :--- | :--- | :---: | :--- |
| **Frontend Web** | `frontend` | **5173** | [http://localhost:5173](http://localhost:5173) |
| **Backend REST API** | `api` | **5000** | [http://localhost:5000](http://localhost:5000) |
| **Swagger UI (OpenAPI)** | `api` | **5000** | [http://localhost:5000/swagger](http://localhost:5000/swagger) |
| **Oracle Database XE** | `oracle-db` | **1523** | `localhost:1523/XEPDB1` |