import { TrueSheet } from "@lodev09/react-native-true-sheet";
import { RouteProp, useNavigation, useRoute } from "@react-navigation/native";
import { NativeStackNavigationProp } from "@react-navigation/native-stack";
import { ArrowUpDown, EllipsisVertical } from "lucide-react-native";
import React, { useCallback, useEffect, useLayoutEffect, useMemo, useRef, useState } from "react";
import { StyleSheet, Text, TouchableOpacity, View } from "react-native";
import ReorderableList, { reorderItems } from "react-native-reorderable-list";
import { CollectionItem } from "../../../types/collection/collectionItem";
import { RootStackParamList } from "../../../types/router";
import AddNoteBottomSheet from "../../components/bottom-sheets/addNoteBottomSheet";
import NoteComponent from "../../components/note/note";
import PassageComponent from "../../components/passage/passage";
import {
    addNoteToCollection,
    removeNoteFromCollection,
    reorderItems as reorderCollectionItems,
    updateNoteInCollection,
} from "../../database/repositories/collections.repository";
import { useCollection } from "../../hooks/useCollections";
import { useBottomSheetsStore } from "../../stores/bottomSheets.store";
import useGlobalStyles from "../../styles/gobalStyles";
import useAppTheme from "../../theme";

const CollectionScreen = () => {
    const navigation = useNavigation<NativeStackNavigationProp<RootStackParamList>>();
    const route = useRoute<RouteProp<RootStackParamList, 'collection'>>();
    const id = route.params?.id;

    const { collection, isLoading } = useCollection(id);
    const noteSheet = useRef<TrueSheet>(null);
    const noteSheetOpen = useBottomSheetsStore((state) => state.noteSheetOpen);
    const setNoteSheetOpen = useBottomSheetsStore((state) => state.setNoteSheetOpen);
    const setCollectionMenuBottomSheet = useBottomSheetsStore((state) => state.setCollectionMenuBottomSheet);
    const setCollectionMenuSheetOpen = useBottomSheetsStore((state) => state.setCollectionMenuSheetOpen);

    const theme = useAppTheme();
    const globalStyles = useGlobalStyles();
    const styles = () => useMemo(() => StyleSheet.create({

    }), [theme]);

    const [reordering, setReordering] = useState(false);

    useLayoutEffect(() => {
        navigation.setOptions({
            headerTitle: collection?.title,
            headerRight: () => (
                <View style={{ flexDirection: 'row', alignItems: 'center', gap: 18 }}>
                    <TouchableOpacity hitSlop={8} onPress={() => setReordering((r) => !r)}>
                        {reordering ? (
                            <Text style={globalStyles.p2}>Done</Text>
                        ) : (
                            <ArrowUpDown size={22} color={theme.colors.onBackground} />
                        )}
                    </TouchableOpacity>
                    <TouchableOpacity
                        hitSlop={{ top: 8, bottom: 8, left: 8, right: 8 }}
                        onPress={() => {
                            if (!collection) return;
                            setCollectionMenuBottomSheet(collection);
                            setCollectionMenuSheetOpen(true);
                        }}
                    >
                        <EllipsisVertical size={22} color={theme.colors.onBackground} />
                    </TouchableOpacity>
                </View>
            )
        })
    }, [collection, reordering]);

    useEffect(() => {
        if (noteSheetOpen)
            noteSheet.current?.present();
        else
            noteSheet.current?.dismiss();
    }, [noteSheetOpen]);

    const collectionId = collection?.id ?? '';
    const renderItem = useCallback(({item}: {item: CollectionItem}) => {
        if (item.type === 'passage')
            return <PassageComponent userPassage={item.passage} itemId={item.id} collectionId={collectionId} reordering={reordering} />;
        if (item.type === 'note')
            return <NoteComponent note={item.note} itemId={item.id} reordering={reordering} />;
        return null;
    }, [collectionId, reordering]);

    if (isLoading) {
        return null;
    }

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
                renderItem={renderItem}
                extraData={reordering}
                dragEnabled={reordering}
                onReorder={({from, to}) => {
                    const updated = reorderItems(collection.items, from, to);
                    reorderCollectionItems(collection.id, updated);
                }}
            />
            <AddNoteBottomSheet
                ref={noteSheet}
                onSave={(text: string, itemId: string | null) => {
                    if (itemId !== null) {
                        updateNoteInCollection(collection.id, itemId, text);
                    } else {
                        addNoteToCollection(collection.id, { text });
                    }
                }}
                onDelete={(itemId: string | null) => {
                    if (itemId !== null)
                        removeNoteFromCollection(collection.id, itemId);
                }}
            />
        </View>
    )
}

export default CollectionScreen;