import { StyleSheet, Text, TouchableOpacity, TouchableWithoutFeedback, View } from "react-native";
import { Note } from "../../../types/note";
import useGlobalStyles from "../../styles/gobalStyles";
import { Pencil } from "lucide-react-native";
import useAppTheme from "../../theme";
import { useMemo } from "react";
import { useIsActive, useReorderableDrag } from "react-native-reorderable-list";
import { useBottomSheetsStore } from "../../stores/bottomSheets.store";
import Swipeable from 'react-native-gesture-handler/ReanimatedSwipeable';

interface NoteProps {
    note: Note;
    itemId: number;
}

const NoteComponent = ({ note, itemId }: NoteProps) => {
    const globalStyles = useGlobalStyles();
    const theme = useAppTheme();
    const useLocalStyles = () => useMemo(() => StyleSheet.create({
        container: {
            maxWidth: '100%', flexDirection: 'column', minHeight: 75
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
    }), [theme])
    const styles = useLocalStyles();
    const drag = useReorderableDrag();
    const isActive = useIsActive();
    const {setNoteBottomSheet, setNoteSheetOpen} = useBottomSheetsStore();

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
        <Swipeable renderRightActions={() => <RightActions />}>
            <TouchableWithoutFeedback onLongPress={drag} disabled={isActive}>
                <View style={styles.container}>
                    <Text style={globalStyles.p3}>{note.text}</Text>
                </View>
            </TouchableWithoutFeedback>
        </Swipeable>
    );
};

export default NoteComponent;
