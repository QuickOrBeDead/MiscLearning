import { IOption, IOptionGroup, IOptionTemplate, IOptionTemplatePart, IOptionsContainer, IQuestion, IQuiz, ISimpleOptionsContainer, QuestionType, OptionTemplatePartType } from "./types";

export class Quiz implements IQuiz {
    title: string;
    questions: Question[];

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
}

export class Question implements IQuestion {
    text: string;
    optionsContainer: SimpleOptionsContainer | TemplatedOptionsContainer;
    questionType: QuestionType;

    constructor(text: string, optionsContainer: SimpleOptionsContainer | TemplatedOptionsContainer, questionType: QuestionType) {
        this.text = text
        this.optionsContainer = optionsContainer
        this.questionType = questionType
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
        let optionsContainer: SimpleOptionsContainer | TemplatedOptionsContainer
        if (q.questionType === 'SimpleChoice') {
            optionsContainer = SimpleOptionsContainer.map(q.optionsContainer as ISimpleOptionsContainer)
        } else if (q.questionType === 'TemplatedChoice') {
            optionsContainer = TemplatedOptionsContainer.map(q.optionsContainer as IOptionTemplate)
        } else {
            throw new Error(`unknown questionType: ${q.questionType}`)
        }
        
        return new Question(q.text, optionsContainer, q.questionType)
    }
}

export class Option implements IOption {
    text: string;
    isCorrect: boolean;
    isSelected?: boolean;

    constructor(text: string, isCorrect: boolean, isSelected?: boolean) {
        this.text = text
        this.isCorrect = isCorrect
        this.isSelected = isSelected
    }

    isSelectedCorrect(): boolean {
        return this.isCorrect && !!this.isSelected
    }

    static map(o: IOption): Option {
        return new Option(o.text, o.isCorrect, o.isSelected)
    }
}

export class SimpleOptionsContainer implements ISimpleOptionsContainer, IOptionsContainer {
    options: Option[];

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
}

export class TemplatedOptionsContainer implements IOptionTemplate, IOptionsContainer {
    parts: OptionTemplatePart[];
    groups: OptionGroup[];

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

    static map(o: IOptionTemplate): TemplatedOptionsContainer {
        const parts: OptionTemplatePart[] = []
        const groups: OptionGroup[] = []

        o.parts.forEach(x => parts.push(OptionTemplatePart.map(x)))
        o.groups.forEach(x => groups.push(OptionGroup.map(x)))

        return new TemplatedOptionsContainer(parts, groups)
    }
}

export class OptionTemplatePart implements IOptionTemplatePart {
    value: string | number;
    type: OptionTemplatePartType;

    constructor(value: string | number, type: OptionTemplatePartType) {
        this.value = value
        this.type = type
    }

    static map(o: IOptionTemplatePart): OptionTemplatePart {
        return new OptionTemplatePart(o.value, o.type)
    }
}

export class OptionGroup implements IOptionGroup {
    itemsContainer: SimpleOptionsContainer;

    constructor(itemsContainer: SimpleOptionsContainer) {
        this.itemsContainer = itemsContainer        
    }

    static map(o: IOptionGroup): OptionGroup {
        return new OptionGroup(SimpleOptionsContainer.map(o.itemsContainer))
    }
}