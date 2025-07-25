import axios from 'axios';
import router from '@/router';
import { useAuthStore } from '@/stores/auth';
import { useToast } from 'vue-toastification';

const API_BASE_URL = 'https://localhost:7511';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

api.interceptors.request.use(
  (config) => {
    const authStore = useAuthStore();
    if (authStore.getAccessToken) {
      config.headers.Authorization = `Bearer ${authStore.getAccessToken}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

api.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    const authStore = useAuthStore();
    if (error.response) {
      const { status, data } = error.response;

      if (data && data.errors) {
        const errorMessages = Object.values(data.errors).flat().join('\n');
        useToast().error(`Erros de validação:\n${errorMessages}`);
      }

      if (status === 401 || status === 403) {
        authStore.logout();
        router.push('/login');
      }
    }
    return Promise.reject(error);
  }
);

export default api;