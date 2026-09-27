import React from "react";
import { StyleProp, Text, TextStyle } from "react-native";
import { ChapterSpan } from "../../../types/bible/chapterJson";

type VerseSpanProps = {
    span: ChapterSpan;
    selected: boolean;
    highlightColor?: string;
    onPress: (verseId: string) => void;
    verseNumberStyle: StyleProp<TextStyle>;
    selectedStyle: StyleProp<TextStyle>;
    italicStyle: StyleProp<TextStyle>;
};

const VerseSpan = React.memo(function VerseSpan({
    span, selected, highlightColor, onPress, verseNumberStyle, selectedStyle, italicStyle
}: VerseSpanProps) {
    const handlePress = () => onPress(span.verseId);
    const highlightStyle = highlightColor ? { backgroundColor: highlightColor } : undefined;

    if (span.isVerseStart) {
        return (
            <Text onPress={handlePress} style={[verseNumberStyle, highlightStyle, selected && selectedStyle]}>
                {' '}{span.verseNumber}{' '}
            </Text>
        );
    }

    return (
        <Text onPress={handlePress} style={[span.italic && italicStyle, highlightStyle, selected && selectedStyle]}>
            {span.text}
        </Text>
    );
});

export default VerseSpan;