import { TrueSheet } from "@lodev09/react-native-true-sheet"
import { forwardRef, useRef } from "react";
import { Text } from "react-native";

interface CollectionsSortBottomSheetProps {
    currentOrder: number;
}

const CollectionsSortBottomSheet = forwardRef<TrueSheet, CollectionsSortBottomSheetProps>(
    ({currentOrder}, ref) => {
    return (
        <TrueSheet
            ref={ref}
            detents={[.45]}
        >
            <Text>Reorder</Text>
        </TrueSheet>
    )
})

export default CollectionsSortBottomSheet;