import AsyncStorage from "@react-native-async-storage/async-storage";
import { create } from "zustand";

interface UserAuthStore {
    authToken: string;
    isAuthenticated: boolean;

    setToken: (t: string) => void;
    logout: () => void;
    loadToken: () => void;
}

export const useUserAuthStore = create<UserAuthStore>((set, get) => ({
    authToken: '',
    isAuthenticated: false,

    setToken: async (t: string) => {
        await AsyncStorage.setItem("auth_token", t);
        set({ authToken: t, isAuthenticated: true });
    },
    logout: async () => {
        await AsyncStorage.removeItem("auth_token");
        set({ authToken: '', isAuthenticated: false});
    },
    loadToken: async () => {
        const token = await AsyncStorage.getItem("auth_token");
        if (token) {
            set({authToken: token, isAuthenticated: true})
        }
    }
}))