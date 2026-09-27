import { TrueSheet } from "@lodev09/react-native-true-sheet";
import { X } from "lucide-react-native";
import { useInfiniteQuery, useQuery } from "@tanstack/react-query";
import React, { forwardRef, useCallback, useEffect, useMemo, useRef, useState } from "react";
import { LayoutChangeEvent, NativeScrollEvent, NativeSyntheticEvent, ScrollView, StyleSheet, Text, TouchableOpacity, View } from "react-native";
import { Collection } from "../../../types/collection/collection";
import { CrossReferenceGroup, initialVerseCardResponse } from "../../../types/verse/verseCard";
import { getSimilarPassages, getVerseCard } from "../../api/verses.api";
import { useBibleVersion } from "../../hooks/useBibleVersion";
import { useBottomSheetStack } from "../../hooks/useBottomSheetStack";
import { useCollectionsContainingVersesOnce } from "../../hooks/useCollections";
import { isCurrentCollectionRoute, pushCollectionRoute } from "../../navigation";
import { getPassageCacheKey, useBottomSheetsStore } from "../../stores/bottomSheets.store";
import { useUserStore } from "../../stores/user.store";
import { useUserAuthStore } from "../../stores/userAuth.store";
import useGlobalStyles from "../../styles/gobalStyles";
import useAppTheme from "../../theme";
import { getVerseNumbers } from "../../utils/referenceUtils";
import Collections from "../passage/collections";
import CrossReferences from "../passage/crossReferences";
import PassageSheetActions from "../passage/passageSheetActions";
import PassageSheetMetadata from "../passage/passageSheetMetadata";
import Similar from "../passage/similar";

const emptyCrossReferences: CrossReferenceGroup[] = [];

