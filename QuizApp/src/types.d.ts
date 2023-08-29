export interface Quiz {
    title: string;
    questions: Question[];
}

export interface Question {
    text: string;
    answers: Answer[];
    questionType: QuestionType;
}

export interface Answer {
    text: string;
    isCorrect: boolean;
    isSelected?: boolean;
}

export type QuestionType =  'SingleChoice' | 'MultipleChoice'
  