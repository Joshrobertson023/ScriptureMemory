import { create } from "zustand";
import { UserPassage } from "../../types/passages/userPassage";
import { initialUserPassage } from "./collections.store";
import { Note } from "../../types/note";
import { VerseCardResponse } from "../../types/verse/verseCard";
import { getSimilarVerses, getVerseCard } from "../api/verses.api";
import { useUserAuthStore } from "./userAuth.store";
import { useUserStore } from "./user.store";
import { queryClient } from "../hooks/queryClient";
import { Passage } from "../../types/passages/passage";
import { Category } from "../../types/category";

export const getPassageCacheKey = (up: UserPassage) => {
    const { book, chapter, verses } = up.passage.reference;
    return `${up.id}-${book}:${chapter}:${verses.join(',')}`; // Remove up.id if stopped working
};

interface BottomSheetsStore {
    passageSheetStack: UserPassage[];
    passageBottomSheet: UserPassage;
    passageSheetOpen: boolean;
    passageSheetPendingTransition: { kind: "next"; passage: UserPassage } | { kind: "last" } | null;

    passageCardCache: Record<string, VerseCardResponse>;
    similarPassagesCache: Record<string, Passage[]>;
    viewNotesBottomSheet: UserPassage;
    viewNotesSheetOpen: boolean;
    saveToCollectionBottomSheet: UserPassage;
    saveToCollectionSheetOpen: boolean;
    categoriesBottomSheet: Category | null;
    categoriesSheetOpen: boolean;

    noteBottomSheet: Note;
    noteBottomSheetItemId: number | null;
    noteSheetOpen: boolean;
    syncSheetOpen: boolean;

    setPassageBottomSheet: (up: UserPassage) => void;
    setPassageSheetOpen: (o: boolean) => void;
    setPassageSheetPendingTransition: (transition: { kind: "next"; passage: UserPassage } | { kind: "last" } | null) => void;

    pushPassage: (up: UserPassage) => void;
    // Push passage, set as this, close and reopen
    popPassage: () => void;
    // Pop passage, set last, close and reopen
    setBottomPassageLastInStack: () => void;
    clearStack: () => void;
    // Reset array, close

    setPassageCardCache: (cacheKey: string, data: VerseCardResponse) => void;
    clearPassageCardCache: () => void;
    setSimilarPassagesCache: (cacheKey: string, data: Passage[]) => void;
    clearSimilarPassagesCache: () => void;
    setViewNotesBottomSheet: (up: UserPassage) => void;
    setViewNotesSheetOpen: (o: boolean) => void;
    setSaveToCollectionBottomSheet: (up: UserPassage) => void;
    setSaveToCollectionSheetOpen: (o: boolean) => void;
    setCategoriesBottomSheet: (category: Category | null) => void;
    setCategoriesSheetOpen: (o: boolean) => void;
    clearCategoriesBottomSheet: () => void;

    setNoteBottomSheet: (note: Note, itemId: number | null) => void;
    setNoteSheetOpen: (o: boolean) => void;
    setSyncSheetOpen: (o: boolean) => void;
    clearNoteBottomSheet: () => void;
}

const initialNote: Note = {
    id: 0,
    text: ""
};

