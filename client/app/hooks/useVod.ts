import { useQuery } from "@tanstack/react-query";
import { getVerseOfDay } from "../api/vod.api";

export function useVod(translation: string) {
    return useQuery({
        queryKey: ["vod", translation],
        queryFn: () => getVerseOfDay(translation),

        staleTime: 1000 * 60 * 60 * 2,

        refetchOnMount: false,
        refetchOnWindowFocus: false,
    })
}
