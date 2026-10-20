#!/bin/sh
set -e

DB_USER="playwright"
DB_PASS="ApiDev1234"
DB_URL="${DB_USER}/${DB_PASS}@oracle-db:1521/XEPDB1"

echo "[seed] Esperando a que la base de datos acepte conexiones..."
until echo "select 1 from dual" | sqlplus -s "$DB_URL" > /dev/null 2>&1; do
  sleep 3
done

echo "[seed] Esperando a que el backend aplique las migraciones y los catálogos..."
until echo "select count(*) from \"roles\" r, \"estados_usuarios\" e where r.\"nombre\" = 'Estudiante' and e.\"nombre\" = 'APROBADO';" \
  | sqlplus -s "$DB_URL" 2>/dev/null | grep -q "^[[:space:]]*1[[:space:]]*$"; do
  sleep 3
done

echo "[seed] Insertando los datos de prueba..."
sqlplus -s "$DB_URL" @/seed/seed-e2e.sql

echo "[seed] Datos de prueba insertados."
