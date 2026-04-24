import { useEffect, useLayoutEffect, useMemo, useRef, useState } from "react";
import useGlobalStyles from "../../styles/gobalStyles";
import useAppTheme from "../../theme";
import { Alert, StyleSheet, TextInput, Text, TouchableOpacity, View } from "react-native";
import { Collection } from "../../../types/collection/collection";
import { initialCollection, useCollectionsStore } from "../../stores/collections.store";
import { useNavigation } from "@react-navigation/native";
import { HeaderTitle } from "@react-navigation/elements";
import { useBottomSheetsStore } from "../../stores/bottomSheets.store";
import { TrueSheet } from "@lodev09/react-native-true-sheet";
import { Check, ChevronRight } from "lucide-react-native";
import ReorderableList, { reorderItems } from "react-native-reorderable-list";
import NewCollectionPassage from "../../components/passage/newCollectionPassage";
import CollectionNote from "../../components/collection/newCollectionNote";
import VisibilityBottomSheet from "../../components/bottom-sheets/visibilityBottomSheet";
import AddPassageBottomSheet from "../../components/bottom-sheets/addPassageBottomSheet";
import AddNoteBottomSheet from "../../components/bottom-sheets/addNoteBottomSheet";
import { Note } from "../../../types/note";
import { CollectionItem } from "../../../types/collection/collectionItem";
import { Passage } from "../../../types/passages/passage";

const EditCollectionScreen = () => {
    const theme = useAppTheme();
    const globalStyles = useGlobalStyles();
    const useLocalStyles = () => useMemo(() => StyleSheet.create({
        screen: {
            gap: 5
        }
    }), [theme]);
    const styles = useLocalStyles();
    const {setNoteBottomSheet, setNoteSheetOpen, noteSheetOpen} = useBottomSheetsStore();

    const navigation = useNavigation();
    const visibilityBottomSheet = useRef<TrueSheet>(null);
    const addPassageBottomSheet = useRef<TrueSheet>(null);
    const addNoteBottomSheet = useRef<TrueSheet>(null);

    const [collection, setLocalCollection] = useState<Collection>(initialCollection);
    const collectionRef = useRef(collection);
    const isSavingRef = useRef(false);
    const {setEditingCollection, clearEditingCollection, editingCollection, setCollection} = useCollectionsStore();

    useEffect(() => {
        setLocalCollection(editingCollection);
    }, []);

    useLayoutEffect(() => {
        navigation.setOptions({
            headerTitle: collection.title,
            headerRight: () => (
                <TouchableOpacity onPress={() => {
                    isSavingRef.current = true;
                    saveCollection(collectionRef.current);
                    navigation.goBack();
                }}>
                    <Check size={28} color={theme.colors.onBackground} />
                </TouchableOpacity>
            )
        });
    }, [collection.title]);

    const saveCollection = (col: Collection) => {
        setCollection(col);
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
        navigation.setOptions({
            HeaderTitle: collection.title
        })
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

            const minId = prev.items.reduce((min, item) => Math.min(min, item.id), 0);
            const nextLocalId = minId <= 0 ? minId - 1 : -1;

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

    const removeEditingCollectionPassage = (itemId: number) => {
        setLocalCollection((prev) => ({
            ...prev,
            items: prev.items.filter((i) => i.id !== itemId)
        }));
    };

    return (
            <View style={[globalStyles.screen, styles.screen]}>
                <View style={{display: 'flex', flexDirection: 'row', justifyContent: 'center', alignItems: 'center', gap: 15}}>
                    <TextInput
                        value={collection.title}
                        onChangeText={(text) => setLocalCollection({...collection, title: text})}
                        maxLength={20}
                        style={globalStyles.input}
                    />
                    {/* <TouchableOpacity style={{...styles.elevationButton, width: '35%', flexDirection: 'column', padding: -10}}>
                        <BadgePlus size={22} color={theme.colors.onBackground} />
                        <Text style={styles.p3}>Create</Text>
                    </TouchableOpacity> */}
                </View>

                <TouchableOpacity 
                    onPress={() => visibilityBottomSheet.current?.present()}>
                    <View style={{backgroundColor: theme.colors.elevation, paddingVertical: 10, paddingHorizontal: 25, borderRadius: 5, display: 'flex', flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', width: '100%'}}>
                        <Text style={globalStyles.p3}>Visibility</Text>
                        <View style={{flexDirection: 'row', justifyContent: 'center', alignItems: 'center', gap: 10}}>
                            <Text style={{...globalStyles.p3, color: theme.colors.onBackgroundSuperSoft}}>
                                {collection.visibility === 0 && 'Private'}
                                {collection.visibility === 1 && 'Friends'}
                                {collection.visibility === 2 && 'Public'}
                            </Text>
                            <ChevronRight size={28} color={theme.colors.onBackground} />
                        </View>
                    </View>
                </TouchableOpacity>

                <View style={{display: 'flex', flexDirection: 'row', justifyContent: 'space-between', width: '100%', gap: 10}}>
                    <TouchableOpacity style={{...globalStyles.elevationButton, width: '49%'}} onPress={() => addPassageBottomSheet.current?.present()}>
                        <Text style={globalStyles.p3}>Add Passage</Text>
                    </TouchableOpacity>

                    <TouchableOpacity style={{...globalStyles.elevationButton, width: '49%'}} onPress={() => {
                        setNoteBottomSheet({ id: 0, text: '' }, null);
                        setNoteSheetOpen(true);
                    }}>
                        <Text style={globalStyles.p3}>Add Note</Text>
                    </TouchableOpacity>
                </View>

                <View style={{height: 10}} />
                
                <ReorderableList
                    data={collection.items}
                    keyExtractor={(item) => `${item.type}-${item.id}`}
                    renderItem={({item}) => {
                        if (item.type === 'passage')
                            return <NewCollectionPassage userPassage={item.passage} itemId={item.id} />;
                        if (item.type === 'note')
                            return <CollectionNote note={item.note} itemId={item.id} />;
                        return null;
                    }}
                    onReorder={({from, to}) => {
                        const updated = reorderItems(collection.items, from, to);
                        setLocalCollection({...collection, items: updated});
                    }}
                />

                {/* <TouchableOpacity style={{...styles.elevationButton, position: 'absolute', bottom: 40}}>
                    <Text style={styles.p3}>Create</Text>
                </TouchableOpacity> */}

                <VisibilityBottomSheet 
                    ref={visibilityBottomSheet}
                    currentVisibility={collection.visibility}
                />
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
                                id: Date.now() * -1,
                                note: { id: 0, text }
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