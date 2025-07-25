<script setup lang="ts">
import { RouterLink, RouterView, useRoute } from 'vue-router';
import { useAuthStore } from './stores/auth';
import { computed } from 'vue';

const authStore = useAuthStore();
const isLoggedIn = computed(() => authStore.getIsAuthenticated);

const handleLogout = () => {
  authStore.logout();
};

const route = useRoute();
const showHeader = computed(() => route.path !== '/login');
</script>

<template>
  <div class="app-container">
    <header v-if="showHeader" class="bg-gray-800 text-white p-4 flex justify-between items-center header-height">
      <div class="flex items-center">
        <img alt="Vue logo" class="logo h-10 mr-4" src="./assets/logo.svg" />
        <nav>
          <RouterLink to="/" class="mr-4 hover:text-gray-300">Tarefas</RouterLink>
          <RouterLink v-if="!isLoggedIn" to="/login" class="hover:text-gray-300">Login</RouterLink>
        </nav>
      </div>
      <div v-if="isLoggedIn" class="flex items-center">
        <span class="mr-4">Bem-vindo, {{ authStore.name }}</span>
        <button @click="handleLogout" class="bg-red-500 hover:bg-red-700 text-white font-bold py-2 px-4 rounded">
          Sair
        </button>
      </div>
    </header>
    
    <main class="p-0 bg-dark-blue-bg main-content">
      <RouterView />
    </main>
  </div>
</template>

<style>
html, body, #app {
  height: 100%;
  margin: 0;
  overflow: hidden;
}

.app-container {
  display: flex;
  flex-direction: column;
  height: 100vh;
}

.header-height {
  height: 64px;
}

.main-content {
  flex-grow: 1;
  overflow-y: auto;
}
</style>
