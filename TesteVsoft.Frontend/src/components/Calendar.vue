<template>
  <div class="calendar-container mt-1 block w-full">
    <div
      class="date-input px-3 py-2 bg-gray-700 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm text-text-primary">
      <input type="text" :value="state.selectedDateString" @input="handleInput" @blur="handleBlur"
        class="bg-transparent outline-none w-full" ref="dateInputRef" />
      <span class="calendar-icon" @click="toggleCalendar">📅</span>
    </div>
    <span v-if="state.dateError" class="error-message text-red-500 text-sm mt-1">{{ state.dateError }}</span>

    <div class="calendar" v-if="state.showCalendar" :style="calendarPosition" ref="calendarRef">
      <div class="header">
        <button type="button" @click="prevMonth">&lt;</button>
        <h2>{{ currentMonthName }} {{ currentYear }}</h2>
        <button type="button" @click="nextMonth">&gt;</button>
      </div>

      <div class="weekdays">
        <div v-for="day in weekdays" :key="day">{{ day }}</div>
      </div>
      <div class="days">
        <div v-for="day in daysInMonth" :key="day ? day.dateString : 'empty-' + Math.random()" :class="{
          day: day,
          'empty-day': !day,
          'current-day': day && isCurrentDay(day.date),
          'selected-day': day && isSelectedDay(day.date),
          'disabled-day': day && isBeforeMinDate(day.date),
        }" @click="day && !isBeforeMinDate(day.date) && selectDay(day.date)">
          {{ day ? day.dayOfMonth : '' }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed, watch, onMounted, onUnmounted } from 'vue';

const formatDateToYYYYMMDD = (date) => {
  const year = date.getFullYear();
  const month = (date.getMonth() + 1).toString().padStart(2, '0');
  const day = date.getDate().toString().padStart(2, '0');
  return `${year}-${month}-${day}`;
};

const formatDateToDDMMYYYY = (date) => {
  if (!date) return '';
  const day = date.getDate().toString().padStart(2, '0');
  const month = (date.getMonth() + 1).toString().padStart(2, '0');
  const year = date.getFullYear();
  return `${day}/${month}/${year}`;
};

const isSameDate = (date1, date2) => {
  if (!date1 && !date2) return true;
  if (!date1 || !date2) return false;
  return date1.getFullYear() === date2.getFullYear() &&
    date1.getMonth() === date2.getMonth() &&
    date1.getDate() === date2.getDate();
};

const props = defineProps({
  selectedDate: { type: String, default: null },
  minDate: { type: String, default: null },
});
const emit = defineEmits(['update:selectedDate']);

const state = reactive({
  dateError: '',
  selectedDateObject: null,
  currentDate: null,
  selectedDateString: '',
  showCalendar: false,
});

const createLocalDate = (dateString) => {
  if (!dateString) return null;
  const parts = dateString.split('-');

  return new Date(parseInt(parts[0]), parseInt(parts[1]) - 1, parseInt(parts[2]));
};

const today = new Date();
const todayFormatted = formatDateToYYYYMMDD(today);
const minSelectableDate = computed(() => createLocalDate(props.minDate || todayFormatted));

state.selectedDateObject = createLocalDate(props.selectedDate);
state.currentDate = state.selectedDateObject || new Date(today.getFullYear(), today.getMonth(), today.getDate());
state.selectedDateString = formatDateToDDMMYYYY(state.selectedDateObject);
state.showCalendar = false;

const dateInputRef = ref(null);
const calendarRef = ref(null);

watch(() => props.selectedDate, (newVal) => {
  const newDate = createLocalDate(newVal);
  if (!isSameDate(state.selectedDateObject, newDate)) {
    state.selectedDateObject = newDate;
    state.selectedDateString = formatDateToDDMMYYYY(state.selectedDateObject);
    if (newVal) {
      state.currentDate = new Date(newVal);
    } else {
      state.currentDate = new Date();
    }
  }
});

