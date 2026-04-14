import { TrueSheet } from "@lodev09/react-native-true-sheet"
import React, { forwardRef, useRef } from "react";
import { Text, TouchableHighlight, View } from "react-native";
import useGlobalStyles from "../../styles/gobalStyles";
import { SORT_OPTIONS, VISIBILITY_OPTIONS } from "../../utils/collectionSortUtils";
import { Check, CircleCheck } from "lucide-react-native";
import useAppTheme from "../../theme";

interface VisibilityBottomSheetProps {
    currentVisibility: number;
}

const VisibilityBottomSheet = forwardRef<TrueSheet, VisibilityBottomSheetProps>(
    ({currentVisibility}, ref) => {
    const styles = useGlobalStyles();
    const theme = useAppTheme();

    const selectOption = (option: number) => {

    }

    return (
        <TrueSheet
            ref={ref}
            detents={[.45]}
        >
            <View style={styles.bottomSheetContainer}>
                {VISIBILITY_OPTIONS.map((option) => {
                    return (
                        <TouchableHighlight
                            key={option.value}
                            onPress={() => selectOption(option.value)}>
                                <View>
                                    <Text style={styles.p2}>{option.label}</Text>
                                    {option.value === currentVisibility && (
                                        <CircleCheck size={22} color={theme.colors.onBackground} />
                                    )}
                                </View>
                        </TouchableHighlight>
                    )
                })}
            </View>
        </TrueSheet>
    )
})

export default VisibilityBottomSheet;