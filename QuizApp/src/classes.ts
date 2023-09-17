import { IOption, IOptionGroup, IOptionTemplate, IOptionTemplatePart, IOptionsContainer, IQuestion, IQuiz, ISimpleOptionsContainer, QuestionType, OptionTemplatePartType, IDragDropOptionsContainer } from "./types"

export class Quiz implements IQuiz {
    title: string
    questions: Question[]

    constructor(title: string, questions: Question[]) {
        this.title = title
        this.questions = questions
    }

    getTotalPoints() {
        return this.questions.reduce((i, q) => i + q.getCorrectAnswerCount(), 0)
    }

    getCurrentPoints() {
        return this.questions.reduce((i, q) => i + q.getSelectedCorrectAnswerCount(), 0)
    }

    getCorrectQuestionCount() {
        return this.questions.reduce((i, q) => i + (q.isCorrect() ? 1 : 0), 0)
    }

    static map(q: IQuiz): Quiz {
        const questions: Question[] = []
        q.questions.forEach(x => questions.push(Question.map(x)))

        return new Quiz(q.title, questions)
    }

    static export(q: Quiz): IQuiz {
        const questions: IQuestion[] = []
        q.questions.forEach(x => questions.push(Question.export(x)))

        return {
            title: q.title,
            questions: questions
        }
    }
}

export class Question implements IQuestion {
    text: string
    optionsContainer: SimpleOptionsContainer | TemplatedOptionsContainer | DragDropOptionsContainer
    questionType: QuestionType
    explanation?: string

    constructor(text: string, optionsContainer: SimpleOptionsContainer | TemplatedOptionsContainer | DragDropOptionsContainer, questionType: QuestionType, explanation?: string) {
        this.text = text
        this.optionsContainer = optionsContainer
        this.questionType = questionType
        this.explanation = explanation
    }

    getCorrectAnswerCount() {
        return this.optionsContainer.getCorrectAnswerCount()
    }

    getSelectedCorrectAnswerCount() {
        return this.optionsContainer.getSelectedCorrectAnswerCount()
    }

    getSelectedAnswerCount() {
        return this.optionsContainer.getSelectedAnswerCount()
    }

    areAllChoicesMade() {
        return this.getCorrectAnswerCount() === this.getSelectedAnswerCount()
    }
      
    isCorrect() {
        return this.getSelectedCorrectAnswerCount() === this.getCorrectAnswerCount()
    }

    static map(q: IQuestion): Question {
        let optionsContainer: SimpleOptionsContainer | TemplatedOptionsContainer | DragDropOptionsContainer
        if (q.questionType === 'SimpleChoice') {
            optionsContainer = SimpleOptionsContainer.map(q.optionsContainer as ISimpleOptionsContainer)
        } else if (q.questionType === 'TemplatedChoice') {
            optionsContainer = TemplatedOptionsContainer.map(q.optionsContainer as IOptionTemplate)
        } else if (q.questionType === 'DragDropChoice') {
            optionsContainer = DragDropOptionsContainer.map(q.optionsContainer as IDragDropOptionsContainer)
        } else {
            throw new Error(`unknown questionType: ${q.questionType}`)
        }
        
        return new Question(q.text, optionsContainer, q.questionType, q.explanation)
    }

    static export(q: Question): IQuestion {
        let optionsContainer: ISimpleOptionsContainer | IOptionTemplate | IDragDropOptionsContainer
        if (q.questionType === 'SimpleChoice') {
            optionsContainer = SimpleOptionsContainer.export(q.optionsContainer as SimpleOptionsContainer)
        } else if (q.questionType === 'TemplatedChoice') {
            optionsContainer = TemplatedOptionsContainer.export(q.optionsContainer as TemplatedOptionsContainer)
        } else if (q.questionType === 'DragDropChoice') {
            optionsContainer = DragDropOptionsContainer.export(q.optionsContainer as DragDropOptionsContainer)
        } else {
            throw new Error(`unknown questionType: ${q.questionType}`)
        }

        return {
            text: q.text,
            optionsContainer: optionsContainer,
            questionType: q.questionType,
            explanation: q.explanation
        }
    }
}

