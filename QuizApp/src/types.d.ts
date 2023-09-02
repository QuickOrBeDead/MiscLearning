export interface Quiz {
    title: string;
    questions: Question[];
}

export interface Question {
    text: string;
    answers: Answer[] | AnswerTemplate;
    questionType: QuestionType;
}

export interface Answer {
    text: string;
    isCorrect: boolean;
    isSelected?: boolean;
}

export interface AnswerTemplate {
    parts: AnswerTemplatePart[];
    groups: AnswerGroup[];
}

export interface AnswerTemplatePart {
    value: string | number;
    type: AnswerTemplatePartType;
}

export interface AnswerGroup {
    answers: Answer[];
}

export type QuestionType =  'SingleChoice' | 'MultipleChoice' | 'AnswerTemplate'
export type AnswerTemplatePartType = 'Text' | 'AnswerGroup'
  