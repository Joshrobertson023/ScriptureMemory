import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import AppShell from "./AppShell";

export const RootStackParamList = {
  
}

export default function App() {
  const queryClient = new QueryClient();

  return (
    <QueryClientProvider client={queryClient}>
      <AppShell />
    </QueryClientProvider>
  )
}