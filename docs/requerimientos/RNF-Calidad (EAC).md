# Requerimientos de Calidad (EAC)

### **EAC-1: Búsqueda de tutores bajo carga**

| Campo | Contenido |
| --- | --- |
| Atributo de calidad | Eficiencia (desempeño) |
| Escenario crudo | Al inicio del ciclo, cientos de estudiantes buscan tutor aplicando filtros de materia, años de experiencia, sexo, edad y universidad al mismo tiempo. |
| Estímulo | Solicitud de listado de tutores con filtros avanzados (HU-16). |
| Fuente del estímulo | Estudiante. |
| Entorno | Operación normal con 500 estudiantes concurrentes. |
| Artefacto | Módulo de búsqueda de tutores y base de datos. |
| Respuesta esperada | Se entrega la lista filtrada correctamente, sin timeouts. |
| Medida de la respuesta | 95% de las consultas en ≤ 2 segundos con 500 usuarios concurrentes y 0% de timeouts. |
| Objetivo de negocio | Evitar que el estudiante abandone la plataforma por lentitud al buscar tutor. |

### **EAC-2: Agendamiento sin doble reserva**

| Campo | Contenido |
| --- | --- |
| Atributo de calidad | Confiabilidad (integridad de datos) |
| Escenario crudo | Dos estudiantes intentan reservar el mismo horario del mismo tutor en el mismo instante. |
| Estímulo | Dos solicitudes simultáneas de programar sesión sobre el mismo espacio (HU-18). |
| Fuente del estímulo | Estudiantes. |
| Entorno | Operación normal; el horario del tutor tiene un único espacio libre. |
| Artefacto | Módulo de agendamiento y base de datos de sesiones. |
| Respuesta esperada | Se registra una sola sesión y a la otra solicitud se le notifica que el espacio ya no está disponible. |
| Medida de la respuesta | 0 dobles reservas en 1,000 pruebas concurrentes sobre el mismo espacio; respuesta en ≤ 2 segundos. |
| Objetivo de negocio | Garantizar la confianza de estudiantes y tutores en la agenda. |

### **EAC-3: Reportes con datos en tiempo real**

| Campo | Contenido |
| --- | --- |
| Atributo de calidad | Eficiencia (desempeño) |
| Escenario crudo | El administrador abre reportes analíticos cuando la plataforma acumula miles de sesiones. |
| Estímulo | Solicitud de generación de un reporte (HU-08, HU-38, HU-39, HU-40). |
| Fuente del estímulo | Administrador. |
| Entorno | Operación normal con 10,000 sesiones y 5,000 usuarios registrados. |
| Artefacto | Módulo de reportes y base de datos. |
| Respuesta esperada | Se muestra el reporte con tablas y gráficos calculados con datos vigentes. |
| Medida de la respuesta | 95% de los reportes generados en ≤ 5 segundos. |
| Objetivo de negocio | Permitir decisiones estratégicas con información oportuna. |

### **EAC-4: Generación del PDF del plan de estudio**

| Campo | Contenido |
| --- | --- |
| Atributo de calidad | Eficiencia (desempeño) |
| Escenario crudo | Varios estudiantes descargan la evidencia de su plan de estudio al finalizar sus tutorías. |
| Estímulo | Solicitud de generación del PDF del plan de estudio (HU-26). |
| Fuente del estímulo | Estudiante. |
| Entorno | Operación normal con 100 solicitudes concurrentes. |
| Artefacto | Generador de PDF y módulo de plan de estudio. |
| Respuesta esperada | Se entrega el PDF con encabezado, tabla de recursos y pie completos. |
| Medida de la respuesta | 95% de los PDF generados en ≤ 5 segundos con 100 solicitudes concurrentes. |
| Objetivo de negocio | Entregar al estudiante evidencia inmediata de sus tutorías. |

### **EAC-5: Bloqueo de cuentas no aprobadas o inactivas**

| Campo | Contenido |
| --- | --- |
| Atributo de calidad | Seguridad (control de acceso) |
| Escenario crudo | Un usuario con cuenta pendiente, rechazada o dada de baja intenta iniciar sesión. |
| Estímulo | Solicitud de inicio de sesión con credenciales correctas (HU-03, HU-07, HU-32 a HU-37). |
| Fuente del estímulo | Estudiante o tutor sin aprobación o en estado inactivo. |
| Entorno | Operación normal. |
| Artefacto | Módulo de autenticación y gestión de usuarios. |
| Respuesta esperada | Se deniega el acceso y se muestra un mensaje con el motivo. |
| Medida de la respuesta | 100% de los intentos denegados; toda baja aplicada por el administrador impide un nuevo inicio de sesión en ≤ 5 segundos. |
| Objetivo de negocio | Garantizar que solo usuarios validados por la administración usen la plataforma. |

### **EAC-6: Doble autenticación del administrador**

