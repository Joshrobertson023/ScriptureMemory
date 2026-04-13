import { useQuery } from "@tanstack/react-query";
import { getVod } from "../api/vod.api";

export function useVod() {
    return useQuery({
        queryKey: ["vod"],
        queryFn: getVod,

        staleTime: 1000 * 60 * 60 * 2,

        refetchOnMount: false,
        refetchOnWindowFocus: false,
    })
}