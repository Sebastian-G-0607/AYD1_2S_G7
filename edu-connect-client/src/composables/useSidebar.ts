import { ref } from 'vue'

function getInitialDesktopSidebarState(): boolean {
  try {
    if (typeof window !== 'undefined') {
      return localStorage.getItem('edu_sidebar_open') !== 'false'
    }
  } catch {
    return true
  }
  return true
}

function persistDesktopSidebarState(value: boolean): void {
  try {
    if (typeof window !== 'undefined') {
      localStorage.setItem('edu_sidebar_open', String(value))
    }
  } catch {
    return
  }
}

const isDesktopSidebarOpen = ref(getInitialDesktopSidebarState())
const isMobileMenuOpen = ref(false)

export function useSidebar() {
  function toggleMobileMenu(): void {
    isMobileMenuOpen.value = !isMobileMenuOpen.value
  }

  function closeMobileMenu(): void {
    isMobileMenuOpen.value = false
  }

  function toggleDesktopSidebar(): void {
    isDesktopSidebarOpen.value = !isDesktopSidebarOpen.value
    persistDesktopSidebarState(isDesktopSidebarOpen.value)
  }

  function openSidebar(): void {
    if (typeof window !== 'undefined' && window.innerWidth < 1024) {
      isMobileMenuOpen.value = true
    } else {
      isDesktopSidebarOpen.value = true
      persistDesktopSidebarState(true)
    }
  }

  function closeSidebar(): void {
    isMobileMenuOpen.value = false
    isDesktopSidebarOpen.value = false
    persistDesktopSidebarState(false)
  }

  function toggleMenu(): void {
    if (typeof window !== 'undefined' && window.innerWidth < 1024) {
      isMobileMenuOpen.value = !isMobileMenuOpen.value
    } else {
      toggleDesktopSidebar()
    }
  }

  return {
    isDesktopSidebarOpen,
    isMobileMenuOpen,
    toggleMobileMenu,
    closeMobileMenu,
    toggleDesktopSidebar,
    openSidebar,
    closeSidebar,
    toggleMenu
  }
}
