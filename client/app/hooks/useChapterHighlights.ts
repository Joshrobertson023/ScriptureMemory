import { useCallback, useMemo } from "react";
import { useLiveQuery } from "drizzle-orm/expo-sqlite";
import {
    highlightsQuery,
    addHighlights,
    removeHighlights,
} from "../database/repositories/highlights.repository";
import { HighlightColorId } from "../styles/highlightColors";

export function useChapterHighlights(verseIds: string[] = []): {
    highlightColorByVerseId: Map<string, HighlightColorId>;
    toggleHighlights: (verseIds: string[], color: HighlightColorId) => void;
} {
    const verseKey = verseIds.join(",");
    const { data } = useLiveQuery(highlightsQuery(verseIds), [verseKey]);

    const highlightColorByVerseId = useMemo(
        () => new Map((data ?? []).map((row) => [row.verseId, row.color as HighlightColorId])),
        [data]
    );

    const toggleHighlights = useCallback((ids: string[], color: HighlightColorId) => {
        const versesToRemove = ids.filter((id) => highlightColorByVerseId.get(id) === color);

        if (versesToRemove.length > 0) removeHighlights(versesToRemove);
        else addHighlights(ids, color);
    }, [highlightColorByVerseId]);

    return { highlightColorByVerseId, toggleHighlights };
}
