import { QueryClientProvider } from "@tanstack/react-query";
import { queryClient } from "../app/hooks/queryClient";
import AppShell from "./AppShell";

export const RootStackParamList = {}

export default function App() {
    return (
        <QueryClientProvider client={queryClient}>
            <AppShell />
        </QueryClientProvider>
    );
}