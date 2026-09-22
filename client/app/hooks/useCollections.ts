import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { useLiveQuery } from "drizzle-orm/expo-sqlite";
import {
    activeCollectionsQuery,
    archivedCollectionsQuery,
    allPassagesQuery,
    allNotesQuery,
    collectionByIdQuery,
    mapCollectionRow,
    assembleItems,
    createDraftCollection,
    commitDraftCollection,
    deleteCollection,
    getCollectionsContainingVerses,
} from "../database/repositories/collections.repository";
import { Collection } from "../../types/collection/collection";

// Reactive read hooks - the DB-backed replacement for reading `useCollectionsStore`.
// Writes don't need a hook (they're plain async functions with no internal React
// state); call the collections.repository functions directly from event handlers,
// same as the old store's methods were called directly.

function useAssembledCollections(status: "active" | "archived"): Collection[] {
    const collectionsLQ = useLiveQuery(status === "active" ? activeCollectionsQuery() : archivedCollectionsQuery());
    const passagesLQ = useLiveQuery(allPassagesQuery());
    const notesLQ = useLiveQuery(allNotesQuery());

    return useMemo(() => {
        const collectionRows = collectionsLQ.data ?? [];
        const passageRows = passagesLQ.data ?? [];
        const noteRows = notesLQ.data ?? [];
        return collectionRows.map((row) => ({
            ...mapCollectionRow(row),
            items: assembleItems(passageRows, noteRows, row.id),
        }));
    }, [collectionsLQ.data, passagesLQ.data, notesLQ.data]);
}

/** Active (non-draft, non-archived) collections for the current user, in orderPosition order. */
export function useUserCollections(): Collection[] {
    return useAssembledCollections("active");
}

export function useArchivedCollections(): Collection[] {
    return useAssembledCollections("archived");
}

/**
 * A single collection (any status - active, archived, or draft) assembled with its items.
 * `isLoading` is true until the underlying live query has resolved at least once, so
 * callers can tell "hasn't loaded yet" apart from "genuinely doesn't exist" - useLiveQuery
 * starts out with an empty result before its first resolution, which would otherwise look
 * identical to a real not-found.
 */
export function useCollection(id: string | null | undefined): { collection: Collection | undefined; isLoading: boolean } {
    const rowLQ = useLiveQuery(collectionByIdQuery(id ?? ""), [id]);
    const passagesLQ = useLiveQuery(allPassagesQuery());
    const notesLQ = useLiveQuery(allNotesQuery());

    const collection = useMemo(() => {
        const row = rowLQ.data?.[0];
        if (!row) return undefined;
        return {
            ...mapCollectionRow(row),
            items: assembleItems(passagesLQ.data ?? [], notesLQ.data ?? [], row.id),
        };
    }, [rowLQ.data, passagesLQ.data, notesLQ.data]);

    return { collection, isLoading: rowLQ.updatedAt === undefined };
}

/** Collections containing a passage that includes any of the given (shared, numeric) verse ids. */
export function useCollectionsContainingVerses(verseIds: Set<number>): Collection[] {
    const collections = useUserCollections();
    return useMemo(
        () =>
            collections.filter((c) =>
                c.items.some(
                    (i) => i.type === "passage" && i.passage.passage.verses.some((v) => verseIds.has(v.id))
                )
            ),
        [collections, verseIds]
    );
}

export function useCollectionsContainingVersesOnce(cacheKey: string, verseIds: number[], enabled: boolean): Collection[] {
    const [collections, setCollections] = useState<Collection[]>([]);

    useEffect(() => {
        if (!enabled) {
            setCollections([]);
            return;
        }

        let cancelled = false;
        getCollectionsContainingVerses(verseIds).then((result) => {
            if (!cancelled) setCollections(result);
        });
        return () => {
            cancelled = true;
        };
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [cacheKey, enabled]);

    return collections;
}

/**
 * The current "in progress" new-collection draft - a real collectionsTable row
 * (status='draft') created fresh each time this hook mounts, so it's a real,
 * reorderable, browsable collection while it's being built and survives the app
 * being backgrounded/killed mid-edit. If the screen/sheet using it unmounts without
 * calling `commit`, the draft (and any items added to it) is deleted.
 */
export function useDraftCollection(enabled: boolean = true): {
    draftId: string | null;
    collection: Collection | undefined;
    commit: (title: string) => Promise<void>;
    discard: () => Promise<void>;
} {
    const [draftId, setDraftId] = useState<string | null>(null);
    const draftIdRef = useRef<string | null>(null);
    const settledRef = useRef(false); // true once committed or explicitly discarded

    useEffect(() => {
        if (!enabled) return;

        let cancelled = false;
        settledRef.current = false;
        createDraftCollection().then((id) => {
            if (cancelled) {
                deleteCollection(id); // flow was disabled/unmounted before the insert resolved
                return;
            }
            draftIdRef.current = id;
            setDraftId(id);
        });
        return () => {
            cancelled = true;
            if (draftIdRef.current && !settledRef.current) {
                deleteCollection(draftIdRef.current);
            }
            draftIdRef.current = null;
            setDraftId(null);
        };
    }, [enabled]);

    const commit = useCallback(async (title: string) => {
        if (!draftIdRef.current) return;
        settledRef.current = true;
        await commitDraftCollection(draftIdRef.current, title);
    }, []);

    const discard = useCallback(async () => {
        if (!draftIdRef.current) return;
        settledRef.current = true;
        await deleteCollection(draftIdRef.current);
    }, []);

    const { collection } = useCollection(draftId);
    return { draftId, collection, commit, discard };
}
