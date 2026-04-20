import { create } from "zustand";
import { UserPassage } from "../../types/passages/userPassage";
import { initialUserPassage } from "./collections.store";
import { Note } from "../../types/note";

interface BottomSheetsStore {
    passageBottomSheet: UserPassage;
    passageSheetOpen: boolean;
    noteBottomSheet: Note;
    noteBottomSheetItemId: number | null;
    noteSheetOpen: boolean;
    syncSheetOpen: boolean;
    setPassageBottomSheet: (up: UserPassage) => void;
    setPassageSheetOpen: (o: boolean) => void;
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
        noteBottomSheet: initialNote,
        noteBottomSheetItemId: null,
        noteSheetOpen: false,
        syncSheetOpen: false,

        setPassageBottomSheet(up: UserPassage) {
            set((state) => ({
                passageBottomSheet: up
            }))
        },

        setPassageSheetOpen(o: boolean) {
            set((state) => ({
                passageSheetOpen: o
            }))
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