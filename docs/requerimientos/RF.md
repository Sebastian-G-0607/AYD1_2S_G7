# Requerimientos Funcionales

### CU1 REGISTRO Y AUTENTICACIÓN

#### CU1-1 Registro de Estudiante (HU-01)

**RF-1-1-1**\
**Descripción:** Se debe permitir al estudiante iniciar su registro en el sistema mediante un formulario de registro.\
**Prioridad:** Alta\
**Actores involucrados:** Estudiante

**RF-1-1-2**\
**Descripción:** Se debe solicitar al estudiante nombre, apellido, carnet universitario, género, dirección, teléfono, fecha de nacimiento, correo electrónico y contraseña.\
**Prioridad:** Alta\
**Actores involucrados:** Estudiante

**RF-1-1-3**\
**Descripción:** Se debe incluir en el formulario el campo de fotografía de perfil, cuya obligatoriedad se define en RF-1-5-1.\
**Prioridad:** Alta\
**Actores involucrados:** Estudiante

**RF-1-1-4**\
**Descripción:** Se debe validar que la contraseña tenga mínimo 8 caracteres e incluya al menos una letra minúscula, una mayúscula y un número.\
**Prioridad:** Alta\
**Actores involucrados:** Estudiante, Sistema

**RF-1-1-5**\
**Descripción:** Se debe validar que el correo electrónico no esté registrado y rechazar el registro si está duplicado.\
**Prioridad:** Alta\
**Actores involucrados:** Estudiante, Sistema

**RF-1-1-6**\
**Descripción:** Se debe crear la cuenta del estudiante en estado «Pendiente de aprobación» al completar el registro.\
**Prioridad:** Alta\
**Actores involucrados:** Sistema

#### CU1-2 Registro de Tutor (HU-02)

**RF-1-2-1**\
**Descripción:** Se debe permitir al tutor iniciar su registro en el sistema mediante un formulario de registro.\
**Prioridad:** Alta\
**Actores involucrados:** Tutor

**RF-1-2-2**\
**Descripción:** Se debe solicitar al tutor nombre, apellido, carnet o ID, fecha de nacimiento, género, dirección, teléfono, número de identificación de tutor, correo electrónico y contraseña.\
**Prioridad:** Alta\
**Actores involucrados:** Tutor

**RF-1-2-3**\
**Descripción:** Se debe solicitar al tutor su información profesional: especialidad (materias que imparte), dirección de tutoría (física u online), año de inicio de tutorías y universidad de graduación.\
**Prioridad:** Alta\
**Actores involucrados:** Tutor

**RF-1-2-4**\
**Descripción:** Se debe solicitar al tutor una fotografía de perfil obligatoria e impedir el registro si no se carga.\
**Prioridad:** Alta\
**Actores involucrados:** Tutor, Sistema

**RF-1-2-5**\
**Descripción:** Se debe validar la unicidad del correo electrónico y del número de identificación del tutor, rechazando el registro si alguno está duplicado.\
**Prioridad:** Alta\
**Actores involucrados:** Tutor, Sistema

**RF-1-2-6**\
**Descripción:** Se debe validar que la contraseña tenga mínimo 8 caracteres e incluya al menos una letra minúscula, una mayúscula y un número.\
**Prioridad:** Alta\
**Actores involucrados:** Tutor, Sistema

**RF-1-2-7**\
**Descripción:** Se debe crear la cuenta del tutor en estado «Pendiente de aprobación» al completar el registro.\
**Prioridad:** Alta\
**Actores involucrados:** Sistema

#### CU1-3 Inicio de Sesión de Estudiantes y Tutores (HU-03)

**RF-1-3-1**\
**Descripción:** Se debe permitir a estudiantes y tutores iniciar sesión con su correo electrónico y contraseña.\
**Prioridad:** Alta\
**Actores involucrados:** Estudiante, Tutor

**RF-1-3-2**\
**Descripción:** Se debe verificar que el usuario haya sido aprobado por el administrador e impedir el acceso si no lo está.\
**Prioridad:** Alta\
**Actores involucrados:** Sistema

**RF-1-3-3**\
**Descripción:** Se debe mostrar un mensaje con la información del problema cuando ocurra un error de autenticación.\
**Prioridad:** Alta\
**Actores involucrados:** Sistema

**RF-1-3-4**\
**Descripción:** Se debe mostrar un enlace visible para registrarse en la pantalla de inicio de sesión.\
**Prioridad:** Alta\
**Actores involucrados:** Estudiante, Tutor

**RF-1-3-5**\
**Descripción:** Se debe redirigir al usuario autenticado al panel correspondiente a su rol (estudiante o tutor).\
**Prioridad:** Alta\
**Actores involucrados:** Sistema

#### CU1-4 Inicio de Sesión de Administrador con 2FA (HU-04)

**RF-1-4-1**\
**Descripción:** Se debe permitir al administrador autenticarse en un primer paso con el usuario y la contraseña predeterminados.\
**Prioridad:** Alta\
**Actores involucrados:** Administrador

