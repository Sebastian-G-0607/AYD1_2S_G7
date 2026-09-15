import { ref, computed, onMounted } from 'vue'
import { adminService } from '../services/admin.service'
import type {
  TutorAtencionesReporteItem,
  MateriaDemandaReporteItem,
  ReportesResumen,
  ReportsTabType
} from '../types'

export function useAdminReports() {
  const tutoresReport = ref<TutorAtencionesReporteItem[]>([])
  const materiasReport = ref<MateriaDemandaReporteItem[]>([])
  const resumen = ref<ReportesResumen>({
    totalSesiones: 0,
    totalSesionesAtendidas: 0,
    totalSesionesPendientes: 0,
    totalSesionesCanceladas: 0,
    tasaEfectividad: 0,
    totalTutoresConAtenciones: 0,
    totalMateriasConDemanda: 0,
    tutorTopNombre: null,
    tutorTopAtenciones: 0,
    materiaTopNombre: null,
    materiaTopSesiones: 0
  })

  const isLoading = ref(false)
  const error = ref<string | null>(null)
  const selectedPeriod = ref<'7d' | '30d' | '90d' | 'all'>('30d')
  const selectedTab = ref<ReportsTabType>('todos')
  const searchQuery = ref('')

  // Máximo valor de atenciones para escalar las barras verticalmente (con mínimo de 1 para evitar división por cero)
  const maxAtencionesTutor = computed(() => {
    if (tutoresReport.value.length === 0) return 1
    return Math.max(...tutoresReport.value.map(t => t.totalSesionesAtendidas), 1)
  })

  // Total acumulado de estudiantes atendidos
  const totalEstudiantesAtendidos = computed(() => {
    if (tutoresReport.value.length === 0) return 0
    return tutoresReport.value.reduce((acc, t) => acc + t.totalEstudiantesAtendidos, 0)
  })

  // Paleta de colores para el gráfico Donut y Barras (Tokens de Stitch)
  const donutPalette = [
    {
      name: 'primary',
      strokeClass: 'stroke-primary hover:stroke-tertiary',
      bgClass: 'bg-primary',
      hex: '#091426'
    },
    {
      name: 'secondary',
      strokeClass: 'stroke-secondary hover:stroke-secondary-container',
      bgClass: 'bg-secondary',
      hex: '#0058be'
    },
    {
      name: 'primary-fixed-dim',
      strokeClass: 'stroke-primary-fixed-dim hover:stroke-primary',
      bgClass: 'bg-primary-fixed-dim',
      hex: '#bcc7de'
    },
    {
      name: 'outline',
      strokeClass: 'stroke-surface-dim hover:stroke-outline',
      bgClass: 'bg-outline',
      hex: '#75777d'
    },
    {
      name: 'secondary-container',
      strokeClass: 'stroke-secondary-container hover:stroke-secondary',
      bgClass: 'bg-secondary-container',
      hex: '#2170e4'
    }
  ]

  // Cálculo geométrico de los arcos del gráfico SVG Donut
  const donutSegments = computed(() => {
    const circumference = 2 * Math.PI * 40 // ~251.327
    let accumulatedOffset = 0

    return materiasReport.value.map((item, idx) => {
      const color = donutPalette[idx % donutPalette.length]
      const dashLength = (item.porcentajeDemanda / 100) * circumference
      const dashOffset = -accumulatedOffset
      accumulatedOffset += dashLength

      return {
        ...item,
        dashArray: `${dashLength.toFixed(1)} ${circumference.toFixed(1)}`,
        dashOffset: dashOffset.toFixed(1),
        strokeClass: color.strokeClass,
        bgClass: color.bgClass,
        hex: color.hex
      }
    })
  })

  // Filtros de búsqueda para las tablas
  const filteredTutores = computed(() => {
    const q = searchQuery.value.trim().toLowerCase()
    if (!q) return tutoresReport.value
    return tutoresReport.value.filter(
      t =>
        t.nombreCompleto.toLowerCase().includes(q) ||
        t.carnet.toLowerCase().includes(q) ||
        t.correo.toLowerCase().includes(q)
    )
  })

  const filteredMaterias = computed(() => {
    const q = searchQuery.value.trim().toLowerCase()
    if (!q) return materiasReport.value
    return materiasReport.value.filter(m => m.nombreMateria.toLowerCase().includes(q))
  })

  // Carga de datos
  async function loadReports() {
    isLoading.value = true
    error.value = null

    try {
      const [resumenData, tutoresData, materiasData] = await Promise.all([
        adminService.getReportesResumen(),
        adminService.getTutoresMasAtendidos(10),
        adminService.getMateriasMayorDemanda(10)
      ])

      resumen.value = resumenData
      tutoresReport.value = tutoresData
      materiasReport.value = materiasData
    } catch (err: unknown) {
      error.value = err instanceof Error ? err.message : 'Error al cargar los datos de reportes'
    } finally {
      isLoading.value = false
    }
  }

  // Exportar reporte a formato CSV
  function exportReport() {
    const rows = [
      ['=== REPORTE DE GESTIÓN ACADÉMICA EDUCONNECT ==='],
      ['Fecha de generación', new Date().toLocaleString()],
      ['Total Sesiones', resumen.value.totalSesiones.toString()],
      ['Total Sesiones Atendidas', resumen.value.totalSesionesAtendidas.toString()],
      ['Tasa de Efectividad', `${resumen.value.tasaEfectividad}%`],
      [''],
      ['=== TUTORES CON MÁS ESTUDIANTES ATENDIDOS ==='],
      ['Tutor', 'Carnet', 'Correo', 'Sesiones Atendidas', 'Estudiantes Únicos'],
      ...tutoresReport.value.map(t => [
        t.nombreCompleto,
        t.carnet,
        t.correo,
        t.totalSesionesAtendidas.toString(),
        t.totalEstudiantesAtendidos.toString()
      ]),
      [''],
      ['=== MATERIAS CON MAYOR DEMANDA ==='],
      ['Materia', 'Total Sesiones', 'Atendidas', 'Pendientes', 'Canceladas', '% Demanda'],
      ...materiasReport.value.map(m => [
        m.nombreMateria,
        m.totalSesiones.toString(),
        m.sesionesAtendidas.toString(),
        m.sesionesPendientes.toString(),
        m.sesionesCanceladas.toString(),
        `${m.porcentajeDemanda}%`
      ])
    ]

    const csvContent =
      'data:text/csv;charset=utf-8,\uFEFF' +
      rows.map(e => e.map(val => `"${val.replace(/"/g, '""')}"`).join(',')).join('\n')

    const encodedUri = encodeURI(csvContent)
    const link = document.createElement('a')
    link.setAttribute('href', encodedUri)
    link.setAttribute('download', `reporte_academico_${new Date().toISOString().slice(0, 10)}.csv`)
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
  }

  onMounted(() => {
    loadReports()
  })

  return {
    tutoresReport,
    materiasReport,
    resumen,
    isLoading,
    error,
    selectedPeriod,
    selectedTab,
    searchQuery,
    maxAtencionesTutor,
    totalEstudiantesAtendidos,
    donutSegments,
    filteredTutores,
    filteredMaterias,
    loadReports,
    exportReport
  }
}
