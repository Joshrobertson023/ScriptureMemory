import { StyleSheet, Text, TouchableOpacity, View } from "react-native";
import { Note } from "../../../types/note";
import useGlobalStyles from "../../styles/gobalStyles";
import { GripVertical, Pencil, Trash2 } from "lucide-react-native";
import useAppTheme from "../../theme";
import { useMemo } from "react";
import { useIsActive, useReorderableDrag } from "react-native-reorderable-list";
import { useCollectionsStore } from "../../stores/collections.store";
import { useBottomSheetsStore } from "../../stores/bottomSheets.store";

interface CollectionNoteProps {
    note: Note;
    itemId: number;
}

const CollectionNote = ({ note, itemId }: CollectionNoteProps) => {
    const globalStyles = useGlobalStyles();
    const theme = useAppTheme();
    const useLocalStyles = () => useMemo(() => StyleSheet.create({
        container: {
            maxWidth: '100%', flexDirection: 'row', alignItems: 'center', gap: 10
        },
        innerContainer: {
            width: '90%', flexDirection: 'column', minHeight: 75
        },
        icons: {
            justifyContent: 'flex-start', alignItems: 'flex-start', height: '100%', width: 50, gap: 10
        }
    }), [theme])
    const styles = useLocalStyles();
    const drag = useReorderableDrag();
    const isActive = useIsActive();
    const {setNoteBottomSheet, setNoteSheetOpen} = useBottomSheetsStore();

    return (
        <View style={styles.container}>
            <View style={styles.innerContainer}>
                <Text style={globalStyles.p3}>{note.text}</Text>
            </View>

            
            <View style={[styles.icons]}>
                <TouchableOpacity onLongPress={drag} disabled={isActive}>
                    <GripVertical size={20} color={theme.colors.onBackground} />
                </TouchableOpacity>
                <TouchableOpacity onPress={() => {
                    setNoteBottomSheet(note, itemId);
                    setNoteSheetOpen(true);
                }}>
                    <Pencil size={20} color={theme.colors.onBackground} />
                </TouchableOpacity>
            </View>
        </View>
    );
};

export default CollectionNote;
