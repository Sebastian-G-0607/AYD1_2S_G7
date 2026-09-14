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

Cada aspecto técnico y de gestión del proyecto se encuentra documentado de forma individual y exhaustiva en los siguientes enlaces:

1. **[Manual Técnico (Arquitectura, Instalación y Endpoints)](docs/manual-tecnico/manual-tecnico.md)**  
2. **[Requerimientos No Funcionales ](docs//requerimientos/requerimientos-no-funcionales.md)**
3. **[Product Backlog & Estimaciones en Story Points ](docs/Product_Backlog.md)**  
4. **[Prototipado y Diseño Frontend con Google Stitch](docs/Diseno_Frontend_Stitch.md)**  
5. **[Gestión Ágil: Tablero Kanban en Jira y Evidencias Scrum](docs/Gestion_Proyectos_Kanban.md)**  
6. **[Sprint 1](docs/Sprint_1.md)** 
7. **[Sprint 2](docs/Sprint_2.md)**  
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