export const useBottomSheetsStore = create<BottomSheetsStore>()(
    (set, get) => ({
        passageSheetStack: [],
        passageBottomSheet: initialUserPassage,
        passageSheetOpen: false,
        passageSheetPendingTransition: null,

        passageCardCache: {},
        similarPassagesCache: {},
        viewNotesBottomSheet: initialUserPassage,
        viewNotesSheetOpen: false,
        saveToCollectionBottomSheet: initialUserPassage,
        saveToCollectionSheetOpen: false,
        categoriesBottomSheet: null,
        categoriesSheetOpen: false,

        noteBottomSheet: initialNote,
        noteBottomSheetItemId: null,
        noteSheetOpen: false,
        syncSheetOpen: false,

        setPassageBottomSheet(up: UserPassage) {
            set(() => ({ passageBottomSheet: up }));
            void loadPassageCard(up, set, get);
            void loadSimilarPassages(up, set, get);
        },
        setPassageSheetOpen(o: boolean) {
            set(() => ({ passageSheetOpen: o }));
        },

        setPassageSheetPendingTransition(transition) {
            set(() => ({ passageSheetPendingTransition: transition }));
        },

        pushPassage(up: UserPassage) {
            set((state) => ({
                passageSheetStack: [...state.passageSheetStack, up],
            }));
        },
        popPassage() {
            const state = get();

            if (state.passageSheetStack.length <= 1)
                return;

            set((state) => ({
                passageSheetStack: state.passageSheetStack.slice(0, -1),
            }));
        },
        setBottomPassageLastInStack() {
            set((state) => ({
                passageBottomSheet: state.passageSheetStack.at(-1)
            }))
        },
        clearStack() {
            set((state) => ({
                passageSheetStack: []
            }))
        },

        setPassageCardCache(cacheKey: string, data: VerseCardResponse) {
            set((state) => ({
                passageCardCache: {
                    ...state.passageCardCache,
                    [cacheKey]: data,
                },
            }));
        },

        clearPassageCardCache() {
            set(() => ({
                passageCardCache: {},
            }));
        },

        setSimilarPassagesCache(cacheKey: string, data: Passage[]) {
            set((state) => ({
                similarPassagesCache: {
                    ...state.similarPassagesCache,
                    [cacheKey]: data,
                },
            }));
        },

        clearSimilarPassagesCache() {
            set(() => ({
                similarPassagesCache: {},
            }));
        },

        setViewNotesBottomSheet(up: UserPassage) {
            set(() => ({ viewNotesBottomSheet: up }));
        },

        setViewNotesSheetOpen(o: boolean) {
            set(() => ({ viewNotesSheetOpen: o }));
        },

        setSaveToCollectionBottomSheet(up: UserPassage) {
            set(() => ({ saveToCollectionBottomSheet: up }));
        },

        setSaveToCollectionSheetOpen(o: boolean) {
            set(() => ({ saveToCollectionSheetOpen: o }));
        },

        setCategoriesBottomSheet(category: Category | null) {
            set(() => ({ categoriesBottomSheet: category }));
        },

        setCategoriesSheetOpen(o: boolean) {
            set(() => ({ categoriesSheetOpen: o }));
        },

        clearCategoriesBottomSheet() {
            set(() => ({ categoriesBottomSheet: null }));
        },
        

        setNoteBottomSheet(note: Note, itemId: number | null) {
            set(() => ({
                noteBottomSheet: note,
                noteBottomSheetItemId: itemId
            }));
        },

        setNoteSheetOpen(o) {
            set(() => ({
                noteSheetOpen: o
            }));
        },

        setSyncSheetOpen(o: boolean) {
            set(() => ({
                syncSheetOpen: o
            }));
        },

        clearNoteBottomSheet() {
            set(() => ({
                noteBottomSheet: initialNote,
                noteBottomSheetItemId: null
            }));
        }
    })
)

async function loadPassageCard(
    up: UserPassage,
    set: any,
    get: any
) {
    const cacheKey = getPassageCacheKey(up);
    const existing = get().passageCardCache[cacheKey];
    if (existing) {
        return;
    }

    const userId = useUserStore.getState().user.id;
    const jwt = useUserAuthStore.getState().jwt;

    if (!userId || !jwt || up.passage.verses.length === 0) {
        return;
    }

    const verseIds = up.passage.verses.map((verse) => verse.id).sort((a, b) => a - b);
    const response = await queryClient.fetchQuery({
        queryKey: ['verseCard', userId, cacheKey],
        queryFn: () => getVerseCard(userId, verseIds, jwt),
        staleTime: Infinity,
    });

    set((state: any) => ({
        passageCardCache: {
            ...state.passageCardCache,
            [cacheKey]: response,
        },
    }));
}

async function loadSimilarPassages(
    up: UserPassage,
    set: any,
    get: any
) {
    const cacheKey = getPassageCacheKey(up);
    const existing = get().similarPassagesCache[cacheKey];
    if (existing) {
        return;
    }

    const jwt = useUserAuthStore.getState().jwt;
    if (!jwt || up.passage.verses.length === 0) {
        return;
    }

    const response = await queryClient.fetchQuery({
        queryKey: ['similarPassages', cacheKey],
        queryFn: () => getSimilarVerses(up.passage, jwt),
        staleTime: Infinity,
    });

    set((state: any) => ({
        similarPassagesCache: {
            ...state.similarPassagesCache,
            [cacheKey]: response,
        },
    }));
}