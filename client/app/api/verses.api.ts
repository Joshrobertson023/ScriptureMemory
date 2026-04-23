import { Passage } from "../../types/passages/passage";
import { VerseCardResponse } from "../../types/verse/verseCard";
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
            const text = await response.text();

            let errorMessage = 'Error searching';

            try {
                const data = JSON.parse(text);
                errorMessage = data?.message || text;
            } catch {
                errorMessage = text;
            }

            throw new Error(errorMessage);
        }
    } catch (error) {
        throw error;
    }
}

export async function getVerseCard(userId: number, verseIds: number[], jwt: string): Promise<VerseCardResponse> {
    try {
        const response = await fetch(`${baseUrl}/verses/verse-card`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${jwt}`
            },
            body: JSON.stringify({ userId, verseIds }),
        });
        if (response.ok) {
            return await response.json();
        } else {
            const text = await response.text();
            let errorMessage = 'Error fetching verse card';
            try {
                const data = JSON.parse(text);
                errorMessage = data?.message || text;
            } catch {
                errorMessage = text;
            }
            throw new Error(errorMessage);
        }
    } catch (error) {
        throw error;
    }
}

export async function getSimilarVerses(passage: Passage, jwt: string): Promise<Passage[]> {
    try {
        console.log('requesting similar for ' + passage.reference.readableReference)
        const response = await fetch(`${baseUrl}/verses/similar`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${jwt}`
            },
            body: JSON.stringify(passage),
        });

        if (!response.ok) {
            const text = await response.text();
            let errorMessage = 'Error fetching similar verses';
            try {
                const data = JSON.parse(text);
                errorMessage = data?.message || text;
            } catch {
                errorMessage = text;
            }
            throw new Error(errorMessage);
        }

        const data = await response.json();
        console.log(data.length);

        return data as Passage[];
    } catch (error) {
        throw error;
    }
}