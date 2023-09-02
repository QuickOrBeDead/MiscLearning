<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import { Question, Quiz, Answer, AnswerTemplate, QuestionType } from '../types'
import { QuizDb } from '../db/QuizDb'

const route = useRoute()
const quiz = ref<Quiz>();
const question = ref<Question>()
const questionIndex = ref<number>(0)
const showAnswer = ref<boolean>()
const totalPoints = ref<number>(0)
const currentPoints = ref<number>(0)
const totalQuestions = ref<number>(0)
const correctQuestions = ref<number>(0)

const quizDb = new QuizDb()

onMounted(() => {
  quizDb.init(() => {
    const id = parseInt(route.params.id as string, 10)
    quizDb.getQuiz(id, q => {
      quiz.value = q
      totalQuestions.value = q.questions.length
  
      calculateTotalPoints()
      calculateCurrentPoints()
      loadQuestion()
    })
  })
})

function loadQuestion() {
  const index = questionIndex.value
  if (quiz.value && index >= 0 && index < quiz.value.questions.length) {
    question.value = quiz.value.questions[index]
    showAnswer.value = areAllChoicesMade(question.value.questionType, question.value.answers)
  }
}

function onAnswerTemplateAnswerGroupChange(answerGroup: number, event: any) {
  if (!question.value) {
    return;
  }

  const answers = question.value.answers as AnswerTemplate;
  const groupAnswers = answers.groups[answerGroup].answers

  if (event.target && event.target.value !== '') {
    const answer = groupAnswers[event.target.value as number]
    groupAnswers.forEach(x => x.isSelected = false)
    answer.isSelected = true

    const maxChoiceCount = getMaxChoiceCount(question.value.questionType, answers)
    const currentChoiceCount = getCurrentChoiceCount(question.value.questionType, answers)

    if (currentChoiceCount === maxChoiceCount) {
      showAnswer.value = true

      calculateCurrentPoints()
    }
  } else {
    groupAnswers.forEach(x => x.isSelected = false)
  }
}

function onAnswerSelected(i: number, event: any) {
  if (!question.value) {
    return;
  }

  const answers = question.value.answers as Answer[]
  const answer = answers[i]

  if (question.value.questionType === 'SingleChoice') {
    answers.forEach(x => x.isSelected = false)
    answer.isSelected = true
    showAnswer.value = true

    calculateCurrentPoints()
  } else {
    if (answer.isSelected) {
      answer.isSelected = false
    } else {
      const maxChoiceCount = getMaxChoiceCount(question.value.questionType, answers)
      const currentChoiceCount = getCurrentChoiceCount(question.value.questionType, answers)

      if (currentChoiceCount < maxChoiceCount) {
        answer.isSelected = true

        if (currentChoiceCount + 1 === maxChoiceCount) {
          showAnswer.value = true

          calculateCurrentPoints()
        }
      } else {
        event.target.checked = false
      }
    }
  }
}

function getMaxChoiceCount(questionType: QuestionType, answers: Answer[] | AnswerTemplate) {
  if (questionType === 'AnswerTemplate')
  {
    return (answers as AnswerTemplate).groups.reduce((n, g) => n + g.answers.reduce((n1, a) => a.isCorrect ? n1 + 1 : n1, 0), 0)
  }

  return (answers as Answer[]).reduce((n, a) => a.isCorrect ? n + 1 : n, 0)
}

function getSelectedAndCorrectChoiceCount(questionType: QuestionType, answers: Answer[] | AnswerTemplate) {
  if (questionType === 'AnswerTemplate')
  {
    return (answers as AnswerTemplate).groups.reduce((n, g) => n + g.answers.reduce((n1, a) => a.isSelected && a.isCorrect ? n1 + 1 : n1, 0), 0)
  }

  return (answers as Answer[]).reduce((n, a) => a.isSelected && a.isCorrect ? n + 1 : n, 0)
}

