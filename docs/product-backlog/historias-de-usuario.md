# EduConnect

## Product Backlog

### Módulo 1: Registro y Autenticación

#### HU-01: Registro de Estudiante

| Campo | Detalle |
| :--- | :--- |
| **Descripción** | Como estudiante quiero registrarme en el sistema ingresando mis datos personales para poder acceder a las funcionalidades de la plataforma y agendar tutorías. |
| **Criterios de Aceptación** | 1. El formulario debe solicitar: nombre, apellido, carnet universitario, género, dirección, teléfono, fecha de nacimiento, correo electrónico y contraseña.<br>2. El formulario permite subir una fotografía de perfil opcional.<br>3. La contraseña debe tener un mínimo de 8 caracteres, incluyendo al menos una letra minúscula, una mayúscula y un número.<br>4. La contraseña debe almacenarse encriptada en la base de datos.<br>5. El sistema debe validar que el correo electrónico no esté duplicado. |
| **Prioridad** | Alta |
| **Estimación** | |
| **Dependencias** | Ninguna |
| **Módulo** | Registro y Autenticación |

#### HU-02: Registro de Tutor

| Campo | Detalle |
| :--- | :--- |
| **Descripción** | Como tutor quiero registrarme en el sistema ingresando mi información profesional y personal para publicar mi perfil y ofrecer sesiones de tutoría. |
| **Criterios de Aceptación** | 1. El formulario debe solicitar: nombre, apellido, carnet o ID, fecha de nacimiento, género, dirección, teléfono, número de identificación de tutor, especialidad (materias que imparte), dirección de tutoría (física u online), correo electrónico, año de inicio de tutorías, universidad de graduación y contraseña.<br>2. El formulario solicita subir una fotografía de perfil obligatoria.<br>3. Se debe validar la unicidad del correo electrónico y del número de identificación del tutor.<br>4. La contraseña debe tener un mínimo de 8 caracteres, incluir minúscula, mayúscula y número, y almacenarse de forma encriptada. |
| **Prioridad** | Alta |
| **Estimación** | |
| **Dependencias** | Ninguna |
| **Módulo** | Registro y Autenticación |

#### HU-03: Inicio de Sesión (Estudiantes y Tutores)

| Campo | Detalle |
| :--- | :--- |
| **Descripción** | Como usuario quiero iniciar sesión con mi correo y contraseña para acceder a mi panel correspondiente. |
| **Criterios de Aceptación** | 1. El sistema debe verificar que el usuario haya sido aprobado previamente por el administrador; si no está aprobado, debe impedir el acceso.<br>2. Si hay un error de autenticación, el sistema debe mostrar un mensaje con la información del problema.<br>3. Debe existir un enlace visible para registrarse si el usuario no tiene cuenta. |
| **Prioridad** | Alta |
| **Estimación** | |
| **Dependencias** | HU-01, HU-02, HU-05, HU-06 |
| **Módulo** | Registro y Autenticación |

#### HU-04: Inicio de Sesión de Administrador (2FA)

| Campo | Detalle |
| :--- | :--- |
| **Descripción** | Como administrador quiero iniciar sesión mediante un proceso de doble autenticación para acceder de forma segura al panel de administración. |
| **Criterios de Aceptación** | 1. El administrador debe ingresar primero con un usuario y contraseña predeterminados.<br>2. Tras el primer paso, debe ser redirigido a una página para subir un archivo llamado `auth2-ayd1.txt`.<br>3. El archivo debe contener una contraseña encriptada que, al ser validada por el sistema, permitirá el acceso a la página principal.<br>4. La contraseña del primer inicio de sesión y la del archivo deben ser diferentes. |
| **Prioridad** | Alta |
| **Estimación** | |
| **Dependencias** | Ninguna |
| **Módulo** | Registro y Autenticación |

---

### Módulo 4: Administrador

#### HU-05: Aprobación de Registro de Estudiantes

| Campo | Detalle |
| :--- | :--- |
| **Descripción** | Como administrador quiero ver la lista de estudiantes pendientes de aprobación para aceptar o rechazar sus solicitudes de ingreso a la plataforma. |
| **Criterios de Aceptación** | 1. El sistema debe mostrar una lista con fotografía (o una por defecto si no tiene), nombre completo, carnet, género, fecha de nacimiento y correo del estudiante.<br>2. Debe existir un botón para aceptar o rechazar la solicitud a la par de cada registro.<br>3. Se debe enviar un correo de notificación al usuario informando si su cuenta fue aprobada o rechazada. |
| **Prioridad** | Alta |
| **Estimación** | |
| **Dependencias** | HU-01 |
| **Módulo** | Administrador |

