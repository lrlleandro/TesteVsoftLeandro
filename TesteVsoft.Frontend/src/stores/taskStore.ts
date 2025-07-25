import { defineStore } from 'pinia';
import api from '../services/api';
import { useToast } from 'vue-toastification';
import { TaskModel } from '../interfaces/task';
import { AssignedUserModel } from '../interface/user';

export const useTaskStore = defineStore('task', {
  state: () => ({
    tasks: [] as TaskModel[],
    users: [] as AssignedUserModel[],
    isLoading: false,
    error: null as string | null
  }),

  getters: {
    filteredTasks: (state) => (status: string) => {
      return state.tasks.filter(task => task.status === status);
    }
  },

  actions: {
    async fetchTasks() {
      this.isLoading = true;
      try {
        const response = await api.post('/tasks/list', {
          page: 1,
          pageSize: 100,
          relations: ['assignedUser'],
          orderBys: [{
            propertyName: 'title',
            direction: 'Ascending'
          }]
        });
        this.tasks = response.data.items;
      } catch (error) {
        this.error = 'Erro ao buscar tarefas';
      } finally {
        this.isLoading = false;
      }
    },

    async fetchUsers() {
      try {
        const response = await api.post('/users/list', {
          page: 1,
          pageSize: 100
        });
        this.users = response.data;
      } catch (error) {
        this.error = 'Erro ao buscar usuários';
      }
    },

    async createOrUpdateTask(taskData: TaskModel) {
      try {
        taskData.dueDate = new Date(taskData.dueDate).toISOString();
        
        if (taskData.id) {
          await api.put(`/tasks/update`, taskData);
        } else {
          await api.post('/tasks/create', taskData);
        }
        await this.fetchTasks();
        return true;
      } catch (error) {
        this.error = 'Erro ao salvar tarefa';
        return false;
      }
    },

    async deleteTask(id: string) {
      try {
        await api.delete(`/tasks/${id}`);
        this.tasks = this.tasks.filter(task => task.id !== id);
        return true;
      } catch (error) {
        this.error = 'Erro ao excluir tarefa';
        return false;
      }
    }
  }
});