| Campo | Contenido |
| --- | --- |
| Atributo de calidad | Seguridad (autenticación) |
| Escenario crudo | Una persona conoce el usuario y la contraseña del primer paso e intenta entrar al panel de administración sin el archivo `auth2-ayd1.txt` válido. |
| Estímulo | Carga de un archivo inexistente, modificado o con contraseña incorrecta (HU-04). |
| Fuente del estímulo | Administrador o atacante. |
| Entorno | Operación normal. |
| Artefacto | Módulo de autenticación del administrador. |
| Respuesta esperada | Se deniega el acceso y el usuario permanece en la página de carga del archivo. |
| Medida de la respuesta | 100% de los intentos con archivo inválido denegados; 0 accesos a páginas del panel sin completar los dos pasos. |
| Objetivo de negocio | Proteger las acciones críticas de administración (aprobaciones, bajas y reportes). |

### **EAC-7: Entrega de correos de notificación**

| Campo | Contenido |
| --- | --- |
| Atributo de calidad | Confiabilidad (disponibilidad del servicio de notificación) |
| Escenario crudo | El sistema debe enviar el token de verificación y las notificaciones de aprobación, rechazo, baja y cancelación de sesión. |
| Estímulo | Evento que dispara un correo (HU-05, HU-06, HU-07, HU-13, HU-24, HU-32, HU-33). |
| Fuente del estímulo | Sistema. |
| Entorno | Operación normal con 200 correos por hora. |
| Artefacto | Servicio de envío de correo. |
| Respuesta esperada | El correo se envía al destinatario; ante una falla del servicio se reintenta el envío. |
| Medida de la respuesta | 95% de los correos enviados en ≤ 60 segundos; hasta 3 reintentos y 99% de los correos entregados. |
| Objetivo de negocio | Que el usuario reciba a tiempo el token y las notificaciones para continuar su proceso. |

### **EAC-8: Liberación inmediata de horarios cancelados**

| Campo | Contenido |
| --- | --- |
| Atributo de calidad | Confiabilidad (consistencia de datos) |
| Escenario crudo | Un tutor o un estudiante cancela una sesión y otro estudiante consulta ese mismo horario. |
| Estímulo | Confirmación de cancelación de una sesión (HU-13, HU-19). |
| Fuente del estímulo | Tutor o estudiante. |
| Entorno | Operación normal. |
| Artefacto | Módulo de sesiones y agenda del tutor. |
| Respuesta esperada | El horario queda disponible en la agenda del tutor. |
| Medida de la respuesta | 100% de las cancelaciones liberan el horario y este se muestra como disponible en ≤ 2 segundos. |
| Objetivo de negocio | Maximizar el aprovechamiento de la agenda de los tutores. |

### **EAC-9: Registro de usuarios sin asistencia**

| Campo | Contenido |
| --- | --- |
| Atributo de calidad | Usabilidad |
| Escenario crudo | Un estudiante o tutor nuevo se registra por primera vez, cargando su fotografía y su documento PDF. |
| Estímulo | Inicio del formulario de registro (HU-01, HU-02, HU-22, HU-23). |
| Fuente del estímulo | Usuario nuevo. |
| Entorno | Primer uso, sin capacitación previa. |
| Artefacto | Formularios de registro. |
| Respuesta esperada | El usuario completa el registro sin ayuda y cada error de validación se muestra junto al campo que lo originó. |
| Medida de la respuesta | 90% de los usuarios de prueba completan el registro en ≤ 5 minutos sin asistencia. |
| Objetivo de negocio | Aumentar la proporción de registros completados. |

### **EAC-10: Búsqueda de tutores con filtros**

| Campo | Contenido |
| --- | --- |
| Atributo de calidad | Usabilidad |
| Escenario crudo | Un estudiante busca un tutor de una materia específica con determinada experiencia y universidad. |
| Estímulo | Aplicación de filtros en la búsqueda avanzada (HU-16). |
| Fuente del estímulo | Estudiante. |
| Entorno | Primer uso, sin capacitación previa. |
| Artefacto | Interfaz de búsqueda avanzada de tutores. |
| Respuesta esperada | El estudiante encuentra un tutor que cumple sus criterios sin ayuda. |
| Medida de la respuesta | 90% de los usuarios de prueba encuentran un tutor que cumple 3 criterios en ≤ 60 segundos y ≤ 5 clics. |
| Objetivo de negocio | Ofrecer una experiencia de búsqueda agradable que impulse el agendamiento de sesiones. |

### **EAC-11: Exactitud de promedios de calificación**

| Campo | Contenido |
| --- | --- |
| Atributo de calidad | Confiabilidad (exactitud de los datos) |
| Escenario crudo | El administrador consulta los promedios de tutores y estudiantes justo después de que se registran nuevas calificaciones. |
| Estímulo | Solicitud de los reportes consolidados de calificación (HU-38, HU-39, HU-40). |
| Fuente del estímulo | Administrador. |
| Entorno | Operación normal con calificaciones registradas hasta el momento de la consulta. |
| Artefacto | Módulo de reportes y registro de calificaciones. |
| Respuesta esperada | Cada promedio se calcula con todas las calificaciones de sesiones en estado Atendido. |
| Medida de la respuesta | 100% de los promedios coinciden con el cálculo manual, expresados con 2 decimales en escala de 0 a 5. |
| Objetivo de negocio | Evaluar la calidad del servicio con datos confiables. |

