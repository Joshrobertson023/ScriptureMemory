// Mirrors the server's json content-type DTOs (Data/Dtos/ChapterJsonDtos.cs).
// This preserves the original paragraph formatting from the Bible API (paragraph style,
// poetry indentation, italicized "added" words) and tags every span of text with the verse
// it belongs to, so the client can render formatted text and still know which verse was tapped.

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
