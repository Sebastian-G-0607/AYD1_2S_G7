import type { Router } from 'vue-router'

function getHomePathForRole(role: string): string | null {
  const normalized = role.toLowerCase().trim()
  if (normalized.includes('admin')) {
    return '/admin/aprobaciones'
  }
  if (normalized === 'tutor') {
    return '/tutor/dashboard'
  }
  if (normalized === 'estudiante' || normalized === 'student') {
    return '/estudiante/explorar-tutores'
  }
  return null
}

function getUserRoleFromStorage(): string {
  const rawUser = localStorage.getItem('edu_auth_user')
  if (!rawUser) return ''
  try {
    const user = JSON.parse(rawUser) as { rol?: string }
    return user.rol || ''
  } catch {
    return ''
  }
}

function clearCorruptStorage(): void {
  localStorage.removeItem('edu_auth_token')
  localStorage.removeItem('edu_auth_user')
}

export function setupRouteGuards(router: Router): void {
  router.beforeEach((to, _from, next) => {
    const token = localStorage.getItem('edu_auth_token')
    const isAuthenticated = Boolean(token)
    const userRole = getUserRoleFromStorage()
    const homePath = getHomePathForRole(userRole)

    if (to.meta.title && typeof to.meta.title === 'string') {
      document.title = to.meta.title
    } else {
      document.title = 'EduConnect'
    }

    if (isAuthenticated && !homePath) {
      clearCorruptStorage()
      if (to.name === 'login' || to.path === '/login') {
        next()
        return
      }
      next({ name: 'login' })
      return
    }

    if (to.meta.requiresAuth && !isAuthenticated) {
      next({ name: 'login', query: { redirect: to.fullPath } })
      return
    }

    if (to.meta.guestOnly && isAuthenticated && homePath) {
      if (to.path !== homePath) {
        next({ path: homePath })
        return
      }
      next()
      return
    }

    if (to.meta.roles && Array.isArray(to.meta.roles) && to.meta.roles.length > 0 && homePath) {
      const allowedRoles = to.meta.roles.map(r => String(r).toLowerCase().trim())
      const isAllowed = allowedRoles.some(r => userRole.toLowerCase().trim().includes(r))
      if (!isAllowed) {
        if (to.path !== homePath) {
          next({ path: homePath })
          return
        }
      }
    }

    next()
  })
}
