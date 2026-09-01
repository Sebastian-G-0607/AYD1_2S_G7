<script setup lang="ts">
import { ref, computed } from 'vue'
import { RouterLink, useRoute } from 'vue-router'
import { useAuthStore } from '@/features/auth/store'
import { useAuth } from '@/features/auth/composables/useAuth'

interface NavItem {
  name: string
  icon: string
  to: string
}

const route = useRoute()
const authStore = useAuthStore()
const { logout } = useAuth()

const isMobileMenuOpen = ref(false)

const userRole = computed(() => {
  return authStore.userRole?.toLowerCase() || ''
})

const userDisplayName = computed(() => {
  if (authStore.user?.nombre) {
    return `${authStore.user.nombre} ${authStore.user.apellido || ''}`.trim()
  }
  if (userRole.value.includes('admin')) return 'Admin EduConnect'
  if (userRole.value === 'tutor') return 'Prof. Tutor'
  if (userRole.value === 'estudiante') return 'Estudiante'
  return authStore.user?.correo || 'Usuario'
})

const userRoleDisplay = computed(() => {
  if (userRole.value.includes('admin')) return 'Super Administrador'
  if (userRole.value === 'tutor') return 'Tutor Académico'
  if (userRole.value === 'estudiante') return 'Estudiante'
  return authStore.userRole || 'Usuario'
})

const userInitials = computed(() => {
  if (authStore.user?.nombre) {
    const first = authStore.user.nombre.charAt(0)
    const second = authStore.user.apellido ? authStore.user.apellido.charAt(0) : ''
    return (first + second).toUpperCase()
  }
  if (userRole.value.includes('admin')) return 'AD'
  if (userRole.value === 'tutor') return 'TU'
  if (userRole.value === 'estudiante') return 'ES'
  return 'EC'
})

const portalSubtitle = computed(() => {
  if (userRole.value.includes('admin')) return 'Panel de Administración'
  if (userRole.value === 'tutor') return 'Portal del Tutor'
  if (userRole.value === 'estudiante') return 'Portal del Estudiante'
  return 'EduConnect'
})

const navItems = computed<NavItem[]>(() => {
  if (userRole.value.includes('admin')) {
    return [
      { name: 'Aprobaciones', icon: 'verified_user', to: '/admin/aprobaciones' },
      { name: 'Usuarios', icon: 'group', to: '/admin/usuarios' },
      { name: 'Reportes', icon: 'bar_chart', to: '/admin/reportes' }
    ]
  }

  if (userRole.value === 'tutor') {
    return [
      { name: 'Dashboard', icon: 'home', to: '/tutor/dashboard' },
      { name: 'Horarios', icon: 'schedule', to: '/tutor/horarios' },
      { name: 'Historial', icon: 'assignment', to: '/tutor/historial' },
      { name: 'Mi Perfil', icon: 'person', to: '/tutor/mi-perfil' }
    ]
  }

  return [
    { name: 'Explorar Tutores', icon: 'search', to: '/estudiante/explorar-tutores' },
    { name: 'Mis Sesiones', icon: 'event_available', to: '/estudiante/mis-sesiones' },
    { name: 'Historial', icon: 'history_edu', to: '/estudiante/historial' },
    { name: 'Mi Perfil', icon: 'person_outline', to: '/estudiante/mi-perfil' }
  ]
})

function isRouteActive(itemPath: string): boolean {
  if (itemPath === route.path) return true
  if (itemPath !== '/' && route.path.startsWith(itemPath)) return true
  return false
}

function toggleMobileMenu() {
  isMobileMenuOpen.value = !isMobileMenuOpen.value
}

function closeMobileMenu() {
  isMobileMenuOpen.value = false
}

async function handleLogout() {
  await logout()
}
</script>

