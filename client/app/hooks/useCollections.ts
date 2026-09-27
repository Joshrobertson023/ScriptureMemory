import { useLiveQuery } from "drizzle-orm/expo-sqlite";
import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { Collection } from "../../types/collection/collection";
import { Verse } from "../../types/verse/verse";
import {
    activeCollectionsQuery,
    allNotesQuery,
    allPassagesQuery,
    archivedCollectionsQuery,
    assembleItems,
    collectionByIdQuery,
    commitDraftCollection,
    createDraftCollection,
    deleteCollection,
    getCollectionsContainingVerses,
    mapCollectionRow,
} from "../database/repositories/collections.repository";
function useAssembledCollections(status: "active" | "archived"): Collection[] {
    const collectionsLQ = useLiveQuery(status === "active" ? activeCollectionsQuery() : archivedCollectionsQuery());
    const passagesLQ = useLiveQuery(allPassagesQuery());

    return useMemo(() => {
        const collectionRows = collectionsLQ.data ?? [];
        const counts = new Map<string, number>();
        for (const row of passagesLQ.data ?? []) {
            if (!row.collectionId) continue;
            counts.set(row.collectionId, (counts.get(row.collectionId) ?? 0) + 1);
        }
        return collectionRows.map((row) => ({
            ...mapCollectionRow(row),
            items: [],
            passageCount: counts.get(row.id) ?? 0,
        }));
    }, [collectionsLQ.data, passagesLQ.data]);
}

export function useUserCollections(): Collection[] {
    return useAssembledCollections("active");
}

export function useArchivedCollections(): Collection[] {
    return useAssembledCollections("archived");
}

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

export function useCollectionsContainingVerses(verseIds: Set<string>): Collection[] {
    const collectionsLQ = useLiveQuery(activeCollectionsQuery());
    const passagesLQ = useLiveQuery(allPassagesQuery());

    return useMemo(() => {
        const matchingIds = new Set(
            (passagesLQ.data ?? [])
                .filter((row) => {
                    if (!row.collectionId) return false;
                    return (JSON.parse(row.verses) as Verse[]).some((verse) => verseIds.has(verse.id));
                })
                .map((row) => row.collectionId as string)
        );

        return (collectionsLQ.data ?? [])
            .filter((row) => matchingIds.has(row.id))
            .map((row) => ({ ...mapCollectionRow(row), items: [] }));
    }, [collectionsLQ.data, passagesLQ.data, verseIds]);
}

export function useCollectionsContainingVersesOnce(cacheKey: string, verseIds: string[], enabled: boolean): Collection[] {
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
    }, [cacheKey, enabled]);

    return collections;
}

export function useDraftCollection(enabled: boolean = true): {
    draftId: string | null;
    collection: Collection | undefined;
    commit: (title: string) => Promise<void>;
    discard: () => Promise<void>;
} {
    const [draftId, setDraftId] = useState<string | null>(null);
    const draftIdRef = useRef<string | null>(null);
    const settledRef = useRef(false);

    useEffect(() => {
        if (!enabled) return;

        let cancelled = false;
        settledRef.current = false;
        createDraftCollection().then((id) => {
            if (cancelled) {
                deleteCollection(id); 
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
