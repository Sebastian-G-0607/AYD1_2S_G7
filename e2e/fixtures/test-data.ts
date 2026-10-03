import path from 'path'

export const STUDENT_TEST_DATA = {
  nombre: 'Carlos',
  apellido: 'Mendoza',
  carnet: '202200123',
  genero: 'masculino',
  telefono: '55551234',
  fechaNacimiento: '2001-03-15',
  direccion: '10 Calle 5-45, Zona 1, Ciudad de Guatemala',
  correo: 'estudiante.e2e@educonnect.com',
  password: 'Password123!',
  confirmPassword: 'Password123!',
  fotoPath: path.resolve(__dirname, 'images/estudiante.png'),
  carnetPdfPath: path.resolve(__dirname, 'documents/carnet.pdf')
}

export const TUTOR_TEST_DATA = {
  nombre: 'Sofia',
  apellido: 'Ramirez',
  carnetId: '201800987',
  numeroIdentificacion: '2589631470101',
  genero: 'femenino',
  telefono: '55559876',
  fechaNacimiento: '1995-07-20',
  direccion: 'Boulevard Los Proceres 12-40, Zona 10, Ciudad de Guatemala',
  universidad: 'Universidad de San Carlos de Guatemala',
  anioInicio: '2018',
  direccionTutoria: 'Edificio T-3, Aula 201, Ciudad Universitaria',
  materia: 'Física',
  horaInicio: '08:00',
  horaFin: '17:00',
  correo: 'tutor.e2e@educonnect.com',
  password: 'Password123!',
  confirmPassword: 'Password123!',
  fotoPath: path.resolve(__dirname, 'images/tutor.png'),
  cvPdfPath: path.resolve(__dirname, 'documents/cv.pdf')
}
