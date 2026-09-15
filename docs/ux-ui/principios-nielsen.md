# Principios de Usabilidad de Nielsen aplicados en EduConnect

### Documentos relacionados

- [Prototipado con Google Stitch](../prototipos/prototipos-stitch.md)
- [Manual de Usuario](../manual-usuario/manual-usuario.md)
- [Requerimientos No Funcionales](../requerimientos/requerimientos-no-funcionales.md)

Este apartado presenta los seis principios de usabilidad de Nielsen seleccionados para EduConnect, justificando su elección y mostrando evidencia de cómo fueron aplicados dentro de las funcionalidades implementadas para los roles de Administrador, Tutor y Estudiante.

---

## 1. Visibilidad del estado del sistema

> El sistema debe mantener informado al usuario sobre lo que está ocurriendo mediante retroalimentación clara y oportuna.

### Aplicación en EduConnect

EduConnect informa constantemente al usuario sobre el resultado de las acciones realizadas y el estado actual de los diferentes procesos.

| Elemento | Aplicación |
|---|---|
| Sesiones | Se identifican mediante estados como **Pendiente**, **Atendida** o **Cancelada**. |
| Disponibilidad | Los horarios se muestran como **Disponibles** u **Ocupados**. |
| Formularios | Se muestran mensajes de confirmación o error luego de una operación. |
| Perfil | Se notifica cuando los datos o la contraseña se actualizan correctamente. |

**Ejemplo:** al programar una sesión, el estudiante recibe una confirmación si la operación fue exitosa o un mensaje explicativo si el horario ya no se encuentra disponible.

### Justificación de la selección

Se seleccionó este principio porque EduConnect maneja diferentes estados y procesos que deben ser comunicados claramente al usuario. Esto es especialmente importante durante la programación de sesiones, donde el estudiante necesita conocer qué horarios puede seleccionar antes de realizar una reserva.

### Evidencia de aplicación

En la disponibilidad del tutor, EduConnect diferencia visualmente los horarios que se encuentran disponibles de aquellos que ya están ocupados, permitiendo al estudiante conocer el estado de cada opción antes de programar una sesión.

