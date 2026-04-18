import { Passage } from "../../types/passages/passage";
import { useUserStore } from "../stores/user.store";
import { useUserAuthStore } from "../stores/userAuth.store";
import { baseUrl } from "./baseUrl";

interface SearchResult {
    passage: Passage;
}

export async function searchPassage(search: string, userId: number, jwt: string): Promise<Passage[]> {
    const searchType = 2;
    console.log(userId + " " + jwt)
    console.log("\n\n" + search)

    try {
        const response = await fetch(`${baseUrl}/search`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${jwt}`
            },
            body: JSON.stringify({
                userId,
                search,
                searchType
            }),
        });
        if (response.ok) {
            const data: SearchResult[] = await response.json();
            return data.map(r => r.passage);
        } else {
            const text = await response.text(); // 👈 read once

            let errorMessage = 'Error searching';

            try {
                const data = JSON.parse(text);  // 👈 manually parse
                errorMessage = data?.message || text;
            } catch {
                errorMessage = text; // 👈 plain text fallback
            }

            throw new Error(errorMessage);
        }
    } catch (error) {
        throw error;
    }
}