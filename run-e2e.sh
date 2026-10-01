#!/bin/bash
set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

if [ "$1" == "--down" ]; then
  echo "Deteniendo y eliminando entorno de pruebas..."
  docker compose -f docker-compose.e2e.yaml down -v --remove-orphans
  exit 0
fi

cleanup() {
  local exit_code=$?
  trap - EXIT INT TERM
  set +e
  echo "=========================================================="
  echo "Deteniendo y eliminando entorno de pruebas..."
  echo "=========================================================="
  docker compose -f docker-compose.e2e.yaml down -v --remove-orphans
  if [ -n "$SHOW_REPORT" ] && [ "$SHOW_REPORT" = true ]; then
    echo "El reporte HTML está disponible en: ./e2e/playwright-report/index.html"
    if command -v xdg-open >/dev/null 2>&1 && [ -f "./e2e/playwright-report/index.html" ]; then
      xdg-open "./e2e/playwright-report/index.html" >/dev/null 2>&1 &
    fi
  fi
  exit $exit_code
}

trap cleanup EXIT INT TERM

if [ -n "$DISPLAY" ] && command -v xhost >/dev/null 2>&1; then
  xhost +local:root >/dev/null 2>&1 || true
  xhost +local:docker >/dev/null 2>&1 || true
fi

if [ "$1" == "--ui" ]; then
  echo "=========================================================="
  echo "Iniciando EduConnect y Playwright UI Web en Docker..."
  echo "=========================================================="
  
  docker compose -f docker-compose.e2e.yaml up -d oracle-db api frontend
  
  echo "Esperando que el frontend responda en http://localhost:5173..."
  timeout 60s bash -c 'until curl -s http://localhost:5173 > /dev/null; do sleep 2; done' || true

  echo "Abriendo Playwright UI en: http://localhost:9323"
  
  (sleep 3 && { command -v xdg-open >/dev/null && xdg-open http://localhost:9323 || true; }) &

  EXIT_CODE=0
  docker compose -f docker-compose.e2e.yaml run --rm --service-ports e2e \
    npx playwright test --ui-port=9323 --ui-host=0.0.0.0 || EXIT_CODE=$?
  exit $EXIT_CODE

elif [ "$1" == "--headed" ]; then
  echo "=========================================================="
  echo "Ejecutando pruebas con ventana gráfica en tu escritorio..."
  echo "=========================================================="
  
  docker compose -f docker-compose.e2e.yaml up -d oracle-db api frontend
  
  echo "Esperando que el frontend responda en http://localhost:5173..."
  timeout 60s bash -c 'until curl -s http://localhost:5173 > /dev/null; do sleep 2; done' || true

  EXIT_CODE=0
  docker compose -f docker-compose.e2e.yaml run --rm e2e \
    npx playwright test --headed || EXIT_CODE=$?
  exit $EXIT_CODE

else
  echo "=========================================================="
  echo "Ejecutando pruebas E2E Full Docker..."
  echo "=========================================================="

  SHOW_REPORT=true
  EXIT_CODE=0
  docker compose -f docker-compose.e2e.yaml up --build --abort-on-container-exit --exit-code-from e2e || EXIT_CODE=$?
  exit $EXIT_CODE
fi
