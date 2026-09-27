import { useNavigation } from "@react-navigation/native";
import { NativeStackNavigationProp } from "@react-navigation/native-stack";
import { Button, Separator } from 'heroui-native';
import { ArrowUpDown, Library, Plus } from "lucide-react-native";
import React, { useCallback, useLayoutEffect, useMemo, useState } from "react";
import { Keyboard, StyleSheet, Text, TouchableOpacity, TouchableWithoutFeedback, View } from "react-native";
import ReorderableList, { reorderItems } from "react-native-reorderable-list";
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { Collection } from "../../../types/collection/collection";
import { RootStackParamList } from "../../../types/router";
import { ReorderableCollectionCard } from "../../components/collection/collectionCard";
import SearchResult from "../../components/collection/searchResults";
import { VerseOfDayHomeCard } from "../../components/home/vod";
import { reorderCollections } from "../../database/repositories/collections.repository";
import { useArchivedCollections, useUserCollections } from "../../hooks/useCollections";
import useGlobalStyles from "../../styles/gobalStyles";
import useAppTheme from "../../theme";

type CollectionsTab = 'all' | 'archived';

const emptyStateCopy = (tab: CollectionsTab) => {
    switch (tab) {
        case 'archived':
            return { title: 'No archived collections', subtitle: 'No archived collections' };
        default:
            return { title: 'No collections yet', subtitle: 'No collections yet, try creating one by pressing "+".' };
    }
};

export const CollectionsScreen = () => {
    const globalStyles = useGlobalStyles();
    const theme = useAppTheme();
    const insets = useSafeAreaInsets();
    const navigation = useNavigation<NativeStackNavigationProp<RootStackParamList>>();
    const userCollections = useUserCollections();
    const archivedCollections = useArchivedCollections();
    const [searchQuery, setSearchQuery] = useState('');
    const [activeTab, setActiveTab] = useState<CollectionsTab>('all');
    const [reordering, setReordering] = useState(false);

    const displayedCollections: Collection[] = useMemo(() => {
        switch (activeTab) {
            case 'archived':
                return archivedCollections;
            default:
                return userCollections;
        }
    }, [activeTab, userCollections, archivedCollections]);

    const styles = useMemo(() => StyleSheet.create({
        tabs: {
            marginTop: 10,
        },
        fab: {
            position: 'absolute',
            bottom: insets.bottom + 95,
            right: 15,
            shadowColor: '#000',
            shadowOffset: { width: 0, height: 2 },
            shadowOpacity: 0.15,
            shadowRadius: 6,
            elevation: 4,
        },
        emptyState: {
            flex: 1,
            width: '100%',
            justifyContent: 'center',
            alignItems: 'center',
            gap: 6,
            paddingHorizontal: 30,
        },
        emptyStateTitle: {
            ...globalStyles.p1,
            color: theme.colors.onBackground,
            fontWeight: '700',
        },
        emptyStateSubtitle: {
            ...globalStyles.p3,
            textAlign: 'center',
            marginBottom: 14,
        },
    }), [globalStyles, theme]);

    const emptyCopy = emptyStateCopy(activeTab);

    useLayoutEffect(() => {
        navigation.setOptions({
            headerSearchBarOptions: {
                placeholder: "Search Collections...",
                onChangeText: (event) => setSearchQuery(event.nativeEvent.text),
            },
            headerRight: () => (
                <TouchableOpacity hitSlop={8} style={{ marginRight: 15 }} onPress={() => setReordering((r) => !r)}>
                    {reordering ? (
                        <Text style={globalStyles.p2}>Done</Text>
                    ) : (
                        <ArrowUpDown size={22} color={theme.colors.onBackground} />
                    )}
                </TouchableOpacity>
            ),
        });
    }, [navigation, reordering, globalStyles, theme]);

    const renderCollection = useCallback(({ item }: { item: Collection }) => (
        <ReorderableCollectionCard collection={item} reordering={reordering} />
    ), [reordering]);

    const keyExtractor = useCallback((col: Collection) => col.id.toString(), []);

    return (
        <TouchableWithoutFeedback onPress={Keyboard.dismiss}>
            <>
                {searchQuery.length > 0 ? (
                    <SearchResult query={searchQuery} collections={displayedCollections} />
                ) : (
                    <>
                        {displayedCollections.length === 0 ? (
                            <View style={styles.emptyState}>
                                <Library size={48} color={theme.colors.onBackgroundSoft} strokeWidth={1.3} />
                                <Text style={styles.emptyStateSubtitle}>{emptyCopy.subtitle}</Text>
                            </View>
                        ) : (
                            <ReorderableList
                                onReorder={({from, to}) => {
                                    if (activeTab !== 'all') 
                                        return;
                                    const updated = reorderItems(displayedCollections, from, to);
                                    reorderCollections(updated.map((c) => c.id));
                                }}
                                data={displayedCollections}
                                keyExtractor={keyExtractor}
                                renderItem={renderCollection}
                                extraData={reordering}
                                dragEnabled={reordering}
                                keyboardShouldPersistTaps="handled"
                                ListHeaderComponent={
                                    <View style={globalStyles.screen}>
                                        <VerseOfDayHomeCard/>

                                        <View style={{height: 10}} />
                                        
                                        <Separator />
                                    </View>
                                }
                                ListFooterComponent={<View style={{height: 100}} />}
                            />
                        )}
                    </>
                )}

                <View style={styles.fab}>
                    <Button
                        isIconOnly
                        variant="primary"
                        size="lg"
                        className="rounded-full"
                        onPress={() => navigation.navigate('createCollection')}
                    >
                        <Plus size={26} color={theme.colors.white} />
                    </Button>
                </View>
            </>
        </TouchableWithoutFeedback>
    );
};
