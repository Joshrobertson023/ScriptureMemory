const useReferenceParser = () => {
    const getVerseTypingPart = (reference: string): string[] => {
        const parts: string[] = [];

        const versesPart = reference.split(':')[1];

        for (const segment of versesPart.split(',')) {
            const trimmed = segment.trim();

            if (trimmed.includes('-')) {
                const range = trimmed.split('-');
                parts.push(range[0].trim());
                parts.push(range[1].trim());
            } else {
                parts.push(trimmed);
            }
        }

        return parts;
    }

    const convertToReadableReference = (book: string, chapter: number, verses: number[]): string => {
        if (!verses || verses.length === 0) return '';

        const sorted = [...verses].sort((a, b) => a - b);
        let result = `${book} ${chapter}:`;

        let i = 0;
        while (i < sorted.length) {
            if (i > 0) result += ', ';

            let rangeStart = sorted[i];
            let rangeEnd = rangeStart;

            while (i + 1 < sorted.length && sorted[i + 1] === sorted[i] + 1) {
                i++;
                rangeEnd = sorted[i];
            }

            result += rangeStart;
            if (rangeEnd > rangeStart) result += `-${rangeEnd}`;

            i++;
        }

        return result;
    }

    return {
        getVerseTypingPart,
        convertToReadableReference
    }
}

export default useReferenceParser;