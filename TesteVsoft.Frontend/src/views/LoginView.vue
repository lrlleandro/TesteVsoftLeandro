<template>
  <div class="fixed inset-0 flex flex-col items-center justify-center overflow-hidden">
      <div class="flex flex-col items-center mb-8 w-full max-w-md px-8">
        <img src="../assets/logo.svg" alt="VSoft Logo" class="h-20 mb-8" />
        <h2 class="text-3xl font-bold text-center mb-4 text-text-primary">LOGIN</h2>
      </div>
      <form @submit.prevent="handleLogin" class="w-full max-w-md px-8">
        <div class="mb-6">
          <label for="userName" class="block text-text-secondary text-sm font-medium mb-2">Usuário</label>
          <div class="relative">
            <input
              type="text"
              id="userName"
              v-model="login.userName"
              class="shadow-lg appearance-none border-2 border-input-border rounded-lg w-full py-3 px-4 bg-input-bg text-text-primary leading-tight focus:outline-none focus:border-light-purple focus:ring-1 focus:ring-light-purple text-lg pl-10 transition-all duration-200"
              required
            />
            <span v-if="errors.userName" class="text-red-500 text-xs mt-1">{{ errors.userName }}</span>
            <font-awesome-icon icon="user" class="absolute left-3 top-1/2 transform -translate-y-1/2 text-text-secondary" />
          </div>
        </div>
        <div class="mb-8">
          <label for="password" class="block text-text-secondary text-sm font-medium mb-2">Senha</label>
          <div class="relative">
            <input
              type="password"
              id="password"
              v-model="login.password"
              class="shadow-lg appearance-none border-2 border-input-border rounded-lg w-full py-3 px-4 bg-input-bg text-text-primary leading-tight focus:outline-none focus:border-light-purple focus:ring-1 focus:ring-light-purple text-lg pl-10 transition-all duration-200"
              required
            />
            <span v-if="errors.password" class="text-red-500 text-xs mt-1">{{ errors.password }}</span>
            <font-awesome-icon icon="lock" class="absolute left-3 top-1/2 transform -translate-y-1/2 text-text-secondary" />
          </div>
        </div>
        <div class="flex items-center justify-between">
          <button
            type="submit"
            class="bg-button-bg hover:bg-button-hover text-text-primary font-medium py-3 px-6 rounded-lg focus:outline-none focus:ring-2 focus:ring-button-hover focus:ring-opacity-50 w-full text-lg transform hover:scale-105 transition-all duration-200 shadow-lg flex items-center justify-center"
          >
            <font-awesome-icon icon="sign-in-alt" class="mr-2" />
            Entrar
          </button>
        </div>
      </form>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { LoginMoldel } from '../interfaces/login';
import { useAuthStore } from '../stores/auth';
import { useToast } from 'vue-toastification';

const router = useRouter();
const authStore = useAuthStore();

const loginData : LoginMoldel = {
  userName: '',
  password: '',
};

const loginErrorData : LoginMoldel = {
  userName: '',
  password: '',
};

const login = ref(loginData);
const errors = ref(loginErrorData);

const validateForm = () => {
  let isValid = true;
  errors.value = { userName: '', password: '' };

  if (!userName.value) {
    errors.value.userName = 'O usuário é obrigatório.';
    isValid = false;
  }

  if (!password.value) {
    errors.value.password = 'A senha é obrigatória.';
    isValid = false;
  }

  return isValid;
};

const handleLogin = async () => {
  if (!validateForm()) {
    return;
  }

  try{
    await authStore.login(userName.value, password.value);
  } catch (error) {
    useToast().error('Erro ao fazer login. Verifique suas credenciais.');
  }

  router.push('/');
};
</script>