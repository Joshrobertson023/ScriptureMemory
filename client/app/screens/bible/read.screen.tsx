import { NativeStackNavigationProp, NativeStackScreenProps } from "@react-navigation/native-stack";
import { View, StyleSheet, Text, TouchableOpacity } from "react-native";
import { RootStackParamList } from "../../../types/router";
import useAppTheme from "../../theme";
import useGlobalStyles from "../../styles/gobalStyles";
import React, { useEffect, useMemo, useState } from "react";
import { Verse } from "../../../types/verse/verse";
import { getChapterVerses } from "../../api/verses.api";
import { useUserAuthStore } from "../../stores/userAuth.store";
import Skeleton from "react-native-reanimated-skeleton";
import { useHideTabBarOnScroll } from "../../hooks/useTabBarOnScroll";
import Animated, { useAnimatedStyle, useSharedValue, withTiming } from "react-native-reanimated";
import { BookmarkPlus, Brain, ChevronUp, Highlighter, MoreHorizontal, Share2 } from "lucide-react-native";
import { useBottomSheetsStore } from "../../stores/bottomSheets.store";
import { useBottomSheetStack } from "../../hooks/useBottomSheetStack";
import { UserPassage } from "../../../types/passages/userPassage";
import { initialReference } from "../../stores/collections.store";
import useReferenceParser from "../../hooks/useReferenceParser";
import { Portal } from 'react-native-paper';

type Props = NativeStackScreenProps<RootStackParamList, 'read'>;

