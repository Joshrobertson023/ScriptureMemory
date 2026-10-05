import AsyncStorage from "@react-native-async-storage/async-storage";
import { Appearance, type ColorSchemeName } from "react-native";
import { Uniwind } from "uniwind";
import { create } from "zustand";
import { createJSONStorage, persist } from 'zustand/middleware';
import { BibleVersion, CollectionsSort, ThemePreference } from "../../types/enums";

function isDarkScheme(scheme: ColorSchemeName | null | undefined): boolean {
    return scheme === "dark";
}

function resolvedDarkFor(theme: ThemePreference): boolean {
    if (theme === 0) return isDarkScheme(Appearance.getColorScheme());
    return theme === 2;
}

function uniwindThemeFor(theme: ThemePreference): "system" | "light" | "dark" {
    if (theme === 0) return "system";
    if (theme === 2) return "dark";
    return "light";
}

export interface Session {
    id?: number;
    deviceId?: string;
    deviceName: string;
    model: string;
    refreshTokenHash?: string;
    pushNotificationToken?: string;
    createdAt?: Date;
    lastSeenAt?: Date;
}

interface User {
    id: string;
    username?: string;
    firstName?: string;
    lastName?: string;
    email?: string;
    hashedPassword?: string;
    dateRegistered?: Date;
    points: number;
    memorizedCount: number;
    profileDescription?: string;
    preferences: UserPreferences;
}

interface UserPreferences {
    theme: ThemePreference;
    bibleVersion: BibleVersion;
    collectionsSort: CollectionsSort;
    typeOutReference: boolean;
}

interface UserStore {
    user: User;
    userId: string;
    resolvedDark: boolean;
    setUserId: (id: string) => void;

    setUser: (user: User) => void;
    setThemePreference: (theme: ThemePreference) => void;
    logout: () => void;
}

const initialPreferences: UserPreferences = {
    theme: 0,
    bibleVersion: 0,
    collectionsSort: 0,
    typeOutReference: true
}

const initialUser: User = {
    id: '',
    points: 0,
    memorizedCount: 0,
    preferences: initialPreferences
}

export const useUserStore = create<UserStore>()(
    persist(
        (set, get) => ({
            user: initialUser,
            userId: '',
            resolvedDark: resolvedDarkFor(0),
            
            setUser(u: User) {
                const theme = u.preferences?.theme;
                if (theme === 0 || theme === 1 || theme === 2) {
                    set({ user: u, resolvedDark: resolvedDarkFor(theme) });
                    Uniwind.setTheme(uniwindThemeFor(theme));
                    return;
                }
                set({ user: u });
            },

            setThemePreference(theme: ThemePreference) {
                const user = get().user;
                set({
                    user: { ...user, preferences: { ...user.preferences, theme } },
                    resolvedDark: resolvedDarkFor(theme),
                });
                Uniwind.setTheme(uniwindThemeFor(theme));
            },

            logout() {
                set({ user: initialUser, resolvedDark: resolvedDarkFor(0) });
                Uniwind.setTheme("system");
            },

            setUserId(id: string) {
                set({userId: id})
            }
        }),
        {
            name: 'user-storage',
            storage: createJSONStorage(() => AsyncStorage),
            partialize: (state) => ({
                user: state.user,
                userId: state.userId,
            }),
            merge: (persistedState, currentState) => {
                const persisted = (persistedState ?? {}) as Partial<Pick<UserStore, "user" | "userId">>;
                const user = persisted.user ?? currentState.user;
                const theme = user.preferences?.theme ?? 0;
                return {
                    ...currentState,
                    ...persisted,
                    user,
                    userId: persisted.userId ?? currentState.userId,
                    resolvedDark: resolvedDarkFor(theme),
                };
            },
            onRehydrateStorage: () => (state) => {
                if (!state) return;
                Uniwind.setTheme(uniwindThemeFor(state.user.preferences.theme ?? 0));
            },
        }
    )
);

Appearance.addChangeListener(({ colorScheme }) => {
    const state = useUserStore.getState();
    if (state.user.preferences.theme !== 0) return;
    const next = isDarkScheme(colorScheme);
    if (next === state.resolvedDark) return;
    useUserStore.setState({ resolvedDark: next });
});