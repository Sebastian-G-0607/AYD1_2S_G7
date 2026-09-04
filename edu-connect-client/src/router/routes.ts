import type { RouteRecordRaw } from 'vue-router'

export const routes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'root',
    redirect: () => {
      const token = localStorage.getItem('edu_auth_token')
      if (!token) return '/login'
      const rawUser = localStorage.getItem('edu_auth_user')
      if (!rawUser) return '/login'
      try {
        const user = JSON.parse(rawUser) as { rol?: string }
        const role = user.rol?.toLowerCase().trim() || ''
        if (role.includes('admin')) return '/admin/aprobaciones'
        if (role === 'tutor') return '/tutor/dashboard'
        if (role === 'estudiante' || role === 'student') return '/estudiante/explorar-tutores'
        return '/login'
      } catch {
        return '/login'
      }
    }
  },
  {
    path: '/login',
    name: 'login',
    component: () => import('@/pages/LoginPage.vue'),
    meta: {
      guestOnly: true,
      requiresAuth: false,
      title: 'Iniciar Sesión - EduConnect',
      layout: 'auth'
    }
  },
  {
    path: '/register/student',
    name: 'register-student',
    component: () => import('@/pages/StudentRegisterPage.vue'),
    meta: {
      guestOnly: true,
      requiresAuth: false,
      title: 'Registro de Estudiante - EduConnect',
      layout: 'auth'
    }
  },
  {
    path: '/register/tutor',
    name: 'register-tutor',
    component: () => import('@/pages/TutorRegisterPage.vue'),
    meta: {
      guestOnly: true,
      requiresAuth: false,
      title: 'Registro de Tutor - EduConnect',
      layout: 'auth'
    }
  },
  {
    path: '/admin/aprobaciones',
    name: 'admin-approvals',
    component: () => import('@/pages/AdminApprovalsPage.vue'),
    meta: {
      requiresAuth: true,
      guestOnly: false,
      roles: ['Administrador', 'Admin'],
      title: 'Panel de Aprobaciones - EduConnect Admin',
      layout: 'dashboard'
    }
  },
  {
    path: '/admin/2fa',
    name: 'admin-2fa',
    component: () => import('@/pages/AdminTwoFactorPage.vue'),
    meta: {
      requiresAuth: false,
      guestOnly: true,
      title: 'Verificación Administrador - EduConnect',
      layout: 'auth'
    }
  },
  {
    path: '/estudiante/explorar-tutores',
    name: 'student-tutors-explorer',
    component: () => import('@/pages/StudentTutorsExplorerPage.vue'),
    meta: {
      requiresAuth: true,
      guestOnly: false,
      roles: ['Estudiante', 'Student'],
      title: 'Explorador de Tutores - EduConnect Estudiante',
      layout: 'dashboard'
    }
  },
  {
    path: '/tutor/dashboard',
    name: 'tutor-dashboard',
    component: () => import('@/pages/TutorDashboardPage.vue'),
    meta: {
      requiresAuth: true,
      guestOnly: false,
      roles: ['Tutor'],
      title: 'Dashboard Principal - Tutor EduConnect',
      layout: 'dashboard'
    }
  },
  {
    path: '/:pathMatch(.*)*',
    redirect: '/'
  }
]

export default routes
