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
            throw new Error('Login failed');
        }
    } catch (error) {
        throw error;
    }
}