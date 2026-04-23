import { TrueSheet } from "@lodev09/react-native-true-sheet";
import { forwardRef, useMemo } from "react";
import { StyleSheet, Text, TouchableOpacity, View, ScrollView } from "react-native";
import useGlobalStyles from "../../styles/gobalStyles";
import useAppTheme from "../../theme";
import { getPassageCacheKey, useBottomSheetsStore } from "../../stores/bottomSheets.store";
import { initialVerseCardResponse } from "../../../types/verse/verseCard";
import Categories from "../passage/categories";
import PassageSheetMetadata from "../passage/passageSheetMetadata";
import CrossReferences from "../passage/crossReferences";
import PassageSheetActions from "../passage/passageSheetActions";
import { useBottomSheetStack } from "../../hooks/useBottomSheetStack";
import Collections from "../passage/collections";
import { Collection } from "../../../types/collection/collection";
import { useCollectionsStore } from "../../stores/collections.store";
import { useShallow } from 'zustand/react/shallow';
import { isCurrentCollectionRoute, pushCollectionRoute } from "../../navigation";
import Similar from "../passage/similar";

const PassageBottomSheet = forwardRef<TrueSheet>(
    (_, ref) => {
        const globalStyles = useGlobalStyles();
        const theme = useAppTheme();
        const styles = useMemo(() => StyleSheet.create({
            verse: {
                justifyContent: 'flex-start',
                alignItems: 'flex-start',
                gap: 5,
                marginTop: 10,
                width: '100%'
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
        }), [theme]);

        const {
            passageBottomSheet,
        } = useBottomSheetsStore();
        const {
            goToLastPassage,
            closePassages,
            handlePassageSheetDidDismiss,
            getLastPassage
        } = useBottomSheetStack();
        const passageKey = useMemo(() => getPassageCacheKey(passageBottomSheet), [passageBottomSheet]);
        const passageCardData = useBottomSheetsStore((state) => state.passageCardCache[passageKey]);
        const similarPassages = useBottomSheetsStore((state) => state.similarPassagesCache[passageKey]);
        const crossReferences = passageCardData?.crossReferences ?? [];
        const passageVerseIds = useMemo(
            () => new Set(passageBottomSheet.passage.verses.map((verse) => verse.id)),
            [passageBottomSheet]
        );
        const collections: Collection[] = useCollectionsStore(useShallow((state) => 
            state.userCollections.filter((col) => 
            col.items.some((i) =>
                i.type === 'passage' && i.passage.verses.some((verse) => passageVerseIds.has(verse.id))
            ))));

        return (
            <TrueSheet
                ref={ref}
                detents={[0.75, 1]}
                onDidDismiss={handlePassageSheetDidDismiss}
                onDidPresent={() => {}}
                style={{backgroundColor: theme.colors.background}}
                scrollable
           >
                <ScrollView
                    style={styles.scrollView}
                    contentContainerStyle={styles.sheetContainer}
                    showsVerticalScrollIndicator={false}
                    onScroll={(e) => {
                        const currentY = e.nativeEvent.contentOffset.y;
                        const targetY = 300;
                    }}
                >
                    {getLastPassage() && (
                        <TouchableOpacity onPress={() => {
                            goToLastPassage();
                        }}>
                            <Text style={[globalStyles.p3, globalStyles.linkButtonText]}>
                                Back to {getLastPassage()?.passage.reference.readableReference}
                            </Text>
                        </TouchableOpacity>
                    )}
                    <View style={styles.verse}>
                        <Text style={globalStyles.verseReference}>
                            {passageBottomSheet.passage.reference.readableReference}
                        </Text>
                        <View style={styles.versesContainer}>
                            <Text style={globalStyles.verseText}>
                                {passageBottomSheet.passage.verses.map((verse, index) => (
                                    <Text key={verse.id}>
                                        {passageBottomSheet.passage.verses.length > 1 && (
                                            <Text style={styles.verseNumber}>
                                                {verse.reference.verses.at(0)}{' '}
                                            </Text>
                                        )}
                                        {verse.text}
                                        {index < passageBottomSheet.passage.verses.length - 1 ? ' ' : ''}
                                    </Text>
                                ))}
                            </Text>
                        </View>
                    </View>
                    <Categories
                        categories={passageBottomSheet.passage.verses.at(0)?.categories || []}
                        multiline
                    />
                    <PassageSheetMetadata
                        passage={passageBottomSheet}
                        loading={!passageCardData}
                        data={passageCardData ?? initialVerseCardResponse}
                    />

                    <View style={{height: 25}} />

                    <PassageSheetActions
                        passageBottomSheet={passageBottomSheet}
                    />

                    <View style={{height: 20}} />

                    <CrossReferences 
                        crossReferences={crossReferences}
                        loading={!passageCardData}
                    />

                    <Collections
                        collections={collections}
                        onCollectionPress={(collection) => {
                            const isCurrentCollection = isCurrentCollectionRoute(collection.id);
                            closePassages();

                            if (!isCurrentCollection) {
                                pushCollectionRoute(collection.id);
                            }
                        }}
                    />
                    
                    <Similar 
                        reference={passageBottomSheet.passage.reference.readableReference}
                        similarPassages={similarPassages}
                        isLoading={!similarPassages}
                    />

                </ScrollView>
            </TrueSheet>
        );
    }
);

export default PassageBottomSheet;