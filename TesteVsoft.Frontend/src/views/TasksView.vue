<template>
  <div class="tasks-board p-4 min-h-screen">
    <KanbanBoard
      :tasks="taskStore.tasks"
      @create-task="createTask"
      @edit-task="editTask"
      @delete-task="deleteTask"
      @update-task-status="handleUpdateTaskStatus"
    />

    <Modal ref="modalRef" @close="closeModal">
      <h3 class="text-xl font-semibold mb-4 text-text-primary">{{ modalTitle }}</h3>
      <TaskForm :initialTask="currentTask" @submit="handleTaskSubmit" ref="taskFormRef" />
    </Modal>

    <Modal ref="confirmModalRef" @close="cancelDelete">
      <div class="p-4">
        <h3 class="text-xl font-semibold mb-4 text-text-primary">Confirmar Exclusão</h3>
        <p class="text-text-secondary mb-6">Tem certeza de que deseja excluir esta tarefa? Esta ação não pode ser desfeita.</p>
        <div class="flex justify-end space-x-4">
          <button @click="cancelDelete" class="px-4 py-2 bg-gray-600 text-white rounded-md hover:bg-gray-700"
            title="Cancelar"><font-awesome-icon icon="times" /></button>
          <button @click="confirmDelete" class="px-4 py-2 bg-red-600 text-white rounded-md hover:bg-red-700"
            title="Excluir"><font-awesome-icon icon="trash" /></button>
        </div>
      </div>
    </Modal>
  </div>
</template>

<script setup lang="ts">
import KanbanBoard from '../components/KanbanBoard.vue';
import { ref, onMounted } from 'vue';
import { useAuthStore } from '../stores/auth';
import { useTaskStore } from '../stores/taskStore';
import Modal from '../components/Modal.vue';
import TaskForm from '../components/TaskForm.vue';
import { Task } from '../interfaces/task';

const authStore = useAuthStore();
const taskStore = useTaskStore();
const currentTask = ref<Task | null>(null);
const modalTitle = ref('');
const modalRef = ref<InstanceType<typeof Modal> | null>(null);
const taskFormRef = ref<InstanceType<typeof TaskForm> | null>(null);
const confirmModalRef = ref<InstanceType<typeof Modal> | null>(null);
const taskIdToDelete = ref<string | null>(null);

const createTask = () => {
  currentTask.value = null;
  modalTitle.value = 'Criar Nova Tarefa';
  modalRef.value?.open();
};

const editTask = (task: Task) => {
  currentTask.value = { ...task };
  modalTitle.value = 'Editar Tarefa';
  modalRef.value?.open();
};

const handleTaskSubmit = async (taskData: Task) => {
  const success = await taskStore.createOrUpdateTask(taskData);
  if (success) {
    modalRef.value?.close();
  }
};

const deleteTask = (id: string) => {
  taskIdToDelete.value = id;
  confirmModalRef.value?.open();
};

const confirmDelete = async () => {
  if (taskIdToDelete.value) {
    const success = await taskStore.deleteTask(taskIdToDelete.value);
    if (success) {
      confirmModalRef.value?.close();
      taskIdToDelete.value = null;
    }
  }
};

const cancelDelete = () => {
  confirmModalRef.value?.close();
  taskIdToDelete.value = null;
};

const closeModal = () => {
  modalRef.value?.close();
  taskFormRef.value?.resetForm();
};

const handleUpdateTaskStatus = (updatedTask: Task) => {
  const index = taskStore.tasks.findIndex(t => t.id === updatedTask.id);
  if (index !== -1) {
    taskStore.tasks[index].status = updatedTask.status;
  }
};

onMounted(() => {
  if (authStore.getIsAuthenticated) {
    taskStore.fetchTasks();
    taskStore.fetchUsers();
  }
});
</script>

<style scoped>
.tasks-board {
  width: 100%;
  max-width: 1200px;
  margin: 0 auto;
}
</style>