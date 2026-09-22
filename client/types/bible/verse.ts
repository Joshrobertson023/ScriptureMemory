import { VerseTranslationContent } from "../verse/translationContent";

export type { VerseTranslationContent };

// Mirrors DataAccess.Models.Verse as it actually serializes: Reference, MemorizedCount,
// and SavedCount are all [JsonIgnore] on the server, so they never reach the client.
export interface BibleVerse {
    id: string; // e.g. "PSA.1.1"
    passageId: number | null;
    translationContents: VerseTranslationContent[] | null;
}
