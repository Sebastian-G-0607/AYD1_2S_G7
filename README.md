# EduConnect — Plataforma de Gestión de Tutorías Académicas

**Universidad de San Carlos de Guatemala**  
**Facultad de Ingeniería — Escuela de Ciencias y Sistemas**  
**Análisis y Diseño de Sistemas 1  — 2do Semestre 2026**  
**Grupo 7**

---

## Integrantes del Equipo

| Nombre Completo | Carnet | Rol Asignado |
| :--- | :---: | :---: |
| **Carlos Eduardo Lau López** | 202202812 | **Scrum Master** / Desarrollador |
| **Eduardo Sebastián Gutiérrez Felipe** | 202300694 | **Product Owner** / Desarrollador |
| **Christian David Chinchilla Santos** | 202308227 | Equipo de Desarrollo |
| **Josue Daniel Revolorio Martinez** | 202102984 | Equipo de Desarrollo |
| **Sebastian Antonio Romero Tzitzimit** | 202201690 | Equipo de Desarrollo |
| **Keitlyn Valentina Tunchez Castañeda** | 202201139 | Equipo de Desarrollo |

---

## Índice de Documentación del Proyecto

La documentación está organizada por etapa del proyecto. El recorrido recomendado es: definir necesidades, modelar el sistema, planificar el trabajo, diseñar la interfaz, implementar la solución y finalmente documentar su uso.

### Análisis y especificación

1. **[Requerimientos Funcionales](docs/requerimientos/requerimientos-funcionales.md)** — Funciones que debe ofrecer EduConnect.
2. **[Requerimientos No Funcionales](docs/requerimientos/requerimientos-no-funcionales.md)** — Seguridad, calidad, rendimiento y restricciones del sistema.
3. **[Diagrama de Casos de Uso](docs/casos-de-uso/diagrama-casos-de-uso.md)** — Actores, procesos principales y alcance funcional.
4. **[Diagrama Entidad-Relación](docs/diagrama-er/)** — Modelo de datos del sistema.

### Planificación y seguimiento

5. **[Product Backlog e Historias de Usuario](docs/product-backlog/historias-de-usuario.md)** — Priorización, dependencias y Story Points.
6. **[Tablero Kanban y Evidencias](docs/tablero-kanban/tablero.md)** — Flujo de trabajo, capturas y ceremonias Scrum.
7. **Sprint 1:** [Planning](scrum/sprint-1/sprint-planning.md) · [Dailies](scrum/sprint-1/sprint-dailies.md) · [Retrospective](scrum/sprint-1/sprint-retrospective.md) · [Calificación](scrum/sprint-1/calificacion-equipo.md)
8. **Sprint 2:** [Dailies](scrum/sprint-2/sprint-dailies.md) · [Retrospective](scrum/sprint-2/sprint-retrospective.md) · [Calificación](scrum/sprint-2/calificacion-equipo.md)

### Diseño y experiencia de usuario

9. **[Prototipado con Google Stitch](docs/prototipos/prototipos-stitch.md)** — Flujos, pantallas y sistema visual.
10. **[Principios de Usabilidad de Nielsen](docs/ux-ui/principios-nielsen.md)** — Aplicación de heurísticas de usabilidad en EduConnect.

### Implementación y operación

11. **[Manual Técnico](docs/manual-tecnico/manual-tecnico.md)** — Arquitectura, instalación, configuración y endpoints.
12. **[Manual de Usuario](docs/manual-usuario/manual-usuario.md)** — Guía visual para estudiantes, tutores y administradores.


Los documentos de cada sprint contienen además los registros de planning, daily scrum, retrospectiva y calificación del equipo en sus respectivas carpetas.

---

## Enlaces Importantes de Gestión y Herramientas

* **Tablero Jira Kanban:** [EduConnect G7 en Jira Cloud](https://edu-connect-g7.atlassian.net/jira/software/projects/ED/summary)
* **Prototipo Interactivo en Google Stitch:** [EduConnect Stitch Project](https://stitch.withgoogle.com/projects/11044310364948617047)
* **Reuniones Virtuales (Google Meet):** [https://meet.google.com/bxn-iweb-mby](https://meet.google.com/bxn-iweb-mby)

---

## Despliegue Rápido con Docker Compose

```bash
# 1. Configurar variables de entorno
cp .env.example .env

# 2. Levantar la plataforma completa en segundo plano
docker compose up --build -d

# 3. Acceso a las aplicaciones:
# - Frontend: http://localhost:5173
# - Backend API / Swagger: http://localhost:5000/swagger
# - Oracle Database XE: localhost:1523/XEPDB1
```