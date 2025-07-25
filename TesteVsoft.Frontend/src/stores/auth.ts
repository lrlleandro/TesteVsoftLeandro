import { defineStore } from 'pinia';
import api from '../services/api';
import router from '../router';

export const useAuthStore = defineStore('auth', {
  persist: true,
  state: () => ({
    isAuthenticated: false,
    userName: null as string | null,
    name: null as string | null,
    userId: null as string | null,
    accessToken: null as string | null,
    expiresIn: null as number | null,
  }),
  actions: {
    async login(userName: string, password: string) {
      try {
        const response = await api.post('/login', { userName, password });

        if (response.status === 200 && response.data.accessToken) {
          this.isAuthenticated = true;
          this.accessToken = response.data.accessToken;
          this.expiresIn = response.data.expiresIn;
          this.userName = response.data.userName;
          this.name = response.data.name;
          this.userId = response.data.userId;
        }
      } catch (error) {
        this.isAuthenticated = false;
        this.accessToken = null;
        this.expiresIn = null;
        this.userName = null;
        this.name = null;
        this.userId = null;
        throw error;
      }
    },
    logout() {
      this.isAuthenticated = false;
      this.userName = null;
      this.name = null;
      this.userId = null;
      this.accessToken = null;
      this.expiresIn = null;
      router.push('/login');
    },
  },
  getters: {
    getIsAuthenticated: (state) => state.isAuthenticated,
    getUserName: (state) => state.userName,
    getName: (state) => state.name,
    getUserId: (state) => state.userId,
    getAccessToken: (state) => state.accessToken,
    getExpiresIn: (state) => state.expiresIn,
  },
});