import { forwardRef } from "react"
import { AddPassageScreen } from "../../screens/addPassage.screen"
import { TrueSheet } from "@lodev09/react-native-true-sheet"
import { useBottomSheetsStore } from "../../stores/bottomSheets.store"
import { Passage } from "../../../types/passages/passage"
import { CollectionItem } from "../../../types/collection/collectionItem"

interface AddPassageBottomSheetProps {
    collectionItems: CollectionItem[];
    savePassage: (passage: Passage) => void;
    removePassage: (itemId: number) => void;
}

const AddPassageBottomSheet = forwardRef<TrueSheet, AddPassageBottomSheetProps>(
    ({ collectionItems, savePassage, removePassage }: AddPassageBottomSheetProps, ref) => {
        const {setPassageSheetOpen} = useBottomSheetsStore();
        return (
            <TrueSheet
                ref={ref}
                detents={[1]}
                scrollable
                onDidDismiss={() => setPassageSheetOpen(false)}
            >
                <AddPassageScreen
                    collectionItems={collectionItems}
                    savePassage={savePassage}
                    removePassage={removePassage}
                />
            </TrueSheet>
        )
    }
)

export default AddPassageBottomSheet;