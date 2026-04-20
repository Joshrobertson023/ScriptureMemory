import { useEffect, useMemo, useState } from "react";
import useGlobalStyles from "../../styles/gobalStyles";
import useAppTheme from "../../theme";
import { Alert, StyleSheet, TextInput, TouchableOpacity, View } from "react-native";
import { Collection } from "../../../types/collection/collection";
import { initialCollection, useCollectionsStore } from "../../stores/collections.store";
import { useNavigation } from "@react-navigation/native";
import { HeaderTitle } from "@react-navigation/elements";

const EditCollectionScreen = () => {
    const theme = useAppTheme();
    const globalStyles = useGlobalStyles();
    const useLocalStyles = () => useMemo(() => StyleSheet.create({
        screen: {
            gap: 5
        }
    }), [theme]);
    const styles = useLocalStyles();

    const navigation = useNavigation();

    useEffect(() => {
        const unsubscribe = navigation.addListener("beforeRemove", (e) => {
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
                        onPress: () => setEditingCollection(collection)
                    }
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

    const [collection, setCollection] = useState<Collection>(initialCollection);
    const {setEditingCollection, clearEditingCollection} = useCollectionsStore();

    return (
            <View style={[globalStyles.screen, styles.screen]}>
                <View style={{display: 'flex', flexDirection: 'row', justifyContent: 'center', alignItems: 'center', gap: 15}}>
                    <TextInput
                        value={collection.title}
                        onChangeText={(text) => setCollection({...collection, title: text})}
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
                    data={newCollection.items}
                    keyExtractor={(item) => `${item.type}-${item.id}`}
                    renderItem={({item}) => {
                        if (item.type === 'passage')
                            return <NewCollectionPassage userPassage={item} itemId={item.id} />;
                        if (item.type === 'note')
                            return <CollectionNote note={item.note} itemId={item.id} />;
                        return null;
                    }}
                    onReorder={({from, to}) => {
                        const updated = reorderItems(items, from, to);
                        setItems(updated);
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
                />
                <AddNoteBottomSheet
                    ref={addNoteBottomSheet}
                    onSave={saveNewCollectionNote}
                    onDelete={deleteNewCollectionNote}
                />
            </View>
    )
}

export default EditCollectionScreen;