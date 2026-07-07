// @ts-check
import { defineConfig } from 'astro/config';
import react from '@astrojs/react';
import tailwindcss from '@tailwindcss/vite';

// https://astro.build/config
export default defineConfig({
  output: 'static', // no server adapter — pure static build, served by nginx
  site: 'https://dashboard.home.local', // placeholder, adjust per deployment
  integrations: [react()],
  vite: {
    plugins: [tailwindcss()],
  },
  redirects: {
    '/': '/air-quality',
  },
});
