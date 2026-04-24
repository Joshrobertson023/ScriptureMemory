import { ActivityIndicator, View } from "react-native"
import useGlobalStyles from "../styles/gobalStyles"
import { useEffect, useState } from "react";
import useAppTheme from "../theme";
import { FlatList } from "react-native-gesture-handler";
import { searchPassage } from "../api/verses.api";
import { Snackbar } from "react-native-snackbar";
import * as Clipboard from 'expo-clipboard';
import AddPassage from "../components/passage/addPassage";
import { useUserStore } from "../stores/user.store";
import { useUserAuthStore } from "../stores/userAuth.store";
import { Searchbar } from "react-native-paper";
import { useSearchStore } from "../stores/search.store";
import { CollectionItem } from "../../types/collection/collectionItem";
import { Passage } from "../../types/passages/passage";

interface AddPassageScreenProps {
    collectionItems: CollectionItem[];
    savePassage: (passage: Passage) => void;
    removePassage: (itemId: number) => void;
}

export const AddPassageScreen = ({ collectionItems, savePassage, removePassage }: AddPassageScreenProps) => {
    const styles = useGlobalStyles();
    const theme = useAppTheme();
    const [loadingSearch, setLoadingSearch] = useState(false);
    const { searchQuery, searchResults, setSearchQuery, setSearchResults, clearSearch } = useSearchStore();
    const [search, setSearch] = useState(searchQuery);  

    const userId = useUserStore().user.id;
    const jwt = useUserAuthStore().jwt;

    const handleSearch = async () => {
        if (search.trim() === '')
            return;

        setLoadingSearch(true);
        try {
            console.log('searching')
            setSearchQuery(search);
            setSearchResults(await searchPassage(search, userId, jwt));
        } catch (error: any) {
            console.error('error searching', error);

            const errorMessage =
                error?.message ||
                error?.toString?.() ||
                JSON.stringify(error) ||
                'Unknown error';

            Snackbar.show({
                text: 'We encountered an error',
                duration: Snackbar.LENGTH_LONG,
                action: {
                    text: 'COPY ERROR',
                    textColor: theme.colors.onBackground,
                    onPress: async () => {await Clipboard.setStringAsync(errorMessage)}
                }
            })
        } finally {
            setLoadingSearch(false);
        }
    }

    return (
        <View style={{...styles.screen, paddingTop: 40}}>
            <Searchbar
                placeholder="Search the Bible"
                onChangeText={(value) => {
                    setSearch(value);
                    setSearchQuery(value);
                    if (value.trim() === '') {
                        clearSearch();
                    }
                }}
                value={search} 
                loading={loadingSearch}
                style={styles.search}
                inputStyle={styles.p3}
                iconColor={theme.colors.onBackgroundSoft}
                placeholderTextColor={theme.colors.onBackgroundSoft}
                onIconPress={handleSearch}
                onClearIconPress={() => {
                    clearSearch();
                }}
                onSubmitEditing={handleSearch}
                />

            {searchResults.length > 0 &&
                <FlatList
                    data={searchResults}
                    initialNumToRender={1}
                    maxToRenderPerBatch={2}
                    windowSize={3}
                    removeClippedSubviews={true}
                    keyExtractor={(item) => item.reference.readableReference}
                    renderItem={({item}) => {
                        const savedPassageItem = collectionItems.find(
                            (i) => i.type === 'passage' && i.passage.passage.reference.readableReference === item.reference.readableReference
                        );
                        return (
                            <AddPassage
                                passage={item}
                                savedItemId={savedPassageItem?.id ?? null}
                                savePassage={savePassage}
                                removePassage={removePassage}
                            />
                        );
                    }}
                />
            }

        </View>
    )
}