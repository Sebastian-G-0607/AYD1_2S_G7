import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { AuthUser, TokenResponseDto } from './types'

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem('edu_auth_token'))
  const user = ref<AuthUser | null>(() => {
    const rawUser = localStorage.getItem('edu_auth_user')
    if (!rawUser) return null
    try {
      return JSON.parse(rawUser) as AuthUser
    } catch {
      return null
    }
  })

  const isAuthenticated = computed(() => Boolean(token.value))
  const userRole = computed(() => user.value?.rol || '')

  function setAuth(newToken: string, newUser: AuthUser) {
    token.value = newToken
    user.value = newUser
    localStorage.setItem('edu_auth_token', newToken)
    localStorage.setItem('edu_auth_user', JSON.stringify(newUser))
  }

  function setAuthFromTokenResponse(response: TokenResponseDto) {
    const authUser: AuthUser = {
      id: response.idUsuario,
      correo: response.correo,
      rol: response.rol
    }
    setAuth(response.token, authUser)
  }

  function clearAuth() {
    token.value = null
    user.value = null
    localStorage.removeItem('edu_auth_token')
    localStorage.removeItem('edu_auth_user')
  }

  return {
    token,
    user,
    isAuthenticated,
    userRole,
    setAuth,
    setAuthFromTokenResponse,
    clearAuth
  }
})