#### HU-06: Aprobación de Registro de Tutores

| Campo | Detalle |
| :--- | :--- |
| **Descripción** | Como administrador quiero visualizar los tutores pendientes de aprobación para verificar sus perfiles y permitirles usar el sistema. |
| **Criterios de Aceptación** | 1. La lista debe mostrar fotografía, nombre completo, carnet, género, especialidad, número de identificación y correo electrónico.<br>2. Debe incluir botones para aceptar o rechazar a cada tutor.<br>3. Se debe enviar un correo electrónico al tutor notificando si fue aprobado o rechazado. |
| **Prioridad** | Alta |
| **Estimación** | |
| **Dependencias** | HU-02 |
| **Módulo** | Administrador |

#### HU-07: Gestión de Usuarios Activos

| Campo | Detalle |
| :--- | :--- |
| **Descripción** | Como administrador quiero ver a todos los estudiantes y tutores activos en el sistema para poder darlos de baja si es necesario. |
| **Criterios de Aceptación** | 1. Debe existir una vista para ver todos los estudiantes ya aceptados y una opción para darlos de baja.<br>2. Debe existir una vista separada para ver todos los tutores aceptados y una opción para darlos de baja.<br>3. Al dar de baja a un usuario, se le debe enviar automáticamente una notificación por correo electrónico informándole sobre la baja de su cuenta.<br>4. El usuario dado de baja no debe poder iniciar sesión y su estado debe pasar a inactivo. |
| **Prioridad** | Media |
| **Estimación** | |
| **Dependencias** | HU-05, HU-06 |
| **Módulo** | Administrador |

#### HU-08: Generación de Reportes

| Campo | Detalle |
| :--- | :--- |
| **Descripción** | Como administrador quiero generar reportes del sistema para tomar decisiones estratégicas basadas en el uso de la plataforma. |
| **Criterios de Aceptación** | 1. El sistema debe permitir generar al menos dos reportes relevantes (ej. tutores que más estudiantes han atendido o materia con más demanda).<br>2. Los reportes deben presentarse de forma gráfica incluyendo tablas y elementos visuales (como gráficos de barras o circulares).<br>3. Los datos consumidos para los reportes y gráficos deben ser precisos y actualizados al momento de la consulta. |
| **Prioridad** | Media |
| **Estimación** | |
| **Dependencias** | HU-12, HU-18 |
| **Módulo** | Administrador |

#### HU-09: Visualización de usuarios dados de baja

| Campo | Detalle |
| :--- | :--- |
| **Descripción** | Como administrador quiero ver la lista de usuarios que han sido dados de baja del sistema. |
| **Criterios de Aceptación** | 1. El sistema debe permitir ver la lista de estudiantes y tutores que han sido dados de baja, mostrando su nombre, correo electrónico, fecha de baja y motivo.<br>2. La vista debe permitir filtrar o buscar usuarios dados de baja por rol (estudiante o tutor), nombre o correo electrónico.<br>3. El administrador debe poder consultar el detalle completo de la información del usuario inactivo y el motivo registrado de su baja. |
| **Prioridad** | Media |
| **Estimación** | |
| **Dependencias** | HU-05, HU-06, HU-07 |
| **Módulo** | Administrador |

---

### Módulo 3: Tutor

#### HU-10: Establecer Horarios de Atención

| Campo | Detalle |
| :--- | :--- |
| **Descripción** | Como tutor quiero configurar mis días y horas de disponibilidad para que los estudiantes puedan agendar sesiones conmigo. |
| **Criterios de Aceptación** | 1. El sistema debe permitir seleccionar los días de la semana en los que se atenderá.<br>2. El sistema debe permitir establecer un rango de horario (ej. 8 am a 5 pm) que aplicará uniformemente para todos los días seleccionados.<br>3. El sistema debe validar que la hora de inicio sea anterior a la hora de fin y confirmar el guardado exitoso de los horarios configurados. |
| **Prioridad** | Alta |
| **Estimación** | |
| **Dependencias** | HU-03 |
| **Módulo** | Tutor |

#### HU-11: Actualizar Horarios de Atención

