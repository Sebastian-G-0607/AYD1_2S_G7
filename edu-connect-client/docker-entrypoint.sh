#!/bin/sh
set -e

# Generate env-config.js dynamically from environment variables
ENV_FILE="/usr/share/nginx/html/env-config.js"

echo "Generating runtime environment config in ${ENV_FILE}..."

cat <<EOF > "${ENV_FILE}"
window.__ENV__ = {
  VITE_API_URL: "${VITE_API_URL:-http://localhost:5000/api}",
  VITE_APP_ENV: "${VITE_APP_ENV:-production}",
  VITE_APP_NAME: "${VITE_APP_NAME:-EduConnect}",
  VITE_APP_VERSION: "${VITE_APP_VERSION:-1.0.0}"
};
EOF

# Ensure nginx non-root user can read the file
chmod 644 "${ENV_FILE}"

echo "Runtime config generated successfully:"
cat "${ENV_FILE}"

# Execute whatever command was passed to docker (typically nginx)
exec "$@"