## 4. Matriz de trazabilidad HU → RF / RR / EAC

| HU | Subcaso de uso | RF | RR | EAC |
| --- | --- | --- | --- | --- |
| HU-01 | CU1-1 | RF-1-1-1 a RF-1-1-6 | RR-3, RR-9 | EAC-9 |
| HU-02 | CU1-2 | RF-1-2-1 a RF-1-2-7 | RR-3, RR-9 | EAC-9 |
| HU-03 | CU1-3 | RF-1-3-1 a RF-1-3-5 | RR-9 | EAC-5 |
| HU-04 | CU1-4 | RF-1-4-1 a RF-1-4-4 | RR-3, RR-7, RR-8 | EAC-6 |
| HU-05 | CU4-1 | RF-4-1-1 a RF-4-1-4 | RR-5 | EAC-7 |
| HU-06 | CU4-2 | RF-4-2-1 a RF-4-2-4 | RR-5 | EAC-7 |
| HU-07 | CU4-3 | RF-4-3-1 a RF-4-3-5 | RR-5 | EAC-5, EAC-7 |
| HU-08 | CU4-4 | RF-4-4-1 a RF-4-4-4 | RR-14 | EAC-3, EAC-11 |
| HU-09 | CU4-5 | RF-4-5-1 a RF-4-5-3 | — | — |
| HU-10 | CU3-1 | RF-3-1-1 a RF-3-1-3 | RR-12 | — |
| HU-11 | CU3-2 | RF-3-2-1 a RF-3-2-3 | RR-12 | — |
| HU-12 | CU3-3 | RF-3-3-1 a RF-3-3-4 | RR-11 | — |
| HU-13 | CU3-4 | RF-3-4-1 a RF-3-4-4 | RR-5, RR-11 | EAC-7, EAC-8 |
| HU-14 | CU3-5 | RF-3-5-1 a RF-3-5-3 | RR-11 | — |
| HU-15 | CU3-6 | RF-3-6-1 a RF-3-6-5 | RR-3, RR-9 | — |
| HU-16 | CU2-1 | RF-2-1-1 a RF-2-1-4 | — | EAC-1, EAC-10 |
| HU-17 | CU2-2 | RF-2-2-1 a RF-2-2-4 | — | — |
| HU-18 | CU2-3 | RF-2-3-1 a RF-2-3-7 | RR-11 | EAC-2 |
| HU-19 | CU2-4 | RF-2-4-1 a RF-2-4-4 | RR-11 | EAC-8 |
| HU-20 | CU2-5 | RF-2-5-1 a RF-2-5-2 | RR-11 | — |
| HU-21 | CU2-6 | RF-2-6-1 a RF-2-6-4 | RR-3, RR-9 | — |
| HU-22 | CU1-5 | RF-1-5-1 a RF-1-5-4 | RR-1 | EAC-9 |
| HU-23 | CU1-6 | RF-1-6-1 a RF-1-6-3 | RR-1 | EAC-9 |
| HU-24 | CU1-7 | RF-1-7-1 a RF-1-7-6 | RR-5, RR-6 | EAC-7 |
| HU-25 | CU2-7 | RF-2-7-1 a RF-2-7-3 | — | — |
| HU-26 | CU2-8 | RF-2-8-1 a RF-2-8-4 | — | EAC-4 |
| HU-27 | CU2-9 | RF-2-9-1 a RF-2-9-3 | RR-10, RR-11 | — |
| HU-28 | CU2-10 | RF-2-10-1 a RF-2-10-5 | RR-11, RR-13 | — |
| HU-29 | CU3-7 | RF-3-7-1 a RF-3-7-4 | RR-11 | — |
| HU-30 | CU3-8 | RF-3-8-1 a RF-3-8-3 | RR-10, RR-11 | — |
| HU-31 | CU3-9 | RF-3-9-1 a RF-3-9-4 | RR-11, RR-13 | — |
| HU-32 | CU4-6 | RF-4-6-1 a RF-4-6-3 | RR-1, RR-2, RR-5 | EAC-5, EAC-7 |
| HU-33 | CU4-7 | RF-4-7-1 a RF-4-7-3 | RR-1, RR-2, RR-5 | EAC-5, EAC-7 |
| HU-34 | CU4-8 | RF-4-8-1 a RF-4-8-3 | RR-9 | EAC-5 |
| HU-35 | CU4-9 | RF-4-9-1 a RF-4-9-4 | RR-9 | EAC-5 |
| HU-36 | CU4-10 | RF-4-10-1 a RF-4-10-4 | RR-13 | EAC-5 |
| HU-37 | CU4-11 | RF-4-11-1 a RF-4-11-4 | RR-13 | EAC-5 |
| HU-38 | CU4-12 | RF-4-12-1 a RF-4-12-4 | RR-10, RR-14 | EAC-3, EAC-11 |
| HU-39 | CU4-13 | RF-4-13-1 a RF-4-13-3 | RR-10, RR-14 | EAC-3, EAC-11 |
| HU-40 | CU4-14 | RF-4-14-1 a RF-4-14-3 | RR-14 | EAC-3, EAC-11 |
