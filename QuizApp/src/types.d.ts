export interface IQuiz {
    title: string
    questions: IQuestion[]
}

export interface IQuestion {
    text: string
    optionsContainer: ISimpleOptionsContainer | IOptionTemplate | IDragDropOptionsContainer
    questionType: QuestionType
}

export interface IOption {
    text: string
    isCorrect: boolean
    isSelected?: boolean
    order?: number
}

export interface IOptionTemplate {
    parts: IOptionTemplatePart[]
    groups: IOptionGroup[]
}

export interface IOptionTemplatePart {
    value: string | number
    type: OptionTemplatePartType
}

export interface IOptionGroup {
    itemsContainer: ISimpleOptionsContainer
}

export interface IOptionsContainer {
    getCorrectAnswerCount(): number
    getSelectedCorrectAnswerCount(): number
}

export interface ISimpleOptionsContainer {
    options: IOption[]
}

export interface IDragDropOptionsContainer {
    isOrdered: boolean
    options: IOption[]
}

export type QuestionType =  'SimpleChoice' | 'TemplatedChoice' | 'DragDropChoice'
export type OptionTemplatePartType = 'Text' | 'OptionsGroup'
  