import { useLiveQuery } from "drizzle-orm/expo-sqlite";
import { useMemo } from "react";
import {
    DEFAULT_BIBLE_VERSION,
    preferencesQuery,
    setPreferredBibleVersion,
} from "../database/repositories/userPreferences.repository";

export function useBibleVersion(): { version: string; setVersion: (version: string) => Promise<void> } {
    const { data } = useLiveQuery(preferencesQuery());

    const version = useMemo(
        () => data?.[0]?.preferredBibleVersion ?? DEFAULT_BIBLE_VERSION,
        [data]
    );

    return { version, setVersion: setPreferredBibleVersion };
}
