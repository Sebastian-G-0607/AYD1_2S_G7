// Helper to retrieve runtime environment variables in production (injected by docker entrypoint)
// or Vite environment variables in local development.

export interface RuntimeEnv {
  VITE_API_URL?: string
  VITE_APP_ENV?: string
  VITE_APP_NAME?: string
  VITE_APP_VERSION?: string
}

declare global {
  interface Window {
    __ENV__?: RuntimeEnv
  }
}

export function getEnv(key: keyof RuntimeEnv, fallback = ''): string {
  // 1. In Vite development mode (pnpm run dev), prioritize import.meta.env (.env files)
  if (import.meta.env.DEV) {
    const metaEnv = import.meta.env[key]
    if (metaEnv !== undefined && metaEnv !== '') {
      return metaEnv as string
    }
  }

  // 2. In Production (Docker / Nginx), use window.__ENV__ generated dynamically at runtime
  if (typeof window !== 'undefined' && window.__ENV__ && window.__ENV__[key] !== undefined && window.__ENV__[key] !== '') {
    return window.__ENV__[key] as string
  }

  // 3. Fallback to any remaining Vite build-time env
  const metaEnv = import.meta.env[key]
  if (metaEnv !== undefined && metaEnv !== '') {
    return metaEnv as string
  }

  return fallback
}
