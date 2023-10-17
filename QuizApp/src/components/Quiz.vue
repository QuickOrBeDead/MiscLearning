<script setup lang="ts">
import { onBeforeMount, onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import { Question, Quiz, SimpleOptionsContainer, TemplatedOptionsContainer, DragDropOptionsContainer, Option, OptionTemplatePart, OptionGroup } from '../classes'
import { QuizDb } from '../db/QuizDb'
import draggable from 'vuedraggable'
import { Modal } from 'bootstrap'
import { QuestionType } from '../types'

const route = useRoute()
const quiz = ref<Quiz>();
const question = ref<Question>()
const questionIndex = ref<number>(0)
const showAnswer = ref<boolean>()
const totalPoints = ref<number>(0)
const currentPoints = ref<number>(0)
const totalQuestions = ref<number>(0)
const correctQuestions = ref<number>(0)
const inCorrectQuestions = ref<number>(0)
const time = ref<string>("0.00:00:00")
const quizJson = ref<string>()
const startDate = new Date()
let timer = 0

const quizDb = new QuizDb()
let editQuestionModal: Modal
let exportQuizModal: Modal

onMounted(() => {
  quizDb.init(() => {
    const id = parseInt(route.params.id as string, 10)
    quizDb.getQuiz(id, q => {
      quiz.value = Quiz.map(q)
      totalQuestions.value = quiz.value.questions.length
      totalPoints.value = quiz.value.getTotalPoints()

      calculateCurrentPoints()
      loadQuestion()

      timer = window.setInterval(() => {
        let delta = Math.floor((new Date().getTime() - startDate.getTime()) / 1000)

        const days = Math.floor(delta / 86400)
        delta -= days * 86400

        const hours = Math.floor(delta / 3600) % 24
        delta -= hours * 3600

        const minutes = Math.floor(delta / 60) % 60
        delta -= minutes * 60

        const seconds = delta % 60

        time.value = `${days}.${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`
      }, 1000)

      editQuestionModal = new Modal('#editQuestionModal')
      exportQuizModal = new Modal('#exportQuizModal')
    })
  })
})

onBeforeMount(() => {
  window.clearInterval(timer)
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
    inCorrectQuestions.value = quiz.value.getInCorrectQuestionCount()
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

function removeDragDropOption(c: DragDropOptionsContainer, i: number) {
  c.options.splice(i, 1)
  c.candidateOptions = [...c.options]
  c.selectedOptions = []
}

function addNewDragDropOption(c: DragDropOptionsContainer) {
  const option = new Option('', false)
  c.options.push(option)
  c.candidateOptions = [...c.options]
  c.selectedOptions = []
}

function removeSimpleChoiceOption(o: Option[], i: number) {
  o.splice(i, 1)
}

function addNewSimpleChoiceOption(o: Option[]) {
  const option = new Option('', false)
  o.push(option)
}

function onOptionTemplatePartTypeChange(event: any, part: OptionTemplatePart) {
  if (event.target?.value === 'OptionsGroup') {
    part.value = 0
  } else if (event.target?.value === 'Text') {
    part.value = ''
  }
}

function addNewTemplatePart(p: OptionTemplatePart[]) {
  p.push(new OptionTemplatePart('', 'Text'))
}

function removeTemplatePart(p: OptionTemplatePart[], i: number) {
  p.splice(i, 1)
}

function addNewOptionGroup(g: OptionGroup[]) {
  g.push(new OptionGroup(new SimpleOptionsContainer([])))
}

function removeOptionGroup(g: OptionGroup[], i: number) {
  g.splice(i, 1)
}

function onQuestionTypeChange(event: any, q: Question | undefined) {
  if (!q) {
    return
  }

  const questionType = event.target?.value as QuestionType | undefined
  if (!questionType) {
    return
  } 

  q.questionType = questionType

  switch (questionType) {
    case 'SimpleChoice':
      q.optionsContainer = new SimpleOptionsContainer([])
      break;

    case 'TemplatedChoice':
      q.optionsContainer = new TemplatedOptionsContainer([], [])
      break;

    case 'DragDropChoice':
      q.optionsContainer = new DragDropOptionsContainer([], false)
      break;
  
    default:
      break;
  }
}

function isCorrect(container: TemplatedOptionsContainer, i : number) {
  if (i === undefined) {
    i = 0
  }

  if (i >= container.groups.length) {
    return false
  }

  const group = container.groups[i]
  if (group === undefined) {
    return false
  }

  return group.itemsContainer.isCorrect()
}

function getOptions(container: TemplatedOptionsContainer, i: number) {
  if (i === undefined) {
    i = 0
  }

  if (i >= container.groups.length) {
    return []
  }

  const group = container.groups[i]
  if (group === undefined) {
    return []
  }
  
  return group.itemsContainer.options
}

function exportQuiz() {
  if (!quiz.value) {
    return
  }

  quizJson.value = JSON.stringify(Quiz.export(quiz.value))

  exportQuizModal.show()
}
</script>

<template>
  <div v-if="quiz">
    <div class="container mt-5">
      <div class="row">
        <div class="col">
          <h1>{{ quiz.title }} <button type="button" class="btn btn-primary" @click="exportQuiz" data-bs-toggle="modal" data-bs-target="#exportQuizModal">Export</button></h1>
          <p>Points: {{ currentPoints }} / {{ totalPoints }}</p>
          <p>Correct: {{ correctQuestions }} / {{ totalQuestions }} Incorrect: {{ inCorrectQuestions }} / {{ correctQuestions + inCorrectQuestions }}</p>
          <p>Time: {{ time }}</p>
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
          <h3>Question {{ questionIndex + 1 }}: <button type="button" class="btn btn-primary" @click="editQuestion" data-bs-toggle="modal" data-bs-target="#editQuestionModal">Edit</button></h3>
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
                  <select :disabled="showAnswer" :class="showAnswer ? (isCorrect(question?.optionsContainer as TemplatedOptionsContainer, part.value as number) ? 'answer-select-correct' : 'answer-select-incorrect') : ''" @change="event => onOptionGroupSelected(part.value as number, event)">
                    <option value="">Choose..</option>
                    <template v-for="(option, index) in getOptions(question?.optionsContainer as TemplatedOptionsContainer, part.value as number)">
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

      <div v-if="showAnswer && question?.questionType === 'TemplatedChoice'" class="row">
        <div class="col">
          <h3>Correct Answers</h3>
          <ol>
            <li v-for="a in (question?.optionsContainer as TemplatedOptionsContainer).getCorrectAnswers()">{{ a }}</li>
          </ol>
        </div>
      </div>

      <div v-if="showAnswer && question?.explanation !== undefined" class="row">
        <div class="col">
          <h3>Explanation</h3>
          <div v-html="question?.explanation"></div>
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
              <form v-if="question">
                <div class="mb-3 row">
                  <label for="question-text" class="form-label">Question</label>
                  <textarea id="question-text" class="form-control" cols="30" rows="3" placeholder="Question Text" v-model="question.text"></textarea>
                </div>
                <div class="mb-3 row">
                  <label for="question-explanation" class="form-label">Question Explanation</label>
                  <textarea id="question-explanation" class="form-control" cols="30" rows="3" placeholder="Question Explanation" v-model="question.explanation"></textarea>
                </div>
                <div class="mb-3 row">
                  <label for="question-type-dropdown" class="col-sm-2 col-form-label">Question Type</label>
                  <div class="col-sm-10">
                    <select class="form-select" id="question-type-dropdown" v-if="question" v-model="question.questionType" @change="event => onQuestionTypeChange(event, question)">
                      <option disabled value="">Choose..</option>
                      <option value="SimpleChoice">Simple</option>
                      <option value="DragDropChoice">Drag Drop</option>
                      <option value="TemplatedChoice">Templated</option>
                    </select>
                  </div>
                </div>
                <template v-if="question?.questionType === 'DragDropChoice'">
                  <div class="row g-3 mb-2">
                    <div class="col-sm">
                      <div class="form-check">
                        <input type="checkbox" v-model="(question.optionsContainer as DragDropOptionsContainer).isOrdered" class="form-check-input" id="drag-drop-is-ordered-cb">
                        <label class="form-check-label" for="drag-drop-is-ordered-cb">Ordered</label>
                      </div>
                    </div>
                  </div>
                  <template v-for="(option, index) in (question.optionsContainer as DragDropOptionsContainer).options">
                    <div class="row g-3 mb-2">
                      <div class="col-sm-9">
                        <input type="text" v-model="option.text" class="form-control" :placeholder="`Option ${index}`">
                      </div>
                      <div class="col-sm">
                        <div class="form-check">
                          <input type="checkbox" v-model="option.isCorrect" class="form-check-input" :id="`is-correct-${index}`">
                          <label class="form-check-label" :for="`is-correct-${index}`">Correct</label>
                        </div>
                      </div>
                      <div class="col-sm" :hidden="!(question.optionsContainer as DragDropOptionsContainer).isOrdered">
                        <input type="number" v-model="option.order" class="form-control" placeholder="Order" min="0">
                      </div>
                      <div class="col-sm">
                        <button type="button" class="btn btn-secondary float-end" @click="() => removeDragDropOption(question?.optionsContainer as DragDropOptionsContainer, index)">
                          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-trash" viewBox="0 0 16 16">
                            <path d="M5.5 5.5A.5.5 0 0 1 6 6v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5Zm2.5 0a.5.5 0 0 1 .5.5v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5Zm3 .5a.5.5 0 0 0-1 0v6a.5.5 0 0 0 1 0V6Z"></path>
                            <path d="M14.5 3a1 1 0 0 1-1 1H13v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V4h-.5a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1H6a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1h3.5a1 1 0 0 1 1 1v1ZM4.118 4 4 4.059V13a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V4.059L11.882 4H4.118ZM2.5 3h11V2h-11v1Z"></path>
                          </svg>
                        </button>
                      </div>
                    </div>
                  </template>
                  <div class="row">
                    <div class="col">
                      <button type="button" class="btn btn-primary float-end" @click="() => addNewDragDropOption(question?.optionsContainer as DragDropOptionsContainer)">Add</button>
                    </div>
                  </div>
                </template>
                <template v-if="question?.questionType === 'TemplatedChoice'">
                  <h5>Parts</h5>
                  <template v-for="(part, index) in (question?.optionsContainer as TemplatedOptionsContainer).parts">
                    <h6>Part {{ index }}</h6>
                    <div class="row g-3 mb-2">
                      <div class="col-sm-9">
                        <input :type="part.type === 'Text' ? 'text' : 'number'" v-model="part.value" class="form-control" :placeholder="`Part ${index}`">
                      </div>
                      <div class="col-sm-2">
                        <div class="form-check form-check-inline">
                          <input class="form-check-input" type="radio" v-model="part.type" value="Text" :name="`tmpl-option-part-type-rb-${index}`" :id="`tmpl-option-part-type-rb-${index}-1`" @change="event => onOptionTemplatePartTypeChange(event, part)">
                          <label class="form-check-label" :for="`tmpl-option-part-type-rb-${index}-1`">Text</label>
                        </div>
                        <div class="form-check form-check-inline">
                          <input class="form-check-input" type="radio" v-model="part.type" value="OptionsGroup" :name="`tmpl-option-part-type-rb-${index}`" :id="`tmpl-option-part-type-rb-${index}-2`" @change="event => onOptionTemplatePartTypeChange(event, part)">
                          <label class="form-check-label" :for="`tmpl-option-part-type-rb-${index}-2`">Group</label>
                        </div>
                      </div>
                      <div class="col-sm">
                        <button type="button" class="btn btn-secondary float-end" @click="() => removeTemplatePart((question?.optionsContainer as TemplatedOptionsContainer).parts, index)">
                          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-trash" viewBox="0 0 16 16">
                            <path d="M5.5 5.5A.5.5 0 0 1 6 6v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5Zm2.5 0a.5.5 0 0 1 .5.5v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5Zm3 .5a.5.5 0 0 0-1 0v6a.5.5 0 0 0 1 0V6Z"></path>
                            <path d="M14.5 3a1 1 0 0 1-1 1H13v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V4h-.5a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1H6a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1h3.5a1 1 0 0 1 1 1v1ZM4.118 4 4 4.059V13a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V4.059L11.882 4H4.118ZM2.5 3h11V2h-11v1Z"></path>
                          </svg>
                        </button>
                      </div>
                    </div>
                  </template>
                  <div class="row">
                    <div class="col">
                      <button type="button" class="btn btn-primary float-end" @click="() => addNewTemplatePart((question?.optionsContainer as TemplatedOptionsContainer).parts)">Add Part</button>
                    </div>
                  </div>

                  <h5>Groups</h5>
                  <template v-for="(group, groupIndex) in (question?.optionsContainer as TemplatedOptionsContainer).groups">
                    <h6>
                      Group {{ groupIndex }}

                      <button type="button" class="btn btn-secondary" @click="() => removeOptionGroup((question?.optionsContainer as TemplatedOptionsContainer).groups, groupIndex)">
                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-trash" viewBox="0 0 16 16">
                          <path d="M5.5 5.5A.5.5 0 0 1 6 6v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5Zm2.5 0a.5.5 0 0 1 .5.5v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5Zm3 .5a.5.5 0 0 0-1 0v6a.5.5 0 0 0 1 0V6Z"></path>
                          <path d="M14.5 3a1 1 0 0 1-1 1H13v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V4h-.5a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1H6a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1h3.5a1 1 0 0 1 1 1v1ZM4.118 4 4 4.059V13a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V4.059L11.882 4H4.118ZM2.5 3h11V2h-11v1Z"></path>
                        </svg>
                      </button>
                    </h6>
                    <template v-for="(option, index) in group.itemsContainer.options">
                      <div class="row g-3 mb-2">
                        <div class="col-sm-10">
                          <input type="text" v-model="option.text" class="form-control" :placeholder="`Option ${index}`">
                        </div>
                        <div class="col-sm-1">
                          <div class="form-check">
                            <input type="checkbox" v-model="option.isCorrect" class="form-check-input" :id="`is-correct-${groupIndex}-${index}`">
                            <label class="form-check-label" :for="`is-correct-${groupIndex}-${index}`">Correct</label>
                          </div>
                        </div>
                        <div class="col-sm">
                          <button type="button" class="btn btn-secondary float-end" @click="() => removeSimpleChoiceOption(group.itemsContainer.options, index)">
                            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-trash" viewBox="0 0 16 16">
                              <path d="M5.5 5.5A.5.5 0 0 1 6 6v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5Zm2.5 0a.5.5 0 0 1 .5.5v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5Zm3 .5a.5.5 0 0 0-1 0v6a.5.5 0 0 0 1 0V6Z"></path>
                              <path d="M14.5 3a1 1 0 0 1-1 1H13v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V4h-.5a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1H6a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1h3.5a1 1 0 0 1 1 1v1ZM4.118 4 4 4.059V13a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V4.059L11.882 4H4.118ZM2.5 3h11V2h-11v1Z"></path>
                            </svg>
                          </button>
                        </div>
                      </div>
                    </template>
                    <div class="row">
                      <div class="col">
                        <button type="button" class="btn btn-primary float-end" @click="() => addNewSimpleChoiceOption(group.itemsContainer.options)">Add Option</button>
                      </div>
                    </div>
                  </template>
                  <div class="row mt-2">
                    <div class="col">
                      <button type="button" class="btn btn-primary float-end" @click="() => addNewOptionGroup((question?.optionsContainer as TemplatedOptionsContainer).groups)">Add Group</button>
                    </div>
                  </div>
                </template>
                <template v-if="question?.questionType === 'SimpleChoice'">
                  <template v-for="(option, index) in (question?.optionsContainer as SimpleOptionsContainer).options">
                    <div class="row g-3 mb-2">
                      <div class="col-sm-10">
                        <input type="text" v-model="option.text" class="form-control" :placeholder="`Option ${index}`">
                      </div>
                      <div class="col-sm-1">
                        <div class="form-check">
                          <input type="checkbox" v-model="option.isCorrect" class="form-check-input" :id="`is-correct-${index}`">
                          <label class="form-check-label" :for="`is-correct-${index}`">Correct</label>
                        </div>
                      </div>
                      <div class="col-sm">
                        <button type="button" class="btn btn-secondary float-end" @click="() => removeSimpleChoiceOption((question?.optionsContainer as SimpleOptionsContainer).options, index)">
                          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-trash" viewBox="0 0 16 16">
                            <path d="M5.5 5.5A.5.5 0 0 1 6 6v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5Zm2.5 0a.5.5 0 0 1 .5.5v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5Zm3 .5a.5.5 0 0 0-1 0v6a.5.5 0 0 0 1 0V6Z"></path>
                            <path d="M14.5 3a1 1 0 0 1-1 1H13v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V4h-.5a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1H6a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1h3.5a1 1 0 0 1 1 1v1ZM4.118 4 4 4.059V13a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V4.059L11.882 4H4.118ZM2.5 3h11V2h-11v1Z"></path>
                          </svg>
                        </button>
                      </div>
                    </div>
                  </template>
                  <div class="row">
                    <div class="col">
                      <button type="button" class="btn btn-primary float-end" @click="() => addNewSimpleChoiceOption((question?.optionsContainer as SimpleOptionsContainer).options)">Add</button>
                    </div>
                  </div>
                </template> 
              </form>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
            </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="exportQuizModal" tabindex="-1" aria-labelledby="exportQuizModalTitle" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="exportQuizModalTitle">Add Quiz</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <form>
                    <div class="mb-3">
                        <textarea class="form-control" cols="30" rows="10" placeholder="Enter quiz json" v-model="quizJson"></textarea>
                    </div>
                </form>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
            </div>
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
