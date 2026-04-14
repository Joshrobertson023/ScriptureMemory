import React, { useRef, useState } from "react"
import { Keyboard, TextInput, TouchableOpacity, View } from "react-native"
import Ionicons from '@expo/vector-icons/Ionicons';
import useAppTheme from "../../theme";
import { useNavigation } from "@react-navigation/native";
import { NativeStackNavigationProp } from "@react-navigation/native-stack";
import { RootStackParamList } from "../../../types/router";
import { TrueSheet } from "@lodev09/react-native-true-sheet";
import CollectionsSortBottomSheet from "../bottom-sheets/collectionsSortBottomSheet";
import { useUserStore } from "../../stores/user.store";
import { ArrowUpDown, CloudAlert, CloudCheck, CloudSync, Plus, Search } from "lucide-react-native";
import SyncBottomSheet from "../bottom-sheets/syncBottomSheet";
import useGlobalStyles from "../../styles/gobalStyles";

interface CollectionsPageHeaderProps {
    onSearch: (query: string) => void;
}

export const CollectionsPageHeader = ({ onSearch }: CollectionsPageHeaderProps) => {
    const [showSearch, setShowSearch] = useState(false);
    const [query, setQuery] = useState('');
    const theme = useAppTheme();
    const navigation = useNavigation<NativeStackNavigationProp<RootStackParamList>>();
    const sortSheet = useRef<TrueSheet>(null);
    const syncSheet = useRef<TrueSheet>(null);
    const collectionsSort = useUserStore().user.preferences.collectionsSort;
    const styles = useGlobalStyles();
    const syncSuccessful = false;
    const syncing = false;
    const syncStatus = 0;

    const handleQueryChange = (text: string) => {
        setQuery(text);
        onSearch(text);
    };

    const handleClose = () => {
        setShowSearch(false);
        setQuery('');
        onSearch('');
    };

    return (
        <>
            {showSearch ? (
                <View style={{ flexDirection: 'row', alignItems: 'center', justifyContent: 'center' }}>
                    <TextInput
                        style={[styles.input, { flex: 1 }]}
                        value={query}
                        onChangeText={handleQueryChange}
                        autoFocus
                        onBlur={() => Keyboard.dismiss()}
                    />
                    <TouchableOpacity onPress={handleClose}>
                        <Ionicons name="close" size={30} color={theme.colors.onBackground} />
                    </TouchableOpacity>
                </View>
            ) : (
                <View style={{ width: '100%', display: 'flex', justifyContent: 'space-between', flexDirection: 'row' }}>
                    <TouchableOpacity onPress={() => navigation.navigate('createCollection')}>
                        <Plus size={28} color={theme.colors.onBackground} />
                    </TouchableOpacity>
                    <View style={{ display: 'flex', flexDirection: 'row', justifyContent: 'center', alignItems: 'center', gap: 15 }}>
                        <TouchableOpacity onPress={() => { syncSheet.current?.present() }}>
                            {syncSuccessful ? (
                                <CloudCheck size={28} color={theme.colors.onBackground} />
                            ) : syncing ? (
                                <CloudSync size={28} color={theme.colors.onBackground} />
                            ) : (
                                <CloudAlert size={28} color={theme.colors.onBackground} />
                            )}
                        </TouchableOpacity>
                        <TouchableOpacity onPress={() => { setShowSearch(true) }}>
                            <Search size={28} color={theme.colors.onBackground} />
                        </TouchableOpacity>
                        <TouchableOpacity onPress={() => { sortSheet.current?.present() }}>
                            <ArrowUpDown size={28} color={theme.colors.onBackground} />
                        </TouchableOpacity>
                    </View>
                </View>
            )}

            <SyncBottomSheet ref={syncSheet} syncStatus={syncStatus} />
            <CollectionsSortBottomSheet ref={sortSheet} currentOrder={collectionsSort} />
        </>
    )
}