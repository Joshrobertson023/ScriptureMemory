import { List, X } from "lucide-react-native";
import React, { useMemo, useState } from "react";
import { FlatList, KeyboardAvoidingView, Modal, Pressable, StyleSheet, Text, TextInput, TouchableOpacity, View } from "react-native";
import { Collection } from "../../../types/collection/collection";
import useAppTheme from "../../theme";
import useGlobalStyles from "../../styles/gobalStyles";
import { useBottomSheetsStore } from "../../stores/bottomSheets.store";
import { useUserCollections, useDraftCollection } from "../../hooks/useCollections";
import { addPassageToCollection } from "../../database/repositories/collections.repository";
import { CollectionCard } from "../collection/collectionCard";

const SaveToCollectionDialog = () => {
    const theme = useAppTheme();
    const globalStyles = useGlobalStyles();
    const [createNewMode, setCreateNewMode] = useState(false);

    const isOpen = useBottomSheetsStore((state) => state.saveToCollectionSheetOpen);
    const saveToCollectionBottomSheet = useBottomSheetsStore((state) => state.saveToCollectionBottomSheet);
    const setSaveToCollectionSheetOpen = useBottomSheetsStore((state) => state.setSaveToCollectionSheetOpen);
    const userCollections = useUserCollections();
    const { collection: draftCollection, commit: commitDraft } = useDraftCollection(createNewMode);
    const [draftTitle, setDraftTitle] = useState('');

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
            gap: 14,
            maxHeight: '80%',
        },
        headerRow: {
            flexDirection: 'row',
            alignItems: 'center',
            justifyContent: 'space-between',
            gap: 12,
        },
        headerActions: {
            flexDirection: 'row',
            alignItems: 'center',
            gap: 14,
        },
        draftCard: {
            backgroundColor: theme.colors.elevation,
            borderRadius: 10,
            paddingHorizontal: 14,
            paddingVertical: 12,
            gap: 10,
        },
        draftInput: {
            flex: 0,
            marginBottom: 0,
            height: 42,
            borderRadius: 8,
            paddingLeft: 10,
            paddingRight: 10,
            borderColor: theme.colors.elevation2,
            borderWidth: 2
        },
        draftMetaRow: {
            flexDirection: 'row',
            justifyContent: 'space-between',
            alignItems: 'center',
        },
        draftCountChip: {
            backgroundColor: theme.colors.elevation2,
            borderRadius: 30,
            paddingVertical: 2,
            paddingHorizontal: 8,
            gap: 3,
            flexDirection: 'row',
            justifyContent: 'center',
            alignItems: 'center'
        },
        draftCountText: {
            ...globalStyles.p4,
            color: theme.colors.onBackgroundSuperSoft,
        },
        draftVisibility: {
            ...globalStyles.p3,
            color: theme.colors.onBackgroundSuperSoft,
            marginBottom: -3,
        },
        actionRow: {
            flexDirection: 'row',
            gap: 8,
        },
        actionButton: {
            ...globalStyles.elevationButton,
            flex: 1,
            width: 'auto',
        },
        list: {
            flexGrow: 0,
        },
        listContent: {
            gap: 10,
        },
    }), [globalStyles, theme]);

    const resetCreateMode = () => {
        setCreateNewMode(false);
        setDraftTitle('');
    };

    const close = () => {
        resetCreateMode();
        setSaveToCollectionSheetOpen(false);
    };

    const handleCreateNew = () => {
        setDraftTitle('');
        setCreateNewMode(true);
    };

    const handleSaveNewCollection = async () => {
        if (!draftCollection) return;
        await commitDraft(draftTitle);
        resetCreateMode();
    };

    const handleCollectionPress = (collection: Collection) => {
        addPassageToCollection(collection.id, saveToCollectionBottomSheet.passage);
        close();
    };

    return (
        <Modal
            visible={isOpen}
            transparent
            animationType="fade"
            statusBarTranslucent
            onRequestClose={close}
        >
            <KeyboardAvoidingView style={{ flex: 1 }} behavior="padding">
                <Pressable style={styles.overlay} onPress={close}>
                    <Pressable style={styles.card}>
                        <View style={styles.headerRow}>
                            <Text style={{ ...globalStyles.p2, fontWeight: 700 }}>Save to collection</Text>
                            <View style={styles.headerActions}>
                                <TouchableOpacity onPress={handleCreateNew}>
                                    <Text style={globalStyles.p3}>Create New</Text>
                                </TouchableOpacity>
                                <Pressable onPress={close} hitSlop={10}>
                                    <X size={22} color={theme.colors.onBackgroundSoft} strokeWidth={1.5} />
                                </Pressable>
                            </View>
                        </View>

                        {createNewMode && (
                            <View style={styles.draftCard}>
                                <TextInput
                                    value={draftTitle}
                                    onChangeText={setDraftTitle}
                                    placeholder="Title"
                                    placeholderTextColor={theme.colors.onBackgroundSuperSoft}
                                    style={[globalStyles.input, styles.draftInput]}
                                    maxLength={20}
                                />
                                <View style={styles.draftMetaRow}>
                                    <View style={styles.draftCountChip}>
                                        <List size={12} color={theme.colors.onBackground} />
                                        <Text style={styles.draftCountText}>{draftCollection?.items.length ?? 0}</Text>
                                    </View>
                                    <Text style={styles.draftVisibility}>Private</Text>
                                </View>
                            </View>
                        )}

                        {createNewMode && (
                            <View style={styles.actionRow}>
                                <TouchableOpacity style={styles.actionButton} onPress={resetCreateMode}>
                                    <Text style={globalStyles.p3}>Cancel</Text>
                                </TouchableOpacity>
                                <TouchableOpacity style={styles.actionButton} onPress={handleSaveNewCollection}>
                                    <Text style={globalStyles.p3}>Save</Text>
                                </TouchableOpacity>
                            </View>
                        )}

                        <FlatList
                            style={styles.list}
                            data={userCollections}
                            keyExtractor={(item) => item.id.toString()}
                            contentContainerStyle={styles.listContent}
                            showsVerticalScrollIndicator={false}
                            keyboardShouldPersistTaps="handled"
                            renderItem={({ item }) => (
                                <CollectionCard collection={item} onPress={handleCollectionPress} />
                            )}
                        />
                    </Pressable>
                </Pressable>
            </KeyboardAvoidingView>
        </Modal>
    );
};

export default SaveToCollectionDialog;
