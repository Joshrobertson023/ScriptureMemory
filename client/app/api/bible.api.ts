import { baseUrl } from "./baseUrl";

/**
 * Lists the abbreviations of the Bible translations available to pick as a
 * preferred version (e.g. ["kjv", "niv", "esv"]).
 */
export async function getAvailableBibleVersions(jwt: string): Promise<string[]> {
    try {
        const response = await fetch(`${baseUrl}/bible/translations`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${jwt}`
            },
        });

        if (!response.ok) {
            const text = await response.text();
            let errorMessage = 'Error fetching available Bible versions';
            try {
                const data = JSON.parse(text);
                errorMessage = data?.message || text;
            } catch {
                errorMessage = text;
            }
            throw new Error(errorMessage);
        }

        return await response.json() as string[];
    } catch (error) {
        throw error;
    }
}
