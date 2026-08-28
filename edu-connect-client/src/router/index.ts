import { createRouter, createWebHistory } from 'vue-router'
import { routes } from './routes'
import { setupRouteGuards } from './guards'

export const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes
})

setupRouteGuards(router)

export * from './routes'
export * from './guards'
export default router
