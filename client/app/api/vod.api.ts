import { Passage } from "../../types/passages/passage";
import { baseUrl } from "./baseUrl";

export async function getVerseOfDay(translation: string): Promise<Passage> {
    try {
        const response = await fetch(`${baseUrl}/verseofday/${translation}`, {
            method: 'GET'
        });
        if (response.ok) {
            const data = await response.json();
            return data;
        } else {
            const responseText = await response.text();
            throw new Error(responseText || 'Failed to fetch verse of day');
        }
    } catch (error) {
        throw error;
    }
}
