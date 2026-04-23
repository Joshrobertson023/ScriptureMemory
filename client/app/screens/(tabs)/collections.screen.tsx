import { Button, FlatList, Keyboard, Text, TouchableWithoutFeedback, View } from "react-native";
import { SafeAreaProvider, SafeAreaView } from "react-native-safe-area-context";
import useStyles from '../../styles/gobalStyles';
import { TrueSheet } from "@lodev09/react-native-true-sheet";
import { useRef, useState } from "react";
import CollectionsSortBottomSheet from "../../components/bottom-sheets/collectionsSortBottomSheet";
import { useUserStore } from "../../stores/user.store";
import { useCollectionsStore } from "../../stores/collections.store";
import { ReorderableCollectionCard } from "../../components/collection/collectionCard";
import SearchResult from "../../components/collection/searchResults";
import useGlobalStyles from "../../styles/gobalStyles";
import ReorderableList, { reorderItems } from "react-native-reorderable-list";

export const CollectionsScreen = () => {
    const globalStyles = useGlobalStyles();
    const {userCollections, setCollections} = useCollectionsStore();
    const [searchQuery, setSearchQuery] = useState('');

    return (
        <TouchableWithoutFeedback onPress={Keyboard.dismiss}>
            <View style={globalStyles.screen}>
                {searchQuery.length > 0 ? (
                    <SearchResult query={searchQuery} />
                ) : (
                    <>
                        <ReorderableList
                            onReorder={({from, to}) => {
                                const updated = reorderItems(userCollections, from, to);
                                setCollections(updated);
                            }}
                            data={userCollections}
                            keyExtractor={(col) => col.id.toString()}
                            renderItem={({ item }) => <ReorderableCollectionCard collection={item} />}
                            keyboardShouldPersistTaps="handled"
                            ListFooterComponent={<View style={{height: 100}} />}
                        />
                    </>
                )}
            </View>
        </TouchableWithoutFeedback>
    );
};