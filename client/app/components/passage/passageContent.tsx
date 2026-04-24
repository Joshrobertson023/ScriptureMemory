import { TouchableWithoutFeedback, Text, View, StyleProp, ViewStyle, StyleSheet, DimensionValue } from "react-native"
import useGlobalStyles from "../../styles/gobalStyles";
import useAppTheme from "../../theme";
import { useBottomSheetsStore } from "../../stores/bottomSheets.store";
import { UserPassage } from "../../../types/passages/userPassage";
import Categories from "./categories";
import React from "react";
import { useBottomSheetStack } from "../../hooks/useBottomSheetStack";

interface PassageContentProps {
    userPassage: UserPassage;
    style?: StyleProp<ViewStyle>;
    maxWidth?: DimensionValue;
    onLongPress?: () => void;
    disabled?: boolean;
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

const PassageContent = React.memo(({userPassage, maxWidth, onLongPress, disabled}: PassageContentProps) => {
    const styles = useGlobalStyles();
    const localStyles = useLocalStyles();
    const theme = useAppTheme();
    const { setPassageSheetOpen, setPassageBottomSheet, pushPassage, passageSheetStack } = useBottomSheetsStore();
    const { goToNextPassage } = useBottomSheetStack();
    const passage = userPassage.passage;

    if (!passage) {
        return null;
    }

    const allCategories = React.useMemo(() =>
        Array.from(new Map(passage.verses.flatMap(v => v.categories).map(c => [c.id, c])).values()),
        [passage]
    );

    return (
        <TouchableWithoutFeedback onLongPress={onLongPress} disabled={disabled} onPress={() => {
            const selectedUserPassage: UserPassage = userPassage;

            if (passageSheetStack.length === 0) {
                pushPassage(selectedUserPassage);
                setPassageBottomSheet(selectedUserPassage);
                setPassageSheetOpen(true);
                return;
            }

            goToNextPassage(selectedUserPassage);
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
                </View>

                <Categories categories={allCategories} multiline={false} />
            </View>
        </TouchableWithoutFeedback>
    )
}
)

export default PassageContent;