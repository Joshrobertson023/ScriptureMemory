import { TrueSheet } from "@lodev09/react-native-true-sheet";
import { forwardRef, Fragment, useMemo, useEffect } from "react";
import { StyleSheet, Text, TouchableOpacity, View, BackHandler } from "react-native";
import useGlobalStyles from "../../styles/gobalStyles";
import useAppTheme from "../../theme";
import { getPassageCacheKey, useBottomSheetsStore } from "../../stores/bottomSheets.store";
import { initialVerseCardResponse } from "../../../types/verse/verseCard";
import Categories from "../passage/categories";
import PassageSheetMetadata from "../passage/passageSheetMetadata";
import CrossReferences from "../passage/crossReferences";

interface PassageBottomSheetProps {
    canGoBack?: boolean;
}

const PassageBottomSheet = forwardRef<TrueSheet, PassageBottomSheetProps>(
    ({ canGoBack = false }, ref) => {
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
                paddingTop: 25
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
            setPassageSheetOpen,
            passageBottomSheet2,
            setPassageSheet2Open,
            setViewNotesSheetOpen,
            setSaveToCollectionSheetOpen
        } = useBottomSheetsStore();

        const activePassage = canGoBack ? passageBottomSheet2 : passageBottomSheet;
        const passageKey = useMemo(() => getPassageCacheKey(activePassage), [activePassage]);
        const passageCardData = useBottomSheetsStore((state) => state.passageCardCache[passageKey]);
        const crossReferences = passageCardData?.crossReferences ?? [];

        useEffect(() => {
            if (!canGoBack) return;

            const backHandler = BackHandler.addEventListener('hardwareBackPress', () => {
                setPassageSheet2Open(false);
                setPassageSheetOpen(true);
                return true;
            });

            return () => backHandler.remove();
        }, [canGoBack, setPassageSheet2Open, setPassageSheetOpen]);

        return (
            <TrueSheet
                ref={ref}
                detents={[0.5, 1]}
                onDidDismiss={() => {
                    if (canGoBack) {
                        setPassageSheet2Open(false);
                        setPassageSheetOpen(true);
                        return;
                    }

                    setPassageSheetOpen(false);
                }}
                onDidPresent={() => canGoBack ? setPassageSheet2Open(true) : setPassageSheetOpen(true)}
                style={{backgroundColor: theme.colors.background}}
                scrollable
           >
                <View style={styles.sheetContainer}>
                    {canGoBack && (
                        <TouchableOpacity onPress={() => {
                            setPassageSheet2Open(false);
                            setPassageSheetOpen(true);
                        }}>
                            <Text style={[globalStyles.p3, globalStyles.linkButtonText]}>
                                Back to {passageBottomSheet.passage.reference.readableReference}
                            </Text>
                        </TouchableOpacity>
                    )}
                    <View style={styles.verse}>
                        <Text style={globalStyles.verseReference}>
                            {activePassage.passage.reference.readableReference}
                        </Text>
                        <View style={styles.versesContainer}>
                            <Text style={globalStyles.verseText}>
                                {activePassage.passage.verses.map((verse, index) => (
                                    <Text key={verse.id}>
                                        {activePassage.passage.verses.length > 1 && (
                                            <Text style={styles.verseNumber}>
                                                {verse.reference.verses.at(0)}{' '}
                                            </Text>
                                        )}
                                        {verse.text}
                                        {index < activePassage.passage.verses.length - 1 ? ' ' : ''}
                                    </Text>
                                ))}
                            </Text>
                        </View>
                    </View>
                    <Categories
                        categories={activePassage.passage.verses.at(0)?.categories || []}
                        multiline
                    />
                    <PassageSheetMetadata
                        passage={activePassage}
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
                        canGoBack={canGoBack} />
                </View>
            </TrueSheet>
        );
    }
);

export default PassageBottomSheet;