export interface AvailabilityBlock {
  horaInicio: string
  horaFin: string
  disponible: boolean
}

export interface SesionOcupada {
  horaInicio: string
  horaFin: string
}

export interface TutorAvailability {
  tutorId: number
  nombreCompleto: string
  diasAtencion: number[]
  horaInicioAtencion: string | null
  horaFinAtencion: string | null
  fecha: string
  atiendeEseDia: boolean
  bloques: AvailabilityBlock[]
  sesionesOcupadas?: SesionOcupada[]
}