**RF-1-4-2**\
**Descripción:** Se debe redirigir al administrador, tras el primer paso exitoso, a una página para cargar el archivo `auth2-ayd1.txt`.\
**Prioridad:** Alta\
**Actores involucrados:** Administrador, Sistema

**RF-1-4-3**\
**Descripción:** Se debe validar la contraseña encriptada contenida en el archivo cargado y permitir el acceso a la página principal del administrador si es correcta.\
**Prioridad:** Alta\
**Actores involucrados:** Administrador, Sistema

**RF-1-4-4**\
**Descripción:** Se debe denegar el acceso al panel de administración si el archivo cargado no contiene la contraseña correcta.\
**Prioridad:** Alta\
**Actores involucrados:** Sistema

#### CU1-5 Fotografía y Carnet del Estudiante (HU-22)

**RF-1-5-1**\
**Descripción:** Se debe solicitar al estudiante una fotografía personal reciente e impedir su registro si no se sube.\
**Prioridad:** Alta\
**Actores involucrados:** Estudiante, Sistema

**RF-1-5-2**\
**Descripción:** Se debe solicitar al estudiante su carnet escaneado en un archivo PDF e impedir su registro si no se carga.\
**Prioridad:** Alta\
**Actores involucrados:** Estudiante, Sistema

**RF-1-5-3**\
**Descripción:** Se debe aceptar únicamente archivos de tipo PDF en el apartado del carnet, rechazando cualquier otro tipo o extensión.\
**Prioridad:** Alta\
**Actores involucrados:** Sistema

**RF-1-5-4**\
**Descripción:** Se debe permitir al estudiante reemplazar la fotografía o el PDF del carnet antes de enviar su registro.\
**Prioridad:** Alta\
**Actores involucrados:** Estudiante

#### CU1-6 CV del Tutor (HU-23)

**RF-1-6-1**\
**Descripción:** Se debe solicitar al tutor su CV actualizado e impedir su registro si no se carga.\
**Prioridad:** Alta\
**Actores involucrados:** Tutor, Sistema

**RF-1-6-2**\
**Descripción:** Se debe aceptar únicamente archivos de tipo PDF en el apartado del CV, rechazando cualquier otra extensión.\
**Prioridad:** Alta\
**Actores involucrados:** Sistema

**RF-1-6-3**\
**Descripción:** Se debe permitir al tutor reemplazar el PDF del CV antes de enviar su registro.\
**Prioridad:** Alta\
**Actores involucrados:** Tutor

#### CU1-7 Validación de Correo Electrónico con Token (HU-24)

**RF-1-7-1**\
**Descripción:** Se debe enviar automáticamente un token de verificación de 6 dígitos al correo del usuario cuando finalice su registro.\
**Prioridad:** Alta\
**Actores involucrados:** Sistema

**RF-1-7-2**\
**Descripción:** Se debe incluir en el correo del token el logo de la plataforma, un mensaje de bienvenida y el token de verificación.\
**Prioridad:** Alta\
**Actores involucrados:** Sistema

**RF-1-7-3**\
**Descripción:** Se debe solicitar el token de verificación al usuario en su primer inicio de sesión.\
**Prioridad:** Alta\
**Actores involucrados:** Estudiante, Tutor

**RF-1-7-4**\
**Descripción:** Se debe impedir el primer inicio de sesión si el token ingresado es incorrecto.\
**Prioridad:** Alta\
**Actores involucrados:** Sistema

**RF-1-7-5**\
**Descripción:** Se debe permitir al usuario solicitar el reenvío del token cuando se haya perdido o no se haya recibido.\
**Prioridad:** Alta\
**Actores involucrados:** Estudiante, Tutor, Sistema

**RF-1-7-6**\
**Descripción:** Se debe marcar el correo del usuario como válido tras el primer inicio de sesión exitoso y no volver a solicitar el token en inicios de sesión posteriores.\
**Prioridad:** Alta\
**Actores involucrados:** Sistema

### CU2 ESTUDIANTE

#### CU2-1 Exploración y Búsqueda de Tutores (HU-16)

**RF-2-1-1**\
**Descripción:** Se debe mostrar en la página principal del estudiante la lista de tutores aprobados, excluyendo aquellos con los que ya tiene una sesión.\
**Prioridad:** Alta\
**Actores involucrados:** Estudiante

**RF-2-1-2**\
**Descripción:** Se debe mostrar por cada tutor su nombre completo, especialidad (materias), dirección de tutoría y fotografía.\
**Prioridad:** Alta\
**Actores involucrados:** Estudiante

**RF-2-1-3**\
**Descripción:** Se debe permitir al estudiante filtrar tutores por materia, años de experiencia, sexo, edad y universidad.\
**Prioridad:** Alta\
**Actores involucrados:** Estudiante

**RF-2-1-4**\
**Descripción:** Se debe mostrar únicamente los tutores que cumplan todos los filtros seleccionados al combinarlos.\
**Prioridad:** Alta\
**Actores involucrados:** Estudiante, Sistema

