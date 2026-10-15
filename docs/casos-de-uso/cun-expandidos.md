# Casos de Uso Expandidos: Descripciones Textuales
## Sistema de Tutorías — EduConnect

> La descripción textual complementa el diagrama y detalla qué hace el actor, qué responde el sistema, qué condiciones deben cumplirse y qué ocurre cuando se presenta una excepción. Describe comportamiento observable, sin mencionar clases, tablas ni endpoints.

Al expandir los procesos de la primera descomposición se obtuvieron **40 casos de uso expandidos**, uno por cada historia de usuario, agrupados en 4 procesos. Los diagramas de cada proceso están en `educonnect-casos-de-uso.drawio` (páginas CU1 a CU4).

---

## Resumen de casos de uso expandidos

| Proceso | Casos de uso | Total |
|---------|--------------|-------|
| CU1 Gestionar Registro y Acceso | CU1-1 al CU1-7 | 7 |
| CU2 Gestionar Tutorías del Estudiante | CU2-1 al CU2-10 | 10 |
| CU3 Gestionar Tutorías del Tutor | CU3-1 al CU3-9 | 9 |
| CU4 Administrar la Plataforma | CU4-1 al CU4-14 | 14 |
| **TOTAL** | | **40** |

## Relaciones entre casos de uso

| Caso de uso base | Relación | Caso de uso relacionado | Punto de la relación |
|------------------|----------|-------------------------|----------------------|
| CU1-1 | «include» | CU1-5 | Paso 4 del registro del estudiante |
| CU1-2 | «include» | CU1-6 | Paso 5 del registro del tutor |
| CU1-3 | «extend» ← | CU1-7 | Primer inicio de sesión |
| CU2-3 | «include» | CU2-2 | Paso 1 del agendamiento |
| CU2-1 | «extend» ← | CU2-2 | Al seleccionar un tutor de la lista |
| CU2-7 | «extend» ← | CU2-8 | Al solicitar el PDF desde el plan de estudio |
| CU3-3 | «extend» ← | CU3-4 | Al elegir «Cancelar» en una sesión pendiente |
| CU3-3 | «extend» ← | CU3-7 | Al elegir «Atendido» en una sesión pendiente |
| CU4-1 | «extend» ← | CU4-6 | Al revisar el carnet de un estudiante pendiente |
| CU4-2 | «extend» ← | CU4-7 | Al revisar el CV de un tutor pendiente |
| CU4-3 | «extend» ← | CU4-8 | Al editar o dar de baja desde la lista de estudiantes activos |
| CU4-3 | «extend» ← | CU4-9 | Al editar o dar de baja desde la lista de tutores activos |

---

## **CU1 GESTIONAR REGISTRO Y ACCESO**

### CU1-1 Registrar Estudiante

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Registrar Estudiante |
| **Actores** | Estudiante (iniciador) |
| **Propósito** | Permitir que un estudiante solicite su ingreso a la plataforma creando una cuenta pendiente de aprobación. |
| **Resumen** | El estudiante completa el formulario de registro con sus datos personales y credenciales. El sistema incluye la carga de fotografía y carnet (CU1-5), valida la contraseña y la unicidad del correo, crea la cuenta en estado «Pendiente de aprobación» y envía el token de verificación al correo. |
| **Condiciones previas** | El estudiante no debe tener una cuenta registrada con el mismo correo electrónico. |
| **Secuencia normal** | 1. El estudiante selecciona la opción «Registrarse como estudiante».<br>2. El sistema muestra el formulario de registro.<br>3. El estudiante ingresa nombre, apellido, carnet universitario, género, dirección, teléfono, fecha de nacimiento, correo electrónico y contraseña.<br>4. Se invoca el caso de uso «Cargar Fotografía y Carnet del Estudiante» (CU1-5).<br>5. El sistema valida que la contraseña tenga mínimo 8 caracteres e incluya al menos una minúscula, una mayúscula y un número.<br>6. El sistema valida que el correo electrónico no esté registrado.<br>7. El sistema crea la cuenta en estado «Pendiente de aprobación» y almacena la contraseña encriptada.<br>8. El sistema envía el token de verificación al correo del estudiante (CU1-7).<br>9. El sistema confirma el registro e informa que la cuenta queda pendiente de aprobación del administrador. |
| **Excepciones / Cursos alternativos** | Paso 3: Si falta algún campo obligatorio, el sistema señala los campos requeridos y no continúa.<br>Paso 5: Si la contraseña no cumple los requisitos, el sistema muestra los criterios faltantes.<br>Paso 6: Si el correo ya está registrado, el sistema rechaza el registro e informa que el correo está duplicado. |
| **Condiciones posteriores** | Existe una cuenta de estudiante en estado «Pendiente de aprobación», con la contraseña encriptada, visible para el administrador en CU4-1. |
| **Comentarios** | HU-01 · RF-1-1-1 a RF-1-1-6 · RR-3, RR-9 · EAC-9. Relación: «include» CU1-5. |

<br>

### CU1-2 Registrar Tutor

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Registrar Tutor |
| **Actores** | Tutor (iniciador) |
| **Propósito** | Permitir que un tutor solicite su ingreso a la plataforma con su información personal y profesional. |
| **Resumen** | El tutor completa el formulario con sus datos personales y profesionales. El sistema incluye la carga de fotografía y CV (CU1-6), valida la unicidad del correo y del número de identificación del tutor, valida la contraseña, crea la cuenta en estado «Pendiente de aprobación» y envía el token de verificación. |
| **Condiciones previas** | El tutor no debe tener una cuenta registrada con el mismo correo ni con el mismo número de identificación de tutor. |
| **Secuencia normal** | 1. El tutor selecciona la opción «Registrarse como tutor».<br>2. El sistema muestra el formulario de registro.<br>3. El tutor ingresa nombre, apellido, carnet o ID, fecha de nacimiento, género, dirección, teléfono, número de identificación de tutor, correo electrónico y contraseña.<br>4. El tutor ingresa especialidad (materias que imparte), dirección de tutoría (física u online), año de inicio de tutorías y universidad de graduación.<br>5. Se invoca el caso de uso «Cargar CV del Tutor» (CU1-6) y se solicita la fotografía de perfil obligatoria.<br>6. El sistema valida la unicidad del correo y del número de identificación del tutor.<br>7. El sistema valida que la contraseña tenga mínimo 8 caracteres e incluya al menos una minúscula, una mayúscula y un número.<br>8. El sistema crea la cuenta en estado «Pendiente de aprobación» y almacena la contraseña encriptada.<br>9. El sistema envía el token de verificación al correo del tutor (CU1-7).<br>10. El sistema confirma el registro e informa que la cuenta queda pendiente de aprobación del administrador. |
| **Excepciones / Cursos alternativos** | Paso 3: Si falta algún campo obligatorio, el sistema señala los campos requeridos y no continúa.<br>Paso 5: Si no se carga la fotografía, el sistema impide el registro.<br>Paso 6: Si el correo o el número de identificación ya existen, el sistema rechaza el registro e indica el campo duplicado.<br>Paso 7: Si la contraseña no cumple los requisitos, el sistema muestra los criterios faltantes. |
| **Condiciones posteriores** | Existe una cuenta de tutor en estado «Pendiente de aprobación», con su CV y fotografía asociados, visible para el administrador en CU4-2. |
| **Comentarios** | HU-02 · RF-1-2-1 a RF-1-2-7 · RR-3, RR-9 · EAC-9. Relación: «include» CU1-6. |

<br>

