import { useCallback } from "react";
import { Book } from "../../types/bible/book";

export const allBooks: Book[] = [
    { displayName: "Genesis", numChapters: 50, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Exodus", numChapters: 40, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Leviticus", numChapters: 27, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Numbers", numChapters: 36, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Deuteronomy", numChapters: 34, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Joshua", numChapters: 24, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Judges", numChapters: 21, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Ruth", numChapters: 4, abbreviation: "", fuzzyMatches: [] },
    { displayName: "1 Samuel", numChapters: 31, abbreviation: "", fuzzyMatches: [] },
    { displayName: "2 Samuel", numChapters: 24, abbreviation: "", fuzzyMatches: [] },
    { displayName: "1 Kings", numChapters: 22, abbreviation: "", fuzzyMatches: [] },
    { displayName: "2 Kings", numChapters: 25, abbreviation: "", fuzzyMatches: [] },
    { displayName: "1 Chronicles", numChapters: 29, abbreviation: "", fuzzyMatches: [] },
    { displayName: "2 Chronicles", numChapters: 36, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Ezra", numChapters: 10, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Nehemiah", numChapters: 13, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Esther", numChapters: 10, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Job", numChapters: 42, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Psalms", numChapters: 150, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Proverbs", numChapters: 31, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Ecclesiastes", numChapters: 12, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Song of Solomon", numChapters: 8, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Isaiah", numChapters: 66, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Jeremiah", numChapters: 52, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Lamentations", numChapters: 5, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Ezekiel", numChapters: 48, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Daniel", numChapters: 12, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Hosea", numChapters: 14, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Joel", numChapters: 3, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Amos", numChapters: 9, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Obadiah", numChapters: 1, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Jonah", numChapters: 4, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Micah", numChapters: 7, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Nahum", numChapters: 3, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Habakkuk", numChapters: 3, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Zephaniah", numChapters: 3, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Haggai", numChapters: 2, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Zechariah", numChapters: 14, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Malachi", numChapters: 4, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Matthew", numChapters: 28, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Mark", numChapters: 16, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Luke", numChapters: 24, abbreviation: "", fuzzyMatches: [] },
    { displayName: "John", numChapters: 21, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Acts", numChapters: 28, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Romans", numChapters: 16, abbreviation: "", fuzzyMatches: [] },
    { displayName: "1 Corinthians", numChapters: 16, abbreviation: "", fuzzyMatches: [] },
    { displayName: "2 Corinthians", numChapters: 13, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Galatians", numChapters: 6, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Ephesians", numChapters: 6, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Philippians", numChapters: 4, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Colossians", numChapters: 4, abbreviation: "", fuzzyMatches: [] },
    { displayName: "1 Thessalonians", numChapters: 5, abbreviation: "", fuzzyMatches: [] },
    { displayName: "2 Thessalonians", numChapters: 3, abbreviation: "", fuzzyMatches: [] },
    { displayName: "1 Timothy", numChapters: 6, abbreviation: "", fuzzyMatches: [] },
    { displayName: "2 Timothy", numChapters: 4, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Titus", numChapters: 3, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Philemon", numChapters: 1, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Hebrews", numChapters: 13, abbreviation: "", fuzzyMatches: [] },
    { displayName: "James", numChapters: 5, abbreviation: "", fuzzyMatches: [] },
    { displayName: "1 Peter", numChapters: 5, abbreviation: "", fuzzyMatches: [] },
    { displayName: "2 Peter", numChapters: 3, abbreviation: "", fuzzyMatches: [] },
    { displayName: "1 John", numChapters: 5, abbreviation: "", fuzzyMatches: [] },
    { displayName: "2 John", numChapters: 1, abbreviation: "", fuzzyMatches: [] },
    { displayName: "3 John", numChapters: 1, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Jude", numChapters: 1, abbreviation: "", fuzzyMatches: [] },
    { displayName: "Revelation", numChapters: 22, abbreviation: "", fuzzyMatches: [] },
];

export const useBooks = () => {
    const previous = useCallback((existingBook: string, chapter: number): {book: string, nextChapter: number} => {
        if (chapter > 1) {
            return {book: existingBook, nextChapter: chapter - 1}
        }

        const previousBook = allBooks.at(allBooks.findIndex(b => b.displayName === existingBook) - 1);

        if (previousBook === undefined) {
            throw new Error('Previous book not found.');
        }

        return {book: previousBook.displayName, nextChapter: previousBook.numChapters}
    }, []);

    const next = useCallback((existingBook: string, chapter: number): {book: string, nextChapter: number} => {
        const currentBook = allBooks.find(b => b.displayName === existingBook);

        if (currentBook === undefined) {
            throw new Error('Error finding book')
        }

        if (chapter < currentBook.numChapters) {
            return {book: existingBook, nextChapter: chapter + 1}
        }

        const nextBook = allBooks.at(allBooks.findIndex(b => b.displayName === existingBook) + 1);

        if (nextBook === undefined) {
            throw new Error('Previous book not found.');
        }

        return {book: nextBook.displayName, nextChapter: 1}
    }, []);

    return { previous, next }
};