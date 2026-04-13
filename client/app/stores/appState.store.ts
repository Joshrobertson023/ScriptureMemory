//     import { create } from 'zustand';
// import { BibleVersion, Status, ThemePreference } from '../../types/enums';
// import { ThemeProvider } from '@react-navigation/native';

// export interface ErrorMessage {
//     type: string;
//     message: string;
// }

// interface AppState {
//   editingCollection: Collection | undefined;
//   publishingCollection: Collection | undefined;
//   editingUserVerse: UserVerse | undefined;
//   selectedUserVerse: UserVerse | undefined;
//   themePreference: ThemePreference;
  
//   setEditingCollection: (collection: Collection | undefined) => void;
//   setPublishingCollection: (collection: Collection | undefined) => void;
//   setEditingUserVerse: (userVerse: UserVerse | undefined) => void;
//   setSelectedUserVerse: (userVerse: UserVerse | undefined) => void;
//   setThemePreference: (preference: ThemePreference) => void;
// }

// export const useAppStore = create<AppState>((set) => ({
//     editingCollection: undefined,
//     publishingCollection: undefined,
//     editingUserVerse: undefined,
//     selectedUserVerse: undefined,
//     themePreference: 0,

//     setEditingCollection: (collection: Collection | undefined) => set({editingCollection: collection ? cloneCollection(collection) : undefined}),
//     setPublishingCollection: (collection: Collection | undefined) => set({publishingCollection: collection ? cloneCollection(collection) : undefined}),
//     setEditingUserVerse: (userVerse: UserVerse | undefined) => set({editingUserVerse: userVerse}),
//     setSelectedUserVerse: (userVerse: UserVerse | undefined) => set({selectedUserVerse: userVerse}),
//     setThemePreference: (preference: ThemePreference) => set({ themePreference: preference }),
// }))