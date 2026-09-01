// Default runtime config for local development (npm run dev)
// In Docker production, this file is dynamically generated on container startup by docker-entrypoint.sh
window.__ENV__ = window.__ENV__ || {
  VITE_API_URL: 'http://localhost:5000/api',
  VITE_APP_ENV: 'develop'
};
