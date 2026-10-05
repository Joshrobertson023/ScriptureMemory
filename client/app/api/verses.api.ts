import { ResponseChapterJson } from "../../types/bible/chapterJson";
import { ChapterResponse } from "../../types/ChapterResponse";
import { Passage } from "../../types/passages/passage";
import { VerseCardResponse } from "../../types/verse/verseCard";
import { allBooks } from "../hooks/useBooks";
import { getBookName, getVerseNumbers } from "../utils/referenceUtils";
import { baseUrl } from "./baseUrl";

interface SearchResult {
    passage: Passage;
}

export async function searchPassage(search: string, translation: string, lastVerseDistance: number, jwt: string): Promise<Passage[]> {
    try {
        const response = await fetch(`${baseUrl}/search`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${jwt}`
            },
            body: JSON.stringify({
                search,
                translation,
                lastVerseDistance
            }),
        });
        if (response.ok) {
            const data: SearchResult[] = await response.json();
            const passages = data.map(r => r.passage);
            return passages;
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

export async function getVerseCard(
    userId: number,
    verseIds: string[],
    translation: string,
    fetchVerseText: boolean,
    jwt: string
): Promise<VerseCardResponse> {
    try {
        const response = await fetch(`${baseUrl}/passage-card`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${jwt}`
            },
            body: JSON.stringify({ userId, verseIds, translation, fetchVerseText }),
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
        console.error('Error fetching verse card:', error);
        throw error;
    }
}

export async function getSimilarPassages(passage: Passage, translation: string, lastVerseDistance: number | null, jwt: string): Promise<Passage[]> {
    const displayName = getBookName(passage.reference);
    const bookInfo = typeof passage.reference.book === 'string' ? undefined : passage.reference.book;
    const reference = {
        book: {
            displayName,
            abbreviation: bookInfo?.abbreviation || passage.verses.at(0)?.id.split('.').at(0) || '',
            numChapters: bookInfo?.numChapters || allBooks.find((b) => b.displayName === displayName)?.numChapters || 0,
        },
        chapter: passage.reference.chapter,
        verseNumbers: getVerseNumbers(passage.reference),
        readableReference: passage.reference.readableReference,
    };
    try {
        const response = await fetch(`${baseUrl}/similar`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${jwt}`
            },
            body: JSON.stringify({ reference, translation, lastVerseDistance }),
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

        const data: (Passage | SearchResult)[] = await response.json();
        return data.map((r) => 'passage' in r ? r.passage : r);
    } catch (error) {
        console.error('Error fetching similar passages:', error);
        throw error;
    }
}

export async function getChapterResponse(book: string, chapter: number, bible: string, jwt: string): Promise<ChapterResponse> {
    try {
        const response = await fetch(`${baseUrl}/bible/chapter/${bible.toLowerCase()}/${book}/${chapter}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${jwt}`
            },
        });

        if (!response.ok) {
            const text = await response.text();
            let errorMessage = 'Error fetching chapter verses';
            try {
                const data = JSON.parse(text);
                errorMessage = data?.message || data?.title || text;
            } catch {
                errorMessage = text;
            }
            throw new Error(errorMessage);
        }

        const data = await response.json();
        console.log(data.length);

        return data as ChapterResponse;
    } catch (error) {
        throw error;
    }
}

export async function getChapterJson(
    bible: string,
    book: string,
    chapter: number,
    jwt: string
): Promise<ResponseChapterJson> {
    try {
        const response = await fetch(
            `${baseUrl}/bible/chapter/${encodeURIComponent(bible.toLowerCase())}/${encodeURIComponent(book)}/${chapter}?contentType=json`,
            {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${jwt}`
                },
            }
        );

        if (!response.ok) {
            const text = await response.text();
            let errorMessage = 'Error fetching chapter';
            try {
                const data = JSON.parse(text);
                errorMessage = data?.message || data?.title || text;
            } catch {
                errorMessage = text;
            }
            throw new Error(errorMessage);
        }

        return await response.json() as ResponseChapterJson;
    } catch (error) {
        throw error;
    }
}