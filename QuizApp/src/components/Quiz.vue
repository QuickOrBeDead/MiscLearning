<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import { Question, Quiz, Answer } from '../types'
import { QuizDb } from '../db/QuizDb'

const route = useRoute()
const quiz = ref<Quiz>();
const question = ref<Question>()
const questionIndex = ref<number>(0)
const showAnswer = ref<boolean>()

const quizDb = new QuizDb()

onMounted(() => {
  quizDb.init(() => {
    const id = parseInt(route.params.id as string, 10)
    quizDb.getQuiz(id, q => {
      quiz.value = q
  
      loadQuestion()
    })
  })
})

function loadQuestion() {
  const index = questionIndex.value
  if (quiz.value && index >= 0 && index < quiz.value.questions.length) {
    question.value = quiz.value.questions[index]
    showAnswer.value = areAllChoicesMade(question.value.answers)
  }
}

function onAnswerSelected(i: number, event: any) {
  const answers = question.value?.answers as Answer[]
  const answer = answers[i]

  if (question.value?.questionType === 'SingleChoice') {
    answers.forEach(x => x.isSelected = false)
    answer.isSelected = true
    showAnswer.value = true
  } else {
    if (answer.isSelected) {
      answer.isSelected = false
    } else {
      const maxChoiceCount = getMaxChoiceCount(answers)
      const currentChoiceCount = getCurrentChoiceCount(answers)

      if (currentChoiceCount < maxChoiceCount) {
        answer.isSelected = true

        if (currentChoiceCount + 1 === maxChoiceCount) {
          showAnswer.value = true
        }
      } else {
        event.target.checked = false
      }
    }
  }
}

function getMaxChoiceCount(answers: Answer[]) {
  return answers.reduce((n, a) => a.isCorrect ? n + 1 : n, 0)
}

function getCurrentChoiceCount(answers: Answer[]) {
  return answers.reduce((n, a) => a.isSelected ? n + 1 : n, 0)
}

function areAllChoicesMade(answers: Answer[]) {
  return getMaxChoiceCount(answers) === getCurrentChoiceCount(answers)
}

function next() {
  if (quiz.value && questionIndex.value < quiz.value.questions.length - 1) {
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
   <div class="container mt-5" v-if="quiz">
      <div class="row">
        <div class="col"><h1>{{ quiz.title }}</h1></div>
      </div>

      <div class="row">
        <div class="col">
          <div class="btn-container">
            <button type="button" class="btn btn-primary" :disabled="questionIndex === 0" @click="prev">Previous</button>
            <button type="button" class="btn btn-primary" :disabled="questionIndex === quiz.questions.length - 1" @click="next">Next</button>
          </div>
        </div>
      </div>

      <div class="row">
        <div class="col">
          <h3>Question {{ questionIndex + 1 }}:</h3>
          <p v-html="question?.text"></p>
        </div>
      </div>

      <div class="row">
        <div class="col">
          <div class="mb-4">
            <div class="form-check" v-for="(answer, index) in question?.answers" :key="index">
              <input class="answer-option-input form-check-input" :disabled="showAnswer" :type="question?.questionType === 'MultipleChoice' ? 'checkbox' : 'radio'" name="answer" :id="'a' + index" :value="index" :checked="answer.isSelected" @change="event => onAnswerSelected(index, event)">
              <label class="answer-option-label form-check-label" :class="showAnswer && answer.isCorrect ? 'bg-success text-white' : (showAnswer && !answer.isCorrect && answer.isSelected ? 'bg-danger text-white' : '')" :for="'a' + index">
                {{ answer.text }} <i class="bi float-end" v-if="showAnswer" :class="answer.isCorrect ? 'bi-check-circle-fill' : 'bi-x-circle-fill'"></i>
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
.form-check input[type="checkbox"]:checked + label {
  background-color: #007bff;
  color: white;
}

.btn-container {
  display: flex;
  justify-content: space-between;
  margin-top: 15px;
  margin-bottom: 15px;
}

.answer-option-input[disabled]~.form-check-label {
  opacity: 0.9;
}
</style>
