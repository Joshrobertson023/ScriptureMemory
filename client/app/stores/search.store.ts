import AsyncStorage from "@react-native-async-storage/async-storage";
import { create } from "zustand";
import { createJSONStorage, persist } from "zustand/middleware";
import { Passage } from "../../types/passages/passage";

interface SearchStore {
    searchQuery: string;
    searchResults: Passage[];

    setSearchQuery: (query: string) => void;
    setSearchResults: (results: Passage[]) => void;
    clearSearchResults: () => void;
    clearSearch: () => void;
}

export const useSearchStore = create<SearchStore>()(
        (set) => ({
            searchQuery: '',
            searchResults: [],

            setSearchQuery(query: string) {
                set({ searchQuery: query });
            },

            setSearchResults(results: Passage[]) {
                set({ searchResults: results });
            },

            clearSearchResults() {
                set({ searchResults: [] });
            },

            clearSearch() {
                set({ searchQuery: '', searchResults: [] });
            },
        }),
);