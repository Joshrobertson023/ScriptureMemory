import { eq } from "drizzle-orm";
import { useReaderSettingsStore } from "../../stores/readerSettings.store";
import { useUserStore } from "../../stores/user.store";
import { db } from "../client";
import { userPreferencesTable } from "../schema";

export const DEFAULT_BIBLE_VERSION = "kjv";

function getUserId(): string {
    return useUserStore.getState().userId;
}

export function preferencesQuery() {
    return db.select().from(userPreferencesTable).where(eq(userPreferencesTable.userId, getUserId()));
}

export async function getPreferredBibleVersion(): Promise<string> {
    const [row] = await preferencesQuery();
    return row?.preferredBibleVersion ?? DEFAULT_BIBLE_VERSION;
}

export async function setPreferredBibleVersion(version: string): Promise<void> {
    await db.update(userPreferencesTable)
        .set({ preferredBibleVersion: version })
        .where(eq(userPreferencesTable.userId, getUserId()));

    // Changing the app-wide preference also takes over the read screen,
    // so drop any translation chosen there just for that screen.
    useReaderSettingsStore.getState().setTranslationOverride(null);
}
