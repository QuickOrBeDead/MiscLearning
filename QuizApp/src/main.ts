import { createApp } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'

import './style.css'
import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap/dist/js/bootstrap.min.js'

import App from './App.vue'
import Quiz from './components/Quiz.vue'
import QuizList from './components/QuizList.vue'

const routes = [
    { path: '/', component: QuizList },
    { path: '/quiz/:id', component: Quiz, name: 'Quiz', props: true },
]
    
createApp(App)
    .use(createRouter({  history : createWebHistory(), routes: routes, linkActiveClass: 'active' }))
    .mount('#app')