### CU1-3 Iniciar Sesión (Estudiantes y Tutores)

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Iniciar Sesión (Estudiantes y Tutores) |
| **Actores** | Estudiante, Tutor |
| **Propósito** | Permitir que un usuario aprobado acceda a su panel con su correo y contraseña. |
| **Resumen** | El usuario ingresa su correo y contraseña. El sistema valida las credenciales y que la cuenta haya sido aprobada por el administrador. En el primer inicio de sesión se extiende con la validación del token (CU1-7). Si todo es correcto, redirige al panel del rol. |
| **Condiciones previas** | La cuenta debe existir y haber sido aprobada por el administrador. |
| **Secuencia normal** | 1. El usuario accede a la pantalla de inicio de sesión, donde se muestra un enlace visible para registrarse.<br>2. El usuario ingresa su correo electrónico y contraseña.<br>3. El sistema valida las credenciales.<br>4. El sistema verifica que la cuenta esté aprobada y activa.<br>5. Si es el primer inicio de sesión del usuario, se ejecuta «Validar Correo Electrónico con Token» (CU1-7).<br>6. El sistema redirige al usuario al panel correspondiente a su rol (estudiante o tutor). |
| **Excepciones / Cursos alternativos** | Paso 3: Si las credenciales son incorrectas, el sistema muestra un mensaje con la información del problema.<br>Paso 4: Si la cuenta está pendiente, rechazada o dada de baja, el sistema impide el acceso e informa el motivo.<br>Paso 5: Si el token es incorrecto, el sistema impide el primer inicio de sesión (ver CU1-7). |
| **Condiciones posteriores** | El usuario queda autenticado y se encuentra en el panel de su rol. |
| **Comentarios** | HU-03 · RF-1-3-1 a RF-1-3-5 · RR-9 · EAC-5. Relación: CU1-7 «extend» CU1-3 en el primer inicio de sesión. |

<br>

### CU1-4 Iniciar Sesión de Administrador (2FA)

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Iniciar Sesión de Administrador (2FA) |
| **Actores** | Administrador (iniciador) |
| **Propósito** | Permitir que el administrador acceda al panel de administración mediante doble autenticación. |
| **Resumen** | El administrador ingresa con el usuario y la contraseña predeterminados. Luego sube el archivo auth2-ayd1.txt, que contiene una contraseña encriptada distinta. Si el sistema la valida, concede acceso a la página principal. |
| **Condiciones previas** | Debe existir la cuenta de administrador predeterminada. |
| **Secuencia normal** | 1. El administrador ingresa el usuario y la contraseña predeterminados.<br>2. El sistema valida las credenciales del primer paso.<br>3. El sistema redirige al administrador a la página de carga del archivo auth2-ayd1.txt.<br>4. El administrador sube el archivo auth2-ayd1.txt.<br>5. El sistema valida la contraseña encriptada contenida en el archivo.<br>6. El sistema concede acceso a la página principal del administrador. |
| **Excepciones / Cursos alternativos** | Paso 2: Si las credenciales del primer paso son incorrectas, el sistema muestra un mensaje de error y no avanza.<br>Paso 5: Si el archivo no contiene la contraseña correcta, el sistema deniega el acceso y el administrador permanece en la página de carga. |
| **Condiciones posteriores** | El administrador queda autenticado y accede a su panel. |
| **Comentarios** | HU-04 · RF-1-4-1 a RF-1-4-4 · RR-3, RR-7, RR-8 · EAC-6. |

<br>

### CU1-5 Cargar Fotografía y Carnet del Estudiante

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Cargar Fotografía y Carnet del Estudiante |
| **Actores** | Estudiante (a través de CU1-1) |
| **Propósito** | Obtener la fotografía personal y el carnet escaneado del estudiante para que el administrador valide su identidad. |
| **Resumen** | Durante el registro, el sistema solicita una fotografía personal reciente y el carnet escaneado en PDF. Ambos son obligatorios, el carnet solo acepta PDF y el estudiante puede reemplazar cualquiera de los dos antes de enviar el registro. |
| **Condiciones previas** | El estudiante debe estar completando el registro (CU1-1). |
| **Secuencia normal** | 1. El sistema solicita la fotografía personal reciente.<br>2. El estudiante selecciona la fotografía.<br>3. El sistema solicita el carnet escaneado en formato PDF.<br>4. El estudiante selecciona el archivo PDF.<br>5. El sistema valida que el carnet sea un archivo PDF.<br>6. El estudiante puede reemplazar la fotografía o el PDF si seleccionó el archivo incorrecto.<br>7. El sistema confirma que ambos archivos quedaron adjuntos al registro. |
| **Excepciones / Cursos alternativos** | Paso 2: Si no se sube la fotografía, el sistema impide el registro.<br>Paso 4: Si no se carga el PDF del carnet, el sistema impide el registro.<br>Paso 5: Si el archivo no es PDF, el sistema lo rechaza por tipo o extensión. |
| **Condiciones posteriores** | La fotografía y el carnet en PDF quedan asociados al registro del estudiante. |
| **Comentarios** | HU-22 · RF-1-5-1 a RF-1-5-4 · RR-1 · EAC-9. Incluido por CU1-1. |

<br>

### CU1-6 Cargar CV del Tutor

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Cargar CV del Tutor |
| **Actores** | Tutor (a través de CU1-2) |
| **Propósito** | Obtener el CV actualizado del tutor como documento de respaldo para su aprobación. |
| **Resumen** | Durante el registro, el sistema solicita el CV en PDF. Es obligatorio, solo acepta PDF y el tutor puede reemplazarlo antes de enviar el registro. |
| **Condiciones previas** | El tutor debe estar completando el registro (CU1-2). |
| **Secuencia normal** | 1. El sistema solicita el CV del tutor en formato PDF.<br>2. El tutor selecciona el archivo.<br>3. El sistema valida que el archivo sea PDF.<br>4. El tutor puede reemplazar el archivo si seleccionó el incorrecto.<br>5. El sistema confirma que el CV quedó adjunto al registro. |
| **Excepciones / Cursos alternativos** | Paso 2: Si no se carga el CV, el sistema impide el registro.<br>Paso 3: Si el archivo no es PDF, el sistema lo rechaza por extensión. |
| **Condiciones posteriores** | El CV en PDF queda asociado al registro del tutor. |
| **Comentarios** | HU-23 · RF-1-6-1 a RF-1-6-3 · RR-1 · EAC-9. Incluido por CU1-2. |

<br>

### CU1-7 Validar Correo Electrónico con Token

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Validar Correo Electrónico con Token |
| **Actores** | Estudiante, Tutor |
| **Propósito** | Verificar que el correo del usuario es auténtico mediante un token de 6 dígitos. |
| **Resumen** | Al finalizar el registro el sistema envía un token al correo. El usuario lo ingresa en su primer inicio de sesión. Si es correcto, el correo se marca como válido y el token no se vuelve a solicitar. |
| **Condiciones previas** | El usuario completó su registro y su cuenta fue aprobada por el administrador. |
| **Secuencia normal** | 1. Al finalizar el registro, el sistema envía al correo un mensaje con el logo de la plataforma, un mensaje de bienvenida y el token de 6 dígitos.<br>2. En el primer inicio de sesión, el sistema solicita el token de verificación.<br>3. El usuario ingresa el token.<br>4. El sistema valida el token.<br>5. El sistema marca el correo del usuario como válido.<br>6. El sistema permite continuar el inicio de sesión. En los inicios de sesión posteriores ya no solicita el token. |
| **Excepciones / Cursos alternativos** | Paso 3: Si el usuario no recibió o perdió el token, solicita el reenvío y el sistema envía un nuevo token.<br>Paso 4: Si el token es incorrecto, el sistema impide el primer inicio de sesión y permite reintentar. |
| **Condiciones posteriores** | El correo del usuario queda marcado como válido y no se solicita más el token. |
| **Comentarios** | HU-24 · RF-1-7-1 a RF-1-7-6 · RR-5, RR-6 · EAC-7. Relación: «extend» CU1-3. |

<br>

---

## **CU2 GESTIONAR TUTORÍAS DEL ESTUDIANTE**

