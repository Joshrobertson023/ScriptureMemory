import { useQuery } from '@tanstack/react-query';
import { useEffect, useMemo } from 'react';
import { useUserStore } from '../stores/user.store';
import { useUserAuthStore } from '../stores/userAuth.store';
import { getVerseCard } from '../api/verses.api';
import { Verse } from '../../types/verse/verse';
import { useVerseCardCacheStore } from '../stores/verseCardCache.store';

export const useVerseCard = (verses: Verse[], passageKey: string) => {
    const { user } = useUserStore();
    const { jwt } = useUserAuthStore();
    const setVerseCard = useVerseCardCacheStore((state) => state.setVerseCard);

    const verseIds = useMemo(() => verses.map(v => v.id).sort((a, b) => a - b), [verses]);
    const verseKey = useMemo(() => verseIds.join(','), [verseIds]);
    const normalizedPassageKey = useMemo(() => passageKey.trim(), [passageKey]);
    const cacheKey = useMemo(
        () => `${user.id}:${normalizedPassageKey}:${verseKey}`,
        [user.id, normalizedPassageKey, verseKey]
    );
    const cachedData = useVerseCardCacheStore((state) => state.cache[cacheKey]);

    const query = useQuery({
        queryKey: ['verseCard', cacheKey],
        queryFn: () => getVerseCard(user.id, verseIds, jwt),
        staleTime: Infinity,
        gcTime: Infinity,
        refetchOnMount: false,
        refetchOnWindowFocus: false,
        enabled: verseIds.length > 0 && !!user.id && !!jwt && !cachedData,
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