import { useQuery } from '@tanstack/react-query';
import { useEffect, useMemo } from 'react';
import { useUserStore } from '../stores/user.store';
import { useUserAuthStore } from '../stores/userAuth.store';
import { getVerseCard } from '../api/verses.api';
import { Verse } from '../../types/verse/verse';
import { useVerseCardCacheStore } from '../stores/verseCardCache.store';
import { useBibleVersion } from './useBibleVersion';

export const useVerseCard = (verses: Verse[], passageKey: string) => {
    const userId = useUserStore((state) => state.user.id);
    const jwt = useUserAuthStore((state) => state.jwt);
    const { version: bibleVersion } = useBibleVersion();
    const setVerseCard = useVerseCardCacheStore((state) => state.setVerseCard);

    const verseIds = useMemo(() => verses.map(v => v.id).sort(), [verses]);
    const verseKey = useMemo(() => verseIds.join(','), [verseIds]);
    const normalizedPassageKey = useMemo(() => passageKey.trim(), [passageKey]);
    const cacheKey = useMemo(
        () => `${userId}:${normalizedPassageKey}:${verseKey}`,
        [userId, normalizedPassageKey, verseKey]
    );
    const cachedData = useVerseCardCacheStore((state) => state.cache[cacheKey]);
    const translation = verses[0]?.translationContents?.at(0)?.version ?? bibleVersion;
    const fetchVerseText = verses.some((verse) => !verse.translationContents?.at(0)?.plainText);

    const query = useQuery({
        queryKey: ['verseCard', cacheKey, translation, fetchVerseText],
        queryFn: () => getVerseCard(Number(userId) || 0, verseIds, translation, fetchVerseText, jwt),
        staleTime: Infinity,
        gcTime: Infinity,
        refetchOnMount: false,
        refetchOnWindowFocus: false,
        enabled: verseIds.length > 0 && !!userId && !!jwt && !cachedData,
    });

    useEffect(() => {
        if (query.data) {
            setVerseCard(cacheKey, query.data);
        }
    }, [cacheKey, query.data, setVerseCard]);

    return {
        ...query,
        data: cachedData ?? query.data,
        isLoading: !cachedData && query.isLoading,
    };
};