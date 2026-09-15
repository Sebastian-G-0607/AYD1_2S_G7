# Sprint Retrospective 1 — EduConnect

### Documentos relacionados

- [README del proyecto](../../README.md)
- [Tablero Kanban y evidencias](../../docs/tablero-kanban/tablero.md)
- [Daily Scrums del Sprint 1](sprint-dailies.md)
- [Sprint Planning 1](sprint-planning.md)
- [Calificación del equipo](calificacion-equipo.md)

**Universidad de San Carlos de Guatemala (USAC)**  
**Facultad de Ingeniería — Escuela de Ciencias y Sistemas**  
**Análisis y Diseño de Sistemas 1 (AYD1) — Segundo Semestre 2026**  
**Grupo 7**

---

### Ficha Técnica de la Ceremonia
* **Evento:** Sprint Retrospective 1
* **Sprint Evaluado:** Sprint 1 — Fundaciones, Autenticación y Flujo Base de Agendamiento
* **Fecha de Realización:** 3 de septiembre de 2026
* **Duración:** 30 minutos
* **Plataforma:** Google Meet
* **Enlace a la Grabación en Video:**  
  [Grabación Sprint Retrospective 1 en Google Drive](https://drive.google.com/file/d/1_vpaSY4U2ffFfP7xb4tgmE0YvEiARGTP/view?usp=drive_link)

### Participantes
* **Carlos Eduardo Lau López** (202202812) — **Scrum Master**
* **Eduardo Sebastián Gutiérrez Felipe** (202300694) — **Product Owner**
* **Christian David Chinchilla Santos** (202308227) — Equipo de Desarrollo
* **Josue Daniel Revolorio Martinez** (202102984) — Equipo de Desarrollo
* **Sebastian Antonio Romero Tzitzimit** (202201690) — Equipo de Desarrollo
* **Keitlyn Valentina Tunchez Castañeda** (202201139) — Equipo de Desarrollo

---

## 1. ¿Qué es la Sprint Retrospective?

La **Sprint Retrospective (Retrospectiva del Sprint)** es el último evento formal del marco de trabajo Scrum. Su propósito principal es permitir que el equipo inspeccione su propio desempeño durante el ciclo recién terminado, identificando fortalezas, dificultades encontradas y estableciendo compromisos concretos de mejora continua (*Kaizen*) para el siguiente Sprint.

---

## 2. ¿Cómo la aplicamos en el Grupo 7?

Al finalizar el **Sprint 1**, el equipo completo se reunió a través de Google Meet para analizar cómo se desarrolló el trabajo técnico y colaborativo. 

Siguiendo las directrices del curso, cada integrante del equipo respondió a las tres preguntas fundamentales de inspección:
1. **¿Qué se hizo bien durante el Sprint?**
2. **¿Qué se hizo mal o qué dificultades surgieron?**
3. **¿Qué mejoras debemos implementar para el siguiente Sprint?**

---

## 3. Respuestas y Conclusiones del Equipo

### 3.1 ¿Qué se hizo bien durante el Sprint?
* **Arquitectura base sólida:** La configuración inicial con Docker Compose y la separación de responsabilidades con Minimal APIs y Vue 3 permitió que todos tuvieran un entorno funcional desde los primeros días.
* **Cumplimiento de objetivos:** Se completaron al 100% las 10 Historias de Usuario planificadas (59 Story Points), dejando funcionales el registro, login, 2FA, aprobación de cuentas y la reserva base.
* **Comunicación constante:** El seguimiento mediante las reuniones Daily Scrum y el canal de mensajería ayudó a resolver dudas rápidamente.
* **Apoyo mutuo:** Cuando surgieron problemas de integración o dudas sobre cómo derivar la clave criptográfica del archivo 2FA, el equipo se coordinó para acordar una solución estándar.

---

### 3.2 ¿Qué se hizo mal o qué dificultades surgieron?
* **Fallas en herramientas locales:** Surgieron inconvenientes con extensiones de prueba (como la extensión de Postman en VS Code) que generaron retrasos puntuales al probar endpoints.
* **Curva de aprendizaje con Docker y Oracle:** En los primeros días, el tiempo de arranque de Oracle XE causó dudas al correr la API localmente.
* **Lógica compleja de traslapes:** El manejo de validaciones de no duplicidad y choque de horarios en la programación de sesiones requirió más iteraciones de las estimadas originalmente.
* **Nombres de ramas en Git:** Al inicio algunos integrantes crearon nombres de ramas que no seguían estrictamente el estándar `feature/carnet` y tuvieron que ser corregidas.

---

### 3.3 ¿Qué mejoras debemos implementar para el Sprint 2?
1. **Estandarizar ramas y Pull Requests desde el día 1:** Seguir rigurosamente la convención de Git Flow (`feature/nombre_carnet`) para evitar renombrados posteriores.
2. **Utilizar archivos `.http` y Swagger UI nativos:** Alternar de inmediato a Swagger UI o archivos `.http` locales cuando las extensiones externas fallen.
3. **Planificar revisiones tempranas de código (Code Reviews):** Realizar revisiones cruzadas de Pull Requests a lo largo del Sprint y no acumular integraciones al final para evitar conflictos de merge.
4. **Mapeo claro de DTOs antes de codificar:** Definir claramente los contratos de datos entre frontend y backend para agilizar la conexión de formularios.

---

## 4. Enlace a la Evidencia en Video

> **Grabación Oficial:**  
> Puedes consultar la grabación completa en video donde se muestra la participación de todos los integrantes y el análisis de la retrospectiva en el siguiente enlace:  
> [https://drive.google.com/file/d/1_vpaSY4U2ffFfP7xb4tgmE0YvEiARGTP/view?usp=drive_link](https://drive.google.com/file/d/1_vpaSY4U2ffFfP7xb4tgmE0YvEiARGTP/view?usp=drive_link)
