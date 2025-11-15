/** @type {import('tailwindcss').Config} */
export default {
  darkMode: 'class',
  content: ['./index.html', './src/**/*.{js,jsx,ts,tsx}'],
  theme: {
    extend: {
      colors: {
        brand: {
          50: '#f5f7ff',
          100: '#eef2ff',
          300: '#c7d2fe',
          500: '#4f46e5', // --primary
          600: '#3730a3'
        },
        accent: {
          400: '#06b6d4'
        }
      },
      fontFamily: {
        sans: ['CustomFont', 'Inter', 'system-ui']
      },
      borderRadius: {
        xl: '12px'
      }
    },
  },
  plugins: [],
};
