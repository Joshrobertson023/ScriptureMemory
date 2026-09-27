import { useMappingHelper } from "@shopify/flash-list";
import { Check } from "lucide-react-native";
import React, { useMemo } from "react";
import { DimensionValue, StyleProp, StyleSheet, Text, TouchableWithoutFeedback, View, ViewStyle } from "react-native";
import { Passage } from "../../../types/passages/passage";
import { UserPassage } from "../../../types/passages/userPassage";
import { useBibleVersion } from "../../hooks/useBibleVersion";
import { useBottomSheetStack } from "../../hooks/useBottomSheetStack";
import { useCollectionsContainingVerses } from "../../hooks/useCollections";
import { useBottomSheetsStore } from "../../stores/bottomSheets.store";
import useGlobalStyles from "../../styles/gobalStyles";
import useAppTheme from "../../theme";

interface PassageContentProps {
    passage: Passage;
    userPassageId?: string;
    style?: StyleProp<ViewStyle>;
    maxWidth?: DimensionValue;
}

const useLocalStyles = () => {
    return StyleSheet.create({
        container: {
            maxWidth: '100%'
        },
        row1: {
            flexDirection: 'row', justifyContent: 'flex-start', alignItems: 'center', marginVertical: 10
        },
        row2: {
            flexDirection: 'row', justifyContent: 'center', alignItems: 'center', gap: 7
        }
    })
}

const AddPassageContent = React.memo(({passage, userPassageId, maxWidth}: PassageContentProps) => {
    const styles = useGlobalStyles();
    const localStyles = useLocalStyles();
    const theme = useAppTheme();
    const { version: bibleVersion } = useBibleVersion();
    const setPassageSheetOpen = useBottomSheetsStore((state) => state.setPassageSheetOpen);
    const setPassageBottomSheet = useBottomSheetsStore((state) => state.setPassageBottomSheet);
    const pushPassage = useBottomSheetsStore((state) => state.pushPassage);
    const { goToNextPassage } = useBottomSheetStack();
    const displayedVersion = passage.verses[0]?.translationContents?.at(0)?.version;

    const passageVerseIds = useMemo(
        () => new Set(passage.verses.map((verse) => verse.id)),
        [passage]
    );
    const collectionsCount = useCollectionsContainingVerses(passageVerseIds).length;

    const { getMappingKey } = useMappingHelper();

    return (
        <TouchableWithoutFeedback onPress={() => {
            const userPassage: UserPassage = {
                passage: passage,
                id: userPassageId,
            }
            if (useBottomSheetsStore.getState().passageSheetStack.length > 0) {
                goToNextPassage(userPassage);
                return;
            }
            pushPassage(userPassage);
            setPassageBottomSheet(userPassage);
            setPassageSheetOpen(true);
        }}>
            <View style={[localStyles.container]}>
                <Text style={{...styles.p3, fontWeight: 600}}>{passage.reference.readableReference}</Text>
                <View>
                    {passage.verses.map((verse, index) => (
                        <Text key={getMappingKey(verse.id, index)} style={styles.p3}>
                            {passage.verses.length > 1 && (verse.reference.verses.at(0) + ": ")}{verse.translationContents?.at(0)?.plainText}
                        </Text>
                    ))}
                </View>
                {displayedVersion && (
                    <Text style={styles.verseVersionLabel}>{displayedVersion.toUpperCase()}</Text>
                )}

                <View style={[localStyles.row1]}>
                    <View style={[localStyles.row2]}>
                    </View>
                    <View style={[localStyles.row2]}>
                        {collectionsCount > 0 && <Check size={16} color={theme.colors.onBackground} />}
                        <Text style={styles.p4}>
                            In {collectionsCount} {collectionsCount === 1 ? 'Collection' : 'Collections'}
                        </Text>
                    </View>
                </View>
            </View>
        </TouchableWithoutFeedback>
    )
}
)

export default AddPassageContent;