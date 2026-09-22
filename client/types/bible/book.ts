export interface Book {
    displayName: string;
    abbreviation: string;
    fuzzyMatches: string[] | null;
    numChapters: number;
}