function getCurrentChoiceCount(questionType: QuestionType, answers: Answer[] | AnswerTemplate) {
  if (questionType === 'AnswerTemplate') 
  {
    return (answers as AnswerTemplate).groups.reduce((n, g) => n + g.answers.reduce((n1, a) => a.isSelected ? n1 + 1 : n1, 0), 0)
  }

  return (answers as Answer[]).reduce((n, a) => a.isSelected ? n + 1 : n, 0)
}

function areAllChoicesMade(questionType: QuestionType, answers: Answer[] | AnswerTemplate) {
  return getMaxChoiceCount(questionType, answers) === getCurrentChoiceCount(questionType, answers)
}

function isCorrect(questionType: QuestionType, answers: Answer[] | AnswerTemplate) {
  return getSelectedAndCorrectChoiceCount(questionType, answers) === getMaxChoiceCount(questionType, answers)
}

function calculateTotalPoints() {
  if (quiz.value) {
    totalPoints.value = quiz.value.questions.reduce((i, q) => i + getMaxChoiceCount(q.questionType, q.answers), 0)
  }
}

function calculateCurrentPoints() {
  if (quiz.value) {
    currentPoints.value = quiz.value.questions.reduce((i, q) => i + getSelectedAndCorrectChoiceCount(q.questionType, q.answers), 0)
    correctQuestions.value = quiz.value.questions.reduce((i, q) => i + (isCorrect(q.questionType, q.answers) ? 1 : 0), 0)
  }
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
        <div class="col">
          <h1>{{ quiz.title }}</h1>
          <p>Points: {{ currentPoints }} / {{ totalPoints }}</p>
          <p>Correct Answers: {{ correctQuestions }} / {{ totalQuestions }}</p>
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

      <div class="row">
        <div class="col">
          <h3>Question {{ questionIndex + 1 }}:</h3>
          <p v-html="question?.text"></p>
        </div>
      </div>

      <div class="row">
        <div class="col">
          <div class="mb-4">
            <template v-if="question?.questionType === 'AnswerTemplate'">
              <template v-for="part in (question?.answers as AnswerTemplate).parts">
                <template v-if="part.type === 'Text'">
                  <b>{{ part.value }}</b>
                </template>
                <template v-if="part.type === 'AnswerGroup'">
                  <select :disabled="showAnswer" :class="showAnswer ? (isCorrect('SingleChoice', (question?.answers as AnswerTemplate).groups[part.value as number].answers) ? 'answer-select-correct' : 'answer-select-incorrect') : ''" @change="event => onAnswerTemplateAnswerGroupChange(part.value as number, event)">
                    <option value="">Choose..</option>
                    <template v-for="(answer, index) in (question?.answers as AnswerTemplate).groups[part.value as number].answers">
                      <option :value="index">{{ answer.text }}</option>
                    </template>
                  </select>
                </template>
              </template>
            </template>
            <template v-if="question?.questionType === 'SingleChoice' || question?.questionType === 'MultipleChoice'">
              <div class="form-check" v-for="(answer, index) in (question?.answers as Answer[])" :key="index">
                <input class="answer-option-input form-check-input" :disabled="showAnswer" :type="question?.questionType === 'MultipleChoice' ? 'checkbox' : 'radio'" name="answer" :id="'a' + index" :value="index" :checked="answer.isSelected" @change="event => onAnswerSelected(index, event)">
                <label class="answer-option-label form-check-label" :class="showAnswer && answer.isCorrect ? 'bg-success text-white' : (showAnswer && !answer.isCorrect && answer.isSelected ? 'bg-danger text-white' : '')" :for="'a' + index">
                  {{ answer.text }} <i class="bi float-end" v-if="showAnswer" :class="answer.isCorrect ? 'bi-check-circle-fill' : 'bi-x-circle-fill'"></i>
                </label>
              </div>
            </template> 
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

.answer-select-correct {
  background-color: #2f9264;
  color: white;
}

.answer-select-incorrect {
  background-color: #df4857;
  color: white;
}
</style>
