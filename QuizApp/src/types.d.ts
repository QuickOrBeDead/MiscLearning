export interface Quiz {
    title: string;
    questions: Question[];
}

export interface Question {
    text: string;
    answers: Answer[];
}

export interface Answer {
    text: string;
    isCorrect: boolean;
    isSelected?: boolean;
}