import { TrueSheet } from "@lodev09/react-native-true-sheet";
import { forwardRef } from "react";
import { Text } from "react-native";
import { useAppStore } from "../../stores/appState.store";
import { useBottomSheetsStore } from "../../stores/bottomSheets.store";

const SyncBottomSheet = forwardRef<TrueSheet>(
    (_, ref) => {
        const {syncStatus} = useAppStore();
        const {setSyncSheetOpen} = useBottomSheetsStore();

        return (
            <TrueSheet
                ref={ref}
                detents={[0.45]}
                onDidDismiss={() => setSyncSheetOpen(false)}
            >
                <Text>Sync status: {syncStatus}</Text>
            </TrueSheet>
        )
    }
);

export default SyncBottomSheet;