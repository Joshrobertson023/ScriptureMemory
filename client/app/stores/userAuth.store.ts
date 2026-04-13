import AsyncStorage from "@react-native-async-storage/async-storage";
import { create } from "zustand";
import { persist, createJSONStorage } from 'zustand/middleware';
import { Session } from "./user.store";

interface UserAuthStore {
    jwt: string;
    refreshToken: string;
    session: Session;

    setSession: (s: Session) => void;
    setJwt: (t: string) => void;
    setRefreshToken: (t: string) => void;
    logout: () => void;
}

const initialSession: Session = {
    deviceName: '',
    deviceId: '',
    id: 0,
    model: ''
}

export const useUserAuthStore = create<UserAuthStore>()(
    persist(
        (set) => ({
            jwt: '',
            refreshToken: '',
            session: initialSession,

            setSession(s: Session) {
                set({session: s})
            },
            setJwt(t: string) {
                set({ jwt: t })
            },
            setRefreshToken(t: string) {
                set({ refreshToken: t })
            },

            logout() {
                set({ 
                    jwt: '', 
                    refreshToken: ''
                })
            }
        }),
        {
            name: 'auth-storage',
            storage: createJSONStorage(() => AsyncStorage)
        }
    )
)