<template>
  <Modal ref="randomUsersModal">
    <template #title>Gerar Usuários Aleatórios</template>
    <div class="p-4">
      <div class="mb-4">
        <label class="block text-text-primary mb-2">Quantidade de usuários</label>
        <input v-model="count" type="number" min="1" max="100"
          class="w-full p-2 rounded-md bg-gray-700 text-text-primary">
      </div>
      <div class="mb-4">
        <label class="block text-text-primary mb-2">Máscara do username</label>
        <input v-model="usernameMask" type="text"
          class="w-full p-2 rounded-md bg-gray-700 text-text-primary">
      </div>
      <div class="flex justify-end space-x-2">
        <button @click="closeModal()" class="px-4 py-2 bg-gray-500 text-white rounded-md hover:bg-gray-600">Cancelar</button>
        <button @click="generateUsers()" class="px-4 py-2 bg-purple-600 text-white rounded-md hover:bg-purple-700">Gerar</button>
      </div>
    </div>
  </Modal>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import Modal from './Modal.vue';
import api from '../services/api';
import { useToast } from 'vue-toastification';

const emit = defineEmits(['users-generated', 'modal-closed']);

const randomUsersModal = ref<InstanceType<typeof Modal> | null>(null);
const count = ref(5);
const usernameMask = ref('{{random}}');
const toast = useToast();

const openModal = () => {
  randomUsersModal.value?.open();
};

const closeModal = () => {
  randomUsersModal.value?.close();
  emit('modal-closed');
};

const generateUsers = async () => {
  try {
    if (!usernameMask.value) {
      toast.error('A máscara do username é obrigatória.');
      return;
    }

    if (!usernameMask.value.includes('{{random}}')) {
      toast.error('A máscara deve conter o marcador {{random}} para gerar valores aleatórios.');
      return;
    }

    await api.post('/users/createRandom', {
      amount: count.value,
      userNameMask: usernameMask.value
    });

    toast.success('A criação dos usuários começou em segundo plano.');
    emit('users-generated');
    closeModal();
  } catch (error) {
    toast.error(error.message);
  }
};

defineExpose({
  openModal,
  closeModal
});
</script>