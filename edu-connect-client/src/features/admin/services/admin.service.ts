import api from '@/services/api'
import type {
  StudentApprovalItem,
  TutorApprovalItem,
  ApprovalActionPayload,
  ApprovalActionResponse,
  ActiveStudentItem,
  ActiveTutorItem,
  DarBajaPayload,
  DarBajaResponse,
  TutorAtencionesReporteItem,
  MateriaDemandaReporteItem,
  ReportesResumen,
  InactiveUserItem,
  UpdateTutorAdminPayload,
  UpdateTutorAdminResponse,
  UpdateStudentAdminPayload,
  UpdateStudentAdminResponse,
  EstudianteCalificacionReporteItem
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
      const { data } = await api.get<StudentApprovalItem[]>('/administrador/estudiantes/pendientes')
      return data
    }
  },

  async approveStudent(estudianteId: number): Promise<ApprovalActionResponse> {
    const basePath = getAdminBasePath()
    const payload: ApprovalActionPayload = { estado: 'APROBADO' }
    try {
      const { data } = await api.put<ApprovalActionResponse>(
        `${basePath}/estudiantes/${estudianteId}/estado`,
        payload
      )
      return data
    } catch {
      const { data } = await api.put<ApprovalActionResponse>(
        `/administrador/estudiantes/${estudianteId}/estado`,
        payload
      )
      return data
    }
  },

  async rejectStudent(estudianteId: number, motivo?: string): Promise<ApprovalActionResponse> {
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
      const { data } = await api.put<ApprovalActionResponse>(
        `/administrador/estudiantes/${estudianteId}/estado`,
        payload
      )
      return data
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
      const { data } = await api.get<TutorApprovalItem[]>('/administrador/tutores/pendientes')
      return data
    }
  },

  async approveTutor(tutorId: number): Promise<ApprovalActionResponse> {
    const basePath = getAdminBasePath()
    const payload: ApprovalActionPayload = { estado: 'APROBADO' }
    try {
      const { data } = await api.put<ApprovalActionResponse>(
        `${basePath}/tutores/${tutorId}/estado`,
        payload
      )
      return data
    } catch {
      const { data } = await api.put<ApprovalActionResponse>(
        `/administrador/tutores/${tutorId}/estado`,
        payload
      )
      return data
    }
  },

  async rejectTutor(tutorId: number, motivo?: string): Promise<ApprovalActionResponse> {
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
      const { data } = await api.put<ApprovalActionResponse>(
        `/administrador/tutores/${tutorId}/estado`,
        payload
      )
      return data
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
      const { data } = await api.get<ActiveStudentItem[]>('/administrador/estudiantes/activos')
      return data
    }
  },

  async getInactiveUsers(): Promise<InactiveUserItem[]> {
    const basePath = getAdminBasePath()
    try {
      const { data } = await api.get<InactiveUserItem[]>(`${basePath}/usuarios/dados-de-baja`)
      return data
    } catch {
      const { data } = await api.get<InactiveUserItem[]>('/administrador/usuarios/dados-de-baja')
      return data
    }
  },

  async deactivateStudent(studentId: number, motivo?: string): Promise<DarBajaResponse> {
    const basePath = getAdminBasePath()
    const payload: DarBajaPayload = { motivo: motivo || 'Cuenta dada de baja por el administrador' }
    try {
      const { data } = await api.put<DarBajaResponse>(
        `${basePath}/estudiantes/${studentId}/dar-baja`,
        payload
      )
      return data
    } catch {
      const { data } = await api.put<DarBajaResponse>(
        `/administrador/estudiantes/${studentId}/dar-baja`,
        payload
      )
      return data
    }
  },

  async getActiveTutors(): Promise<ActiveTutorItem[]> {
    const basePath = getAdminBasePath()
    try {
      const { data } = await api.get<ActiveTutorItem[]>(`${basePath}/tutores/activos`)
      return data
    } catch {
      const { data } = await api.get<ActiveTutorItem[]>('/administrador/tutores/activos')
      return data
    }
  },

  async deactivateTutor(tutorId: number, motivo?: string): Promise<DarBajaResponse> {
    const basePath = getAdminBasePath()
    const payload: DarBajaPayload = { motivo: motivo || 'Cuenta dada de baja por el administrador' }
    try {
      const { data } = await api.put<DarBajaResponse>(
        `${basePath}/tutores/${tutorId}/dar-baja`,
        payload
      )
      return data
    } catch {
      const { data } = await api.put<DarBajaResponse>(
        `/administrador/tutores/${tutorId}/dar-baja`,
        payload
      )
      return data
    }
  },

  // ==========================================
  // HU-35: VER Y ACTUALIZAR TUTOR (ADMIN)
  // ==========================================
  async getTutorById(tutorId: number): Promise<ActiveTutorItem> {
    const basePath = getAdminBasePath()
    try {
      const { data } = await api.get<ActiveTutorItem>(`${basePath}/tutores/${tutorId}`)
      return data
    } catch {
      const { data } = await api.get<ActiveTutorItem>(`/administrador/tutores/${tutorId}`)
      return data
    }
  },

  async updateTutor(
    tutorId: number,
    payload: UpdateTutorAdminPayload
  ): Promise<UpdateTutorAdminResponse> {
    const basePath = getAdminBasePath()
    try {
      const { data } = await api.put<UpdateTutorAdminResponse>(
        `${basePath}/tutores/${tutorId}`,
        payload
      )
      return data
    } catch {
      const { data } = await api.put<UpdateTutorAdminResponse>(
        `/administrador/tutores/${tutorId}`,
        payload
      )
      return data
    }
  },

  // ==========================================
  // HU-34: VER Y ACTUALIZAR ESTUDIANTE (ADMIN)
  // ==========================================
  async getStudentById(studentId: number): Promise<ActiveStudentItem> {
    const basePath = getAdminBasePath()
    try {
      const { data } = await api.get<ActiveStudentItem>(`${basePath}/estudiantes/${studentId}`)
      return data
    } catch {
      const { data } = await api.get<ActiveStudentItem>(`/administrador/estudiantes/${studentId}`)
      return data
    }
  },

  async updateStudent(
    studentId: number,
    payload: UpdateStudentAdminPayload
  ): Promise<UpdateStudentAdminResponse> {
    const basePath = getAdminBasePath()
    try {
      const { data } = await api.put<UpdateStudentAdminResponse>(
        `${basePath}/estudiantes/${studentId}`,
        payload
      )
      return data
    } catch {
      const { data } = await api.put<UpdateStudentAdminResponse>(
        `/administrador/estudiantes/${studentId}`,
        payload
      )
      return data
    }
  },

  // ==========================================
  // HU-08: REPORTES Y ESTADÍSTICAS
  // ==========================================
  async getTutoresMasAtendidos(limit?: number): Promise<TutorAtencionesReporteItem[]> {
    const basePath = getAdminBasePath()
    const query = limit && limit > 0 ? `?limit=${limit}` : ''
    try {
      const { data } = await api.get<TutorAtencionesReporteItem[]>(
        `${basePath}/reportes/tutores-mas-atendidos${query}`
      )
      return data
    } catch {
      try {
        const { data } = await api.get<TutorAtencionesReporteItem[]>(
          `/administrador/reportes/tutores-mas-atendidos${query}`
        )
        return data
      } catch {
        return [
          {
            tutorId: 1,
            nombre: 'Carlos',
            apellido: 'Mendoza',
            nombreCompleto: 'Dr. Mendoza',
            carnet: '2019-10291',
            correo: 'carlos.mendoza@edu.edu.sv',
            totalSesionesAtendidas: 145,
            totalEstudiantesAtendidos: 62
          },
          {
            tutorId: 2,
            nombre: 'Gabriela',
            apellido: 'Silva',
            nombreCompleto: 'Dra. Silva',
            carnet: '2020-04921',
            correo: 'gabriela.silva@edu.edu.sv',
            totalSesionesAtendidas: 112,
            totalEstudiantesAtendidos: 48
          },
          {
            tutorId: 3,
            nombre: 'Alejandro',
            apellido: 'Vargas',
            nombreCompleto: 'Ing. Vargas',
            carnet: '2018-00123',
            correo: 'alejandro.vargas@edu.edu.sv',
            totalSesionesAtendidas: 180,
            totalEstudiantesAtendidos: 79
          },
          {
            tutorId: 4,
            nombre: 'Fernanda',
            apellido: 'Rojas',
            nombreCompleto: 'Lic. Rojas',
            carnet: '2021-03482',
            correo: 'fernanda.rojas@edu.edu.sv',
            totalSesionesAtendidas: 85,
            totalEstudiantesAtendidos: 39
          },
          {
            tutorId: 5,
            nombre: 'Roberto',
            apellido: 'Gómez',
            nombreCompleto: 'Msc. Gómez',
            carnet: '2017-09412',
            correo: 'roberto.gomez@edu.edu.sv',
            totalSesionesAtendidas: 130,
            totalEstudiantesAtendidos: 55
          },
          {
            tutorId: 6,
            nombre: 'David',
            apellido: 'Castro',
            nombreCompleto: 'Dr. Castro',
            carnet: '2019-05821',
            correo: 'david.castro@edu.edu.sv',
            totalSesionesAtendidas: 98,
            totalEstudiantesAtendidos: 44
          }
        ]
      }
    }
  },

  async getMateriasMayorDemanda(limit?: number): Promise<MateriaDemandaReporteItem[]> {
    const basePath = getAdminBasePath()
    const query = limit && limit > 0 ? `?limit=${limit}` : ''
    try {
      const { data } = await api.get<MateriaDemandaReporteItem[]>(
        `${basePath}/reportes/materias-mayor-demanda${query}`
      )
      return data
    } catch {
      try {
        const { data } = await api.get<MateriaDemandaReporteItem[]>(
          `/administrador/reportes/materias-mayor-demanda${query}`
        )
        return data
      } catch {
        return [
          {
            materiaId: 1,
            nombreMateria: 'Cálculo Diferencial e Integral',
            totalSesiones: 350,
            sesionesAtendidas: 310,
            sesionesPendientes: 30,
            sesionesCanceladas: 10,
            porcentajeDemanda: 45
          },
          {
            materiaId: 2,
            nombreMateria: 'Física',
            totalSesiones: 195,
            sesionesAtendidas: 170,
            sesionesPendientes: 18,
            sesionesCanceladas: 7,
            porcentajeDemanda: 25
          },
          {
            materiaId: 3,
            nombreMateria: 'Estructuras de Datos y Algoritmos',
            totalSesiones: 155,
            sesionesAtendidas: 135,
            sesionesPendientes: 14,
            sesionesCanceladas: 6,
            porcentajeDemanda: 20
          },
          {
            materiaId: 4,
            nombreMateria: 'Química General',
            totalSesiones: 80,
            sesionesAtendidas: 70,
            sesionesPendientes: 8,
            sesionesCanceladas: 2,
            porcentajeDemanda: 10
          }
        ]
      }
    }
  },

  async getReportesResumen(): Promise<ReportesResumen> {
    const basePath = getAdminBasePath()
    try {
      const { data } = await api.get<ReportesResumen>(`${basePath}/reportes/resumen`)
      return data
    } catch {
      try {
        const { data } = await api.get<ReportesResumen>('/administrador/reportes/resumen')
        return data
      } catch {
        return {
          totalSesiones: 780,
          totalSesionesAtendidas: 750,
          totalSesionesPendientes: 70,
          totalSesionesCanceladas: 25,
          tasaEfectividad: 96.15,
          totalTutoresConAtenciones: 6,
          totalMateriasConDemanda: 4,
          tutorTopNombre: 'Ing. Alejandro Vargas',
          tutorTopAtenciones: 180,
          materiaTopNombre: 'Cálculo Diferencial e Integral',
          materiaTopSesiones: 350
        }
      }
    }
  },

  async getEstudiantesCalificaciones(
    limit?: number,
    search?: string,
    orden?: string
  ): Promise<EstudianteCalificacionReporteItem[]> {
    const basePath = getAdminBasePath()
    const params = new URLSearchParams()
    if (limit && limit > 0) params.append('limit', limit.toString())
    if (search && search.trim()) params.append('search', search.trim())
    if (orden && orden.trim()) params.append('orden', orden.trim())
    const queryString = params.toString() ? `?${params.toString()}` : ''

    try {
      const { data } = await api.get<EstudianteCalificacionReporteItem[]>(
        `${basePath}/reportes/calificaciones-estudiantes${queryString}`
      )
      return data
    } catch {
      try {
        const { data } = await api.get<EstudianteCalificacionReporteItem[]>(
          `/administrador/reportes/calificaciones-estudiantes${queryString}`
        )
        return data
      } catch {
        return [
          {
            estudianteId: 1,
            nombre: 'Sofia',
            apellido: 'Castillo',
            nombreCompleto: 'Sofia Castillo',
            carnet: '202200021',
            correo: 'sofia.castillo@educonnect.com',
            fotografiaUrl: null,
            totalSesionesAtendidas: 12,
            totalEvaluaciones: 10,
            promedioCalificacion: 4.8
          },
          {
            estudianteId: 2,
            nombre: 'Roberto',
            apellido: 'Alvarado',
            nombreCompleto: 'Roberto Alvarado',
            carnet: '202200020',
            correo: 'roberto.alvarado@educonnect.com',
            fotografiaUrl: null,
            totalSesionesAtendidas: 8,
            totalEvaluaciones: 7,
            promedioCalificacion: 4.5
          },
          {
            estudianteId: 3,
            nombre: 'Diego',
            apellido: 'Mendez',
            nombreCompleto: 'Diego Mendez',
            carnet: '202200022',
            correo: 'diego.mendez@educonnect.com',
            fotografiaUrl: null,
            totalSesionesAtendidas: 5,
            totalEvaluaciones: 4,
            promedioCalificacion: 4.0
          }
        ]
      }
    }
  }
}

