<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { Question, Quiz, Answer } from '../types'


const quiz: Quiz = {
  title: "Quiz 1",
  questions: [
    {
      text: "What is the capital of France?",
      questionType: "SingleChoice",
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
    },
    {
      text: "1 + 1 = ?",
      questionType: "SingleChoice",
      answers: [{
                  text: "1",
                  isCorrect: false
                }, 
                {
                  text: "2",
                  isCorrect: true
                }, 
                {
                  text: "3",
                  isCorrect: false
                }]
    },
    {
      text: "Which of the following 2 answers are true?",
      questionType: "MultipleChoice",
      answers: [{
                  text: "1 + 1 = 2",
                  isCorrect: true
                }, 
                {
                  text: "2 + 2 = 5",
                  isCorrect: false
                }, 
                {
                  text: "2 + 2 = 4",
                  isCorrect: true
                }]
    }
  ]
}

const title = ref<string>()
const question = ref<Question>()
const questionIndex = ref<number>(0)

onMounted(() => {
  title.value = quiz.title
  
  loadQuestion()
})

function loadQuestion() {
  const index = questionIndex.value
  if (index >= 0 && index < quiz.questions.length) {
    question.value = quiz.questions[index]
  }
}

function onAnswerSelected(i: number, event: any) {
  const answers = question.value?.answers as Answer[]
  const answer = answers[i]

  if (question.value?.questionType === 'SingleChoice') {
    answers.forEach(x => x.isSelected = false)
    answer.isSelected = true
  } else {
    if (answer.isSelected) {
      answer.isSelected = false
    } else {
      const maxChoiceCount = answers.reduce((n, a) => a.isCorrect ? n + 1 : n, 0)
      const currentChoiceCount = answers.reduce((n, a) => a.isSelected ? n + 1 : n, 0)

      if (currentChoiceCount < maxChoiceCount) {
        answer.isSelected = true
      } else {
        event.target.checked= false
      }
    }
  }
}

function next() {
  if (questionIndex.value < quiz.questions.length - 1) {
    questionIndex.value++
  }

  loadQuestion()
}

function prev() {
  if (questionIndex.value > 0) {
    questionIndex.value--
  }

  loadQuestion()
}

</script>

<template>
   <div class="container mt-5">
      <div class="row">
        <div class="col"><h1>{{ title }}</h1></div>
      </div>

      <div class="row">
        <div class="col"><h3>Question {{ questionIndex + 1 }}: {{ question?.text }}</h3></div>
      </div>

      <div class="row">
        <div class="col">
          <div class="mb-4">
            <div class="form-check" v-for="(answer, index) in question?.answers" :key="index">
              <input class="answer-option-input form-check-input" :type="question?.questionType === 'MultipleChoice' ? 'checkbox' : 'radio'" name="answer" :id="'a' + index" :value="index" :checked="answer.isSelected" @change="event => onAnswerSelected(index, event)">
              <label class="answer-option-label form-check-label" :for="'a' + index">
                {{ answer.text }}
              </label>
            </div>
          </div>
        </div>
      </div>
      
      <div class="row">
        <div class="col">
          <div class="btn-container">
            <button type="button" class="btn btn-primary" :disabled="questionIndex === 0" @click="prev">Previous</button>
            <button type="button" class="btn btn-primary" :disabled="questionIndex === quiz.questions.length - 1" @click="next">Next</button>
          </div>
        </div>
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


.form-check input[type="radio"]:checked + label,
.form-check input[type="checkbox"]:checked + label,
.answer-option input[type="radio"]:checked ~ .answer-option-label,
.answer-option input[type="checkbox"]:checked ~ .answer-option-label {
  background-color: #007bff;
  color: white;
}

.btn-container {
  display: flex;
  justify-content: space-between;
  margin-top: 20px;
}
</style>
