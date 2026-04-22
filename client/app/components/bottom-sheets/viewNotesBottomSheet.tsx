import { TrueSheet } from "@lodev09/react-native-true-sheet";
import { forwardRef } from "react";
import { useBottomSheetsStore } from "../../stores/bottomSheets.store";

const ViewNotesBottomSheet = forwardRef<TrueSheet>((_, ref) => {
    const { setViewNotesSheetOpen } = useBottomSheetsStore();

    return (
        <TrueSheet
            ref={ref}
            detents={[0.8, 1]}
            onDidDismiss={() => setViewNotesSheetOpen(false)}
        />
    );
});

export default ViewNotesBottomSheet;
