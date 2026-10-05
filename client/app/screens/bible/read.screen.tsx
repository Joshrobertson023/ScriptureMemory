import { useBottomTabBarHeight } from "@react-navigation/bottom-tabs";
import { useNavigation } from "@react-navigation/native";
import { NativeStackNavigationProp, NativeStackScreenProps } from "@react-navigation/native-stack";
import { useQuery } from "@tanstack/react-query";
import * as Clipboard from 'expo-clipboard';
import { LinkButton, Portal } from 'heroui-native';
import { BookmarkPlus, Brain, Highlighter, MoreHorizontal, MoveLeft, MoveRight, Share2 } from "lucide-react-native";
import React, { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { FlatList, ListRenderItem, StyleSheet, Text, TouchableOpacity, View } from "react-native";
import Animated, { useAnimatedStyle, useSharedValue, withTiming } from "react-native-reanimated";
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { ChapterParagraph, ChapterVerseJson, ResponseChapterJson } from "../../../types/bible/chapterJson";
import { UserPassage } from "../../../types/passages/userPassage";
import { RootStackParamList } from "../../../types/router";
import { Verse } from "../../../types/verse/verse";
import { getChapterJson } from "../../api/verses.api";
import HighlightColorDialog from "../../components/dialogs/highlightColorDialog";
import ReaderSettingsDialog from "../../components/dialogs/readerSettingsDialog";
import ReadTranslationDialog from "../../components/dialogs/readTranslationDialog";
import Paragraph from "../../components/read/paragraph";
import { useBibleVersion } from "../../hooks/useBibleVersion";
import { useBooks } from "../../hooks/useBooks";
import { useBottomSheetStack } from "../../hooks/useBottomSheetStack";
import { useChapterHighlights } from "../../hooks/useChapterHighlights";
import { pushPracticeSessionRoute } from "../../navigation";
import useReferenceParser from "../../hooks/useReferenceParser";
import { useBottomSheetsStore } from "../../stores/bottomSheets.store";
import { initialReference } from "../../stores/collections.store";
import { useReaderSettingsStore } from "../../stores/readerSettings.store";
import { useUserAuthStore } from "../../stores/userAuth.store";
import useGlobalStyles from "../../styles/gobalStyles";
import { HighlightColorId } from "../../styles/highlightColors";
import { getReaderBackground, getReaderFont } from "../../styles/readerFonts";
import useAppTheme from "../../theme";
import { sharePassage } from "../../utils/share";
import { showToast } from "../../utils/toast";

type Props = NativeStackScreenProps<RootStackParamList, 'read'>;

const emptyUserPassage: UserPassage = {
    passage: {
        reference: initialReference,
        verses: []
    }
};

const ReadScreen: React.FC<Props> = ({ route }: Props) => {
    const theme = useAppTheme();
    const globalStyles = useGlobalStyles();
    const insets = useSafeAreaInsets();
    const navigation = useNavigation<NativeStackNavigationProp<RootStackParamList>>();

    const { fontId, fontSize, margin, background, translationOverride } = useReaderSettingsStore();
    const activeFont = useMemo(() => getReaderFont(fontId), [fontId]);
    const activeBackground = useMemo(() => getReaderBackground(background), [background]);
    const tabBarHeight = useBottomTabBarHeight();

    const styles = useMemo(() => StyleSheet.create({
        screen: {
            flex: 1,
            backgroundColor: activeBackground.backgroundColor,
        },
        title: {
            marginVertical: 40,
            marginTop: 70,
            fontSize: 32,
            fontWeight: 600,
            textAlign: 'center',
            fontFamily: activeFont.fontFamily,
            color: activeBackground.textColor,
        },
        versionLabel: {
            textAlign: 'center',
            marginTop: -30,
            marginBottom: 10,
            color: activeBackground.hintColor,
        },
        container: {
            flexDirection: 'column',
            alignItems: 'center',
            justifyContent: 'center',
            paddingHorizontal: margin,
            paddingTop: 25,
            paddingBottom: 120,
        },
        sheetContainer: {
            padding: 15,
            paddingTop: 25,
            paddingBottom: 24,
        },
        verseNumber: {
            fontSize: 10,
            color: activeBackground.hintColor,
            verticalAlign: 'top',
            includeFontPadding: false,
        },
        verseSelected: {
            textDecorationLine: 'underline',
        },
        paragraph: {
            fontFamily: activeFont.fontFamily,
            fontSize: fontSize,
            color: activeBackground.textColor,
            lineHeight: Math.round(fontSize * 1.55),
            textAlign: 'justify',
            marginBottom: 10,
        },
        poetryLine: {
            textAlign: 'left',
            marginBottom: 0,
            paddingLeft: 20,
        },
        poetryLineIndent: {
            paddingLeft: 40,
        },
        blankLine: {
            height: 16,
            width: '100%',
        },
        italicText: {
            fontStyle: 'italic',
        },
        backButton: {
            position: 'absolute',
            top: insets.top + 10,
            left: 15,
            height: 40,
            width: 60,
            borderRadius: 20,
            backgroundColor: theme.colors.elevation,
            alignItems: 'center',
            justifyContent: 'center',
        },
        personalizeButtonRow: {
            position: 'absolute',
            top: insets.top + 10,
            right: 15,
            flexDirection: 'row',
            alignItems: 'center',
            gap: 8,
        },
        navigationRow: {
            position: 'absolute',
            bottom: tabBarHeight,
            flexDirection: 'row',
            alignItems: 'center',
            justifyContent: 'space-between',
            backgroundColor: theme.colors.background,
            width: '100%',
            paddingLeft: 25,
            paddingRight: 25,
            borderBottomColor: theme.colors.elevation2,
            borderBottomWidth: 3
        },
        previousButton: {
        },
        nextButton: {
        },
        translationButton: {
            height: 40,
            minWidth: 40,
            paddingHorizontal: 14,
            borderRadius: 20,
            backgroundColor: theme.colors.elevation,
            alignItems: 'center',
            justifyContent: 'center',
        },
        personalizeButton: {
            width: 40,
            height: 40,
            borderRadius: 20,
            backgroundColor: theme.colors.elevation,
            alignItems: 'center',
            justifyContent: 'center',
        },
        actionBar: {
            position: 'absolute',
            bottom: 0,
            left: 0,
            right: 0,
            padding: 15,
            paddingBottom: insets.bottom + 15,
            backgroundColor: theme.colors.elevation,
            borderTopLeftRadius: 16,
            borderTopRightRadius: 16,
            shadowColor: '#000',
            shadowOffset: { width: 0, height: -2 },
            shadowOpacity: 0.1,
            shadowRadius: 4,
            elevation: 5,
            gap: 8,
        },
        actionBarHeader: {
            flexDirection: 'row',
            alignItems: 'center',
            justifyContent: 'space-between',
        },
        actionBarButtons: {
            flexDirection: 'row',
            justifyContent: 'center',
            alignItems: 'center',
            gap: 5,
        },
    }), [theme, activeFont, activeBackground, margin, fontSize, insets.top, insets.bottom]);

    const { book, chapter } = route.params;

    const [currentBook, setCurrentBook] = useState(book);
    const [currentChapter, setCurrentChapter] = useState(chapter);

    const { version: preferredBibleVersion } = useBibleVersion();
    const bibleVersion = translationOverride ?? preferredBibleVersion;
    const setPassageSheetOpen = useBottomSheetsStore((state) => state.setPassageSheetOpen);
    const setPassageBottomSheet = useBottomSheetsStore((state) => state.setPassageBottomSheet);
    const pushPassage = useBottomSheetsStore((state) => state.pushPassage);
    const passageSheetOpen = useBottomSheetsStore((state) => state.passageSheetOpen);
    const setSaveToCollectionBottomSheet = useBottomSheetsStore((state) => state.setSaveToCollectionBottomSheet);
    const setSaveToCollectionSheetOpen = useBottomSheetsStore((state) => state.setSaveToCollectionSheetOpen);
    const { closePassages } = useBottomSheetStack();

    const [isPersonalizeOpen, setIsPersonalizeOpen] = useState(false);
    const [isTranslationDialogOpen, setIsTranslationDialogOpen] = useState(false);
    const [isHighlightDialogOpen, setIsHighlightDialogOpen] = useState(false);

    const jwt = useUserAuthStore((state) => state.jwt);
    const { data: chapterData, isLoading: loading, error: chapterError } = useQuery({
        queryKey: ['chapterJson', bibleVersion, currentBook, currentChapter],
        queryFn: () => getChapterJson(bibleVersion, currentBook, currentChapter, jwt),
        staleTime: Infinity,
        gcTime: 1000 * 60 * 60,
    });
    const contentOpacity = useSharedValue(0);
    const contentStyle = useAnimatedStyle(() => ({
        opacity: contentOpacity.value,
    }));

    const actionBarTranslateY = useSharedValue(200);

    const actionBarStyle = useAnimatedStyle(() => ({
        transform: [{ translateY: actionBarTranslateY.value }],
    }));

    const showActionBar = useCallback(() => {
        actionBarTranslateY.value = withTiming(0, { duration: 250 });
    }, [actionBarTranslateY]);

    const hideActionBar = useCallback(() => {
        actionBarTranslateY.value = withTiming(200, { duration: 250 });
    }, [actionBarTranslateY]);

    const versesById = useMemo(() => {
        const map: Record<string, ChapterVerseJson> = {};
        chapterData?.verses.forEach(v => { map[v.verseId] = v; });
        return map;
    }, [chapterData]);
    
    const listRef = useRef<FlatList<ChapterParagraph>>(null);

    const verseFromJson = useCallback((jsonVerse: ChapterVerseJson): Verse => ({
        id: jsonVerse.verseId,
        text: jsonVerse.text,
        reference: {
            book,
            chapter,
            verses: [jsonVerse.verseNumber],
            readableReference: `${currentBook} ${currentChapter}:${jsonVerse.verseNumber}`,
        },
        translationContents: [{
            version: bibleVersion,
            plainText: jsonVerse.text,
            contentUsx: '',
            lastUpdated: null,
            verseId: jsonVerse.verseId,
        }],
        savedCount: 0,
        memorizedCount: 0,
        verseNumbers: `${jsonVerse.verseNumber}`,
    }), [currentBook, currentChapter, bibleVersion]);

    const [highlightedPassage, setHighlightedPassage] = useState<UserPassage>(emptyUserPassage);
    const highlightedPassageRef = useRef(highlightedPassage);
    highlightedPassageRef.current = highlightedPassage;
    const { convertToReadableReference } = useReferenceParser();

    const {next, previous} = useBooks();

    const handlePreviousTap = useCallback(() => {
        contentOpacity.value = 0;
        const previousBook: {book: string; nextChapter: number} = previous(currentBook, currentChapter);

        console.log(previousBook.book, previousBook.nextChapter);
        setCurrentBook(previousBook.book);
        setCurrentChapter(previousBook.nextChapter);
    }, [currentBook, currentChapter, previous]);

    const handleNextTap = useCallback(() => {
        contentOpacity.value = 0;
        const nextBook: {book: string; nextChapter: number} = next(currentBook, currentChapter);

        console.log(nextBook.book, nextBook.nextChapter);
        setCurrentBook(nextBook.book);
        setCurrentChapter(nextBook.nextChapter);
    }, [currentBook, currentChapter, next]);

    const selectedVerseNumbers = useMemo(
        () => new Set(highlightedPassage.passage.verses.map(v => v.reference.verses[0])),
        [highlightedPassage]
    );

    const { highlightColorByVerseId, toggleHighlights } = useChapterHighlights();

    useEffect(() => {
        if (passageSheetOpen) {
            hideActionBar();
        } else {
            highlightedPassageRef.current = emptyUserPassage;
            setHighlightedPassage(emptyUserPassage);
        }
    }, [passageSheetOpen, hideActionBar]);

    const readerActionsRef = useRef({
        currentBook,
        currentChapter,
        convertToReadableReference,
        closePassages,
        showActionBar,
        hideActionBar,
    });
    readerActionsRef.current = {
        currentBook,
        currentChapter,
        convertToReadableReference,
        closePassages,
        showActionBar,
        hideActionBar,
    };

    const handleVerseTap = useCallback((verse: Verse) => {
        const {
            currentBook: bookName,
            currentChapter: chapterNumber,
            convertToReadableReference: toReference,
            closePassages: close,
            showActionBar: show,
            hideActionBar: hide,
        } = readerActionsRef.current;

        const current = highlightedPassageRef.current;
        const alreadySelected = current.passage.verses.some((v) => v.id === verse.id);
        const updatedVerses = alreadySelected
            ? current.passage.verses.filter((v) => v.id !== verse.id)
            : [...current.passage.verses, verse];
        const updatedVerseNumbers = updatedVerses.flatMap((v) => v.reference.verses);

        const nextPassage: UserPassage = {
            ...current,
            passage: {
                ...current.passage,
                verses: updatedVerses,
                reference: {
                    book: bookName,
                    chapter: chapterNumber,
                    verses: updatedVerseNumbers,
                    readableReference: updatedVerses.length === 0
                        ? ''
                        : toReference(bookName, chapterNumber, updatedVerseNumbers),
                },
            },
        };

        highlightedPassageRef.current = nextPassage;
        setHighlightedPassage(nextPassage);

        if (updatedVerses.length === 0) {
            hide();
            close();
        } else if (!alreadySelected && current.passage.verses.length === 0) {
            show();
        }
    }, []);

    const handleOpenPassageSheet = useCallback(() => {
        if (highlightedPassage.passage.verses.length === 0) return;

        pushPassage(highlightedPassage);
        setPassageBottomSheet(highlightedPassage);
        setPassageSheetOpen(true);
    }, [highlightedPassage, pushPassage, setPassageBottomSheet, setPassageSheetOpen]);

    const handleCloseSelection = useCallback(() => {
        highlightedPassageRef.current = emptyUserPassage;
        setHighlightedPassage(emptyUserPassage);
        hideActionBar();
    }, [hideActionBar]);

    const handleSelectHighlightColor = (color: HighlightColorId) => {
        toggleHighlights(highlightedPassage.passage.verses.map((verse) => verse.id), color);
        setIsHighlightDialogOpen(false);
        handleCloseSelection();
    };

    const handleSave = () => {
        const passage = highlightedPassage;
        handleCloseSelection();
        setSaveToCollectionBottomSheet(passage);
        setSaveToCollectionSheetOpen(true);
    };

    const handlePractice = () => {
        const passage = highlightedPassage;
        handleCloseSelection();
        pushPracticeSessionRoute(passage);
    };

    const versesByIdRef = useRef(versesById);
    versesByIdRef.current = versesById;
    const verseFromJsonRef = useRef(verseFromJson);
    verseFromJsonRef.current = verseFromJson;

    const handleSpanTap = useCallback((verseId: string) => {
        const jsonVerse = versesByIdRef.current[verseId];
        if (!jsonVerse) return;

        handleVerseTap(verseFromJsonRef.current(jsonVerse));
    }, [handleVerseTap]);

    const keyExtractor = useCallback(
        (_: ChapterParagraph, index: number) => `${currentBook}-${currentChapter}-${index}`,
        [currentBook, currentChapter],
    );

    useEffect(() => {
        if (!chapterData) return;
        contentOpacity.value = 0;
        contentOpacity.value = withTiming(1, { duration: 300 });
    }, [chapterData]);

    useEffect(() => {
        if (!chapterError) return;
        console.error('error fetching chapter', chapterError);

        const errorMessage = chapterError.message || 'Unknown error';

        showToast({
            type: 'danger',
            title: 'We encountered an error loading this chapter.',
            actionLabel: 'COPY ERROR',
            onActionPress: async ({ hide }) => { await Clipboard.setStringAsync(errorMessage); hide(); }
        });
    }, [chapterError]);

    const renderParagraph: ListRenderItem<ChapterParagraph> = useCallback(({ item, index }) => (
        <Paragraph
            paragraph={item}
            paragraphIndex={index}
            versesById={versesById}
            selectedVerseNumbers={selectedVerseNumbers}
            highlightColorByVerseId={highlightColorByVerseId}
            onSpanPress={handleSpanTap}
            paragraphStyle={[globalStyles.verseText, styles.paragraph]}
            poetryLineStyle={styles.poetryLine}
            poetryLineIndentStyle={styles.poetryLineIndent}
            blankLineStyle={styles.blankLine}
            verseNumberStyle={styles.verseNumber}
            selectedStyle={styles.verseSelected}
            italicStyle={styles.italicText}
        />
    ), [versesById, selectedVerseNumbers, highlightColorByVerseId, handleSpanTap, globalStyles.verseText, styles]);

    const listHeader = useMemo(() => (
        <>
            <Text style={[globalStyles.verseReference, styles.title]}>
                {currentBook} {currentChapter}
            </Text>
        </>
    ), [globalStyles.verseReference, globalStyles.verseVersionLabel, styles.title, styles.versionLabel, currentBook, currentChapter, bibleVersion]);

    const listFooter = useMemo(() => (
        chapterData?.copyright ? (
            <View style={{display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', marginBottom: 100}}>
                <Text style={{ ...globalStyles.p1, marginTop: 30, textAlign: 'center', maxWidth: '80%', color: activeBackground.textColor, fontSize: 12 }}>
                    {chapterData.copyright}
                </Text>
                <LinkButton style={{marginTop: 10, padding: 5}} onPress={() => {navigation.navigate('credits');}}><Text style={{...globalStyles.linkButtonText, color: activeBackground.textColor}}>Credits</Text></LinkButton>
            </View>
        ) : null
    ), [chapterData?.copyright, globalStyles.p1, activeBackground.textColor]);

    return (
        <View style={styles.screen}>
            {!loading && (
                <>
                    <Animated.View style={[{ flex: 1 }, contentStyle]}>
                        <FlatList
                            ref={listRef}
                            style={{ flex: 1}}
                            contentContainerStyle={styles.container}
                            data={chapterData?.paragraphs ?? []}
                            renderItem={renderParagraph}
                            keyExtractor={keyExtractor}
                            ListHeaderComponent={listHeader}
                            ListFooterComponent={listFooter}
                            scrollEventThrottle={1}
                            initialNumToRender={8}
                            maxToRenderPerBatch={8}
                            windowSize={7}
                        />

                        <View style={[styles.backButton]}>
                            <TouchableOpacity onPress={() => {
                                navigation.goBack();
                            }} activeOpacity={0.7} >
                                <Text style={globalStyles.p3}>Back</Text>
                            </TouchableOpacity>
                        </View>

                        <View style={[styles.personalizeButtonRow]}>
                            <TouchableOpacity
                                onPress={() => setIsTranslationDialogOpen(true)}
                                activeOpacity={0.7}
                                style={styles.translationButton}
                            >
                                <Text style={globalStyles.p3}>{bibleVersion.toUpperCase()}</Text>
                            </TouchableOpacity>
                            <TouchableOpacity
                                onPress={() => setIsPersonalizeOpen(true)}
                                activeOpacity={0.7}
                                style={styles.personalizeButton}
                            >
                                <Text style={globalStyles.p3}>Aa</Text>
                            </TouchableOpacity>
                        </View>
                    </Animated.View>
                </>
            )}

            <View style={[styles.navigationRow]}>
                <TouchableOpacity onPress={handlePreviousTap} activeOpacity={0.7} style={styles.previousButton} disabled={currentBook === "Genesis" && currentChapter === 1}>
                    <MoveLeft color={
                        currentBook === "Genesis" && currentChapter === 1 ? theme.colors.elevation3 : theme.colors.onBackground
                    } size={42} />
                </TouchableOpacity>
                <TouchableOpacity onPress={handleNextTap} activeOpacity={0.7} style={styles.nextButton} disabled={currentBook === "Revelation" && currentChapter === 22}>
                    <MoveRight color={
                        currentBook === "Revelation" && currentChapter === 22 ? theme.colors.elevation3 : theme.colors.onBackground
                    } size={42} />
                </TouchableOpacity>
            </View>

            <Portal name="action-bar-portal">
                <Animated.View style={[styles.actionBar, actionBarStyle]}>
                    <View style={styles.actionBarHeader}>
                        <Text style={globalStyles.p3}>
                            {highlightedPassage.passage.reference.readableReference}
                        </Text>
                        <TouchableOpacity onPress={handleCloseSelection}>
                            <Text style={globalStyles.p3}>Close</Text>
                        </TouchableOpacity>
                    </View>
                    <View style={styles.actionBarButtons}>
                        <TouchableOpacity style={globalStyles.elevationButtomSquare} onPress={() => setIsHighlightDialogOpen(true)}>
                            <Highlighter size={20} color={theme.colors.onBackground} strokeWidth={1.3} />
                            <Text style={{ ...globalStyles.p4, fontWeight: 600 }}>Highlight</Text>
                        </TouchableOpacity>
                        <TouchableOpacity style={globalStyles.elevationButtomSquare} onPress={handleSave}>
                            <BookmarkPlus size={20} color={theme.colors.onBackground} strokeWidth={1.3} />
                            <Text style={{ ...globalStyles.p4, fontWeight: 600 }}>Save</Text>
                        </TouchableOpacity>
                        <TouchableOpacity style={globalStyles.elevationButtomSquare} onPress={handlePractice}>
                            <Brain size={20} color={theme.colors.onBackground} strokeWidth={1.3} />
                            <Text style={{ ...globalStyles.p4, fontWeight: 600 }}>Practice</Text>
                        </TouchableOpacity>
                        <TouchableOpacity style={globalStyles.elevationButtomSquare} onPress={() => sharePassage(highlightedPassage.passage)}>
                            <Share2 size={20} color={theme.colors.onBackground} strokeWidth={1.3} />
                            <Text style={{ ...globalStyles.p4, fontWeight: 600 }}>Share</Text>
                        </TouchableOpacity>
                        <TouchableOpacity style={globalStyles.elevationButtomSquare} onPress={handleOpenPassageSheet}>
                            <MoreHorizontal size={20} color={theme.colors.onBackground} strokeWidth={1.3} />
                            <Text style={{ ...globalStyles.p4, fontWeight: 600 }}>More</Text>
                        </TouchableOpacity>
                    </View>
                </Animated.View>
            </Portal>

            <HighlightColorDialog
                isOpen={isHighlightDialogOpen}
                onOpenChange={setIsHighlightDialogOpen}
                onSelectColor={handleSelectHighlightColor}
                selectedColors={new Set(highlightedPassage.passage.verses.map((verse) => highlightColorByVerseId.get(verse.id)))}
            />
            <ReaderSettingsDialog isOpen={isPersonalizeOpen} onOpenChange={setIsPersonalizeOpen} />
            <ReadTranslationDialog isOpen={isTranslationDialogOpen} onOpenChange={setIsTranslationDialogOpen} />
        </View>
    );
};

export default ReadScreen;