#### CU2-2 Consulta de Horarios y Disponibilidad (HU-17)

**RF-2-2-1**\
**Descripción:** Se debe mostrar los días y horarios de atención del tutor al seleccionarlo.\
**Prioridad:** Alta\
**Actores involucrados:** Estudiante

**RF-2-2-2**\
**Descripción:** Se debe permitir al estudiante filtrar la disponibilidad del tutor por fecha.\
**Prioridad:** Alta\
**Actores involucrados:** Estudiante

**RF-2-2-3**\
**Descripción:** Se debe mostrar de forma diferenciada los horarios ocupados y los disponibles de la fecha seleccionada.\
**Prioridad:** Alta\
**Actores involucrados:** Estudiante, Sistema

**RF-2-2-4**\
**Descripción:** Se debe indicar al estudiante cuando el tutor no atiende en la fecha seleccionada.\
**Prioridad:** Alta\
**Actores involucrados:** Estudiante, Sistema

#### CU2-3 Programar Sesión de Tutoría (HU-18)

**RF-2-3-1**\
**Descripción:** Se debe permitir al estudiante agendar una sesión con un tutor mediante un formulario.\
**Prioridad:** Alta\
**Actores involucrados:** Estudiante

**RF-2-3-2**\
**Descripción:** Se debe solicitar en el formulario la fecha, la hora, la materia (de las impartidas por el tutor) y el motivo de la sesión.\
**Prioridad:** Alta\
**Actores involucrados:** Estudiante

**RF-2-3-3**\
**Descripción:** Se debe validar que la fecha y hora seleccionadas estén dentro del horario del tutor y que el espacio esté disponible.\
**Prioridad:** Alta\
**Actores involucrados:** Sistema

**RF-2-3-4**\
**Descripción:** Se debe impedir que el estudiante tenga más de una sesión activa con el mismo tutor al mismo tiempo.\
**Prioridad:** Alta\
**Actores involucrados:** Sistema

**RF-2-3-5**\
**Descripción:** Se debe impedir el traslape de horario entre las sesiones del estudiante en un mismo día.\
**Prioridad:** Alta\
**Actores involucrados:** Sistema

**RF-2-3-6**\
**Descripción:** Se debe notificar al estudiante el motivo específico cuando falle alguna validación del agendamiento.\
**Prioridad:** Alta\
**Actores involucrados:** Estudiante, Sistema

**RF-2-3-7**\
**Descripción:** Se debe registrar la sesión en estado «Pendiente» y marcar el horario como ocupado en la agenda del tutor al agendarla.\
**Prioridad:** Alta\
**Actores involucrados:** Sistema

#### CU2-4 Gestión de Sesiones Activas (HU-19)

**RF-2-4-1**\
**Descripción:** Se debe mostrar al estudiante una vista de «Sesiones Activas» con fecha, hora, tutor, materia, dirección y motivo de cada sesión.\
**Prioridad:** Media\
**Actores involucrados:** Estudiante

**RF-2-4-2**\
**Descripción:** Se debe permitir al estudiante cancelar una sesión activa, solicitando un mensaje de confirmación antes de removerla de la lista.\
**Prioridad:** Media\
**Actores involucrados:** Estudiante

**RF-2-4-3**\
**Descripción:** Se debe cambiar la sesión al estado «Cancelada por el estudiante» al confirmar la cancelación.\
**Prioridad:** Media\
**Actores involucrados:** Sistema

**RF-2-4-4**\
**Descripción:** Se debe liberar inmediatamente el horario de la sesión cancelada en la agenda del tutor para que otros estudiantes puedan reservarlo.\
**Prioridad:** Media\
**Actores involucrados:** Sistema

#### CU2-5 Visualización de Historial de Sesiones (HU-20)

**RF-2-5-1**\
**Descripción:** Se debe mostrar al estudiante el historial de sus sesiones atendidas y canceladas.\
**Prioridad:** Baja\
**Actores involucrados:** Estudiante

**RF-2-5-2**\
**Descripción:** Se debe mostrar por cada sesión del historial la fecha, el tutor, la materia, el motivo, la dirección, el estado y el resumen de la sesión (si fue atendida).\
**Prioridad:** Baja\
**Actores involucrados:** Estudiante

#### CU2-6 Gestión de Perfil del Estudiante (HU-21)

**RF-2-6-1**\
**Descripción:** Se debe permitir al estudiante visualizar todos los datos registrados en su perfil.\
**Prioridad:** Baja\
**Actores involucrados:** Estudiante

**RF-2-6-2**\
**Descripción:** Se debe permitir al estudiante modificar cualquier campo de su perfil, excepto el correo electrónico.\
**Prioridad:** Baja\
**Actores involucrados:** Estudiante

**RF-2-6-3**\
**Descripción:** Se debe exigir la contraseña original y validarla antes de permitir el cambio de contraseña.\
**Prioridad:** Baja\
**Actores involucrados:** Estudiante, Sistema

