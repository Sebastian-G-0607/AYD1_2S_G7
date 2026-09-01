import type { Router } from 'vue-router'

export function setupRouteGuards(router: Router): void {
  router.beforeEach((to, _from, next) => {
    const token = localStorage.getItem('edu_auth_token')
    const isAuthenticated = Boolean(token)

    if (to.meta.title && typeof to.meta.title === 'string') {
      document.title = to.meta.title
    } else {
      document.title = 'EduConnect'
    }

    if (to.meta.requiresAuth && !isAuthenticated) {
      next({ name: 'login', query: { redirect: to.fullPath } })
      return
    }

    if (to.meta.guestOnly && isAuthenticated) {
      next({ path: '/' })
      return
    }

    next()
  })
}
