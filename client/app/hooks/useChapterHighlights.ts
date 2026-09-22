import { useMemo } from "react";
import { useLiveQuery } from "drizzle-orm/expo-sqlite";
import {
    highlightsQuery,
    addHighlights,
    removeHighlights,
} from "../database/repositories/highlights.repository";
import { HighlightColorId } from "../styles/highlightColors";

/** Live-query wrapper over persisted verse highlights, backing the read-Bible screen. */
export function useChapterHighlights(): {
    highlightColorByVerseId: Map<string, HighlightColorId>;
    addHighlights: (verseIds: string[], color?: HighlightColorId) => Promise<void>;
    removeHighlights: (verseIds: string[]) => Promise<void>;
} {
    const { data } = useLiveQuery(highlightsQuery());

    const highlightColorByVerseId = useMemo(
        () => new Map((data ?? []).map((row) => [row.verseId, row.color as HighlightColorId])),
        [data]
    );

    return { highlightColorByVerseId, addHighlights, removeHighlights };
}
