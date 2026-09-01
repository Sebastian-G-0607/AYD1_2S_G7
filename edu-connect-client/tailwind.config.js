export default {
  content: ['./index.html', './src/**/*.{vue,js,ts,jsx,tsx}'],
  darkMode: 'class',
  theme: {
    extend: {
      colors: {
        primary: '#091426',
        'on-primary': '#ffffff',
        'primary-container': '#1e293b',
        'on-primary-container': '#8590a6',
        'primary-fixed': '#d8e3fb',
        'primary-fixed-dim': '#bcc7de',
        'on-primary-fixed': '#111c2d',
        'on-primary-fixed-variant': '#3c475a',
        'inverse-primary': '#bcc7de',

        secondary: '#0058be',
        'on-secondary': '#ffffff',
        'secondary-container': '#2170e4',
        'on-secondary-container': '#fefcff',
        'secondary-fixed': '#d8e2ff',
        'secondary-fixed-dim': '#adc6ff',
        'on-secondary-fixed': '#001a42',
        'on-secondary-fixed-variant': '#004395',

        tertiary: '#041528',
        'on-tertiary': '#ffffff',
        'tertiary-container': '#1a2a3e',
        'on-tertiary-container': '#8191a9',
        'tertiary-fixed': '#d3e4fe',
        'tertiary-fixed-dim': '#b7c8e1',
        'on-tertiary-fixed': '#0b1c30',
        'on-tertiary-fixed-variant': '#38485d',

        error: '#ba1a1a',
        'on-error': '#ffffff',
        'error-container': '#ffdad6',
        'on-error-container': '#93000a',

        surface: '#f7f9fb',
        'surface-dim': '#d8dadc',
        'surface-bright': '#f7f9fb',
        'surface-container-lowest': '#ffffff',
        'surface-container-low': '#f2f4f6',
        'surface-container': '#eceef0',
        'surface-container-high': '#e6e8ea',
        'surface-container-highest': '#e0e3e5',
        'surface-variant': '#e0e3e5',
        'on-surface': '#191c1e',
        'on-surface-variant': '#45474c',
        'inverse-surface': '#2d3133',
        'inverse-on-surface': '#eff1f3',

        outline: '#75777d',
        'outline-variant': '#c5c6cd',
        'surface-tint': '#545f73',
        background: '#f7f9fb',
        'on-background': '#191c1e'
      },
      fontFamily: {
        sans: ['Plus Jakarta Sans', 'Inter', 'sans-serif'],
        display: ['Plus Jakarta Sans', 'sans-serif'],
        headline: ['Plus Jakarta Sans', 'sans-serif'],
        body: ['Inter', 'sans-serif']
      }
    }
  },
  plugins: []
}