**RF-2-6-4**\
**Descripción:** Se debe validar que la nueva contraseña tenga mínimo 8 caracteres e incluya al menos una letra minúscula, una mayúscula y un número.\
**Prioridad:** Baja\
**Actores involucrados:** Sistema

#### CU2-7 Plan de Estudio del Estudiante (HU-25)

**RF-2-7-1**\
**Descripción:** Se debe mostrar al estudiante en su plan de estudio la fecha de su última sesión con el nombre, la identificación y la especialidad del tutor.\
**Prioridad:** Alta\
**Actores involucrados:** Estudiante

**RF-2-7-2**\
**Descripción:** Se debe mostrar en el plan de estudio las dificultades identificadas por el tutor.\
**Prioridad:** Alta\
**Actores involucrados:** Estudiante

**RF-2-7-3**\
**Descripción:** Se debe mostrar en el plan de estudio los recursos recomendados con su nombre, tipo y descripción de uso.\
**Prioridad:** Alta\
**Actores involucrados:** Estudiante

#### CU2-8 Imprimir Plan de Estudio (HU-26)

**RF-2-8-1**\
**Descripción:** Se debe permitir al estudiante generar un archivo PDF de su plan de estudio.\
**Prioridad:** Media\
**Actores involucrados:** Estudiante, Sistema

**RF-2-8-2**\
**Descripción:** Se debe incluir en el encabezado del PDF el nombre de la plataforma, la fecha de emisión y el teléfono de contacto.\
**Prioridad:** Media\
**Actores involucrados:** Sistema

**RF-2-8-3**\
**Descripción:** Se debe incluir en el cuerpo del PDF una tabla con los recursos recomendados (nombre, tipo y descripción de uso).\
**Prioridad:** Media\
**Actores involucrados:** Sistema

**RF-2-8-4**\
**Descripción:** Se debe incluir en el pie del PDF el nombre del tutor, su especialidad y su número de identificación.\
**Prioridad:** Media\
**Actores involucrados:** Sistema

#### CU2-9 Calificar Atención de Tutoría (HU-27)

**RF-2-9-1**\
**Descripción:** Se debe permitir al estudiante calificar la atención recibida con una calificación obligatoria de 0 a 5 estrellas.\
**Prioridad:** Media\
**Actores involucrados:** Estudiante

**RF-2-9-2**\
**Descripción:** Se debe permitir al estudiante agregar un comentario opcional con el motivo de la calificación.\
**Prioridad:** Media\
**Actores involucrados:** Estudiante

**RF-2-9-3**\
**Descripción:** Se debe permitir calificar únicamente sesiones en estado «Atendido» y rechazar la calificación de sesiones canceladas o pendientes.\
**Prioridad:** Media\
**Actores involucrados:** Sistema

#### CU2-10 Reportar Tutor (HU-28)

**RF-2-10-1**\
**Descripción:** Se debe permitir al estudiante reportar a un tutor después de haber sido atendido.\
**Prioridad:** Media\
**Actores involucrados:** Estudiante

**RF-2-10-2**\
**Descripción:** Se debe mostrar en el formulario una lista predefinida de categorías de reporte (por ejemplo, Ética y Negligencia académica).\
**Prioridad:** Media\
**Actores involucrados:** Estudiante

**RF-2-10-3**\
**Descripción:** Se debe solicitar al estudiante la explicación del motivo del reporte.\
**Prioridad:** Media\
**Actores involucrados:** Estudiante

**RF-2-10-4**\
**Descripción:** Se debe enviar automáticamente el reporte al administrador al confirmarlo.\
**Prioridad:** Media\
**Actores involucrados:** Sistema, Administrador

**RF-2-10-5**\
**Descripción:** Se debe permitir reportar únicamente sesiones en estado «Atendido» y rechazar el reporte de sesiones canceladas o pendientes.\
**Prioridad:** Media\
**Actores involucrados:** Sistema

### CU3 TUTOR

#### CU3-1 Establecer Horarios de Atención (HU-10)

**RF-3-1-1**\
**Descripción:** Se debe permitir al tutor seleccionar los días de la semana en los que atenderá.\
**Prioridad:** Alta\
**Actores involucrados:** Tutor

**RF-3-1-2**\
**Descripción:** Se debe permitir al tutor establecer un rango de horario (por ejemplo, de 8 am a 5 pm) que se aplique uniformemente a todos los días seleccionados.\
**Prioridad:** Alta\
**Actores involucrados:** Tutor

**RF-3-1-3**\
**Descripción:** Se debe publicar los días y el rango de horario del tutor para que los estudiantes puedan consultarlos y agendar sesiones.\
**Prioridad:** Alta\
**Actores involucrados:** Sistema, Estudiante

#### CU3-2 Actualizar Horarios de Atención (HU-11)

**RF-3-2-1**\
**Descripción:** Se debe permitir al tutor modificar los días y el rango de horario de atención.\
**Prioridad:** Media\
**Actores involucrados:** Tutor