watch(() => state.selectedDateObject, (newVal) => {
  const formattedNewVal = newVal ? formatDateToYYYYMMDD(newVal) : null;

  if (props.selectedDate !== formattedNewVal) {
    emit('update:selectedDate', formattedNewVal);
  }
  state.selectedDateString = formatDateToDDMMYYYY(newVal);
}, { immediate: true });

const weekdays = ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sáb'];

const currentMonthName = computed(() => {
  return state.currentDate.toLocaleString('pt-BR', { month: 'long' });
});

const currentYear = computed(() => {
  return state.currentDate.getFullYear();
});

const daysInMonth = computed(() => {
  const year = state.currentDate.getFullYear();
  const month = state.currentDate.getMonth();
  const firstDayOfMonth = new Date(year, month, 1, 0, 0, 0);
  const lastDayOfMonth = new Date(year, month + 1, 0, 0, 0, 0);
  const numDays = lastDayOfMonth.getDate();

  const days = [];
  const startDay = firstDayOfMonth.getDay();

  for (let i = 0; i < startDay; i++) {
    days.push(null);
  }

  for (let i = 1; i <= numDays; i++) {
    const date = new Date(year, month, i, 0, 0, 0);
    days.push({
      dayOfMonth: i,
      date: date,
      dateString: formatDateToYYYYMMDD(date),
    });
  }

  return days;
});

const prevMonth = () => {
  const newDate = new Date(
    state.currentDate.getFullYear(),
    state.currentDate.getMonth() - 1,
    1
  );

  if (newDate.getFullYear() < minSelectableDate.getFullYear() ||
    (newDate.getFullYear() === minSelectableDate.getFullYear() &&
      newDate.getMonth() < minSelectableDate.getMonth())) {
    return;
  }
  state.currentDate = new Date(
    state.currentDate.getFullYear(),
    state.currentDate.getMonth() - 1,
    1
  );
};

const nextMonth = () => {
  state.currentDate = new Date(
    state.currentDate.getFullYear(),
    state.currentDate.getMonth() + 1,
    1
  );
};

const isCurrentDay = (date) => {
  const today = new Date();
  return (
    date.getDate() === today.getDate() &&
    date.getMonth() === today.getMonth() &&
    date.getFullYear() === today.getFullYear()
  );
};

const isSelectedDay = (date) => {
  if (!state.selectedDateObject) return false;
  return (
    date.getDate() === state.selectedDateObject.getDate() &&
    date.getMonth() === state.selectedDateObject.getMonth() &&
    state.selectedDateObject.getFullYear() === date.getFullYear()
  );
};

const isBeforeMinDate = (date) => {
  if (!minSelectableDate) return false;

  const dateWithoutTime = new Date(date.getFullYear(), date.getMonth(), date.getDate());
  const minDateWithoutTime = new Date(minSelectableDate.value.getFullYear(), minSelectableDate.value.getMonth(), minSelectableDate.value.getDate());
  return dateWithoutTime.getTime() < minDateWithoutTime.getTime();
};

const selectDay = (date) => {
  console.log('selectDay - Date clicked:', date, ' (Timestamp:', date.getTime(), ')');
  state.selectedDateObject = new Date(date.getFullYear(), date.getMonth(), date.getDate());
  state.currentDate = new Date(date.getFullYear(), date.getMonth(), date.getDate());
  state.showCalendar = false;
};

const goToSelectedDate = () => {
  if (state.selectedDateObject) {
    state.currentDate = state.selectedDateObject;
  }
};

const handleInput = (event) => {
  state.selectedDateString = event.target.value;
  state.dateError = '';
  state.showCalendar = true;
};

