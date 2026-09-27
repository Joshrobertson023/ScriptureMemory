import { BookInfo } from "../../types/bible/chapterJson";
import { Reference } from "../../types/verse/reference";

export const getBookName = (reference: Reference): string =>
    typeof reference.book === 'string' ? reference.book : (reference.book as BookInfo).displayName;

export const getVerseNumbers = (reference: Reference): number[] =>
    reference.verses ?? reference.verseNumbers ?? [];
