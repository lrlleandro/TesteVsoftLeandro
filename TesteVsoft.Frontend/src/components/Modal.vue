<template>
  <div v-if="isVisible" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full flex items-center justify-center">
    <div :class="['relative p-5 w-96 shadow-lg rounded-md', themeClasses]">
      <div class="flex justify-between items-center mb-4">
        <h3 class="text-lg font-semibold text-text-primary"><slot name="title"></slot></h3>
        <button @click="close" class="text-gray-400 hover:text-gray-200 text-xl leading-none font-semibold"><font-awesome-icon icon="times" /></button>
      </div>
      <slot></slot>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import { faTimes } from '@fortawesome/free-solid-svg-icons';

const props = defineProps({
  theme: {
    type: String,
    default: 'dark'
  }
});

const isVisible = ref(false);

const themeClasses = computed(() => {
  return props.theme === 'dark' ? 'bg-gray-800 text-text-primary' : 'bg-white text-gray-900';
});

const open = () => {
  isVisible.value = true;
};

const close = () => {
  isVisible.value = false;
};

defineExpose({
  open,
  close,
});
</script>