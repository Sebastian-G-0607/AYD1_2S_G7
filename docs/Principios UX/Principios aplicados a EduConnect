# Principios de Usabilidad de Nielsen aplicados en EduConnect

Este apartado describe cómo se aplican los diez principios de usabilidad de Jakob Nielsen dentro de EduConnect, tomando como referencia las funcionalidades implementadas para los roles de Administrador, Tutor y Estudiante.

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

---

## 2. Relación entre el sistema y el mundo real

> La interfaz debe utilizar conceptos, palabras y estructuras familiares para el usuario.

### Aplicación en EduConnect

La plataforma emplea terminología directamente relacionada con el entorno académico y las tutorías universitarias.

| Términos utilizados |
|---|
| Estudiante |
| Tutor |
| Materia |
| Sesión de tutoría |
| Horario de atención |
| Historial |
| Perfil |

Esto permite que las funcionalidades puedan comprenderse sin necesidad de conocer términos técnicos relacionados con la implementación del sistema.

---

## 3. Control y libertad del usuario

> El usuario debe poder controlar sus acciones y abandonar procesos cuando sea necesario.

### Aplicación en EduConnect

Las operaciones importantes requieren una acción explícita del usuario antes de ser ejecutadas.

Por ejemplo, el estudiante puede decidir cuándo guardar los cambios realizados en su perfil, cerrar modales antes de confirmar una operación y gestionar sus sesiones según las opciones habilitadas para su rol.

---

## 4. Consistencia y estándares

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

---

## 5. Prevención de errores

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

---

## 6. Reconocimiento antes que memorización

> La información necesaria debe estar visible para reducir la carga de memoria del usuario.

### Aplicación en EduConnect

La plataforma muestra información que el usuario necesitaría recordar de otra manera.

Por ejemplo, al seleccionar un tutor se presentan únicamente las materias que imparte y se permite consultar sus horarios disponibles y ocupados. Los datos existentes del perfil también se cargan automáticamente al ingresar a la sección **Mi Perfil**.

---

## 7. Flexibilidad y eficiencia de uso

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

---

## 8. Diseño estético y minimalista

> La interfaz debe presentar únicamente la información necesaria para cada tarea.

### Aplicación en EduConnect

Las vistas utilizan jerarquías visuales, espacios adecuados, tarjetas, colores e iconos para organizar la información sin sobrecargar la pantalla.

Cada rol visualiza únicamente las opciones correspondientes a sus responsabilidades. Por ejemplo, el estudiante dispone de accesos como **Explorar Tutores**, **Mis Sesiones**, **Historial** y **Mi Perfil**.

---

## 9. Reconocer, diagnosticar y recuperarse de errores

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

---

## 10. Ayuda y documentación

> El sistema debe proporcionar orientación cuando el usuario la necesite.

### Aplicación en EduConnect

La plataforma utiliza etiquetas descriptivas, textos de ayuda, placeholders, mensajes de validación y explicaciones sobre los requisitos de ciertos campos.

Adicionalmente, el proyecto cuenta con documentación específica, como el **Manual de Usuario**, que permite consultar los principales flujos de la plataforma.

---

## Resumen

| # | Principio | Aplicación principal en EduConnect |
|---:|---|---|
| 1 | Visibilidad del estado | Alertas, estados y disponibilidad |
| 2 | Relación con el mundo real | Terminología académica |
| 3 | Control y libertad | Confirmaciones y acciones cancelables |
| 4 | Consistencia | Componentes y patrones visuales comunes |
| 5 | Prevención de errores | Validaciones en formularios y sesiones |
| 6 | Reconocimiento | Datos, materias y horarios visibles |
| 7 | Flexibilidad | Filtros, búsquedas y navegación por rol |
| 8 | Diseño minimalista | Información específica para cada tarea |
| 9 | Recuperación de errores | Mensajes claros y específicos |
| 10 | Ayuda y documentación | Textos orientativos y manuales |