<template>
  <div class="min-h-screen bg-surface font-body text-on-surface flex flex-col">
    <div
      v-if="isMobileMenuOpen"
      class="fixed inset-0 bg-on-background/40 backdrop-blur-sm z-40 lg:hidden"
      @click="closeMobileMenu"
    />

    <aside
      :class="[
        'fixed top-0 bottom-0 left-0 w-72 bg-surface-container-lowest z-50 flex flex-col border-r border-outline-variant/30 shadow-[4px_0_12px_rgba(30,41,59,0.03)] transition-transform duration-300 ease-in-out',
        isMobileMenuOpen ? 'translate-x-0' : '-translate-x-full lg:translate-x-0'
      ]"
    >
      <div
        class="px-8 py-6 flex items-center justify-between relative border-b border-surface-container/60"
      >
        <div class="flex items-center gap-3">
          <div
            class="w-9 h-9 rounded-xl bg-primary flex items-center justify-center text-on-primary font-bold shadow-sm"
          >
            <span class="material-symbols-outlined text-[20px]">school</span>
          </div>
          <div class="flex flex-col">
            <span class="font-headline text-lg font-bold text-primary tracking-tight leading-none"
              >EduConnect</span
            >
            <span
              class="text-[11px] font-medium text-on-surface-variant uppercase tracking-wider mt-1"
              >{{ userRoleDisplay }}</span
            >
          </div>
        </div>

        <button
          type="button"
          aria-label="Cerrar menú"
          class="lg:hidden text-on-surface-variant hover:text-primary transition-colors p-1.5 rounded-lg hover:bg-surface-container-low"
          @click="closeMobileMenu"
        >
          <span class="material-symbols-outlined text-[20px]">close</span>
        </button>
      </div>

      <nav class="flex-1 px-4 py-6 space-y-1.5 overflow-y-auto">
        <RouterLink
          v-for="item in navItems"
          :key="item.to"
          :to="item.to"
          :class="[
            'flex items-center gap-3 px-4 py-3 rounded-xl transition-all font-medium text-sm',
            isRouteActive(item.to)
              ? 'bg-primary-container text-white font-semibold shadow-sm'
              : 'text-on-surface-variant hover:bg-surface-container-low hover:text-on-surface'
          ]"
          @click="closeMobileMenu"
        >
          <span class="material-symbols-outlined text-[22px]">{{ item.icon }}</span>
          <span>{{ item.name }}</span>
        </RouterLink>
      </nav>

      <div class="p-4 border-t border-surface-container/60 flex flex-col gap-2">
        <button
          type="button"
          class="w-full flex items-center gap-3 px-4 py-2.5 rounded-xl text-error hover:bg-error-container/40 transition-colors font-medium text-sm"
          @click="handleLogout"
        >
          <span class="material-symbols-outlined text-[20px]">logout</span>
          <span>Cerrar Sesión</span>
        </button>
      </div>
    </aside>

    <div class="lg:pl-72 flex-1 flex flex-col min-w-0">
      <header
        class="sticky top-0 z-30 h-20 bg-surface/85 backdrop-blur-xl border-b border-outline-variant/20 px-4 sm:px-8 flex items-center justify-between shadow-[0_1px_8px_rgba(0,0,0,0.03)]"
      >
        <div class="flex items-center gap-4">
          <button
            type="button"
            aria-label="Abrir menú"
            class="lg:hidden p-2 rounded-lg text-on-surface-variant hover:text-primary hover:bg-surface-container-low transition-colors"
            @click="toggleMobileMenu"
          >
            <span class="material-symbols-outlined text-[24px]">menu</span>
          </button>

          <div class="flex items-center gap-2 text-on-surface-variant">
            <span class="material-symbols-outlined text-[20px]">school</span>
            <span class="text-sm font-semibold text-on-surface tracking-tight">{{
              portalSubtitle
            }}</span>
          </div>
        </div>

        <div class="flex items-center gap-4">
          <button
            type="button"
            aria-label="Notificaciones"
            class="p-2 rounded-full text-on-surface-variant hover:text-primary hover:bg-surface-container-low transition-colors relative"
          >
            <span class="material-symbols-outlined text-[22px]">notifications</span>
            <span class="absolute top-1.5 right-1.5 w-2 h-2 bg-error rounded-full" />
          </button>

          <div class="flex items-center gap-3 pl-4 border-l border-surface-container-high">
            <div class="hidden sm:flex flex-col text-right">
              <span class="text-sm font-semibold text-on-surface leading-tight">{{
                userDisplayName
              }}</span>
              <span class="text-xs text-on-surface-variant">{{ userRoleDisplay }}</span>
            </div>

            <div
              class="w-10 h-10 rounded-full bg-primary flex items-center justify-center text-on-primary font-bold text-sm shadow-sm overflow-hidden flex-shrink-0"
            >
              <img
                v-if="authStore.user?.fotografiaUrl"
                :src="authStore.user.fotografiaUrl"
                alt="Avatar de usuario"
                class="w-full h-full object-cover"
              />
              <span v-else>{{ userInitials }}</span>
            </div>
          </div>
        </div>
      </header>

      <main class="flex-1 p-4 sm:p-8 bg-surface">
        <slot />
      </main>
    </div>
  </div>
</template>