### CU2-1 Explorar y Buscar Tutores

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Explorar y Buscar Tutores |
| **Actores** | Estudiante (iniciador) |
| **Propósito** | Permitir al estudiante encontrar el tutor que se adapte a sus necesidades. |
| **Resumen** | El estudiante ve la lista de tutores aprobados y aplica filtros avanzados por materia, experiencia, sexo, edad y universidad. El sistema muestra solo los tutores que cumplen todos los filtros. |
| **Condiciones previas** | El estudiante debe estar autenticado. |
| **Secuencia normal** | 1. El estudiante accede a su página principal.<br>2. El sistema muestra los tutores aprobados, excluyendo aquellos con los que ya tiene una sesión, con nombre completo, especialidad, dirección de tutoría y fotografía.<br>3. El estudiante abre la búsqueda avanzada.<br>4. El estudiante selecciona los filtros: materia, años de experiencia, sexo, edad y universidad.<br>5. El sistema muestra únicamente los tutores que cumplen todos los filtros seleccionados.<br>6. El estudiante selecciona un tutor para consultar sus horarios (CU2-2). |
| **Excepciones / Cursos alternativos** | Paso 2: Si no hay tutores disponibles, el sistema informa que no hay tutores para mostrar.<br>Paso 5: Si ningún tutor cumple los filtros, el sistema informa «No se encontraron tutores con los filtros seleccionados». |
| **Condiciones posteriores** | El estudiante visualiza la lista de tutores que cumple sus criterios. |
| **Comentarios** | HU-16 · RF-2-1-1 a RF-2-1-4 · EAC-1, EAC-10. Extendido por CU2-2. |

<br>

### CU2-2 Consultar Horarios y Disponibilidad

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Consultar Horarios y Disponibilidad |
| **Actores** | Estudiante (iniciador) |
| **Propósito** | Mostrar al estudiante los momentos en que un tutor está disponible para una sesión. |
| **Resumen** | El estudiante selecciona un tutor y el sistema muestra sus días y horarios de atención. El estudiante filtra por fecha y ve diferenciados los horarios ocupados y disponibles. |
| **Condiciones previas** | El estudiante debe estar autenticado y haber seleccionado un tutor. |
| **Secuencia normal** | 1. El estudiante selecciona un tutor.<br>2. El sistema muestra los días y horarios en que el tutor atiende.<br>3. El estudiante filtra por una fecha.<br>4. El sistema muestra, de forma diferenciada, los horarios ocupados y los disponibles de esa fecha. |
| **Excepciones / Cursos alternativos** | Paso 4: Si el tutor no atiende en la fecha seleccionada, el sistema lo indica claramente. |
| **Condiciones posteriores** | El estudiante conoce los horarios disponibles del tutor para la fecha consultada. |
| **Comentarios** | HU-17 · RF-2-2-1 a RF-2-2-4. Relación: «extend» CU2-1; incluido por CU2-3. |

<br>

### CU2-3 Programar Sesión de Tutoría

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Programar Sesión de Tutoría |
| **Actores** | Estudiante (iniciador) |
| **Propósito** | Permitir al estudiante reservar una sesión con un tutor en un horario disponible. |
| **Resumen** | El estudiante consulta la disponibilidad (CU2-2) y llena el formulario de agendamiento. El sistema valida horario, disponibilidad, que no exista otra sesión activa con el mismo tutor ni traslapes, y registra la sesión como «Pendiente». |
| **Condiciones previas** | El estudiante debe estar autenticado y el tutor debe tener horario de atención establecido. |
| **Secuencia normal** | 1. Se invoca «Consultar Horarios y Disponibilidad» (CU2-2).<br>2. El estudiante selecciona un espacio disponible.<br>3. El sistema muestra el formulario de agendamiento.<br>4. El estudiante ingresa fecha, hora, materia (de las impartidas por el tutor) y motivo de la sesión.<br>5. El sistema valida que la fecha y hora estén dentro del horario del tutor y que el espacio esté disponible.<br>6. El sistema valida que el estudiante no tenga otra sesión activa con el mismo tutor.<br>7. El sistema valida que la sesión no se traslape con otras sesiones del estudiante en el mismo día.<br>8. El sistema registra la sesión en estado «Pendiente» y marca el horario como ocupado.<br>9. El sistema confirma el agendamiento. |
| **Excepciones / Cursos alternativos** | Paso 5: Si la fecha u hora están fuera del horario del tutor o el espacio está ocupado, el sistema notifica el motivo y no agenda.<br>Paso 6: Si ya existe una sesión activa con ese tutor, el sistema notifica el motivo y no agenda.<br>Paso 7: Si hay traslape con otra sesión del mismo día, el sistema notifica el motivo y no agenda. |
| **Condiciones posteriores** | La sesión queda registrada en estado «Pendiente» y el horario queda ocupado en la agenda del tutor. |
| **Comentarios** | HU-18 · RF-2-3-1 a RF-2-3-7 · RR-11 · EAC-2. Relación: «include» CU2-2. |

<br>

### CU2-4 Gestionar Sesiones Activas

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Gestionar Sesiones Activas |
| **Actores** | Estudiante (iniciador) |
| **Propósito** | Permitir al estudiante ver sus próximas sesiones y cancelarlas si no puede asistir. |
| **Resumen** | El estudiante consulta la lista de sesiones activas y puede cancelar una. Tras confirmar, la sesión cambia a «Cancelada por el estudiante» y el horario se libera de inmediato. |
| **Condiciones previas** | El estudiante debe estar autenticado y tener al menos una sesión activa. |
| **Secuencia normal** | 1. El estudiante accede a «Sesiones Activas».<br>2. El sistema muestra fecha, hora, tutor, materia, dirección y motivo de cada sesión.<br>3. El estudiante selecciona «Cancelar» en una sesión.<br>4. El sistema solicita un mensaje de confirmación.<br>5. El estudiante confirma la cancelación.<br>6. El sistema cambia el estado a «Cancelada por el estudiante» y retira la sesión de la lista.<br>7. El sistema libera el horario en la agenda del tutor. |
| **Excepciones / Cursos alternativos** | Paso 2: Si no hay sesiones activas, el sistema informa que no existen sesiones próximas.<br>Paso 5: Si el estudiante no confirma, la sesión se mantiene sin cambios. |
| **Condiciones posteriores** | La sesión queda cancelada y el horario disponible para otros estudiantes. |
| **Comentarios** | HU-19 · RF-2-4-1 a RF-2-4-4 · RR-11 · EAC-8. |

<br>

### CU2-5 Visualizar Historial de Sesiones (Estudiante)

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Visualizar Historial de Sesiones (Estudiante) |
| **Actores** | Estudiante (iniciador) |
| **Propósito** | Permitir al estudiante llevar el control de sus sesiones pasadas o canceladas. |
| **Resumen** | El sistema muestra las sesiones atendidas y canceladas del estudiante con los datos de cada una. |
| **Condiciones previas** | El estudiante debe estar autenticado. |
| **Secuencia normal** | 1. El estudiante accede a su historial de sesiones.<br>2. El sistema muestra las sesiones atendidas y canceladas.<br>3. El sistema presenta por cada sesión fecha, tutor, materia, motivo, dirección, estado y resumen de la sesión (si fue atendida). |
| **Excepciones / Cursos alternativos** | Paso 2: Si el estudiante no tiene sesiones atendidas ni canceladas, el sistema informa que el historial está vacío. |
| **Condiciones posteriores** | El estudiante conoce el detalle de sus sesiones pasadas. |
| **Comentarios** | HU-20 · RF-2-5-1, RF-2-5-2 · RR-11. |

<br>

