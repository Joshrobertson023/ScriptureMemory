export interface BookInfo {
    displayName: string;
    abbreviation: string;
    fuzzyMatches: string[] | null;
    numChapters: number;
}

export interface ChapterSpan {
    text: string;
    verseId: string;
    verseNumber: number;
    isVerseStart: boolean;
    italic: boolean;
}

export interface ChapterParagraph {
    style: string;
    spans: ChapterSpan[];
}

export interface ChapterVerseJson {
    verseId: string;
    verseNumber: number;
    text: string;
}

export interface ResponseChapterJson {
    book: BookInfo;
    chapterNumber: number;
    paragraphs: ChapterParagraph[];
    verses: ChapterVerseJson[];
    copyright: string;
}

export interface ResponseVerseJson {
    verseId: string;
    verseNumber: number;
    spans: ChapterSpan[];
    text: string;
}
