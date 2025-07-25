<template>
  <CalendarRoot
    :model-value="modelValue"
    @update:model-value="(date) => $emit('update:modelValue', date)"
    :is-date-unavailable="isDateUnavailable"
    class="rdx-calendar-root"
    v-slot="{ weekDays, grid }"
  >
    <CalendarHeader class="rdx-calendar-header">
      <CalendarPrev class="rdx-calendar-nav-button" />
      <CalendarHeading class="rdx-calendar-heading" />
      <CalendarNext class="rdx-calendar-nav-button" />
    </CalendarHeader>

    <CalendarGrid class="rdx-calendar-grid">
      <CalendarGridHead class="rdx-calendar-grid-head">
        <CalendarGridRow class="rdx-calendar-grid-row">
          <CalendarHeadCell v-for="day in weekDays" :key="day" class="rdx-calendar-head-cell">
            {{ day }}
          </CalendarHeadCell>
        </CalendarGridRow>
      </CalendarGridHead>
      <CalendarGridBody class="rdx-calendar-grid-body">
        <CalendarGridRow v-for="(week, index) in grid" :key="index" class="rdx-calendar-grid-row">
          <CalendarCellTrigger
            v-for="(day, dateIndex) in week"
            :key="dateIndex"
            :day="day"
            :month="day.month"
            class="rdx-calendar-cell-trigger"
          />
        </CalendarGridRow>
      </CalendarGridBody>
    </CalendarGrid>
  </CalendarRoot>
</template>

<script setup lang="ts">
import { CalendarDate, getLocalTimeZone, today } from '@internationalized/date';
import { CalendarRoot, CalendarGrid, CalendarGridRow, CalendarGridHead, CalendarGridBody, CalendarHeadCell, CalendarHeader, CalendarNext, CalendarPrev, CalendarHeading, CalendarCellTrigger } from 'radix-vue';

const props = defineProps({
  modelValue: { // Usar modelValue para v-model
    type: Object as () => CalendarDate | undefined,
    default: undefined,
  },
});

const emit = defineEmits(['update:modelValue']);

const isDateUnavailable = (date: CalendarDate) => {
  if (!date || !(date instanceof CalendarDate)) return false;
  const todayDate = today(getLocalTimeZone());
  return date.compare(todayDate) < 0 || date.day === 6 || date.day === 0;
};
</script>

<style scoped>
.rdx-calendar-root {
  @apply mt-1 block w-full px-3 py-2 bg-input-bg border border-input-border rounded-md shadow-sm focus:outline-none focus:ring-light-blue focus:border-light-blue sm:text-sm;
}

.rdx-calendar-header {
  @apply flex justify-between items-center mb-4;
}

.rdx-calendar-nav-button {
  @apply text-text-primary p-2 rounded-md hover:bg-gray-700;
}

.rdx-calendar-heading {
  @apply text-text-primary text-lg font-semibold;
}

.rdx-calendar-grid {
  @apply w-full;
}

.rdx-calendar-grid-head {
  @apply text-text-primary;
}

.rdx-calendar-grid-row {
  @apply flex justify-around;
}

.rdx-calendar-head-cell {
  @apply text-text-primary text-sm font-medium py-2;
}

.rdx-calendar-grid-body {
  @apply text-text-primary;
}

.rdx-calendar-cell {
  @apply p-1;
}

.rdx-calendar-cell-trigger {
  @apply w-8 h-8 flex items-center justify-center rounded-full text-sm;
}

.rdx-calendar-cell-trigger[data-selected],
.rdx-calendar-cell-trigger[data-selected]:hover {
  @apply bg-blue-600 text-white;
}

.rdx-calendar-cell-trigger[data-unavailable],
.rdx-calendar-cell-trigger[data-unavailable]:hover {
  @apply text-gray-500 cursor-not-allowed;
}

.rdx-calendar-cell-trigger:not([data-selected]):not([data-unavailable]):hover {
  @apply bg-gray-200;
}
</style>