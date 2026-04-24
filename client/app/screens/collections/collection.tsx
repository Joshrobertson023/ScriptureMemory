import { RouteProp, useNavigation, useRoute } from "@react-navigation/native";
import { View, Text, StyleSheet } from "react-native";
import { useCollectionsStore } from "../../stores/collections.store";
import useAppTheme from "../../theme";
import useGlobalStyles from "../../styles/gobalStyles";
import { useEffect, useLayoutEffect, useMemo, useRef } from "react";
import { NativeStackNavigationProp } from "@react-navigation/native-stack";
import { RootStackParamList } from "../../../types/router";
import ReorderableList, { reorderItems } from "react-native-reorderable-list";
import NewCollectionPassage from "../../components/passage/newCollectionPassage";
import CollectionNote from "../../components/collection/newCollectionNote";
import Passage from "../../components/passage/passage";
import Note from "../../components/note/note";
import PassageComponent from "../../components/passage/passage";
import NoteComponent from "../../components/note/note";
import AddNoteBottomSheet from "../../components/bottom-sheets/addNoteBottomSheet";
import { TrueSheet } from "@lodev09/react-native-true-sheet";
import { useBottomSheetsStore } from "../../stores/bottomSheets.store";
import { CollectionItem } from "../../../types/collection/collectionItem";

const CollectionScreen = () => {
    const navigation = useNavigation<NativeStackNavigationProp<RootStackParamList>>();
    const route = useRoute<RouteProp<RootStackParamList, 'collection'>>();
    const id = route.params?.id;

    const collection = useCollectionsStore((state) =>
        state.userCollections.find((c) => c.id === id)
    );
    const noteSheet = useRef<TrueSheet>(null);
    const {noteSheetOpen, setNoteSheetOpen, noteBottomSheet} = useBottomSheetsStore();

    const { setCollectionItems, addNoteToCollection, updateNoteInCollection, removeNoteFromCollection } = useCollectionsStore();

    const theme = useAppTheme();
    const globalStyles = useGlobalStyles();
    const styles = () => useMemo(() => StyleSheet.create({

    }), [theme]);
    
    useLayoutEffect(() => {
        navigation.setOptions({
            headerTitle: collection?.title
        })
    });

    useEffect(() => {
        if (noteSheetOpen)
            noteSheet.current?.present();
        else
            noteSheet.current?.dismiss();
    }, [noteSheetOpen]);

    if (!collection) {
        navigation.goBack();
        console.error('collection not found for id:', id)
        return null;
    }

    return (
        <View style={globalStyles.screen}>
            <ReorderableList
                data={collection.items}
                keyExtractor={(item) => item.id.toString()}
                renderItem={({item}) => {
                        if (item.type === 'passage')
                            return <PassageComponent userPassage={item.passage} itemId={item.id} collectionId={collection.id} />;
                        if (item.type === 'note')
                            return <NoteComponent note={item.note} itemId={item.id} />;
                        return null;
                    }}
                onReorder={({from, to}) => {
                    const updated = reorderItems(collection.items, from, to);
                    setCollectionItems(collection.id, updated);
                }}
            />
            <AddNoteBottomSheet
                ref={noteSheet}
                onSave={(text: string, itemId: number | null) => {
                    if (itemId !== null) {
                        updateNoteInCollection(collection.id, itemId, text);
                    } else {
                        addNoteToCollection(collection.id, { id: 0, text });
                    }
                }}
                onDelete={(itemId: number | null) => {
                    if (itemId !== null)
                        removeNoteFromCollection(collection.id, itemId);
                }}
            />
        </View>
    )
}

export default CollectionScreen;