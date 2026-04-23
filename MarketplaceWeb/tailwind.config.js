/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{js,ts,jsx,tsx}'],
  theme: {
    extend: {
      colors: {
        ink: '#111827',
        cloud: '#f3f4f6',
        mint: '#059669',
        sunset: '#f97316',
      },
      boxShadow: {
        card: '0 10px 30px rgba(0, 0, 0, 0.08)',
      },
      fontFamily: {
        sans: ['Manrope', 'Segoe UI', 'Tahoma', 'sans-serif'],
      },
    },
  },
  plugins: [],
}
