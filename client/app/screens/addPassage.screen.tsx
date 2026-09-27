import * as Clipboard from 'expo-clipboard';
import { SearchField, Spinner } from 'heroui-native';
import React, { useState } from "react";
import { View } from "react-native";
import { FlatList } from "react-native-gesture-handler";
import { CollectionItem } from "../../types/collection/collectionItem";
import { Passage } from "../../types/passages/passage";
import { Verse } from '../../types/verse/verse';
import { searchPassage } from "../api/verses.api";
import AddPassage from "../components/passage/addPassage";
import { useBibleVersion } from "../hooks/useBibleVersion";
import { useSearchStore } from "../stores/search.store";
import { useUserAuthStore } from "../stores/userAuth.store";
import useGlobalStyles from "../styles/gobalStyles";
import useAppTheme from "../theme";
import { showToast } from "../utils/toast";

interface AddPassageScreenProps {
    collectionItems: CollectionItem[];
    savePassage: (passage: Passage) => void;
    removePassage: (itemId: string) => void;
}

export const AddPassageScreen = ({ collectionItems, savePassage, removePassage }: AddPassageScreenProps) => {
    const styles = useGlobalStyles();
    const theme = useAppTheme();
    const [loadingSearch, setLoadingSearch] = useState(false);
    const { searchQuery, searchResults, setSearchQuery, setSearchResults, clearSearch, setLastVerse } = useSearchStore();
    const [search, setSearch] = useState(searchQuery);
    const [loading, setLoading] = useState(false);
    const [loadingMore, setLoadingMore] = useState(false);

    const jwt = useUserAuthStore((state) => state.jwt);
    const { version: bibleVersion } = useBibleVersion();

    const onEndReached = async() => {
        console.log('Fetching next page');
        
        if (search.trim() === '')
            return;

        let lastVerse: Verse | undefined = undefined;

        setLoadingMore(true);

        if (searchResults.length > 0) {
            lastVerse = searchResults.at(searchResults.length - 1)?.verses.at(0);
        }
        try {
            console.log('searching')
            setSearchQuery(search);
            console.log(lastVerse?.searchDistance);
            const nextPage = await searchPassage(
                search, 
                bibleVersion, 
                lastVerse?.searchDistance || 0.0,
                jwt);

            setSearchResults([...searchResults, ...nextPage]);
            lastVerse = nextPage.at(nextPage.length - 1)?.verses.at(0);
            console.log(lastVerse);
            if (lastVerse?.searchDistance !== undefined) {
                setLastVerse(lastVerse.searchDistance);
            } else {
                showToast({
                    type: 'danger',
                    title: 'Error encountered an error',
                    actionLabel: 'COPY ERROR',
                    onActionPress: async ({ hide }) => { await Clipboard.setStringAsync("lastVerse.distance was undefined."); hide(); }
                })
            }
        } catch (error: any) {
            console.error('error searching', error);

            const errorMessage =
                error?.message ||
                error?.toString?.() ||
                JSON.stringify(error) ||
                'Unknown error';

            showToast({
                type: 'danger',
                title: 'We encountered an error searching.',
                actionLabel: 'COPY ERROR',
                onActionPress: async ({ hide }) => { await Clipboard.setStringAsync(errorMessage); hide(); }
            })
        } finally {
            setLoadingMore(false);
        }
    }

    const handleSearch = async() => {
        if (search.trim() === '')
            return;

        let lastVerse: Verse | undefined = undefined;

        if (searchResults.length > 0) {
            lastVerse = searchResults.at(searchResults.length - 1)?.verses.at(0);
        }

        setLoadingSearch(true);
        try {
            console.log('searching')
            setSearchQuery(search);
            setSearchResults(await searchPassage(
                search, 
                bibleVersion, 
                lastVerse?.searchDistance || 0.0,
                jwt));
            lastVerse = searchResults.at(searchResults.length - 1)?.verses.at(0);
            if (lastVerse?.searchDistance !== undefined) {
                setLastVerse(lastVerse.searchDistance);
                console.log(lastVerse.searchDistance);
            } else {
                showToast({
                    type: 'danger',
                    title: 'Error encountered an error',
                    actionLabel: 'COPY ERROR',
                    onActionPress: async ({ hide }) => { await Clipboard.setStringAsync("lastVerse.distance was undefined."); hide(); }
                })
            }
        } catch (error: any) {
            console.error('error searching', error);

            const errorMessage =
                error?.message ||
                error?.toString?.() ||
                JSON.stringify(error) ||
                'Unknown error';

            showToast({
                type: 'danger',
                title: 'We encountered an error searching.',
                actionLabel: 'COPY ERROR',
                onActionPress: async ({ hide }) => { await Clipboard.setStringAsync(errorMessage); hide(); }
            })
        } finally {
            setLoadingSearch(false);
        }
    }

    return (
        <View style={{...styles.screen, paddingTop: 40}}>
            {/* <Searchbar
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
                /> */}

            <SearchField 
                value={search}
                onChange={(value) => {
                    setLoading(true);
                    setSearch(value);
                    setSearchQuery(value);
                    if (value.trim() === '') {
                        clearSearch();
                    }
                }}
                style={{width: '100%'}}
            >
                <SearchField.Group>
                    <SearchField.SearchIcon />
                    <SearchField.Input
                        returnKeyType="search"
                        onSubmitEditing={handleSearch}
                    />
                    {!loadingSearch &&
                            <SearchField.ClearButton />
                    }
                </SearchField.Group>
            </SearchField>

            {loadingSearch && 
            <>
                <Spinner style={{marginTop: 25}} />
            </>
                }

            {searchResults.length > 0 &&
                <FlatList
                    data={searchResults}
                    initialNumToRender={1}
                    maxToRenderPerBatch={2}
                    windowSize={3}
                    removeClippedSubviews={true}
                    keyExtractor={(item) => item.reference.readableReference}
                    ListFooterComponent={() => <Spinner />}
                    onEndReached={onEndReached}
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