**RF-3-2-2**\
**Descripción:** Se debe validar que no existan sesiones activas fuera del nuevo rango de horario al actualizarlo.\
**Prioridad:** Media\
**Actores involucrados:** Sistema

**RF-3-2-3**\
**Descripción:** Se debe impedir la actualización del horario mientras existan sesiones en conflicto, hasta que sean reprogramadas o canceladas.\
**Prioridad:** Media\
**Actores involucrados:** Sistema, Tutor

#### CU3-3 Gestión y Atención de Sesiones Pendientes (HU-12)

**RF-3-3-1**\
**Descripción:** Se debe mostrar al tutor sus sesiones pendientes ordenadas por fecha más próxima.\
**Prioridad:** Alta\
**Actores involucrados:** Tutor

**RF-3-3-2**\
**Descripción:** Se debe mostrar por cada sesión pendiente la fecha, la hora, el nombre del estudiante, el motivo y la materia.\
**Prioridad:** Alta\
**Actores involucrados:** Tutor

**RF-3-3-3**\
**Descripción:** Se debe incluir un botón «Atendido» en cada sesión pendiente para marcar al estudiante como atendido.\
**Prioridad:** Alta\
**Actores involucrados:** Tutor

**RF-3-3-4**\
**Descripción:** Se debe desplegar, al presionar «Atendido», el formulario de plan de estudio definido en CU3-7 antes de retirar la sesión de la lista de pendientes.\
**Prioridad:** Alta\
**Actores involucrados:** Tutor, Sistema

#### CU3-4 Cancelación de Sesión por el Tutor (HU-13)

**RF-3-4-1**\
**Descripción:** Se debe permitir al tutor seleccionar una sesión pendiente y cancelarla, retirándola de su lista.\
**Prioridad:** Alta\
**Actores involucrados:** Tutor

**RF-3-4-2**\
**Descripción:** Se debe liberar inmediatamente el horario de la sesión cancelada en la agenda del tutor para que vuelva a estar disponible.\
**Prioridad:** Alta\
**Actores involucrados:** Sistema

**RF-3-4-3**\
**Descripción:** Se debe enviar un correo automático al estudiante notificando la cancelación de la sesión.\
**Prioridad:** Alta\
**Actores involucrados:** Sistema, Estudiante

**RF-3-4-4**\
**Descripción:** Se debe incluir en el correo de cancelación la fecha, la hora, el motivo de la sesión, el nombre del tutor, la materia y un mensaje de disculpa.\
**Prioridad:** Alta\
**Actores involucrados:** Sistema

#### CU3-5 Historial de Sesiones del Tutor (HU-14)

**RF-3-5-1**\
**Descripción:** Se debe mostrar al tutor, desde su panel de usuario, el listado de todas sus sesiones con fecha, hora, nombre del estudiante y estado.\
**Prioridad:** Alta\
**Actores involucrados:** Tutor

**RF-3-5-2**\
**Descripción:** Se debe permitir al tutor filtrar su historial de sesiones por fechas.\
**Prioridad:** Alta\
**Actores involucrados:** Tutor

**RF-3-5-3**\
**Descripción:** Se debe permitir al tutor filtrar su historial de sesiones por estudiante.\
**Prioridad:** Alta\
**Actores involucrados:** Tutor

#### CU3-6 Ver y Actualizar Perfil del Tutor (HU-15)

**RF-3-6-1**\
**Descripción:** Se debe permitir al tutor visualizar todos los datos registrados en su perfil.\
**Prioridad:** Alta\
**Actores involucrados:** Tutor

**RF-3-6-2**\
**Descripción:** Se debe permitir al tutor modificar cualquier campo de su perfil, excepto el correo electrónico.\
**Prioridad:** Alta\
**Actores involucrados:** Tutor

**RF-3-6-3**\
**Descripción:** Se debe permitir al tutor subir una nueva fotografía de perfil.\
**Prioridad:** Alta\
**Actores involucrados:** Tutor

**RF-3-6-4**\
**Descripción:** Se debe exigir la contraseña original y validarla contra la base de datos antes de permitir el cambio de contraseña.\
**Prioridad:** Alta\
**Actores involucrados:** Tutor, Sistema

**RF-3-6-5**\
**Descripción:** Se debe validar que la nueva contraseña tenga mínimo 8 caracteres e incluya al menos una letra minúscula, una mayúscula y un número.\
**Prioridad:** Alta\
**Actores involucrados:** Sistema

#### CU3-7 Atender Estudiante (HU-29)

**RF-3-7-1**\
**Descripción:** Se debe incluir un botón que despliegue el formulario donde el tutor ingresa el plan de estudio del estudiante.\
**Prioridad:** Alta\
**Actores involucrados:** Tutor

**RF-3-7-2**\
**Descripción:** Se debe solicitar en el formulario, de forma obligatoria, las dificultades identificadas del estudiante.\
**Prioridad:** Alta\
**Actores involucrados:** Tutor

