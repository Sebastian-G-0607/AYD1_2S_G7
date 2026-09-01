import api from '@/services/api'
import type { TutorExplorerItem, TutorFilterCriteria } from '../types'

export const tutorsExplorerService = {
  async getTutors(filters?: Partial<TutorFilterCriteria>): Promise<TutorExplorerItem[]> {
    try {
      const { data } = await api.get<TutorExplorerItem[]>('/tutores/explorar', {
        params: filters
      })
      return data
    } catch {
      return [
        {
          id: 1,
          nombre: 'Dra. Elena Valdez',
          titulo: 'Doctora en Física',
          especialidad: 'Física Cuántica',
          rating: 4.9,
          totalResenas: 120,
          ubicacion: 'Campus Central, Edificio C o Remoto',
          universidad: 'UNAM',
          aniosExperiencia: 8,
          tags: ['Matemáticas', 'Álgebra'],
          fotografiaUrl:
            'https://lh3.googleusercontent.com/aida-public/AB6AXuDCBKb1SYZQdIOx17UVf9vUpS7acmrP5H6uUe4pQJQ-Hl5OBFddy_r0IsHY3xuObJ1TCyxUyAv3vxHdL7LKHb0aCXQATCJd-zqfFpWijuVtjkauVPejQxLhfKk8dwXYxlyb_MXVStYk4pubQq-P9ZnQgLpn_AsA30cjsCZDycrw4VfK36NBwb_L6tw766Yu1k1_zQiQHHP4LfiqJsf0hyHcJuOCZhyJ5OU7qno-BmeDNaMWxrOQwf77PQ',
          isOnline: true,
          genero: 'femenino',
          edad: 34
        },
        {
          id: 2,
          nombre: 'Ing. Carlos Ríos',
          titulo: 'Ingeniero de Software',
          especialidad: 'Ciencias de la Computación',
          rating: 4.8,
          totalResenas: 85,
          ubicacion: 'Zona Norte, Cafetería Tec',
          universidad: 'Tec de Monterrey',
          aniosExperiencia: 4,
          tags: ['Python', 'Estructuras'],
          fotografiaUrl:
            'https://lh3.googleusercontent.com/aida-public/AB6AXuD_GI3Hcs_rjL7QbYAPWk9-S_f_aWCdffB0Mr0gptG7CtBcADeQyIqNGYh-2zXsGBIp2hTmZ0pOKHI07MG6dmZauDHXXuPlbjtBitGUpDryjMWK0_cgpuThLcNrG1oJ3ZwBGkZT70O9ntrwx_cR5AKCB4jRfpnWk2ppKe7KUkIvL2Rg6c1KhW2Ckv1u84Aey3oMTlhFE6Ft443kc5_Hrhr1kEZeVIxQgmxRZYi5teBc0-rrsEkhy6vkrA',
          isOnline: false,
          genero: 'masculino',
          edad: 28
        },
        {
          id: 3,
          nombre: 'Lic. Mariana Paz',
          titulo: 'Licenciada en Letras',
          especialidad: 'Literatura Contemporánea',
          rating: 5.0,
          totalResenas: 210,
          ubicacion: '100% Remoto',
          universidad: 'Universidad Iberoamericana',
          aniosExperiencia: 12,
          tags: ['Ensayo', 'Redacción'],
          fotografiaUrl:
            'https://lh3.googleusercontent.com/aida-public/AB6AXuBk8_F3Zxu_cMRD1gUNC90gZfSDGiM0-WY3vBerJDngsS5y-iBRapR4svEvnpCuTwMuOJUh7rbL4Wib3t0FoKphafjjRGgK-FqNdcbFkM-gbOw8thF2xOO2opbyVmuW5GXP-D4KNXQoah9NL-7A7AJjYRjZyQ6hFADbuQLFM9XU7kafIvOGT3pYZl7i8I_oxf3YBmG0x-WBwtqSHb2bl5UtZHiICgIfUYRyChYQ_VcCmYOT9ISFB9Nb_w',
          isOnline: true,
          genero: 'femenino',
          edad: 42
        }
      ]
    }
  }
}
