<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { Question, Quiz, Answer } from '../types'


const quiz: Quiz = {
  title: "Quiz 1",
  questions: [
    {
      text: "What is the capital of France?",
      answers: [{
                  text: "Paris",
                  isCorrect: true
                }, 
                {
                  text: "Rome",
                  isCorrect: false
                }, 
                {
                  text: "Madrid",
                  isCorrect: false
                }]
    }
  ]
}

const title = ref<string>()
const question = ref<Question>()

onMounted(() => {
  title.value = quiz.title
  question.value = quiz.questions[0]
})

function onAnswerSelected(i: number) {
  const answers = question.value?.answers as Answer[]
  answers.forEach(x => x.isSelected = false)
  answers[i].isSelected = true
}

</script>

<template>
   <div class="container mt-5">
    <h1>{{ title }}</h1>
      <div class="mb-4">
        <h3>Question 1: {{ question?.text }}</h3>
        <div class="form-check" v-for="(answer, index) in question?.answers" :key="index">
          <input class="answer-option-input form-check-input" type="radio" name="answer" :id="'a' + index" :value="index" @change="onAnswerSelected(index)">
          <label class="answer-option-label form-check-label" :for="'a' + index">
            {{ answer.text }}
          </label>
        </div>
      </div>
      
      <div class="btn-container">
        <button type="button" class="btn btn-primary">Previous</button>
        <button type="button" class="btn btn-primary">Next</button>
      </div>
  </div>
</template>

<style scoped>
.answer-option-label {
  padding: 10px;
  margin: 5px 0;
  border-radius: 5px;
  display: block;
  background-color: #f0f0f0;
  cursor: pointer;
}

.answer-option-input {
  margin-top: 13px;
  cursor: pointer;
}


.form-check input[type="radio"]:checked + label {
  background-color: #007bff;
  color: white;
}

.answer-option input[type="radio"]:checked ~ .answer-option-label {
  background-color: #007bff;
  color: white;
}

.btn-container {
  display: flex;
  justify-content: space-between;
  margin-top: 20px;
}
</style>
