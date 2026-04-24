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

    return {
        getVerseTypingPart
    }
}

export default useReferenceParser;