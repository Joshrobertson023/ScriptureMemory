import { create } from "zustand";
import { UserPassage } from "../../types/passages/userPassage";
import { initialUserPassage } from "./collections.store";

interface BottomSheetsStore {
    passageBottomSheet: UserPassage;
    passageSheetOpen: boolean;
    setPassageBottomSheet: (up: UserPassage) => void;
    setPassageSheetOpen: (o: boolean) => void;
}

export const useBottomSheetsStore = create<BottomSheetsStore>()(
    (set, get) => ({
        passageBottomSheet: initialUserPassage,
        passageSheetOpen: false,

        setPassageBottomSheet(up: UserPassage) {
            set((state) => ({
                passageBottomSheet: up
            }))
        },

        setPassageSheetOpen(o) {
            set((state) => ({
                passageSheetOpen: o
            }))
        },
    })
)