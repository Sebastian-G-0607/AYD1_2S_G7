-- =============================================================================
-- Datos de prueba del entorno E2E (docker-compose.e2e.yaml)
-- -----------------------------------------------------------------------------
-- Este script SOLO se ejecuta dentro del stack de pruebas: monta el contenedor
-- `db-seed`, que corre sqlplus contra la base de datos de la imagen de Oracle.
-- El backend de producción no se modifica ni se siembran usuarios de prueba.
--
-- Notas del esquema (Oracle, identificadores en minúsculas y entre comillas):
--   * usuarios.id es identidad (no se informa).
--   * usuarios.correo_validado = 1 evita que el login exija token de correo.
--   * estudiantes.usuario_id y tutores.usuario_id son la clave primaria (1:1).
--   * las horas se guardan como INTERVAL DAY TO SECOND (literal 'HH24:MI:SS' HOUR TO SECOND).
-- =============================================================================

WHENEVER SQLERROR EXIT FAILURE ROLLBACK
SET DEFINE OFF
SET FEEDBACK ON

-- -----------------------------------------------------------------------------
-- Estudiante de prueba (APROBADO y con el correo validado)
-- -----------------------------------------------------------------------------
INSERT INTO "usuarios" ("correo", "password_hash", "rol_id", "estado_id", "correo_validado")
SELECT 'estudiante.e2e@educonnect.com',
       '$2y$10$h32YuIaqdlHb.C/3ySrgzeHLBdPtk3wavvCwnZZCkN3dMkJdxZIJe',
       r."id",
       e."id",
       1
FROM "roles" r, "estados_usuarios" e
WHERE r."nombre" = 'Estudiante'
  AND e."nombre" = 'APROBADO'
  AND NOT EXISTS (
      SELECT 1 FROM "usuarios" u WHERE u."correo" = 'estudiante.e2e@educonnect.com'
  );

INSERT INTO "estudiantes" ("usuario_id", "nombre", "apellido", "carnet", "genero", "direccion", "telefono", "fecha_nacimiento", "documento_carnet_url")
SELECT u."id",
       'Carlos',
       'Mendoza',
       '202200123',
       'masculino',
       '10 Calle 5-45, Zona 1, Ciudad de Guatemala',
       '55551234',
       DATE '2001-03-15',
       'e2e/estudiante-carnet.pdf'
FROM "usuarios" u
WHERE u."correo" = 'estudiante.e2e@educonnect.com'
  AND NOT EXISTS (
      SELECT 1 FROM "estudiantes" e WHERE e."usuario_id" = u."id"
  );

-- -----------------------------------------------------------------------------
-- Tutor de prueba (APROBADO y con el correo validado)
-- -----------------------------------------------------------------------------
INSERT INTO "usuarios" ("correo", "password_hash", "rol_id", "estado_id", "correo_validado")
SELECT 'tutor.e2e@educonnect.com',
       '$2y$10$h32YuIaqdlHb.C/3ySrgzeHLBdPtk3wavvCwnZZCkN3dMkJdxZIJe',
       r."id",
       e."id",
       1
FROM "roles" r, "estados_usuarios" e
WHERE r."nombre" = 'Tutor'
  AND e."nombre" = 'APROBADO'
  AND NOT EXISTS (
      SELECT 1 FROM "usuarios" u WHERE u."correo" = 'tutor.e2e@educonnect.com'
  );

INSERT INTO "tutores" ("usuario_id", "nombre", "apellido", "carnet_id", "numero_identificacion", "genero", "direccion", "telefono", "fecha_nacimiento", "fotografia_url", "documento_cv_url", "direccion_tutoria", "anio_inicio", "universidad", "hora_inicio", "hora_fin")
SELECT u."id",
       'Sofia',
       'Ramirez',
       '201800987',
       '2589631470101',
       'femenino',
       'Boulevard Los Proceres 12-40, Zona 10, Ciudad de Guatemala',
       '55559876',
       DATE '1995-07-20',
       'e2e/tutor-fotografia.png',
       'e2e/tutor-cv.pdf',
       'Edificio T-3, Aula 201, Ciudad universitaria',
       2018,
       'Universidad de San Carlos de Guatemala',
       INTERVAL '08:00:00' HOUR TO SECOND,
       INTERVAL '17:00:00' HOUR TO SECOND
