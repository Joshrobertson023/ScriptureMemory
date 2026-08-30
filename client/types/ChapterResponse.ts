import { Book } from "./bible/book";
import { BibleVerse } from "./bible/verse";

// Mirrors ScriptureMemory.Server.Data.Dtos.ResponseChapterDto, returned by
// POST /bible/chapter/{bible}/{book}/{chapter}.
export interface ChapterResponse {
    book: Book;
    chapterNumber: number;
    verses: BibleVerse[];
    title: string | null;
    copyright: string;
}
