import { TrueSheet } from "@lodev09/react-native-true-sheet";
import { useNavigation } from "@react-navigation/native";
import { NativeStackNavigationProp } from "@react-navigation/native-stack";
import { BottomSheet, Button, Input } from 'heroui-native';
import { CirclePlus, FileText } from "lucide-react-native";
import React, { useCallback, useEffect, useRef, useState } from "react";
import { Text, View } from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import ReorderableList, { reorderItems } from "react-native-reorderable-list";
import { RootStackParamList } from "../../../types/router";
import AddNoteBottomSheet from "../../components/bottom-sheets/addNoteBottomSheet";
import AddPassageBottomSheet from "../../components/bottom-sheets/addPassageBottomSheet";
import NoteComponent from "../../components/note/note";
import PassageComponent from "../../components/passage/passage";
import {
    addNoteToCollection,
    addPassageToCollection,
    removeItemFromCollection,
    reorderItems as reorderCollectionItems,
    updateCollectionMeta,
    updateNoteInCollection,
} from "../../database/repositories/collections.repository";
import { useDraftCollection } from "../../hooks/useCollections";
import { useBottomSheetsStore } from "../../stores/bottomSheets.store";
import useGlobalStyles from "../../styles/gobalStyles";
import useAppTheme from "../../theme";
import { VISIBILITY_OPTIONS } from "../../utils/collectionSortUtils";
import { showToast } from "../../utils/toast";

const visibilityLabel = (key: string) => {
    switch (key) {
        case 'friends':
            return 'Visible to Friends';
        case 'public':
            return 'Public';
        default:
            return 'Not Visible to Friends';
    }
};

