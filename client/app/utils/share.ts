import { Share } from "react-native";
import { Passage } from "../../types/passages/passage";
import { getVerseNumbers } from "./referenceUtils";

export function sharePassage(passage: Passage) {
    const verses = [...passage.verses].sort(
        (a, b) => (getVerseNumbers(a.reference)[0] ?? 0) - (getVerseNumbers(b.reference)[0] ?? 0)
    );
    const text = verses.map((verse) => verse.translationContents?.at(0)?.plainText ?? verse.text).join(' ');
    const version = verses.at(0)?.translationContents?.at(0)?.version?.toUpperCase();
    const reference = version
        ? `${passage.reference.readableReference} (${version})`
        : passage.reference.readableReference;

    return Share.share({ message: `${text}\n\n${reference}` });
}