### CU2-6 Gestionar Perfil del Estudiante

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Gestionar Perfil del Estudiante |
| **Actores** | Estudiante (iniciador) |
| **Propósito** | Permitir al estudiante ver y mantener actualizados sus datos personales. |
| **Resumen** | El estudiante consulta su perfil y modifica cualquier campo excepto el correo. Para cambiar la contraseña debe ingresar la actual y la nueva debe cumplir los requisitos de seguridad. |
| **Condiciones previas** | El estudiante debe estar autenticado. |
| **Secuencia normal** | 1. El estudiante accede a «Mi perfil».<br>2. El sistema muestra todos los datos registrados.<br>3. El estudiante modifica los campos deseados, salvo el correo electrónico.<br>4. Si desea cambiar la contraseña, el estudiante ingresa la contraseña original.<br>5. El sistema valida la contraseña original.<br>6. El sistema valida que la nueva contraseña tenga mínimo 8 caracteres e incluya una minúscula, una mayúscula y un número.<br>7. El sistema guarda los cambios, almacenando la contraseña encriptada.<br>8. El sistema confirma la actualización. |
| **Excepciones / Cursos alternativos** | Paso 5: Si la contraseña original es incorrecta, el sistema niega el cambio de contraseña.<br>Paso 6: Si la nueva contraseña no cumple los requisitos, el sistema muestra los criterios faltantes. |
| **Condiciones posteriores** | Los datos del estudiante quedan actualizados. |
| **Comentarios** | HU-21 · RF-2-6-1 a RF-2-6-4 · RR-3, RR-9. |

<br>

### CU2-7 Consultar Plan de Estudio

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Consultar Plan de Estudio |
| **Actores** | Estudiante (iniciador) |
| **Propósito** | Dar al estudiante una guía de los temas que debe reforzar y las áreas donde presentó dificultades. |
| **Resumen** | El sistema muestra el plan de estudio generado por el tutor en la última sesión atendida: datos del tutor, dificultades identificadas y recursos recomendados. |
| **Condiciones previas** | El estudiante debe estar autenticado y tener al menos una sesión atendida con plan de estudio (CU3-7). |
| **Secuencia normal** | 1. El estudiante accede a su plan de estudio.<br>2. El sistema muestra la fecha de la última sesión con nombre, identificación y especialidad del tutor.<br>3. El sistema muestra las dificultades identificadas.<br>4. El sistema muestra los recursos recomendados con nombre, tipo y descripción de uso. |
| **Excepciones / Cursos alternativos** | Paso 1: Si el estudiante no tiene sesiones atendidas, el sistema informa que aún no cuenta con un plan de estudio. |
| **Condiciones posteriores** | El estudiante conoce las áreas a reforzar y los recursos recomendados. |
| **Comentarios** | HU-25 · RF-2-7-1 a RF-2-7-3. Extendido por CU2-8. |

<br>

### CU2-8 Imprimir Plan de Estudio

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Imprimir Plan de Estudio |
| **Actores** | Estudiante (iniciador) |
| **Propósito** | Permitir al estudiante guardar evidencia de sus tutorías en un archivo PDF. |
| **Resumen** | Desde su plan de estudio, el estudiante solicita un PDF con encabezado, tabla de recursos y pie con los datos del tutor. |
| **Condiciones previas** | El estudiante debe estar consultando un plan de estudio (CU2-7). |
| **Secuencia normal** | 1. El estudiante solicita generar el PDF desde su plan de estudio.<br>2. El sistema genera el PDF con encabezado: nombre de la plataforma, fecha de emisión y teléfono de contacto.<br>3. El sistema incluye en el cuerpo una tabla con los recursos recomendados (nombre, tipo y descripción de uso).<br>4. El sistema incluye en el pie el nombre del tutor, su especialidad y su número de identificación.<br>5. El sistema entrega el PDF al estudiante. |
| **Excepciones / Cursos alternativos** | Paso 2: Si ocurre un error al generar el PDF, el sistema notifica al estudiante y permite reintentar. |
| **Condiciones posteriores** | El estudiante dispone de un PDF de su plan de estudio. |
| **Comentarios** | HU-26 · RF-2-8-1 a RF-2-8-4 · EAC-4. Relación: «extend» CU2-7. |

<br>

### CU2-9 Calificar Atención de Tutoría

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Calificar Atención de Tutoría |
| **Actores** | Estudiante (iniciador) |
| **Propósito** | Permitir al estudiante valorar la atención recibida en una tutoría. |
| **Resumen** | El estudiante selecciona una sesión atendida y la califica de 0 a 5 estrellas, con un comentario opcional. |
| **Condiciones previas** | La sesión debe estar en estado «Atendido». |
| **Secuencia normal** | 1. El estudiante selecciona una sesión atendida de su historial.<br>2. El sistema muestra el formulario de calificación.<br>3. El estudiante selecciona una calificación de 0 a 5 estrellas.<br>4. El estudiante puede agregar un comentario con el motivo de la calificación.<br>5. El estudiante confirma la calificación.<br>6. El sistema registra la calificación y confirma. |
| **Excepciones / Cursos alternativos** | Paso 1: Si la sesión está cancelada o pendiente, el sistema no permite calificarla.<br>Paso 3: Si no se selecciona calificación, el sistema no permite enviar el formulario. |
| **Condiciones posteriores** | La calificación queda asociada a la sesión y al tutor. |
| **Comentarios** | HU-27 · RF-2-9-1 a RF-2-9-3 · RR-10, RR-11. |

<br>

### CU2-10 Reportar Tutor

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Reportar Tutor |
| **Actores** | Estudiante (iniciador), Administrador (receptor) |
| **Propósito** | Permitir al estudiante reportar a un tutor tras haber sido atendido. |
| **Resumen** | El estudiante selecciona una sesión atendida, elige una categoría y explica el motivo. El sistema envía el reporte al administrador. |
| **Condiciones previas** | La sesión debe estar en estado «Atendido». |
| **Secuencia normal** | 1. El estudiante selecciona una sesión atendida y elige «Reportar tutor».<br>2. El sistema muestra una lista predefinida de categorías (por ejemplo, Ética y Negligencia académica) y un campo de explicación.<br>3. El estudiante selecciona la categoría e ingresa la explicación del motivo.<br>4. El estudiante confirma el reporte.<br>5. El sistema registra el reporte y lo envía al administrador.<br>6. El sistema confirma el envío. |
| **Excepciones / Cursos alternativos** | Paso 1: Si la sesión está cancelada o pendiente, el sistema no permite el reporte.<br>Paso 4: Si falta la categoría o la explicación, el sistema señala los campos requeridos. |
| **Condiciones posteriores** | El reporte queda disponible para el administrador en CU4-10. |
| **Comentarios** | HU-28 · RF-2-10-1 a RF-2-10-5 · RR-11, RR-13. |

<br>

---

## **CU3 GESTIONAR TUTORÍAS DEL TUTOR**

### CU3-1 Establecer Horarios de Atención

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Establecer Horarios de Atención |
| **Actores** | Tutor (iniciador) |
| **Propósito** | Permitir al tutor configurar sus días y horas de disponibilidad para que los estudiantes agenden sesiones. |
| **Resumen** | El tutor selecciona los días de la semana en que atiende y un único rango horario que se aplica a todos los días seleccionados. El sistema publica la disponibilidad. |
| **Condiciones previas** | El tutor debe estar autenticado. |
| **Secuencia normal** | 1. El tutor accede a «Horario de atención».<br>2. El tutor selecciona los días de la semana en que atenderá.<br>3. El tutor establece un rango de horario (por ejemplo, de 8 am a 5 pm) que se aplica a todos los días seleccionados.<br>4. El tutor confirma.<br>5. El sistema guarda el horario y lo publica para consulta de los estudiantes. |
| **Excepciones / Cursos alternativos** | Paso 2: Si no se selecciona ningún día, el sistema señala que debe elegir al menos uno.<br>Paso 3: Si la hora de fin no es posterior a la hora de inicio, el sistema rechaza el rango. |
| **Condiciones posteriores** | El tutor queda con horario de atención y es agendable por los estudiantes. |
| **Comentarios** | HU-10 · RF-3-1-1 a RF-3-1-3 · RR-12. |

<br>

