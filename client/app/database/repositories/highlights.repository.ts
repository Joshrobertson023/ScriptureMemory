import { and, eq, inArray } from "drizzle-orm";
import * as Crypto from 'expo-crypto';
import { DEFAULT_HIGHLIGHT_COLOR, HighlightColorId } from "../../styles/highlightColors";
import { useUserStore } from "../../stores/user.store";
import { db } from "../client";
import { highlightsTable } from "../schema";

function getUserId(): string {
    return useUserStore.getState().userId;
}

export function highlightsQuery(verseIds?: string[]) {
    const userId = getUserId();
    if (verseIds && verseIds.length > 0) {
        return db.select().from(highlightsTable).where(and(
            eq(highlightsTable.userId, userId),
            inArray(highlightsTable.verseId, verseIds)
        ));
    }
    return db.select().from(highlightsTable).where(eq(highlightsTable.userId, userId));
}

export async function addHighlights(verseIds: string[], color: HighlightColorId = DEFAULT_HIGHLIGHT_COLOR): Promise<void> {
    const rows = await highlightsQuery();
    const existingByVerseId = new Map(rows.map((r) => [r.verseId, r]));

    for (const verseId of verseIds) {
        const existing = existingByVerseId.get(verseId);
        if (existing) {
            await db.update(highlightsTable).set({ color }).where(eq(highlightsTable.id, existing.id));
        } else {
            await db.insert(highlightsTable).values({ id: Crypto.randomUUID(), verseId, userId: getUserId(), color });
        }
    }
}

export async function removeHighlights(verseIds: string[]): Promise<void> {
    const rows = await highlightsQuery();
    const idsToDelete = rows.filter((r) => verseIds.includes(r.verseId)).map((r) => r.id);
    for (const id of idsToDelete) {
        await db.delete(highlightsTable).where(eq(highlightsTable.id, id));
    }
}