export class Option implements IOption {
    text: string
    isCorrect: boolean
    isSelected?: boolean
    order?: number
    selectedOrder?: number

    constructor(text: string, isCorrect: boolean, order?: number) {
        this.text = text
        this.isCorrect = isCorrect
        this.order = order
    }

    isSelectedCorrect(): boolean {
        return this.isCorrect && !!this.isSelected && (this.order === undefined || this.order === this.selectedOrder)
    }

    static map(o: IOption): Option {
        return new Option(o.text, o.isCorrect, o.order)
    }

    static export(o: Option): IOption {
        return {
            text: o.text,
            isCorrect: o.isCorrect,
            order: o.order
        }
    }
}

export class SimpleOptionsContainer implements ISimpleOptionsContainer, IOptionsContainer {
    options: Option[]

    constructor(options: Option[]) {
        this.options = options
    }

    getCorrectAnswerCount() {
        return this.getCount(a => a.isCorrect)
    }

    getSelectedAnswerCount() {
        return this.getCount(a => !!a.isSelected)
    }

    getSelectedCorrectAnswerCount() {
        return this.getCount(a => a.isSelectedCorrect())
    }

    getCount(fn: (a: Option) => boolean) {
        return this.options.reduce((n, a) => fn(a) ? n + 1 : n, 0)
    }

    isCorrect() {
        return this.getSelectedCorrectAnswerCount() === this.getCorrectAnswerCount()
    }

    isMultipleChoice() {
        return this.getCorrectAnswerCount() > 1
    }

    selectChoice(choice?: number): { isSelected: boolean, allSelected: boolean } {
        const choiceCount = this.getCorrectAnswerCount()

        if (choiceCount === 1) {
            if (choice === undefined) {
                this.options.forEach(x => x.isSelected = false)
            } else {
                const option = this.options[choice]
                this.options.forEach(x => x.isSelected = false)
                option.isSelected = true

                return { isSelected: true, allSelected: true }
            }
        } else if (choiceCount > 1) {
            if (choice === undefined) {
                return { isSelected: false, allSelected: false }
            }

            const option = this.options[choice]
            if (option.isSelected) {
                option.isSelected = false
            } else {
                const selectedCount = this.getSelectedAnswerCount()
            
                if (selectedCount < choiceCount) {
                    option.isSelected = true
            
                    if (selectedCount + 1 === choiceCount) {
                        return { isSelected: true, allSelected: true }
                    }

                    return { isSelected: true, allSelected: false }
                } else {
                    return { isSelected: false, allSelected: false }
                }
            }
        }

        return { isSelected: false, allSelected: false }
    }

    static map(c: ISimpleOptionsContainer): SimpleOptionsContainer {
        const options: Option[] = []
        c.options.forEach(x => options.push(Option.map(x)))

        return new SimpleOptionsContainer(options)
    }

    static export(c: SimpleOptionsContainer): ISimpleOptionsContainer {
        const options: IOption[] = []
        c.options.forEach(x => options.push(Option.export(x)))

        return {
            options: options
        }
    }
}

export class TemplatedOptionsContainer implements IOptionTemplate, IOptionsContainer {
    parts: OptionTemplatePart[]
    groups: OptionGroup[]

    constructor(parts: OptionTemplatePart[], groups: OptionGroup[]) {
        this.parts = parts
        this.groups = groups
    }

    getCorrectAnswerCount() {
        return this.getCount(a => a.isCorrect)
    }

    getSelectedAnswerCount() {
        return this.getCount(a => !!a.isSelected)
    }

    getSelectedCorrectAnswerCount() {
        return this.getCount(a => a.isSelectedCorrect())
    }

    getCount(fn: (a: Option) => boolean) {
        return this.groups.reduce((n, g) => n + g.itemsContainer.getCount(fn), 0)
    }

