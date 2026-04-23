import { UserPassage } from "../../types/passages/userPassage"
import { useBottomSheetsStore } from "../stores/bottomSheets.store";

export const useBottomSheetStack = () => {
    const {
        pushPassage, 
        popPassage, 
        setBottomPassageLastInStack, 
        clearStack, 
        setPassageBottomSheet, 
        setPassageSheetOpen,
        setPassageSheetPendingTransition,
        passageSheetPendingTransition,
        passageSheetStack
    } = useBottomSheetsStore();

    const reopenPassageSheet = () => {
        setTimeout(() => {
            setPassageSheetOpen(true);
        }, 0);
    };

    const goToNextPassage = (up: UserPassage) => {
        setPassageSheetPendingTransition({ kind: "next", passage: up });
        pushPassage(up);
        setPassageBottomSheet(up);
        setPassageSheetOpen(false);
    }

    const goToLastPassage = () => {
        setPassageSheetPendingTransition({ kind: "last" });
        popPassage();
        setBottomPassageLastInStack();
        setPassageSheetOpen(false);
    }

    const closePassages = () => {
        setPassageSheetPendingTransition(null);
        setPassageSheetOpen(false);
        clearStack();
    }

    const handlePassageSheetDidDismiss = () => {
        const transition = passageSheetPendingTransition;

        if (transition?.kind === "next") {
            setPassageSheetPendingTransition(null);
            reopenPassageSheet();
            return;
        }

        if (transition?.kind === "last") {
            setPassageSheetPendingTransition(null);
            reopenPassageSheet();
            return;
        }

        setPassageSheetOpen(false);
        clearStack();
    }

    const getLastPassage = () => {
        if (passageSheetStack.length > 1) {
            return passageSheetStack.at(passageSheetStack.length - 2);
        }
        return undefined;
    }

    return {
        goToNextPassage,
        goToLastPassage,
        closePassages,
        handlePassageSheetDidDismiss,
        getLastPassage
    };
}