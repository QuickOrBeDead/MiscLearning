<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { QuizDb } from '../db/QuizDb'
import { Modal } from 'bootstrap'

const quizDb = new QuizDb()
const quizzes = ref<{ id: number, title: string }[]>()
const quizJson = ref<string>()
let addQuizModal: Modal

onMounted(() => {
  addQuizModal = new Modal('#addQuizModal')
  quizDb.init(() => {
    loadQuizzes()
  })
})

function loadQuizzes() {
    quizDb.getQuizzes(q => {
        quizzes.value = q
    })
}

function openAddQuizDialog() {
    addQuizModal?.show()
}

function addQuiz() {
    if (quizJson.value) {
        quizDb.addQuiz(JSON.parse(quizJson.value), () => {
            addQuizModal?.hide()
            loadQuizzes()
        })
    }
}

function deleteQuiz(id: number) {
   const isConfirmed = confirm("Are you sure?")
   if (isConfirmed) {
        quizDb.deleteQuiz(id, () => {
            loadQuizzes()
        })
   }
}

function fileChange(e: any) {
    const files = e.target?.files
    const f = files && files.length ? files[0] : undefined
    
    if (f) {
        const reader = new FileReader();

        reader.onload = () => quizJson.value = reader.result as string
        reader.readAsText(f);
    }
}
</script>
<template>
    <div class="container mt-5">
        <div class="row">
            <div class="col">
                <button type="button" class="btn btn-primary" data-bs-toggle="modal" @click="openAddQuizDialog">Add Quiz</button>
            </div>
        </div>
        <div class="row">
            <div class="col">
                <h1>Quizzes</h1>
            </div>
        </div>
        <div class="row">
            <div class="col">
                <ul v-for="q in quizzes">
                    <li><router-link :to="{ name: 'Quiz', params: { id: q.id } }">{{ q.title }}</router-link> <button type="button" class="btn btn-sm btn-primary" @click="deleteQuiz(q.id)">Delete</button></li>
                </ul>
            </div>
        </div>
    </div>
    <div class="modal fade" id="addQuizModal" tabindex="-1" aria-labelledby="addQuizModalTitle" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="addQuizModalTitle">Add Quiz</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <form>
                    <div class="mb-3">
                        <textarea class="form-control" cols="30" rows="10" placeholder="Enter quiz json" v-model="quizJson"></textarea>
                    </div>
                    <div class="mb-3"><input class="form-control" type="file" @change="fileChange"></div>
                </form>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                <button type="button" class="btn btn-primary" @click="addQuiz">Add</button>
            </div>
            </div>
        </div>
    </div>
</template>