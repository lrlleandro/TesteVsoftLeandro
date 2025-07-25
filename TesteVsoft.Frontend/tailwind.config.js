/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        'dark-blue-bg': '#0A1128',
        'dark-purple-bg': '#1C1B33',
        'light-purple': '#7E3AF2',
        'light-blue': '#63B3ED',
        'input-bg': '#1A1F36',
        'input-border': '#2D3748',
        'card-bg': '#2A2F40',
        'button-bg': '#7E3AF2',
        'button-hover': '#6C2BD9',
        'text-primary': '#FFFFFF',
        'text-secondary': '#A0AEC0'
      },
    },
  },
  plugins: [],
  darkMode: 'class',
}