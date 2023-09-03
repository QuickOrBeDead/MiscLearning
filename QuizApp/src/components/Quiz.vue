<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import { Question, Quiz, SimpleOptionsContainer, TemplatedOptionsContainer, DragDropOptionsContainer } from '../classes'
import { QuizDb } from '../db/QuizDb'
import draggable from 'vuedraggable'
import { Modal } from 'bootstrap'

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
let editQuestionModal: Modal

onMounted(() => {
  editQuestionModal = new Modal('#editQuestionModal')

  quizDb.init(() => {
    const id = parseInt(route.params.id as string, 10)
    quizDb.getQuiz(id, q => {
      quiz.value = Quiz.map(q)
      totalQuestions.value = quiz.value.questions.length
      totalPoints.value = quiz.value.getTotalPoints()

      calculateCurrentPoints()
      loadQuestion()
    })
  })
})

function loadQuestion() {
  const index = questionIndex.value
  if (quiz.value && index >= 0 && index < quiz.value.questions.length) {
    question.value = quiz.value.questions[index]
    showAnswer.value = question.value.areAllChoicesMade()
  }
}

function onOptionGroupSelected(optionGroup: number, event: any) {
  if (!question.value) {
    return;
  }

  const i = event.target && event.target.value !== '' ? parseInt(event.target.value, 10) : undefined

  onChoiceCompleted((question.value.optionsContainer as TemplatedOptionsContainer).selectChoice(optionGroup, i), event)
}

function onOptionSelected(i: number, event: any) {
  if (!question.value) {
    return;
  }

  onChoiceCompleted((question.value.optionsContainer as SimpleOptionsContainer).selectChoice(i), event)
}

function onCompleteDragDropChoice() {
  if (!question.value) {
    return;
  }

  (question.value.optionsContainer as DragDropOptionsContainer).completeChoice()

  showAnswer.value = true
  calculateCurrentPoints()
}

function onChoiceCompleted(selectChoiceResult: { isSelected: boolean; allSelected: boolean; }, event: any) {
  if (!selectChoiceResult.isSelected) {
    event.target.checked = false
  }

  if (selectChoiceResult.allSelected) {
    showAnswer.value = true
    calculateCurrentPoints()
  }
}

function calculateCurrentPoints() {
  if (quiz.value) {
    currentPoints.value = quiz.value.getCurrentPoints()
    correctQuestions.value = quiz.value.getCorrectQuestionCount()
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

function editQuestion() {
  editQuestionModal.show()
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
          <h3>Question {{ questionIndex + 1 }}: <button type="button" class="btn btn-primary" @click="editQuestion">Edit</button></h3>
          <p v-html="question?.text"></p>
        </div>
      </div>

      <div class="row">
        <div class="col">
          <div class="mb-4">
            <template v-if="question?.questionType === 'DragDropChoice'">
              <div class="row">
                <div class="col-6">
                  <h3>Actions</h3>
                  <draggable
                    class="list-group"
                    :list="(question?.optionsContainer as DragDropOptionsContainer).candidateOptions"
                    :disabled="showAnswer"
                    group="choice"
                    itemKey="text">
                    <template #item="{ element }">
                      <div class="list-group-item" :class="showAnswer ? (element.isCorrect ? 'bg-success text-white' : '') : ''">{{ element.text }} <span v-if="showAnswer && element.order !== undefined" class="badge bg-secondary">{{ element.order + 1 }}</span></div>
                    </template>
                  </draggable>
                </div>
                <div class="col-6">
                  <h3>Answer <button type="button" class="btn btn-success" :disabled="showAnswer" @click="onCompleteDragDropChoice">Complete</button></h3>
                  <draggable
                    class="list-group"
                    :list="(question?.optionsContainer as DragDropOptionsContainer).selectedOptions"
                    :disabled="showAnswer"
                    group="choice"
                    itemKey="text">
                    <template #item="{ element }">
                      <div class="list-group-item" :class="showAnswer ? (element.isSelectedCorrect() ? 'bg-success text-white' : 'bg-danger text-white') : ''">{{ element.text }} <span v-if="showAnswer && element.order !== undefined" class="badge bg-secondary">{{ element.order + 1 }}</span></div>
                    </template>
                  </draggable>
                </div>
              </div>
            </template>
            <template v-if="question?.questionType === 'TemplatedChoice'">
              <template v-for="part in (question?.optionsContainer as TemplatedOptionsContainer).parts">
                <template v-if="part.type === 'Text'">
                  <b v-html="part.value"></b>
                </template>
                <template v-if="part.type === 'OptionsGroup'">
                  <select :disabled="showAnswer" :class="showAnswer ? ((question?.optionsContainer as TemplatedOptionsContainer).groups[part.value as number].itemsContainer.isCorrect() ? 'answer-select-correct' : 'answer-select-incorrect') : ''" @change="event => onOptionGroupSelected(part.value as number, event)">
                    <option value="">Choose..</option>
                    <template v-for="(option, index) in (question?.optionsContainer as TemplatedOptionsContainer).groups[part.value as number].itemsContainer.options">
                      <option :value="index">{{ option.text }}</option>
                    </template>
                  </select>
                </template>
              </template>
            </template>
            <template v-if="question?.questionType === 'SimpleChoice'">
              <div class="form-check" v-for="(option, index) in ((question?.optionsContainer as SimpleOptionsContainer).options)" :key="index">
                <input class="answer-option-input form-check-input" :disabled="showAnswer" :type="(question?.optionsContainer as SimpleOptionsContainer).isMultipleChoice() ? 'checkbox' : 'radio'" name="answer" :id="'a' + index" :value="index" :checked="option.isSelected" @change="event => onOptionSelected(index, event)">
                <label class="answer-option-label form-check-label" :class="showAnswer && option.isCorrect ? 'bg-success text-white' : (showAnswer && !option.isCorrect && option.isSelected ? 'bg-danger text-white' : '')" :for="'a' + index">
                  {{ option.text }} <i class="bi float-end" v-if="showAnswer" :class="option.isCorrect ? 'bi-check-circle-fill' : 'bi-x-circle-fill'"></i>
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

  <div class="modal fade modal-xl" id="editQuestionModal" tabindex="-1" aria-labelledby="editQuestionTitle" aria-hidden="true">
      <div class="modal-dialog">
          <div class="modal-content">
          <div class="modal-header">
              <h5 class="modal-title" id="editQuestionTitle">Edit Question</h5>
              <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
          </div>
          <div class="modal-body">
            <form>
              <template v-if="question?.questionType === 'DragDropChoice'">
                <template v-for="(option, index) in (question?.optionsContainer as DragDropOptionsContainer).options">
                  <div class="row g-3 mb-2">
                    <div class="col-sm-9">
                      <input type="text" v-model="option.text" class="form-control" :placeholder="`Option ${index}`">
                    </div>
                    <div class="col-sm-1">
                      <div class="form-check">
                        <input type="checkbox" v-model="option.isCorrect" class="form-check-input" :id="`is-correct-${index}`">
                        <label class="form-check-label" :for="`is-correct-${index}`">Correct</label>
                      </div>
                    </div>
                    <div class="col-sm">
                      <input type="number" v-model="option.order" class="form-control" placeholder="Order" min="0">
                    </div>
                  </div>
                </template>
              </template>
              <template v-if="question?.questionType === 'TemplatedChoice'">
              </template>
              <template v-if="question?.questionType === 'SimpleChoice'">
              </template> 
            </form>
          </div>
          <div class="modal-footer">
              <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
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
