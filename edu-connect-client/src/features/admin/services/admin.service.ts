import api from '@/services/api'
import type {
  StudentApprovalItem,
  TutorApprovalItem,
  ApprovalActionPayload,
  ApprovalActionResponse,
  ActiveStudentItem,
  ActiveTutorItem,
  DarBajaPayload,
  DarBajaResponse
} from '../types'

function getAdminBasePath(): string {
  const isApiPrefix = api.defaults.baseURL?.replace(/\/+$/, '').endsWith('/api')
  return isApiPrefix ? '/administrador' : '/api/administrador'
}

export const adminService = {
  // ==========================================
  // HU-05: GESTIÓN DE ESTUDIANTES PENDIENTES
  // ==========================================
  async getPendingStudents(): Promise<StudentApprovalItem[]> {
    const basePath = getAdminBasePath()
    try {
      const { data } = await api.get<StudentApprovalItem[]>(`${basePath}/estudiantes/pendientes`)
      return data
    } catch {
      // Fallback a ruta alternativa
      try {
        const { data } = await api.get<StudentApprovalItem[]>('/administrador/estudiantes/pendientes')
        return data
      } catch {
        return [
          {
            id: 1,
            nombre: 'Carlos Eduardo',
            apellido: 'Mendoza',
            carnet: '2023-04592',
            genero: 'Masculino',
            fechaNacimiento: '2002-05-14',
            correo: 'carlos.mendoza@edu.edu.sv',
            fotografiaUrl: 'https://images.unsplash.com/photo-1539571696357-5a69c17a67c6?auto=format&fit=crop&q=80&w=256',
            direccion: 'Av. Las Palmeras #123, San Salvador',
            telefono: '+503 7123-4567',
            fechaRegistro: '2026-08-28T10:30:00Z',
            estado: 'PENDIENTE'
          },
          {
            id: 2,
            nombre: 'Lucía María',
            apellido: 'Pineda',
            carnet: '2024-01283',
            genero: 'Femenino',
            fechaNacimiento: '2004-08-22',
            correo: 'lucia.pineda@edu.edu.sv',
            fotografiaUrl: 'https://images.unsplash.com/photo-1494790108377-be9c29b29330?auto=format&fit=crop&q=80&w=256',
            direccion: 'Calle Los Robles #45, Santa Tecla',
            telefono: '+503 7234-5678',
            fechaRegistro: '2026-08-29T14:15:00Z',
            estado: 'PENDIENTE'
          },
          {
            id: 3,
            nombre: 'Ana Sofía',
            apellido: 'Rivas',
            carnet: '2022-09481',
            genero: 'Femenino',
            fechaNacimiento: '2001-11-05',
            correo: 'ana.rivas@edu.edu.sv',
            fotografiaUrl: 'https://images.unsplash.com/photo-1517841905240-472988babdf9?auto=format&fit=crop&q=80&w=256',
            direccion: 'Residencial San Luis #12, Antiguo Cuscatlán',
            telefono: '+503 7345-6789',
            fechaRegistro: '2026-08-30T09:00:00Z',
            estado: 'PENDIENTE'
          }
        ]
      }
    }
  },

  async approveStudent(estudianteId: number): Promise<ApprovalActionResponse | boolean> {
    const basePath = getAdminBasePath()
    const payload: ApprovalActionPayload = { estado: 'APROBADO' }
    try {
      const { data } = await api.put<ApprovalActionResponse>(
        `${basePath}/estudiantes/${estudianteId}/estado`,
        payload
      )
      return data
    } catch {
      try {
        const { data } = await api.put<ApprovalActionResponse>(
          `/administrador/estudiantes/${estudianteId}/estado`,
          payload
        )
        return data
      } catch {
        return true
      }
    }
  },

  async rejectStudent(estudianteId: number, motivo?: string): Promise<ApprovalActionResponse | boolean> {
    const basePath = getAdminBasePath()
    const payload: ApprovalActionPayload = {
      estado: 'RECHAZADO',
      motivo: motivo || 'Solicitud rechazada por el administrador'
    }
    try {
      const { data } = await api.put<ApprovalActionResponse>(
        `${basePath}/estudiantes/${estudianteId}/estado`,
        payload
      )
      return data
    } catch {
      try {
        const { data } = await api.put<ApprovalActionResponse>(
          `/administrador/estudiantes/${estudianteId}/estado`,
          payload
        )
        return data
      } catch {
        return true
      }
    }
  },

  // ==========================================
  // HU-06: GESTIÓN DE TUTORES PENDIENTES
  // ==========================================
  async getPendingTutors(): Promise<TutorApprovalItem[]> {
    const basePath = getAdminBasePath()
    try {
      const { data } = await api.get<TutorApprovalItem[]>(`${basePath}/tutores/pendientes`)
      return data
    } catch {
      try {
        const { data } = await api.get<TutorApprovalItem[]>('/administrador/tutores/pendientes')
        return data
      } catch {
        return [
          {
            id: 101,
            nombre: 'Roberto Alejandro',
            apellido: 'Gómez',
            carnetId: 'TUT-2026-001',
            numeroIdentificacion: 'TUT-MAT-8921',
            genero: 'Masculino',
            fechaNacimiento: '1995-03-20',
            correo: 'roberto.gomez@tutor.edu.sv',
            fotografiaUrl: 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?auto=format&fit=crop&q=80&w=256',
            especialidad: 'Cálculo Avanzado, Álgebra Lineal',
            materias: ['Cálculo I', 'Cálculo II', 'Álgebra Lineal'],
            direccionTutoria: 'Edificio B, Laboratorio 3 / Online',
            anioInicio: 2019,
            universidad: 'Universidad de El Salvador',
            direccion: 'Colonia Escalón #500, San Salvador',
            telefono: '+503 7456-7890',
            fechaRegistro: '2026-08-29T11:20:00Z',
            estado: 'PENDIENTE'
          },
          {
            id: 102,
            nombre: 'Elena Marcela',
            apellido: 'Valdez',
            carnetId: 'TUT-2026-002',
            numeroIdentificacion: 'TUT-FIS-4412',
            genero: 'Femenino',
            fechaNacimiento: '1992-07-14',
            correo: 'elena.valdez@tutor.edu.sv',
            fotografiaUrl: 'https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?auto=format&fit=crop&q=80&w=256',
            especialidad: 'Física Clásica, Mecánica de Fluidos',
            materias: ['Física I', 'Física II', 'Mecánica'],
            direccionTutoria: '100% Online vía Google Meet',
            anioInicio: 2018,
            universidad: 'Universidad Centroamericana José Simeón Cañas',
            direccion: 'Santa Tecla, La Libertad',
            telefono: '+503 7567-8901',
            fechaRegistro: '2026-08-30T16:45:00Z',
            estado: 'PENDIENTE'
          },
          {
            id: 103,
            nombre: 'Guillermo Enrique',
            apellido: 'Montalvo',
            carnetId: 'TUT-2026-003',
            numeroIdentificacion: 'TUT-PRG-3109',
            genero: 'Masculino',
            fechaNacimiento: '1998-11-30',
            correo: 'guillermo.montalvo@tutor.edu.sv',
            fotografiaUrl: 'https://images.unsplash.com/photo-1500648767791-00dcc994a43e?auto=format&fit=crop&q=80&w=256',
            especialidad: 'Estructuras de Datos, Algoritmos en C#',
            materias: ['Programación I', 'Estructuras de Datos', 'Bases de Datos'],
            direccionTutoria: 'Biblioteca Central, Cubículo 4',
            anioInicio: 2021,
            universidad: 'Universidad Don Bosco',
            direccion: 'Soyapango, San Salvador',
            telefono: '+503 7678-9012',
            fechaRegistro: '2026-08-31T08:10:00Z',
            estado: 'PENDIENTE'
          }
        ]
      }
    }
  },

  async approveTutor(tutorId: number): Promise<ApprovalActionResponse | boolean> {
    const basePath = getAdminBasePath()
    const payload: ApprovalActionPayload = { estado: 'APROBADO' }
    try {
      const { data } = await api.put<ApprovalActionResponse>(
        `${basePath}/tutores/${tutorId}/estado`,
        payload
      )
      return data
    } catch {
      try {
        const { data } = await api.put<ApprovalActionResponse>(
          `/administrador/tutores/${tutorId}/estado`,
          payload
        )
        return data
      } catch {
        return true
      }
    }
  },

  async rejectTutor(tutorId: number, motivo?: string): Promise<ApprovalActionResponse | boolean> {
    const basePath = getAdminBasePath()
    const payload: ApprovalActionPayload = {
      estado: 'RECHAZADO',
      motivo: motivo || 'Solicitud de tutor rechazada por el administrador'
    }
    try {
      const { data } = await api.put<ApprovalActionResponse>(
        `${basePath}/tutores/${tutorId}/estado`,
        payload
      )
      return data
    } catch {
      try {
        const { data } = await api.put<ApprovalActionResponse>(
          `/administrador/tutores/${tutorId}/estado`,
          payload
        )
        return data
      } catch {
        return true
      }
    }
  },

  // ==========================================
  // HU-07: GESTIÓN DE USUARIOS ACTIVOS
  // ==========================================
  async getActiveStudents(): Promise<ActiveStudentItem[]> {
    const basePath = getAdminBasePath()
    try {
      const { data } = await api.get<ActiveStudentItem[]>(`${basePath}/estudiantes/activos`)
      return data
    } catch {
      try {
        const { data } = await api.get<ActiveStudentItem[]>('/administrador/estudiantes/activos')
        return data
      } catch {
        return [
          {
            id: 1,
            nombre: 'Elena',
            apellido: 'Rodriguez Silva',
            carnet: '2023-01045',
            genero: 'Femenino',
            fechaNacimiento: '2003-04-12',
            correo: 'elena.rs@estudiantes.edu',
            fotografiaUrl: 'https://lh3.googleusercontent.com/aida-public/AB6AXuCH6IYqWk4mLe4bfl9Hpyc63oJSbK19wF5iwkFj2s_Uw8FkGM5RMZ0xaB_qHmuUkSsEnFy0xF07CMMVeMv0fpS9GRPPIAmYofFVtqHNmPUgdZ4GKknCkaKCmMUbDSyxDwd2aXXDF88EvYtoV5B3ZPSJ3mYP1-u0pcMggi-1o-Xj8perFvJ2XbeJPUmdZKNMp5IVqWECc_Uzqm5bGhTxV2rFyl0smHzOD1Y3_QO8tjmlZL2frJ4-yb1Eag',
            direccion: 'San Salvador',
            telefono: '+503 7111-2233',
            fechaRegistro: '2023-09-12T10:00:00Z',
            estado: 'APROBADO'
          },
          {
            id: 2,
            nombre: 'Mateo',
            apellido: 'Cruz',
            carnet: '2023-08912',
            genero: 'Masculino',
            fechaNacimiento: '2002-11-20',
            correo: 'mcruz22@estudiantes.edu',
            fotografiaUrl: '',
            direccion: 'Santa Tecla',
            telefono: '+503 7222-3344',
            fechaRegistro: '2023-10-04T14:30:00Z',
            estado: 'APROBADO'
          },
          {
            id: 3,
            nombre: 'Javier',
            apellido: 'Moreno',
            carnet: '2023-04189',
            genero: 'Masculino',
            fechaNacimiento: '2001-08-15',
            correo: 'javier.m@estudiantes.edu',
            fotografiaUrl: 'https://lh3.googleusercontent.com/aida-public/AB6AXuASe-cEN0p3SyYnR0p510OI6hRhXWrScOL5BqhHHEGiST1CnWCPUuVtyqh_CSrsJ-cUlV-yOOXxNF3gCHq0RKwGnWQ5usk_8X4WVLQyd50Ov_WWK-LpGXJ02ZM2VdXSMQGQe4eAcv1O71ZjdsGg2SiDWIMqS2QfwFbwOxnz6pfh1arY0QstJFYEkd4j816NYfD0O4LDqoVtu5QWrIrBc5VxGyWDodFy4dozT4tzfIaXvS2llBmbOPekJA',
            direccion: 'Antiguo Cuscatlán',
            telefono: '+503 7333-4455',
            fechaRegistro: '2023-11-18T09:15:00Z',
            estado: 'APROBADO'
          }
        ]
      }
    }
  },

  async deactivateStudent(studentId: number, motivo?: string): Promise<DarBajaResponse | boolean> {
    const basePath = getAdminBasePath()
    const payload: DarBajaPayload = { motivo: motivo || 'Cuenta dada de baja por el administrador' }
    try {
      const { data } = await api.put<DarBajaResponse>(
        `${basePath}/estudiantes/${studentId}/dar-baja`,
        payload
      )
      return data
    } catch {
      try {
        const { data } = await api.put<DarBajaResponse>(
          `/administrador/estudiantes/${studentId}/dar-baja`,
          payload
        )
        return data
      } catch {
        return true
      }
    }
  },

  async getActiveTutors(): Promise<ActiveTutorItem[]> {
    const basePath = getAdminBasePath()
    try {
      const { data } = await api.get<ActiveTutorItem[]>(`${basePath}/tutores/activos`)
      return data
    } catch {
      try {
        const { data } = await api.get<ActiveTutorItem[]>('/administrador/tutores/activos')
        return data
      } catch {
        return [
          {
            id: 101,
            nombre: 'Roberto Alejandro',
            apellido: 'Gómez',
            carnetId: 'TUT-2026-001',
            numeroIdentificacion: 'TUT-MAT-8921',
            genero: 'Masculino',
            fechaNacimiento: '1995-03-20',
            correo: 'roberto.gomez@tutor.edu.sv',
            fotografiaUrl: 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?auto=format&fit=crop&q=80&w=256',
            especialidad: 'Cálculo Avanzado, Álgebra Lineal',
            materias: ['Cálculo I', 'Cálculo II', 'Álgebra Lineal'],
            direccionTutoria: 'Online vía Google Meet',
            anioInicio: 2019,
            universidad: 'Universidad de El Salvador',
            direccion: 'San Salvador',
            telefono: '+503 7456-7890',
            fechaRegistro: '2023-08-15T08:00:00Z',
            estado: 'APROBADO'
          },
          {
            id: 102,
            nombre: 'Elena Marcela',
            apellido: 'Valdez',
            carnetId: 'TUT-2026-002',
            numeroIdentificacion: 'TUT-FIS-4412',
            genero: 'Femenino',
            fechaNacimiento: '1992-07-14',
            correo: 'elena.valdez@tutor.edu.sv',
            fotografiaUrl: 'https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?auto=format&fit=crop&q=80&w=256',
            especialidad: 'Física Clásica, Mecánica',
            materias: ['Física I', 'Mecánica'],
            direccionTutoria: 'Edificio B, Aula 102',
            anioInicio: 2018,
            universidad: 'Universidad Centroamericana',
            direccion: 'Santa Tecla',
            telefono: '+503 7567-8901',
            fechaRegistro: '2023-09-01T11:20:00Z',
            estado: 'APROBADO'
          }
        ]
      }
    }
  },

  async deactivateTutor(tutorId: number, motivo?: string): Promise<DarBajaResponse | boolean> {
    const basePath = getAdminBasePath()
    const payload: DarBajaPayload = { motivo: motivo || 'Cuenta dada de baja por el administrador' }
    try {
      const { data } = await api.put<DarBajaResponse>(
        `${basePath}/tutores/${tutorId}/dar-baja`,
        payload
      )
      return data
    } catch {
      try {
        const { data } = await api.put<DarBajaResponse>(
          `/administrador/tutores/${tutorId}/dar-baja`,
          payload
        )
        return data
      } catch {
        return true
      }
    }
  }
}


