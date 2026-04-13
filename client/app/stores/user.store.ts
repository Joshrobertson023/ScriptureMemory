import AsyncStorage from "@react-native-async-storage/async-storage";
import { create } from "zustand";
import { persist, createJSONStorage } from 'zustand/middleware';
import { BibleVersion, CollectionsSort, ThemePreference } from "../../types/enums";

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
    id: number;
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

    setUser: (user: User) => void;
    logout: () => void;
}

const initialPreferences: UserPreferences = {
    theme: 0,
    bibleVersion: 0,
    collectionsSort: 0,
    typeOutReference: true
}

const initialUser: User = {
    id: 0,
    points: 0,
    memorizedCount: 0,
    preferences: initialPreferences
}

export const useUserStore = create<UserStore>()(
    persist(
        (set, get) => ({
            user: initialUser,
            
            setUser(u: User) {
                set({user: u})
            },

            logout() {
                set({user: initialUser})
            }
        }),
        {
            name: 'user-storage',
            storage: createJSONStorage(() => AsyncStorage)
        }
    )
)