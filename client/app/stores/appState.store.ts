    import { create } from 'zustand';
import { BibleVersion, Status, ThemePreference } from '../../types/enums';
import { ThemeProvider } from '@react-navigation/native';
import { Collection } from '../../types/collection/collection';

export interface ErrorMessage {
    type: string;
    message: string;
}

const initialErrorMessage: ErrorMessage = {
    type: 'ERROR',
    message: ''
}

interface AppState {
  editingCollection: Collection | undefined;
  publishingCollection: Collection | undefined;
  themePreference: ThemePreference;
  errorMessage: ErrorMessage;
  syncStatus: 'Syncing' | 'Error' | 'Synced';
  
  setEditingCollection: (collection: Collection | undefined) => void;
  setPublishingCollection: (collection: Collection | undefined) => void;
  setThemePreference: (preference: ThemePreference) => void;

  displayErrorMessage: (msg: ErrorMessage) => void;

  setSyncStatus: (s: 'Syncing' | 'Error' | 'Synced') => void;
}

export const useAppStore = create<AppState>((set) => ({
    editingCollection: undefined,
    publishingCollection: undefined,
    editingUserVerse: undefined,
    selectedUserVerse: undefined,
    themePreference: 0,
    errorMessage: initialErrorMessage,
    syncStatus: 'Error',

    setEditingCollection: (collection: Collection | undefined) => set({editingCollection: collection ? collection : undefined}),
    setPublishingCollection: (collection: Collection | undefined) => set({publishingCollection: collection ? collection : undefined}),
    setThemePreference: (preference: ThemePreference) => set({ themePreference: preference }),

    displayErrorMessage: (msg: ErrorMessage) => {
        set({errorMessage: msg })
    },

    setSyncStatus: (s: 'Syncing' | 'Error' | 'Synced') => {
        set({syncStatus: s})
    }
}))