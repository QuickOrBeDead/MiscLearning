import { createApp } from 'vue'
import { createPinia } from 'pinia'
import './style.css'
import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap/dist/js/bootstrap.min.js'
import App from './App.vue'
import miniToastr from 'mini-toastr'

miniToastr.init()

createApp(App).use(createPinia()).mount('#app')
