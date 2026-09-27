import { useLiveQuery } from "drizzle-orm/expo-sqlite";
import React, { useCallback, useDeferredValue, useMemo } from "react";
import { FlatList, Text, View } from "react-native";
import { Collection } from "../../../types/collection/collection";
import { Reference } from "../../../types/verse/reference";
import { allPassagesQuery } from "../../database/repositories/collections.repository";
import useGlobalStyles from "../../styles/gobalStyles";
import { getBookName, getVerseNumbers } from "../../utils/referenceUtils";
import { CollectionCard } from "./collectionCard";

interface SearchResultsProps {
    query: string;
    collections: Collection[];
}

interface PassageRef {
    collectionId: string;
    book: string;
    chapter: number;
    verses: number[];
}

const referencePattern = /^(\d?\s*[a-z][a-z.\s]*?)\s*(\d+)?(?:\s*:\s*(\d+)(?:\s*-\s*(\d+))?)?$/;

const normalizeBook = (book: string) => book.toLowerCase().replace(/[\s.]/g, '');

const parseReferenceQuery = (query: string) => {
    const match = referencePattern.exec(query);
    if (!match) return null;
    const start = match[3] ? Number(match[3]) : undefined;
    return {
        book: normalizeBook(match[1]),
        chapter: match[2] ? Number(match[2]) : undefined,
        start,
        end: match[4] ? Number(match[4]) : start,
    };
};

const SearchResult = ({ query, collections }: SearchResultsProps) => {
    const globalStyles = useGlobalStyles();
    const deferredQuery = useDeferredValue(query);
    const passagesLQ = useLiveQuery(allPassagesQuery());

    const passageRefs = useMemo(() => {
        const refs: PassageRef[] = [];
        for (const row of passagesLQ.data ?? []) {
            if (!row.collectionId) continue;
            const reference = JSON.parse(row.reference) as Reference;
            refs.push({
                collectionId: row.collectionId,
                book: normalizeBook(getBookName(reference)),
                chapter: reference.chapter,
                verses: getVerseNumbers(reference),
            });
        }
        return refs;
    }, [passagesLQ.data]);

    const results = useMemo(() => {
        const search = deferredQuery.trim().toLowerCase();
        if (!search) return [];

        const reference = parseReferenceQuery(search);
        const referenceMatches = new Set<string>();
        if (reference) {
            for (const ref of passageRefs) {
                if (!ref.book.startsWith(reference.book)) continue;
                if (reference.chapter !== undefined && ref.chapter !== reference.chapter) continue;
                if (reference.start !== undefined && !ref.verses.some((v) => v >= reference.start! && v <= reference.end!)) continue;
                referenceMatches.add(ref.collectionId);
            }
        }

        return collections.filter((c) => c.title.toLowerCase().includes(search) || referenceMatches.has(c.id));
    }, [deferredQuery, passageRefs, collections]);

    const renderItem = useCallback(({ item }: { item: Collection }) => (
        <CollectionCard collection={item} />
    ), []);

    return (
        <FlatList
            data={results}
            keyExtractor={(item) => item.id}
            renderItem={renderItem}
            keyboardShouldPersistTaps="handled"
            contentContainerStyle={globalStyles.screen}
            ListEmptyComponent={
                <View style={{ marginTop: 30, alignItems: 'center' }}>
                    <Text style={globalStyles.p3}>No collections match "{deferredQuery.trim()}"</Text>
                </View>
            }
            ListFooterComponent={<View style={{ height: 100 }} />}
        />
    );
};

export default SearchResult;
