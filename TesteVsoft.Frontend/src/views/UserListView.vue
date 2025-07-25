<template>
  <div class="kanban-board p-2 flex flex-col h-full mx-auto max-w-4xl">
    <div class="flex justify-between items-center mb-4 flex-shrink-0" ref="headerRef">
      <h2 class="text-2xl font-bold text-text-primary">Lista de Usuários</h2>
      <div class="flex space-x-2">
        <button @click="openRandomUsersModal()"
          class="px-4 py-2 bg-purple-600 text-white rounded-md hover:bg-purple-700 flex items-center justify-center" title="Gerar Usuários Aleatórios"><font-awesome-icon
            icon="random" /> </button>
        <button @click="openUserModal()"
          class="px-4 py-2 bg-green-600 text-white rounded-md hover:bg-green-700 flex items-center justify-center" title="Adicionar Novo Usuário"><font-awesome-icon
            icon="plus" /> </button>
      </div>
    </div>

    <div class="flex-grow overflow-y-auto" ref="userListContainer">
      <UserListItem v-for="(user, index) in users" :key="user.id" :user="user" @edit-user="openUserModal" @delete-user="deleteUser" :ref="el => { if (el) userListItemRefs[index] = el as HTMLElement; }" />

      <div v-if="loading" class="text-center py-4">
        <span class="text-text-secondary">Carregando usuários...</span>
      </div>
    </div>

    <div v-if="users.length > 0" class="flex justify-center items-center space-x-2 mt-2 flex-shrink-0" ref="paginationRef">
      <button @click="goToPage(page - 1)" :disabled="page === 1" class="px-3 py-1 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50">Anterior</button>
      <span class="text-text-primary">Página {{ page }} de {{ totalPages }}</span>
      <button @click="goToPage(page + 1)" :disabled="page === totalPages" class="px-3 py-1 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50">Próxima</button>
    </div>

    <Modal ref="userModal">
      <template #title>{{ currentUser ? 'Editar Usuário' : 'Adicionar Usuário' }}</template>
      <UserForm :user="currentUser" @user-saved="handleUserSaved" @user-canceled="closeUserModal" />
    </Modal>

    <Modal ref="confirmDeleteModal">
      <template #title>Confirmar Exclusão</template>
      <div class="p-4">
        <p class="text-text-primary mb-4">Tem certeza que deseja excluir este usuário?</p>
        <div class="flex justify-end space-x-2">
          <button @click="closeConfirmDeleteModal()" class="px-4 py-2 bg-gray-500 text-white rounded-md hover:bg-gray-600">Cancelar</button>
          <button @click="confirmDeleteUser()" class="px-4 py-2 bg-red-600 text-white rounded-md hover:bg-red-700">Excluir</button>
        </div>
      </div>
    </Modal>

    <RandomUserGeneratorModal ref="randomUsersModal" @users-generated="handleUsersGenerated" />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, nextTick, computed, onBeforeUnmount, watch } from 'vue';
import { useUserStore } from '../stores/userStore';
import Modal from '../components/Modal.vue';
import UserForm from '../components/UserForm.vue';
import RandomUserGeneratorModal from '../components/RandomUserGeneratorModal.vue';
import UserListItem from '../components/UserListItem.vue';
import { useToast } from 'vue-toastification';
import api from '../services/api';
import { UserModel } from '../interfaces/User';
import 'vue-toastification/dist/index.css';

const userStore = useUserStore();

const users = computed(() => userStore.getUsers);
const page = computed(() => userStore.getCurrentPage);
const totalPages = computed(() => userStore.getTotalPages);
const loading = computed(() => userStore.isLoading);
const userListContainer = ref<HTMLElement | null>(null);
const userListItemRefs = ref<HTMLElement[]>([]);
const headerRef = ref<HTMLElement | null>(null);
const paginationRef = ref<HTMLElement | null>(null);

const userModal = ref<InstanceType<typeof Modal> | null>(null);
const confirmDeleteModal = ref<InstanceType<typeof Modal> | null>(null);
const randomUsersModal = ref<InstanceType<typeof RandomUserGeneratorModal> | null>(null);
const currentUser = ref<User | null>(null);
const userToDeleteId = ref<string | null>(null);

const calculatePageSize = async () => {
  await nextTick();
  if (userListContainer.value && headerRef.value && paginationRef.value) {
    const windowHeight = window.innerHeight;
    const headerHeight = headerRef.value.offsetHeight;
    const paginationHeight = paginationRef.value.offsetHeight;
    const itemHeight = userListItemRefs.value.length > 0 ? userListItemRefs.value[0].offsetHeight : 60;
    const mainContainerPadding = 16;
    const availableHeight = windowHeight - headerHeight - paginationHeight - mainContainerPadding;

    if (itemHeight > 0) {
      const newPageSize = Math.floor(availableHeight / itemHeight);
      
      if (newPageSize > 0 && userStore.getPageSize !== newPageSize) {
        userStore.setPageSize(newPageSize);
      }
    }
  }
};

const handleResize = () => {
  calculatePageSize();
};

const fetchUsers = () => {
  try {
    userStore.fetchUsers();
  } catch (error) {
    useToast().error('Erro ao carregar os usuários.');
  }
};

const openUserModal = (user?: User) => {
  currentUser.value = user || null;
  userModal.value?.open();
};

const closeUserModal = () => {
  userModal.value?.close();
};

const toast = useToast();

const handleUserSaved = () => {
  closeUserModal();
  userStore.resetPage();
  toast.success('Usuário salvo com sucesso!');
};

const openConfirmDeleteModal = (id: string) => {
  userToDeleteId.value = id;
  confirmDeleteModal.value?.open();
};

const closeConfirmDeleteModal = () => {
  userToDeleteId.value = null;
  confirmDeleteModal.value?.close();
};

const confirmDeleteUser = async () => {
  if (userToDeleteId.value) {
    try {
      await api.delete(`/users/${userToDeleteId.value}`);
      userStore.fetchUsers();
      toast.success('Usuário excluído com sucesso!');
    } catch (error) {
      toast.error('Erro ao excluir usuário.');
    } finally {
      closeConfirmDeleteModal();
    }
  }
};

const deleteUser = (id: string) => {
  openConfirmDeleteModal(id);
};

const openRandomUsersModal = () => {
  randomUsersModal.value?.openModal();
};

const handleUsersGenerated = () => {
  userStore.resetPage();
};

onMounted(() => {
  fetchUsers();
  window.addEventListener('resize', handleResize);
});

watch(users, async () => {
  await nextTick();
  calculatePageSize();
}, { immediate: true });

onBeforeUnmount(() => {
  window.removeEventListener('resize', handleResize);
});

const goToPage = (newPage: number) => {
  try {
    userStore.setPage(newPage);  
  }
  catch (error) {
    useToast().error('Erro ao carregar a página.'); 
  }  
};
</script>