export const CreateCollectionScreen = () => {
    const styles = useGlobalStyles();
    const theme = useAppTheme();
    const insets = useSafeAreaInsets();
    const { draftId, collection: draftCollection, commit: commitDraft } = useDraftCollection();
    const setNoteBottomSheet = useBottomSheetsStore((state) => state.setNoteBottomSheet);
    const setNoteSheetOpen = useBottomSheetsStore((state) => state.setNoteSheetOpen);
    const noteSheetOpen = useBottomSheetsStore((state) => state.noteSheetOpen);

    const addPassageBottomSheet = useRef<TrueSheet>(null);
    const addNoteBottomSheet = useRef<TrueSheet>(null);

    const [sheetsReady, setSheetsReady] = useState(false);
    useEffect(() => {
        const frame = requestAnimationFrame(() => setSheetsReady(true));
        return () => cancelAnimationFrame(frame);
    }, []);

    const [title, setTitle] = useState('');
    const seededTitleRef = useRef(false);
    useEffect(() => {
        if (draftCollection && !seededTitleRef.current) {
            setTitle(draftCollection.title);
            seededTitleRef.current = true;
        }
    }, [draftCollection]);

    const handleTitleChange = (text: string) => {
        setTitle(text);
        if (draftId) updateCollectionMeta(draftId, { title: text });
    };

    useEffect(() => {
        if (noteSheetOpen) {
            addNoteBottomSheet.current?.present();
        } else {
            addNoteBottomSheet.current?.dismiss();
        }
    }, [noteSheetOpen, sheetsReady]);

    const navigation = useNavigation<NativeStackNavigationProp<RootStackParamList>>();

    const saveCollection = useCallback(() => {
        if (!draftId || !draftCollection) return;

        if (draftCollection.items.length <= 0) {
            showToast({
                type: 'default',
                title: 'Collection must have at least one item.',
                message: 'A new collection must have at least a note or passage.'
            });
            return;
        }

        commitDraft(title);
        //setSyncStatus('Syncing');
        navigation.goBack();

    }, [draftId, draftCollection, title, commitDraft, navigation]);

    const saveNewCollectionNote = (text: string, itemId: string | null) => {
        if (!draftId) return;

        if (itemId !== null) {
            updateNoteInCollection(draftId, itemId, text);
            return;
        }

        addNoteToCollection(draftId, { text });
    };

    const removeDraftItem = (itemId: string | null) => {
        if (itemId === null || !draftId) {
            return;
        }
        removeItemFromCollection(draftId, itemId);
    };

    const [visibility, setVisibility] = useState('private');
    const [visibilitySheetOpen, setVisibilitySheetOpen] = useState(false);

    const handleVerseCatalog = () => {
        
    };

    return (
        <View style={{ flex: 1 }}>
            <ReorderableList
                style={{ flex: 1 }}
                data={draftCollection?.items ?? []}
                keyExtractor={(item) => `${item.type}-${item.id}`}
                renderItem={({item}) => {
                    if (!item) return null;
                    if (item.type === 'passage')
                        return (
                            <PassageComponent userPassage={item.passage} itemId={item.id} collectionId={draftCollection?.id || ''} onRemove={removeDraftItem} />
                        );
                    if (item.type === 'note')
                        return <NoteComponent note={item.note} itemId={item.id} />;
                    return null;
                }}
                onReorder={({from, to}) => {
                    if (!draftId || !draftCollection) return;
                    const updated = reorderItems(draftCollection.items, from, to);
                    reorderCollectionItems(draftId, updated);
                }}
                ListHeaderComponent={
                    <View style={{...styles.screen, gap: 5}}>
                    <Input
                        placeholder="Collection Title"
                        value={title}
                        onChangeText={handleTitleChange}
                        style={{width: '100%'}}
                    />

                    <View style={{flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', width: '100%', marginTop: 15}}>
                        <Text style={styles.p3}>Visibility: {visibilityLabel(visibility)}</Text>
                        <BottomSheet isOpen={visibilitySheetOpen} onOpenChange={setVisibilitySheetOpen}>
                            <BottomSheet.Trigger asChild>
                                <Button variant="outline" size="sm" className="rounded-full">
                                    <Button.Label style={{color: theme.colors.onBackground}}>Change</Button.Label>
                                </Button>
                            </BottomSheet.Trigger>
                            <BottomSheet.Portal disableFullWindowOverlay>
                                <BottomSheet.Overlay />
                                <BottomSheet.Content>
                                    <View style={{marginTop: 16, gap: 8}}>
                                        {VISIBILITY_OPTIONS.map((option) => {
                                            const value = option.label.toLowerCase();
                                            return (
                                                <Button
                                                    key={option.label}
                                                    variant={visibility === value ? 'secondary' : 'ghost'}
                                                    size="md"
                                                    className="w-full justify-start"
                                                    onPress={() => {
                                                        setVisibility(value);
                                                        setVisibilitySheetOpen(false);
                                                    }}
                                                >
                                                    <Button.Label>{option.label}</Button.Label>
                                                </Button>
                                            );
                                        })}
                                    </View>
                                </BottomSheet.Content>
                            </BottomSheet.Portal>
                        </BottomSheet>
                    </View>

                    <View style={{flexDirection: 'row', justifyContent: 'space-between', width: '100%', marginTop: 10}}>
                        <Button variant="ghost" size="sm" onPress={() => addPassageBottomSheet.current?.present()}>
                            <CirclePlus size={18} color={theme.colors.onBackground} />
                            <Button.Label style={{color: theme.colors.onBackground}}>Add Passage</Button.Label>
                        </Button>

                        <Button variant="ghost" size="sm" onPress={() => {
                            setNoteBottomSheet({ id: '', text: '' }, null);
                            setNoteSheetOpen(true);
                        }}>
                            <FileText size={18} color={theme.colors.onBackground} />
                            <Button.Label style={{color: theme.colors.onBackground}}>Add Note</Button.Label>
                        </Button>
                    </View>


                    <View style={{height: 10}} />
                </View>}
                    ListFooterComponent={<View style={{height: 100}} />}
                />

                <View style={{ position: 'absolute', bottom: insets.bottom + 16, left: 15, right: 15 }}>
                    <Button
                        variant="primary"
                        size="md"
                        className="rounded-full"
                        onPress={saveCollection}
                    >
                        <Button.Label>Create Collection</Button.Label>
                    </Button>
                </View>

                {sheetsReady && (
                    <AddPassageBottomSheet
                        ref={addPassageBottomSheet}
                        collectionItems={draftCollection?.items ?? []}
                        savePassage={(passage) => { if (draftId) addPassageToCollection(draftId, passage); }}
                        removePassage={removeDraftItem}
                    />
                )}
                {sheetsReady && (
                    <AddNoteBottomSheet
                        ref={addNoteBottomSheet}
                        onSave={saveNewCollectionNote}
                        onDelete={removeDraftItem}
                    />
                )}
            </View>
    )
}
