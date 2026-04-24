import { UserPassage } from "./passages/userPassage"

export type RootStackParamList = {
    '(tabs)': undefined,
    'createCollection': undefined,
    'collection': {id: number},
    'editCollection': undefined,
    'practiceSession': {practicingPassage: UserPassage},
    'chooseBook': undefined,
    'chooseChapter': {book: string},
    'read': {book: string, chapter: number},
}