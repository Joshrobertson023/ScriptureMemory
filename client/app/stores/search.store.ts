import AsyncStorage from "@react-native-async-storage/async-storage";
import { create } from "zustand";
import { createJSONStorage, persist } from "zustand/middleware";
import { Passage } from "../../types/passages/passage";

interface SearchStore {
    searchQuery: string;
    searchResults: Passage[];
    lastVerseDistance: number;

    setSearchQuery: (query: string) => void;
    setSearchResults: (results: Passage[]) => void;
    setLastVerse: (distance: number) => void;
    clearSearchResults: () => void;
    clearSearch: () => void;
}

export const useSearchStore = create<SearchStore>()(
    persist(
        (set) => ({
            searchQuery: '',
            searchResults: [],
            lastVerseDistance: 0.0,

            setSearchQuery(query: string) {
                set({ searchQuery: query });
            },

            setSearchResults(results: Passage[]) {
                set({ searchResults: results });
            },

            setLastVerse(distance: number) {
                set({ lastVerseDistance: distance});
            },

            clearSearchResults() {
                set({ searchResults: [], lastVerseDistance: 0.0 });
            },

            clearSearch() {
                set({ searchQuery: '', searchResults: [], lastVerseDistance: 0.0 });
            },
        }),
        {
            name: 'search-storage',
            storage: createJSONStorage(() => AsyncStorage),
        }
    )
);