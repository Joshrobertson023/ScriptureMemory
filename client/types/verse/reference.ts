import { BookInfo } from "../bible/chapterJson";

export interface Reference {
    book: string | BookInfo;
    chapter: number;
    verses: number[];
    verseNumbers?: number[];
    readableReference: string;
}