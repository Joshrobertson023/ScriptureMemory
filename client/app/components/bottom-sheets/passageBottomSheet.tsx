import { TrueSheet } from "@lodev09/react-native-true-sheet";
import { forwardRef, useMemo, useEffect } from "react";
import { StyleSheet, Text, TouchableOpacity, View, BackHandler, ScrollView } from "react-native";
import useGlobalStyles from "../../styles/gobalStyles";
import useAppTheme from "../../theme";
import { getPassageCacheKey, useBottomSheetsStore } from "../../stores/bottomSheets.store";
import { initialVerseCardResponse } from "../../../types/verse/verseCard";
import Categories from "../passage/categories";
import PassageSheetMetadata from "../passage/passageSheetMetadata";
import CrossReferences from "../passage/crossReferences";
import { useBottomSheetStack } from "../../hooks/useBottomSheetStack";

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
            setViewNotesSheetOpen,
            setSaveToCollectionSheetOpen
        } = useBottomSheetsStore();
        const {
            goToNextPassage,
            goToLastPassage,
            handlePassageSheetDidDismiss,
            getLastPassage
        } = useBottomSheetStack();
        const passageKey = useMemo(() => getPassageCacheKey(passageBottomSheet), [passageBottomSheet]);
        const passageCardData = useBottomSheetsStore((state) => state.passageCardCache[passageKey]);
        const crossReferences = passageCardData?.crossReferences ?? [];

        return (
            <TrueSheet
                ref={ref}
                detents={[0.5, 1]}
                onDidDismiss={handlePassageSheetDidDismiss}
                onDidPresent={() => {}}
                style={{backgroundColor: theme.colors.background}}
                scrollable
           >
                <ScrollView
                    style={styles.scrollView}
                    contentContainerStyle={styles.sheetContainer}
                    showsVerticalScrollIndicator={false}
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
                    <View style={{height: 20}} />
                    <TouchableOpacity style={globalStyles.elevationButton} onPress={() => {
                        setViewNotesSheetOpen(true);
                    }}>
                        <Text style={globalStyles.p3}>
                            Notes
                        </Text>
                    </TouchableOpacity>
                    <View style={{height: 10}} />
                    <TouchableOpacity style={globalStyles.elevationButton} onPress={() => {
                        setSaveToCollectionSheetOpen(true);
                    }}>
                        <Text style={globalStyles.p3}>
                            Add to Collection
                        </Text>
                    </TouchableOpacity>

                    <View style={{height: 20}} />
                    <CrossReferences 
                        crossReferences={crossReferences}
                        loading={!passageCardData}
                    />
                </ScrollView>
            </TrueSheet>
        );
    }
);

export default PassageBottomSheet;