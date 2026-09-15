export function formatDateLabel(dateStr?: string): string {
  if (!dateStr) return ''
  const clean = dateStr.split('T')[0]
  const [year, month, day] = clean.split('-').map(Number)
  if (!year || !month || !day) return dateStr

  const d = new Date(year, month - 1, day)
  return d.toLocaleDateString('es-ES', {
    day: 'numeric',
    month: 'long',
    year: 'numeric'
  })
}

export function formatTimeLabel(dateStr?: string, timeStr?: string): string {
  const formattedTime = timeStr ? timeStr.slice(0, 5) : ''
  if (!dateStr) return formattedTime

  const clean = dateStr.split('T')[0]
  const [year, month, day] = clean.split('-').map(Number)
  if (!year || !month || !day) return formattedTime

  const targetDate = new Date(year, month - 1, day)
  const today = new Date()
  today.setHours(0, 0, 0, 0)
  targetDate.setHours(0, 0, 0, 0)

  const diffDays = Math.round((targetDate.getTime() - today.getTime()) / (1000 * 60 * 60 * 24))

  if (diffDays === 0) {
    return formattedTime ? `Hoy, ${formattedTime}` : 'Hoy'
  }
  if (diffDays === 1) {
    return formattedTime ? `Mañana, ${formattedTime}` : 'Mañana'
  }

  const dayOfWeek = targetDate.toLocaleDateString('es-ES', { weekday: 'short' })
  const capitalizedDay = dayOfWeek.charAt(0).toUpperCase() + dayOfWeek.slice(1).replace('.', '')

  return formattedTime ? `${capitalizedDay}, ${formattedTime}` : capitalizedDay
}

export function parseLocation(rawLocation?: string): {
  locationType: 'presencial' | 'virtual'
  locationTitle: string
  locationSubtitle: string
  meetingUrl?: string
} {
  if (!rawLocation || rawLocation.trim().length === 0) {
    return {
      locationType: 'presencial',
      locationTitle: 'Por coordinar con el tutor',
      locationSubtitle: 'Campus Universitario'
    }
  }

  const lower = rawLocation.toLowerCase()
  const isVirtual =
    rawLocation.includes('http') ||
    lower.includes('zoom') ||
    lower.includes('meet') ||
    lower.includes('teams')

  if (isVirtual) {
    return {
      locationType: 'virtual',
      locationTitle: 'Sesión Virtual',
      locationSubtitle: 'Enlace de la reunión',
      meetingUrl: rawLocation.startsWith('http') ? rawLocation : `https://${rawLocation}`
    }
  }

  if (rawLocation.includes(',')) {
    const parts = rawLocation.split(',')
    return {
      locationType: 'presencial',
      locationTitle: parts
        .slice(0, parts.length - 1)
        .join(',')
        .trim(),
      locationSubtitle: parts[parts.length - 1].trim()
    }
  }

  return {
    locationType: 'presencial',
    locationTitle: rawLocation.trim(),
    locationSubtitle: 'Campus Universitario'
  }
}

export function extractInitials(name?: string): string {
  if (!name) return 'TU'
  const parts = name.trim().split(/\s+/)
  if (parts.length >= 2) {
    return (parts[0].charAt(0) + parts[parts.length - 1].charAt(0)).toUpperCase()
  }
  return name.slice(0, 2).toUpperCase()
}
