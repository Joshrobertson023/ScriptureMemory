import { TrueSheet } from "@lodev09/react-native-true-sheet";
import { forwardRef } from "react";
import { Text } from "react-native";

interface SyncBottomSheetProps {
    syncStatus: number;
}

const SyncBottomSheet = forwardRef<TrueSheet, SyncBottomSheetProps>(
    ({syncStatus}, ref) => {
        <TrueSheet
            ref={ref}
            detents={[.45]}
        >
            <Text>Sync status: {syncStatus}</Text>
        </TrueSheet>
    }
)

export default SyncBottomSheet;