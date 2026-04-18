import { Alert, TextInput, Text, TouchableOpacity, View } from "react-native"
import useGlobalStyles from "../../styles/gobalStyles"
import { initialCollection, useCollectionsStore } from "../../stores/collections.store";
import { ArrowUpDown, ChevronRight } from "lucide-react-native";
import useAppTheme from "../../theme";
import VisibilityBottomSheet from "../../components/bottom-sheets/visibilityBottomSheet";
import { useEffect, useRef, useState } from "react";
import { TrueSheet } from "@lodev09/react-native-true-sheet";
import AddPassageBottomSheet from "../../components/bottom-sheets/addPassageBottomSheet";
import NewCollectionPassage from "../../components/passage/newCollectionPassage";
import CollectionNote from "../../components/collection/newCollectionNote";
import ReorderableList, { reorderItems } from "react-native-reorderable-list";
import { useNavigation } from "@react-navigation/native";
import { NativeStackNavigationProp } from "@react-navigation/native-stack";
import { RootStackParamList } from "../../../types/router";

export const CreateCollectionScreen = () => {
    const styles = useGlobalStyles();
    const theme = useAppTheme();
    const {newCollection, setNewCollection, setNewCollectionItems} = useCollectionsStore();
    const navigation = useNavigation<NativeStackNavigationProp<RootStackParamList>>();

    // Local list state avoids persistence churn during drag operations.
    const [items, setItems] = useState(newCollection.items);
    const visibilityBottomSheet = useRef<TrueSheet>(null);
    const addPassageBottomSheet = useRef<TrueSheet>(null);
    const bypassBackPromptRef = useRef(false);

    useEffect(() => {
        setItems(newCollection.items);
    }, [newCollection.items.length]);

    useEffect(() => {
        const unsubscribe = navigation.addListener('beforeRemove', (e) => {
            if (bypassBackPromptRef.current) {
                bypassBackPromptRef.current = false;
                return;
            }

            e.preventDefault();
            Alert.alert(
                'Save draft',
                'Would you like to save your progress for later?',
                [
                    {
                        text: 'Cancel',
                        style: 'cancel',
                    },
                    {
                        text: 'Delete',
                        style: 'destructive',
                        onPress: () => {
                            setNewCollection(initialCollection);
                            bypassBackPromptRef.current = true;
                            navigation.dispatch(e.data.action);
                        },
                    },
                    {
                        text: 'Save Draft',
                        onPress: () => {
                            const updatedDraft = {
                                ...newCollection,
                                items,
                            };
                            setNewCollection(updatedDraft);
                            bypassBackPromptRef.current = true;
                            navigation.dispatch(e.data.action);
                        },
                    },
                ]
            );
        });

        return unsubscribe;
    }, [navigation, newCollection, items, setNewCollection]);

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
                    <TouchableOpacity >
                        <ArrowUpDown size={28} color={theme.colors.onBackground} />
                    </TouchableOpacity>
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

                <TouchableOpacity style={styles.elevationButton} onPress={() => addPassageBottomSheet.current?.present()}>
                    <Text style={styles.p3}>Add Passage</Text>
                </TouchableOpacity>

                <View style={{height: 10}} />
                
                <ReorderableList
                    data={items}
                    keyExtractor={(item) => `${item.type}-${item.id}`}
                    renderItem={({item}) => {
                        if (item.type === 'passage')
                            return <NewCollectionPassage userPassage={item} itemId={item.id} />;
                        if (item.type === 'note')
                            return <CollectionNote note={item.note} />;
                        return null;
                    }}
                    onReorder={({from, to}) => {
                        const updated = reorderItems(items, from, to);
                        setItems(updated);
                        setNewCollectionItems(updated);
                    }}
                />

                <VisibilityBottomSheet 
                    ref={visibilityBottomSheet}
                    currentVisibility={newCollection.visibility}
                />
                <AddPassageBottomSheet
                    ref={addPassageBottomSheet}
                />
            </View>
    )
}