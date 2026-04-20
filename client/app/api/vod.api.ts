import { Vod } from "../../types/verse/vod";
import { baseUrl } from "./baseUrl";

export async function getVod(): Promise<Vod> {
    try {
        const response = await fetch(`${baseUrl}/verseofday`, {
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