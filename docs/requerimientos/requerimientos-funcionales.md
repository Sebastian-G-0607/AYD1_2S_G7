# Requerimientos Funcionales

Los siguientes requerimientos funcionales describen las principales acciones y comportamientos que debe proporcionar la plataforma EduConnect para permitir la gestión de usuarios, tutores, sesiones académicas y procesos administrativos.

---

## Módulo de Registro y Autenticación

### RF-001 - Registrar estudiante
**Código:** RF-001  
**Descripción:** El sistema debe permitir que un estudiante se registre proporcionando la información personal, académica y de acceso solicitada por la plataforma.  
**Actores involucrados:** Estudiante

### RF-002 - Registrar tutor
**Código:** RF-002  
**Descripción:** El sistema debe permitir que un tutor se registre proporcionando sus datos personales, información académica, especialidades, dirección de tutoría y credenciales de acceso.  
**Actores involucrados:** Tutor

### RF-003 - Autenticar estudiante y tutor
**Código:** RF-003  
**Descripción:** El sistema debe permitir el inicio de sesión de estudiantes y tutores mediante correo electrónico y contraseña únicamente cuando su cuenta haya sido aprobada por un administrador.  
**Actores involucrados:** Estudiante, Tutor

### RF-004 - Autenticar administrador
**Código:** RF-004  
**Descripción:** El sistema debe permitir que el administrador inicie sesión mediante sus credenciales y complete una segunda autenticación utilizando el archivo de validación establecido por el sistema.  
**Actores involucrados:** Administrador

---

## Módulo Estudiante

### RF-005 - Visualizar tutores disponibles
**Código:** RF-005  
**Descripción:** El sistema debe mostrar al estudiante los tutores disponibles en la plataforma junto con su nombre, especialidad, dirección de tutoría y fotografía.  
**Actores involucrados:** Estudiante

### RF-006 - Buscar tutores
**Código:** RF-006  
**Descripción:** El sistema debe permitir al estudiante buscar y filtrar tutores según materia, años de experiencia, género, edad y universidad.  
**Actores involucrados:** Estudiante

### RF-007 - Consultar horario del tutor
**Código:** RF-007  
**Descripción:** El sistema debe permitir al estudiante consultar los días y horarios de atención configurados por un tutor, incluyendo los espacios ocupados y disponibles.  
**Actores involucrados:** Estudiante

### RF-008 - Programar sesión de tutoría
**Código:** RF-008  
**Descripción:** El sistema debe permitir al estudiante programar una sesión indicando fecha, hora, materia y motivo de la tutoría.  
**Actores involucrados:** Estudiante

### RF-009 - Validar disponibilidad de sesión
**Código:** RF-009  
**Descripción:** El sistema debe comprobar que la fecha seleccionada corresponda a un día de atención del tutor y que el horario solicitado se encuentre disponible.  
**Actores involucrados:** Estudiante

### RF-010 - Evitar conflictos de horario
**Código:** RF-010  
**Descripción:** El sistema debe impedir que un estudiante programe sesiones que coincidan o se traslapen con otras sesiones previamente registradas.  
**Actores involucrados:** Estudiante

### RF-011 - Visualizar sesiones activas
**Código:** RF-011  
**Descripción:** El sistema debe permitir al estudiante consultar las sesiones que se encuentran pendientes de ser atendidas, mostrando fecha, hora, tutor, materia, dirección y motivo.  
**Actores involucrados:** Estudiante

### RF-012 - Cancelar sesión
**Código:** RF-012  
**Descripción:** El sistema debe permitir al estudiante cancelar una sesión activa, solicitando confirmación antes de realizar la acción y eliminándola posteriormente de la lista de sesiones activas.  
**Actores involucrados:** Estudiante

---

## Módulo Tutor

### RF-013 - Visualizar sesiones pendientes
**Código:** RF-013  
**Descripción:** El sistema debe permitir al tutor visualizar las sesiones pendientes de atención, ordenadas por fecha y mostrando la información correspondiente del estudiante y de la tutoría.  
**Actores involucrados:** Tutor

### RF-014 - Marcar sesión como atendida
**Código:** RF-014  
**Descripción:** El sistema debe permitir al tutor marcar una sesión como atendida después de ingresar un resumen de la tutoría realizada y las recomendaciones correspondientes.  
**Actores involucrados:** Tutor

### RF-015 - Cancelar sesión de estudiante
**Código:** RF-015  
**Descripción:** El sistema debe permitir al tutor cancelar una sesión pendiente cuando tenga algún inconveniente con la fecha u horario establecido.  
**Actores involucrados:** Tutor

### RF-016 - Notificar cancelación de sesión
**Código:** RF-016  
**Descripción:** El sistema debe enviar un correo electrónico al estudiante cuando el tutor cancele una sesión, indicando los datos principales de la tutoría cancelada.  
**Actores involucrados:** Tutor, Estudiante

### RF-017 - Configurar horarios de atención
**Código:** RF-017  
**Descripción:** El sistema debe permitir al tutor seleccionar los días de atención y establecer el rango horario en el que brindará tutorías, así como actualizar dicha disponibilidad cuando sea necesario.  
**Actores involucrados:** Tutor

---

## Módulo Administrador

### RF-018 - Aprobar o rechazar usuarios
**Código:** RF-018  
**Descripción:** El sistema debe permitir al administrador revisar las solicitudes de estudiantes y tutores pendientes y aprobarlas o rechazarlas, notificando posteriormente al usuario sobre la decisión.  
**Actores involucrados:** Administrador, Estudiante, Tutor

### RF-019 - Gestionar estudiantes y tutores
**Código:** RF-019  
**Descripción:** El sistema debe permitir al administrador visualizar los estudiantes y tutores registrados y dar de baja a un usuario cuando exista un motivo válido.  
**Actores involucrados:** Administrador

### RF-020 - Generar reportes
**Código:** RF-020  
**Descripción:** El sistema debe permitir al administrador generar al menos dos reportes con información relevante sobre el uso de la plataforma, como los tutores con mayor cantidad de estudiantes atendidos o las materias con mayor demanda.  
**Actores involucrados:** Administrador