| Campo | Detalle |
| :--- | :--- |
| **Descripción** | Como tutor quiero modificar mis horarios y días de atención para adaptar mi disponibilidad a mis necesidades actuales. |
| **Criterios de Aceptación** | 1. El tutor puede cambiar los días y la hora de atención en el sistema.<br>2. El sistema debe validar que no existan sesiones activas fuera del nuevo rango de horario.<br>3. Si existen conflictos, no se debe permitir la actualización hasta que las sesiones afectadas sean reprogramadas o canceladas. |
| **Prioridad** | Media |
| **Estimación** | |
| **Dependencias** | HU-10 |
| **Módulo** | Tutor |

#### HU-12: Gestión y Atención de Sesiones Pendientes

| Campo | Detalle |
| :--- | :--- |
| **Descripción** | Como tutor quiero ver mis sesiones pendientes y marcarlas como atendidas para llevar el control del progreso de mis estudiantes. |
| **Criterios de Aceptación** | 1. La vista debe mostrar las sesiones ordenadas por fecha más próxima, indicando fecha, hora, nombre del estudiante, motivo y materia.<br>2. Debe existir un botón para marcar al estudiante como "Atendido".<br>3. Al marcar como atendido, debe desplegarse un formulario para ingresar el resumen o recomendaciones de la sesión antes de hacerla desaparecer de la lista de pendientes. |
| **Prioridad** | Alta |
| **Estimación** | |
| **Dependencias** | HU-18 |
| **Módulo** | Tutor |

#### HU-13: Cancelación de Sesión por el Tutor

| Campo | Detalle |
| :--- | :--- |
| **Descripción** | Como tutor quiero poder cancelar una sesión agendada para notificar al estudiante si me surge algún inconveniente. |
| **Criterios de Aceptación** | 1. El tutor debe poder seleccionar una sesión pendiente y cancelarla, quitándola de su lista.<br>2. Al confirmar la cancelación, el horario asociado debe liberarse inmediatamente en la agenda del tutor para que vuelva a estar disponible.<br>3. El sistema debe enviar un correo automático al estudiante notificando la cancelación.<br>4. El correo debe incluir: fecha, hora, motivo de la sesión cancelada, nombre del tutor, materia y un mensaje de disculpa. |
| **Prioridad** | Alta |
| **Estimación** | |
| **Dependencias** | HU-12 |
| **Módulo** | Tutor |

#### HU-14: Historial de Sesiones

| Campo | Detalle |
| :--- | :--- |
| **Descripción** | Como tutor quiero poder ver el historial de todas mis sesiones para tener un registro de mi actividad. |
| **Criterios de Aceptación** | 1. El sistema debe mostrar un listado de todas las sesiones que ha tenido el tutor, incluyendo fecha, hora, nombre del estudiante y estado de la sesión.<br>2. Debe permitir filtrar por fechas o por estudiante.<br>3. El historial debe ser accesible desde el panel de usuario del tutor. |
| **Prioridad** | Alta |
| **Estimación** | |
| **Dependencias** | HU-12, HU-13 |
| **Módulo** | Tutor |

#### HU-15: Ver y actualizar perfil

| Campo | Detalle |
| :--- | :--- |
| **Descripción** | Como tutor quiero poder visualizar los datos de mi perfil y actualizarlos si es necesario. |
| **Criterios de Aceptación** | 1. El sistema debe permitir ver todos los datos registrados en el perfil del tutor.<br>2. Debe permitir modificar cualquiera de los campos mostrados, a excepción del correo electrónico.<br>3. Debe permitir subir una nueva fotografía de perfil.<br>4. Si se solicita cambio de contraseña, el formulario debe exigir el ingreso de la contraseña original para validarla contra la base de datos antes de proceder.<br>5. La nueva contraseña debe cumplir con las validaciones de seguridad (mínimo 8 caracteres, mayúscula, minúscula y número) y guardarse encriptada en la base de datos. |
| **Prioridad** | Alta |
| **Estimación** | |
| **Dependencias** | HU-02 |
| **Módulo** | Tutor |

---

### Módulo 2: Estudiante

#### HU-16: Exploración y Búsqueda de Tutores

| Campo | Detalle |
| :--- | :--- |
| **Descripción** | Como estudiante quiero ver la lista de tutores disponibles y usar filtros avanzados para encontrar al tutor que mejor se adapte a mis necesidades. |
| **Criterios de Aceptación** | 1. La página principal debe mostrar los tutores registrados (exceptuando aquellos con los que ya se tiene sesión), exhibiendo nombre completo, especialidad (materias), dirección de tutoría y foto.<br>2. Debe existir una búsqueda avanzada para filtrar por: materia, años de experiencia, sexo, edad y universidad.<br>3. Los filtros deben implementarse de manera intuitiva y eficiente, permitiendo al usuario una experiencia agradable. |
| **Prioridad** | Alta |
| **Estimación** | |
| **Dependencias** | HU-02, HU-06 |
| **Módulo** | Estudiante |