    selectChoice(group: number, choice?: number): { isSelected: boolean, allSelected: boolean } {
        const selectionResult = this.groups[group].itemsContainer.selectChoice(choice)
      
        if (selectionResult.isSelected) {
            if (selectionResult.allSelected) {
                if (this.getCorrectAnswerCount() === this.getSelectedAnswerCount()) {
                    return { isSelected: true, allSelected: true }
                }

                return { isSelected: true, allSelected: false }
            }

            return { isSelected: true, allSelected: false }
  
        }

        return { isSelected: false, allSelected: false }
    }

    getCorrectAnswers(): string[] {
        const result: string[] = []
        this.groups.forEach(g => {
            g.itemsContainer.options.forEach(o => {
                if (o.isCorrect) {
                    result.push(o.text)
                }
            })
        })

        return result
    }

    static map(o: IOptionTemplate): TemplatedOptionsContainer {
        const parts: OptionTemplatePart[] = []
        const groups: OptionGroup[] = []

        o.parts.forEach(x => parts.push(OptionTemplatePart.map(x)))
        o.groups.forEach(x => groups.push(OptionGroup.map(x)))

        return new TemplatedOptionsContainer(parts, groups)
    }

    static export(o: TemplatedOptionsContainer): IOptionTemplate {
        const parts: IOptionTemplatePart[] = []
        const groups: IOptionGroup[] = []

        o.parts.forEach(x => parts.push(OptionTemplatePart.export(x)))
        o.groups.forEach(x => groups.push(OptionGroup.export(x)))

        return {
            groups: groups,
            parts: parts
        }
    }
}

export class DragDropOptionsContainer implements IOptionsContainer {
    isOrdered: boolean
    options: Option[]
    candidateOptions: Option[] = []
    selectedOptions: Option[] = []

    constructor(options: Option[], isOrdered: boolean) {
        this.isOrdered = isOrdered  
        this.options = options
        this.candidateOptions.push(...options)
    }

    getCorrectAnswerCount() {
        return this.getCount(a => a.isCorrect)
    }

    getSelectedAnswerCount() {
        return this.getCount(a => !!a.isSelected)
    }

    getSelectedCorrectAnswerCount() {
        if (this.isOrdered) {
            return this.getCount(a => a.isSelectedCorrect() && a.order === a.selectedOrder)
        } else {
            return this.getCount(a => a.isSelectedCorrect())
        }
    }

    completeChoice() {
        for (let i = 0; i < this.selectedOptions.length; i++) {
            const option = this.selectedOptions[i]
            option.isSelected = true
            option.selectedOrder = i 
        }
    }

    getCount(fn: (a: Option) => boolean) {
        return this.options.reduce((n, a) => fn(a) ? n + 1 : n, 0)
    }

    static map(c: IDragDropOptionsContainer): DragDropOptionsContainer {
        const options: Option[] = []
        c.options.forEach(x => options.push(Option.map(x)))

        return new DragDropOptionsContainer(options, c.isOrdered)
    }

    static export(c: DragDropOptionsContainer): IDragDropOptionsContainer {
        const options: IOption[] = []
        c.options.forEach(x => options.push(Option.export(x)))

        return {
            options: options,
            isOrdered: c.isOrdered
        }
    }
}

export class OptionTemplatePart implements IOptionTemplatePart {
    value: string | number
    type: OptionTemplatePartType

    constructor(value: string | number, type: OptionTemplatePartType) {
        this.value = value
        this.type = type
    }

    static map(o: IOptionTemplatePart): OptionTemplatePart {
        return new OptionTemplatePart(o.value, o.type)
    }

    static export(o: OptionTemplatePart): IOptionTemplatePart {
        return {
            type: o.type,
            value: o.value
        }
    }
}

export class OptionGroup implements IOptionGroup {
    itemsContainer: SimpleOptionsContainer

    constructor(itemsContainer: SimpleOptionsContainer) {
        this.itemsContainer = itemsContainer        
    }

    static map(o: IOptionGroup): OptionGroup {
        return new OptionGroup(SimpleOptionsContainer.map(o.itemsContainer))
    }

    static export(o: OptionGroup): IOptionGroup {
        return {
           itemsContainer: SimpleOptionsContainer.export(o.itemsContainer) 
        }
    }
}