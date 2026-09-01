import axios from 'axios'
import { setupInterceptors } from './interceptors'
import { getEnv } from '@/config/env'

export const api = axios.create({
  baseURL: getEnv('VITE_API_URL', ''),
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json'
  }
})

setupInterceptors(api)

export default api
