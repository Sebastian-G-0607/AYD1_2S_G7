# Requerimientos de Restricción (RR)

### RESTRICCIONES TECNOLÓGICAS

**RR-1**\
**Descripción:** El carnet escaneado del estudiante y el CV del tutor deben aceptarse únicamente en formato PDF.\
**Justificación:** HU-22 y HU-23 exigen PDF y el rechazo de cualquier otra extensión, lo que permite al administrador revisarlos en un visor PDF embebido.\
**Categoría:** Tecnológica\
**Impacto:** Se rechazan DOCX, imágenes y cualquier otro formato; la validación verifica tipo y extensión del archivo.

**RR-2**\
**Descripción:** Los PDF del carnet y del CV deben visualizarse en un visor PDF embebido en el navegador, sin descarga local.\
**Justificación:** HU-32 y HU-33 piden validar identidad y perfil profesional sin descargar archivos.\
**Categoría:** Tecnológica / Interfaz\
**Impacto:** La interfaz del administrador requiere un componente de visualización de PDF integrado.

**RR-3**\
**Descripción:** Las contraseñas de estudiantes, tutores y administrador deben almacenarse encriptadas en la base de datos y nunca en texto plano.\
**Justificación:** HU-01, HU-02, HU-04, HU-15 y HU-21 exigen almacenamiento encriptado.\
**Categoría:** Tecnológica\
**Impacto:** El registro, el inicio de sesión y el cambio de contraseña usan comparación sobre valores encriptados; las contraseñas no pueden recuperarse en su forma original.

**RR-4**\
**Descripción:** El sistema debe operar como una aplicación web accesible desde un navegador, sin instalación en el equipo del usuario.\
**Justificación:** Las HU definen paneles, páginas, formularios y un visor PDF embebido en el navegador.\
**Categoría:** Tecnológica\
**Impacto:** Toda la funcionalidad se entrega mediante interfaz web.

**RR-5**\
**Descripción:** Las notificaciones a los usuarios (aprobación, rechazo, baja, cancelación de sesión y token de verificación) deben enviarse únicamente por correo electrónico.\
**Justificación:** HU-05, HU-06, HU-07, HU-13, HU-24, HU-32 y HU-33 definen el correo como canal de notificación.\
**Categoría:** Tecnológica\
**Impacto:** Requiere integración con un servicio de envío de correo y descarta otros canales (SMS, notificaciones push).

**RR-6**\
**Descripción:** El token de verificación de correo debe ser un código de exactamente 6 dígitos.\
**Justificación:** HU-24 define el formato del token.\
**Categoría:** Tecnológica\
**Impacto:** Define el formato de generación, envío y validación del token.

**RR-7**\
**Descripción:** La segunda autenticación del administrador debe realizarse cargando el archivo de texto `auth2-ayd1.txt`, que contiene una contraseña encriptada distinta a la del primer paso.\
**Justificación:** HU-04 define el mecanismo de doble autenticación.\
**Categoría:** Tecnológica / Organizacional\
**Impacto:** Fija el nombre y el formato (.txt) del archivo y exige dos credenciales diferentes para el administrador.

### RESTRICCIONES ORGANIZACIONALES

**RR-8**\
**Descripción:** La cuenta del administrador debe ser predeterminada; el sistema no ofrece registro de administradores.\
**Justificación:** HU-04 establece un usuario y una contraseña predeterminados para el administrador.\
**Categoría:** Organizacional\
**Impacto:** La cuenta se precarga en el despliegue y no existe un flujo de alta de administradores.

**RR-9**\
**Descripción:** El correo electrónico debe ser el identificador único de la cuenta y no puede modificarse después del registro.\
**Justificación:** HU-01 y HU-02 exigen unicidad; HU-15, HU-21, HU-34 y HU-35 excluyen el correo de la edición.\
**Categoría:** Organizacional\
**Impacto:** Restricción de unicidad en la base de datos; los formularios de perfil muestran el correo en solo lectura.

**RR-10**\
**Descripción:** Las calificaciones deben usar una escala de 0 a 5 estrellas.\
**Justificación:** HU-27, HU-30, HU-38 y HU-39 definen la escala.\
**Categoría:** Organizacional\
**Impacto:** Los promedios de los reportes se expresan en la misma escala.

**RR-11**\
**Descripción:** Una sesión debe estar en uno de estos cuatro estados: Pendiente, Atendido, Cancelada por el estudiante o Cancelada por el tutor; solo las sesiones en estado Atendido admiten calificación y reporte.\
**Justificación:** HU-12, HU-13, HU-19, HU-27, HU-28, HU-30 y HU-31 condicionan las acciones al estado de la sesión.\
**Categoría:** Organizacional\
**Impacto:** El modelo de datos y las reglas de transición se limitan a esos cuatro estados.

**RR-12**\
**Descripción:** El horario de atención del tutor debe definirse como un único rango horario aplicado a todos los días seleccionados.\
**Justificación:** HU-10 establece el rango uniforme para todos los días.\
**Categoría:** Organizacional\
**Impacto:** No se soportan horarios distintos por día.

**RR-13**\
**Descripción:** Los reportes entre usuarios deben clasificarse únicamente con categorías predefinidas.\
**Justificación:** HU-28 y HU-31 exigen una lista predefinida de categorías.\
**Categoría:** Organizacional\
**Impacto:** Las categorías son un catálogo cerrado; no se aceptan categorías de texto libre.

### RESTRICCIONES DE INTERFAZ

**RR-14**\
**Descripción:** Los reportes del administrador deben presentarse con tablas y gráficos (barras, circulares o líneas).\
**Justificación:** HU-08 y HU-40 exigen presentación gráfica.\
**Categoría:** Interfaz\
**Impacto:** Requiere una biblioteca de gráficos en el front-end.

Las categorías Legal y Presupuesto no generan RR porque las 40 HU no contienen condiciones de ese tipo.