#### HU-17: Consulta de Horarios y Disponibilidad

| Campo | Detalle |
| :--- | :--- |
| **Descripción** | Como estudiante quiero ver los horarios de un tutor específico para saber en qué momentos está disponible para una sesión. |
| **Criterios de Aceptación** | 1. Al seleccionar un tutor, se deben mostrar los días y horarios que atiende.<br>2. El sistema debe permitir filtrar por fecha y mostrar claramente los horarios ocupados y disponibles para ese día específico.<br>3. Debe indicar si el tutor no atiende en la fecha seleccionada. |
| **Prioridad** | Alta |
| **Estimación** | |
| **Dependencias** | HU-10, HU-16 |
| **Módulo** | Estudiante |

#### HU-18: Programar Sesión de Tutoría

| Campo | Detalle |
| :--- | :--- |
| **Descripción** | Como estudiante quiero agendar una sesión con un tutor llenando un formulario para reservar mi espacio. |
| **Criterios de Aceptación** | 1. El formulario debe solicitar: fecha, hora, materia (impartida por el tutor) y motivo de la sesión.<br>2. El sistema debe validar que la fecha y hora seleccionadas estén dentro del horario del tutor y que el espacio esté disponible.<br>3. El sistema debe impedir agendar más de una sesión con el mismo tutor simultáneamente, y evitar traslapes de horario en sesiones del mismo día.<br>4. Si falla alguna validación, se debe notificar al usuario el motivo específico. |
| **Prioridad** | Alta |
| **Estimación** | |
| **Dependencias** | HU-17 |
| **Módulo** | Estudiante |

#### HU-19: Gestión de Sesiones Activas

| Campo | Detalle |
| :--- | :--- |
| **Descripción** | Como estudiante quiero ver la lista de mis próximas sesiones y tener la opción de cancelarlas si no podré asistir. |
| **Criterios de Aceptación** | 1. Debe existir una vista de "Sesiones Activas" mostrando fecha, hora, tutor, materia, dirección y motivo.<br>2. Debe haber una opción para cancelar la sesión, requiriendo un mensaje de confirmación antes de removerla de la lista de activas.<br>3. Al confirmar la cancelación, la sesión debe cambiar a estado "Cancelada por el estudiante" y liberar inmediatamente ese horario en la agenda del tutor para que esté disponible para otros alumnos. |
| **Prioridad** | Media |
| **Estimación** | |
| **Dependencias** | HU-18 |
| **Módulo** | Estudiante |

#### HU-20: Visualización de Historial de Sesiones

| Campo | Detalle |
| :--- | :--- |
| **Descripción** | Como estudiante quiero ver un historial de mis sesiones pasadas o canceladas para llevar un control de mi actividad en la plataforma. |
| **Criterios de Aceptación** | 1. El estudiante debe ver sesiones atendidas y canceladas mostrando: fecha, tutor, materia, motivo, dirección, resumen de la sesión (si fue atendida) y estado.<br>2. El sistema debe permitir filtrar o buscar en el historial por tutor, materia, estado o rango de fechas.<br>3. El estudiante debe poder acceder al detalle completo de las notas y el resumen pedagógico proporcionado por el tutor en las sesiones completadas. |
| **Prioridad** | Baja |
| **Estimación** | |
| **Dependencias** | HU-12, HU-13, HU-19 |
| **Módulo** | Estudiante |

#### HU-21: Gestión de Perfil

| Campo | Detalle |
| :--- | :--- |
| **Descripción** | Como estudiante quiero ver y editar mis datos personales para mantener mi información actualizada. |
| **Criterios de Aceptación** | 1. El estudiante debe poder visualizar todos los datos registrados en su perfil.<br>2. El estudiante debe poder modificar cualquiera de los campos mostrados, a excepción del correo electrónico.<br>3. Si se solicita cambio de contraseña, el sistema debe requerir obligatoriamente la contraseña original y validarla antes de permitir el cambio.<br>4. La nueva contraseña debe cumplir con los requisitos mínimos de seguridad (8 caracteres, mayúscula, minúscula y número) y almacenarse encriptada en la base de datos. |
| **Prioridad** | Baja |
| **Estimación** | |
| **Dependencias** | HU-01 |
| **Módulo** | Estudiante |