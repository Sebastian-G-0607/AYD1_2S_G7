import type { RouteRecordRaw } from 'vue-router'

export const routes: RouteRecordRaw[] = [
  {
    path: '/',
    redirect: '/login'
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
  }
]

export default routes