FROM "usuarios" u
WHERE u."correo" = 'tutor.e2e@educonnect.com'
  AND NOT EXISTS (
      SELECT 1 FROM "tutores" t WHERE t."usuario_id" = u."id"
  );

-- -----------------------------------------------------------------------------
-- Materias y días de atención del tutor de prueba
-- -----------------------------------------------------------------------------
INSERT INTO "tutores_materias" ("tutor_id", "materia_id")
SELECT t."usuario_id", m."id"
FROM "tutores" t, "materias" m
WHERE m."nombre" = 'Física'
  AND NOT EXISTS (
      SELECT 1 FROM "tutores_materias" tm
      WHERE tm."tutor_id" = t."usuario_id" AND tm."materia_id" = m."id"
  );

INSERT INTO "tutores_dias_atencion" ("tutor_id", "dia_semana")
SELECT t."usuario_id", d."dia"
FROM "tutores" t,
     (SELECT 1 AS "dia" FROM dual UNION ALL SELECT 2 FROM dual UNION ALL
      SELECT 3 FROM dual UNION ALL SELECT 4 FROM dual UNION ALL SELECT 5 FROM dual) d
WHERE NOT EXISTS (
    SELECT 1 FROM "tutores_dias_atencion" tda
    WHERE tda."tutor_id" = t."usuario_id" AND tda."dia_semana" = d."dia"
);

-- -----------------------------------------------------------------------------
-- Sesión atendida con plan de estudio (para la constancia en PDF del estudiante)
-- -----------------------------------------------------------------------------
INSERT INTO "sesiones" ("estudiante_id", "tutor_id", "materia_id", "estado_id", "fecha_sesion", "hora_inicio", "hora_fin", "motivo", "fecha_creacion")
SELECT ue."id",
       ut."id",
       m."id",
       es."id",
       CURRENT_TIMESTAMP - 7,
       INTERVAL '10:00:00' HOUR TO SECOND,
       INTERVAL '11:00:00' HOUR TO SECOND,
       'Repaso de Física: ejercicios de cinemática',
       CURRENT_TIMESTAMP - 7
FROM "usuarios" ue, "usuarios" ut, "materias" m, "estados_sesiones" es
WHERE ue."correo" = 'estudiante.e2e@educonnect.com'
  AND ut."correo" = 'tutor.e2e@educonnect.com'
  AND m."nombre" = 'Física'
  AND es."nombre" = 'ATENDIDA'
  AND NOT EXISTS (
      SELECT 1 FROM "sesiones" s WHERE s."estudiante_id" = ue."id"
  );

INSERT INTO "planes_estudio" ("sesion_id", "dificultades_identificadas", "fecha_creacion")
SELECT s."id",
       'Se le dificulta despejar variables y aplicar las leyes de Newton',
       CURRENT_TIMESTAMP - 7
FROM "sesiones" s, "usuarios" ue
WHERE s."estudiante_id" = ue."id"
  AND ue."correo" = 'estudiante.e2e@educonnect.com'
  AND NOT EXISTS (
      SELECT 1 FROM "planes_estudio" p WHERE p."sesion_id" = s."id"
  );

INSERT INTO "recursos_plan_estudio" ("plan_estudio_id", "nombre", "tipo", "descripcion_uso")
SELECT p."id", r."nombre", r."tipo", r."descripcion_uso"
FROM "planes_estudio" p
JOIN "sesiones" s ON s."id" = p."sesion_id"
JOIN "usuarios" u ON u."id" = s."estudiante_id"
CROSS JOIN (
    SELECT 'Guía de ejercicios' AS "nombre", 'PDF' AS "tipo", 'Resolver los ejercicios del 1 al 5' AS "descripcion_uso" FROM dual
    UNION ALL
    SELECT 'Video de apoyo', 'VIDEO', 'Repasar el ejemplo de fuerza y aceleración' FROM dual
) r
WHERE u."correo" = 'estudiante.e2e@educonnect.com'
  AND NOT EXISTS (
      SELECT 1 FROM "recursos_plan_estudio" rp WHERE rp."plan_estudio_id" = p."id"
  );

COMMIT;
EXIT;
