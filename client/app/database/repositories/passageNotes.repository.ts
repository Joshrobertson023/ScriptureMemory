import { and, asc, eq } from "drizzle-orm";
import * as Crypto from "expo-crypto";
import { Passage } from "../../../types/passages/passage";
import { useUserStore } from "../../stores/user.store";
import { db } from "../client";
import { passageNotesTable } from "../schema";

function getUserId(): string {
    return useUserStore.getState().userId;
}

export const getPassageNoteKey = (passage: Passage): string =>
    passage.verses.map((verse) => verse.id).sort().join(',');

export function passageNotesQuery(passageKey: string) {
    return db.select().from(passageNotesTable)
        .where(and(eq(passageNotesTable.userId, getUserId()), eq(passageNotesTable.passageKey, passageKey)))
        .orderBy(asc(passageNotesTable.dateAdded));
}

export async function addPassageNote(passageKey: string, text: string): Promise<void> {
    const note = { id: Crypto.randomUUID(), passageKey, userId: getUserId(), text };
    console.log('[passageNotes] adding note', note);
    try {
        await db.insert(passageNotesTable).values(note);
        const rows = await db.select().from(passageNotesTable).where(eq(passageNotesTable.passageKey, passageKey));
        console.log('[passageNotes] added note, rows for key now:', rows);
    } catch (error) {
        console.error('[passageNotes] failed to add note', error);
    }
}

export async function updatePassageNote(id: string, text: string): Promise<void> {
    console.log('[passageNotes] updating note', { id, text });
    try {
        await db.update(passageNotesTable).set({ text }).where(eq(passageNotesTable.id, id));
        console.log('[passageNotes] updated note', id);
    } catch (error) {
        console.error('[passageNotes] failed to update note', error);
    }
}

export async function deletePassageNote(id: string): Promise<void> {
    console.log('[passageNotes] deleting note', id);
    try {
        await db.delete(passageNotesTable).where(eq(passageNotesTable.id, id));
        console.log('[passageNotes] deleted note', id);
    } catch (error) {
        console.error('[passageNotes] failed to delete note', error);
    }
}