### CU3-2 Actualizar Horarios de Atención

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Actualizar Horarios de Atención |
| **Actores** | Tutor (iniciador) |
| **Propósito** | Permitir al tutor adaptar sus días y horas de atención a su disponibilidad actual. |
| **Resumen** | El tutor modifica sus días y su rango horario. El sistema impide el cambio si existen sesiones activas fuera del nuevo horario. |
| **Condiciones previas** | El tutor debe tener un horario establecido (CU3-1). |
| **Secuencia normal** | 1. El tutor accede a «Horario de atención».<br>2. El tutor modifica los días y el rango horario.<br>3. El tutor confirma los cambios.<br>4. El sistema valida que no existan sesiones activas fuera del nuevo horario.<br>5. El sistema guarda el nuevo horario y confirma la actualización. |
| **Excepciones / Cursos alternativos** | Paso 4: Si existen sesiones fuera del nuevo horario, el sistema informa del conflicto y no permite la actualización hasta que esas sesiones sean reprogramadas o canceladas (CU3-4). |
| **Condiciones posteriores** | El horario del tutor queda actualizado. |
| **Comentarios** | HU-11 · RF-3-2-1 a RF-3-2-3 · RR-12. |

<br>

### CU3-3 Gestionar Sesiones Pendientes

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Gestionar Sesiones Pendientes |
| **Actores** | Tutor (iniciador) |
| **Propósito** | Permitir al tutor ver sus sesiones pendientes para atenderlas o cancelarlas. |
| **Resumen** | El sistema lista las sesiones pendientes del tutor ordenadas por fecha más próxima. Desde cada sesión el tutor puede marcarla como atendida (CU3-7) o cancelarla (CU3-4). |
| **Condiciones previas** | El tutor debe estar autenticado. |
| **Secuencia normal** | 1. El tutor accede a «Sesiones pendientes».<br>2. El sistema muestra las sesiones ordenadas por fecha más próxima, con fecha, hora, nombre del estudiante, motivo y materia.<br>3. El tutor selecciona una sesión.<br>4. El tutor elige «Atendido» (CU3-7) o «Cancelar» (CU3-4). |
| **Excepciones / Cursos alternativos** | Paso 2: Si no hay sesiones pendientes, el sistema informa que no existen sesiones por atender. |
| **Condiciones posteriores** | El tutor visualiza sus sesiones pendientes actualizadas. |
| **Comentarios** | HU-12 · RF-3-3-1 a RF-3-3-4 · RR-11. Extendido por CU3-4 y CU3-7. |

<br>

### CU3-4 Cancelar Sesión (Tutor)

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Cancelar Sesión (Tutor) |
| **Actores** | Tutor (iniciador), Estudiante (receptor de la notificación) |
| **Propósito** | Permitir al tutor cancelar una sesión agendada cuando le surge un inconveniente y avisar al estudiante. |
| **Resumen** | El tutor cancela una sesión pendiente. El sistema libera el horario y envía un correo al estudiante con los datos de la sesión y un mensaje de disculpa. |
| **Condiciones previas** | La sesión debe estar en estado «Pendiente». |
| **Secuencia normal** | 1. Desde «Sesiones pendientes» (CU3-3), el tutor selecciona «Cancelar» en una sesión.<br>2. El sistema solicita confirmación.<br>3. El tutor confirma la cancelación.<br>4. El sistema cambia la sesión a «Cancelada por el tutor» y la retira de la lista.<br>5. El sistema libera de inmediato el horario en la agenda del tutor.<br>6. El sistema envía un correo al estudiante con fecha, hora, motivo de la sesión, nombre del tutor, materia y un mensaje de disculpa. |
| **Excepciones / Cursos alternativos** | Paso 3: Si el tutor no confirma, la sesión se mantiene sin cambios.<br>Paso 6: Si falla el envío del correo, el sistema registra el error y mantiene la cancelación. |
| **Condiciones posteriores** | La sesión queda cancelada, el horario disponible y el estudiante notificado. |
| **Comentarios** | HU-13 · RF-3-4-1 a RF-3-4-4 · RR-5, RR-11 · EAC-7, EAC-8. Relación: «extend» CU3-3. |

<br>

### CU3-5 Consultar Historial de Sesiones (Tutor)

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Consultar Historial de Sesiones (Tutor) |
| **Actores** | Tutor (iniciador) |
| **Propósito** | Permitir al tutor tener un registro de su actividad. |
| **Resumen** | El sistema muestra todas las sesiones del tutor y permite filtrarlas por fechas o por estudiante. |
| **Condiciones previas** | El tutor debe estar autenticado. |
| **Secuencia normal** | 1. El tutor accede a su historial desde su panel.<br>2. El sistema muestra todas las sesiones con fecha, hora, nombre del estudiante y estado.<br>3. El tutor filtra por rango de fechas o por estudiante.<br>4. El sistema muestra las sesiones que cumplen el filtro. |
| **Excepciones / Cursos alternativos** | Paso 4: Si ninguna sesión cumple el filtro, el sistema informa que no hay resultados. |
| **Condiciones posteriores** | El tutor visualiza el historial solicitado. |
| **Comentarios** | HU-14 · RF-3-5-1 a RF-3-5-3 · RR-11. |

<br>

### CU3-6 Gestionar Perfil del Tutor

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Gestionar Perfil del Tutor |
| **Actores** | Tutor (iniciador) |
| **Propósito** | Permitir al tutor ver y actualizar los datos de su perfil. |
| **Resumen** | El tutor consulta su perfil, modifica cualquier campo excepto el correo y puede cambiar su fotografía. Para cambiar la contraseña debe ingresar la actual. |
| **Condiciones previas** | El tutor debe estar autenticado. |
| **Secuencia normal** | 1. El tutor accede a «Mi perfil».<br>2. El sistema muestra todos los datos registrados.<br>3. El tutor modifica los campos deseados, salvo el correo electrónico.<br>4. El tutor puede subir una nueva fotografía de perfil.<br>5. Si desea cambiar la contraseña, el tutor ingresa la contraseña original.<br>6. El sistema valida la contraseña original contra la base de datos.<br>7. El sistema valida que la nueva contraseña tenga mínimo 8 caracteres e incluya una minúscula, una mayúscula y un número.<br>8. El sistema guarda los cambios, almacenando la contraseña encriptada, y confirma. |
| **Excepciones / Cursos alternativos** | Paso 6: Si la contraseña original es incorrecta, el sistema niega el cambio de contraseña.<br>Paso 7: Si la nueva contraseña no cumple los requisitos, el sistema muestra los criterios faltantes. |
| **Condiciones posteriores** | Los datos del tutor quedan actualizados. |
| **Comentarios** | HU-15 · RF-3-6-1 a RF-3-6-5 · RR-3, RR-9. |

<br>

### CU3-7 Atender Estudiante

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Atender Estudiante |
| **Actores** | Tutor (iniciador) |
| **Propósito** | Permitir al tutor marcar una sesión como atendida y registrar el plan de estudio del estudiante. |
| **Resumen** | El tutor presiona «Atendido» en una sesión pendiente, completa el plan de estudio con las dificultades y los recursos recomendados, y el sistema marca la sesión como atendida. |
| **Condiciones previas** | La sesión debe estar en estado «Pendiente». |
| **Secuencia normal** | 1. Desde «Sesiones pendientes» (CU3-3), el tutor presiona «Atendido» en una sesión.<br>2. El sistema despliega el formulario de plan de estudio.<br>3. El tutor ingresa las dificultades identificadas del estudiante.<br>4. El tutor agrega uno o más recursos recomendados, indicando nombre, tipo y descripción de uso de cada uno.<br>5. El tutor confirma el plan de estudio.<br>6. El sistema guarda el plan, marca la sesión como «Atendido» y la retira de la lista de pendientes. |
| **Excepciones / Cursos alternativos** | Paso 3: Si no se ingresan las dificultades identificadas, el sistema no permite continuar.<br>Paso 4: Si no se agrega al menos un recurso recomendado, el sistema no permite continuar.<br>Paso 5: Si el tutor cancela el formulario, la sesión permanece pendiente. |
| **Condiciones posteriores** | La sesión queda en estado «Atendido» y el plan de estudio disponible para el estudiante en CU2-7. |
| **Comentarios** | HU-29 · RF-3-7-1 a RF-3-7-4 · RR-11. Relación: «extend» CU3-3. |

