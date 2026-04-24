import { TrueSheet } from "@lodev09/react-native-true-sheet";
import { forwardRef, useEffect, useMemo, useState } from "react";
import { FlatList, StyleSheet, Text, TextInput, TouchableOpacity, View } from "react-native";
import Swipeable from "react-native-gesture-handler/ReanimatedSwipeable";
import { Pencil, Trash } from "lucide-react-native";
import { Note } from "../../../types/note";
import { useBottomSheetsStore } from "../../stores/bottomSheets.store";
import useGlobalStyles from "../../styles/gobalStyles";
import useAppTheme from "../../theme";

const ViewNotesBottomSheet = forwardRef<TrueSheet>((_, ref) => {
    const theme = useAppTheme();
    const globalStyles = useGlobalStyles();
    const { viewNotesBottomSheet, setViewNotesSheetOpen } = useBottomSheetsStore();

    const [editingNoteId, setEditingNoteId] = useState<number | null>(null);
    const [draftText, setDraftText] = useState("");

    const selectedPassage = viewNotesBottomSheet;
    const notes: Note[] = [];

    const styles = useMemo(() => StyleSheet.create({
        container: {
            flex: 1,
            paddingHorizontal: 15,
            paddingTop: 16,
            paddingBottom: 22,
        },
        title: {
            ...globalStyles.p2,
            fontWeight: 700,
            marginBottom: 10,
        },
        listContent: {
            paddingBottom: 14,
        },
        separator: {
            height: StyleSheet.hairlineWidth,
            backgroundColor: theme.colors.elevation2,
            marginVertical: 10,
        },
        noteText: {
            ...globalStyles.p3,
        },
        input: {
            ...globalStyles.input,
            marginBottom: 0,
            height: 120,
            textAlignVertical: 'top',
            paddingTop: 10,
        },
        actionRow: {
            marginTop: 10,
            flexDirection: 'row',
            gap: 8,
        },
        actionButton: {
            ...globalStyles.elevationButton,
            width: '32%',
            paddingHorizontal: 0,
        },
        rightAction: {
            justifyContent: 'center',
            alignItems: 'center',
            paddingHorizontal: 18,
            borderRadius: 10,
            marginLeft: 6,
        },
        rightActionEdit: {
            backgroundColor: '#6b6f7c',
        },
        rightActionDelete: {
            backgroundColor: '#E25D5D',
        },
        emptyState: {
            ...globalStyles.p3,
            color: theme.colors.onBackgroundSuperSoft,
            marginTop: 6,
        },
    }), [globalStyles, theme]);

    useEffect(() => {
        if (!editingNoteId || notes.length > 0) {
            return;
        }

        setEditingNoteId(null);
        setDraftText("");
    }, [editingNoteId, notes.length]);

    const closeSheet = () => {
        setEditingNoteId(null);
        setDraftText("");
        setViewNotesSheetOpen(false);
    };

    const handleStartEdit = (note: Note) => {
        setEditingNoteId(note.id);
        setDraftText(note.text);
    };

    const handleSave = (_note: Note) => {
        if (!draftText.trim()) {
            return;
        }

        setEditingNoteId(null);
        setDraftText("");
    };

    const handleCancel = () => {
        setEditingNoteId(null);
        setDraftText("");
    };

    const handleDelete = (noteId: number) => {
        if (editingNoteId === noteId) {
            handleCancel();
        }
    };

    const handleAddNote = () => {
        if (!draftText.trim()) {
            return;
        }

        handleCancel();
    };

    return (
        <TrueSheet
            ref={ref}
            detents={[0.8, 1]}
            scrollable
            style={{ backgroundColor: theme.colors.background }}
            onDidDismiss={closeSheet}
        >
            <View style={styles.container}>
                <Text style={styles.title}>{selectedPassage.passage.reference.readableReference} Notes</Text>

                <FlatList
                    data={notes}
                    keyExtractor={(item) => item.id.toString()}
                    contentContainerStyle={styles.listContent}
                    showsVerticalScrollIndicator={false}
                    ListEmptyComponent={<Text style={styles.emptyState}>No notes yet.</Text>}
                    ItemSeparatorComponent={() => <View style={styles.separator} />}
                    renderItem={({ item }) => {
                        const isEditing = editingNoteId === item.id;

                        if (isEditing) {
                            return (
                                <View>
                                    <TextInput
                                        value={draftText}
                                        onChangeText={setDraftText}
                                        multiline
                                        style={styles.input}
                                        placeholder="Edit note"
                                        placeholderTextColor={theme.colors.onBackgroundSuperSoft}
                                    />
                                    <View style={styles.actionRow}>
                                        <TouchableOpacity style={styles.actionButton} onPress={() => handleSave(item)}>
                                            <Text style={globalStyles.p3}>Save</Text>
                                        </TouchableOpacity>
                                        <TouchableOpacity style={styles.actionButton} onPress={handleCancel}>
                                            <Text style={globalStyles.p3}>Cancel</Text>
                                        </TouchableOpacity>
                                        <TouchableOpacity style={styles.actionButton} onPress={() => handleDelete(item.id)}>
                                            <Text style={globalStyles.p3}>Delete</Text>
                                        </TouchableOpacity>
                                    </View>
                                </View>
                            );
                        }

                        return (
                            <Swipeable
                                renderRightActions={() => (
                                    <>
                                        <TouchableOpacity
                                            style={[styles.rightAction, styles.rightActionEdit]}
                                            onPress={() => handleStartEdit(item)}
                                        >
                                            <Pencil size={20} color={theme.colors.background} />
                                        </TouchableOpacity>
                                        <TouchableOpacity
                                            style={[styles.rightAction, styles.rightActionDelete]}
                                            onPress={() => handleDelete(item.id)}
                                        >
                                            <Trash size={20} color={theme.colors.background} />
                                        </TouchableOpacity>
                                    </>
                                )}
                            >
                                <Text style={styles.noteText}>{item.text}</Text>
                            </Swipeable>
                        );
                    }}
                />

                {editingNoteId === null && (
                    <View>
                        <View style={styles.separator} />
                        <TextInput
                            value={draftText}
                            onChangeText={setDraftText}
                            multiline
                            style={styles.input}
                            placeholder="Add a note"
                            placeholderTextColor={theme.colors.onBackgroundSuperSoft}
                        />
                        <View style={styles.actionRow}>
                            <TouchableOpacity style={styles.actionButton} onPress={handleAddNote}>
                                <Text style={globalStyles.p3}>Save</Text>
                            </TouchableOpacity>
                            <TouchableOpacity style={styles.actionButton} onPress={handleCancel}>
                                <Text style={globalStyles.p3}>Cancel</Text>
                            </TouchableOpacity>
                        </View>
                    </View>
                )}
            </View>
        </TrueSheet>
    );
});

export default ViewNotesBottomSheet;