**RF-3-7-3**\
**Descripción:** Se debe solicitar uno o más recursos recomendados, permitiendo ingresar de forma dinámica el nombre, el tipo y la descripción de uso de cada uno.\
**Prioridad:** Alta\
**Actores involucrados:** Tutor

**RF-3-7-4**\
**Descripción:** Se debe marcar la sesión como «Atendido» y retirarla de la lista de sesiones pendientes del tutor al guardar el plan de estudio.\
**Prioridad:** Alta\
**Actores involucrados:** Sistema

#### CU3-8 Calificar Estudiante (HU-30)

**RF-3-8-1**\
**Descripción:** Se debe permitir al tutor calificar a un estudiante atendido con una calificación obligatoria de 0 a 5 estrellas.\
**Prioridad:** Media\
**Actores involucrados:** Tutor

**RF-3-8-2**\
**Descripción:** Se debe solicitar al tutor un comentario obligatorio con el motivo de la calificación.\
**Prioridad:** Media\
**Actores involucrados:** Tutor

**RF-3-8-3**\
**Descripción:** Se debe permitir calificar únicamente sesiones en estado «Atendido» y rechazar la calificación de sesiones canceladas o pendientes.\
**Prioridad:** Media\
**Actores involucrados:** Sistema

#### CU3-9 Reportar Estudiante (HU-31)

**RF-3-9-1**\
**Descripción:** Se debe mostrar al tutor, en el formulario de reporte, una lista predefinida de categorías de reporte.\
**Prioridad:** Media\
**Actores involucrados:** Tutor

**RF-3-9-2**\
**Descripción:** Se debe solicitar al tutor una descripción detallada del reporte.\
**Prioridad:** Media\
**Actores involucrados:** Tutor

**RF-3-9-3**\
**Descripción:** Se debe enviar automáticamente el reporte al administrador al enviar el formulario.\
**Prioridad:** Media\
**Actores involucrados:** Sistema, Administrador

**RF-3-9-4**\
**Descripción:** Se debe permitir reportar únicamente sesiones en estado «Atendido» y rechazar el reporte de sesiones canceladas o pendientes.\
**Prioridad:** Media\
**Actores involucrados:** Sistema

### CU4 ADMINISTRADOR

#### CU4-1 Aprobación de Registro de Estudiantes (HU-05)

**RF-4-1-1**\
**Descripción:** Se debe mostrar al administrador la lista de estudiantes pendientes de aprobación.\
**Prioridad:** Alta\
**Actores involucrados:** Administrador

**RF-4-1-2**\
**Descripción:** Se debe mostrar por cada estudiante pendiente su fotografía (o una por defecto si no tiene), nombre completo, carnet, género, fecha de nacimiento y correo electrónico.\
**Prioridad:** Alta\
**Actores involucrados:** Administrador

**RF-4-1-3**\
**Descripción:** Se debe incluir un botón para aceptar y otro para rechazar la solicitud a un lado de cada registro.\
**Prioridad:** Alta\
**Actores involucrados:** Administrador

**RF-4-1-4**\
**Descripción:** Se debe enviar un correo al estudiante informando si su cuenta fue aprobada o rechazada.\
**Prioridad:** Alta\
**Actores involucrados:** Sistema, Estudiante

#### CU4-2 Aprobación de Registro de Tutores (HU-06)

**RF-4-2-1**\
**Descripción:** Se debe mostrar al administrador la lista de tutores pendientes de aprobación.\
**Prioridad:** Alta\
**Actores involucrados:** Administrador

**RF-4-2-2**\
**Descripción:** Se debe mostrar por cada tutor pendiente su fotografía, nombre completo, carnet, género, especialidad, número de identificación y correo electrónico.\
**Prioridad:** Alta\
**Actores involucrados:** Administrador

**RF-4-2-3**\
**Descripción:** Se debe incluir un botón para aceptar y otro para rechazar a cada tutor.\
**Prioridad:** Alta\
**Actores involucrados:** Administrador

**RF-4-2-4**\
**Descripción:** Se debe enviar un correo al tutor notificando si fue aprobado o rechazado.\
**Prioridad:** Alta\
**Actores involucrados:** Sistema, Tutor

#### CU4-3 Gestión de Usuarios Activos (HU-07)

**RF-4-3-1**\
**Descripción:** Se debe mostrar al administrador una vista con todos los estudiantes aceptados y una opción para darlos de baja.\
**Prioridad:** Media\
**Actores involucrados:** Administrador

**RF-4-3-2**\
**Descripción:** Se debe mostrar al administrador una vista separada con todos los tutores aceptados y una opción para darlos de baja.\
**Prioridad:** Media\
**Actores involucrados:** Administrador

**RF-4-3-3**\
**Descripción:** Se debe solicitar al administrador el motivo de la baja y registrar el motivo y la fecha de baja del usuario.\
**Prioridad:** Media\
**Actores involucrados:** Administrador, Sistema

**RF-4-3-4**\
**Descripción:** Se debe enviar automáticamente un correo al usuario informando la baja de su cuenta.\
**Prioridad:** Media\
**Actores involucrados:** Sistema

