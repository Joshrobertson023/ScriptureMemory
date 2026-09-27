import { TrueSheet } from "@lodev09/react-native-true-sheet";
import { RouteProp, useNavigation, useRoute } from "@react-navigation/native";
import { BottomSheet, Button } from "heroui-native";
import { CirclePlus, FileText } from "lucide-react-native";
import React, { useEffect, useLayoutEffect, useMemo, useRef, useState } from "react";
import { Alert, StyleSheet, Text, TextInput, View } from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import ReorderableList, { reorderItems } from "react-native-reorderable-list";
import { Collection } from "../../../types/collection/collection";
import { CollectionItem } from "../../../types/collection/collectionItem";
import { Passage } from "../../../types/passages/passage";
import { RootStackParamList } from "../../../types/router";
import AddNoteBottomSheet from "../../components/bottom-sheets/addNoteBottomSheet";
import AddPassageBottomSheet from "../../components/bottom-sheets/addPassageBottomSheet";
import NoteComponent from "../../components/note/note";
import PassageComponent from "../../components/passage/passage";
import { applyCollectionEdit } from "../../database/repositories/collections.repository";
import { useCollection } from "../../hooks/useCollections";
import { useBottomSheetsStore } from "../../stores/bottomSheets.store";
import { initialCollection } from "../../stores/collections.store";
import useGlobalStyles from "../../styles/gobalStyles";
import useAppTheme from "../../theme";
import { VISIBILITY_OPTIONS } from "../../utils/collectionSortUtils";

const visibilityLabel = (value: string) => {
    switch (value) {
        case 'Friends':
            return 'Visible to Friends';
        case 'Public':
            return 'Public';
        default:
            return 'Not Visible to Friends';
    }
};

