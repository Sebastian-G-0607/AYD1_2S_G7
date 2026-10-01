# DIAGRAMA ENTIDAD RELACIÓN DEL NEGOCIO

```mermaid
erDiagram
    roles ||--o{ usuarios : "tiene"
    estados_usuarios ||--o{ usuarios : "tiene"
    
    usuarios ||--o| administradores : "es"
    usuarios ||--o| estudiantes : "es"
    usuarios ||--o| tutores : "es"
    usuarios ||--|| token_correo : "genera"

    tutores ||--o{ tutores_dias_atencion : "dispone_de"
    tutores ||--o{ tutores_materias : "ensenia"
    materias ||--o{ tutores_materias : "asignada_a"

    estudiantes ||--o{ sesiones : "solicita"
    tutores ||--o{ sesiones : "imparte"
    materias ||--o{ sesiones : "sobre"
    estados_sesiones ||--o{ sesiones : "define_estado"

    %% Planes de estudio y recursos
    sesiones ||--o| planes_estudio : "genera"
    planes_estudio ||--|{ recursos_plan_estudio : "contiene"

    %% Calificaciones mutuas
    sesiones ||--o| calificaciones_tutores : "evaluada_por_estudiante"
    sesiones ||--o| calificaciones_estudiantes : "evaluada_por_tutor"

    %% Reportes de incidencias
    categorias_reportes_tutores ||--o{ reportes_tutores : "clasifica"
    sesiones ||--o{ reportes_tutores : "origina"

    categorias_reportes_estudiantes ||--o{ reportes_estudiantes : "clasifica"
    sesiones ||--o{ reportes_estudiantes : "origina"

    roles {
        int id PK
        varchar nombre UK
        varchar descripcion
    }

    estados_usuarios {
        int id PK
        varchar nombre UK
        varchar descripcion
    }

    estados_sesiones {
        int id PK
        varchar nombre UK
        varchar descripcion
    }

    usuarios {
        int id PK
        varchar correo UK
        varchar password_hash
        int rol_id FK
        int estado_id FK
        timestamp fecha_registro
        bit correo_validado
        timestamp fecha_baja
        text motivo_baja
    }

    token_correo {
        int id PK
        int usuario_id
        varchar token
        datetime fecha_generacion
        datetime fecha_expiracion
        bit revocado
    }

    administradores {
        int usuario_id PK, FK
        varchar password_fase2_hash
    }

    estudiantes {
        int usuario_id PK, FK
        varchar nombre
        varchar apellido
        varchar carnet UK
        varchar genero
        varchar direccion
        varchar telefono
        date fecha_nacimiento
        varchar fotografia_url
        varchar documento_carnet_url
    }

    tutores {
        int usuario_id PK, FK
        varchar nombre
        varchar apellido
        varchar carnet_id UK
        varchar numero_identificacion UK
        varchar genero
        varchar direccion
        varchar telefono
        date fecha_nacimiento
        varchar fotografia_url
        varchar documento_cv_url
        varchar direccion_tutoria
        int anio_inicio
        varchar universidad
        time hora_inicio
        time hora_fin
    }

    tutores_dias_atencion {
        int id PK
        int tutor_id FK
        int dia_semana
    }

    materias {
        int id PK
        varchar nombre UK
    }

    tutores_materias {
        int tutor_id PK, FK
        int materia_id PK, FK
    }

    sesiones {
        int id PK
        int estudiante_id FK
        int tutor_id FK
        int materia_id FK
        int estado_id FK
        date fecha_sesion
        time hora_inicio
        time hora_fin
        text motivo
        text resumen
        text motivo_cancelacion
        timestamp fecha_creacion
    }

    planes_estudio {
        int id PK
        int sesion_id FK, UK
        text dificultades_identificadas
        timestamp fecha_creacion
    }

    recursos_plan_estudio {
        int id PK
        int plan_estudio_id FK
        varchar nombre
        varchar tipo
        text descripcion_uso
    }

    calificaciones_tutores {
        int id PK
        int sesion_id FK, UK
        int estrellas
        text comentario
        timestamp fecha_creacion
    }

    calificaciones_estudiantes {
        int id PK
        int sesion_id FK, UK
        int estrellas
        text comentario
        timestamp fecha_creacion
    }

    categorias_reportes_tutores {
        int id PK
        varchar nombre UK
        varchar descripcion
    }

    reportes_tutores {
        int id PK
        int sesion_id FK
        int categoria_id FK
        text motivo
        timestamp fecha_reporte
        varchar estado
    }

    categorias_reportes_estudiantes {
        int id PK
        varchar nombre UK
        varchar descripcion
    }

    reportes_estudiantes {
        int id PK
        int sesion_id FK
        int categoria_id FK
        text motivo
        timestamp fecha_reporte
        varchar estado
    }
```

---

## Consideraciones de Diseño y Normalización (3FN)

1. **Cumplimiento de 3FN en `planes_estudio`**:
   - Para evitar redundancias y dependencias transitivas (`plan_estudio -> sesion -> tutor -> datos_tutor`), no se duplican en la entidad los campos `nombre_tutor`, `numero_identificacion_tutor`, `especialidad_tutor` ni `fecha_ultima_sesion`.
   - Toda esta información se obtiene mediante consultas relacionales (`JOIN`) con las tablas `sesiones`, `tutores` y `materias` a través de la clave foránea `sesion_id`.
2. **Cardinalidad de `recursos_plan_estudio` (1:N)**:
   - Cumple con la Primera Forma Normal (1FN) al modelar los recursos recomendados (nombre, tipo y descripción de uso) en una entidad dependiente, permitiendo múltiples recursos por plan de estudio sin almacenar estructuras compuestas o listas en una sola columna.
3. **Calificaciones Mutuas**:
   - `calificaciones_tutores`: Almacena la puntuación (0-5 estrellas) y comentario opcional emitidos por el estudiante hacia el tutor tras la sesión atendida.
   - `calificaciones_estudiantes`: Almacena la puntuación (0-5 estrellas) y comentario emitidos por el tutor evaluando al estudiante en la sesión.
4. **Reportes de Conducta e Incidencias**:
   - Los catálogos `categorias_reportes_tutores` y `categorias_reportes_estudiantes` almacenan las listas predefinidas exigidas por el negocio.
   - Las entidades `reportes_tutores` y `reportes_estudiantes` quedan vinculadas a la sesión origen (`sesion_id`) y a la categoría correspondiente (`categoria_id`), manteniendo el motivo y estado para seguimiento por parte del administrador.