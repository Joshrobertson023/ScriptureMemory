import { Reference } from "./reference";
import { VerseTranslationContent } from "./translationContent";

export interface Verse {
    id: string;
    reference: Reference;
    readableReference?: string;
    votes?: number;
    text: string;
    translationContents?: VerseTranslationContent[];
    savedCount: number;
    memorizedCount: number;
    verseNumbers: string;
    searchDistance: number;
}