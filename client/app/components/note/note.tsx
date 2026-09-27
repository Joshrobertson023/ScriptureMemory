import { PressableFeedback } from "heroui-native";
import { StyleSheet, Text, TouchableOpacity, View } from "react-native";
import { Note } from "../../../types/note";
import useGlobalStyles from "../../styles/gobalStyles";
import { Pencil } from "lucide-react-native";
import useAppTheme from "../../theme";
import { useEffect, useRef } from "react";
import { useIsActive, useReorderableDrag } from "react-native-reorderable-list";
import { useBottomSheetsStore } from "../../stores/bottomSheets.store";
import Swipeable, { SwipeableMethods } from 'react-native-gesture-handler/ReanimatedSwipeable';
import React from "react";

interface NoteProps {
    note: Note;
    itemId: string;
    reordering?: boolean;
}

const useLocalStyles = () => StyleSheet.create({
    container: {
        maxWidth: '100%', flexDirection: 'row', alignItems: 'center'
    },
    content: {
        flex: 1
    },
    sideEdit: {
        backgroundColor: '#A9A9A9',
        justifyContent: 'center',
        alignItems: 'center',
        padding: 20,
        marginTop: 10,
        borderRadius: 10,
        marginLeft: 5
    }
})

const NoteComponent = React.memo(({ note, itemId, reordering = false }: NoteProps) => {
    const globalStyles = useGlobalStyles();
    const theme = useAppTheme();
    const styles = useLocalStyles();
    const drag = useReorderableDrag();
    const isActive = useIsActive();
    const setNoteBottomSheet = useBottomSheetsStore((state) => state.setNoteBottomSheet);
    const setNoteSheetOpen = useBottomSheetsStore((state) => state.setNoteSheetOpen);
    const noteSheetOpen = useBottomSheetsStore((state) => state.noteSheetOpen);
    const swipeable = useRef<SwipeableMethods>(null);

    useEffect(() => {
        if (!noteSheetOpen)
            swipeable.current?.close();
    }, [noteSheetOpen]);

    const RightActions = () => (
        <TouchableOpacity
            style={styles.sideEdit}
            onPress={() => {
                setNoteBottomSheet(note, itemId);
                setNoteSheetOpen(true);
            }}
        >
            <Pencil size={25} color={theme.colors.background} />
        </TouchableOpacity>
    );

    return (
        <Swipeable ref={swipeable} enabled={!reordering} renderRightActions={() => <RightActions />}>
            <PressableFeedback
                onLongPress={drag}
                isDisabled={isActive}
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
                    <View style={styles.content}>
                        <Text style={globalStyles.p3}>{note.text}</Text>
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
        </Swipeable>
    );
});

export default NoteComponent;