<br>

### CU3-8 Calificar Estudiante

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Calificar Estudiante |
| **Actores** | Tutor (iniciador) |
| **Propósito** | Permitir al tutor calificar a los estudiantes que atendió. |
| **Resumen** | El tutor selecciona una sesión atendida y la califica de 0 a 5 estrellas con un comentario obligatorio. |
| **Condiciones previas** | La sesión debe estar en estado «Atendido». |
| **Secuencia normal** | 1. El tutor selecciona una sesión atendida.<br>2. El sistema muestra el formulario de calificación.<br>3. El tutor selecciona una calificación de 0 a 5 estrellas.<br>4. El tutor ingresa el comentario con el motivo de la calificación.<br>5. El tutor confirma.<br>6. El sistema registra la calificación y confirma. |
| **Excepciones / Cursos alternativos** | Paso 1: Si la sesión está cancelada o pendiente, el sistema no permite calificarla.<br>Paso 3: Si no se selecciona calificación, el sistema no permite enviar el formulario.<br>Paso 4: Si no se ingresa el comentario, el sistema no permite enviar el formulario. |
| **Condiciones posteriores** | La calificación queda asociada a la sesión y al estudiante. |
| **Comentarios** | HU-30 · RF-3-8-1 a RF-3-8-3 · RR-10, RR-11. |

<br>

### CU3-9 Reportar Estudiante

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Reportar Estudiante |
| **Actores** | Tutor (iniciador), Administrador (receptor) |
| **Propósito** | Permitir al tutor reportar a un estudiante problemático al que atendió. |
| **Resumen** | El tutor selecciona una sesión atendida, elige una categoría y describe el reporte. El sistema lo envía al administrador. |
| **Condiciones previas** | La sesión debe estar en estado «Atendido». |
| **Secuencia normal** | 1. El tutor selecciona una sesión atendida y elige «Reportar estudiante».<br>2. El sistema muestra una lista predefinida de categorías y un campo de descripción detallada.<br>3. El tutor selecciona la categoría e ingresa la descripción.<br>4. El tutor envía el formulario.<br>5. El sistema registra el reporte y lo envía al administrador.<br>6. El sistema confirma el envío. |
| **Excepciones / Cursos alternativos** | Paso 1: Si la sesión está cancelada o pendiente, el sistema no permite el reporte.<br>Paso 4: Si falta la categoría o la descripción, el sistema señala los campos requeridos. |
| **Condiciones posteriores** | El reporte queda disponible para el administrador en CU4-11. |
| **Comentarios** | HU-31 · RF-3-9-1 a RF-3-9-4 · RR-11, RR-13. |

<br>

---

## **CU4 ADMINISTRAR LA PLATAFORMA**

### CU4-1 Aprobar Registro de Estudiantes

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Aprobar Registro de Estudiantes |
| **Actores** | Administrador (iniciador), Estudiante (receptor de la notificación) |
| **Propósito** | Permitir al administrador aceptar o rechazar las solicitudes de ingreso de los estudiantes. |
| **Resumen** | El administrador revisa la lista de estudiantes pendientes y acepta o rechaza cada solicitud. El sistema notifica el resultado por correo. |
| **Condiciones previas** | Deben existir estudiantes en estado «Pendiente de aprobación». |
| **Secuencia normal** | 1. El administrador accede a «Estudiantes pendientes».<br>2. El sistema muestra fotografía (o una por defecto), nombre completo, carnet, género, fecha de nacimiento y correo de cada estudiante.<br>3. El administrador selecciona «Aceptar» o «Rechazar» en un registro.<br>4. El sistema actualiza el estado de la cuenta.<br>5. El sistema envía un correo al estudiante informando si fue aprobado o rechazado. |
| **Excepciones / Cursos alternativos** | Paso 2: Si no hay estudiantes pendientes, el sistema informa que la lista está vacía.<br>Paso 5: Si falla el envío del correo, el sistema registra el error y mantiene la decisión. |
| **Condiciones posteriores** | La cuenta queda aprobada o rechazada y el estudiante notificado. |
| **Comentarios** | HU-05 · RF-4-1-1 a RF-4-1-4 · RR-5 · EAC-7. Extendido por CU4-6. |

<br>

### CU4-2 Aprobar Registro de Tutores

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Aprobar Registro de Tutores |
| **Actores** | Administrador (iniciador), Tutor (receptor de la notificación) |
| **Propósito** | Permitir al administrador verificar los perfiles de los tutores y aceptar o rechazar su ingreso. |
| **Resumen** | El administrador revisa la lista de tutores pendientes y acepta o rechaza cada uno. El sistema notifica el resultado por correo. |
| **Condiciones previas** | Deben existir tutores en estado «Pendiente de aprobación». |
| **Secuencia normal** | 1. El administrador accede a «Tutores pendientes».<br>2. El sistema muestra fotografía, nombre completo, carnet, género, especialidad, número de identificación y correo de cada tutor.<br>3. El administrador selecciona «Aceptar» o «Rechazar» en un registro.<br>4. El sistema actualiza el estado de la cuenta.<br>5. El sistema envía un correo al tutor notificando si fue aprobado o rechazado. |
| **Excepciones / Cursos alternativos** | Paso 2: Si no hay tutores pendientes, el sistema informa que la lista está vacía.<br>Paso 5: Si falla el envío del correo, el sistema registra el error y mantiene la decisión. |
| **Condiciones posteriores** | La cuenta queda aprobada o rechazada y el tutor notificado. |
| **Comentarios** | HU-06 · RF-4-2-1 a RF-4-2-4 · RR-5 · EAC-7. Extendido por CU4-7. |

<br>

### CU4-3 Gestionar Usuarios Activos

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Gestionar Usuarios Activos |
| **Actores** | Administrador (iniciador), Estudiante y Tutor (receptores de la notificación) |
| **Propósito** | Permitir al administrador ver los estudiantes y tutores activos y darlos de baja cuando sea necesario. |
| **Resumen** | El administrador consulta las vistas separadas de estudiantes y tutores aceptados y da de baja a un usuario indicando el motivo. El sistema lo pasa a inactivo, bloquea su acceso y le envía un correo. |
| **Condiciones previas** | El administrador debe estar autenticado y existir usuarios aprobados. |
| **Secuencia normal** | 1. El administrador elige la vista de estudiantes activos o la de tutores activos.<br>2. El sistema muestra los usuarios aceptados.<br>3. El administrador selecciona «Dar de baja» en un usuario.<br>4. El sistema solicita el motivo de la baja.<br>5. El administrador ingresa el motivo.<br>6. El sistema registra el motivo y la fecha de baja, y cambia el estado del usuario a inactivo.<br>7. El sistema envía un correo al usuario informando la baja de su cuenta.<br>8. El sistema impide que el usuario vuelva a iniciar sesión. |
| **Excepciones / Cursos alternativos** | Paso 5: Si no se ingresa el motivo, el sistema no permite completar la baja.<br>Paso 7: Si falla el envío del correo, el sistema registra el error y mantiene la baja. |
| **Condiciones posteriores** | El usuario queda inactivo, sin acceso, y aparece en la lista de CU4-5. |
| **Comentarios** | HU-07 · RF-4-3-1 a RF-4-3-5 · RR-5 · EAC-5, EAC-7. Extendido por CU4-8 y CU4-9. |

<br>

