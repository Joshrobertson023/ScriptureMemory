import { Passage } from "../../types/passages/passage";
import { useUserStore } from "../stores/user.store";
import { baseUrl } from "./baseUrl";

export async function addPassageSearch(query: string): Promise<Passage> {
    const userId = useUserStore().user.id;
    const searchType = 2;

    try {
        const response = await fetch(`${baseUrl}/users/login/token`, {
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
            throw new Error('Login failed');
        }
    } catch (error) {
        throw error;
    }
}