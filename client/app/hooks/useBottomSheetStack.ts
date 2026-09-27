import { UserPassage } from "../../types/passages/userPassage";
import { useBottomSheetsStore } from "../stores/bottomSheets.store";

const reopenPassageSheet = () => {
    setTimeout(() => {
        useBottomSheetsStore.getState().setPassageSheetOpen(true);
    }, 0);
};

const goToNextPassage = (up: UserPassage) => {
    const { setPassageSheetPendingTransition, pushPassage, setPassageBottomSheet, setPassageSheetOpen } = useBottomSheetsStore.getState();
    setPassageSheetPendingTransition({ kind: "next", passage: up });
    pushPassage(up);
    setPassageBottomSheet(up);
    setPassageSheetOpen(false);
}

const goToLastPassage = () => {
    const { setPassageSheetPendingTransition, popPassage, setBottomPassageLastInStack, setPassageSheetOpen } = useBottomSheetsStore.getState();
    setPassageSheetPendingTransition({ kind: "last" });
    popPassage();
    setBottomPassageLastInStack();
    setPassageSheetOpen(false);
}

const closePassages = () => {
    const { setPassageSheetPendingTransition, setPassageSheetOpen, clearStack } = useBottomSheetsStore.getState();
    setPassageSheetPendingTransition(null);
    setPassageSheetOpen(false);
    clearStack();
}

const handlePassageSheetDidDismiss = () => {
    const { passageSheetPendingTransition, setPassageSheetPendingTransition, setPassageSheetOpen, clearStack } = useBottomSheetsStore.getState();

    if (passageSheetPendingTransition) {
        setPassageSheetPendingTransition(null);
        reopenPassageSheet();
        return;
    }

    setPassageSheetOpen(false);
    clearStack();
}

const bottomSheetStack = {
    goToNextPassage,
    goToLastPassage,
    closePassages,
    handlePassageSheetDidDismiss,
};

export const useBottomSheetStack = () => bottomSheetStack;
