import { create } from "zustand";
import { UserPassage } from "../../types/passages/userPassage";
import { initialUserPassage } from "./collections.store";
import { Note } from "../../types/note";
import { VerseCardResponse } from "../../types/verse/verseCard";
import { getVerseCard } from "../api/verses.api";
import { useUserAuthStore } from "./userAuth.store";
import { useUserStore } from "./user.store";
import { queryClient } from "../hooks/queryClient";

export const getPassageCacheKey = (up: UserPassage) => {
    const { book, chapter, verses } = up.passage.reference;
    return `${book}:${chapter}:${verses.join(',')}`;
};

interface BottomSheetsStore {
    passageBottomSheet: UserPassage;
    passageSheetOpen: boolean;
    passageBottomSheet2: UserPassage;
    passageSheet2Open: boolean;
    passageCardCache: Record<string, VerseCardResponse>;

    noteBottomSheet: Note;
    noteBottomSheetItemId: number | null;
    noteSheetOpen: boolean;
    syncSheetOpen: boolean;

    setPassageBottomSheet: (up: UserPassage) => void;
    setPassageSheetOpen: (o: boolean) => void;
    setPassageBottomSheet2: (up: UserPassage) => void;
    setPassageSheet2Open: (o: boolean) => void;
    setPassageCardCache: (cacheKey: string, data: VerseCardResponse) => void;
    clearPassageCardCache: () => void;

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
        passageBottomSheet: initialUserPassage,
        passageSheetOpen: false,
        passageBottomSheet2: initialUserPassage,
        passageSheet2Open: false,
        passageCardCache: {},

        noteBottomSheet: initialNote,
        noteBottomSheetItemId: null,
        noteSheetOpen: false,
        syncSheetOpen: false,

        setPassageBottomSheet(up: UserPassage) {
            set(() => ({ passageBottomSheet: up }));
            void loadPassageCard(up, set, get);
        },
        setPassageSheetOpen(o: boolean) {
            set(() => ({ passageSheetOpen: o }));
        },
        setPassageBottomSheet2(up: UserPassage) {
            set(() => ({ passageBottomSheet2: up }));
            void loadPassageCard(up, set, get);
        },
        setPassageSheet2Open(o: boolean) {
            set(() => ({ passageSheet2Open: o }));
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