### CU4-4 Generar Reportes Operativos

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Generar Reportes Operativos |
| **Actores** | Administrador (iniciador) |
| **Propósito** | Proveer reportes del uso de la plataforma para tomar decisiones estratégicas. |
| **Resumen** | El administrador genera el reporte de tutores que más estudiantes han atendido o el de materias con mayor demanda. El sistema los presenta con tabla y gráfico, con datos vigentes. |
| **Condiciones previas** | El administrador debe estar autenticado. |
| **Secuencia normal** | 1. El administrador accede a «Reportes».<br>2. El administrador elige el reporte: «Tutores con más estudiantes atendidos» o «Materias con mayor demanda».<br>3. El sistema calcula los datos con la información vigente al momento de la consulta.<br>4. El sistema presenta el reporte con una tabla y un gráfico de barras o circular. |
| **Excepciones / Cursos alternativos** | Paso 3: Si no existen datos suficientes, el sistema informa que no hay información para generar el reporte. |
| **Condiciones posteriores** | El administrador visualiza el reporte solicitado. |
| **Comentarios** | HU-08 · RF-4-4-1 a RF-4-4-4 · RR-14 · EAC-3, EAC-11. |

<br>

### CU4-5 Visualizar Usuarios Dados de Baja

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Visualizar Usuarios Dados de Baja |
| **Actores** | Administrador (iniciador) |
| **Propósito** | Permitir al administrador consultar quiénes han sido dados de baja y por qué. |
| **Resumen** | El sistema lista los estudiantes y tutores dados de baja con su nombre, correo, fecha de baja y motivo. |
| **Condiciones previas** | El administrador debe estar autenticado. |
| **Secuencia normal** | 1. El administrador accede a «Usuarios dados de baja».<br>2. El administrador elige estudiantes o tutores.<br>3. El sistema muestra nombre, correo electrónico, fecha de baja y motivo de cada usuario. |
| **Excepciones / Cursos alternativos** | Paso 3: Si no hay usuarios dados de baja, el sistema informa que la lista está vacía. |
| **Condiciones posteriores** | El administrador conoce los usuarios dados de baja. |
| **Comentarios** | HU-09 · RF-4-5-1 a RF-4-5-3. |

<br>

### CU4-6 Aprobar Estudiantes con Visor PDF Incrustado

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Aprobar Estudiantes con Visor PDF Incrustado |
| **Actores** | Administrador (iniciador), Estudiante (receptor de la notificación) |
| **Propósito** | Permitir al administrador validar la identidad del estudiante viendo su carnet sin descargarlo. |
| **Resumen** | Dentro de la aprobación de estudiantes, el administrador abre el carnet en un visor PDF embebido en el navegador y decide aceptar o rechazar. |
| **Condiciones previas** | Debe existir un estudiante pendiente con carnet PDF cargado. |
| **Secuencia normal** | 1. Desde «Estudiantes pendientes» (CU4-1), el administrador selecciona ver el carnet de un estudiante.<br>2. El sistema muestra el PDF en un visor embebido, sin forzar la descarga.<br>3. El administrador valida la identidad comparando el carnet con los datos del registro.<br>4. El administrador selecciona «Aceptar» o «Rechazar».<br>5. Si acepta, el sistema pasa la cuenta a estado activo, lo que permite al estudiante autenticarse con sus credenciales y el token. Si rechaza, revoca el acceso.<br>6. El sistema envía un correo al estudiante con el resultado. |
| **Excepciones / Cursos alternativos** | Paso 2: Si el PDF no puede mostrarse, el sistema informa el error y no permite decidir sin visualizar el documento. |
| **Condiciones posteriores** | La cuenta queda activa o rechazada y el estudiante notificado. |
| **Comentarios** | HU-32 · RF-4-6-1 a RF-4-6-3 · RR-1, RR-2, RR-5 · EAC-5, EAC-7. Relación: «extend» CU4-1. |

<br>

### CU4-7 Aprobar Tutores con Visor PDF Incrustado

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Aprobar Tutores con Visor PDF Incrustado |
| **Actores** | Administrador (iniciador), Tutor (receptor de la notificación) |
| **Propósito** | Permitir al administrador validar el perfil profesional del tutor consultando su CV sin descargarlo. |
| **Resumen** | Dentro de la aprobación de tutores, el administrador abre el CV en un visor PDF embebido y decide aceptar o rechazar. |
| **Condiciones previas** | Debe existir un tutor pendiente con CV PDF cargado. |
| **Secuencia normal** | 1. Desde «Tutores pendientes» (CU4-2), el administrador selecciona ver el CV de un tutor.<br>2. El sistema muestra el PDF en un visor embebido, sin descargas locales.<br>3. El administrador valida el perfil profesional.<br>4. El administrador selecciona «Aceptar» o «Rechazar».<br>5. Si acepta, el sistema habilita la cuenta para que el tutor complete su autenticación. Si rechaza, le deniega el ingreso.<br>6. El sistema envía un correo al tutor con el resultado. |
| **Excepciones / Cursos alternativos** | Paso 2: Si el PDF no puede mostrarse, el sistema informa el error y no permite decidir sin visualizar el documento. |
| **Condiciones posteriores** | La cuenta queda habilitada o rechazada y el tutor notificado. |
| **Comentarios** | HU-33 · RF-4-7-1 a RF-4-7-3 · RR-1, RR-2, RR-5 · EAC-5, EAC-7. Relación: «extend» CU4-2. |

<br>

### CU4-8 Actualizar y Dar de Baja Estudiantes

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Actualizar y Dar de Baja Estudiantes |
| **Actores** | Administrador (iniciador) |
| **Propósito** | Permitir al administrador mantener actualizados los registros de los estudiantes o deshabilitar sus cuentas. |
| **Resumen** | Desde la lista de estudiantes activos, el administrador edita cualquier campo del perfil, salvo el correo, o da de baja al estudiante con confirmación explícita. |
| **Condiciones previas** | Debe existir un estudiante activo. |
| **Secuencia normal** | 1. El administrador selecciona un estudiante de la lista de activos (CU4-3).<br>2. El sistema muestra el perfil con el correo electrónico bloqueado contra edición.<br>**Flujo Editar**<br>3. El administrador modifica los campos deseados (datos personales, carnet, teléfono, dirección, entre otros).<br>4. El administrador confirma los cambios.<br>5. El sistema guarda los cambios y confirma la actualización.<br>**Flujo Dar de baja**<br>6. El administrador selecciona «Dar de baja».<br>7. El sistema solicita una confirmación explícita.<br>8. El administrador confirma y se ejecuta la baja según CU4-3 (estado inactivo, correo de notificación y bloqueo de inicio de sesión). |
| **Excepciones / Cursos alternativos** | Paso 4: Si falta un dato obligatorio, el sistema señala el campo y no guarda.<br>Paso 8: Si el administrador no confirma la baja, la cuenta se mantiene activa. |
| **Condiciones posteriores** | El perfil queda actualizado o el estudiante queda inactivo y sin acceso. |
| **Comentarios** | HU-34 · RF-4-8-1 a RF-4-8-3 · RR-9 · EAC-5. Relación: «extend» CU4-3. |

<br>

### CU4-9 Actualizar y Dar de Baja Tutores

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Actualizar y Dar de Baja Tutores |
| **Actores** | Administrador (iniciador) |
| **Propósito** | Permitir al administrador editar la información de los tutores o darlos de baja ante faltas operativas. |
| **Resumen** | Desde la lista de tutores activos, el administrador edita cualquier campo, salvo el correo, o da de baja al tutor con confirmación previa. Un tutor dado de baja no puede iniciar sesión ni recibir reservas. |
| **Condiciones previas** | Debe existir un tutor activo. |
| **Secuencia normal** | 1. El administrador selecciona un tutor de la lista de activos (CU4-3), con su información personal y profesional detallada.<br>2. El sistema muestra el perfil con el correo electrónico en solo lectura.<br>**Flujo Editar**<br>3. El administrador modifica los campos deseados.<br>4. El administrador confirma los cambios.<br>5. El sistema guarda los cambios y confirma la actualización.<br>**Flujo Dar de baja**<br>6. El administrador selecciona «Dar de baja».<br>7. El sistema solicita un cuadro de confirmación.<br>8. El administrador confirma y se ejecuta la baja según CU4-3. El tutor queda sin inicio de sesión y sin nuevas reservas. |
| **Excepciones / Cursos alternativos** | Paso 4: Si falta un dato obligatorio, el sistema señala el campo y no guarda.<br>Paso 8: Si el administrador no confirma la baja, la cuenta se mantiene activa. |
| **Condiciones posteriores** | El perfil queda actualizado o el tutor queda inactivo, sin acceso y sin recibir reservas. |
| **Comentarios** | HU-35 · RF-4-9-1 a RF-4-9-4 · RR-9 · EAC-5. Relación: «extend» CU4-3. |

