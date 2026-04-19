import { Button, FlatList, Keyboard, Text, TouchableWithoutFeedback, View } from "react-native";
import { SafeAreaProvider, SafeAreaView } from "react-native-safe-area-context";
import useStyles from '../../styles/gobalStyles';
import { CollectionsPageHeader } from "../../components/collection/header";
import { TrueSheet } from "@lodev09/react-native-true-sheet";
import { useRef, useState } from "react";
import CollectionsSortBottomSheet from "../../components/bottom-sheets/collectionsSortBottomSheet";
import { useUserStore } from "../../stores/user.store";
import { useCollectionsStore } from "../../stores/collections.store";
import { CollectionCard } from "../../components/collection/collectionCard";
import SearchResult from "../../components/collection/searchResults";
import useGlobalStyles from "../../styles/gobalStyles";

export const CollectionsScreen = () => {
    const globalStyles = useGlobalStyles();
    const userCollections = useCollectionsStore().userCollections;
    const [searchQuery, setSearchQuery] = useState('');

    return (
        <TouchableWithoutFeedback onPress={Keyboard.dismiss}>
            <View style={globalStyles.screen}>
                <CollectionsPageHeader />
                <View style={globalStyles.collectionCardsContainer}>
                    {searchQuery.length > 0 ? (
                        <SearchResult query={searchQuery} />
                    ) : (
                        <FlatList
                            data={userCollections}
                            keyExtractor={(col) => col.id.toString()}
                            renderItem={({ item }) => <CollectionCard collection={item} />}
                            keyboardShouldPersistTaps="handled"
                        />
                    )}
                </View>
            </View>
        </TouchableWithoutFeedback>
    );
};