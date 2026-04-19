import { TrueSheet } from "@lodev09/react-native-true-sheet";
import { forwardRef } from "react";
import { Text } from "react-native";
import { useAppStore } from "../../stores/appState.store";

const SyncBottomSheet = forwardRef<TrueSheet>(
    (_, ref) => {
        const {syncStatus} = useAppStore();

        return (
            <TrueSheet
                ref={ref}
                detents={[0.45]}
            >
                <Text>Sync status: {syncStatus}</Text>
            </TrueSheet>
        )
    }
);

export default SyncBottomSheet;