import AsyncStorage from "@react-native-async-storage/async-storage";
import { create } from "zustand";
import { persist, createJSONStorage } from 'zustand/middleware';
import { ReaderBackgroundId, ReaderFontId } from "../styles/readerFonts";

interface ReaderSettingsStore {
    fontId: ReaderFontId;
    fontSize: number;
    margin: number;
    background: ReaderBackgroundId;
    translationOverride: string | null;

    setFontId: (fontId: ReaderFontId) => void;
    setFontSize: (fontSize: number) => void;
    setMargin: (margin: number) => void;
    setBackground: (background: ReaderBackgroundId) => void;
    setTranslationOverride: (translationOverride: string | null) => void;
    reset: () => void;
}

const DEFAULT_SETTINGS = {
    fontId: 'notoSerif' as ReaderFontId,
    fontSize: 19,
    margin: 25,
    background: 'default' as ReaderBackgroundId,
    translationOverride: null as string | null,
};

export const useReaderSettingsStore = create<ReaderSettingsStore>()(
    persist(
        (set) => ({
            ...DEFAULT_SETTINGS,

            setFontId(fontId: ReaderFontId) {
                set({ fontId });
            },
            setFontSize(fontSize: number) {
                set({ fontSize });
            },
            setMargin(margin: number) {
                set({ margin });
            },
            setBackground(background: ReaderBackgroundId) {
                set({ background });
            },
            setTranslationOverride(translationOverride: string | null) {
                set({ translationOverride });
            },
            reset() {
                set({ ...DEFAULT_SETTINGS });
            },
        }),
        {
            name: 'reader-settings-storage',
            storage: createJSONStorage(() => AsyncStorage),
        }
    )
);
