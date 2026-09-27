import React, { useEffect, useMemo, useState } from "react";
import { FlatList, KeyboardAvoidingView, Modal, Pressable, StyleSheet, Text, TextInput, TouchableOpacity, View } from "react-native";
import { GestureHandlerRootView } from "react-native-gesture-handler";
import Swipeable from "react-native-gesture-handler/ReanimatedSwipeable";
import { Pencil, Trash, X } from "lucide-react-native";
import { useLiveQuery } from "drizzle-orm/expo-sqlite";
import { Note } from "../../../types/note";
import { addPassageNote, deletePassageNote, getPassageNoteKey, passageNotesQuery, updatePassageNote } from "../../database/repositories/passageNotes.repository";
import { useBottomSheetsStore } from "../../stores/bottomSheets.store";
import { useUserStore } from "../../stores/user.store";
import useGlobalStyles from "../../styles/gobalStyles";
import useAppTheme from "../../theme";

const ViewNotesDialog = () => {
    const theme = useAppTheme();
    const globalStyles = useGlobalStyles();
    const isOpen = useBottomSheetsStore((state) => state.viewNotesSheetOpen);
    const selectedPassage = useBottomSheetsStore((state) => state.viewNotesBottomSheet);
    const setViewNotesSheetOpen = useBottomSheetsStore((state) => state.setViewNotesSheetOpen);

    const [editingNoteId, setEditingNoteId] = useState<string | null>(null);
    const [draftText, setDraftText] = useState("");

    const passageKey = useMemo(() => getPassageNoteKey(selectedPassage.passage), [selectedPassage]);
    const { data, error } = useLiveQuery(passageNotesQuery(passageKey), [passageKey]);
    const notes: Note[] = data ?? [];

    useEffect(() => {
        if (!isOpen) return;
        console.log('[passageNotes] dialog open', { passageKey, userId: useUserStore.getState().userId, notes: data, error });
    }, [isOpen, passageKey, data, error]);

    const styles = useMemo(() => StyleSheet.create({
        overlay: {
            flex: 1,
            backgroundColor: '#00000080',
            justifyContent: 'center',
            padding: 20,
        },
        card: {
            backgroundColor: theme.colors.background,
            borderRadius: 16,
            padding: 20,
            maxHeight: '80%',
        },
        headerRow: {
            flexDirection: 'row',
            alignItems: 'center',
            justifyContent: 'space-between',
            gap: 12,
            marginBottom: 14,
        },
        title: {
            ...globalStyles.p2,
            fontWeight: 700,
            flexShrink: 1,
        },
        list: {
            flexGrow: 0,
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
            flex: 0,
            marginBottom: 0,
            height: 110,
            textAlignVertical: 'top',
            paddingTop: 10,
            paddingRight: 10,
        },
        composer: {
            marginBottom: 16,
        },
        actionRow: {
            marginTop: 10,
            flexDirection: 'row',
            gap: 8,
        },
        actionButton: {
            ...globalStyles.elevationButton,
            flex: 1,
            width: 'auto',
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
        },
    }), [globalStyles, theme]);

    useEffect(() => {
        if (!editingNoteId || notes.length > 0) {
            return;
        }

        setEditingNoteId(null);
        setDraftText("");
    }, [editingNoteId, notes.length]);

    const close = () => {
        setEditingNoteId(null);
        setDraftText("");
        setViewNotesSheetOpen(false);
    };

    const handleStartEdit = (note: Note) => {
        setEditingNoteId(note.id);
        setDraftText(note.text);
    };

    const handleSave = (note: Note) => {
        if (!draftText.trim()) {
            return;
        }

        updatePassageNote(note.id, draftText.trim());
        setEditingNoteId(null);
        setDraftText("");
    };

    const handleCancel = () => {
        setEditingNoteId(null);
        setDraftText("");
    };

    const handleDelete = (noteId: string) => {
        if (editingNoteId === noteId) {
            handleCancel();
        }
        deletePassageNote(noteId);
    };

    const handleAddNote = () => {
        console.log('[passageNotes] save pressed', { passageKey, draftText });
        if (!draftText.trim()) {
            return;
        }

        addPassageNote(passageKey, draftText.trim());
        handleCancel();
    };

    return (
        <Modal
            visible={isOpen}
            transparent
            animationType="fade"
            statusBarTranslucent
            onRequestClose={close}
        >
            <GestureHandlerRootView style={{ flex: 1 }}>
                <KeyboardAvoidingView style={{ flex: 1 }} behavior="padding">
                    <Pressable style={styles.overlay} onPress={close}>
                        <Pressable style={styles.card}>
                            <View style={styles.headerRow}>
                                <Text style={styles.title}>{selectedPassage.passage.reference.readableReference} Notes</Text>
                                <Pressable onPress={close} hitSlop={10}>
                                    <X size={22} color={theme.colors.onBackgroundSoft} strokeWidth={1.5} />
                                </Pressable>
                            </View>

                            {editingNoteId === null && (
                                <View style={styles.composer}>
                                    <TextInput
                                        value={draftText}
                                        onChangeText={setDraftText}
                                        multiline
                                        style={styles.input}
                                        placeholder="Add a note"
                                        placeholderTextColor={theme.colors.onBackgroundSuperSoft}
                                    />
                                    <View style={styles.actionRow}>
                                        <TouchableOpacity style={styles.actionButton} onPress={handleCancel}>
                                            <Text style={globalStyles.p3}>Cancel</Text>
                                        </TouchableOpacity>
                                        <TouchableOpacity style={styles.actionButton} onPress={handleAddNote}>
                                            <Text style={globalStyles.p3}>Save</Text>
                                        </TouchableOpacity>
                                    </View>
                                </View>
                            )}

                            <FlatList
                                style={styles.list}
                                data={notes}
                                keyExtractor={(item) => item.id.toString()}
                                showsVerticalScrollIndicator={false}
                                keyboardShouldPersistTaps="handled"
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
                                                    <TouchableOpacity style={styles.actionButton} onPress={handleCancel}>
                                                        <Text style={globalStyles.p3}>Cancel</Text>
                                                    </TouchableOpacity>
                                                    <TouchableOpacity style={styles.actionButton} onPress={() => handleSave(item)}>
                                                        <Text style={globalStyles.p3}>Save</Text>
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
                        </Pressable>
                    </Pressable>
                </KeyboardAvoidingView>
            </GestureHandlerRootView>
        </Modal>
    );
};

export default ViewNotesDialog;
