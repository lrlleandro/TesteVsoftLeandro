import './assets/index.css'
import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { createPinia } from 'pinia'
import piniaPluginPersistedstate from 'pinia-plugin-persistedstate'
import './assets/index.css'
import Toast from 'vue-toastification';
import 'vue-toastification/dist/index.css';
import { library } from '@fortawesome/fontawesome-svg-core'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'
import { faUser, faLock, faSignInAlt, faSun, faMoon, faSave, faPlus, faEdit, faTrash, faTimes, faRandom } from '@fortawesome/free-solid-svg-icons'

library.add(faUser, faLock, faSignInAlt, faSun, faMoon, faSave, faPlus, faEdit, faTrash, faTimes, faRandom)

const app = createApp(App)
const pinia = createPinia()
pinia.use(piniaPluginPersistedstate)

app.use(pinia)
app.use(router)
app.use(Toast, {
  transition: 'Vue-Toastification__fade',
  maxToasts: 20,
  newestOnTop: true,
});

app.component('font-awesome-icon', FontAwesomeIcon)

if (window.matchMedia('(prefers-color-scheme: dark)').matches) {
  document.documentElement.classList.add('dark');
}

app.mount('#app');
