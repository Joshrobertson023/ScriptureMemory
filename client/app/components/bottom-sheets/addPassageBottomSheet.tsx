import { forwardRef } from "react"
import { AddPassageScreen } from "../../screens/addPassage.screen"
import { TrueSheet } from "@lodev09/react-native-true-sheet"

const AddPassageBottomSheet = forwardRef<TrueSheet>(
    (_, ref) => {
        return (
            <TrueSheet
                ref={ref}
                detents={[1]}
                scrollable
                initialDetentIndex={0}
            >
                <AddPassageScreen />
            </TrueSheet>
        )
    }
)

export default AddPassageBottomSheet;