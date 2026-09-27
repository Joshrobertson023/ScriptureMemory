import { PressableFeedback } from "heroui-native";
import React from "react";
import { StyleSheet, View } from "react-native";
import { useReorderableDrag } from "react-native-reorderable-list";
import { UserPassage } from "../../../types/passages/userPassage";
import { useBottomSheetStack } from "../../hooks/useBottomSheetStack";
import { useBottomSheetsStore } from "../../stores/bottomSheets.store";
import useAppTheme from "../../theme";
import PassageContent from "./passageContent";

interface PassageProps {
    userPassage: UserPassage;
    itemId: string;
    collectionId: string;
    onRemove?: (itemId: string) => void;
    reordering?: boolean;
}

const useLocalStyles = () => StyleSheet.create({
    container: {
        maxWidth: '100%', flexDirection: 'row', alignItems: 'center'
    },
    content: {
        flex: 1
    },
    menuButton: {
        padding: 8
    }
})

const PassageComponent = React.memo(({userPassage, itemId, collectionId, onRemove, reordering = false}: PassageProps) => {
    const theme = useAppTheme();
    const drag = useReorderableDrag();
    const styles = useLocalStyles();
    const setPassageSheetOpen = useBottomSheetsStore((state) => state.setPassageSheetOpen);
    const setPassageBottomSheet = useBottomSheetsStore((state) => state.setPassageBottomSheet);
    const pushPassage = useBottomSheetsStore((state) => state.pushPassage);
    const { goToNextPassage } = useBottomSheetStack();
    const passage = userPassage.passage;

    if (!passage) {
        return null;
    }

    return (
        <PressableFeedback onLongPress={reordering ? drag : undefined} delayLongPress={150} onPress={reordering ? undefined : () => {
                    const selectedUserPassage: UserPassage = userPassage;
                    const stackLength = useBottomSheetsStore.getState().passageSheetStack.length;
        
                    if (stackLength === 0) {
                        pushPassage(selectedUserPassage);
                        setPassageBottomSheet(selectedUserPassage);
                        setPassageSheetOpen(true);
                        return;
                    }
        
                    goToNextPassage(selectedUserPassage);
                }}
                style={{display: 'flex', flexDirection: 'row', alignItems: 'center'}}
                animation={{
                        scale: {
                            value: 0.99,
                            timingConfig: {
                                duration: 400,
                            },
                            ignoreScaleCoefficient: true,
                        },
                    }}
                >
                <View style={{display: 'flex', flexDirection: 'row', alignItems: 'center', margin: 10}}>
                    <View>
                        <PassageContent
                            userPassage={userPassage}
                            onRemove={onRemove}
                        />
                    </View>

                </View>
                <PressableFeedback.Ripple
                    styles={{
                        container: {
                            borderRadius: 20,
                            overflow: 'hidden',
                        },
                        ripple: {
                            borderRadius: 999,
                        },
                    }}
                    animation={{
                        backgroundColor: {
                            value: theme.colors.onBackground,
                        },
                        opacity: {
                            value: [0, 0.15, 0],
                        },
                        progress: {
                            baseDuration: 200,
                        },
                    }}
                />
        </PressableFeedback>
    )
});

export default PassageComponent;
