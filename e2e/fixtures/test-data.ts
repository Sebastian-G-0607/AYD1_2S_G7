import path from 'path'

/**
 * Usuarios de prueba que el backend siembra en la base de datos con estado APROBADO y con el
 * correo ya validado, por lo que pueden iniciar sesión sin tokens de confirmación de correo.
 * Ver DataExtensions.SeedUsuariosPrueba en el backend.
 */
export const E2E_STUDENT_USER = {
  correo: 'estudiante.e2e@educonnect.com',
  password: 'Password123!',
  nombre: 'Carlos',
  apellido: 'Mendoza',
  nombreCompleto: 'Carlos Mendoza'
}

export const E2E_TUTOR_USER = {
  correo: 'tutor.e2e@educonnect.com',
  password: 'Password123!',
  nombre: 'Sofia',
  apellido: 'Ramirez',
  nombreCompleto: 'Sofia Ramirez'
}

/**
 * Datos de registro de un estudiante. El identificador es único por archivo de
 * prueba y por corrida, porque el carnet y el correo son únicos en la base de datos.
 */
export function crearDatosEstudianteRegistro(identificador: string) {
  return {
    nombre: 'Carlos',
    apellido: 'Mendoza',
    carnet: `2022${identificador}`,
    genero: 'masculino',
    telefono: '55551234',
    fechaNacimiento: '2001-03-15',
    direccion: '10 Calle 5-45, Zona 1, Ciudad de Guatemala',
    correo: `estudiante.registro.${identificador}@educonnect.com`,
    password: 'Password123!',
    confirmPassword: 'Password123!',
    fotoPath: path.resolve(__dirname, 'images/estudiante.png'),
    carnetPdfPath: path.resolve(__dirname, 'documents/carnet.pdf')
  }
}

/**
 * Datos de registro de un tutor. El identificador es único por archivo de prueba y por
 * corrida, porque el carnet, el DPI y el correo son únicos en la base de datos.
 */
export function crearDatosTutorRegistro(identificador: string) {
  const digitosDpi = identificador.slice(0, 3)

  return {
    nombre: 'Sofia',
    apellido: 'Ramirez',
    carnetId: `2018${identificador}`,
    numeroIdentificacion: `2589631470${digitosDpi}`,
    genero: 'femenino',
    telefono: '55559876',
    fechaNacimiento: '1995-07-20',
    direccion: 'Boulevard Los Proceres 12-40, Zona 10, Ciudad de Guatemala',
    universidad: 'Universidad de San Carlos de Guatemala',
    anioInicio: '2018',
    direccionTutoria: 'Edificio T-3, Aula 201, Ciudad universitaria',
    materia: 'Física',
    horaInicio: '08:00',
    horaFin: '17:00',
    correo: `tutor.registro.${identificador}@educonnect.com`,
    password: 'Password123!',
    confirmPassword: 'Password123!',
    fotoPath: path.resolve(__dirname, 'images/tutor.png'),
    cvPdfPath: path.resolve(__dirname, 'documents/cv.pdf')
  }
}
