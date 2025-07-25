import { defineStore } from 'pinia';
import api from '../services/api';
import { UserNodel } from '../interfaces/User';

export const useUserStore = defineStore('user', {
  state: () => ({
    users: [] as UserNodel[],
    page: 1,
    pageSize: 10,
    totalPages: 1,
    loading: false,
  }),
  actions: {
    async fetchUsers() {
      if (this.loading) return;
      this.loading = true;
      try {
        const response = await api.post('/users/list', {
          page: this.page,
          pageSize: this.pageSize,
          orderBys: [{
            propertyName: 'name',
            direction: 'Ascending'
          }]
        });
        this.users = response.data.items;
        this.page = response.data.page;
        this.totalPages = response.data.totalPages;
      } catch (error) {
        throw error;
      } finally {
        this.loading = false;
      }
    },
    setPage(newPage: number) {
      if (newPage >= 1 && newPage <= this.totalPages) {
        this.page = newPage;
        this.fetchUsers();
      }
    },
    resetPage() {
      this.page = 1;
      this.fetchUsers();
    },
    setPageSize(newSize: number) {
      if (this.pageSize !== newSize) {
        this.pageSize = newSize;
        this.page = 1;
        this.fetchUsers();
      }
    },
  },
  getters: {
    getUsers: (state) => state.users,
    getCurrentPage: (state) => state.page,
    getTotalPages: (state) => state.totalPages,
    isLoading: (state) => state.loading,
    getPageSize: (state) => state.pageSize,
  },
});