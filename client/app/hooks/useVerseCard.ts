import { useQuery } from '@tanstack/react-query';
import { useMemo } from 'react';
import { useUserStore } from '../stores/user.store';
import { useUserAuthStore } from '../stores/userAuth.store';
import { getVerseCard } from '../api/verses.api';
import { Verse } from '../../types/verse/verse';
import { queryClient } from './queryClient';

export const useVerseCard = (verses: Verse[]) => {
    const { user } = useUserStore();
    const { jwt } = useUserAuthStore();

    const verseIds = useMemo(() => verses.map(v => v.id), [verses]);
    const verseKey = useMemo(() => verseIds.sort((a, b) => a - b).join(','), [verseIds]);

    return useQuery({
        queryKey: ['verseCard', verseKey],
        queryFn: () => getVerseCard(user.id, verseIds, jwt),
        staleTime: 1000 * 60 * 5,
        gcTime: Infinity,
        enabled: verseIds.length > 0 && !!user.id && !!jwt,
    });
};