const EditCollectionScreen = () => {
    const theme = useAppTheme();
    const globalStyles = useGlobalStyles();
    const insets = useSafeAreaInsets();
    const useLocalStyles = () => useMemo(() => StyleSheet.create({
        screen: {
            gap: 5
        }
    }), [theme]);
    const styles = useLocalStyles();
    const setNoteBottomSheet = useBottomSheetsStore((state) => state.setNoteBottomSheet);
    const setNoteSheetOpen = useBottomSheetsStore((state) => state.setNoteSheetOpen);
    const noteSheetOpen = useBottomSheetsStore((state) => state.noteSheetOpen);

    const navigation = useNavigation();
    const route = useRoute<RouteProp<RootStackParamList, 'editCollection'>>();
    const [visibilitySheetOpen, setVisibilitySheetOpen] = useState(false);
    const addPassageBottomSheet = useRef<TrueSheet>(null);
    const addNoteBottomSheet = useRef<TrueSheet>(null);

    const [collection, setLocalCollection] = useState<Collection>(initialCollection);
    const collectionRef = useRef(collection);
    const isSavingRef = useRef(false);
    const seededRef = useRef(false);
    // Seeded once from the stored collection - after that this screen owns its own
    // working copy until Save/Discard, same as before.
    const { collection: storedCollection } = useCollection(route.params?.id);

    useEffect(() => {
        if (storedCollection && !seededRef.current) {
            setLocalCollection(storedCollection);
            seededRef.current = true;
        }
    }, [storedCollection]);

    useLayoutEffect(() => {
        navigation.setOptions({
            headerTitle: collection.title
        });
    }, [collection.title]);

    const saveCollection = (col: Collection) => {
        applyCollectionEdit(col);
    }

    useEffect(() => {
        collectionRef.current = collection;
    }, [collection]);

    useEffect(() => {
        const unsubscribe = navigation.addListener("beforeRemove", (e) => {
            if (isSavingRef.current) return;
            e.preventDefault();

            Alert.alert(
                "Unsaved changes",
                "Do you want to save your changes?",
                [
                    {
                        "text": "Discard",
                        "style": 'destructive',
                        'onPress': () => navigation.dispatch(e.data.action),
                    },
                    {
                        'text': 'Cancel',
                        'style': 'cancel',
                    },
                    {
                        'text': 'Save',
                        onPress: () => {
                        saveCollection(collectionRef.current);
                        navigation.dispatch(e.data.action);
                    }}
                ]
            )
        });
        return unsubscribe;
    }, [navigation]);

    useEffect(() => {
        if (noteSheetOpen) {
            addNoteBottomSheet.current?.present();
        } else {
            addNoteBottomSheet.current?.dismiss();
        }
    }, [noteSheetOpen]);

    const saveEditingCollectionPassage = (passage: Passage) => {
        setLocalCollection((prev) => {
            const alreadyExists = prev.items.some(
                (i) => i.type === 'passage' && i.passage.passage.reference.readableReference === passage.reference.readableReference
            );
            if (alreadyExists) {
                return prev;
            }

            // Locally-unique id for this editing session only - applyCollectionEdit
            // mints real UUIDs for every item when the edit is saved.
            const nextLocalId = `local-${Date.now()}-${Math.random()}`;

            const newItem: CollectionItem = {
                type: 'passage',
                id: nextLocalId,
                passage: {
                    id: nextLocalId,
                    passage,
                }
            };

            return {
                ...prev,
                items: [...prev.items, newItem]
            };
        });
    };

    const removeEditingCollectionPassage = (itemId: string) => {
        setLocalCollection((prev) => ({
            ...prev,
            items: prev.items.filter((i) => i.id !== itemId)
        }));
    };

    return (
        <View style={{ flex: 1 }}>
                <ReorderableList
                    style={{ flex: 1 }}
                    data={collection.items}
                    keyExtractor={(item) => `${item.type}-${item.id}`}
                    renderItem={({item}) => {
                        if (item.type === 'passage')
                            return (
                                <PassageComponent
                                    userPassage={item.passage}
                                    itemId={item.id}
                                    collectionId={collection.id}
                                    onRemove={removeEditingCollectionPassage}
                                />
                            );
                        if (item.type === 'note')
                            return <NoteComponent note={item.note} itemId={item.id} />;
                        return null;
                    }}
                    onReorder={({from, to}) => {
                        const updated = reorderItems(collection.items, from, to);
                        setLocalCollection({...collection, items: updated});
                    }}
                    ListHeaderComponent={

                        <View style={[globalStyles.screen, styles.screen]}>

                            <View style={{display: 'flex', flexDirection: 'row', justifyContent: 'center', alignItems: 'center', gap: 15}}>
                                <TextInput
                                    value={collection.title}
                                    onChangeText={(text) => setLocalCollection({...collection, title: text})}
                                    maxLength={20}
                                    style={globalStyles.input}
                                />
                            </View>

                            <View style={{flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', width: '100%', marginTop: 15}}>
                                <Text style={globalStyles.p3}>Visibility: {visibilityLabel(collection.visibility)}</Text>
                                <BottomSheet isOpen={visibilitySheetOpen} onOpenChange={setVisibilitySheetOpen}>
                                    <BottomSheet.Trigger asChild>
                                        <Button variant="outline" size="sm" className="rounded-full">
                                            <Button.Label>Change</Button.Label>
                                        </Button>
                                    </BottomSheet.Trigger>
                                    <BottomSheet.Portal disableFullWindowOverlay>
                                        <BottomSheet.Overlay />
                                        <BottomSheet.Content>
                                            <BottomSheet.Title>Visibility</BottomSheet.Title>
                                            <View style={{marginTop: 16, gap: 8}}>
                                                {VISIBILITY_OPTIONS.map((option) => (
                                                    <Button
                                                        key={option.label}
                                                        variant={collection.visibility === option.label ? 'secondary' : 'ghost'}
                                                        size="md"
                                                        className="w-full justify-start"
                                                        onPress={() => {
                                                            setLocalCollection({...collection, visibility: option.label});
                                                            setVisibilitySheetOpen(false);
                                                        }}
                                                    >
                                                        <Button.Label>{option.label}</Button.Label>
                                                    </Button>
                                                ))}
                                            </View>
                                        </BottomSheet.Content>
                                    </BottomSheet.Portal>
                                </BottomSheet>
                            </View>

                            <View style={{flexDirection: 'row', justifyContent: 'space-evenly', width: '100%', marginTop: 10}}>
                                <Button variant="ghost" size="sm" onPress={() => addPassageBottomSheet.current?.present()}>
                                    <CirclePlus size={18} color={theme.colors.onBackground} />
                                    <Button.Label>Add Passage</Button.Label>
                                </Button>

                                <Button variant="ghost" size="sm" onPress={() => {
                                    setNoteBottomSheet({ id: '', text: '' }, null);
                                    setNoteSheetOpen(true);
                                }}>
                                    <FileText size={18} color={theme.colors.onBackground} />
                                    <Button.Label>Add Note</Button.Label>
                                </Button>
                            </View>

                            <View style={{height: 10}} />
                    </View>
                    }
                    ListFooterComponent={<View style={{height: 100}} />}
                />

                <View style={{ position: 'absolute', bottom: insets.bottom + 16, left: 15, right: 15 }}>
                    <Button
                        variant="primary"
                        size="lg"
                        className="rounded-full"
                        onPress={() => {
                            isSavingRef.current = true;
                            saveCollection(collectionRef.current);
                            navigation.goBack();
                        }}
                    >
                        <Button.Label>Save</Button.Label>
                    </Button>
                </View>

                <AddPassageBottomSheet
                    ref={addPassageBottomSheet}
                    collectionItems={collection.items}
                    savePassage={saveEditingCollectionPassage}
                    removePassage={removeEditingCollectionPassage}
                />
                <AddNoteBottomSheet
                    ref={addNoteBottomSheet}
                    onSave={(text, id) => {
                        if (id !== null) {
                            setLocalCollection((prev) => ({
                                ...prev,
                                items: prev.items.map((i) =>
                                    i.id === id && i.type === 'note'
                                        ? { ...i, note: { ...i.note, text } }
                                        : i
                                )
                            }));
                        } else {
                            const newItem: CollectionItem = {
                                type: 'note',
                                id: `local-${Date.now()}-${Math.random()}`,
                                note: { id: '', text }
                            };
                            setLocalCollection((prev) => ({
                                ...prev,
                                items: [...prev.items, newItem]
                            }));
                        }
                    }}
                    onDelete={(id) => {
                        if (id !== null)
                            setLocalCollection((prev) => ({
                                ...prev,
                                items: prev.items.filter((i) => i.id !== id)
                            }));
                    }}
                />
        </View>
    )
}

export default EditCollectionScreen;