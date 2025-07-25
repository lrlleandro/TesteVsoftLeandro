<template>
  <div class="p-2 bg-dark-purple-bg rounded-md shadow-sm flex justify-between items-center mb-2">
    <div>
      <h3 class="text-base font-medium text-gray-400">{{ user.name }}</h3>
      <p class="text-xs text-text-secondary">{{ user.email }}</p>
    </div>
    <div class="flex space-x-2">
      <button @click="emit('edit-user', user)"
        class="p-1 bg-blue-800 text-white rounded-md text-sm hover:bg-blue-700 flex items-center justify-center" title="Editar Usuário">
        <font-awesome-icon :icon="faEdit" />
      </button>
      <button @click="emit('delete-user', user.id)"
        class="p-1 bg-red-600 text-white rounded-md text-sm hover:bg-red-700 flex items-center justify-center" title="Excluir Usuário">
        <font-awesome-icon :icon="faTrash" />
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { defineProps, defineEmits } from 'vue';
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import { faEdit, faTrash } from '@fortawesome/free-solid-svg-icons';
import { UserModel } from '../interfaces/user';

const props = defineProps({
  user: {
    type: Object as () => UserModel,
    required: true,
  },
});

const emit = defineEmits(['edit-user', 'delete-user']);
</script>