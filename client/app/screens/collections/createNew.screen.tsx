import { Alert, TextInput, Text, TouchableOpacity, View } from "react-native"
import useGlobalStyles from "../../styles/gobalStyles"
import { useCollectionsStore } from "../../stores/collections.store";
import { BadgePlus, Check, ChevronRight } from "lucide-react-native";
import useAppTheme from "../../theme";
import VisibilityBottomSheet from "../../components/bottom-sheets/visibilityBottomSheet";
import { useCallback, useEffect, useLayoutEffect, useRef, useState } from "react";
import { TrueSheet } from "@lodev09/react-native-true-sheet";
import AddPassageBottomSheet from "../../components/bottom-sheets/addPassageBottomSheet";
import NewCollectionPassage from "../../components/passage/newCollectionPassage";
import CollectionNote from "../../components/collection/newCollectionNote";
import ReorderableList, { reorderItems } from "react-native-reorderable-list";
import { useNavigation } from "@react-navigation/native";
import { NativeStackNavigationProp } from "@react-navigation/native-stack";
import { RootStackParamList } from "../../../types/router";
import { useBottomSheetsStore } from "../../stores/bottomSheets.store";
import AddNoteBottomSheet from "../../components/bottom-sheets/addNoteBottomSheet";
import { Note } from "../../../types/note";
import { useAppStore } from "../../stores/appState.store";
import { Snackbar } from "react-native-snackbar";

export const CreateCollectionScreen = () => {
    const styles = useGlobalStyles();
    const theme = useAppTheme();
    const {
        newCollection,
        setNewCollection,
        setNewCollectionItems,
        addPassageToNewCollection,
        addNoteToNewCollection,
        updateNoteInNewCollection,
        removeItemFromNewCollection
    } = useCollectionsStore();
    const {setNoteBottomSheet, setNoteSheetOpen, noteSheetOpen} = useBottomSheetsStore();

    const [items, setItems] = useState(newCollection.items ?? []);
    const visibilityBottomSheet = useRef<TrueSheet>(null);
    const addPassageBottomSheet = useRef<TrueSheet>(null);
    const addNoteBottomSheet = useRef<TrueSheet>(null);

    const {addCollection, clearNewCollection} = useCollectionsStore();
    const {setSyncStatus} = useAppStore();

    useEffect(() => {
        if (noteSheetOpen) {
            addNoteBottomSheet.current?.present();
        } else {
            addNoteBottomSheet.current?.dismiss();
        }
    }, [noteSheetOpen]);

    const navigation = useNavigation<NativeStackNavigationProp<RootStackParamList>>();

    const saveCollection = useCallback(() => {
        const current = useCollectionsStore.getState().newCollection;
        const title = current.title.trim() === '' ? 'New Collection' : current.title.trim();

        if (current.items.length <= 0) {
            Snackbar.show({
                text: 'A new collection must have at least a note or passage.',
                duration: Snackbar.LENGTH_SHORT
            });
            return;
        }

        addCollection({
            ...current,
            title,
        });
        clearNewCollection();
        //setSyncStatus('Syncing');
        navigation.goBack();

    }, [addCollection, clearNewCollection, navigation]);

    const saveNewCollectionNote = (text: string, itemId: number | null) => {
        if (itemId !== null) {
            updateNoteInNewCollection(itemId, text);
            return;
        }

        const note: Note = {
            id: 0,
            text
        };
        addNoteToNewCollection(note);
    };

    const deleteNewCollectionNote = (itemId: number | null) => {
        if (itemId === null) {
            return;
        }
        removeItemFromNewCollection(itemId);
    };

    useLayoutEffect(() => {
        navigation.setOptions({
            headerRight: () => (
                <TouchableOpacity onPress={saveCollection}>
                    <Check size={28} color={theme.colors.onBackground} />
                </TouchableOpacity>
            )
        })
    }, [navigation, saveCollection, theme.colors.onBackground])

    return (
            <View style={{...styles.screen, gap: 5}}>
                <View style={{display: 'flex', flexDirection: 'row', justifyContent: 'center', alignItems: 'center', gap: 15}}>
                    <TextInput
                        value={newCollection.title}
                        onChangeText={(text) => setNewCollection({...newCollection, title: text})}
                        maxLength={20}
                        style={styles.input}
                        placeholder="New Collection Title"
                    />
                    {/* <TouchableOpacity style={{...styles.elevationButton, width: '35%', flexDirection: 'column', padding: -10}}>
                        <BadgePlus size={22} color={theme.colors.onBackground} />
                        <Text style={styles.p3}>Create</Text>
                    </TouchableOpacity> */}
                </View>

                <TouchableOpacity 
                    onPress={() => visibilityBottomSheet.current?.present()}>
                    <View style={{backgroundColor: theme.colors.elevation, paddingVertical: 10, paddingHorizontal: 25, borderRadius: 5, display: 'flex', flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', width: '100%'}}>
                        <Text style={styles.p3}>Visibility</Text>
                        <View style={{flexDirection: 'row', justifyContent: 'center', alignItems: 'center', gap: 10}}>
                            <Text style={{...styles.p3, color: theme.colors.onBackgroundSuperSoft}}>
                                {newCollection.visibility === 0 && 'Private'}
                                {newCollection.visibility === 1 && 'Friends'}
                                {newCollection.visibility === 2 && 'Public'}
                            </Text>
                            <ChevronRight size={28} color={theme.colors.onBackground} />
                        </View>
                    </View>
                </TouchableOpacity>

                <View style={{display: 'flex', flexDirection: 'row', justifyContent: 'space-between', width: '100%', gap: 10}}>
                    <TouchableOpacity style={{...styles.elevationButton, width: '49%'}} onPress={() => addPassageBottomSheet.current?.present()}>
                        <Text style={styles.p3}>Add Passage</Text>
                    </TouchableOpacity>

                    <TouchableOpacity style={{...styles.elevationButton, width: '49%'}} onPress={() => {
                        setNoteBottomSheet({ id: 0, text: '' }, null);
                        setNoteSheetOpen(true);
                    }}>
                        <Text style={styles.p3}>Add Note</Text>
                    </TouchableOpacity>
                </View>

                <View style={{height: 10}} />
                
                <ReorderableList
                    data={newCollection.items ?? []}
                    keyExtractor={(item) => `${item.type}-${item.id}`}
                    renderItem={({item}) => {
                        if (!item) return null;
                        if (item.type === 'passage')
                            return <NewCollectionPassage userPassage={item.passage} itemId={item.id} />;
                        if (item.type === 'note')
                            return <CollectionNote note={item.note} itemId={item.id} />;
                        return null;
                    }}
                    onReorder={({from, to}) => {
                        const current = useCollectionsStore.getState().newCollection.items ?? [];
                        const updated = reorderItems(current, from, to);
                        setNewCollectionItems(updated);
                    }}
                />

                {/* <TouchableOpacity style={{...styles.elevationButton, position: 'absolute', bottom: 40}}>
                    <Text style={styles.p3}>Create</Text>
                </TouchableOpacity> */}

                <VisibilityBottomSheet 
                    ref={visibilityBottomSheet}
                    currentVisibility={newCollection.visibility}
                />
                <AddPassageBottomSheet
                    ref={addPassageBottomSheet}
                    collectionItems={newCollection.items ?? []}
                    savePassage={addPassageToNewCollection}
                    removePassage={removeItemFromNewCollection}
                />
                <AddNoteBottomSheet
                    ref={addNoteBottomSheet}
                    onSave={saveNewCollectionNote}
                    onDelete={deleteNewCollectionNote}
                />
            </View>
    )
}