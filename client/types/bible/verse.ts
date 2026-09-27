import { VerseTranslationContent } from "../verse/translationContent";

export type { VerseTranslationContent };

export interface BibleVerse {
    id: string;
    passageId: number | null;
    translationContents: VerseTranslationContent[] | null;
}
