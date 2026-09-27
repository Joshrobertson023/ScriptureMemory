import { UserPassage } from "./passages/userPassage";

export type RootStackParamList = {
    '(tabs)': undefined,
    'createCollection': undefined,
    'collection': {id: string},
    'editCollection': {id: string},
    'practiceSession': {userPassage: UserPassage},
    'chooseBook': undefined,
    'chooseChapter': {book: string},
    'read': {book: string, chapter: number, highlightVerses?: number[]},
    'credits': undefined
}
