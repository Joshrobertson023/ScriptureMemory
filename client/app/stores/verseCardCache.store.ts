import { create } from "zustand";
import { VerseCardResponse } from "../../types/verse/verseCard";

interface VerseCardCacheStore {
    cache: Record<string, VerseCardResponse>;

    setVerseCard: (cacheKey: string, response: VerseCardResponse) => void;
    clearVerseCardCache: () => void;
}

export const useVerseCardCacheStore = create<VerseCardCacheStore>()((set) => ({
    cache: {},

    setVerseCard(cacheKey: string, response: VerseCardResponse) {
        set((state) => ({
            cache: {
                ...state.cache,
                [cacheKey]: response,
            },
        }));
    },

    clearVerseCardCache() {
        set({ cache: {} });
    },
}));