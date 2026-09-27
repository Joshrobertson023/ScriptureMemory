import { create } from "zustand";
import { Collection } from "../../types/collection/collection";
import { Note } from "../../types/note";
import { UserPassage } from "../../types/passages/userPassage";
import { getBookName, getVerseNumbers } from "../utils/referenceUtils";
import { initialUserPassage } from "./collections.store";

export const getPassageCacheKey = (up: UserPassage) => {
    const { reference } = up.passage;
    return `${up.id}-${getBookName(reference)}:${reference.chapter}:${getVerseNumbers(reference).join(',')}`;
};

interface BottomSheetsStore {
    passageSheetStack: UserPassage[];
    passageBottomSheet: UserPassage;
    passageSheetOpen: boolean;
    passageSheetPendingTransition: { kind: "next"; passage: UserPassage } | { kind: "last" } | null;

    viewNotesBottomSheet: UserPassage;
    viewNotesSheetOpen: boolean;
    saveToCollectionBottomSheet: UserPassage;
    saveToCollectionSheetOpen: boolean;

    noteBottomSheet: Note;
    noteBottomSheetItemId: string | null;
    noteSheetOpen: boolean;
    syncSheetOpen: boolean;

    collectionMenuBottomSheet: Collection | null;
    collectionMenuSheetOpen: boolean;

    passageMenuBottomSheet: { userPassage: UserPassage; itemId: string; collectionId: string } | null;
    passageMenuSheetOpen: boolean;

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

    setViewNotesBottomSheet: (up: UserPassage) => void;
    setViewNotesSheetOpen: (o: boolean) => void;
    setSaveToCollectionBottomSheet: (up: UserPassage) => void;
    setSaveToCollectionSheetOpen: (o: boolean) => void;

    setNoteBottomSheet: (note: Note, itemId: string | null) => void;
    setNoteSheetOpen: (o: boolean) => void;
    setSyncSheetOpen: (o: boolean) => void;
    clearNoteBottomSheet: () => void;

    setCollectionMenuBottomSheet: (collection: Collection | null) => void;
    setCollectionMenuSheetOpen: (o: boolean) => void;

    setPassageMenuBottomSheet: (item: { userPassage: UserPassage; itemId: string; collectionId: string } | null) => void;
    setPassageMenuSheetOpen: (o: boolean) => void;
}

const initialNote: Note = {
    id: '',
    text: ""
};

export const useBottomSheetsStore = create<BottomSheetsStore>()(
    (set, get) => ({
        passageSheetStack: [],
        passageBottomSheet: initialUserPassage,
        passageSheetOpen: false,
        passageSheetPendingTransition: null,

        viewNotesBottomSheet: initialUserPassage,
        viewNotesSheetOpen: false,
        saveToCollectionBottomSheet: initialUserPassage,
        saveToCollectionSheetOpen: false,

        noteBottomSheet: initialNote,
        noteBottomSheetItemId: null,
        noteSheetOpen: false,
        syncSheetOpen: false,

        collectionMenuBottomSheet: null,
        collectionMenuSheetOpen: false,

        passageMenuBottomSheet: null,
        passageMenuSheetOpen: false,

        setPassageBottomSheet(up: UserPassage) {
            set(() => ({ passageBottomSheet: up }));
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

        setNoteBottomSheet(note: Note, itemId: string | null) {
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
        },

        setCollectionMenuBottomSheet(collection: Collection | null) {
            set(() => ({ collectionMenuBottomSheet: collection }));
        },

        setCollectionMenuSheetOpen(o: boolean) {
            set(() => ({ collectionMenuSheetOpen: o }));
        },

        setPassageMenuBottomSheet(item) {
            set(() => ({ passageMenuBottomSheet: item }));
        },

        setPassageMenuSheetOpen(o: boolean) {
            set(() => ({ passageMenuSheetOpen: o }));
        }
    })
)