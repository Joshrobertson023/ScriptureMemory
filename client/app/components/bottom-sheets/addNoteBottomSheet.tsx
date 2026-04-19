import { TrueSheet } from "@lodev09/react-native-true-sheet";
import { forwardRef, useEffect, useMemo, useState } from "react";
import { StyleSheet, Text, TextInput, TouchableOpacity, View } from 'react-native';
import useAppTheme from "../../theme";
import useGlobalStyles from "../../styles/gobalStyles";
import { useBottomSheetsStore } from "../../stores/bottomSheets.store";

interface AddNoteBottomSheetProps {
    onSave: (text: string, itemId: number | null) => void;
    onDelete: (itemId: number | null) => void;
}

const AddNoteBottomSheet = forwardRef<TrueSheet, AddNoteBottomSheetProps>(
    ({ onSave, onDelete }, ref) => {
        const theme = useAppTheme();
        const globalStyles = useGlobalStyles();
        const useLocalStyles = () => useMemo(() => StyleSheet.create({
            input: {
                width: '100%',
                height: 200,
                alignItems: 'flex-start',
                textAlignVertical: 'top'
            },
            conatiner: {
                height: 225,
                alignItems: 'flex-start'
            },
            buttonRow: {
                flexDirection: 'row',
                width: '100%',
                justifyContent: 'space-between',
                gap: 10
            },
            button: {
                width: '32%'
            }
        }), [theme])
        const styles = useLocalStyles();

        const [noteText, setNoteText] = useState('');
        const {
            noteBottomSheet,
            noteBottomSheetItemId,
            setNoteSheetOpen,
            clearNoteBottomSheet,
        } = useBottomSheetsStore();

        useEffect(() => {
            setNoteText(noteBottomSheet.text || '');
        }, [noteBottomSheet.text, noteBottomSheetItemId]);

        const closeSheet = () => {
            setNoteSheetOpen(false);
            clearNoteBottomSheet();
            setNoteText('');
        };

        const handleSave = () => {
            if (!noteText.trim()) {
                closeSheet();
                return;
            }

            onSave(noteText.trim(), noteBottomSheetItemId);
            closeSheet();
        };

        const handleDelete = () => {
            onDelete(noteBottomSheetItemId);
            closeSheet();
        };


        return (
            <TrueSheet
                ref={ref}
                detents={[.45, 1]}
                scrollable
                onDidDismiss={closeSheet}
            >
                <View style={[globalStyles.bottomSheetContainer, styles.conatiner]}>
                    <TextInput 
                        value={noteText} 
                        placeholder="Enter text"
                        multiline 
                        style={[globalStyles.input, styles.input]} 
                        onChangeText={((text) => {setNoteText(text)})}    
                    />
                    <View style={styles.buttonRow}>
                        <TouchableOpacity style={[globalStyles.elevationButton, styles.button]} onPress={closeSheet}>
                            <Text style={globalStyles.p3}>Cancel</Text>
                        </TouchableOpacity>
                        <TouchableOpacity style={[globalStyles.elevationButton, styles.button]} onPress={handleDelete}>
                            <Text style={globalStyles.p3}>Delete</Text>
                        </TouchableOpacity>
                        <TouchableOpacity style={[globalStyles.elevationButton, styles.button]} onPress={handleSave}>
                            <Text style={globalStyles.p3}>Save</Text>
                        </TouchableOpacity>
                    </View>
                </View>
            </TrueSheet>
        )
    }
)

export default AddNoteBottomSheet;