<template>
  <form @submit.prevent="saveUser" class="space-y-4">
    <div>
      <label for="hame" class="block text-sm font-medium text-text-primary">Nome:</label>
      <input type="text"
        id="name"
        v-model="user.name"
        class="mt-1 block w-full px-3 py-2 bg-gray-700 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm text-text-primary">
      <span v-if="errors.name" class="text-red-500 text-xs mt-1">{{ errors.name }}</span>
    </div>
    <div>
      <label for="userName" class="block text-sm font-medium text-text-primary">Nome de Usuário:</label>
      <input type="text"
        id="userName"
        v-model="user.userName"
        class="mt-1 block w-full px-3 py-2 bg-gray-700 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm text-text-primary">
      <span v-if="errors.userName" class="text-red-500 text-xs mt-1">{{ errors.userName }}</span>
    </div>
    <div>
      <label for="email" class="block text-sm font-medium text-text-primary">Email:</label>
      <input type="email"
        id="email"
        v-model="user.email"
        class="mt-1 block w-full px-3 py-2 bg-gray-700 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm text-text-primary">
      <span v-if="errors.email" class="text-red-500 text-xs mt-1">{{ errors.email }}</span>
    </div>
    <div v-if="isEditMode">
      <label for="oldPassword" class="block text-sm font-medium text-text-primary">Senha Antiga:</label>
      <input type="password"
        id="oldPassword"
        v-model="user.oldPassword"
        class="mt-1 block w-full px-3 py-2 bg-gray-700 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm text-text-primary">
      <span v-if="errors.oldPassword" class="text-red-500 text-xs mt-1">{{ errors.oldPassword }}</span>
    </div>
    <div>
      <label v-if="isEditMode" for="newPassword" class="block text-sm font-medium text-text-primary">Nova Senha:</label>
      <label v-else="isEditMode" for="newPassword" class="block text-sm font-medium text-text-primary">Senha:</label>
      <input type="password"
        id="newPassword"
        v-model="user.newPassword"
        class="mt-1 block w-full px-3 py-2 bg-gray-700 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm text-text-primary">
      <span v-if="errors.newPassword" class="text-red-500 text-xs mt-1">{{ errors.newPassword }}</span>
    </div>

    <button type="submit" class="w-full flex justify-center items-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500">
      <font-awesome-icon :icon="isEditMode ? 'save' : 'plus'" class="mr-2" />
      {{ isEditMode ? 'Atualizar Usuário' : 'Criar Usuário' }}
    </button>
  </form>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import api from '../services/api';
import { UserFormModel } from '../interfaces/user';
import { defineProps, defineEmits } from 'vue';

const props = defineProps<{ user: UserForm | null }>();
const emit = defineEmits(['user-saved', 'user-canceled']);

const errors : UserFormModel = ref({
  email: '',
  userName: '',
  password: '',
  oldPassword: '',
  newPassword: '',
});

const user : UserFormModel = ref<UserForm>({ 
  email: '',
  userName: '',
  password: '',
  oldPassword: '',
  newPassword: '',
 });

const isEditMode = computed(() => !!user.value.id);

watch(() => props.user, (newUser) => {
  if (newUser) {
    user.value = { ...newUser, oldPassword: '' };
  } else {
    user.value = { name: '', email: '', userName: '', oldPassword: '', newPassword: '' };
  }
}, { immediate: true });

const validateForm = () => {
  let isValid = true;
  errors.value = { email: '', userName: '', password: '', oldPassword: '', newPassword: '' };

  if (!user.value.email) {
    errors.value.email = 'O email é obrigatório.';
    isValid = false;
  } else if (!/^[\w-.]+@([\w-]+\.)+[\w-]{2,4}$/.test(user.value.email)) {
    errors.value.email = 'Formato de email inválido.';
    isValid = false;
  }

  if (!user.value.userName) {
    errors.value.userName = 'O nome de usuário é obrigatório.';
    isValid = false;
  }

  if (!isEditMode.value && !user.value.newPassword) {
    errors.value.newPassword = 'A senha é obrigatória.';
    isValid = false;
  }

  if (isEditMode.value && (user.value.oldPassword || user.value.newPassword)) {
    if (!user.value.oldPassword) {
      errors.value.oldPassword = 'A senha antiga é obrigatória para alterar a senha.';
      isValid = false;
    }
    if (!user.value.newPassword) {
      errors.value.newPassword = 'A nova senha é obrigatória para alterar a senha.';
      isValid = false;
    }
  }

  return isValid;
};

const saveUser = async () => {
  if (!validateForm()) {
    return;
  }
  try {
    if (isEditMode.value) {
      const payload = {
        id: user.value.id,
        name: user.value.name,
        email: user.value.email,
        userName: user.value.userName,
        oldPassword: user.value.oldPassword,
        newPassword: user.value.newPassword
      };
      await api.put(`/users/${user.value.id}`, payload);
    } else {
      const payload = {
        name: user.value.name,
        email: user.value.email,
        userName: user.value.userName,
        password: user.value.newPassword
      };
      await api.post('/users/create', payload);
    }
    emit('user-saved');
  } catch (error) {
    if (error.response && error.response.data && error.response.data.errors) {
      for (const key in error.response.data.errors) {
        if (errors.value.hasOwnProperty(key)) {
          errors.value[key] = error.response.data.errors[key][0];
        }
      }
    }
  }
};

const cancel = () => {
  emit('user-canceled');
};
</script>