**RF-4-3-5**\
**Descripción:** Se debe cambiar el estado del usuario dado de baja a inactivo e impedir su inicio de sesión.\
**Prioridad:** Media\
**Actores involucrados:** Sistema

#### CU4-4 Generación de Reportes (HU-08)

**RF-4-4-1**\
**Descripción:** Se debe permitir al administrador generar el reporte de tutores que más estudiantes han atendido.\
**Prioridad:** Media\
**Actores involucrados:** Administrador

**RF-4-4-2**\
**Descripción:** Se debe permitir al administrador generar el reporte de materias con mayor demanda.\
**Prioridad:** Media\
**Actores involucrados:** Administrador

**RF-4-4-3**\
**Descripción:** Se debe presentar cada reporte de forma gráfica, incluyendo una tabla y un gráfico de barras o circular.\
**Prioridad:** Media\
**Actores involucrados:** Sistema

**RF-4-4-4**\
**Descripción:** Se debe calcular los datos de los reportes con la información vigente al momento de la consulta.\
**Prioridad:** Media\
**Actores involucrados:** Sistema

#### CU4-5 Visualización de Usuarios Dados de Baja (HU-09)

**RF-4-5-1**\
**Descripción:** Se debe mostrar al administrador la lista de estudiantes dados de baja.\
**Prioridad:** Media\
**Actores involucrados:** Administrador

**RF-4-5-2**\
**Descripción:** Se debe mostrar al administrador la lista de tutores dados de baja.\
**Prioridad:** Media\
**Actores involucrados:** Administrador

**RF-4-5-3**\
**Descripción:** Se debe mostrar por cada usuario dado de baja su nombre, correo electrónico, fecha de baja y motivo.\
**Prioridad:** Media\
**Actores involucrados:** Administrador

#### CU4-6 Aprobación de Estudiantes con Visor PDF Incrustado (HU-32)

**RF-4-6-1**\
**Descripción:** Se debe mostrar el carnet escaneado del estudiante pendiente en un visor PDF embebido dentro de la interfaz, sin forzar la descarga del archivo.\
**Prioridad:** Alta\
**Actores involucrados:** Administrador

**RF-4-6-2**\
**Descripción:** Se debe cambiar el estado del estudiante a activo al aceptarlo, permitiendo su autenticación con sus credenciales y el token de verificación.\
**Prioridad:** Alta\
**Actores involucrados:** Administrador, Sistema

**RF-4-6-3**\
**Descripción:** Se debe revocar el acceso del estudiante al rechazar su solicitud.\
**Prioridad:** Alta\
**Actores involucrados:** Administrador, Sistema

#### CU4-7 Aprobación de Tutores con Visor PDF Incrustado (HU-33)

**RF-4-7-1**\
**Descripción:** Se debe mostrar el CV del tutor pendiente en un visor PDF embebido dentro de la interfaz, sin descargas locales.\
**Prioridad:** Alta\
**Actores involucrados:** Administrador

**RF-4-7-2**\
**Descripción:** Se debe habilitar la cuenta del tutor al aprobarlo para que pueda completar su autenticación.\
**Prioridad:** Alta\
**Actores involucrados:** Administrador, Sistema

**RF-4-7-3**\
**Descripción:** Se debe denegar el ingreso del tutor al rechazar su solicitud.\
**Prioridad:** Alta\
**Actores involucrados:** Administrador, Sistema

#### CU4-8 Actualizar y Dar de Baja Estudiantes (HU-34)

**RF-4-8-1**\
**Descripción:** Se debe permitir al administrador editar cualquier campo del perfil de un estudiante aprobado (datos personales, carnet, teléfono, dirección, entre otros).\
**Prioridad:** Media\
**Actores involucrados:** Administrador

**RF-4-8-2**\
**Descripción:** Se debe mantener bloqueado contra edición el correo electrónico del estudiante.\
**Prioridad:** Media\
**Actores involucrados:** Sistema

**RF-4-8-3**\
**Descripción:** Se debe solicitar una confirmación explícita al administrador antes de ejecutar la acción «Dar de baja» sobre un estudiante.\
**Prioridad:** Media\
**Actores involucrados:** Administrador

#### CU4-9 Ver, Actualizar y Dar de Baja Tutores (HU-35)

**RF-4-9-1**\
**Descripción:** Se debe mostrar al administrador la lista de tutores aceptados con su información personal y profesional detallada.\
**Prioridad:** Media\
**Actores involucrados:** Administrador

**RF-4-9-2**\
**Descripción:** Se debe permitir al administrador editar todos los campos del tutor, excepto el correo electrónico, que permanece de solo lectura.\
**Prioridad:** Media\
**Actores involucrados:** Administrador

**RF-4-9-3**\
**Descripción:** Se debe solicitar un cuadro de confirmación al administrador antes de ejecutar la acción «Dar de baja» sobre un tutor.\
**Prioridad:** Media\
**Actores involucrados:** Administrador