const PassageBottomSheet = forwardRef<TrueSheet>(
    (_, ref) => {
        const globalStyles = useGlobalStyles();
        const theme = useAppTheme();
        const styles = useMemo(() => StyleSheet.create({
            verseTextBlock: {
                justifyContent: 'flex-start',
                alignItems: 'flex-start',
                marginTop: 10,
                width: '100%'
            },
            headerRow: {
                flexDirection: 'row',
                alignItems: 'flex-start',
                justifyContent: 'space-between',
            },
            reference: {
                flex: 1,
                marginRight: 12,
            },
            closeButton: {
                padding: 4,
                marginTop: -4,
                marginRight: -4,
            },
            sheetContainer: {
                padding: 15,
                paddingTop: 25,
                paddingBottom: 24,
            },
            scrollView: {
                flex: 1,
            },
            verseTextContainer: {
                flexDirection: 'row',
                flexWrap: 'wrap',
                alignItems: 'flex-start',
            },
            versesContainer: {
                gap: 2,
                width: '100%'
            },
            verseRow: {
                flexDirection: 'row',
                alignItems: 'flex-start',
            },
            verseNumber: {
                fontSize: 10,
                color: theme.colors.onBackgroundSoft,
                verticalAlign: 'top',
                includeFontPadding: false,
            },
            spacer20: {
                height: 20,
            },
            spacer25: {
                height: 25,
            },
        }), [theme]);

        const { version: bibleVersion } = useBibleVersion();
        const jwt = useUserAuthStore((state) => state.jwt);
        const userId = useUserStore((state) => state.user.id);
        const passageBottomSheet = useBottomSheetsStore((state) => state.passageBottomSheet);
        const {
            goToLastPassage,
            closePassages,
            handlePassageSheetDidDismiss,
        } = useBottomSheetStack();
        const lastPassage = useBottomSheetsStore((state) => state.passageSheetStack.at(-2));
        const passageKey = useMemo(() => getPassageCacheKey(passageBottomSheet), [passageBottomSheet]);
        const passageVerseIds = useMemo(
            () => passageBottomSheet.passage.verses.map((verse) => verse.id),
            [passageBottomSheet]
        );
        const displayedVersion = passageBottomSheet.passage.verses[0]?.translationContents?.at(0)?.version;
        const translation = displayedVersion ?? bibleVersion;

        const { data: passageCardData, error: passageCardError } = useQuery({
            queryKey: ['passageCard', passageKey],
            queryFn: () => getVerseCard(Number(userId) || 0, passageVerseIds, jwt),
            enabled: passageVerseIds.length > 0,
            staleTime: Infinity,
        });
        const crossReferences = passageCardData?.crossReferences ?? emptyCrossReferences;

        const [similarVisible, setSimilarVisible] = useState(false);
        const similarY = useRef(Infinity);
        const viewportHeight = useRef(0);
        const scrollBottom = useRef(0);

        useEffect(() => {
            setSimilarVisible(false);
            similarY.current = Infinity;
            scrollBottom.current = viewportHeight.current;
        }, [passageKey]);

        const similar = useInfiniteQuery({
            queryKey: ['similarPassages', passageKey, translation],
            queryFn: ({ pageParam }) => getSimilarPassages(passageBottomSheet.passage, translation, pageParam, jwt),
            initialPageParam: null as number | null,
            getNextPageParam: (lastPage) => lastPage.at(-1)?.verses.at(0)?.searchDistance ?? undefined,
            enabled: similarVisible && passageVerseIds.length > 0,
            staleTime: Infinity,
            retry: false,
        });
        const similarPassages = useMemo(() => similar.data?.pages.flat(), [similar.data]);

        const handleScroll = ({ nativeEvent }: NativeSyntheticEvent<NativeScrollEvent>) => {
            const { contentOffset, layoutMeasurement, contentSize } = nativeEvent;
            scrollBottom.current = contentOffset.y + layoutMeasurement.height;

            if (scrollBottom.current >= similarY.current) {
                setSimilarVisible(true);
            }
            if (scrollBottom.current >= contentSize.height - 300 && similar.hasNextPage && !similar.isFetchingNextPage && !similar.error) {
                similar.fetchNextPage();
            }
        };

        const handleSimilarLayout = ({ nativeEvent }: LayoutChangeEvent) => {
            similarY.current = nativeEvent.layout.y;
            if (scrollBottom.current >= similarY.current) {
                setSimilarVisible(true);
            }
        };

        const verseText = useMemo(() => (
                passageBottomSheet.passage.verses.map((verse, index) => (
                <Text key={verse.id}>
                    {passageBottomSheet.passage.verses.length > 1 && (
                        <Text style={styles.verseNumber}>{getVerseNumbers(verse.reference).at(0)} </Text>
                    )}
                    {verse.translationContents?.at(0)?.plainText}
                    {index < passageBottomSheet.passage.verses.length - 1 ? ' ' : ''}
                </Text>
            ))
        ), [passageBottomSheet, styles]);

        const handleCollectionPress = useCallback((collection: Collection) => {
            const isCurrentCollection = isCurrentCollectionRoute(collection.id);
            closePassages();

            if (!isCurrentCollection) {
                pushCollectionRoute(collection.id);
            }
        }, [closePassages]);

        const collections = useCollectionsContainingVersesOnce(passageKey, passageVerseIds, true);

        return (
            <TrueSheet
                ref={ref}
                detents={[.75, 1]}
                onDidDismiss={handlePassageSheetDidDismiss}
                style={{backgroundColor: theme.colors.background}}
                scrollable
           >
                <ScrollView
                    style={styles.scrollView}
                    contentContainerStyle={styles.sheetContainer}
                    showsVerticalScrollIndicator={false}
                    onScroll={handleScroll}
                    onLayout={({ nativeEvent }) => {
                        viewportHeight.current = nativeEvent.layout.height;
                        scrollBottom.current = Math.max(scrollBottom.current, nativeEvent.layout.height);
                    }}
                    scrollEventThrottle={16}
                >
                    {lastPassage && (
                        <TouchableOpacity onPress={goToLastPassage}>
                            <Text style={[globalStyles.p3, globalStyles.linkButtonText]}>
                                Back to {lastPassage.passage.reference.readableReference}
                            </Text>
                        </TouchableOpacity>
                    )}
                    <View style={styles.headerRow}>
                        <Text style={[globalStyles.verseReference, styles.reference]}>
                            {passageBottomSheet.passage.reference.readableReference}
                        </Text>
                        <TouchableOpacity style={styles.closeButton} onPress={closePassages} hitSlop={10}>
                            <X size={22} color={theme.colors.onBackgroundSoft} strokeWidth={1.5} />
                        </TouchableOpacity>
                    </View>

                    <View style={[styles.verseTextBlock]}>
                        <View style={styles.versesContainer}>
                            <Text style={globalStyles.verseText}>
                                {verseText}
                            </Text>
                            {displayedVersion && (
                                <Text style={globalStyles.verseVersionLabel}>{displayedVersion.toUpperCase()}</Text>
                            )}
                        </View>
                    </View>

                    <View style={{height: 20}} />

                    <PassageSheetMetadata
                        passage={passageBottomSheet}
                        loading={!passageCardData && !passageCardError}
                        data={passageCardData ?? initialVerseCardResponse}
                        collectionsCount={collections.length}
                    />

                    <View style={styles.spacer25} />

                    <PassageSheetActions passageBottomSheet={passageBottomSheet} />

                    <View style={styles.spacer20} />

                    <CrossReferences
                        crossReferences={crossReferences}
                        loading={!passageCardData && !passageCardError}
                    />

                    <Collections
                        collections={collections}
                        onCollectionPress={handleCollectionPress}
                    />

                    <View onLayout={handleSimilarLayout}>
                        <Similar
                            reference={passageBottomSheet.passage.reference.readableReference}
                            similarPassages={similarPassages}
                            isLoading={!similarPassages && !similar.error}
                            isFetchingMore={similar.isFetchingNextPage}                        />
                    </View>

                </ScrollView>
            </TrueSheet>
        );
    }
);

export default PassageBottomSheet;