[![Evidencia 1](https://i.ibb.co/6RVVbHwM/visibilidad.png)

---

## 2. Consistencia y estándares

> Los elementos similares deben mantener el mismo comportamiento y presentación dentro del sistema.

### Aplicación en EduConnect

EduConnect mantiene una estructura visual consistente entre los diferentes módulos.

| Componente | Uso |
|---|---|
| Menú lateral | Navegación principal según el rol |
| Encabezado | Información del usuario y acciones generales |
| Tarjetas | Agrupación de información |
| Formularios | Registro y actualización de datos |
| Modales | Acciones complementarias |
| Alertas | Confirmaciones y errores |
| Badges | Identificación visual de estados |

El uso de componentes reutilizables en Vue también permite mantener una experiencia uniforme en las distintas vistas.

### Justificación de la selección

Se seleccionó este principio porque EduConnect cuenta con diferentes módulos y roles. Mantener patrones visuales y de navegación similares facilita que los usuarios comprendan rápidamente cómo utilizar nuevas secciones de la plataforma.

### Evidencia de aplicación

La vista de **Historial de Sesiones** mantiene el mismo menú lateral, encabezado, tipografía, colores, botones y componentes utilizados en el resto del módulo del estudiante.

[![Evidencia 2](https://i.ibb.co/svd2mD3X/Consistencia-y-est-ndares.png)

---

## 3. Prevención de errores

> El sistema debe evitar que el usuario cometa errores antes de procesar una acción.

### Aplicación en EduConnect

La plataforma incorpora validaciones tanto en frontend como en backend.

| Validación | Comportamiento |
|---|---|
| Carnet | Valida el formato permitido |
| Teléfono | Requiere exactamente 8 dígitos |
| Contraseña | Exige mínimo 8 caracteres, mayúscula, minúscula y número |
| Cambio de contraseña | Solicita y valida la contraseña actual |
| Correo del perfil | Se visualiza, pero no puede modificarse |
| Materias | Solo se muestran las impartidas por el tutor seleccionado |
| Horarios | Se evita programar sesiones en horarios ocupados |
| Traslapes | Se evita que un estudiante tenga sesiones superpuestas |

### Justificación de la selección

Se seleccionó este principio porque varias operaciones de EduConnect requieren validar información antes de ser procesada. Esto permite reducir registros incorrectos, sesiones inválidas y errores relacionados con datos personales o credenciales.

### Evidencia de aplicación

Al cambiar la contraseña desde el perfil del estudiante, el sistema muestra los requisitos mínimos de seguridad y evita realizar el cambio cuando la nueva contraseña no cumple con las condiciones establecidas.

[![Evidencia 3](https://i.ibb.co/MkQr8SnG/validacion-contra.png)

---

## 4. Reconocimiento antes que memorización

> La información necesaria debe estar visible para reducir la carga de memoria del usuario.

### Aplicación en EduConnect

La plataforma muestra información que el usuario necesitaría recordar de otra manera.

Por ejemplo, al seleccionar un tutor se presentan únicamente las materias que imparte y se permite consultar sus horarios disponibles y ocupados. Los datos existentes del perfil también se cargan automáticamente al ingresar a la sección **Mi Perfil**.

### Justificación de la selección

Se seleccionó este principio porque permite que el estudiante tome decisiones utilizando información que el sistema ya conoce, evitando que tenga que memorizar datos previamente consultados sobre tutores, materias o disponibilidad.

### Evidencia de aplicación

Al programar una sesión, EduConnect muestra únicamente las materias impartidas por el tutor seleccionado. De esta manera, el estudiante no necesita recordar qué materias ofrece cada tutor y solo puede seleccionar opciones válidas.

[![Evidencia 4](https://i.ibb.co/sJ3qcwFJ/recon.png)

---

## 5. Flexibilidad y eficiencia de uso

> La interfaz debe facilitar que las tareas puedan completarse de forma rápida y eficiente.

### Aplicación en EduConnect

EduConnect incorpora herramientas que reducen el tiempo necesario para localizar información.

| Funcionalidad | Beneficio |
|---|---|
| Filtros de tutores | Reduce los resultados según las preferencias del estudiante |
| Buscadores | Facilitan encontrar registros específicos |
| Filtro de historial | Permite separar sesiones atendidas y canceladas |
| Datos precargados | Facilitan la actualización del perfil |
| Navegación por rol | Muestra únicamente las opciones relevantes |

### Justificación de la selección

Se seleccionó este principio porque los usuarios pueden acumular una cantidad considerable de información dentro de la plataforma. Las herramientas de búsqueda y filtrado permiten encontrar rápidamente los datos necesarios sin revisar manualmente todos los registros.

### Evidencia de aplicación

La vista de **Historial de Sesiones** incluye un buscador y un filtro por estado, permitiendo localizar sesiones específicas por tutor, materia, motivo o estado.

[![Evidencia 2](https://i.ibb.co/svd2mD3X/Consistencia-y-est-ndares.png)

---

## 6. Reconocer, diagnosticar y recuperarse de errores

> Los mensajes de error deben indicar claramente qué ocurrió y cómo puede corregirse.

### Aplicación en EduConnect

Los errores se presentan mediante mensajes específicos en lugar de respuestas genéricas.

Algunos casos contemplados son:

- Contraseña actual incorrecta.
- Nueva contraseña que no cumple los requisitos.
- Tutor no disponible en la fecha seleccionada.
- Horario ocupado.
- Conflicto o traslape de sesiones.
- Materia no impartida por el tutor.
- Datos obligatorios incompletos.

### Justificación de la selección

Se seleccionó este principio porque el usuario debe conocer la causa de una operación inválida para poder corregirla y continuar utilizando la plataforma sin confusión.

### Evidencia de aplicación

Cuando el estudiante selecciona una fecha en la que el tutor no brinda atención, EduConnect muestra un mensaje indicando que el tutor no atiende en la fecha seleccionada. Esto permite identificar inmediatamente el problema y elegir otra fecha.

[![Evidencia 6](https://i.ibb.co/Vcr7dkGX/noatiende.png)

---

## Resumen

| # | Principio | Aplicación principal en EduConnect |
|---:|---|---|
| 1 | Visibilidad del estado del sistema | Estados, alertas y disponibilidad |
| 2 | Consistencia y estándares | Componentes y patrones visuales comunes |
| 3 | Prevención de errores | Validaciones en formularios y sesiones |
| 4 | Reconocimiento antes que memorización | Materias, horarios y datos visibles |
| 5 | Flexibilidad y eficiencia de uso | Filtros, búsquedas y navegación |
| 6 | Reconocer, diagnosticar y recuperarse de errores | Mensajes claros y específicos |
