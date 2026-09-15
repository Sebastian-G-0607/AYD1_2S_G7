# Sprint Retrospective 2 — EduConnect

### Documentos relacionados

- [README del proyecto](../../README.md)
- [Tablero Kanban y evidencias](../../docs/tablero-kanban/tablero.md)
- [Daily Scrums del Sprint 2](sprint-dailies.md)
- [Calificación del equipo](calificacion-equipo.md)

**Universidad de San Carlos de Guatemala (USAC)**  
**Facultad de Ingeniería — Escuela de Ciencias y Sistemas**  
**Análisis y Diseño de Sistemas 1 (AYD1) — Segundo Semestre 2026**  
**Grupo 7**

---

### Ficha Técnica de la Ceremonia
* **Evento:** Sprint Retrospective 2 (Cierre del Proyecto)
* **Sprint Evaluado:** Sprint 2 — Gestión Operativa, Cancelaciones, Reportes, Historiales y Perfiles
* **Fecha de Realización:** 13 de septiembre de 2026
* **Duración:** 35 minutos
* **Plataforma:** Google Meet
* **Enlace a la Grabación en Video:**  
  `[PEGA_AQUÍ_EL_ENLACE_AL_VIDEO_DE_SPRINT_RETROSPECTIVE_2]`

### Participantes
* **Carlos Eduardo Lau López** (202202812) — **Scrum Master**
* **Eduardo Sebastián Gutiérrez Felipe** (202300694) — **Product Owner**
* **Christian David Chinchilla Santos** (202308227) — Equipo de Desarrollo
* **Josue Daniel Revolorio Martinez** (202102984) — Equipo de Desarrollo
* **Sebastian Antonio Romero Tzitzimit** (202201690) — Equipo de Desarrollo
* **Keitlyn Valentina Tunchez Castañeda** (202201139) — Equipo de Desarrollo

---

## 1. ¿Qué es la Sprint Retrospective?

La **Sprint Retrospective** es la reunión de cierre de cada iteración en la que el equipo evalúa cómo funcionaron las personas, las relaciones, los procesos y las herramientas. 

Para este **Sprint 2**, al ser el cierre definitivo del desarrollo de EduConnect, la retrospectiva tuvo un doble valor: evaluar la iteración reciente y reflexionar sobre todo el ciclo de vida del proyecto desde su concepción hasta la entrega final.

---

## 2. ¿Cómo la aplicamos en el Grupo 7?

El equipo se reunió por Google Meet al finalizar la entrega técnica del Sprint 2. Cada integrante del equipo compartió sus impresiones abiertas y sinceras sobre el proceso respondiendo a las preguntas de Scrum:

1. **¿Qué se hizo bien durante el Sprint 2 y a lo largo del proyecto?**
2. **¿Qué se hizo mal o qué obstáculos se presentaron?**
3. **¿Qué lecciones aprendidas y mejoras nos deja esta experiencia para futuros proyectos?**

---

## 3. Respuestas y Conclusiones del Equipo

### 3.1 ¿Qué se hizo bien durante el Sprint?
* **Entrega completa del alcance:** Se completaron las 11 Historias de Usuario asignadas (47 Story Points), alcanzando el 100% de la funcionalidad solicitada en el enunciado (reportes gráficos, cancelaciones con correos automáticos, gestión de bajas y perfiles).
* **Mejora en la dinámica de equipo:** Las recomendaciones del Sprint 1 se aplicaron exitosamente; el equipo mantuvo un ritmo constante y la comunicación en las Dailies permitió coordinar tareas cruzadas eficientemente.
* **Integración de servicios externos:** Se logró conectar de forma transparente el servicio de correos institucionales vía SMTP y el almacenamiento de fotografías en AWS S3 sin romper el flujo de la aplicación.
* **Cultura de apoyo:** Varios integrantes que finalizaron sus historias antes de tiempo apoyaron activamente en pruebas cruzadas de calidad (QA), detección de errores y documentación.

---

### 3.2 ¿Qué se hizo mal o qué obstáculos se presentaron?
* **Conflictos de merge en Git:** Al desarrollarse módulos simultáneos que modificaban rutas y vistas de administración (como HU-07 y HU-08), surgieron conflictos al integrar hacia la rama `develop` que requirieron tiempo adicional para resolverlos cuidadosamente.
* **Configuración inicial de S3:** Se presentó un contratiempo temporal con los permisos y nombres de variables del bucket de AWS S3 en el archivo `.env`, lo cual impidió momentáneamente la subida de fotos hasta corregir los parámetros.
* **Concentración de esfuerzo al cierre:** La redacción de manuales y consolidación de documentación se acumuló en los últimos días del Sprint junto con las pruebas finales del sistema.

---

### 3.3 Lecciones Aprendidas y Mejoras a Futuro
1. **Integración Continua Temprana:** Integrar ramas a `develop` de manera más frecuente y en bloques pequeños para que los conflictos de merge sean mínimos y fáciles de resolver.
2. **Documentación Progresiva:** Avanzar la documentación técnica y los manuales en paralelo con cada funcionalidad terminada, evitando la sobrecarga en las jornadas previas a la entrega.
3. **Validación de Entornos Compartidos:** Mantener un archivo `.env.example` rigurosamente actualizado desde el momento en que se introduce un servicio nuevo (como S3 o SMTP).
4. **Valor del Tablero Kanban:** Mantener actualizado el tablero en Jira permitió que todo el equipo tuviera claridad visual sobre el avance general y eliminó la duplicidad de esfuerzos.

---

## 4. Enlace a la Evidencia en Video

> **Grabación Oficial:**  
> La reunión de retrospectiva del Sprint 2 y cierre de proyecto fue grabada en video donde se muestra la participación del equipo completo:  
>  `[PEGA_AQUÍ_EL_ENLACE_AL_VIDEO_DE_SPRINT_RETROSPECTIVE_2]`
