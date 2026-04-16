import { Passage } from "../../types/passages/passage";
import { useUserStore } from "../stores/user.store";
import { baseUrl } from "./baseUrl";

export async function searchPassage(query: string): Promise<Passage[]> {
    const userId = useUserStore().user.id;
    const searchType = 2;

    const response = await fetch(`${baseUrl}/search`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            userId,
            query,
            searchType
        }),
    });
    if (response.ok) {
        return await response.json();
    } else {
        throw Error(await response.text())
    }
}