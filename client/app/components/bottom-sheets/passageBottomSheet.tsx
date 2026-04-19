import { TrueSheet } from "@lodev09/react-native-true-sheet";
import { forwardRef } from "react";
import { Text } from "react-native";
import { Passage } from "../../../types/passages/passage";
import useGlobalStyles from "../../styles/gobalStyles";
import useAppTheme from "../../theme";
import { useBottomSheetsStore } from "../../stores/bottomSheets.store";

const PassageBottomSheet = forwardRef<TrueSheet>(
    (_, ref) => {
        const styles = useGlobalStyles();
        const theme = useAppTheme();
        const {passageBottomSheet, setPassageSheetOpen} = useBottomSheetsStore();

        return (
            <TrueSheet ref={ref} detents={[0.5, 1]} onDidDismiss={() => setPassageSheetOpen(false)}>
                <Text style={styles.p3}>{passageBottomSheet.passage.reference.readableReference}</Text>
            </TrueSheet>
        )
    }
)

export default PassageBottomSheet;