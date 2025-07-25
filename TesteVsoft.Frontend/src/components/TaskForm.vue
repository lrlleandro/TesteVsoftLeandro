<template>
  <form @submit.prevent="submitForm" class="space-y-4">
    <div>
      <label for="title" class="block text-sm font-medium text-text-primary">Título</label>
      <input type="text" id="title" v-model="task.title" required
             class="mt-1 block w-full px-3 py-2 bg-gray-700 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm text-text-primary">
      <span v-if="errors.title" class="text-red-500 text-xs mt-1">{{ errors.title }}</span>
    </div>
    <div>
      <label for="description" class="block text-sm font-medium text-text-primary">Descrição</label>
      <textarea id="description" v-model="task.description" required
             class="mt-1 block w-full px-3 py-2 bg-gray-700 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm text-text-primary"></textarea>
      <span v-if="errors.description" class="text-red-500 text-xs mt-1">{{ errors.description }}</span>
    </div>
    <div>
      <label for="dueDate" class="block text-sm font-medium text-text-primary">Data de Vencimento</label>
      <Calendar v-model:selectedDate="task.dueDate" :minDate="todayFormatted" />
      <span v-if="errors.dueDate" class="text-red-500 text-xs mt-1">{{ errors.dueDate }}</span>
    </div>

    <div v-if="isEditMode">
      <label for="assignedUser" class="block text-sm font-medium text-text-primary">Responsável</label>
      <select id="assignedUser" v-model="selectedAssignedUserId"
             class="mt-1 block w-full px-3 py-2 bg-gray-700 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm text-text-primary"
             @scroll="handleScroll">
        <option :value="null">Nenhum</option>
        <option v-for="user in users" :key="user.id" :value="user.id">{{ user.name }}</option>
        <option v-if="loadingUsers" disabled>Carregando mais usuários...</option>
      </select>
    </div>

    <div v-if="isEditMode">
      <label for="status" class="block text-sm font-medium text-text-primary">Status</label>
      <select id="status" v-model="task.status"
             class="mt-1 block w-full px-3 py-2 bg-gray-700 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm text-text-primary">
        <option value="Pending">Pendente</option>
        <option value="InProgress">Em Progresso</option>
        <option value="Completed">Concluído</option>
      </select>
    </div>
    <button type="submit" class="w-full flex justify-center items-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500">
      <font-awesome-icon :icon="isEditMode ? 'save' : 'plus'" class="mr-2" />
      {{ isEditMode ? 'Atualizar Tarefa' : 'Criar Tarefa' }}
    </button>
  </form>
</template>

<script setup lang="ts">
import { ref, watch, onMounted, computed } from 'vue';
import api from '../services/api';
import Calendar from './Calendar.vue';
import { useToast } from 'vue-toastification';
import { TaskModel } from '../interfaces/task';
import { AssignedUserModel } from '../interfaces/user';

const formatDateToYYYYMMDD = (date) => {
  const year = date.getFullYear();
  const month = (date.getMonth() + 1).toString().padStart(2, '0');
  const day = date.getDate().toString().padStart(2, '0');
  return `${year}-${month}-${day}`;
};

const today = new Date();
const todayFormatted = computed(() => formatDateToYYYYMMDD(today));

const props = defineProps({
  initialTask: {
    type: Object as () => Task | null,
    default: null,
  },
});

const emit = defineEmits(['submit']);

const errors : TaskModel = ref({
  title: '',
  description: '',
  dueDate: '',
});

const users = ref<AssignedUserModal[]>([]);
const selectedAssignedUserId = ref<string | null>(null);
const currentPage = ref(1);
const hasMoreUsers = ref(true);
const loadingUsers = ref(false);

const task : TaskModel = ref<Task>(props.initialTask ? { ...props.initialTask } : {
  title: '',
  description: '',
  dueDate: null,
  status: 'Pending',
  assignedUser: null,
});

if (props.initialTask?.assignedUser) {
  selectedAssignedUserId.value = props.initialTask.assignedUser.id;
}

const isEditMode = ref(!!props.initialTask);

watch(() => props.initialTask, (newTask) => {
  if (newTask) {
    task.value = { ...newTask };
    if (!newTask.dueDate) {
      task.value.dueDate = null;
    }
    if (newTask.assignedUser) {
      selectedAssignedUserId.value = newTask.assignedUser.id;
    } else {
      selectedAssignedUserId.value = null;
    }
    isEditMode.value = true;
  } else {
    task.value = {
      title: '',
      description: '',
      dueDate: null,
      status: 'Pending',
      assignedUser: null,
    };
    selectedAssignedUserId.value = null;
    isEditMode.value = false;
  }
}, { deep: true });

const fetchUsers = async (page: number) => {
  if (loadingUsers.value || !hasMoreUsers.value) return;
  loadingUsers.value = true;

  do
  {
    try {
      const response = await api.post('/users/list', {
        page: page,
        pageSize: 100,
        sorts: [{
          propertyName: 'name',
          direction: 'Ascending',
        }],
      });
      const newUsers = response.data.items;
      users.value = [...users.value, ...newUsers];
      hasMoreUsers.value = response.data.totalPages > response.data.pageNumber;
      currentPage.value = response.data.pageNumber;
    } catch {
      useToast().error("Erro ao buscar usuários");
    } finally {
      loadingUsers.value = false;
    }
  } while(hasMoreUsers.value === true);
};

onMounted(() => {
  fetchUsers(currentPage.value);
});

const validateForm = () => {
  let isValid = true;
  errors.value = { title: '', description: '', dueDate: '' };

  if (!task.value.title) {
    errors.value.title = 'O título é obrigatório.';
    isValid = false;
  }

  if (!task.value.description) {
    errors.value.description = 'A descrição é obrigatória.';
    isValid = false;
  }

  if (!task.value.dueDate) {
    errors.value.dueDate = 'A data de vencimento é obrigatória.';
    isValid = false;
  }

  return isValid;
};

const submitForm = () => {
  if (!validateForm()) {
    return;
  }
  const taskToSubmit = { ...task.value };
  
  if (taskToSubmit.dueDate) {
    taskToSubmit.dueDate = new Date(taskToSubmit.dueDate).toISOString().split('T')[0];
  } else {
    taskToSubmit.dueDate = '';
  }
  
  if (selectedAssignedUserId.value) {
    taskToSubmit.assignedUserId = selectedAssignedUserId.value;
    taskToSubmit.assignedUser = users.value.find(u => u.id === selectedAssignedUserId.value) || null;
  } else {
    taskToSubmit.assignedUserId = null;
    taskToSubmit.assignedUser = null;
  }

  emit('submit', taskToSubmit);
};

defineExpose({
  resetForm: () => {
    task.value = {
      title: '',
      description: '',
      dueDate: null,
      status: 'Pending',
      assignedUser: null,
    };
    currentPage.value = 1;
    hasMoreUsers.value = true;
    users.value = [];
    fetchUsers(currentPage.value);
  }
});

const handleScroll = (event: Event) => {
  const target = event.target as HTMLElement;
  if (target.scrollTop + target.clientHeight >= target.scrollHeight - 50 && hasMoreUsers.value && !loadingUsers.value) {
    fetchUsers(currentPage.value + 1);
  }
};

</script>