const handleBlur = () => {

  const dateString = state.selectedDateString;
  state.showCalendar = false;
  if (!dateString) {
    state.dateError = 'A data de vencimento é obrigatória.';
    emit('update:selectedDate', null);
    return;
  }

  const parts = dateString.split('/');
  if (parts.length !== 3) {
    state.dateError = 'Formato de data inválido. Use DD/MM/YYYY.';
    emit('update:selectedDate', null);
    return;
  }

  const day = parseInt(parts[0], 10);
  const month = parseInt(parts[1], 10) - 1;
  const year = parseInt(parts[2], 10);

  const date = new Date(year, month, day);

  if (isNaN(date.getTime()) || date.getDate() !== day || date.getMonth() !== month || date.getFullYear() !== year) {
    state.dateError = 'Data inválida.';
    emit('update:selectedDate', null);
    return;
  }

  if (isBeforeMinDate(date)) {
    state.dateError = `A data não pode ser anterior a ${formatDateToDDMMYYYY(minSelectableDate)}.`;
    emit('update:selectedDate', null);
    return;
  }

  state.selectedDateObject = date;
  state.currentDate = date;
  state.dateError = '';
  emit('update:selectedDate', formatDateToYYYYMMDD(date));
};

const toggleCalendar = () => {
  state.showCalendar = !state.showCalendar;
  if (state.showCalendar) {
    goToSelectedDate();
  }
};

const handleClickOutside = (event) => {
  if (calendarRef.value && dateInputRef.value &&
    !calendarRef.value.contains(event.target) &&
    !dateInputRef.value.contains(event.target)) {
    state.showCalendar = false;
  }
};

onMounted(() => {
  document.addEventListener('click', handleClickOutside);
});

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside);
});

const calendarPosition = ref({});

const calculateCalendarPosition = () => {
  if (!dateInputRef.value || !calendarRef.value) return;

  const inputRect = dateInputRef.value.getBoundingClientRect();
  const calendarRect = calendarRef.value.getBoundingClientRect();

  const viewportHeight = window.innerHeight || document.documentElement.clientHeight;
  const viewportWidth = window.innerWidth || document.documentElement.clientWidth;

  let top = inputRect.bottom + 5;
  let left = inputRect.right - calendarRect.width;

  if (top + calendarRect.height > viewportHeight) {
    top = inputRect.top - calendarRect.height - 5; // 5px de margem
    if (top < 0) {
      top = inputRect.top; // Alinha com o topo do input
      left = inputRect.right + 5; // 5px de margem
      if (left + calendarRect.width > viewportWidth) {
        left = inputRect.left - calendarRect.width - 5; // 5px de margem
        if (left < 0) {
          top = inputRect.bottom + 5;
          left = Math.max(0, viewportWidth - calendarRect.width);
        }
      }
    }
  }

  calendarPosition.value = {
    top: `${top}px`,
    left: `${left}px`,
    position: 'fixed',
  };
};

window.addEventListener('resize', () => {
  if (state.showCalendar) {
    calculateCalendarPosition();
  }
});

</script>
<style scoped>
.calendar-container {
  position: relative;
}

.date-input {
  display: flex;
  justify-content: space-between;
  align-items: center;
  cursor: pointer;
}

.calendar-icon {
  margin-left: 10px;
  font-size: 1.2em;
}

.calendar {
  font-family: Arial, sans-serif;
  border: 1px solid #4a5568;
  border-radius: 8px;
  padding: 10px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  position: fixed;
  z-index: 1000;
  background-color: #2d3748;
  color: #f7fafc;
  font-size: 0.9em;
  max-width: 300px;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.header button {
  background-color: #007bff;
  color: white;
  border: none;
  padding: 8px 12px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 16px;
}

.header button:hover {
  background-color: #0056b3;
}

.header h2 {
  margin: 0;
  font-size: 1.1em;
}

.weekdays,
.days {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 1px;
}

.weekdays div,
.day {
  text-align: center;
  padding: 5px 2px;
  border-radius: 4px;
  font-size: 0.8em;
}

.weekdays div {
  font-weight: bold;
  color: #a0aec0;
}

.day {
  cursor: pointer;
  transition: background-color 0.2s ease-in-out;
}

.day:hover {
  background-color: #4a5568;
}

.empty-day {
  visibility: hidden;
}

.current-day {
  background-color: #4299e1;
  color: white;
}

.selected-day {
  background-color: #2b6cb0;
  color: white;
  font-weight: bold;
}

.weekdays div {
  color: #a0aec0;
}

.day {
  color: #f7fafc;
}

.empty-day {
  background-color: #2d3748;
}
</style>