const ReadScreen: React.FC<Props> = ({ route }: Props) => {
    const theme = useAppTheme();
    const globalStyles = useGlobalStyles();

    const styles = useMemo(() => StyleSheet.create({
        title: {
            marginVertical: 40,
            marginTop: 70,
            fontSize: 32,
            fontWeight: 600
        },
        container: {
            flexDirection: 'row',
            flexWrap: 'wrap',
            justifyContent: 'center',
            padding: 25,
            gap: 10,
            paddingBottom: 120,
        },
        sheetContainer: {
            padding: 15,
            paddingTop: 25,
            paddingBottom: 24,
        },
        verseNumber: {
            fontSize: 10,
            color: theme.colors.onBackgroundSoft,
            verticalAlign: 'top',
            includeFontPadding: false,
        },
        verseText: {
            lineHeight: 35,
            textAlign: 'justify'
        },
        verseHighlighted: {
            textDecorationLine: 'underline',
        },
        actionBar: {
            position: 'absolute',
            bottom: 0,
            left: 0,
            right: 0,
            padding: 15,
            paddingBottom: 30,
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
        actionBarButtons: {
            flexDirection: 'row',
            justifyContent: 'center',
            alignItems: 'center',
            gap: 5,
        },
    }), [theme]);

    const { book, chapter } = route.params;

    const { setPassageSheetOpen, setPassageBottomSheet, pushPassage, passageSheetStack } = useBottomSheetsStore();
    const { goToNextPassage } = useBottomSheetStack();

    const skeletonProps = {
        boneColor: theme.colors.elevation,
        highlightColor: theme.colors.elevation3,
    };
    const skeletonStyle = { width: '100%' as const };
    const skeletonLayout = [
        { width: '100%' as const, height: 20, borderRadius: 4, marginBottom: 20 },
        { width: '100%' as const, height: 20, borderRadius: 4, marginBottom: 20 },
        { width: '100%' as const, height: 20, borderRadius: 4, marginBottom: 20 },
        { width: '100%' as const, height: 20, borderRadius: 4, marginBottom: 20 },
        { width: '100%' as const, height: 20, borderRadius: 4, marginBottom: 20 },
        { width: '100%' as const, height: 20, borderRadius: 4, marginBottom: 20 },
        { width: '100%' as const, height: 20, borderRadius: 4, marginBottom: 20 },
        { width: '100%' as const, height: 20, borderRadius: 4, marginBottom: 20 },
        { width: '100%' as const, height: 20, borderRadius: 4, marginBottom: 20 },
        { width: '100%' as const, height: 20, borderRadius: 4, marginBottom: 20 },
        { width: '100%' as const, height: 20, borderRadius: 4, marginBottom: 20 },
        { width: '100%' as const, height: 20, borderRadius: 4, marginBottom: 20 },
        { width: '100%' as const, height: 20, borderRadius: 4, marginBottom: 20 },
        { width: '100%' as const, height: 20, borderRadius: 4, marginBottom: 20 },
        { width: '100%' as const, height: 20, borderRadius: 4, marginBottom: 20 },
        { width: '100%' as const, height: 20, borderRadius: 4, marginBottom: 20 },
        { width: '100%' as const, height: 20, borderRadius: 4, marginBottom: 20 },
        { width: '100%' as const, height: 20, borderRadius: 4, marginBottom: 20 },
        { width: '100%' as const, height: 20, borderRadius: 4, marginBottom: 20 },
        { width: '100%' as const, height: 20, borderRadius: 4, marginBottom: 20 },
        { width: '100%' as const, height: 20, borderRadius: 4, marginBottom: 20 },
        { width: '100%' as const, height: 20, borderRadius: 4, marginBottom: 20 },
    ];

    const [verses, setVerses] = useState<Verse[]>([]);
    const [loading, setLoading] = useState(true);

    const emptyUserPassage: UserPassage = {
        passage: {
            reference: initialReference,
            verses: []
        }
    };

    const [highlightedPassage, setHighlightedPassage] = useState<UserPassage>(emptyUserPassage);
    const { convertToReadableReference } = useReferenceParser();

    // Reanimated shared value for 120fps UI-thread animation
    const actionBarTranslateY = useSharedValue(200);

    const actionBarStyle = useAnimatedStyle(() => ({
        transform: [{ translateY: actionBarTranslateY.value }],
    }));

    const showActionBar = () => {
        actionBarTranslateY.value = withTiming(0, { duration: 250 });
    };

    const hideActionBar = () => {
        actionBarTranslateY.value = withTiming(200, { duration: 250 });
    };

    const highlightVerse = (v: Verse) => {
        setHighlightedPassage(prev => ({
            ...prev,
            passage: {
                ...prev.passage,
                verses: [...prev.passage.verses, v],
                reference: {
                    book: book,
                    chapter: chapter,
                    verses: [...prev.passage.reference.verses, ...(v.reference.verses[0] ? [v.reference.verses[0]] : [])],
                    readableReference: prev.passage.reference.verses.length === 0
                        ? `${book} ${chapter}:${v.reference.verses.at(0)}`
                        : convertToReadableReference(book, chapter, [...prev.passage.reference.verses, ...(v.reference.verses[0] ? [v.reference.verses[0]] : [])])
                }
            }
        }));
    };

    const unhighlightVerse = (verseId: number) => {
        setHighlightedPassage(prev => {
            const updatedVerses = prev.passage.verses.filter(v => v.id !== verseId);
            const updatedVerseNumbers = updatedVerses.flatMap(v => v.reference.verses);
            return {
                ...prev,
                passage: {
                    ...prev.passage,
                    verses: updatedVerses,
                    reference: {
                        book: book,
                        chapter: chapter,
                        verses: updatedVerseNumbers,
                        readableReference: updatedVerses.length === 0
                            ? ''
                            : convertToReadableReference(book, chapter, updatedVerseNumbers)
                    }
                }
            };
        });
    };

    const isVerseHighlighted = (verseId: number): boolean =>
        highlightedPassage.passage.verses.some(v => v.id === verseId);

    const handleVerseTap = (verse: Verse) => {
        if (isVerseHighlighted(verse.id)) {
            unhighlightVerse(verse.id);
            if (highlightedPassage.passage.verses.length === 1) {
                hideActionBar();
            }
        } else {
            if (highlightedPassage.passage.verses.length === 0) {
                showActionBar();
            }
            highlightVerse(verse);
        }
    };

    const { jwt } = useUserAuthStore();

    useEffect(() => {
        const getChapter = async () => {
            setLoading(true);
            setVerses(await getChapterVerses(book, chapter, jwt));
            setLoading(false);
        };

        getChapter();
    }, [chapter]);

    const { onScroll } = useHideTabBarOnScroll();

    return (
        <>
            <Animated.ScrollView contentContainerStyle={styles.container} onScroll={onScroll} scrollEventThrottle={1}>
                <Text style={[globalStyles.verseReference, styles.title]}>
                    {book} {chapter}
                </Text>

                <Skeleton
                    isLoading={loading}
                    containerStyle={skeletonStyle}
                    layout={skeletonLayout}
                    {...skeletonProps}
                >
                    <Text style={globalStyles.verseText}>
                        {verses.map((verse) => (
                            <Text
                                key={verse.id}
                                onPress={() => handleVerseTap(verse)}
                                style={[
                                    styles.verseText,
                                    isVerseHighlighted(verse.id) && styles.verseHighlighted
                                ]}
                            >
                                <Text style={styles.verseNumber}>
                                    {verse.reference.verses.at(0)}{' '}
                                </Text>
                                {verse.text}{' '}
                            </Text>
                        ))}
                    </Text>
                </Skeleton>
            </Animated.ScrollView>

            <Portal>
            <Animated.View style={[styles.actionBar, actionBarStyle]}>
                <View style={{display: 'flex', flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between'}}>
                    <Text style={globalStyles.p3}>
                        {highlightedPassage.passage.reference.readableReference}
                    </Text>
                    <TouchableOpacity onPress={() => {
                        setHighlightedPassage(emptyUserPassage);
                        hideActionBar();
                    }}>
                        <Text style={globalStyles.p3}>Close</Text>
                    </TouchableOpacity>
                </View>
                <View style={styles.actionBarButtons}>
                    <TouchableOpacity
                        style={globalStyles.elevationButtomSquare}
                        onPress={() => {
                            
                        }}
                    >
                        <Highlighter size={20} color={theme.colors.onBackground} strokeWidth={1.3} />
                        <Text style={{ ...globalStyles.p4, fontWeight: 600 }}>Highlight</Text>
                    </TouchableOpacity>
                    <TouchableOpacity
                        style={globalStyles.elevationButtomSquare}
                        onPress={() => {
                            
                        }}
                    >
                        <BookmarkPlus size={20} color={theme.colors.onBackground} strokeWidth={1.3} />
                        <Text style={{ ...globalStyles.p4, fontWeight: 600 }}>Save</Text>
                    </TouchableOpacity>
                    <TouchableOpacity style={globalStyles.elevationButtomSquare}>
                        <Brain size={20} color={theme.colors.onBackground} strokeWidth={1.3} />
                        <Text style={{ ...globalStyles.p4, fontWeight: 600 }}>Practice</Text>
                    </TouchableOpacity>
                    <TouchableOpacity style={globalStyles.elevationButtomSquare}>
                        <Share2 size={20} color={theme.colors.onBackground} strokeWidth={1.3} />
                        <Text style={{ ...globalStyles.p4, fontWeight: 600 }}>Share</Text>
                    </TouchableOpacity>
                    <TouchableOpacity
                        style={globalStyles.elevationButtomSquare}
                        onPress={() => {
                            if (highlightedPassage.passage.verses.length === 0) return;

                            if (passageSheetStack.length === 0) {
                                pushPassage(highlightedPassage);
                                setPassageBottomSheet(highlightedPassage);
                                setPassageSheetOpen(true);
                                return;
                            }

                            goToNextPassage(highlightedPassage);
                        }}
                    >
                        <ChevronUp size={20} color={theme.colors.onBackground} strokeWidth={1.3} />
                        <Text style={{ ...globalStyles.p4, fontWeight: 600 }}>Open</Text>
                    </TouchableOpacity>
                </View>
            </Animated.View>
                
            </Portal>
        </>
    );
};

export default ReadScreen;