<br>

### CU4-10 Gestionar Reportes hacia Tutores

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Gestionar Reportes hacia Tutores |
| **Actores** | Administrador (iniciador) |
| **Propósito** | Permitir al administrador evaluar los reportes de estudiantes contra tutores y sancionar o desestimar el caso. |
| **Resumen** | El administrador revisa los reportes recibidos y decide dar de baja al tutor o rechazar la denuncia. |
| **Condiciones previas** | Deben existir reportes enviados por estudiantes (CU2-10). |
| **Secuencia normal** | 1. El administrador accede a «Reportes contra tutores».<br>2. El sistema muestra categoría, motivo, nombre del tutor denunciado, nombre del estudiante denunciante y fecha de emisión.<br>3. El administrador selecciona un reporte.<br>4. El administrador elige «Dar de baja al tutor» o «Rechazar denuncia».<br>5. Si da de baja, el sistema desactiva y suspende de inmediato la cuenta del tutor.<br>6. Si rechaza, el sistema cambia el reporte a «Desestimado» sin alterar el estado del tutor. |
| **Excepciones / Cursos alternativos** | Paso 2: Si no hay reportes, el sistema informa que la lista está vacía. |
| **Condiciones posteriores** | El reporte queda resuelto: tutor dado de baja o denuncia desestimada. |
| **Comentarios** | HU-36 · RF-4-10-1 a RF-4-10-4 · RR-13 · EAC-5. |

<br>

### CU4-11 Gestionar Reportes hacia Estudiantes

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Gestionar Reportes hacia Estudiantes |
| **Actores** | Administrador (iniciador) |
| **Propósito** | Permitir al administrador resolver las quejas de tutores sobre la conducta de estudiantes. |
| **Resumen** | El administrador revisa los reportes emitidos por tutores y decide dar de baja al estudiante o rechazar la denuncia. |
| **Condiciones previas** | Deben existir reportes enviados por tutores (CU3-9). |
| **Secuencia normal** | 1. El administrador accede a «Reportes contra estudiantes».<br>2. El sistema muestra categoría, motivo, nombre del estudiante involucrado, nombre del tutor denunciante y fecha del incidente.<br>3. El administrador selecciona un reporte.<br>4. El administrador elige «Dar de baja al estudiante» o «Rechazar denuncia».<br>5. Si da de baja, el sistema desactiva de inmediato la cuenta e impide el inicio de sesión.<br>6. Si rechaza, el sistema archiva el caso y el estudiante se mantiene activo. |
| **Excepciones / Cursos alternativos** | Paso 2: Si no hay reportes, el sistema informa que la lista está vacía. |
| **Condiciones posteriores** | El reporte queda resuelto: estudiante dado de baja o caso archivado. |
| **Comentarios** | HU-37 · RF-4-11-1 a RF-4-11-4 · RR-13 · EAC-5. |

<br>

### CU4-12 Consultar Reporte de Calificación de Tutores

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Consultar Reporte de Calificación de Tutores |
| **Actores** | Administrador (iniciador) |
| **Propósito** | Permitir al administrador monitorear la calidad del servicio docente. |
| **Resumen** | El sistema muestra el promedio de calificación de cada tutor activo y permite ordenar para identificar a los de mejor y menor rendimiento. |
| **Condiciones previas** | Deben existir tutores activos; las calificaciones provienen de CU2-9. |
| **Secuencia normal** | 1. El administrador accede al reporte de calificación de tutores.<br>2. El sistema incluye a todos los tutores activos.<br>3. El sistema calcula el promedio de cada tutor con todas las calificaciones de estudiantes en sesiones atendidas.<br>4. El sistema muestra nombre completo, especialidad y promedio en escala de 0 a 5 estrellas.<br>5. El administrador ordena por promedio de mayor a menor o de menor a mayor. |
| **Excepciones / Cursos alternativos** | Paso 3: Si un tutor no tiene calificaciones, el sistema lo muestra sin promedio. |
| **Condiciones posteriores** | El administrador identifica a los tutores con mejor y menor rendimiento. |
| **Comentarios** | HU-38 · RF-4-12-1 a RF-4-12-4 · RR-10, RR-14 · EAC-3, EAC-11. |

<br>

### CU4-13 Consultar Reporte de Calificación de Estudiantes

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Consultar Reporte de Calificación de Estudiantes |
| **Actores** | Administrador (iniciador) |
| **Propósito** | Permitir al administrador identificar patrones de conducta o estudiantes con desempeño sobresaliente. |
| **Resumen** | El sistema muestra el promedio de calificación de cada estudiante activo, calculado con las calificaciones de los tutores. |
| **Condiciones previas** | Deben existir estudiantes activos; las calificaciones provienen de CU3-8. |
| **Secuencia normal** | 1. El administrador accede al reporte de calificación de estudiantes.<br>2. El sistema incluye a todos los estudiantes activos.<br>3. El sistema calcula el promedio de cada estudiante con las calificaciones de los tutores en sesiones atendidas.<br>4. El sistema muestra nombre completo y promedio en escala de 0 a 5 estrellas. |
| **Excepciones / Cursos alternativos** | Paso 3: Si un estudiante no tiene calificaciones, el sistema lo muestra sin promedio. |
| **Condiciones posteriores** | El administrador visualiza el desempeño evaluativo de los estudiantes. |
| **Comentarios** | HU-39 · RF-4-13-1 a RF-4-13-3 · RR-10, RR-14 · EAC-3, EAC-11. |

<br>

### CU4-14 Generar Reportes Analíticos Avanzados

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** | Generar Reportes Analíticos Avanzados |
| **Actores** | Administrador (iniciador) |
| **Propósito** | Respaldar la toma de decisiones estratégicas con métricas operativas adicionales. |
| **Resumen** | El administrador genera el Top 10 de tutores con más sesiones atendidas y el reporte de materias con mayor tasa de cancelación, con gráficos y datos en tiempo real. |
| **Condiciones previas** | El administrador debe estar autenticado. |
| **Secuencia normal** | 1. El administrador accede a «Reportes analíticos».<br>2. El administrador elige «Top 10 de tutores con más sesiones atendidas» o «Materias con mayor tasa de cancelación».<br>3. El sistema consolida y calcula los datos en tiempo real según la actividad de la plataforma.<br>4. El sistema presenta el reporte con gráficos de barras o líneas y tablas estructuradas. |
| **Excepciones / Cursos alternativos** | Paso 3: Si no existen datos suficientes, el sistema informa que no hay información para generar el reporte. |
| **Condiciones posteriores** | El administrador visualiza el reporte analítico solicitado. |
| **Comentarios** | HU-40 · RF-4-14-1 a RF-4-14-3 · RR-14 · EAC-3, EAC-11. |

<br>

---

## Plantilla vacía para un caso de uso

> Copia esta estructura para documentar cada caso de uso expandido. La secuencia normal debe indicar el orden esperado de las acciones; las excepciones deben explicar condiciones alternativas, errores o decisiones del actor.

### CU__ Nombre del caso de uso

| Campo | Descripción |
|-------|-------------|
| **Nombre del caso de uso** |  |
| **Actores** |  |
| **Propósito** |  |
| **Resumen** |  |
| **Condiciones previas** |  |
| **Secuencia normal** | 1.  <br>2.  <br>3.  |
| **Excepciones / Cursos alternativos** |  |
| **Condiciones posteriores** |  |
| **Comentarios** |  |