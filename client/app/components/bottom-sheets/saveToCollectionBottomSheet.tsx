import { TrueSheet } from "@lodev09/react-native-true-sheet";
import { BadgePlus, List } from "lucide-react-native";
import { forwardRef, useMemo, useState } from "react";
import { FlatList, StyleSheet, Text, TextInput, TouchableOpacity, View } from "react-native";
import { Collection } from "../../../types/collection/collection";
import useAppTheme from "../../theme";
import useGlobalStyles from "../../styles/gobalStyles";
import { useBottomSheetsStore } from "../../stores/bottomSheets.store";
import { useCollectionsStore } from "../../stores/collections.store";
import { CollectionCard } from "../collection/collectionCard";

const SaveToCollectionBottomSheet = forwardRef<TrueSheet>((_, ref) => {
    const theme = useAppTheme();
    const globalStyles = useGlobalStyles();
    const [createNewMode, setCreateNewMode] = useState(false);

    const saveToCollectionBottomSheet = useBottomSheetsStore((state) => state.saveToCollectionBottomSheet);
    const setSaveToCollectionSheetOpen = useBottomSheetsStore((state) => state.setSaveToCollectionSheetOpen);
    const {
        userCollections,
        newCollection,
        setNewCollection,
        clearNewCollection,
        addCollection,
        addPassageToNewCollection,
        addPassageToCollection,
    } = useCollectionsStore();

    const styles = useMemo(() => StyleSheet.create({
        container: {
            flex: 1,
            paddingHorizontal: 15,
            paddingTop: 18,
            paddingBottom: 20,
            gap: 14,
        },
        headerRow: {
            flexDirection: 'row',
            alignItems: 'center',
            justifyContent: 'space-between',
            gap: 12,
        },
        headerButton: {
            flexDirection: 'row',
            alignItems: 'center',
            justifyContent: 'center',
            gap: 6,
            paddingHorizontal: 10,
            paddingVertical: 6,
            borderRadius: 8,
        },
        draftCard: {
            backgroundColor: theme.colors.elevation,
            borderRadius: 10,
            paddingHorizontal: 14,
            paddingVertical: 12,
            gap: 10,
        },
        draftTopRow: {
            flexDirection: 'row',
            alignItems: 'center',
            justifyContent: 'space-between',
            gap: 10,
        },
        draftInput: {
            flex: 1,
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
        draftCount: {
            flexDirection: 'row',
            alignItems: 'center',
            gap: 5,
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
        actionStack: {
            gap: 8,
        },
        list: {
            flex: 1,
        },
        listContent: {
            gap: 10,
            paddingBottom: 12,
        },
    }), [globalStyles, theme]);

    const resetCreateMode = () => {
        setCreateNewMode(false);
        clearNewCollection();
    };

    const handleCreateNew = () => {
        clearNewCollection();
        setCreateNewMode(true);
    };

    const handleSaveNewCollection = () => {
        const passage = saveToCollectionBottomSheet.passage;

        if (passage.verses.length === 0) {
            return;
        }

        addPassageToNewCollection(passage);
        const currentNewCollection = useCollectionsStore.getState().newCollection;
        const title = currentNewCollection.title.trim() === '' ? 'New Collection' : currentNewCollection.title.trim();

        addCollection({
            ...currentNewCollection,
            title,
        });

        resetCreateMode();
        setSaveToCollectionSheetOpen(false);
    };

    const handleCollectionPress = (collection: Collection) => {
        addPassageToCollection(collection.id, saveToCollectionBottomSheet.passage);
        resetCreateMode();
        setSaveToCollectionSheetOpen(false);
    };

    return (
        <TrueSheet
            ref={ref}
            detents={[0.8, 1]}
            scrollable
            style={{ backgroundColor: theme.colors.background }}
            onDidDismiss={() => {
                resetCreateMode();
                setSaveToCollectionSheetOpen(false);
            }}
        >
            <View style={styles.container}>
                <View style={styles.headerRow}>
                    <Text style={globalStyles.p2}>Save to collection:</Text>
                    <TouchableOpacity style={styles.headerButton} onPress={handleCreateNew}>
                        <Text style={globalStyles.p3}>Create New</Text>
                    </TouchableOpacity>
                </View>

                {createNewMode && (
                    <View style={styles.draftCard}>
                        <View style={styles.draftTopRow}>
                            <TextInput
                                value={newCollection.title}
                                onChangeText={(text) => setNewCollection({ ...newCollection, title: text })}
                                placeholder="Title"
                                placeholderTextColor={theme.colors.onBackgroundSuperSoft}
                                style={[globalStyles.input, styles.draftInput]}
                                maxLength={20}
                            />
                        </View>

                        <View style={styles.draftMetaRow}>
                            <View style={styles.draftCountChip}>
                                <List size={12} color={theme.colors.onBackground} />
                                <Text style={styles.draftCountText}>{newCollection.items.length}</Text>
                            </View>
                            <Text style={styles.draftVisibility}>Private</Text>
                        </View>
                    </View>
                )}

                {createNewMode && (
                    <View style={styles.actionStack}>
                        <TouchableOpacity style={globalStyles.elevationButton} onPress={handleSaveNewCollection}>
                            <Text style={globalStyles.p3}>Save</Text>
                        </TouchableOpacity>
                        <TouchableOpacity style={globalStyles.elevationButton} onPress={resetCreateMode}>
                            <Text style={globalStyles.p3}>Cancel</Text>
                        </TouchableOpacity>
                    </View>
                )}

                <FlatList
                    style={styles.list}
                    data={userCollections}
                    keyExtractor={(item) => item.id.toString()}
                    contentContainerStyle={styles.listContent}
                    showsVerticalScrollIndicator={false}
                    renderItem={({ item }) => (
                        <CollectionCard collection={item} onPress={handleCollectionPress} />
                    )}
                />
            </View>
        </TrueSheet>
    );
});

export default SaveToCollectionBottomSheet;
