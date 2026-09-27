import { PressableFeedback } from 'heroui-native';
import React, { useMemo } from "react";
import { StyleSheet, Text, View } from "react-native";
import Skeleton from "react-native-reanimated-skeleton";
import { Passage } from '../../../types/passages/passage';
import { UserPassage } from '../../../types/passages/userPassage';
import { Reference } from '../../../types/verse/reference';
import { useBibleVersion } from "../../hooks/useBibleVersion";
import { useBottomSheetStack } from '../../hooks/useBottomSheetStack';
import { useVod } from "../../hooks/useVod";
import { useBottomSheetsStore } from '../../stores/bottomSheets.store';
import useGlobalStyles from "../../styles/gobalStyles";
import useAppTheme from "../../theme";

export const VerseOfDayHomeCard = () => {
    const theme = useAppTheme();
    const style = useGlobalStyles();
    const styles = useMemo(() => StyleSheet.create({
        verseNumber: {
            fontSize: 10,
            color: theme.colors.onBackgroundSoft,
            verticalAlign: 'top',
            includeFontPadding: false,
        }
    }), [theme]);

    const { version: bibleVersion } = useBibleVersion();
    const { data: vod, isLoading, isError } = useVod(bibleVersion);
    const displayedVersion = vod?.verses.at(0)?.translationContents?.at(0)?.version;

    const setPassageSheetOpen = useBottomSheetsStore((state) => state.setPassageSheetOpen);
    const setPassageBottomSheet = useBottomSheetsStore((state) => state.setPassageBottomSheet);
    const pushPassage = useBottomSheetsStore((state) => state.pushPassage);
    const { goToNextPassage } = useBottomSheetStack();
        
    const skeletonProps = {
        boneColor: 'rgba(255, 255, 255, 0.15)',
        highlightColor: 'rgba(255, 255, 255, 0.3)',
    };
    const titleSkeletonLayout = [{ width: '70%' as const, height: 24, borderRadius: 6 }];
    const verseSkeletonLayout = [
        { width: '100%' as const, height: 16, borderRadius: 4, marginBottom: 8 },
        { width: '100%' as const, height: 16, borderRadius: 4, marginBottom: 8 },
        { width: '60%' as const, height: 16, borderRadius: 4 },
    ];

    const emptyReference: Reference = {
        book: '',
        chapter: 0,
        verses: [],
        readableReference: '',
    }

    const passage: Passage = {
        reference: vod?.reference || emptyReference,
        verses: vod ? vod.verses : []
    }

    const userPasage: UserPassage = {
        passage: passage
    }

    if (isError) {
        return null;
    }

    return (
        <PressableFeedback onPress={() => {
            const selectedUserPassage: UserPassage = userPasage;

            if (useBottomSheetsStore.getState().passageSheetStack.length === 0) {
                pushPassage(selectedUserPassage);
                setPassageBottomSheet(selectedUserPassage);
                setPassageSheetOpen(true);
                return;
            }

            goToNextPassage(selectedUserPassage);
        }}
        style={{display: 'flex', flexDirection: 'row', alignItems: 'center'}}
        animation={{
                scale: {
                    value: 0.99,
                    timingConfig: {
                        duration: 400,
                    },
                    ignoreScaleCoefficient: true,
                },
            }}
        >
            <View style={{width: '100%', backgroundColor: theme.colors.elevation, borderRadius: 30,
                flexDirection: 'row', justifyContent: 'space-between', padding: 20}}>
                <View style={{width: '100%', justifyContent: 'space-between'}}>
                    <View>
                        <Text style={style.p4}>Verse of the Day</Text>
                        <Skeleton
                            isLoading={isLoading}
                            hasFadeIn
                            containerStyle={{width: '100%', marginTop: 4}}
                            layout={titleSkeletonLayout}
                            {...skeletonProps}
                        >
                            <Text style={{...style.h2, fontFamily: 'Noto Serif', fontWeight: '600'}}>{vod?.reference?.readableReference}</Text>
                        </Skeleton>
                    </View>
                    <Skeleton
                        isLoading={isLoading}
                        hasFadeIn
                        containerStyle={{width: '100%', marginTop: 20}}
                        layout={verseSkeletonLayout}
                        {...skeletonProps}
                    >
                        <View>
                            <Text style={{...style.p3, fontFamily: 'Noto Serif', fontWeight: '400', lineHeight: 25}}>
                                {vod?.verses.map((verse, index) => (
                                <Text key={verse.id}>
                                    {vod.verses.length > 1 && (
                                        <Text style={styles.verseNumber}>{verse.reference.verses.at(0)} </Text>
                                    )}
                                    {verse.translationContents?.at(0)?.plainText}
                                    {index < vod.verses.length - 1 ? ' ' : ''}
                                </Text>
                            ))}
                            </Text>
                            {displayedVersion && (
                                <Text style={style.verseVersionLabel}>{displayedVersion.toUpperCase()}</Text>
                            )}
                        </View>
                    </Skeleton>
                </View>
            </View>
            <PressableFeedback.Ripple
                styles={{
                    container: {
                        borderRadius: 20,
                        overflow: 'hidden',
                    },
                    ripple: {
                        borderRadius: 999,
                    },
                }}
                animation={{
                    backgroundColor: {
                        value: theme.colors.onBackground,
                    },
                    opacity: {
                        value: [0, 0.15, 0],
                    },
                    progress: {
                        baseDuration: 200,
                    },
                }}
            />
        </PressableFeedback>
    )
}
