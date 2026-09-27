import React from "react";
import { StyleProp, Text, TextStyle, View, ViewStyle } from "react-native";
import { ChapterParagraph, ChapterVerseJson } from "../../../types/bible/chapterJson";
import { getHighlightColor, HighlightColorId } from "../../styles/highlightColors";
import VerseSpan from "./verseSpan";


type ParagraphProps = {
    paragraph: ChapterParagraph;
    paragraphIndex: number;
    versesById: Record<string, ChapterVerseJson>;
    selectedVerseNumbers: Set<number>;
    highlightColorByVerseId: Map<string, HighlightColorId>;
    onSpanPress: (verseId: string) => void;
    paragraphStyle: StyleProp<TextStyle>;
    poetryLineStyle: StyleProp<TextStyle>;
    poetryLineIndentStyle: StyleProp<TextStyle>;
    blankLineStyle: StyleProp<ViewStyle>;
    verseNumberStyle: StyleProp<TextStyle>;
    selectedStyle: StyleProp<TextStyle>;
    italicStyle: StyleProp<TextStyle>;
};

const Paragraph = React.memo(function Paragraph({
    paragraph, paragraphIndex, versesById, selectedVerseNumbers, highlightColorByVerseId,
    onSpanPress,
    paragraphStyle, poetryLineStyle, poetryLineIndentStyle, blankLineStyle,
    verseNumberStyle, selectedStyle, italicStyle,
}: ParagraphProps) {
    if (paragraph.spans.length === 0) {
        return <View style={blankLineStyle} />;
    }

    const isPoetry = paragraph.style === 'q1' || paragraph.style === 'q2';

    return (
        <Text style={[paragraphStyle, isPoetry && poetryLineStyle, paragraph.style === 'q2' && poetryLineIndentStyle]}>
            {paragraph.spans.map((span, spanIndex) => {
                const verse = versesById[span.verseId];
                const selected = verse ? selectedVerseNumbers.has(verse.verseNumber) : false;
                const highlightColorId = highlightColorByVerseId.get(span.verseId);
                const highlightColor = verse ? highlightColorId ? getHighlightColor(highlightColorId).hex : undefined : undefined;

                return (
                    <VerseSpan
                        key={`span-${paragraphIndex}-${spanIndex}`}
                        span={span}
                        selected={selected}
                        highlightColor={highlightColor}
                        onPress={onSpanPress}
                        verseNumberStyle={verseNumberStyle}
                        selectedStyle={selectedStyle}
                        italicStyle={italicStyle}
                    />
                );
            })}
        </Text>
    );
}, (prev, next) => {
    if (prev.paragraph !== next.paragraph) return false;
    if (prev.paragraphIndex !== next.paragraphIndex) return false;
    if (prev.versesById !== next.versesById) return false;
    if (prev.onSpanPress !== next.onSpanPress) return false;
    if (prev.paragraphStyle !== next.paragraphStyle) return false;
    if (prev.poetryLineStyle !== next.poetryLineStyle) return false;
    if (prev.poetryLineIndentStyle !== next.poetryLineIndentStyle) return false;
    if (prev.blankLineStyle !== next.blankLineStyle) return false;
    if (prev.verseNumberStyle !== next.verseNumberStyle) return false;
    if (prev.selectedStyle !== next.selectedStyle) return false;
    if (prev.italicStyle !== next.italicStyle) return false;

    for (const span of prev.paragraph.spans) {
        const verse = next.versesById[span.verseId];
        const prevSelected = verse ? prev.selectedVerseNumbers.has(verse.verseNumber) : false;
        const nextSelected = verse ? next.selectedVerseNumbers.has(verse.verseNumber) : false;
        if (prevSelected !== nextSelected) return false;
        if (prev.highlightColorByVerseId.get(span.verseId) !== next.highlightColorByVerseId.get(span.verseId)) return false;
    }

    return true;
});

export default Paragraph;
