import { TouchableWithoutFeedback, Text, View, StyleProp, ViewStyle, StyleSheet, DimensionValue } from "react-native"
import { Passage } from "../../../types/passages/passage"
import useGlobalStyles from "../../styles/gobalStyles";
import useAppTheme from "../../theme";
import { TrueSheet } from "@lodev09/react-native-true-sheet";
import { useEffect, useRef, useState } from "react";
import PassageBottomSheet from "../bottom-sheets/passageBottomSheet";
import { useBottomSheetsStore } from "../../stores/bottomSheets.store";
import { UserPassage } from "../../../types/passages/userPassage";
import Categories from "./categories";
import { Category } from "../../../types/category";
import React from "react";
import { Check } from "lucide-react-native";

interface PassageContentProps {
    passage: Passage;
    style?: StyleProp<ViewStyle>;
    maxWidth?: DimensionValue;
}

const useLocalStyles = () => {
    return StyleSheet.create({
        container: {
            
        },
        row1: {
            flexDirection: 'row', justifyContent: 'flex-start', alignItems: 'center', marginVertical: 10
        },
        row2: {
            flexDirection: 'row', justifyContent: 'center', alignItems: 'center', gap: 7
        }
    })
}

const PassageContent = React.memo(({passage, maxWidth}: PassageContentProps) => {
    const styles = useGlobalStyles();
    const localStyles = useLocalStyles();
    const theme = useAppTheme();
    const {setPassageSheetOpen, setPassageBottomSheet} = useBottomSheetsStore();

    const allCategories = React.useMemo(() =>
        Array.from(new Map(passage.verses.flatMap(v => v.categories).map(c => [c.id, c])).values()),
        [passage]
    );

    return (
        <TouchableWithoutFeedback onPress={() => {
            const userPassage: UserPassage = {
                passage: passage,
            }
            setPassageBottomSheet(userPassage);
            setPassageSheetOpen(true);
        }}>
            <View style={[localStyles.container, {maxWidth}]}>
                <Text style={{...styles.p3, fontWeight: 600}}>{passage.reference.readableReference}</Text>
                <View>
                    {passage.verses.map((verse, index) => (
                        <Text key={verse.id} style={styles.p3}>
                            {passage.verses.length > 1 && (verse.reference.verses.at(0) + ": ")}{verse.text}
                        </Text>
                    ))}
                </View>
                
                <View style={[localStyles.row1]}>
                    <View style={[localStyles.row2]}>
                    </View>
                    <View style={[localStyles.row2]}>
                        <Check size={16} color={theme.colors.onBackground} />
                        <Text style={styles.p4}>In 1 Collection</Text>
                    </View>
                </View>

                <Categories categories={allCategories} multiline={false} />
            </View>
        </TouchableWithoutFeedback>
    )
}
)

export default PassageContent;