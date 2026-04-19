import { forwardRef } from "react"
import { AddPassageScreen } from "../../screens/addPassage.screen"
import { TrueSheet } from "@lodev09/react-native-true-sheet"
import { useBottomSheetsStore } from "../../stores/bottomSheets.store"

const AddPassageBottomSheet = forwardRef<TrueSheet>(
    (_, ref) => {
        const {setPassageSheetOpen} = useBottomSheetsStore();
        return (
            <TrueSheet
                ref={ref}
                detents={[1]}
                scrollable
                onDidDismiss={() => setPassageSheetOpen(false)}
            >
                <AddPassageScreen />
            </TrueSheet>
        )
    }
)

export default AddPassageBottomSheet;