**RF-4-9-4**\
**Descripción:** Se debe impedir que un tutor dado de baja inicie sesión o reciba nuevas reservas de tutorías.\
**Prioridad:** Media\
**Actores involucrados:** Sistema

#### CU4-10 Gestión de Reportes hacia Tutores (HU-36)

**RF-4-10-1**\
**Descripción:** Se debe mostrar al administrador la lista de reportes recibidos contra tutores con categoría, motivo, nombre completo del tutor denunciado, nombre completo del estudiante denunciante y fecha de emisión.\
**Prioridad:** Media\
**Actores involucrados:** Administrador

**RF-4-10-2**\
**Descripción:** Se debe ofrecer en cada reporte las acciones «Dar de baja al tutor» y «Rechazar denuncia».\
**Prioridad:** Media\
**Actores involucrados:** Administrador

**RF-4-10-3**\
**Descripción:** Se debe desactivar y suspender inmediatamente la cuenta del tutor al seleccionar «Dar de baja al tutor».\
**Prioridad:** Media\
**Actores involucrados:** Sistema

**RF-4-10-4**\
**Descripción:** Se debe cambiar el reporte al estado «Desestimado», sin alterar el estado del tutor, al seleccionar «Rechazar denuncia».\
**Prioridad:** Media\
**Actores involucrados:** Sistema

#### CU4-11 Gestión de Reportes hacia Estudiantes (HU-37)

**RF-4-11-1**\
**Descripción:** Se debe mostrar al administrador la lista de reportes emitidos por tutores con categoría, motivo, nombre del estudiante involucrado, nombre del tutor denunciante y fecha del incidente.\
**Prioridad:** Media\
**Actores involucrados:** Administrador

**RF-4-11-2**\
**Descripción:** Se debe ofrecer en cada reporte las acciones «Dar de baja al estudiante» y «Rechazar denuncia».\
**Prioridad:** Media\
**Actores involucrados:** Administrador

**RF-4-11-3**\
**Descripción:** Se debe desactivar inmediatamente la cuenta del estudiante e impedir su inicio de sesión al seleccionar «Dar de baja al estudiante».\
**Prioridad:** Media\
**Actores involucrados:** Sistema

**RF-4-11-4**\
**Descripción:** Se debe archivar el caso, manteniendo al estudiante en estado activo, al seleccionar «Rechazar denuncia».\
**Prioridad:** Media\
**Actores involucrados:** Sistema

#### CU4-12 Reporte Consolidado de Calificación de Tutores (HU-38)

**RF-4-12-1**\
**Descripción:** Se debe generar un reporte que incluya a todos los tutores activos de la plataforma.\
**Prioridad:** Baja\
**Actores involucrados:** Administrador

**RF-4-12-2**\
**Descripción:** Se debe mostrar por cada tutor su nombre completo, su especialidad y su promedio general de calificación en escala de 0 a 5 estrellas.\
**Prioridad:** Baja\
**Actores involucrados:** Administrador

**RF-4-12-3**\
**Descripción:** Se debe calcular automáticamente el promedio de cada tutor a partir de todas las calificaciones emitidas por estudiantes en sesiones atendidas.\
**Prioridad:** Baja\
**Actores involucrados:** Sistema

**RF-4-12-4**\
**Descripción:** Se debe permitir ordenar el reporte por promedio, de mayor a menor y de menor a mayor, para identificar a los tutores con mejor y menor rendimiento.\
**Prioridad:** Baja\
**Actores involucrados:** Administrador

#### CU4-13 Reporte Consolidado de Calificación de Estudiantes (HU-39)

**RF-4-13-1**\
**Descripción:** Se debe generar un reporte que incluya a todos los estudiantes activos del sistema.\
**Prioridad:** Baja\
**Actores involucrados:** Administrador

**RF-4-13-2**\
**Descripción:** Se debe mostrar por cada estudiante su nombre completo y su promedio de calificación en escala de 0 a 5 estrellas.\
**Prioridad:** Baja\
**Actores involucrados:** Administrador

**RF-4-13-3**\
**Descripción:** Se debe calcular el promedio de cada estudiante a partir de las calificaciones asignadas por los tutores en sesiones con estado «Atendido».\
**Prioridad:** Baja\
**Actores involucrados:** Sistema

#### CU4-14 Reportes Analíticos Avanzados (HU-40)

**RF-4-14-1**\
**Descripción:** Se debe permitir al administrador generar el reporte Top 10 de tutores con más sesiones atendidas y el reporte de materias con mayor tasa de cancelación, distintos a los reportes de CU4-4.\
**Prioridad:** Baja\
**Actores involucrados:** Administrador

**RF-4-14-2**\
**Descripción:** Se debe presentar los reportes analíticos con gráficos de barras o líneas y tablas estructuradas.\
**Prioridad:** Baja\
**Actores involucrados:** Sistema

**RF-4-14-3**\
**Descripción:** Se debe calcular los datos de los reportes analíticos de forma consolidada y en tiempo real según la actividad de la plataforma.\
**Prioridad:** Baja\
**Actores involucrados:** Sistema