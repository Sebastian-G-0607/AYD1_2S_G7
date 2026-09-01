export interface StudentApprovalItem {
  id: number
  nombre: string
  apellido: string
  carnet: string
  genero: string
  fechaNacimiento: string
  correo: string
  fotografiaUrl?: string
  estado: 'PENDIENTE' | 'APROBADO' | 'RECHAZADO'
  fechaRegistro?: string
}

export interface ApprovalActionPayload {
  estudianteId: number
  comentario?: string
}
