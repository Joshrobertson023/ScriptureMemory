import { TrueSheet } from "@lodev09/react-native-true-sheet";
import { forwardRef } from "react";
import { useBottomSheetsStore } from "../../stores/bottomSheets.store";

const SaveToCollectionBottomSheet = forwardRef<TrueSheet>((_, ref) => {
    const { setSaveToCollectionSheetOpen } = useBottomSheetsStore();

    return (
        <TrueSheet
            ref={ref}
            detents={[0.8, 1]}
            onDidDismiss={() => setSaveToCollectionSheetOpen(false)}
        />
    );
});

export default SaveToCollectionBottomSheet;
