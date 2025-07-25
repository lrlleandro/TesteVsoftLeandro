<template>
  <div class="kanban-board p-4 min-h-screen">
    <div class="flex justify-between items-center mb-4">
      <h2 class="text-2xl font-bold text-text-primary">Quadro Kanban</h2>
      <button @click="$emit('create-task')"
        class="px-4 py-2 bg-green-600 text-white rounded-md hover:bg-green-700 flex items-center justify-center"
        title="Criar Nova Tarefa"><font-awesome-icon icon="plus" /></button>
    </div>
    <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
      <div v-for="status in taskStatuses" :key="status.value"
        class="kanban-column bg-dark-purple-bg p-4 rounded-lg shadow-md h-full"
        @dragover.prevent="dragOver(status.value)" @dragleave="dragLeave(status.value)" @drop="drop(status.value)"
        :class="{ 'border-blue-500 border-2': currentDragOverStatus === status.value }">
        <h3 class="text-xl font-semibold mb-3 text-text-primary">{{ status.label }}</h3>
        <div class="task-list space-y-3">
          <div v-for="task in filteredTasks(status.value)" :key="task.id"
            class="task-card bg-card-bg p-3 rounded-md shadow-sm relative" draggable="true"
            @dragstart="dragStart(task)">
            <div class="absolute top-2 right-2 flex space-x-1">
              <button @click="$emit('edit-task', task)"
                class="p-1 bg-blue-800 text-white rounded-md text-sm hover:bg-blue-700 flex items-center justify-center"
                title="Editar Tarefa"><font-awesome-icon icon="edit" /></button>
              <button @click="$emit('delete-task', task.id)"
                class="p-1 bg-red-600 text-white rounded-md text-sm hover:bg-red-700 flex items-center justify-center"
                title="Excluir Tarefa"><font-awesome-icon icon="trash" /></button>
            </div>
            <h4 class="font-bold text-text-primary">{{ task.title }}</h4>
            <p class="text-sm text-text-secondary max-h-20 overflow-y-auto whitespace-pre-wrap break-words">{{ task.description }}</p>
            <p class="text-xs text-white mt-2">Data de Vencimento: {{ formatDateToDDMMYYYY(task.dueDate) }}</p>
            <p class="text-xs text-white mt-2">Responsável: {{ task.assignedUser?.name ?? 'Nenhum' }}</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, defineProps, defineEmits } from 'vue';
import api from '../services/api';
import { useToast } from 'vue-toastification';
import { useTaskStore } from '../stores/taskStore';
import { Task } from '../interfaces/task';

const props = defineProps({
  tasks: { type: Array as () => Task[], required: true },
});

const emit = defineEmits(['create-task', 'edit-task', 'delete-task', 'update-task-status']);

const formatDateToDDMMYYYY = (dateString: string) => {
  if (!dateString) return '';
  const date = new Date(dateString);
  const day = date.getDate().toString().padStart(2, '0');
  const month = (date.getMonth() + 1).toString().padStart(2, '0');
  const year = date.getFullYear();
  return `${day}/${month}/${year}`;
};

const taskStore = useTaskStore();

const taskStatuses = [
  { label: 'Pendente', value: 'Pending' },
  { label: 'Em Progresso', value: 'InProgress' },
  { label: 'Concluído', value: 'Completed' },
];

const filteredTasks = computed(() => (status: string) => {
  return props.tasks.filter(task => task.status === status);
});

const currentDragTask = ref<Task | null>(null);
const currentDragOverStatus = ref<string | null>(null);

const dragStart = (task: Task) => {
  currentDragTask.value = task;
};

const dragOver = (status: string) => {
  currentDragOverStatus.value = status;
};

const dragLeave = (status: string) => {
  if (currentDragOverStatus.value === status) {
    currentDragOverStatus.value = null;
  }
};

const drop = async (newStatus: string) => {
  if (currentDragTask.value) {
    const updatedTask = { ...currentDragTask.value, status: newStatus };
    try {
      await api.post(`/tasks/status/change`, {
        id: updatedTask.id,
        status: updatedTask.status
      });
      emit('update-task-status', updatedTask);
      useToast().success('Status da tarefa atualizado com sucesso!');
    } catch (error) {
      useToast().error('Erro ao atualizar status da tarefa.');
    } finally {
      currentDragTask.value = null;
      currentDragOverStatus.value = null;
    }
  }
};
</script>

<style scoped>
.kanban-board {
  width: 100%;
  max-width: 1200px;
  margin: 0 auto;
}
</style>