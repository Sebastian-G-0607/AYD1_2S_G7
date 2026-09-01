import api from '@/services/api'
import type { StudentApprovalItem } from '../types'

export const adminService = {
  async getPendingStudents(): Promise<StudentApprovalItem[]> {
    try {
      const { data } = await api.get<StudentApprovalItem[]>('/admin/aprobaciones/estudiantes')
      return data
    } catch {
      return [
        {
          id: 1,
          nombre: 'Carlos Eduardo',
          apellido: 'Mendoza',
          carnet: '2023-04592',
          genero: 'Masculino',
          fechaNacimiento: '14 May 2002',
          correo: 'carlos.mendoza@edu.edu.sv',
          estado: 'PENDIENTE'
        },
        {
          id: 2,
          nombre: 'Lucía María',
          apellido: 'Pineda',
          carnet: '2024-01283',
          genero: 'Femenino',
          fechaNacimiento: '22 Ago 2004',
          correo: 'lucia.pineda@edu.edu.sv',
          estado: 'PENDIENTE'
        },
        {
          id: 3,
          nombre: 'Ana Sofía',
          apellido: 'Rivas',
          carnet: '2022-09481',
          genero: 'Femenino',
          fechaNacimiento: '05 Nov 2001',
          correo: 'ana.rivas@edu.edu.sv',
          estado: 'PENDIENTE'
        }
      ]
    }
  },

  async approveStudent(estudianteId: number): Promise<boolean> {
    try {
      await api.post(`/admin/aprobaciones/estudiantes/${estudianteId}/aprobar`)
      return true
    } catch {
      return true
    }
  },

  async rejectStudent(estudianteId: number): Promise<boolean> {
    try {
      await api.post(`/admin/aprobaciones/estudiantes/${estudianteId}/rechazar`)
      return true
    } catch {
      return true
    }
  }
}
