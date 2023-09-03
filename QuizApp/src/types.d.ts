export interface IQuiz {
    title: string;
    questions: IQuestion[];
}

export interface IQuestion {
    text: string;
    optionsContainer: ISimpleOptionsContainer | IOptionTemplate;
    questionType: QuestionType;
}

export interface IOption {
    text: string;
    isCorrect: boolean;
    isSelected?: boolean;
}

export interface IOptionTemplate {
    parts: IOptionTemplatePart[];
    groups: IOptionGroup[];
}

export interface IOptionTemplatePart {
    value: string | number;
    type: OptionTemplatePartType;
}

export interface IOptionGroup {
    itemsContainer: ISimpleOptionsContainer;
}

export interface IOptionsContainer {
    getCorrectAnswerCount(): number;

    getSelectedCorrectAnswerCount(): number;

    getCount(fn: (a: Option) => boolean): number;
}

export interface ISimpleOptionsContainer {
    options: IOption[];
}

export type QuestionType =  'SimpleChoice' | 'TemplatedChoice'
export type OptionTemplatePartType = 'Text' | 'OptionsGroup'
  