import { BookmarkPlus, BookOpenText, Brain, Highlighter, NotebookText, Share2 } from "lucide-react-native";
import React, { useCallback, useMemo, useState } from "react";
import { ScrollView, Text, TouchableOpacity } from "react-native";
import { UserPassage } from "../../../types/passages/userPassage";
import { useBottomSheetStack } from "../../hooks/useBottomSheetStack";
import { useChapterHighlights } from "../../hooks/useChapterHighlights";
import { pushPracticeSessionRoute, pushReadRoute } from "../../navigation";
import { useBottomSheetsStore } from "../../stores/bottomSheets.store";
import useGlobalStyles from "../../styles/gobalStyles";
import { HighlightColorId } from "../../styles/highlightColors";
import useAppTheme from "../../theme";
import { getBookName, getVerseNumbers } from "../../utils/referenceUtils";
import { sharePassage } from "../../utils/share";
import HighlightColorDialog from "../dialogs/highlightColorDialog";

interface PassageSheetActionsProps {
    passageBottomSheet: UserPassage;
}

const PassageSheetActions = React.memo(({ passageBottomSheet }: PassageSheetActionsProps) => {
    const globalStyles = useGlobalStyles();
    const theme = useAppTheme();
    const setSaveToCollectionBottomSheet = useBottomSheetsStore((state) => state.setSaveToCollectionBottomSheet);
    const setSaveToCollectionSheetOpen = useBottomSheetsStore((state) => state.setSaveToCollectionSheetOpen);
    const setViewNotesBottomSheet = useBottomSheetsStore((state) => state.setViewNotesBottomSheet);
    const setViewNotesSheetOpen = useBottomSheetsStore((state) => state.setViewNotesSheetOpen);
    const { closePassages } = useBottomSheetStack();
    const [isHighlightColorDialogOpen, setIsHighlightColorDialogOpen] = useState(false);

    const handleRead = () => {
        const { reference } = passageBottomSheet.passage;
        closePassages();
        pushReadRoute(getBookName(reference), reference.chapter, getVerseNumbers(reference));
    };

    const handlePractice = () => {
        closePassages();
        pushPracticeSessionRoute(passageBottomSheet);
    };

    const verseIds = useMemo(
        () => passageBottomSheet.passage.verses.map((verse) => verse.id),
        [passageBottomSheet]
    );
    const { highlightColorByVerseId, toggleHighlights } = useChapterHighlights(verseIds);

    const selectedHighlightColors = useMemo(
        () => new Set(verseIds.map((id) => highlightColorByVerseId.get(id))),
        [verseIds, highlightColorByVerseId]
    );

    const handleSelectHighlightColor = useCallback((color: HighlightColorId) => {
        toggleHighlights(verseIds, color);
        setIsHighlightColorDialogOpen(false);
    }, [verseIds, toggleHighlights]);

    return (
        <>
        <ScrollView
            horizontal
            nestedScrollEnabled
            showsHorizontalScrollIndicator={false}
            contentContainerStyle={{ display: 'flex', flexDirection: 'row', justifyContent: 'center', alignItems: 'center', gap: 5, flexGrow: 1 }}
        >
            <TouchableOpacity
                style={globalStyles.elevationButtomSquare}
                onPress={() => {
                    setSaveToCollectionBottomSheet(passageBottomSheet);
                    setSaveToCollectionSheetOpen(true);
                }}
            >
                <BookmarkPlus size={20} color={theme.colors.onBackground} strokeWidth={1.3} />
                <Text style={{ ...globalStyles.p4, fontWeight: 600 }}>
                    Save
                </Text>
            </TouchableOpacity>
            <TouchableOpacity
                style={globalStyles.elevationButtomSquare}
                onPress={() => setIsHighlightColorDialogOpen(true)}
            >
                <Highlighter size={20} color={theme.colors.onBackground} strokeWidth={1.3} />
                <Text style={{ ...globalStyles.p4, fontWeight: 600 }}>
                    Highlight
                </Text>
            </TouchableOpacity>
            <TouchableOpacity
                style={globalStyles.elevationButtomSquare}
                onPress={() => {
                    setViewNotesBottomSheet(passageBottomSheet);
                    setViewNotesSheetOpen(true);
                }}
            >
                <NotebookText size={20} color={theme.colors.onBackground} strokeWidth={1.3} />
                <Text style={{ ...globalStyles.p4, fontWeight: 600 }}>
                    Notes
                </Text>
            </TouchableOpacity>
            <TouchableOpacity style={globalStyles.elevationButtomSquare} onPress={handlePractice}>
                <Brain size={20} color={theme.colors.onBackground} strokeWidth={1.3} />
                <Text style={{ ...globalStyles.p4, fontWeight: 600 }}>
                    Practice
                </Text>
            </TouchableOpacity>
            <TouchableOpacity style={globalStyles.elevationButtomSquare} onPress={handleRead}>
                <BookOpenText size={20} color={theme.colors.onBackground} strokeWidth={1.3} />
                <Text style={{ ...globalStyles.p4, fontWeight: 600 }}>
                    Read
                </Text>
            </TouchableOpacity>
            <TouchableOpacity style={globalStyles.elevationButtomSquare} onPress={() => sharePassage(passageBottomSheet.passage)}>
                <Share2 size={20} color={theme.colors.onBackground} strokeWidth={1.3} />
                <Text style={{ ...globalStyles.p4, fontWeight: 600 }}>
                    Share
                </Text>
            </TouchableOpacity>
        </ScrollView>
        <HighlightColorDialog
            isOpen={isHighlightColorDialogOpen}
            onOpenChange={setIsHighlightColorDialogOpen}
            onSelectColor={handleSelectHighlightColor}
            selectedColors={selectedHighlightColors}
        />
        </>
    );
});

export default PassageSheetActions;