import {
    QueryClient,
    QueryClientProvider
} from '@tanstack/react-query';
import { HeroUINativeProvider } from 'heroui-native';
import React from "react";
import { GestureHandlerRootView } from 'react-native-gesture-handler';
import { SafeAreaProvider } from 'react-native-safe-area-context';
import '../global.css';
import AppShell from "./AppShell";
import GlobalToastBridge from "./components/GlobalToastBridge";

export const RootStackParamList = {}

export const queryClient = new QueryClient();

export default function App() {
    return (
        <GestureHandlerRootView style={{ flex: 1 }}>
            <SafeAreaProvider>
                <HeroUINativeProvider>
                    <GlobalToastBridge />
                    <QueryClientProvider client={queryClient}>
                        <AppShell />
                    </QueryClientProvider>
                </HeroUINativeProvider>
            </SafeAreaProvider>
        </GestureHandlerRootView>
    );
}