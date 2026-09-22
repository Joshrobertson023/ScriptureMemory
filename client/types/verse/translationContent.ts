// Mirrors ScriptureMemory.Server.DataAccess.Models.VerseTranslationContent, minus the
// vector Embedding field and the [JsonIgnore]'d VerseNavigation back-reference.
export interface VerseTranslationContent {
    version: string;
    plainText: string;
    contentUsx: string;
    lastUpdated: string | null;
    verseId: string;
}

/**
 * Picks the translation to display for a verse out of its translationContents.
 * Prefers `preferredVersion` (e.g. the user's selected Bible version) when present,
 * falling back to the first available translation, or null if there are none.
 *
 * A verse already saved locally only ever carries the translation it was saved
 * with, so when `preferredVersion` doesn't match, this returns that saved
 * translation as-is rather than fetching a different one - saved verses don't
 * change when the user's preferred version changes later.
 */
export function getVerseTranslation(
    translationContents: VerseTranslationContent[] | null | undefined,
    preferredVersion?: string
): VerseTranslationContent | null {
    if (!translationContents || translationContents.length === 0) {
        return null;
    }

    if (preferredVersion) {
        const match = translationContents.find(
            (tc) => tc.version.toLowerCase() === preferredVersion.toLowerCase()
        );
        if (match) {
            return match;
        }
    }

    return translationContents[0];
}

/**
 * Picks the plain text to display for a verse out of its translationContents.
 * See `getVerseTranslation` for the selection rules. Returns '' if there are none.
 */
export function getVerseText(
    translationContents: VerseTranslationContent[] | null | undefined,
    preferredVersion?: string
): string {
    return getVerseTranslation(translationContents, preferredVersion)?.plainText ?? '';
}
